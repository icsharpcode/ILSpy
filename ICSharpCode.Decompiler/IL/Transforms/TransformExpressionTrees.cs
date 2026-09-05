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
		/// 
		/// call Lambda(&lt;body&gt;, &lt;parameter array&gt;)
		/// 
		/// where &lt;parameter array&gt; is either an empty parameter list (see
		/// <see cref="IsEmptyParameterList"/>) or a Block of kind ArrayInitializer.
		/// This is only a cheap pre-filter, the actual conversion is done by
		/// <see cref="ConvertLambda"/>.
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

		/// <summary>
		/// Matches the argument array of a call that has no arguments:
		/// call System.Array.Empty(), newarr System.Linq.Expressions.ParameterExpression(...)
		/// or newarr System.Linq.Expressions.Expression(...).
		/// The array length is not inspected for the two newarr forms.
		/// </summary>
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

		/// <summary>
		/// stloc v(call Parameter(call GetTypeFromHandle(ldtypetoken T), ldstr "name"))
		/// =&gt;
		/// true, with parameterReferenceVar = v, type = T and name = "name".
		/// 
		/// v must be a single-definition local or stack slot of type
		/// System.Linq.Expressions.ParameterExpression.
		/// </summary>
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

		/// <summary>
		/// Starting at pos, collects the leading run of lambda parameter declarations
		/// 
		/// stloc v(call Parameter(call GetTypeFromHandle(ldtypetoken T), ldstr "name"))
		/// 
		/// then tries to convert the first statement that is not such a declaration; see
		/// <see cref="TryConvertExpressionTree"/>. On success the parameter declarations
		/// consumed by the converted tree are removed from the block.
		/// </summary>
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

		/// <summary>
		/// Searches instruction for the first
		/// 
		/// call Lambda(&lt;body&gt;, &lt;parameter array&gt;)
		/// 
		/// and replaces it with the ILFunction built by <see cref="ConvertLambda"/>.
		/// Nested control-flow blocks are not searched. Returns true if a tree was converted.
		/// </summary>
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
		/// 
		/// call Lambda(&lt;body&gt;, Block (ArrayInitializer) { stobj System.Object(delayex.ldelema System.Object(ldloc S, ldc.i4 0), ldloc V_0), ... })
		/// =&gt;
		/// ILFunction(&lt;parameters&gt;) { BlockContainer { Block { leave (&lt;converted body&gt;) } } }
		/// 
		/// The parameters are read from the array initializer by <see cref="ReadParameters"/>.
		/// The call must return Expression&lt;TDelegate&gt;; the ILFunction gets
		/// DelegateType = TDelegate and kind ExpressionTree if TDelegate is itself an
		/// expression tree type, Delegate otherwise. The returned delegate does the actual
		/// building: nothing is mutated until it is invoked.
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

		/// <summary>
		/// call Quote(&lt;lambda&gt;)
		/// =&gt;
		/// &lt;converted lambda&gt;
		/// 
		/// An argument that is already an ILFunction is passed through unchanged. Otherwise
		/// the argument (typically a nested call Lambda(...)) is converted, and if that
		/// yields an ILFunction its DelegateType and kind are taken from the return type of
		/// the argument call; see <see cref="SetExpressionTreeFlag"/>.
		/// </summary>
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

		/// <summary>
		/// Sets DelegateType and Kind of lambda from the return type of call: a return type
		/// Expression&lt;TDelegate&gt; gives ILFunctionKind.ExpressionTree, any other type gives
		/// ILFunctionKind.Delegate.
		/// </summary>
		void SetExpressionTreeFlag(ILFunction lambda, CallInstruction call)
		{
			lambda.Kind = IsExpressionTree(call.Method.ReturnType) ? ILFunctionKind.ExpressionTree : ILFunctionKind.Delegate;
			lambda.DelegateType = call.Method.ReturnType;
		}

		/// <summary>
		/// Reads the lambda parameter list from the ParameterExpression[] argument of a
		/// call Lambda(...).
		/// 
		/// Block (ArrayInitializer) { stobj System.Object(delayex.ldelema System.Object(ldloc S, ldc.i4 i), ldloc V_i), ... }
		/// =&gt;
		/// one IParameter and one ILVariable of kind Parameter per element, using the type
		/// and name recorded for V_i by <see cref="MatchParameterVariableAssignment"/>.
		/// An empty parameter list (see <see cref="IsEmptyParameterList"/>) yields none.
		/// 
		/// Each ParameterExpression variable enters the mapping only once; its defining
		/// stloc is queued for removal.
		/// </summary>
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

		/// <summary>
		/// Converts one node of the expression tree into a Func&lt;ILInstruction&gt; building the
		/// equivalent ILAst, or null if the node cannot be converted:
		/// 
		/// call &lt;name&gt;(...) on System.Linq.Expressions.Expression =&gt; the result of the
		///   Convert* method for &lt;name&gt;, e.g. call Add(a, b) =&gt; binary.numeric.add(a, b).
		/// ILFunction (an already converted nested lambda) =&gt; the same function, with an
		///   expression tree DelegateType unwrapped to TDelegate and kind set to Delegate.
		/// ldloc v, v a ParameterExpression =&gt; ldloc/ldloca of the mapped parameter variable,
		///   or, for a not yet mapped parameter of an enclosing lambda,
		///   expression.tree.cast T(ldloc v), so conversion can continue.
		/// 
		/// If typeHint is given and the built instruction has a different stack type, it is
		/// wrapped in a conv to that stack type.
		/// </summary>
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

		/// <summary>
		/// Returns true for System.Linq.Expressions.Expression&lt;T&gt;.
		/// </summary>
		bool IsExpressionTree(IType delegateType) => delegateType is ParameterizedType pt
			&& pt.FullName == "System.Linq.Expressions.Expression"
			&& pt.TypeArguments.Count == 1;

		/// <summary>
		/// Returns T for System.Linq.Expressions.Expression&lt;T&gt;; any other type is returned unchanged.
		/// </summary>
		IType UnwrapExpressionTree(IType delegateType)
		{
			if (delegateType is ParameterizedType pt && pt.FullName == "System.Linq.Expressions.Expression" && pt.TypeArguments.Count == 1)
			{
				return pt.TypeArguments[0];
			}
			return delegateType;
		}

		/// <summary>
		/// call ArrayIndex(array, index)
		/// call ArrayIndex(array, argumentList)  // multi-dimensional arrays
		/// =&gt;
		/// ldobj T(delayex.ldelema T(array, indices))
		/// The element type T is taken from the inferred type of the converted array expression;
		/// conversion fails if that type is not an array type.
		/// </summary>
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

		/// <summary>
		/// call ArrayLength(array)
		/// =&gt;
		/// ldlen.i4(array)
		/// </summary>
		Func<ILInstruction> ConvertArrayLength(CallInstruction invocation)
		{
			if (invocation.Arguments.Count != 1)
				return null;
			var converted = ConvertInstruction(invocation.Arguments[0]);
			if (converted == null)
				return null;
			return () => new LdLen(StackType.I4, converted());
		}

		/// <summary>
		/// call Add(left, right)                                       // built-in operator
		/// call Add(left, right, MethodInfo)                           // user-defined operator
		/// call Add(left, right, ldc.i4 isLiftedToNull, MethodInfo)    // user-defined operator
		/// =&gt;
		/// binary.add.i4(left, right) | call op_Addition(left, right)
		/// The two-argument shape infers both operand types: decimal operands select the operator
		/// method named operatorName, everything else produces a BinaryNumericInstruction, lifted
		/// if either operand type is nullable. Shift operators require an Int32 right operand, all
		/// other operators require the two operand types to match. The four-argument shape lifts
		/// the given method if the left operand type is nullable.
		/// </summary>
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
				// call Add(left, right): built-in operator, or the operator method of decimal
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
							GetSignForOperator(op, isChecked == true, leftType),
							isLifted: NullableType.IsNullable(leftType) || NullableType.IsNullable(rightType));
					};
				// call Add(left, right, methodInfo): user-defined operator
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return null;
					return () => new Call((IMethod)method) {
						Arguments = { left(), right() }
					};
				// call Add(left, right, ldc.i4 liftToNull, methodInfo): the shape of the
				// comparison factories; no arithmetic or bitwise factory declares it
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

		/// <summary>
		/// call Bind(castclass System.Reflection.MethodInfo(call GetMethodFromHandle(ldmembertoken set_P)), value)
		/// call Bind(call GetFieldFromHandle(ldmembertoken F), value)
		/// =&gt;
		/// callvirt set_P(ldloc target, value)
		/// stobj T(delayex.ldflda F(ldloc target), value)
		/// The returned builder takes the variable holding the object being initialized.
		/// </summary>
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

		/// <summary>
		/// call Call(MethodInfo, argumentList)          // static method
		/// call Call(target, MethodInfo, argumentList)  // target is ldnull for static methods
		/// =&gt;
		/// call M(arguments) | callvirt M(target, arguments)
		///
		/// Method group conversion:
		/// call Call(call Constant(MethodInfo M, ...), MethodInfo MethodInfo.CreateDelegate, argumentList { call Constant(typeof(D), ...), targetObject })
		/// =&gt;
		/// newobj D..ctor(targetObject, ldftn M)
		///
		/// The argument list is normally a single array-initializer block; if it is not, the
		/// remaining arguments of the invocation are taken as the argument list directly.
		/// </summary>
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

		/// <summary>
		/// Adapts a converted call target to the 'this' argument expected by a call on
		/// expectedType: takes its address (ldloca or addressof) where a by-reference 'this' is
		/// required, and boxes it where a boxed value type is required. If exactly one of the
		/// expected type and the result is unknown, a conv to the other side's primitive type is
		/// inserted, so that missing references do not produce mismatched call arguments.
		/// </summary>
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

		/// <summary>
		/// Returns the value of call Constant(value, typeToken); any other instruction is returned unchanged.
		/// </summary>
		ILInstruction UnpackConstant(ILInstruction inst)
		{
			if (!(inst is CallInstruction call && call.Method.FullName == "System.Linq.Expressions.Expression.Constant" && call.Arguments.Count == 2))
				return inst;
			return call.Arguments[0];
		}

		/// <summary>
		/// Converts each argument using the corresponding parameter type of method as type hint.
		/// Returns null if any argument cannot be converted.
		/// </summary>
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

		/// <summary>
		/// call Convert(expr, call GetTypeFromHandle(ldtypetoken T))
		/// =&gt;
		/// expression.tree.cast T(expr)
		///
		/// call Convert(expr, call GetTypeFromHandle(ldtypetoken T), methodInfo)
		/// =&gt;
		/// call methodInfo(expr)
		///
		/// The three-argument overload carries the user-defined conversion operator, which
		/// includes the decimal conversions; it is lifted when the operand is Nullable&lt;T&gt;.
		/// A conversion from a small integer type to Int32 produces the operand unchanged,
		/// because such values already occupy an I4 stack slot.
		/// </summary>
		Func<ILInstruction> ConvertCast(CallInstruction invocation, bool isChecked)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			if (!MatchGetTypeFromHandle(invocation.Arguments[1], out var targetType))
				return null;
			var expr = ConvertInstruction(invocation.Arguments[0]);
			if (expr == null)
				return null;
			if (invocation.Arguments.Count == 3 && MatchGetMethodFromHandle(invocation.Arguments[2], out var conversionOperator))
			{
				var unliftedOperator = (IMethod)conversionOperator;
				return () => {
					var exprInst = expr();
					if (exprInst == null)
						return null;
					var op_Method = unliftedOperator;
					if (NullableType.IsNullable(exprInst.InferType(context.TypeSystem)))
					{
						op_Method = CSharpOperators.LiftUserDefinedOperator(unliftedOperator);
						if (op_Method == null)
							return new ExpressionTreeCast(targetType, exprInst, isChecked);
					}
					return new Call(op_Method) {
						Arguments = { exprInst }
					};
				};
			}
			return () => {
				var exprInst = expr();
				if (exprInst == null)
					return null;
				if (exprInst.InferType(context.TypeSystem).IsSmallIntegerType() && targetType.IsKnownType(KnownTypeCode.Int32))
					return exprInst;
				return new ExpressionTreeCast(targetType, exprInst, isChecked);
			};
		}

		/// <summary>
		/// call Coalesce(leftExpr, rightExpr)
		/// =&gt;
		/// if.notnull(left, right)
		/// The result type and NullCoalescingKind are picked from the inferred operand types: a
		/// nullable left whose underlying type the right operand implicitly converts to gives
		/// Nullable or NullableWithValueFallback, everything else gives Ref.
		/// The three-argument overload, which carries an explicit conversion lambda, is not matched.
		/// </summary>
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

		/// <summary>
		/// call Equal(left, right, ldc.i4 liftToNull, castclass System.Reflection.MethodInfo(call GetMethodFromHandle(ldmembertoken op_Equality)))
		/// =&gt;
		/// call op_Equality(left, right), lifted via LiftUserDefinedOperator when left is Nullable&lt;T&gt;
		/// call Equal(left, right)
		/// =&gt;
		/// call op_Equality(left, right) for a user-defined operator found by the resolver, or for two
		/// string operands; otherwise comp.i4(left == right), lifted[C#] when left is Nullable&lt;T&gt;.
		/// Equal stands for whichever factory kind selects: NotEqual, LessThan, GreaterThan, ...
		/// </summary>
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

		/// <summary>
		/// call Condition(conditionExpr, trueExpr, falseExpr)
		/// =&gt;
		/// if (condition) trueValue else falseValue
		/// The builder bails out unless the condition infers to bool and both branches infer to types
		/// that are equivalent under type erasure; the true branch's type becomes the result type.
		/// </summary>
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

		/// <summary>
		/// call Constant(box T(value), call GetTypeFromHandle(ldtypetoken T))
		/// =&gt;
		/// value, or expression.tree.cast T(value) when T is an enum or bool
		/// call Constant(ldstr "a" / ldnull / call GetTypeFromHandle(ldtypetoken X) / ldloc displayClass)
		/// =&gt;
		/// the reference itself; only value-type constants are boxed.
		/// Roslyn emits the two-argument Constant(object, Type) overload; the legacy .NET Framework
		/// csc uses the one-argument Constant(object) overload for display-class instances.
		/// </summary>
		Func<ILInstruction> ConvertConstant(CallInstruction invocation)
		{
			if (!MatchConstantCall(invocation, out var value))
				return null;
			if (value.MatchBox(out var arg, out var boxType))
			{
				if (boxType.Kind == TypeKind.Enum || boxType.IsKnownType(KnownTypeCode.Boolean))
					return () => new ExpressionTreeCast(boxType, ConvertValue(arg, invocation), false);
				return () => ConvertValue(arg, invocation);
			}
			return () => ConvertValue(value, invocation);

			static bool MatchConstantCall(ILInstruction inst, out ILInstruction value)
			{
				value = null;
				if (inst is CallInstruction call && call.Method.FullName == "System.Linq.Expressions.Expression.Constant")
				{
					value = call.Arguments[0];
					// The two-argument overload passes the constant's type as typeof(T);
					// legacy csc uses the one-argument overload for display-class instances.
					return call.Arguments.Count != 2 || MatchGetTypeFromHandle(call.Arguments[1], out _);
				}
				return false;
			}
		}

		/// <summary>
		/// call ElementInit(castclass System.Reflection.MethodInfo(call GetMethodFromHandle(ldmembertoken Add)),
		///                  block ArrayInitializer { newarr Expression + one stobj per argument })
		/// =&gt;
		/// callvirt Add(args), or call Add(args) for a static method, with no target argument yet;
		/// ConvertListInit inserts the collection instance at index 0.
		/// </summary>
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

		/// <summary>
		/// call Field(ldnull, call GetFieldFromHandle(ldmembertoken F))
		/// =&gt;
		/// ldobj T(ldsflda F)
		/// call Field(targetExpr, call GetFieldFromHandle(ldmembertoken F))
		/// =&gt;
		/// ldobj T(delayex.ldflda F(target)), with target wrapped in addressof when the declaring
		/// type is a value type.
		/// A by-ref typeHint on a field whose type is not by-ref-like drops the ldobj, so the field
		/// address itself is produced.
		/// </summary>
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

		/// <summary>
		/// call Invoke(targetExpr, block ArrayInitializer { newarr Expression + one stobj per argument })
		/// =&gt;
		/// callvirt Invoke(target, args)
		/// The invoke method comes from the delegate type the target infers to; the builder bails out
		/// if that type has none, or if an argument fails to convert.
		/// </summary>
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

		/// <summary>
		/// call ListInit(call New(...), block ArrayInitializer { call ElementInit(addMethod, args), ... })
		/// or, with the add-method handle passed separately:
		/// call ListInit(call New(...), addMethod, block ArrayInitializer { args })
		/// =>
		/// Block (CollectionInitializer) {
		/// 	stloc initializer(newobj ctor(...))
		/// 	callvirt Add(ldloc initializer, args)   // one per element
		/// 	final: ldloc initializer
		/// }
		/// </summary>
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

		/// <summary>
		/// call AndAlso(left, right) / call OrElse(left, right)
		/// =>
		/// if (left) right else ldc.i4 0 / if (left) ldc.i4 1 else right
		///
		/// call AndAlso(left, right, method)
		/// call AndAlso(left, right, ldc.i4 liftToNull, method)
		/// =>
		/// call method(left, right); the four-argument form lifts the user-defined operator
		/// if the left operand infers to Nullable&lt;T&gt;.
		/// </summary>
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
				// call AndAlso(left, right): built-in operator
				case 2:
					return () => and ? IfInstruction.LogicAnd(left(), right(), context.TypeSystem) : IfInstruction.LogicOr(left(), right(), context.TypeSystem);
				// call AndAlso(left, right, methodInfo): user-defined operator
				case 3:
					if (!MatchGetMethodFromHandle(invocation.Arguments[2], out method))
						return null;
					return () => new Call((IMethod)method) {
						Arguments = { left(), right() }
					};
				// call AndAlso(left, right, ldc.i4 liftToNull, methodInfo): AndAlso and OrElse
				// declare no such overload
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

		/// <summary>
		/// call MemberInit(call New(...), block ArrayInitializer { call Bind(member, value), ... })
		/// =>
		/// Block (CollectionInitializer) {
		/// 	stloc initializer(newobj ctor(...))
		/// 	callvirt set_Member(ldloc initializer, value)   // stobj for field bindings
		/// 	final: ldloc initializer
		/// }
		/// Only Expression.Bind elements are supported; any other binding kind fails the match.
		/// </summary>
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

				var initializerBlock = new Block(BlockKind.ObjectInitializer);
				initializerBlock.FinalInstruction = new LdLoc(initializer);
				initializerBlock.Instructions.Add(new StLoc(initializer, newObj()));
				initializerBlock.Instructions.AddRange(convertedArguments.Select(f => f(initializer)));

				return initializerBlock;
			}

			return BuildBlock;
		}

		/// <summary>
		/// call NewArrayBounds(call GetTypeFromHandle(ldtypetoken T), block ArrayInitializer { bounds })
		/// =>
		/// newarr T(bounds)
		/// </summary>
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

		/// <summary>
		/// call NewArrayInit(call GetTypeFromHandle(ldtypetoken T), block ArrayInitializer { values })
		/// =>
		/// Block (ArrayInitializer) {
		/// 	stloc initializer(newarr T(ldc.i4 n))
		/// 	stobj T(delayex.ldelema T(ldloc initializer, ldc.i4 i), value)   // one per element
		/// 	final: ldloc initializer
		/// }
		/// An empty value list produces a bare newarr T(ldc.i4 0) instead of a block.
		/// </summary>
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

		/// <summary>
		/// Matches the constructor named by a call to Expression.New; produces no ILAst.
		/// call New(call GetTypeFromHandle(ldtypetoken T)) -&gt; the parameterless constructor of T
		/// call New(ctorInfo)
		/// call New(ctorInfo, block ArrayInitializer { args })
		/// call New(ctorInfo, block ArrayInitializer { args }, block ArrayInitializer { members })
		/// -&gt; the constructor named by ctorInfo, which is
		/// castclass ConstructorInfo(call GetMethodFromHandle(ldmembertoken .ctor, ldtypetoken T)).
		/// </summary>
		bool MatchNew(CallInstruction invocation, out IMethod ctor)
		{
			ctor = null;
			if (invocation.Method.Name != "New")
				return false;
			switch (invocation.Arguments.Count)
			{
				// call New(typeHandle) or call New(constructorInfo)
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
				// call New(constructorInfo, argumentList[, memberList])
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

		/// <summary>
		/// call New(call GetTypeFromHandle(ldtypetoken T)) / call New(ctorInfo)
		/// => newobj ctor()
		/// call New(ctorInfo, block ArrayInitializer { args })
		/// => newobj ctor(args)
		/// call New(ctorInfo, block ArrayInitializer { args }, block ArrayInitializer { members })
		/// => newobj ctor(args); the member list, which names the anonymous type's property
		/// accessors, has no ILAst equivalent and is dropped.
		/// ctorInfo is castclass ConstructorInfo(call GetMethodFromHandle(ldmembertoken .ctor, ldtypetoken T)).
		/// </summary>
		Func<ILInstruction> ConvertNewObject(CallInstruction invocation)
		{
			switch (invocation.Arguments.Count)
			{
				// call New(typeHandle) or call New(constructorInfo): parameterless constructor
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
				// call New(constructorInfo, argumentList)
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
				// call New(constructorInfo, argumentList, memberList): anonymous types
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

		/// <summary>
		/// call Not(value) / call OnesComplement(value)
		/// =>
		/// logic.not(value) if value infers to bool, otherwise bit.not(value) on the
		/// underlying type's stack type; both are lifted if the inferred type is Nullable&lt;T&gt;.
		///
		/// call Not(value, castclass MethodInfo(call GetMethodFromHandle(ldmembertoken op_LogicalNot, ldtypetoken T)))
		/// =>
		/// call op_LogicalNot(value)
		/// </summary>
		Func<ILInstruction> ConvertNotOperator(CallInstruction invocation)
		{
			if (invocation.Arguments.Count < 1)
				return null;
			var argument = ConvertInstruction(invocation.Arguments[0]);
			if (argument == null)
				return null;
			switch (invocation.Arguments.Count)
			{
				// call Not(expression): built-in operator
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
				// call Not(expression, methodInfo): user-defined op_LogicalNot or op_OnesComplement
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

		/// <summary>
		/// call Property(target, castclass MethodInfo(call GetMethodFromHandle(ldmembertoken get_X, ldtypetoken T)))
		/// call Property(target, accessorInfo, block ArrayInitializer { indices })
		/// =>
		/// callvirt get_X(target, indices)
		/// A static accessor uses call instead of callvirt; ldnull as the first argument
		/// emits no target argument. The target is adapted to the accessor's this-pointer
		/// stack type (address-of or box for value types).
		/// </summary>
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

		/// <summary>
		/// call TypeAs(value, call GetTypeFromHandle(ldtypetoken T))
		/// =>
		/// isinst T(value)
		/// For T = Nullable&lt;U&gt; the result is wrapped in unbox.any T, because isinst on a
		/// nullable type tests for boxed U per ECMA-335, III.4.6.
		/// </summary>
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

		/// <summary>
		/// call TypeIs(value, call GetTypeFromHandle(ldtypetoken T))
		/// =>
		/// comp.obj(isinst T(value) != ldnull)
		/// </summary>
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

		/// <summary>
		/// call Negate(argumentExpr)
		/// =&gt;
		/// binary.sub.i4(ldc.i4 0, argument)
		///
		/// The built-in form has no MethodInfo: the operation is expressed as a binary
		/// instruction with a zero literal on the left. The literal is picked in the returned
		/// builder from the stack type inferred for the converted argument: ldc.i4 0 for I4,
		/// ldc.i8 0 for I8, conv i4-&gt;i for I, ldc.f4/ldc.f8 0 for F4/F8 and ldc.decimal 0
		/// for System.Decimal; any other stack type is rejected. A nullable argument type
		/// produces a lifted instruction over the underlying type.
		///
		/// call Negate(argumentExpr, castclass System.Reflection.MethodInfo(call GetMethodFromHandle(ldmembertoken op_UnaryNegation)))
		/// =&gt;
		/// call op_UnaryNegation(argument)
		/// </summary>
		Func<ILInstruction> ConvertUnaryNumericOperator(CallInstruction invocation, BinaryNumericOperator op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 1)
				return null;
			var argument = ConvertInstruction(invocation.Arguments[0]);
			if (argument == null)
				return null;
			switch (invocation.Arguments.Count)
			{
				// call Negate(expression): built-in operator
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
							GetSignForOperator(op, isChecked == true, argumentType),
							isLifted: NullableType.IsNullable(argumentType));
					};
				// call Negate(expression, methodInfo): user-defined op_UnaryNegation
				case 2:
					if (!MatchGetMethodFromHandle(invocation.Arguments[1], out var method))
						return null;
					return () => new Call((IMethod)method) {
						Arguments = { argument() }
					};
			}
			return null;
		}

		/// <summary>
		/// The sign is part of the IL opcode only where it changes the operation: for the
		/// checked add/sub/mul (add.ovf vs add.ovf.un) and for div/rem/shr. Everywhere else
		/// ILReader leaves it at Sign.None, so a converted expression tree must do the same
		/// to produce the same ILAst as the equivalent lambda.
		/// </summary>
		static Sign GetSignForOperator(BinaryNumericOperator op, bool isChecked, IType type)
		{
			switch (op)
			{
				case BinaryNumericOperator.Div:
				case BinaryNumericOperator.Rem:
				case BinaryNumericOperator.ShiftRight:
					return type.GetSign();
				case BinaryNumericOperator.Add:
				case BinaryNumericOperator.Sub:
				case BinaryNumericOperator.Mul:
					return isChecked ? type.GetSign() : Sign.None;
				default:
					return Sign.None;
			}
		}

		/// <summary>
		/// Post-processes the value operand of a converted Expression.Constant call;
		/// <paramref name="context"/> is the surrounding Expression call instruction.
		/// A ldloc of an expression-tree ParameterExpression variable is mapped to the
		/// ILVariable generated for that parameter, but only where a constant may legally
		/// stand in for it: under Expression.Call with an integer stack type it becomes
		/// ldloca of the mapped variable, an unmapped variable is cloned unchanged, and any
		/// other mapped use is rejected (null).
		/// A ldloc of a closure reference is returned as is, after marking the variable as
		/// a display-class local and registering it as a captured variable of the enclosing
		/// ILFunction. Everything else is cloned.
		/// </summary>
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

		/// <summary>
		/// Whether the variable has a single store of the form stloc v(newobj DisplayClass..ctor())
		/// that TransformDisplayClassUsage recognizes as a potential closure.
		/// </summary>
		bool IsClosureReference(ILVariable variable)
		{
			if (!variable.IsSingleDefinition || !(variable.StoreInstructions.SingleOrDefault() is StLoc store))
				return false;
			if (!(store.Value is NewObj newObj))
				return false;
			return TransformDisplayClassUsage.IsPotentialClosure(this.context, newObj);
		}

		/// <summary>
		/// Whether the variable holds a System.Linq.Expressions.ParameterExpression.
		/// </summary>
		bool IsExpressionTreeParameter(ILVariable variable)
		{
			return variable.Type.FullName == "System.Linq.Expressions.ParameterExpression";
		}

		/// <summary>
		/// call GetTypeFromHandle(ldtypetoken T)
		/// Hands back T.
		/// </summary>
		internal static bool MatchGetTypeFromHandle(ILInstruction inst, out IType type)
		{
			type = null;
			return inst is CallInstruction getTypeCall
				&& getTypeCall.Method.FullName == "System.Type.GetTypeFromHandle"
				&& getTypeCall.Arguments.Count == 1
				&& getTypeCall.Arguments[0].MatchLdTypeToken(out type);
		}

		/// <summary>
		/// castclass System.Reflection.MethodInfo(call GetMethodFromHandle(ldmembertoken M))
		/// Hands back the method M; see MatchFromHandleParameterList for the accepted
		/// argument lists of the GetMethodFromHandle call.
		/// </summary>
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

		/// <summary>
		/// castclass System.Reflection.ConstructorInfo(call GetMethodFromHandle(ldmembertoken C))
		/// Hands back the constructor C; see MatchFromHandleParameterList for the accepted
		/// argument lists of the GetMethodFromHandle call.
		/// </summary>
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

		/// <summary>
		/// call GetFieldFromHandle(ldmembertoken F)
		/// Hands back the field F; see MatchFromHandleParameterList for the accepted
		/// argument lists of the call.
		/// </summary>
		bool MatchGetFieldFromHandle(ILInstruction inst, out IMember member)
		{
			member = null;
			if (!(inst is CallInstruction call && call.Method.FullName == "System.Reflection.FieldInfo.GetFieldFromHandle"))
				return false;
			return MatchFromHandleParameterList(call, out member);
		}

		/// <summary>
		/// Accepts the argument list of a GetMethodFromHandle/GetFieldFromHandle call in both
		/// its overloads: (ldmembertoken M), and (ldmembertoken M, ldtypetoken T) for a member
		/// of a generic type. Hands back M; the declaring-type token is only checked for shape,
		/// because the member token already carries the specialized member.
		/// </summary>
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

		/// <summary>
		/// Block (ArrayInitializer) {
		///		stloc S(newarr T(ldc.i4 n))
		///		stobj T(ldelema T(ldloc S, ldc.i4 0), value0)
		///		...
		///		stobj T(ldelema T(ldloc S, ldc.i4 n-1), value_n-1)
		///		final: ldloc S
		/// }
		/// Hands back the element values in index order; the indices must be the dense
		/// sequence 0..n-1. An empty list is also matched outside a block, as
		/// newarr ParameterExpression/Expression(ldc.i4 0) or call Array.Empty().
		/// </summary>
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
