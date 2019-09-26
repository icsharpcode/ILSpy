// Copyright (c) 2017 Siegfried Pammer
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Converts LINQ Expression Trees to ILFunctions/ILAst instructions.
	/// </summary>
	public class TransformExpressionTrees : IStatementTransform
	{
		/// <summary>
		/// Returns true if the instruction matches the pattern for Expression.Lambda calls.
		/// </summary>
		static bool MightBeExpressionTree(ILInstruction inst, ILInstruction stmt)
		{
			if (!(inst is CallInstruction call
				&& call.Method.FullNameIs("System.Linq.Expressions.Expression", "Lambda")
				&& call.Arguments.Count == 2))
				return false;
			if (!(IsEmptyParameterList(call.Arguments[1]) || (call.Arguments[1] is Block block && block.Kind == BlockKind.ArrayInitializer)))
				return false;
			//if (!ILInlining.CanUninline(call, stmt))
			//	return false;
			return true;
		}

		static bool IsEmptyParameterList(ILInstruction inst)
		{
			if (inst is CallInstruction emptyCall && emptyCall.Method.FullNameIs("System.Array", "Empty") && emptyCall.Arguments.Count == 0)
				return true;
			if (inst.MatchNewArr(out var type) && type.FullName == "System.Linq.Expressions.ParameterExpression")
				return true;
			if (inst.MatchNewArr(out type) && type.FullName == "System.Linq.Expressions.Expression")
				return true;
			return false;
		}

		bool MatchParameterVariableAssignment(ILInstruction expr, out ILVariable parameterReferenceVar, out IType type, out string name)
		{
			// stloc(v, call(Expression::Parameter, call(Type::GetTypeFromHandle, ldtoken(...)), ldstr(...)))
			type = null;
			name = null;
			if (!expr.MatchStLoc(out parameterReferenceVar, out var init))
				return false;
			if (!parameterReferenceVar.IsSingleDefinition)
				return false;
			if (!(parameterReferenceVar.Kind == VariableKind.Local || parameterReferenceVar.Kind == VariableKind.StackSlot))
				return false;
			if (parameterReferenceVar.Type == null || parameterReferenceVar.Type.FullName != "System.Linq.Expressions.ParameterExpression")
				return false;
			if (!(init is CallInstruction initCall && initCall.Arguments.Count == 2))
				return false;
			if (!(initCall.Method.FullNameIs("System.Linq.Expressions.Expression", "Parameter")))
				return false;
			CallInstruction typeArg = initCall.Arguments[0] as CallInstruction;
			if (typeArg == null || typeArg.Arguments.Count != 1)
				return false;
			if (!typeArg.Method.FullNameIs("System.Type", "GetTypeFromHandle"))
				return false;
			return typeArg.Arguments[0].MatchLdTypeToken(out type) && initCall.Arguments[1].MatchLdStr(out name);
		}

		StatementTransformContext context;
		Dictionary<ILVariable, (IType, string)> parameters;
		Dictionary<ILVariable, ILVariable> parameterMapping;
		List<ILInstruction> instructionsToRemove;
		Stack<ILFunction> lambdaStack;
		CSharpConversions conversions;
		CSharpResolver resolver;

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.ExpressionTrees) return;
			this.context = context;
			this.conversions = CSharpConversions.Get(context.TypeSystem);
			this.resolver = new CSharpResolver(context.TypeSystem);
			this.parameters = new Dictionary<ILVariable, (IType, string)>();
			this.parameterMapping = new Dictionary<ILVariable, ILVariable>();
			this.instructionsToRemove = new List<ILInstruction>();
			this.lambdaStack = new Stack<ILFunction>();
			for (int i = pos; i < block.Instructions.Count; i++) {
				if (MatchParameterVariableAssignment(block.Instructions[i], out var v, out var type, out var name)) {
					parameters.Add(v, (type, name));
					continue;
				}
				if (TryConvertExpressionTree(block.Instructions[i], block.Instructions[i])) {
					foreach (var inst in instructionsToRemove)
						block.Instructions.Remove(inst);
					instructionsToRemove.Clear();
				}
				break;
			}
		}

		bool TryConvertExpressionTree(ILInstruction instruction, ILInstruction statement)
		{
			if (MightBeExpressionTree(instruction, statement)) {
				var (lambda, type) = ConvertLambda((CallInstruction)instruction);
				if (lambda != null) {
					SetExpressionTreeFlag((ILFunction)lambda, (CallInstruction)instruction);
					context.Step("Convert Expression Tree", instruction);
					instruction.ReplaceWith(lambda);
					return true;
				}
				return false;
			}
			if (instruction is Block block && block.Kind == BlockKind.ControlFlow)
				return false;  // don't look into nested blocks
			foreach (var child in instruction.Children) {
				if (TryConvertExpressionTree(child, statement))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Converts a Expression.Lambda call into an ILFunction.
		/// If the conversion fails, null is returned.
		/// </summary>
		(ILInstruction, IType) ConvertLambda(CallInstruction instruction)
		{
			if (instruction.Method.Name != "Lambda" || instruction.Arguments.Count != 2 || instruction.Method.ReturnType.FullName != "System.Linq.Expressions.Expression" || instruction.Method.ReturnType.TypeArguments.Count != 1)
				return (null, SpecialType.UnknownType);
			var parameterList = new List<IParameter>();
			var parameterVariablesList = new List<ILVariable>();
			if (!ReadParameters(instruction.Arguments[1], parameterList, parameterVariablesList, new SimpleTypeResolveContext(context.Function.Method)))
				return (null, SpecialType.UnknownType);
			var container = new BlockContainer();
			container.AddILRange(instruction);
			var functionType = instruction.Method.ReturnType.TypeArguments[0];
			var returnType = functionType.GetDelegateInvokeMethod()?.ReturnType;
			var function = new ILFunction(returnType, parameterList, context.Function.GenericContext, container);
			function.DelegateType = functionType;
			function.Kind = IsExpressionTree(functionType) ? ILFunctionKind.ExpressionTree : ILFunctionKind.Delegate;
			function.Variables.AddRange(parameterVariablesList);
			function.AddILRange(instruction);
			lambdaStack.Push(function);
			var (bodyInstruction, type) = ConvertInstruction(instruction.Arguments[0]);
			lambdaStack.Pop();
			if (bodyInstruction == null)
				return (null, SpecialType.UnknownType);
			container.ExpectedResultType = bodyInstruction.ResultType;
			container.Blocks.Add(new Block() { Instructions = { new Leave(container, bodyInstruction) } });
			// Replace all other usages of the parameter variable
			foreach (var mapping in parameterMapping) {
				foreach (var load in mapping.Key.LoadInstructions.ToArray()) {
					if (load.IsDescendantOf(instruction))
						continue;
					load.ReplaceWith(new LdLoc(mapping.Value));
				}
			}
			return (function, function.DelegateType);
		}

		(ILInstruction, IType) ConvertQuote(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 1)
				return (null, SpecialType.UnknownType);
			var argument = invocation.Arguments.Single();
			if (argument is ILFunction function) {
				return (function, function.DelegateType);
			} else {
				var converted = ConvertInstruction(argument);

				if (converted.Item1 is ILFunction lambda && argument is CallInstruction call) {
					SetExpressionTreeFlag(lambda, call);
				}

				return converted;
			}
		}

		void SetExpressionTreeFlag(ILFunction lambda, CallInstruction call)
		{
			lambda.Kind = IsExpressionTree(call.Method.ReturnType) ? ILFunctionKind.ExpressionTree : ILFunctionKind.Delegate;
			lambda.DelegateType = call.Method.ReturnType;
		}

		bool ReadParameters(ILInstruction initializer, IList<IParameter> parameters, IList<ILVariable> parameterVariables, ITypeResolveContext resolveContext)
		{
			switch (initializer) {
				case Block initializerBlock:
					if (initializerBlock.Kind != BlockKind.ArrayInitializer)
						return false;
					int i = 0;
					foreach (var inst in initializerBlock.Instructions.OfType<StObj>()) {
						if (i >= this.parameters.Count)
							return false;
						if (!inst.Value.MatchLdLoc(out var v))
							return false;
						if (!this.parameters.TryGetValue(v, out var value))
							return false;
						// Parameter variable cannot be used in two different compiler-generated expression trees,
						// therefore we have to abort if the parameter is already mapped to a variable.
						if (this.parameterMapping.ContainsKey(v))
							return false;
						var param = new ILVariable(VariableKind.Parameter, value.Item1, i) { Name = value.Item2 };
						parameterMapping.Add(v, param);
						parameterVariables.Add(param);
						parameters.Add(new DefaultParameter(value.Item1, value.Item2));
						instructionsToRemove.Add((ILInstruction)v.StoreInstructions[0]);
						i++;
					}
					return true;
				default:
					return IsEmptyParameterList(initializer);
			}
		}

		(ILInstruction, IType) ConvertInstruction(ILInstruction instruction, IType typeHint = null)
		{
			var result = Convert();
			if (result.Item1 != null) {
				Debug.Assert(result.Item2 != null, "IType must be non-null!");
				Debug.Assert(result.Item1.ResultType == result.Item2.GetStackType(), "StackTypes must match!");
				if (typeHint != null) {
					var inst = result.Item1;
					if (inst.ResultType != typeHint.GetStackType()) {
						return (new Conv(inst, typeHint.GetStackType().ToPrimitiveType(), false, typeHint.GetSign()), typeHint);
					}
				}
			}
			return result;

			(ILInstruction, IType) Convert() {
				switch (instruction) {
					case CallInstruction invocation:
						if (invocation.Method.DeclaringType.FullName != "System.Linq.Expressions.Expression")
							return (null, SpecialType.UnknownType);

						switch (invocation.Method.Name) {
							case "Add":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Add, false);
							case "AddChecked":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Add, true);
							case "And":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.BitAnd);
							case "AndAlso":
								return ConvertLogicOperator(invocation, true);
							case "ArrayAccess":
							case "ArrayIndex":
								return ConvertArrayIndex(invocation);
							case "ArrayLength":
								return ConvertArrayLength(invocation);
							case "Call":
								return ConvertCall(invocation);
							case "Coalesce":
								return ConvertCoalesce(invocation);
							case "Condition":
								return ConvertCondition(invocation);
							case "Constant":
								return ConvertConstant(invocation);
							case "Convert":
								return ConvertCast(invocation, false);
							case "ConvertChecked":
								return ConvertCast(invocation, true);
							case "Divide":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Div);
							case "Equal":
								return ConvertComparison(invocation, ComparisonKind.Equality);
							case "ExclusiveOr":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.BitXor);
							case "Field":
								return ConvertField(invocation, typeHint);
							case "GreaterThan":
								return ConvertComparison(invocation, ComparisonKind.GreaterThan);
							case "GreaterThanOrEqual":
								return ConvertComparison(invocation,  ComparisonKind.GreaterThanOrEqual);
							case "Invoke":
								return ConvertInvoke(invocation);
							case "Lambda":
								return ConvertLambda(invocation);
							case "LeftShift":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.ShiftLeft);
							case "LessThan":
								return ConvertComparison(invocation, ComparisonKind.LessThan);
							case "LessThanOrEqual":
								return ConvertComparison(invocation, ComparisonKind.LessThanOrEqual);
							case "ListInit":
								return ConvertListInit(invocation);
							case "MemberInit":
								return ConvertMemberInit(invocation);
							case "Modulo":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Rem);
							case "Multiply":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Mul, false);
							case "MultiplyChecked":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Mul, true);
							case "Negate":
								return ConvertUnaryNumericOperator(invocation, BinaryNumericOperator.Sub, false);
							case "NegateChecked":
								return ConvertUnaryNumericOperator(invocation, BinaryNumericOperator.Sub, true);
							case "New":
								return ConvertNewObject(invocation);
							case "NewArrayBounds":
								return ConvertNewArrayBounds(invocation);
							case "NewArrayInit":
								return ConvertNewArrayInit(invocation);
							case "Not":
								return ConvertNotOperator(invocation);
							case "NotEqual":
								return ConvertComparison(invocation, ComparisonKind.Inequality);
							case "OnesComplement":
								return ConvertNotOperator(invocation);
							case "Or":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.BitOr);
							case "OrElse":
								return ConvertLogicOperator(invocation, false);
							case "Property":
								return ConvertProperty(invocation);
							case "Quote":
								return ConvertQuote(invocation);
							case "RightShift":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.ShiftRight);
							case "Subtract":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Sub, false);
							case "SubtractChecked":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Sub, true);
							case "TypeAs":
								return ConvertTypeAs(invocation);
							case "TypeIs":
								return ConvertTypeIs(invocation);
						}
						return (null, SpecialType.UnknownType);
					case ILFunction function:
						if (function.Kind == ILFunctionKind.ExpressionTree) {
							function.DelegateType = UnwrapExpressionTree(function.DelegateType);
							function.Kind = ILFunctionKind.Delegate;
						}
						return (function, function.DelegateType);
					case LdLoc ldloc:
						if (IsExpressionTreeParameter(ldloc.Variable)) {
							// Replace an already mapped parameter with the actual ILVariable,
							// we generated earlier.
							if (parameterMapping.TryGetValue(ldloc.Variable, out var v)) {
								if (typeHint.SkipModifiers() is ByReferenceType && !v.Type.IsByRefLike)
									return (new LdLoca(v), typeHint);
								return (new LdLoc(v), v.Type);
							}
							// This is a parameter variable from an outer scope.
							// We can't replace these variables just yet, because the transform works backwards.
							// We simply return the same instruction again, but return the actual expected type,
							// so our transform can continue normally.
							// Later, we will replace all references to unmapped variables,
							// with references to mapped parameters.
							if (ldloc.Variable.IsSingleDefinition && ldloc.Variable.StoreInstructions[0] is ILInstruction inst) {
								if (MatchParameterVariableAssignment(inst, out _, out var type, out _))
									return (new ExpressionTreeCast(type, ldloc, false), type);
							}
						}
						return (null, SpecialType.UnknownType);
					default:
						return (null, SpecialType.UnknownType);
				}
			}
		}

		bool IsExpressionTree(IType delegateType) => delegateType is ParameterizedType pt
			&& pt.FullName == "System.Linq.Expressions.Expression"
			&& pt.TypeArguments.Count == 1;

		IType UnwrapExpressionTree(IType delegateType)
		{
			if (delegateType is ParameterizedType pt && pt.FullName == "System.Linq.Expressions.Expression" && pt.TypeArguments.Count == 1) {
				return pt.TypeArguments[0];
			}
			return delegateType;
		}

		(ILInstruction, IType) ConvertArrayIndex(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			var (array, arrayType) = ConvertInstruction(invocation.Arguments[0]);
			if (array == null)
				return (null, SpecialType.UnknownType);
			if (!(arrayType is ArrayType type))
				return (null, SpecialType.UnknownType);
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				arguments = new[] { invocation.Arguments[1] };
			for (int i = 0; i < arguments.Count; i++) {
				var (converted, indexType) = ConvertInstruction(arguments[i]);
				if (converted == null)
					return (null, SpecialType.UnknownType);
				arguments[i] = converted;
			}
			return (new LdObj(new LdElema(type.ElementType, array, arguments.ToArray()), type.ElementType), type.ElementType);
		}

		(ILInstruction, IType) ConvertArrayLength(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 1)
				return (null, SpecialType.UnknownType);
			var (converted, arrayType) = ConvertInstruction(invocation.Arguments[0]);
			if (converted == null)
				return (null, SpecialType.UnknownType);
			return (new LdLen(StackType.I4, converted), context.TypeSystem.FindType(KnownTypeCode.Int32));
		}

		(ILInstruction, IType) ConvertBinaryNumericOperator(CallInstruction invocation, BinaryNumericOperator op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 2)
				return (null, SpecialType.UnknownType);
			var (left, leftType) = ConvertInstruction(invocation.Arguments[0]);
			if (left == null)
				return (null, SpecialType.UnknownType);
			var (right, rightType) = ConvertInstruction(invocation.Arguments[1]);
			if (right == null)
				return (null, SpecialType.UnknownType);
			IMember method;
			switch (invocation.Arguments.Count) {
				case 2:
					if (op == BinaryNumericOperator.ShiftLeft || op == BinaryNumericOperator.ShiftRight) {
						if (!rightType.IsKnownType(KnownTypeCode.Int32))
							return (null, SpecialType.UnknownType);
					} else {
						if (!rightType.Equals(leftType))
							return (null, SpecialType.UnknownType);
					}
					return (new BinaryNumericInstruction(op, left, right, isChecked == true, leftType.GetSign()), leftType);
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return (null, SpecialType.UnknownType);
					return (new Call((IMethod)method) {
						Arguments = { left, right }
					}, method.ReturnType);
				case 4:
					if (!invocation.Arguments[2].MatchLdcI4(out var isLifted))
						return (null, SpecialType.UnknownType);
					if (!MatchGetMethodFromHandle(invocation.Arguments[3], out method))
						return (null, SpecialType.UnknownType);
					if (isLifted != 0)
						method = CSharpOperators.LiftUserDefinedOperator((IMethod)method);
					return (new Call((IMethod)method) {
						Arguments = { left, right }
					}, method.ReturnType);
				default:
					return (null, SpecialType.UnknownType);
			}
		}

		(ILInstruction, IType) ConvertBind(CallInstruction invocation, ILVariable targetVariable)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			var (value, typeValue) = ConvertInstruction(invocation.Arguments[1]);
			if (value == null)
				return (null, SpecialType.UnknownType);
			if (MatchGetMethodFromHandle(invocation.Arguments[0], out var member)) {
			} else if (MatchGetFieldFromHandle(invocation.Arguments[0], out member)) {
			} else {
				return (null, SpecialType.UnknownType);
			}
			switch (member) {
				case IMethod method:
					return (new Call(method) { Arguments = { new LdLoc(targetVariable), value } }, method.ReturnType);
				case IField field:
					return (new StObj(new LdFlda(new LdLoc(targetVariable), (IField)member), value, member.ReturnType), field.ReturnType);
			}
			return (null, SpecialType.UnknownType);
		}

		(ILInstruction, IType) ConvertCall(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return (null, SpecialType.UnknownType);
			IList<ILInstruction> arguments = null;
			ILInstruction target = null;
			IType targetType = null;
			if (MatchGetMethodFromHandle(invocation.Arguments[0], out var member)) {
				// static method
				if (invocation.Arguments.Count != 2 || !MatchArgumentList(invocation.Arguments[1], out arguments)) {
					arguments = new List<ILInstruction>(invocation.Arguments.Skip(1));
				}
			} else if (MatchGetMethodFromHandle(invocation.Arguments[1], out member)) {
				if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments)) {
					arguments = new List<ILInstruction>(invocation.Arguments.Skip(2));
				}
				if (!invocation.Arguments[0].MatchLdNull()) {
					(target, targetType) = ConvertInstruction(invocation.Arguments[0]);
					if (target == null)
						return (null, SpecialType.UnknownType);
				}
			}
			if (arguments == null)
				return (null, SpecialType.UnknownType);
			IMethod method = (IMethod)member;
			if (!ConvertCallArguments(arguments, method))
				return (null, SpecialType.UnknownType);
			if (method.FullName == "System.Reflection.MethodInfo.CreateDelegate" && method.Parameters.Count == 2) {
				if (!MatchGetMethodFromHandle(target, out var targetMethod))
					return (null, SpecialType.UnknownType);
				if (!MatchGetTypeFromHandle(arguments[0], out var delegateType))
					return (null, SpecialType.UnknownType);
				return (new NewObj(delegateType.GetConstructors().Single()) {
					Arguments = { arguments[1], new LdFtn((IMethod)targetMethod) }
				}, delegateType);
			}
			CallInstruction call;
			if (method.IsAbstract || method.IsVirtual || method.IsOverride) {
				call = new CallVirt(method);
			} else {
				call = new Call(method);
			}
			if (target != null) {
				call.Arguments.Add(PrepareCallTarget(method.DeclaringType, target, targetType));
			}
			call.Arguments.AddRange(arguments);
			return (call, method.ReturnType);
		}

		ILInstruction PrepareCallTarget(IType expectedType, ILInstruction target, IType targetType)
		{
			switch (CallInstruction.ExpectedTypeForThisPointer(expectedType)) {
				case StackType.Ref:
					if (target.ResultType == StackType.Ref)
						return target;
					else
						return new AddressOf(target, expectedType);
				case StackType.O:
					if (targetType.IsReferenceType == false) {
						return new Box(target, targetType);
					} else {
						return target;
					}
				default:
					if (expectedType.Kind == TypeKind.Unknown && target.ResultType != StackType.Unknown) {
						return new Conv(target, PrimitiveType.Unknown, false, Sign.None);
					}
					return target;
			}
		}

		bool ConvertCallArguments(IList<ILInstruction> arguments, IMethod method)
		{
			Debug.Assert(arguments.Count == method.Parameters.Count);
			for (int i = 0; i < arguments.Count; i++) {
				var expectedType = method.Parameters[i].Type;
				var argument = ConvertInstruction(arguments[i], expectedType).Item1;
				if (argument == null)
					return false;
				arguments[i] = argument;
			}
			return true;
		}

		(ILInstruction, IType) ConvertCast(CallInstruction invocation, bool isChecked)
		{
			if (invocation.Arguments.Count < 2)
				return (null, SpecialType.UnknownType);
			if (!MatchGetTypeFromHandle(invocation.Arguments[1], out var targetType))
				return (null, SpecialType.UnknownType);
			var (expr, exprType) = ConvertInstruction(invocation.Arguments[0]);
			if (expr == null)
				return (null, SpecialType.UnknownType);
			if (exprType.IsSmallIntegerType() && targetType.IsKnownType(KnownTypeCode.Int32))
				return (expr, targetType);
			return (new ExpressionTreeCast(targetType, expr, isChecked), targetType);
		}

		(ILInstruction, IType) ConvertCoalesce(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			var (trueInst, trueInstType) = ConvertInstruction(invocation.Arguments[0]);
			if (trueInst == null)
				return (null, SpecialType.UnknownType);
			var (fallbackInst, fallbackInstType) = ConvertInstruction(invocation.Arguments[1]);
			if (fallbackInst == null)
				return (null, SpecialType.UnknownType);
			var kind = NullCoalescingKind.Ref;
			var trueInstTypeNonNullable = NullableType.GetUnderlyingType(trueInstType);
			IType targetType;
			if (NullableType.IsNullable(trueInstType) && conversions.ImplicitConversion(fallbackInstType, trueInstTypeNonNullable).IsValid) {
				targetType = trueInstTypeNonNullable;
				kind = NullableType.IsNullable(fallbackInstType) ? NullCoalescingKind.Nullable : NullCoalescingKind.NullableWithValueFallback;
			} else if (conversions.ImplicitConversion(fallbackInstType, trueInstType).IsValid) {
				targetType = trueInstType;
			} else {
				targetType = fallbackInstType;
			}
			return (new NullCoalescingInstruction(kind, trueInst, fallbackInst), targetType);
		}

		(ILInstruction, IType) ConvertComparison(CallInstruction invocation, ComparisonKind kind)
		{
			if (invocation.Arguments.Count < 2)
				return (null, SpecialType.UnknownType);
			var (left, leftType) = ConvertInstruction(invocation.Arguments[0]);
			if (left == null)
				return (null, SpecialType.UnknownType);
			var (right, rightType) = ConvertInstruction(invocation.Arguments[1]);
			if (right == null)
				return (null, SpecialType.UnknownType);
			if (invocation.Arguments.Count == 4 && invocation.Arguments[2].MatchLdcI4(out var isLifted) && MatchGetMethodFromHandle(invocation.Arguments[3], out var method)) {
				if (isLifted != 0) {
					method = CSharpOperators.LiftUserDefinedOperator((IMethod)method);
				}
				return (new Call((IMethod)method) { Arguments = { left, right } }, method.ReturnType);
			}
			var rr = resolver.ResolveBinaryOperator(kind.ToBinaryOperatorType(), new ResolveResult(leftType), new ResolveResult(rightType)) as OperatorResolveResult;
			if (rr != null && !rr.IsError && rr.UserDefinedOperatorMethod != null) {
				return (new Call(rr.UserDefinedOperatorMethod) { Arguments = { left, right } }, rr.UserDefinedOperatorMethod.ReturnType);
			}
			if (leftType.IsKnownType(KnownTypeCode.String) && rightType.IsKnownType(KnownTypeCode.String)) {
				IMethod operatorMethod;
				switch (kind) {
					case ComparisonKind.Equality:
						operatorMethod = leftType.GetMethods(m => m.IsOperator && m.Name == "op_Equality" && m.Parameters.Count == 2).FirstOrDefault(m => m.Parameters[0].Type.IsKnownType(KnownTypeCode.String) && m.Parameters[1].Type.IsKnownType(KnownTypeCode.String));
						if (operatorMethod == null)
							return (null, SpecialType.UnknownType);
						break;
					case ComparisonKind.Inequality:
						operatorMethod = leftType.GetMethods(m => m.IsOperator && m.Name == "op_Inequality" && m.Parameters.Count == 2).FirstOrDefault(m => m.Parameters[0].Type.IsKnownType(KnownTypeCode.String) && m.Parameters[1].Type.IsKnownType(KnownTypeCode.String));
						if (operatorMethod == null)
							return (null, SpecialType.UnknownType);
						break;
					default:
						return (null, SpecialType.UnknownType);
				}
				return (new Call(operatorMethod) { Arguments = { left, right } }, operatorMethod.ReturnType);
			}
			var resultType = context.TypeSystem.FindType(KnownTypeCode.Boolean);
			var lifting = NullableType.IsNullable(leftType) ? ComparisonLiftingKind.CSharp : ComparisonLiftingKind.None;
			var utype = NullableType.GetUnderlyingType(leftType);
			return (new Comp(kind, lifting, utype.GetStackType(), utype.GetSign(), left, right), resultType);
		}

		(ILInstruction, IType) ConvertCondition(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 3)
				return (null, SpecialType.UnknownType);
			var (condition, conditionType) = ConvertInstruction(invocation.Arguments[0]);
			if (condition == null || !conditionType.IsKnownType(KnownTypeCode.Boolean))
				return (null, SpecialType.UnknownType);
			var (trueInst, trueInstType) = ConvertInstruction(invocation.Arguments[1]);
			if (trueInst == null)
				return (null, SpecialType.UnknownType);
			var (falseInst, falseInstType) = ConvertInstruction(invocation.Arguments[2]);
			if (falseInst == null)
				return (null, SpecialType.UnknownType);
			if (!trueInstType.Equals(falseInstType))
				return (null, SpecialType.UnknownType);
			return (new IfInstruction(condition, trueInst, falseInst), trueInstType);
		}

		(ILInstruction, IType) ConvertConstant(CallInstruction invocation)
		{
			if (!MatchConstantCall(invocation, out var value, out var type))
				return (null, SpecialType.UnknownType);
			if (value.MatchBox(out var arg, out var boxType)) {
				if (boxType.Kind == TypeKind.Enum || boxType.IsKnownType(KnownTypeCode.Boolean))
					return (new ExpressionTreeCast(boxType, ConvertValue(arg, invocation), false), boxType);
				value = ConvertValue(arg, invocation);
				return (value, type);
			}
			return (ConvertValue(value, invocation), type);
		}

		(ILInstruction, IType) ConvertElementInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			if (!MatchGetMethodFromHandle(invocation.Arguments[0], out var member))
				return (null, SpecialType.UnknownType);
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return (null, SpecialType.UnknownType);
			CallInstruction call = new Call((IMethod)member);
			for (int i = 0; i < arguments.Count; i++) {
				ILInstruction arg = ConvertInstruction(arguments[i]).Item1;
				if (arg == null)
					return (null, SpecialType.UnknownType);
				arguments[i] = arg;
			}
			call.Arguments.AddRange(arguments);
			return (call, member.ReturnType);
		}

		(ILInstruction, IType) ConvertField(CallInstruction invocation, IType typeHint)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			ILInstruction target = null;
			if (!invocation.Arguments[0].MatchLdNull()) {
				target = ConvertInstruction(invocation.Arguments[0]).Item1;
				if (target == null)
					return (null, SpecialType.UnknownType);
			}
			if (!MatchGetFieldFromHandle(invocation.Arguments[1], out var member))
				return (null, SpecialType.UnknownType);
			IType type = member.ReturnType;
			ILInstruction inst;
			if (target == null) {
				inst = new LdsFlda((IField)member);
			} else {
				if (member.DeclaringType.IsReferenceType == true) {
					inst = new LdFlda(target, (IField)member);
				} else {
					inst = new LdFlda(new AddressOf(target, member.DeclaringType), (IField)member);
				}
			}
			if (typeHint.SkipModifiers() is ByReferenceType brt && !member.ReturnType.IsByRefLike) {
				type = typeHint;
			} else {
				inst = new LdObj(inst, member.ReturnType);
			}
			return (inst, type);
		}

		(ILInstruction, IType) ConvertInvoke(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			var (target, targetType) = ConvertInstruction(invocation.Arguments[0]);
			if (target == null)
				return (null, SpecialType.UnknownType);
			var invokeMethod = targetType.GetDelegateInvokeMethod();
			if (invokeMethod == null)
				return (null, SpecialType.UnknownType);
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return (null, SpecialType.UnknownType);
			if (!ConvertCallArguments(arguments, invokeMethod))
				return (null, SpecialType.UnknownType);
			var call = new CallVirt(invokeMethod);
			call.Arguments.Add(target);
			call.Arguments.AddRange(arguments);
			return (call, invokeMethod.ReturnType);
		}

		(ILInstruction, IType) ConvertListInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return (null, SpecialType.UnknownType);
			var newObj = ConvertInstruction(invocation.Arguments[0]).Item1 as NewObj;
			if (newObj == null)
				return (null, SpecialType.UnknownType);
			IList<ILInstruction> arguments = null;
			ILFunction function = lambdaStack.Peek();
			if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var member)) {
				if (!MatchArgumentList(invocation.Arguments[1], out arguments))
					return (null, SpecialType.UnknownType);
			} else {
				if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments))
					return (null, SpecialType.UnknownType);
			}
			if (arguments == null || arguments.Count == 0)
				return (null, SpecialType.UnknownType);
			var initializer = function.RegisterVariable(VariableKind.InitializerTarget, newObj.Method.DeclaringType);
			for (int i = 0; i < arguments.Count; i++) {
				ILInstruction arg;
				if (arguments[i] is CallInstruction elementInit && elementInit.Method.FullName == "System.Linq.Expressions.Expression.ElementInit") {
					arg = ConvertElementInit(elementInit).Item1;
					if (arg == null)
						return (null, SpecialType.UnknownType);
					((CallInstruction)arg).Arguments.Insert(0, new LdLoc(initializer));
				} else {
					arg = ConvertInstruction(arguments[i]).Item1;
					if (arg == null)
						return (null, SpecialType.UnknownType);
				}
				arguments[i] = arg;
			}
			var initializerBlock = new Block(BlockKind.CollectionInitializer);
			initializerBlock.FinalInstruction = new LdLoc(initializer);
			initializerBlock.Instructions.Add(new StLoc(initializer, newObj));
			initializerBlock.Instructions.AddRange(arguments);
			return (initializerBlock, initializer.Type);
		}

		(ILInstruction, IType) ConvertLogicOperator(CallInstruction invocation, bool and)
		{
			if (invocation.Arguments.Count < 2)
				return (null, SpecialType.UnknownType);
			var (left, leftType) = ConvertInstruction(invocation.Arguments[0]);
			if (left == null)
				return (null, SpecialType.UnknownType);
			var (right, rightType) = ConvertInstruction(invocation.Arguments[1]);
			if (right == null)
				return (null, SpecialType.UnknownType);
			IMember method;
			switch (invocation.Arguments.Count) {
				case 2:
					var resultType = context.TypeSystem.FindType(KnownTypeCode.Boolean);
					return (and ? IfInstruction.LogicAnd(left, right) : IfInstruction.LogicOr(left, right), resultType);
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return (null, SpecialType.UnknownType);
					return (new Call((IMethod)method) {
						Arguments = { left, right }
					}, method.ReturnType);
				case 4:
					if (!invocation.Arguments[2].MatchLdcI4(out var isLifted))
						return (null, SpecialType.UnknownType);
					if (!MatchGetMethodFromHandle(invocation.Arguments[3], out method))
						return (null, SpecialType.UnknownType);
					if (isLifted != 0)
						method = CSharpOperators.LiftUserDefinedOperator((IMethod)method);
					return (new Call((IMethod)method) {
						Arguments = { left, right }
					}, method.ReturnType);
				default:
					return (null, SpecialType.UnknownType);
			}
		}

		(ILInstruction, IType) ConvertMemberInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			var newObj = ConvertInstruction(invocation.Arguments[0]).Item1 as NewObj;
			if (newObj == null)
				return (null, SpecialType.UnknownType);
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return (null, SpecialType.UnknownType);
			if (arguments == null || arguments.Count == 0)
				return (null, SpecialType.UnknownType);
			var function = lambdaStack.Peek();
			var initializer = function.RegisterVariable(VariableKind.InitializerTarget, newObj.Method.DeclaringType);
			for (int i = 0; i < arguments.Count; i++) {
				ILInstruction arg;
				if (arguments[i] is CallInstruction bind && bind.Method.FullName == "System.Linq.Expressions.Expression.Bind") {
					arg = ConvertBind(bind, initializer).Item1;
					if (arg == null)
						return (null, SpecialType.UnknownType);
				} else {
					return (null, SpecialType.UnknownType);
				}
				arguments[i] = arg;
			}
			var initializerBlock = new Block(BlockKind.CollectionInitializer);
			initializerBlock.FinalInstruction = new LdLoc(initializer);
			initializerBlock.Instructions.Add(new StLoc(initializer, newObj));
			initializerBlock.Instructions.AddRange(arguments);
			return (initializerBlock, initializer.Type);
		}

		(ILInstruction, IType) ConvertNewArrayBounds(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			if (!MatchGetTypeFromHandle(invocation.Arguments[0], out var type))
				return (null, SpecialType.UnknownType);
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return (null, SpecialType.UnknownType);
			if (arguments.Count == 0)
				return (null, SpecialType.UnknownType);
			var indices = new ILInstruction[arguments.Count];
			for (int i = 0; i < arguments.Count; i++) {
				var index = ConvertInstruction(arguments[i]).Item1;
				if (index == null)
					return (null, SpecialType.UnknownType);
				indices[i] = index;
			}
			return (new NewArr(type, indices), new ArrayType(context.TypeSystem, type, arguments.Count));
		}

		(ILInstruction, IType) ConvertNewArrayInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			if (!MatchGetTypeFromHandle(invocation.Arguments[0], out var type))
				return (null, SpecialType.UnknownType);
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return (null, SpecialType.UnknownType);
			ArrayType arrayType = new ArrayType(context.BlockContext.TypeSystem, type);
			if (arguments.Count == 0)
				return (new NewArr(type, new LdcI4(0)), arrayType);
			var block = (Block)invocation.Arguments[1];
			var function = lambdaStack.Peek();
			var variable = function.RegisterVariable(VariableKind.InitializerTarget, arrayType);
			Block initializer = new Block(BlockKind.ArrayInitializer);
			int i = 0;
			initializer.Instructions.Add(new StLoc(variable, new NewArr(type, new LdcI4(arguments.Count))));
			foreach (var item in arguments) {
				var value = ConvertInstruction(item).Item1;
				if (value == null)
					return (null, SpecialType.UnknownType);
				initializer.Instructions.Add(new StObj(new LdElema(type, new LdLoc(variable), new LdcI4(i)), value, type));
			}
			initializer.FinalInstruction = new LdLoc(variable);
			return (initializer, variable.Type);
		}

		(ILInstruction, IType) ConvertNewObject(CallInstruction invocation)
		{
			IMember member;
			IList<ILInstruction> arguments;
			NewObj newObj;
			switch (invocation.Arguments.Count) {
				case 1:
					if (MatchGetTypeFromHandle(invocation.Arguments[0], out var type)) {
						var ctor = type.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();
						if (ctor == null)
							return (null, SpecialType.UnknownType);
						return (new NewObj(ctor), type);
					}
					if (MatchGetConstructorFromHandle(invocation.Arguments[0], out member)) {
						return (new NewObj((IMethod)member), member.DeclaringType);
					}
					return (null, SpecialType.UnknownType);
				case 2:
					if (!MatchGetConstructorFromHandle(invocation.Arguments[0], out member))
						return (null, SpecialType.UnknownType);
					if (!MatchArgumentList(invocation.Arguments[1], out arguments))
						return (null, SpecialType.UnknownType);
					IMethod method = (IMethod)member;
					if (!ConvertCallArguments(arguments, method))
						return (null, SpecialType.UnknownType);
					newObj = new NewObj(method);
					newObj.Arguments.AddRange(arguments);
					return (newObj, member.DeclaringType);
				case 3:
					if (!MatchGetConstructorFromHandle(invocation.Arguments[0], out member))
						return (null, SpecialType.UnknownType);
					if (!MatchArgumentList(invocation.Arguments[1], out arguments))
						return (null, SpecialType.UnknownType);
					method = (IMethod)member;
					if (!ConvertCallArguments(arguments, method))
						return (null, SpecialType.UnknownType);
					newObj = new NewObj(method);
					newObj.Arguments.AddRange(arguments);
					return (newObj, member.DeclaringType);
			}
			return (null, SpecialType.UnknownType);
		}

		(ILInstruction, IType) ConvertNotOperator(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 1)
				return (null, SpecialType.UnknownType);
			var (argument, argumentType) = ConvertInstruction(invocation.Arguments[0]);
			if (argument == null)
				return (null, SpecialType.UnknownType);
			switch (invocation.Arguments.Count) {
				case 1:
					return (argumentType.IsKnownType(KnownTypeCode.Boolean) ? Comp.LogicNot(argument) : (ILInstruction)new BitNot(argument), argumentType);
				case 2:
					if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var method))
						return (null, SpecialType.UnknownType);
					return (new Call((IMethod)method) {
						Arguments = { argument }
					}, method.ReturnType);
				default:
					return (null, SpecialType.UnknownType);
			}
		}

		(ILInstruction, IType) ConvertProperty(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return (null, SpecialType.UnknownType);
			ILInstruction target = null;
			IType targetType = null;
			if (!invocation.Arguments[0].MatchLdNull()) {
				(target, targetType) = ConvertInstruction(invocation.Arguments[0]);
				if (target == null)
					return (null, SpecialType.UnknownType);
			}
			if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var member))
				return (null, SpecialType.UnknownType);
			IList<ILInstruction> arguments;
			if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments)) {
				arguments = new List<ILInstruction>();
			} else {
				if (!ConvertCallArguments(arguments, (IMethod)member))
					return (null, SpecialType.UnknownType);
			}
			CallInstruction call;
			if (member.IsAbstract || member.IsVirtual || member.IsOverride) {
				call = new CallVirt((IMethod)member);
			} else {
				call = new Call((IMethod)member);
			}
			if (target != null) {
				call.Arguments.Add(PrepareCallTarget(member.DeclaringType, target, targetType));
			}
			call.Arguments.AddRange(arguments);
			return (call, member.ReturnType);
		}

		(ILInstruction, IType) ConvertTypeAs(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			var converted = ConvertInstruction(invocation.Arguments[0]).Item1;
			if (!MatchGetTypeFromHandle(invocation.Arguments[1], out var type))
				return (null, SpecialType.UnknownType);
			if (converted == null)
				return (null, SpecialType.UnknownType);

			ILInstruction inst = new IsInst(converted, type);
			// We must follow ECMA-335, III.4.6:
			// If typeTok is a nullable type, Nullable<T>, it is interpreted as "boxed" T.
			if (type.IsKnownType(KnownTypeCode.NullableOfT))
				inst = new UnboxAny(inst, type);
			return (inst, type);
		}

		(ILInstruction, IType) ConvertTypeIs(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return (null, SpecialType.UnknownType);
			var converted = ConvertInstruction(invocation.Arguments[0]).Item1;
			if (!MatchGetTypeFromHandle(invocation.Arguments[1], out var type))
				return (null, SpecialType.UnknownType);
			var resultType = context.TypeSystem.FindType(KnownTypeCode.Boolean);
			if (converted != null)
				return (new Comp(ComparisonKind.Inequality, Sign.None, new IsInst(converted, type), new LdNull()), resultType);
			return (null, SpecialType.UnknownType);
		}

		(ILInstruction, IType) ConvertUnaryNumericOperator(CallInstruction invocation, BinaryNumericOperator op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 1)
				return (null, SpecialType.UnknownType);
			var (argument, argumentType) = ConvertInstruction(invocation.Arguments[0]);
			if (argument == null)
				return (null, SpecialType.UnknownType);
			switch (invocation.Arguments.Count) {
				case 1:
					ILInstruction left;
					switch (argument.ResultType) {
						case StackType.I4:
							left = new LdcI4(0);
							break;
						case StackType.I8:
							left = new LdcI8(0);
							break;
						case StackType.I:
							left = new Conv(new LdcI4(0), PrimitiveType.I, false, Sign.None);
							break;
						case StackType.F4:
							left = new LdcF4(0);
							break;
						case StackType.F8:
							left = new LdcF8(0);
							break;
						default:
							return (null, SpecialType.UnknownType);
					}
					return (new BinaryNumericInstruction(op, left, argument, isChecked == true, argumentType.GetSign()), argumentType);
				case 2:
					if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var method))
						return (null, SpecialType.UnknownType);
					return (new Call((IMethod)method) {
						Arguments = { argument }
					}, method.ReturnType);
			}
			return (null, SpecialType.UnknownType);
		}

		ILInstruction ConvertValue(ILInstruction value, ILInstruction context)
		{
			switch (value) {
				case LdLoc ldloc:
					if (IsExpressionTreeParameter(ldloc.Variable)) {
						if (!parameterMapping.TryGetValue(ldloc.Variable, out var v))
							return ldloc;
						if (context is CallInstruction parentCall
							&& parentCall.Method.FullName == "System.Linq.Expressions.Expression.Call"
							&& v.StackType.IsIntegerType())
							return new LdLoca(v);
						return null;
					} else if (IsClosureReference(ldloc.Variable)) {
						if (ldloc.Variable.Kind == VariableKind.Local) {
							ldloc.Variable.Kind = VariableKind.DisplayClassLocal;
						}
						if (ldloc.Variable.CaptureScope == null) {
							ldloc.Variable.CaptureScope = BlockContainer.FindClosestContainer(context);
						}
						return ldloc;
					} else {
						return ldloc;
					}
				default:
					return value.Clone();
			}
		}

		bool IsClosureReference(ILVariable variable)
		{
			if (!variable.IsSingleDefinition || !(variable.StoreInstructions.SingleOrDefault() is StLoc store))
				return false;
			if (!(store.Value is NewObj newObj))
				return false;
			return TransformDisplayClassUsage.IsPotentialClosure(this.context, newObj);
		}

		bool IsExpressionTreeParameter(ILVariable variable)
		{
			return variable.Type.FullName == "System.Linq.Expressions.ParameterExpression";
		}

		bool MatchConstantCall(ILInstruction inst, out ILInstruction value, out IType type)
		{
			value = null;
			type = null;
			if (inst is CallInstruction call && call.Method.FullName == "System.Linq.Expressions.Expression.Constant") {
				value = call.Arguments[0];
				if (call.Arguments.Count == 2)
					return MatchGetTypeFromHandle(call.Arguments[1], out type);
				type = value.InferType(context.TypeSystem);
				return true;
			}
			return false;
		}

		internal static bool MatchGetTypeFromHandle(ILInstruction inst, out IType type)
		{
			type = null;
			return inst is CallInstruction getTypeCall
				&& getTypeCall.Method.FullName == "System.Type.GetTypeFromHandle"
				&& getTypeCall.Arguments.Count == 1
				&& getTypeCall.Arguments[0].MatchLdTypeToken(out type);
		}

		bool MatchGetMethodFromHandle(ILInstruction inst, out IMember member)
		{
			member = null;
			//castclass System.Reflection.MethodInfo(call GetMethodFromHandle(ldmembertoken op_Addition))
			if (!inst.MatchCastClass(out var arg, out var type))
				return false;
			if (!type.Equals(context.TypeSystem.FindType(new FullTypeName("System.Reflection.MethodInfo"))))
				return false;
			if (!(arg is CallInstruction call && call.Method.FullName == "System.Reflection.MethodBase.GetMethodFromHandle"))
				return false;
			switch (call.Arguments.Count) {
				case 1:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					break;
				case 2:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					if (!call.Arguments[1].MatchLdTypeToken(out var genericType))
						return false;
					break;
			}
			return true;
		}

		bool MatchGetConstructorFromHandle(ILInstruction inst, out IMember member)
		{
			member = null;
			//castclass System.Reflection.ConstructorInfo(call GetMethodFromHandle(ldmembertoken op_Addition))
			if (!inst.MatchCastClass(out var arg, out var type))
				return false;
			if (!type.Equals(context.TypeSystem.FindType(new FullTypeName("System.Reflection.ConstructorInfo"))))
				return false;
			if (!(arg is CallInstruction call && call.Method.FullName == "System.Reflection.MethodBase.GetMethodFromHandle"))
				return false;
			switch (call.Arguments.Count) {
				case 1:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					break;
				case 2:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					if (!call.Arguments[1].MatchLdTypeToken(out var genericType))
						return false;
					break;
			}
			return true;
		}

		bool MatchGetFieldFromHandle(ILInstruction inst, out IMember member)
		{
			member = null;
			if (!(inst is CallInstruction call && call.Method.FullName == "System.Reflection.FieldInfo.GetFieldFromHandle"))
				return false;
			switch (call.Arguments.Count) {
				case 1:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					break;
				case 2:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					if (!call.Arguments[1].MatchLdTypeToken(out var genericType))
						return false;
					break;
			}
			return true;
		}

		bool MatchArgumentList(ILInstruction inst, out IList<ILInstruction> arguments)
		{
			arguments = null;
			if (!(inst is Block block && block.Kind == BlockKind.ArrayInitializer)) {
				if (IsEmptyParameterList(inst)) {
					arguments = new List<ILInstruction>();
					return true;
				}
				return false;
			}
			int i = 0;
			arguments = new List<ILInstruction>();
			foreach (var item in block.Instructions.OfType<StObj>()) {
				if (!(item.Target is LdElema ldelem && ldelem.Indices.Single().MatchLdcI4(i)))
					return false;
				arguments.Add(item.Value);
				i++;
			}
			return true;
		}
	}
}
