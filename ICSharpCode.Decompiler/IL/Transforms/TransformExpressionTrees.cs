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
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Converts LINQ Expression Trees to ILFunctions/ILAst instructions.
	/// 
	/// We build a tree of Func{ILInstruction}s, which are only executed, if the whole transform succeeds.
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
			if (!context.Settings.ExpressionTrees)
				return;
			this.context = context;
			this.conversions = CSharpConversions.Get(context.TypeSystem);
			this.resolver = new CSharpResolver(context.TypeSystem);
			this.parameters = new Dictionary<ILVariable, (IType, string)>();
			this.parameterMapping = new Dictionary<ILVariable, ILVariable>();
			this.instructionsToRemove = new List<ILInstruction>();
			this.lambdaStack = new Stack<ILFunction>();
			for (int i = pos; i < block.Instructions.Count; i++)
			{
				if (MatchParameterVariableAssignment(block.Instructions[i], out var v, out var type, out var name))
				{
					parameters.Add(v, (type, name));
					continue;
				}
				if (TryConvertExpressionTree(block.Instructions[i], block.Instructions[i]))
				{
					foreach (var inst in instructionsToRemove)
						block.Instructions.Remove(inst);
					instructionsToRemove.Clear();
				}
				break;
			}
		}

		bool TryConvertExpressionTree(ILInstruction instruction, ILInstruction statement)
		{
			if (MightBeExpressionTree(instruction, statement))
			{
				var lambda = ConvertLambda((CallInstruction)instruction);
				if (lambda != null)
				{
					context.Step("Convert Expression Tree", instruction);
					var newLambda = (ILFunction)lambda();
					SetExpressionTreeFlag(newLambda, (CallInstruction)instruction);
					instruction.ReplaceWith(newLambda);
					context.EndStep(newLambda);
					return true;
				}
				return false;
			}
			if (instruction is Block block && block.Kind == BlockKind.ControlFlow)
				return false;  // don't look into nested blocks
			foreach (var child in instruction.Children)
			{
				if (TryConvertExpressionTree(child, statement))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Converts a Expression.Lambda call into an ILFunction.
		/// If the conversion fails, null is returned.
		/// </summary>
		Func<ILInstruction> ConvertLambda(CallInstruction instruction)
		{
			if (instruction.Method.Name != "Lambda" || instruction.Arguments.Count != 2 || instruction.Method.ReturnType.FullName != "System.Linq.Expressions.Expression" || instruction.Method.ReturnType.TypeArguments.Count != 1)
				return null;
			var parameterList = new List<IParameter>();
			var parameterVariablesList = new List<ILVariable>();
			if (!ReadParameters(instruction.Arguments[1], parameterList, parameterVariablesList, new SimpleTypeResolveContext(context.Function.Method)))
				return null;
			var container = new BlockContainer();
			container.AddILRange(instruction);
			var functionType = instruction.Method.ReturnType.TypeArguments[0];
			var returnType = functionType.GetDelegateInvokeMethod()?.ReturnType ?? SpecialType.UnknownType;
			var function = new ILFunction(returnType, parameterList, context.Function.GenericContext, container, ILFunctionKind.ExpressionTree);
			function.DelegateType = functionType;
			function.Kind = IsExpressionTree(functionType) ? ILFunctionKind.ExpressionTree : ILFunctionKind.Delegate;
			function.Variables.AddRange(parameterVariablesList);
			function.AddILRange(instruction);
			lambdaStack.Push(function);
			var bodyInstruction = ConvertInstruction(instruction.Arguments[0]);
			lambdaStack.Pop();
			if (bodyInstruction == null)
				return null;
			return BuildFunction;

			ILFunction BuildFunction()
			{
				lambdaStack.Push(function);
				var convertedBody = bodyInstruction();
				lambdaStack.Pop();
				if (convertedBody == null)
					return null;
				container.ExpectedResultType = convertedBody.InferType(context.TypeSystem);
				container.Blocks.Add(new Block() { Instructions = { new Leave(container, convertedBody) } });
				// Replace all other usages of the parameter variable
				foreach (var mapping in parameterMapping)
				{
					foreach (var load in mapping.Key.LoadInstructions.ToArray())
					{
						if (load.IsDescendantOf(instruction))
							continue;
						load.ReplaceWith(new LdLoc(mapping.Value));
					}
				}
				return function;
			}
		}

		Func<ILInstruction> ConvertQuote(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 1)
				return null;
			var argument = invocation.Arguments.Single();
			if (argument is ILFunction function)
			{
				return () => function;
			}
			else
			{
				var converted = ConvertInstruction(argument);
				if (converted == null)
					return null;
				return BuildQuote;

				ILInstruction BuildQuote()
				{
					var f = converted();
					if (f is ILFunction lambda && argument is CallInstruction call)
					{
						SetExpressionTreeFlag(lambda, call);
					}

					return f;
				}
			}
		}

		void SetExpressionTreeFlag(ILFunction lambda, CallInstruction call)
		{
			lambda.Kind = IsExpressionTree(call.Method.ReturnType) ? ILFunctionKind.ExpressionTree : ILFunctionKind.Delegate;
			lambda.DelegateType = call.Method.ReturnType;
		}

		bool ReadParameters(ILInstruction initializer, IList<IParameter> parameters, IList<ILVariable> parameterVariables, ITypeResolveContext resolveContext)
		{
			switch (initializer)
			{
				case Block initializerBlock:
					if (initializerBlock.Kind != BlockKind.ArrayInitializer)
						return false;
					int i = 0;
					foreach (var inst in initializerBlock.Instructions.OfType<StObj>())
					{
						if (i >= this.parameters.Count)
							return false;
						if (!inst.Value.MatchLdLoc(out var v))
							return false;
						if (!this.parameters.TryGetValue(v, out var value))
							return false;
						// Add parameter variable only once to mapping.
						if (!this.parameterMapping.ContainsKey(v))
						{
							var param = new ILVariable(VariableKind.Parameter, value.Item1, i) { Name = value.Item2 };
							parameterMapping.Add(v, param);
							parameterVariables.Add(param);
							parameters.Add(new DefaultParameter(value.Item1, value.Item2));
							instructionsToRemove.Add((ILInstruction)v.StoreInstructions[0]);
						}
						i++;
					}
					return true;
				default:
					return IsEmptyParameterList(initializer);
			}
		}

		Func<ILInstruction> ConvertInstruction(ILInstruction instruction, IType typeHint = null)
		{
			var inst = Convert();

			if (inst == null)
				return null;

			ILInstruction DoConvert()
			{
				var result = inst();
				if (result == null)
					return null;
				if (typeHint != null)
				{
					if (result.ResultType != typeHint.GetStackType())
					{
						return new Conv(result, typeHint.GetStackType().ToPrimitiveType(), false, typeHint.GetSign());
					}
				}
				return result;
			}
			return DoConvert;

			Func<ILInstruction> Convert()
			{
				switch (instruction)
				{
					case CallInstruction invocation:
						if (invocation.Method.DeclaringType.FullName != "System.Linq.Expressions.Expression")
							return null;

						switch (invocation.Method.Name)
						{
							case "Add":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Add, "op_Addition", false);
							case "AddChecked":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Add, "op_Addition", true);
							case "And":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.BitAnd, "op_BitwiseAnd");
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
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Div, "op_Division");
							case "Equal":
								return ConvertComparison(invocation, ComparisonKind.Equality);
							case "ExclusiveOr":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.BitXor, "op_ExclusiveOr");
							case "Field":
								return ConvertField(invocation, typeHint);
							case "GreaterThan":
								return ConvertComparison(invocation, ComparisonKind.GreaterThan);
							case "GreaterThanOrEqual":
								return ConvertComparison(invocation, ComparisonKind.GreaterThanOrEqual);
							case "Invoke":
								return ConvertInvoke(invocation);
							case "Lambda":
								return ConvertLambda(invocation);
							case "LeftShift":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.ShiftLeft, "op_LeftShift");
							case "LessThan":
								return ConvertComparison(invocation, ComparisonKind.LessThan);
							case "LessThanOrEqual":
								return ConvertComparison(invocation, ComparisonKind.LessThanOrEqual);
							case "ListInit":
								return ConvertListInit(invocation);
							case "MemberInit":
								return ConvertMemberInit(invocation);
							case "Modulo":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Rem, "op_Modulus");
							case "Multiply":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Mul, "op_Multiply", false);
							case "MultiplyChecked":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Mul, "op_Multiply", true);
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
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.BitOr, "op_BitwiseOr");
							case "OrElse":
								return ConvertLogicOperator(invocation, false);
							case "Property":
								return ConvertProperty(invocation);
							case "Quote":
								return ConvertQuote(invocation);
							case "RightShift":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.ShiftRight, "op_RightShift");
							case "Subtract":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Sub, "op_Subtraction", false);
							case "SubtractChecked":
								return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.Sub, "op_Subtraction", true);
							case "TypeAs":
								return ConvertTypeAs(invocation);
							case "TypeIs":
								return ConvertTypeIs(invocation);
						}
						return null;
					case ILFunction function:
						ILFunction ApplyChangesToILFunction()
						{
							if (function.Kind == ILFunctionKind.ExpressionTree)
							{
								function.DelegateType = UnwrapExpressionTree(function.DelegateType);
								function.Kind = ILFunctionKind.Delegate;
							}
							return function;
						}
						return ApplyChangesToILFunction;
					case LdLoc ldloc:
						if (IsExpressionTreeParameter(ldloc.Variable))
						{
							// Replace an already mapped parameter with the actual ILVariable,
							// we generated earlier.
							if (parameterMapping.TryGetValue(ldloc.Variable, out var v))
							{
								if (typeHint.SkipModifiers() is ByReferenceType && !v.Type.IsByRefLike)
									return () => new LdLoca(v);
								return () => new LdLoc(v);
							}
							// This is a parameter variable from an outer scope.
							// We can't replace these variables just yet, because the transform works backwards.
							// We simply return the same instruction again, but return the actual expected type,
							// so our transform can continue normally.
							// Later, we will replace all references to unmapped variables,
							// with references to mapped parameters.
							if (ldloc.Variable.IsSingleDefinition && ldloc.Variable.StoreInstructions[0] is ILInstruction instr)
							{
								if (MatchParameterVariableAssignment(instr, out _, out var t, out _))
									return () => new ExpressionTreeCast(t, ldloc, false);
							}
						}
						return null;
					default:
						return null;
				}
			}
		}

		bool IsExpressionTree(IType delegateType) => delegateType is ParameterizedType pt
			&& pt.FullName == "System.Linq.Expressions.Expression"
			&& pt.TypeArguments.Count == 1;

		IType UnwrapExpressionTree(IType delegateType)
		{
			if (delegateType is ParameterizedType pt && pt.FullName == "System.Linq.Expressions.Expression" && pt.TypeArguments.Count == 1)
			{
				return pt.TypeArguments[0];
			}
			return delegateType;
		}

		Func<ILInstruction> ConvertArrayIndex(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var array = ConvertInstruction(invocation.Arguments[0]);
			if (array == null)
				return null;
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				arguments = new[] { invocation.Arguments[1] };

			ILInstruction Convert()
			{
				var arrayInst = array();
				if (arrayInst == null)
					return null;
				if (arrayInst.InferType(context.TypeSystem) is not ArrayType type)
					return null;
				Func<ILInstruction>[] toBeConverted = new Func<ILInstruction>[arguments.Count];
				for (int i = 0; i < arguments.Count; i++)
				{
					var converted = ConvertInstruction(arguments[i]);
					if (converted == null)
						return null;
					toBeConverted[i] = converted;
				}
				return new LdObj(new LdElema(type.ElementType, arrayInst, toBeConverted.SelectArray(f => f())) { DelayExceptions = true }, type.ElementType);
			}
			return Convert;
		}

		Func<ILInstruction> ConvertArrayLength(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 1)
				return null;
			var converted = ConvertInstruction(invocation.Arguments[0]);
			if (converted == null)
				return null;
			return () => new LdLen(StackType.I4, converted());
		}

		Func<ILInstruction> ConvertBinaryNumericOperator(CallInstruction invocation, BinaryNumericOperator op, string operatorName, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			var left = ConvertInstruction(invocation.Arguments[0]);
			if (left == null)
				return null;
			var right = ConvertInstruction(invocation.Arguments[1]);
			if (right == null)
				return null;

			IMember method;
			switch (invocation.Arguments.Count)
			{
				case 2:
					return () => {
						var leftInst = left();
						var rightInst = right();
						if (leftInst == null || rightInst == null)
							return null;
						var leftType = leftInst.InferType(context.TypeSystem);
						var rightType = rightInst.InferType(context.TypeSystem);
						if (op is BinaryNumericOperator.ShiftLeft or BinaryNumericOperator.ShiftRight)
						{
							if (!NullableType.GetUnderlyingType(rightType).IsKnownType(KnownTypeCode.Int32))
								return null;
						}
						else
						{
							if (!rightType.Equals(leftType))
								return null;
						}
						if (leftType.IsKnownType(KnownTypeCode.Decimal))
						{
							var op_Method = leftType.GetMethods(m => m.IsOperator && m.Name == operatorName).FirstOrDefault();
							if (op_Method == null)
								return null;
							return new Call(op_Method) {
								Arguments = { leftInst, rightInst }
							};
						}
						return new BinaryNumericInstruction(op, leftInst, rightInst,
							NullableType.GetUnderlyingType(leftType).GetStackType(),
							NullableType.GetUnderlyingType(rightType).GetStackType(),
							isChecked == true,
							leftType.GetSign(),
							isLifted: NullableType.IsNullable(leftType) || NullableType.IsNullable(rightType));
					};
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return null;
					return () => new Call((IMethod)method) {
						Arguments = { left(), right() }
					};
				case 4:
					if (!invocation.Arguments[2].MatchLdcI4(out _))
						return null;
					if (!MatchGetMethodFromHandle(invocation.Arguments[3], out method))
						return null;
					return () => {
						var leftInst = left();
						var rightInst = right();
						if (leftInst == null || rightInst == null)
							return null;
						var op_Method = (IMethod)method;
						if (NullableType.IsNullable(leftInst.InferType(context.TypeSystem)))
							op_Method = CSharpOperators.LiftUserDefinedOperator(op_Method);
						return new Call(op_Method) {
							Arguments = { leftInst, rightInst }
						};
					};
				default:
					return null;
			}
		}

		Func<ILVariable, ILInstruction> ConvertBind(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var value = ConvertInstruction(invocation.Arguments[1]);
			if (value == null)
				return null;
			if (MatchGetMethodFromHandle(invocation.Arguments[0], out var member))
			{
				var method = (IMethod)member;
				// It is possible to use Expression.Bind with a get-accessor,
				// however, it would be an invalid expression tree if the property is readonly.
				// As this is an assignment, the ILAst expects a set-accessor. To avoid any problems
				// constructing property assignments, we explicitly use the set-accessor instead.
				if (method.AccessorOwner is IProperty { CanSet: true } property && method != property.Setter)
				{
					member = property.Setter;
				}
			}
			else if (MatchGetFieldFromHandle(invocation.Arguments[0], out member))
			{
			}
			else
			{
				return null;
			}
			switch (member)
			{
				case IMethod method:
					if (method.IsStatic)
						return targetVariable => new Call(method) { Arguments = { new LdLoc(targetVariable), value() } };
					else
						return targetVariable => new CallVirt(method) { Arguments = { new LdLoc(targetVariable), value() } };
				case IField field:
					return targetVariable => new StObj(new LdFlda(new LdLoc(targetVariable), (IField)member) { DelayExceptions = true }, value(), member.ReturnType);
			}
			return null;
		}

		Func<ILInstruction> ConvertCall(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			IList<ILInstruction> arguments = null;
			Func<ILInstruction> targetConverter = null;
			if (MatchGetMethodFromHandle(invocation.Arguments[0], out var member))
			{
				// static method
				if (invocation.Arguments.Count != 2 || !MatchArgumentList(invocation.Arguments[1], out arguments))
				{
					arguments = new List<ILInstruction>(invocation.Arguments.Skip(1));
				}
			}
			else if (MatchGetMethodFromHandle(invocation.Arguments[1], out member))
			{
				if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments))
				{
					arguments = new List<ILInstruction>(invocation.Arguments.Skip(2));
				}
				if (!invocation.Arguments[0].MatchLdNull())
				{
					targetConverter = ConvertInstruction(invocation.Arguments[0]);
					if (targetConverter == null)
						return null;
				}
			}
			if (arguments == null)
				return null;
			IMethod method = (IMethod)member;
			var convertedArguments = ConvertCallArguments(arguments, method);
			if (convertedArguments == null)
				return null;
			if (method.FullName == "System.Reflection.MethodInfo.CreateDelegate" && method.Parameters.Count == 2)
			{
				if (!MatchGetMethodFromHandle(UnpackConstant(invocation.Arguments[0]), out var targetMethod))
					return null;
				if (!MatchGetTypeFromHandle(UnpackConstant(arguments[0]), out var delegateType))
					return null;
				return () => new NewObj(delegateType.GetConstructors().Single()) {
					Arguments = { convertedArguments[1](), new LdFtn((IMethod)targetMethod) }
				};
			}

			CallInstruction BuildCall()
			{
				CallInstruction call;
				if (method.IsStatic)
				{
					call = new Call(method);
				}
				else
				{
					call = new CallVirt(method);
				}
				if (targetConverter != null)
				{
					var target = targetConverter();
					if (target == null)
						return null;
					call.Arguments.Add(PrepareCallTarget(method.DeclaringType, target, target.InferType(context.TypeSystem)));
				}
				call.Arguments.AddRange(convertedArguments.Select(f => f()));
				return call;
			}
			return BuildCall;
		}

		ILInstruction PrepareCallTarget(IType expectedType, ILInstruction target, IType targetType)
		{
			ILInstruction result;
			switch (CallInstruction.ExpectedTypeForThisPointer(expectedType, null))
			{
				case StackType.Ref:
					if (target.ResultType == StackType.Ref)
					{
						result = target;
					}
					else if (target is LdLoc ldloc)
					{
						result = new LdLoca(ldloc.Variable).WithILRange(ldloc);
					}
					else
					{
						result = new AddressOf(target, expectedType);
					}
					break;
				case StackType.Obj:
					result = target;
					break;
				case StackType.VT:
					result = new Box(target, targetType);
					break;
				default:
					result = target;
					break;
			}

			if (expectedType.Kind == TypeKind.Unknown && result.ResultType != StackType.Unknown)
			{
				result = new Conv(target, PrimitiveType.Unknown, false, Sign.None);
			}
			else if (expectedType.Kind != TypeKind.Unknown && result.ResultType == StackType.Unknown)
			{
				// if references are missing, we need to coerce the unknown type to the expected type.
				// Otherwise we will get loads of assertions and expression trees
				// are usually explicit about any conversions.
				result = new Conv(result, expectedType.ToPrimitiveType(), false, Sign.None);
			}

			return result;
		}

		ILInstruction UnpackConstant(ILInstruction inst)
		{
			if (!(inst is CallInstruction call && call.Method.FullName == "System.Linq.Expressions.Expression.Constant" && call.Arguments.Count == 2))
				return inst;
			return call.Arguments[0];
		}

		Func<ILInstruction>[] ConvertCallArguments(IList<ILInstruction> arguments, IMethod method)
		{
			var converted = new Func<ILInstruction>[arguments.Count];
			Debug.Assert(arguments.Count == method.Parameters.Count);
			for (int i = 0; i < arguments.Count; i++)
			{
				var expectedType = method.Parameters[i].Type;
				var argument = ConvertInstruction(arguments[i], expectedType);
				if (argument == null)
					return null;
				converted[i] = argument;
			}
			return converted;
		}

		Func<ILInstruction> ConvertCast(CallInstruction invocation, bool isChecked)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			if (!MatchGetTypeFromHandle(invocation.Arguments[1], out var targetType))
				return null;
			var expr = ConvertInstruction(invocation.Arguments[0]);
			if (expr == null)
				return null;
			return () => {
				var exprInst = expr();
				if (exprInst == null)
					return null;
				if (exprInst.InferType(context.TypeSystem).IsSmallIntegerType() && targetType.IsKnownType(KnownTypeCode.Int32))
					return exprInst;
				return new ExpressionTreeCast(targetType, exprInst, isChecked);
			};
		}

		Func<ILInstruction> ConvertCoalesce(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var trueInst = ConvertInstruction(invocation.Arguments[0]);
			if (trueInst == null)
				return null;
			var fallbackInst = ConvertInstruction(invocation.Arguments[1]);
			if (fallbackInst == null)
				return null;
			return () => {
				var trueValue = trueInst();
				var fallbackValue = fallbackInst();
				if (trueValue == null || fallbackValue == null)
					return null;
				var trueInstType = trueValue.InferType(context.TypeSystem);
				var fallbackInstType = fallbackValue.InferType(context.TypeSystem);
				var kind = NullCoalescingKind.Ref;
				var trueInstTypeNonNullable = NullableType.GetUnderlyingType(trueInstType);
				IType targetType;
				if (NullableType.IsNullable(trueInstType) && conversions.ImplicitConversion(fallbackInstType, trueInstTypeNonNullable).IsValid)
				{
					targetType = trueInstTypeNonNullable;
					kind = NullableType.IsNullable(fallbackInstType) ? NullCoalescingKind.Nullable : NullCoalescingKind.NullableWithValueFallback;
				}
				else if (conversions.ImplicitConversion(fallbackInstType, trueInstType).IsValid)
				{
					targetType = trueInstType;
				}
				else
				{
					targetType = fallbackInstType;
				}
				return new NullCoalescingInstruction(targetType, kind, trueValue, fallbackValue);
			};
		}

		Func<ILInstruction> ConvertComparison(CallInstruction invocation, ComparisonKind kind)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			var left = ConvertInstruction(invocation.Arguments[0]);
			if (left == null)
				return null;
			var right = ConvertInstruction(invocation.Arguments[1]);
			if (right == null)
				return null;
			if (invocation.Arguments.Count == 4 && invocation.Arguments[2].MatchLdcI4(out _) && MatchGetMethodFromHandle(invocation.Arguments[3], out var method))
			{
				return () => {
					var leftInst = left();
					var rightInst = right();
					if (leftInst == null || rightInst == null)
						return null;
					var op_Method = (IMethod)method;
					if (NullableType.IsNullable(leftInst.InferType(context.TypeSystem)))
						op_Method = CSharpOperators.LiftUserDefinedOperator(op_Method);
					return new Call(op_Method) { Arguments = { leftInst, rightInst } };
				};
			}
			return () => {
				var leftInst = left();
				var rightInst = right();
				if (leftInst == null || rightInst == null)
					return null;
				var leftType = leftInst.InferType(context.TypeSystem);
				var rightType = rightInst.InferType(context.TypeSystem);
				var rr = resolver.ResolveBinaryOperator(kind.ToBinaryOperatorType(), new ResolveResult(leftType), new ResolveResult(rightType)) as OperatorResolveResult;
				if (rr != null && !rr.IsError && rr.UserDefinedOperatorMethod != null)
				{
					return new Call(rr.UserDefinedOperatorMethod) { Arguments = { leftInst, rightInst } };
				}
				if (leftType.IsKnownType(KnownTypeCode.String) && rightType.IsKnownType(KnownTypeCode.String))
				{
					IMethod operatorMethod;
					switch (kind)
					{
						case ComparisonKind.Equality:
							operatorMethod = leftType.GetMethods(m => m.IsOperator && m.Name == "op_Equality" && m.Parameters.Count == 2).FirstOrDefault(m => m.Parameters[0].Type.IsKnownType(KnownTypeCode.String) && m.Parameters[1].Type.IsKnownType(KnownTypeCode.String));
							if (operatorMethod == null)
								return null;
							break;
						case ComparisonKind.Inequality:
							operatorMethod = leftType.GetMethods(m => m.IsOperator && m.Name == "op_Inequality" && m.Parameters.Count == 2).FirstOrDefault(m => m.Parameters[0].Type.IsKnownType(KnownTypeCode.String) && m.Parameters[1].Type.IsKnownType(KnownTypeCode.String));
							if (operatorMethod == null)
								return null;
							break;
						default:
							return null;
					}
					return new Call(operatorMethod) { Arguments = { leftInst, rightInst } };
				}
				var lifting = NullableType.IsNullable(leftType) ? ComparisonLiftingKind.CSharp : ComparisonLiftingKind.None;
				var utype = NullableType.GetUnderlyingType(leftType);
				return new Comp(kind, lifting, utype.GetStackType(), utype.GetSign(), leftInst, rightInst);
			};
		}

		Func<ILInstruction> ConvertCondition(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 3)
				return null;
			var condition = ConvertInstruction(invocation.Arguments[0]);
			if (condition == null)
				return null;
			var trueInst = ConvertInstruction(invocation.Arguments[1]);
			if (trueInst == null)
				return null;
			var falseInst = ConvertInstruction(invocation.Arguments[2]);
			if (falseInst == null)
				return null;
			return () => {
				var conditionValue = condition();
				var trueValue = trueInst();
				var falseValue = falseInst();
				if (conditionValue == null || trueValue == null || falseValue == null)
					return null;
				if (!conditionValue.InferType(context.TypeSystem).IsKnownType(KnownTypeCode.Boolean))
					return null;
				var trueInstType = trueValue.InferType(context.TypeSystem);
				var falseInstType = falseValue.InferType(context.TypeSystem);
				if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(trueInstType, falseInstType))
					return null;
				return new IfInstruction(conditionValue, trueValue, falseValue, trueInstType);
			};
		}

		Func<ILInstruction> ConvertConstant(CallInstruction invocation)
		{
			if (!MatchConstantCall(invocation, out var value, out var type))
				return null;
			if (value.MatchBox(out var arg, out var boxType))
			{
				if (boxType.Kind == TypeKind.Enum || boxType.IsKnownType(KnownTypeCode.Boolean))
					return () => new ExpressionTreeCast(boxType, ConvertValue(arg, invocation), false);
				return () => ConvertValue(arg, invocation);
			}
			return () => ConvertValue(value, invocation);
		}

		Func<ILInstruction> ConvertElementInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			if (!MatchGetMethodFromHandle(invocation.Arguments[0], out var member))
				return null;
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return null;
			var args = new Func<ILInstruction>[arguments.Count];
			for (int i = 0; i < arguments.Count; i++)
			{
				var arg = ConvertInstruction(arguments[i]);
				if (arg == null)
					return null;
				args[i] = arg;
			}

			ILInstruction BuildCall()
			{
				CallInstruction call = member.IsStatic
					? (CallInstruction)new Call((IMethod)member)
					: new CallVirt((IMethod)member);
				call.Arguments.AddRange(args.Select(f => f()));
				return call;
			}
			return BuildCall;
		}

		Func<ILInstruction> ConvertField(CallInstruction invocation, IType typeHint)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			Func<ILInstruction> targetConverter = null;
			if (!invocation.Arguments[0].MatchLdNull())
			{
				targetConverter = ConvertInstruction(invocation.Arguments[0]);
				if (targetConverter == null)
					return null;
			}
			if (!MatchGetFieldFromHandle(invocation.Arguments[1], out var member))
				return null;
			IType type = member.ReturnType;
			if (typeHint.SkipModifiers() is ByReferenceType && !member.ReturnType.IsByRefLike)
			{
				type = typeHint;
			}
			return BuildField;

			ILInstruction BuildField()
			{
				ILInstruction inst;
				if (targetConverter == null)
				{
					inst = new LdsFlda((IField)member);
				}
				else
				{
					var target = targetConverter();
					if (member.DeclaringType.IsReferenceType == true)
					{
						inst = new LdFlda(target, (IField)member) { DelayExceptions = true };
					}
					else
					{
						inst = new LdFlda(new AddressOf(target, member.DeclaringType), (IField)member) { DelayExceptions = true };
					}
				}
				if (!(typeHint.SkipModifiers() is ByReferenceType && !member.ReturnType.IsByRefLike))
				{
					inst = new LdObj(inst, member.ReturnType);
				}
				return inst;
			}
		}

		Func<ILInstruction> ConvertInvoke(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var targetConverter = ConvertInstruction(invocation.Arguments[0]);
			if (targetConverter == null)
				return null;
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return null;

			ILInstruction BuildCall()
			{
				var target = targetConverter();
				if (target == null)
					return null;
				var invokeMethod = target.InferType(context.TypeSystem).GetDelegateInvokeMethod();
				if (invokeMethod == null)
					return null;
				var convertedArguments = ConvertCallArguments(arguments, invokeMethod);
				if (convertedArguments == null)
					return null;
				var call = new CallVirt(invokeMethod);
				call.Arguments.Add(target);
				call.Arguments.AddRange(convertedArguments.Select(f => f()));
				return call;
			}
			return BuildCall;
		}

		Func<ILInstruction> ConvertListInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			var newObj = ConvertInstruction(invocation.Arguments[0]);
			if (newObj == null)
				return null;
			if (!MatchNew((CallInstruction)invocation.Arguments[0], out var ctor))
				return null;
			IList<ILInstruction> arguments;
			if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var member))
			{
				if (!MatchArgumentList(invocation.Arguments[1], out arguments))
					return null;
			}
			else
			{
				if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments))
					return null;
			}
			if (arguments == null || arguments.Count == 0)
				return null;
			Func<ILVariable, ILInstruction>[] convertedArguments = new Func<ILVariable, ILInstruction>[arguments.Count];
			for (int i = 0; i < arguments.Count; i++)
			{
				if (arguments[i] is CallInstruction elementInit && elementInit.Method.FullName == "System.Linq.Expressions.Expression.ElementInit")
				{
					var arg = ConvertElementInit(elementInit);
					if (arg == null)
						return null;

					convertedArguments[i] = v => { var a = arg(); ((CallInstruction)a).Arguments.Insert(0, new LdLoc(v)); return a; };
				}
				else
				{
					var arg = ConvertInstruction(arguments[i]);
					if (arg == null)
						return null;
					convertedArguments[i] = v => arg();
				}
			}

			Block BuildBlock()
			{
				var initializerBlock = new Block(BlockKind.CollectionInitializer);

				ILFunction function = lambdaStack.Peek();
				var initializer = function.RegisterVariable(VariableKind.InitializerTarget, ctor.DeclaringType);
				initializerBlock.FinalInstruction = new LdLoc(initializer);
				initializerBlock.Instructions.Add(new StLoc(initializer, newObj()));
				initializerBlock.Instructions.AddRange(convertedArguments.Select(f => f(initializer)));
				return initializerBlock;
			}
			return BuildBlock;
		}

		Func<ILInstruction> ConvertLogicOperator(CallInstruction invocation, bool and)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			var left = ConvertInstruction(invocation.Arguments[0]);
			if (left == null)
				return null;
			var right = ConvertInstruction(invocation.Arguments[1]);
			if (right == null)
				return null;
			IMember method;
			switch (invocation.Arguments.Count)
			{
				case 2:
					return () => and ? IfInstruction.LogicAnd(left(), right(), context.TypeSystem) : IfInstruction.LogicOr(left(), right(), context.TypeSystem);
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return null;
					return () => new Call((IMethod)method) {
						Arguments = { left(), right() }
					};
				case 4:
					if (!invocation.Arguments[2].MatchLdcI4(out _))
						return null;
					if (!MatchGetMethodFromHandle(invocation.Arguments[3], out method))
						return null;
					return () => {
						var leftInst = left();
						var rightInst = right();
						if (leftInst == null || rightInst == null)
							return null;
						var op_Method = (IMethod)method;
						if (NullableType.IsNullable(leftInst.InferType(context.TypeSystem)))
							op_Method = CSharpOperators.LiftUserDefinedOperator(op_Method);
						return new Call(op_Method) {
							Arguments = { leftInst, rightInst }
						};
					};
				default:
					return null;
			}
		}

		Func<ILInstruction> ConvertMemberInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var newObj = ConvertInstruction(invocation.Arguments[0]);
			if (newObj == null)
				return null;
			if (!MatchNew((CallInstruction)invocation.Arguments[0], out var ctor))
				return null;
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return null;
			if (arguments == null || arguments.Count == 0)
				return null;

			Func<ILVariable, ILInstruction>[] convertedArguments = new Func<ILVariable, ILInstruction>[arguments.Count];
			for (int i = 0; i < arguments.Count; i++)
			{
				Func<ILVariable, ILInstruction> arg;
				if (arguments[i] is CallInstruction bind && bind.Method.FullName == "System.Linq.Expressions.Expression.Bind")
				{
					arg = ConvertBind(bind);
					if (arg == null)
						return null;
				}
				else
				{
					return null;
				}
				convertedArguments[i] = arg;
			}

			ILInstruction BuildBlock()
			{
				var function = lambdaStack.Peek();
				var initializer = function.RegisterVariable(VariableKind.InitializerTarget, ctor.DeclaringType);

				var initializerBlock = new Block(BlockKind.CollectionInitializer);
				initializerBlock.FinalInstruction = new LdLoc(initializer);
				initializerBlock.Instructions.Add(new StLoc(initializer, newObj()));
				initializerBlock.Instructions.AddRange(convertedArguments.Select(f => f(initializer)));

				return initializerBlock;
			}

			return BuildBlock;
		}

		Func<ILInstruction> ConvertNewArrayBounds(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			if (!MatchGetTypeFromHandle(invocation.Arguments[0], out var type))
				return null;
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return null;
			if (arguments.Count == 0)
				return null;
			var indices = new Func<ILInstruction>[arguments.Count];
			for (int i = 0; i < arguments.Count; i++)
			{
				var index = ConvertInstruction(arguments[i]);
				if (index == null)
					return null;
				indices[i] = index;
			}
			return () => new NewArr(type, indices.SelectArray(f => f()));
		}

		Func<ILInstruction> ConvertNewArrayInit(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			if (!MatchGetTypeFromHandle(invocation.Arguments[0], out var type))
				return null;
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				return null;
			ArrayType arrayType = new ArrayType(context.BlockContext.TypeSystem, type);
			if (arguments.Count == 0)
				return () => new NewArr(type, new LdcI4(0));
			var convertedArguments = new Func<ILInstruction>[arguments.Count];
			for (int i = 0; i < arguments.Count; i++)
			{
				ILInstruction item = arguments[i];
				var value = ConvertInstruction(item);
				if (value == null)
					return null;
				convertedArguments[i] = value;
			}

			ILInstruction BuildInitializer()
			{
				var block = (Block)invocation.Arguments[1];
				var function = lambdaStack.Peek();
				var variable = function.RegisterVariable(VariableKind.InitializerTarget, arrayType);
				Block initializer = new Block(BlockKind.ArrayInitializer);
				initializer.Instructions.Add(new StLoc(variable, new NewArr(type, new LdcI4(convertedArguments.Length))));
				for (int i = 0; i < convertedArguments.Length; i++)
				{
					initializer.Instructions.Add(new StObj(new LdElema(type, new LdLoc(variable), new LdcI4(i)) { DelayExceptions = true }, convertedArguments[i](), type));
				}
				initializer.FinalInstruction = new LdLoc(variable);
				return initializer;
			}

			return BuildInitializer;
		}

		bool MatchNew(CallInstruction invocation, out IMethod ctor)
		{
			ctor = null;
			if (invocation.Method.Name != "New")
				return false;
			switch (invocation.Arguments.Count)
			{
				case 1:
					if (MatchGetTypeFromHandle(invocation.Arguments[0], out var type))
					{
						ctor = type.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();
						return ctor != null;
					}
					if (MatchGetConstructorFromHandle(invocation.Arguments[0], out var member))
					{
						ctor = (IMethod)member;
						return true;
					}
					return false;
				case 2:
				case 3:
					if (!MatchGetConstructorFromHandle(invocation.Arguments[0], out member))
						return false;
					ctor = (IMethod)member;
					return true;
				default:
					return false;
			}
		}

		Func<ILInstruction> ConvertNewObject(CallInstruction invocation)
		{
			switch (invocation.Arguments.Count)
			{
				case 1:
					if (MatchGetTypeFromHandle(invocation.Arguments[0], out var type))
					{
						var ctor = type.GetConstructors(c => c.Parameters.Count == 0).FirstOrDefault();
						if (ctor == null)
							return null;
						return () => new NewObj(ctor);
					}
					if (MatchGetConstructorFromHandle(invocation.Arguments[0], out var member))
					{
						return () => new NewObj((IMethod)member);
					}
					return null;
				case 2:
					if (!MatchGetConstructorFromHandle(invocation.Arguments[0], out member))
						return null;
					if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
						return null;
					IMethod method = (IMethod)member;
					Func<ILInstruction>[] convertedArguments = ConvertCallArguments(arguments, method);
					if (convertedArguments == null)
						return null;
					return () => BuildNewObj(method, convertedArguments);
				case 3:
					if (!MatchGetConstructorFromHandle(invocation.Arguments[0], out member))
						return null;
					if (!MatchArgumentList(invocation.Arguments[1], out arguments))
						return null;
					method = (IMethod)member;
					convertedArguments = ConvertCallArguments(arguments, method);
					if (convertedArguments == null)
						return null;
					return () => BuildNewObj(method, convertedArguments);
			}

			ILInstruction BuildNewObj(IMethod method, Func<ILInstruction>[] args)
			{
				var newObj = new NewObj(method);
				newObj.Arguments.AddRange(args.Select(f => f()));
				return newObj;
			}

			return null;
		}

		Func<ILInstruction> ConvertNotOperator(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 1)
				return null;
			var argument = ConvertInstruction(invocation.Arguments[0]);
			if (argument == null)
				return null;
			switch (invocation.Arguments.Count)
			{
				case 1:
					return () => {
						var argumentInst = argument();
						if (argumentInst == null)
							return null;
						var argumentType = argumentInst.InferType(context.TypeSystem);
						var underlyingType = NullableType.GetUnderlyingType(argumentType);
						bool isLifted = NullableType.IsNullable(argumentType);
						return underlyingType.IsKnownType(KnownTypeCode.Boolean)
							? Comp.LogicNot(argumentInst, isLifted)
							: (ILInstruction)new BitNot(argumentInst, isLifted, underlyingType.GetStackType());
					};
				case 2:
					if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var method))
						return null;
					return () => new Call((IMethod)method) {
						Arguments = { argument() }
					};
				default:
					return null;
			}
		}

		Func<ILInstruction> ConvertProperty(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			Func<ILInstruction> targetConverter = null;
			if (!invocation.Arguments[0].MatchLdNull())
			{
				targetConverter = ConvertInstruction(invocation.Arguments[0]);
				if (targetConverter == null)
					return null;
			}
			if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var member))
				return null;
			IList<ILInstruction> arguments;
			if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments))
			{
				arguments = new List<ILInstruction>();
			}
			var convertedArguments = ConvertCallArguments(arguments, (IMethod)member);
			if (convertedArguments == null)
				return null;
			ILInstruction BuildProperty()
			{
				CallInstruction call;
				if (member.IsStatic)
				{
					call = new Call((IMethod)member);
				}
				else
				{
					call = new CallVirt((IMethod)member);
				}
				if (targetConverter != null)
				{
					var target = targetConverter();
					if (target == null)
						return null;
					call.Arguments.Add(PrepareCallTarget(member.DeclaringType, target, target.InferType(context.TypeSystem)));
				}
				call.Arguments.AddRange(convertedArguments.Select(f => f()));
				return call;
			}
			return BuildProperty;
		}

		Func<ILInstruction> ConvertTypeAs(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var converted = ConvertInstruction(invocation.Arguments[0]);
			if (!MatchGetTypeFromHandle(invocation.Arguments[1], out var type))
				return null;
			if (converted == null)
				return null;
			ILInstruction BuildTypeAs()
			{
				ILInstruction inst = new IsInst(converted(), type);
				// We must follow ECMA-335, III.4.6:
				// If typeTok is a nullable type, Nullable<T>, it is interpreted as "boxed" T.
				if (type.IsKnownType(KnownTypeCode.NullableOfT))
					inst = new UnboxAny(inst, type);
				return inst;
			}
			return BuildTypeAs;
		}

		Func<ILInstruction> ConvertTypeIs(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var converted = ConvertInstruction(invocation.Arguments[0]);
			if (!MatchGetTypeFromHandle(invocation.Arguments[1], out var type))
				return null;
			var resultType = context.TypeSystem.FindType(KnownTypeCode.Boolean);
			if (converted != null)
				return () => new Comp(ComparisonKind.Inequality, Sign.None, new IsInst(converted(), type), new LdNull());
			return null;
		}

		Func<ILInstruction> ConvertUnaryNumericOperator(CallInstruction invocation, BinaryNumericOperator op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 1)
				return null;
			var argument = ConvertInstruction(invocation.Arguments[0]);
			if (argument == null)
				return null;
			switch (invocation.Arguments.Count)
			{
				case 1:
					return () => {
						var argumentInst = argument();
						if (argumentInst == null)
							return null;
						ILInstruction left;
						var argumentType = argumentInst.InferType(context.TypeSystem);
						var underlyingType = NullableType.GetUnderlyingType(argumentType);

						switch (underlyingType.GetStackType())
						{
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
							case StackType.VT when underlyingType.IsKnownType(KnownTypeCode.Decimal):
								left = new LdcDecimal(0);
								break;
							default:
								return null;
						}
						return new BinaryNumericInstruction(op, left, argumentInst,
							underlyingType.GetStackType(),
							underlyingType.GetStackType(),
							isChecked == true,
							argumentType.GetSign(),
							isLifted: NullableType.IsNullable(argumentType));
					};
				case 2:
					if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var method))
						return null;
					return () => new Call((IMethod)method) {
						Arguments = { argument() }
					};
			}
			return null;
		}

		ILInstruction ConvertValue(ILInstruction value, ILInstruction context)
		{
			switch (value)
			{
				case LdLoc ldloc:
					if (IsExpressionTreeParameter(ldloc.Variable))
					{
						if (!parameterMapping.TryGetValue(ldloc.Variable, out var v))
							return ldloc.Clone();
						if (context is CallInstruction parentCall
							&& parentCall.Method.FullName == "System.Linq.Expressions.Expression.Call"
							&& v.StackType.IsIntegerType())
							return new LdLoca(v).WithILRange(ldloc);
						return null;
					}
					else if (IsClosureReference(ldloc.Variable))
					{
						if (ldloc.Variable.Kind == VariableKind.Local)
						{
							ldloc.Variable.Kind = VariableKind.DisplayClassLocal;
						}
						if (ldloc.Variable.CaptureScope == null)
						{
							ldloc.Variable.CaptureScope = BlockContainer.FindClosestContainer(context);
							var f = ldloc.Variable.CaptureScope.Ancestors.OfType<ILFunction>().FirstOrDefault();
							if (f != null)
							{
								f.CapturedVariables.Add(ldloc.Variable);
							}
						}
						return ldloc;
					}
					else
					{
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
			if (inst is CallInstruction call && call.Method.FullName == "System.Linq.Expressions.Expression.Constant")
			{
				value = call.Arguments[0];
				if (call.Arguments.Count == 2)
					return MatchGetTypeFromHandle(call.Arguments[1], out type);
				type = value switch {
					LdNull => SpecialType.NullType,
					LdStr => context.TypeSystem.FindType(KnownTypeCode.String),
					LdcF4 => context.TypeSystem.FindType(KnownTypeCode.Single),
					LdcF8 => context.TypeSystem.FindType(KnownTypeCode.Double),
					LdcI4 => context.TypeSystem.FindType(KnownTypeCode.Int32),
					LdcI8 => context.TypeSystem.FindType(KnownTypeCode.Int64),
					_ => value.InferType(context.TypeSystem),
				};
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
			if (type.FullName != "System.Reflection.MethodInfo")
				return false;
			if (!(arg is CallInstruction call && call.Method.FullName == "System.Reflection.MethodBase.GetMethodFromHandle"))
				return false;
			return MatchFromHandleParameterList(call, out member);
		}

		bool MatchGetConstructorFromHandle(ILInstruction inst, out IMember member)
		{
			member = null;
			//castclass System.Reflection.ConstructorInfo(call GetMethodFromHandle(ldmembertoken op_Addition))
			if (!inst.MatchCastClass(out var arg, out var type))
				return false;
			if (type.FullName != "System.Reflection.ConstructorInfo")
				return false;
			if (!(arg is CallInstruction call && call.Method.FullName == "System.Reflection.MethodBase.GetMethodFromHandle"))
				return false;
			return MatchFromHandleParameterList(call, out member);
		}

		bool MatchGetFieldFromHandle(ILInstruction inst, out IMember member)
		{
			member = null;
			if (!(inst is CallInstruction call && call.Method.FullName == "System.Reflection.FieldInfo.GetFieldFromHandle"))
				return false;
			return MatchFromHandleParameterList(call, out member);
		}

		static bool MatchFromHandleParameterList(CallInstruction call, out IMember member)
		{
			member = null;
			switch (call.Arguments.Count)
			{
				case 1:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					break;
				case 2:
					if (!call.Arguments[0].MatchLdMemberToken(out member))
						return false;
					if (!call.Arguments[1].MatchLdTypeToken(out _))
						return false;
					break;
				default:
					return false;
			}
			return true;
		}

		bool MatchArgumentList(ILInstruction inst, out IList<ILInstruction> arguments)
		{
			arguments = null;
			if (!(inst is Block block && block.Kind == BlockKind.ArrayInitializer))
			{
				if (IsEmptyParameterList(inst))
				{
					arguments = new List<ILInstruction>();
					return true;
				}
				return false;
			}
			int i = 0;
			arguments = new List<ILInstruction>();
			foreach (var item in block.Instructions.OfType<StObj>())
			{
				if (!(item.Target is LdElema ldelem && ldelem.Indices.Single().MatchLdcI4(i)))
					return false;
				arguments.Add(item.Value);
				i++;
			}
			return true;
		}
	}
}
