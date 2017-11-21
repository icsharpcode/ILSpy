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
using System.Linq;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class TransformExpressionTrees : IStatementTransform
	{
		static bool MightBeExpressionTree(ILInstruction inst, ILInstruction stmt)
		{
			if (!(inst is CallInstruction call && call.Method.FullName == "System.Linq.Expressions.Expression.Lambda"))
				return false;
			if (!ILInlining.CanUninline(call, stmt) || call.Arguments.Count != 2)
				return false;
			if (!((call.Arguments[1] is CallInstruction emptyCall && emptyCall.Method.FullName == "System.Array.Empty" && emptyCall.Arguments.Count == 0)
				|| (call.Arguments[1] is Block block && block.Kind == BlockKind.ArrayInitializer)))
				return false;
			return true;
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
			IMethod parameterMethod = initCall.Method;
			if (!(parameterMethod.Name == "Parameter" && parameterMethod.DeclaringType.FullName == "System.Linq.Expressions.Expression"))
				return false;
			CallInstruction typeArg = initCall.Arguments[0] as CallInstruction;
			if (typeArg == null || typeArg.Arguments.Count != 1)
				return false;
			if (typeArg.Method.FullName != "System.Type.GetTypeFromHandle")
				return false;
			return typeArg.Arguments[0].MatchLdTypeToken(out type) && initCall.Arguments[1].MatchLdStr(out name);
		}

		StatementTransformContext context;
		Dictionary<ILVariable, (IType, string)> parameters;
		Dictionary<ILVariable, ILVariable> parameterMapping;
		List<ILInstruction> instructionsToRemove;

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			this.context = context;
			this.parameters = new Dictionary<ILVariable, (IType, string)>();
			this.parameterMapping = new Dictionary<ILVariable, ILVariable>();
			this.instructionsToRemove = new List<ILInstruction>();
			for (int i = pos; i < block.Instructions.Count; i++) {
				if (MatchParameterVariableAssignment(block.Instructions[i], out var v, out var type, out var name)) {
					parameters.Add(v, (type, name));
					continue;
				}
				if (TryConvertExpressionTree(block.Instructions[i], block.Instructions[i])) {
					foreach (var inst in instructionsToRemove)
						block.Instructions.Remove(inst);
					instructionsToRemove.Clear();
					continue;
				}
			}
		}

		bool TryConvertExpressionTree(ILInstruction instruction, ILInstruction statement)
		{
			if (MightBeExpressionTree(instruction, statement)) {
				var lambda = ConvertLambda((CallInstruction)instruction);
				if (lambda != null) {
					instruction.ReplaceWith(lambda);
					return true;
				}
				return false;
			}
			foreach (var child in instruction.Children) {
				if (TryConvertExpressionTree(child, statement))
					return true;
			}
			return false;
		}

		NewObj ConvertLambda(CallInstruction instruction)
		{
			if (instruction.Method.Name != "Lambda" || instruction.Arguments.Count != 2 || instruction.Method.ReturnType.FullName != "System.Linq.Expressions.Expression" || instruction.Method.ReturnType.TypeArguments.Count != 1)
				return null;
			var parameterList = new List<IParameter>();
			var parameterVariablesList = new List<ILVariable>();
			if (!ReadParameters(instruction.Arguments[1], parameterList, parameterVariablesList, new SimpleTypeResolveContext(context.Function.Method)))
				return null;
			var bodyInstruction = ConvertInstruction(instruction.Arguments[0]);
			if (bodyInstruction == null)
				return null;
			var container = new BlockContainer(expectedResultType: bodyInstruction.ResultType);
			container.Blocks.Add(new Block() { Instructions = { new Leave(container, bodyInstruction) } });
			var function = new ILFunction(instruction.Method.ReturnType.TypeArguments[0].TypeArguments.LastOrDefault() ?? context.TypeSystem.Compilation.FindType(KnownTypeCode.Void), parameterList, container);
			function.IsExpressionTree = true;
			function.Variables.AddRange(parameterVariablesList);
			return new NewObj(instruction.Method.ReturnType.TypeArguments[0].GetConstructors().First()) {
				Arguments = {
					new LdNull(),
					function
				}
			};
		}

		bool ReadParameters(ILInstruction initializer, IList<IParameter> parameters, IList<ILVariable> parameterVariables, ITypeResolveContext resolveContext)
		{
			if (!context.Function.Method.IsStatic) {
				var thisParam = context.Function.Variables[0];
				parameterVariables.Add(new ILVariable(VariableKind.Parameter, thisParam.Type, -1) { Name = "this" });
			}
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
						var param = new ILVariable(VariableKind.Parameter, value.Item1, i) { Name = value.Item2 };
						parameterMapping.Add(v, param);
						parameterVariables.Add(param);
						parameters.Add(new DefaultUnresolvedParameter(value.Item1.ToTypeReference(), value.Item2).CreateResolvedParameter(resolveContext));
						instructionsToRemove.Add((ILInstruction)v.StoreInstructions[0]);
						i++;
					}
					return true;
				case CallInstruction emptyCall:
					return emptyCall.Method.FullName == "System.Array.Empty" && emptyCall.Arguments.Count == 0;
				default:
					return false;
			}
		}

		ILInstruction ConvertInstruction(ILInstruction instruction)
		{
			switch (instruction) {
				case CallInstruction invocation:
					if (invocation.Method.DeclaringType.FullName != "System.Linq.Expressions.Expression")
						return null;

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
							return ConvertField(invocation);
						case "GreaterThan":
							return ConvertComparison(invocation, ComparisonKind.GreaterThan);
						case "GreaterThanOrEqual":
							return ConvertComparison(invocation, ComparisonKind.GreaterThanOrEqual);
						/*case "Invoke":
							return ConvertInvoke(invocation);*/
						case "Lambda":
							return ConvertLambda(invocation);
						case "LeftShift":
							return ConvertBinaryNumericOperator(invocation, BinaryNumericOperator.ShiftLeft);
						case "LessThan":
							return ConvertComparison(invocation, ComparisonKind.LessThan);
						case "LessThanOrEqual":
							return ConvertComparison(invocation, ComparisonKind.LessThanOrEqual);
						/*case "ListInit":
							return ConvertListInit(invocation);
						case "MemberInit":
							return ConvertMemberInit(invocation);*/
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
						/*case "NewArrayBounds":
							return ConvertNewArrayBounds(invocation);
						case "NewArrayInit":
							return ConvertNewArrayInit(invocation);*/
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
						/*case "Quote":
							if (invocation.Arguments.Count == 1)
								return Convert(invocation.Arguments.Single());
							else
								return null;*/
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
					return null;
				default:
					return ConvertValue(instruction);
			}
		}

		ILInstruction ConvertArrayIndex(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var array = ConvertInstruction(invocation.Arguments[0]);
			if (array == null)
				return null;
			var type = ((ArrayType)array.InferType()).ElementType;
			if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
				arguments = new[] { invocation.Arguments[1] };
			arguments = arguments.Select(ConvertInstruction).ToArray();
			if (arguments.Any(p => p == null))
				return null;
			return new LdObj(new LdElema(type, array, arguments.ToArray()), type);
		}

		ILInstruction ConvertArrayLength(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 1)
				return null;
			var converted = ConvertInstruction(invocation.Arguments[0]);
			if (converted == null)
				return null;
			return new LdLen(StackType.I4, converted);
		}

		ILInstruction ConvertBinaryNumericOperator(CallInstruction invocation, BinaryNumericOperator op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			var left = ConvertInstruction(invocation.Arguments.ElementAt(0));
			if (left == null)
				return null;
			var right = ConvertInstruction(invocation.Arguments.ElementAt(1));
			if (right == null)
				return null;
			IMember method;
			switch (invocation.Arguments.Count) {
				case 2:
					return new BinaryNumericInstruction(op, left, right, isChecked == true, Sign.None);
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return null;
					return new Call((IMethod)method) {
						Arguments = { left, right }
					};
				case 4:
					//if (!trueOrFalse.IsMatch(invocation.Arguments.ElementAt(2)))
					//	return null;
					if (!MatchGetMethodFromHandle(invocation.Arguments[3], out method))
						return null;
					return new Call((IMethod)method) {
						Arguments = { left, right }
					};
				default:
					return null;
			}
		}

		ILInstruction ConvertNotOperator(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 1)
				return null;
			var argument = ConvertInstruction(invocation.Arguments.ElementAt(0));
			if (argument == null)
				return null;
			switch (invocation.Arguments.Count) {
				case 1:
					return argument.InferType().IsKnownType(KnownTypeCode.Boolean) ? Comp.LogicNot(argument) : (ILInstruction)new BitNot(argument);
				case 2:
					if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var method))
						return null;
					return new Call((IMethod)method) {
						Arguments = { argument }
					};
				default:
					return null;
			}
		}

		ILInstruction ConvertCall(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			IList<ILInstruction> arguments = null;
			if (MatchGetMethodFromHandle(invocation.Arguments[0], out var member)) {
				// static method
				if (invocation.Arguments.Count != 2 || !MatchArgumentList(invocation.Arguments[1], out arguments)) {
					arguments = new List<ILInstruction>(invocation.Arguments.Skip(1));
				}
			} else if (MatchGetMethodFromHandle(invocation.Arguments[1], out member)) {
				if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments)) {
					arguments = new List<ILInstruction>(invocation.Arguments.Skip(2));
				}
				if (!invocation.Arguments[0].MatchLdNull())
					arguments.Insert(0, invocation.Arguments[0]);
			}
			if (arguments == null)
				return null;
			arguments = arguments.Select(ConvertInstruction).ToArray();
			if (arguments.Any(p => p == null))
				return null;
			CallInstruction call;
			if (member.IsAbstract || member.IsVirtual || member.IsOverridable) {
				call = new CallVirt((IMethod)member);
			} else {
				call = new Call((IMethod)member);
			}
			call.Arguments.AddRange(arguments);
			return call;
		}

		ILInstruction ConvertCast(CallInstruction invocation, bool isChecked)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			if (!MatchTypeOfCall(invocation.Arguments[1], out var targetType))
				return null;
			var expr = ConvertInstruction(invocation.Arguments[0]);
			if (expr == null)
				return null;
			var sourceType = expr.InferType();
			if (sourceType.Equals(SpecialType.UnknownType))
				return null;
			if (sourceType.IsReferenceType != true) {
				if (targetType.IsKnownType(KnownTypeCode.Object))
					return new Box(expr, sourceType);
			}
			return null;
		}

		ILInstruction ConvertCoalesce(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var trueInst = ConvertInstruction(invocation.Arguments[0]);
			if (trueInst == null)
				return null;
			var fallbackInst = ConvertInstruction(invocation.Arguments[1]);
			if (fallbackInst == null)
				return null;
			// TODO : nullable types?
			return new NullCoalescingInstruction(NullCoalescingKind.Ref, trueInst, fallbackInst);
		}

		ILInstruction ConvertComparison(CallInstruction invocation, ComparisonKind kind)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			var left = ConvertInstruction(invocation.Arguments[0]);
			if (left == null)
				return null;
			var right = ConvertInstruction(invocation.Arguments[1]);
			if (right == null)
				return null;
			if (invocation.Arguments.Count == 3 && MatchGetMethodFromHandle(invocation.Arguments[2], out var method)) {
				return new Call((IMethod)method) { Arguments = { left, right } };
			}
			// TODO: Sign??
			return new Comp(kind, Sign.None, left, right);
		}

		ILInstruction ConvertCondition(CallInstruction invocation)
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
			return new IfInstruction(condition, trueInst, falseInst);
		}

		ILInstruction ConvertConstant(CallInstruction invocation)
		{
			if (!MatchConstantCall(invocation, out var value, out var type))
				return null;
			if (value.MatchBox(out var arg, out _)) {
				value = arg;
			}
			return ConvertValue(value);
		}

		ILInstruction ConvertField(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			ILInstruction target = null;
			if (!invocation.Arguments[0].MatchLdNull()) {
				target = ConvertInstruction(invocation.Arguments[0]);
				if (target == null)
					return null;
			}
			if (!MatchGetFieldFromHandle(invocation.Arguments[1], out var member))
				return null;
			if (target == null) {
				return new LdObj(new LdsFlda((IField)member), member.ReturnType);
			} else {
				return new LdObj(new LdFlda(target, (IField)member), member.ReturnType);
			}
		}

		ILInstruction ConvertLogicOperator(CallInstruction invocation, bool and)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			var left = ConvertInstruction(invocation.Arguments.ElementAt(0));
			if (left == null)
				return null;
			var right = ConvertInstruction(invocation.Arguments.ElementAt(1));
			if (right == null)
				return null;
			IMember method;
			switch (invocation.Arguments.Count) {
				case 2:
					return and ? IfInstruction.LogicAnd(left, right) : IfInstruction.LogicOr(left, right);
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return null;
					return new Call((IMethod)method) {
						Arguments = { left, right }
					};
				case 4:
					//if (!trueOrFalse.IsMatch(invocation.Arguments.ElementAt(2)))
					//	return null;
					if (!MatchGetMethodFromHandle(invocation.Arguments[3], out method))
						return null;
					return new Call((IMethod)method) {
						Arguments = { left, right }
					};
				default:
					return null;
			}
		}

		ILInstruction ConvertNewObject(CallInstruction invocation)
		{
			IMember member;
			switch (invocation.Arguments.Count) {
				case 1:
					if (MatchTypeOfCall(invocation.Arguments[0], out var type)) {
						var ctors = type.GetConstructors().ToArray();
						if (ctors.Length != 1 || ctors[0].Parameters.Count > 0)
							return null;
						return new NewObj(ctors[0]);
					}
					if (MatchGetConstructorFromHandle(invocation.Arguments[0], out member)) {
						return new NewObj((IMethod)member);
					}
					return null;
				case 2:
					if (!MatchGetConstructorFromHandle(invocation.Arguments[0], out member))
						return null;
					if (!MatchArgumentList(invocation.Arguments[1], out var arguments))
						return null;
					var args = arguments.SelectArray(ConvertInstruction);
					if (args.Any(a => a == null))
						return null;
					var newObj = new NewObj((IMethod)member);
					newObj.Arguments.AddRange(args);
					return newObj;
			}
			return null;
		}

		ILInstruction ConvertProperty(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			ILInstruction target = null;
			if (!invocation.Arguments[0].MatchLdNull()) {
				target = ConvertInstruction(invocation.Arguments[0]);
				if (target == null)
					return null;
			}
			if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var member))
				return null;
			IList<ILInstruction> arguments;
			if (invocation.Arguments.Count != 3 || !MatchArgumentList(invocation.Arguments[2], out arguments)) {
				arguments = new List<ILInstruction>();
			} else {
				for (int i = 0; i < arguments.Count; i++) {
					arguments[i] = ConvertInstruction(arguments[i]);
					if (arguments[i] == null)
						return null;
				}
			}
			if (target != null) {
				arguments.Insert(0, target);
			}
			CallInstruction call;
			if (member.IsAbstract || member.IsVirtual || member.IsOverridable) {
				call = new CallVirt((IMethod)member);
			} else {
				call = new Call((IMethod)member);
			}
			call.Arguments.AddRange(arguments);
			return call;
		}

		ILInstruction ConvertTypeAs(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var converted = ConvertInstruction(invocation.Arguments[0]);
			if (!MatchTypeOfCall(invocation.Arguments[1], out var type))
				return null;
			if (converted != null)
				return new IsInst(converted, type);
			return null;
		}

		ILInstruction ConvertTypeIs(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			var converted = ConvertInstruction(invocation.Arguments[0]);
			if (!MatchTypeOfCall(invocation.Arguments[1], out var type))
				return null;
			if (converted != null)
				return new Comp(ComparisonKind.Inequality, Sign.None, new IsInst(converted, type), new LdNull());
			return null;
		}

		ILInstruction ConvertUnaryNumericOperator(CallInstruction invocation, BinaryNumericOperator op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 1)
				return null;
			var argument = ConvertInstruction(invocation.Arguments.ElementAt(0));
			if (argument == null)
				return null;
			switch (invocation.Arguments.Count) {
				case 1:
					return new BinaryNumericInstruction(op, new LdcI4(0), argument, isChecked == true, Sign.None);
				case 2:
					if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var method))
						return null;
					return new Call((IMethod)method) {
						Arguments = { argument }
					};
			}
			return null;
		}

		ILInstruction ConvertValue(ILInstruction value)
		{
			switch (value) {
				case LdLoc ldloc:
					if (IsExpressionTreeParameter(ldloc.Variable)) {
						if (!parameterMapping.TryGetValue(ldloc.Variable, out var v))
							return null;
						return new LdLoc(v);
					} else {
						return ldloc;
					}
				default:
					return value;
			}
		}

		bool IsExpressionTreeParameter(ILVariable variable)
		{
			return variable.Type.FullName == "System.Linq.Expressions.ParameterExpression";
		}

		bool MatchConstantCall(ILInstruction inst, out ILInstruction value, out IType type)
		{
			value = null;
			type = null;
			if (inst is CallInstruction call && call.Arguments.Count == 2 && call.Method.FullName == "System.Linq.Expressions.Expression.Constant") {
				value = call.Arguments[0];
				return MatchTypeOfCall(call.Arguments[1], out type);
			}
			return false;
		}

		bool MatchTypeOfCall(ILInstruction inst, out IType type)
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
			if (!type.Equals(context.TypeSystem.Compilation.FindType(new FullTypeName("System.Reflection.MethodInfo"))))
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
			if (!type.Equals(context.TypeSystem.Compilation.FindType(new FullTypeName("System.Reflection.ConstructorInfo"))))
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
				if (inst is CallInstruction emptyCall && emptyCall.Method.FullName == "System.Array.Empty" && emptyCall.Arguments.Count == 0) {
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
