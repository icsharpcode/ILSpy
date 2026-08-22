// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Replaces method calls with the appropriate operator expressions.
	/// </summary>
	public class ReplaceMethodCallsWithOperators : DepthFirstAstVisitor, IAstTransform
	{
		static readonly MemberReferenceExpression typeHandleOnTypeOfPattern = new MemberReferenceExpression {
			Target = new Choice {
				new TypeOfExpression(new AnyNode()),
				new UndocumentedExpression { UndocumentedExpressionType = UndocumentedExpressionType.RefType, Arguments = { new AnyNode() } }
			},
			MemberName = "TypeHandle"
		};

		[AllowNull]
		TransformContext context;

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression(invocationExpression);
			ProcessInvocationExpression(invocationExpression);
		}

		void ProcessInvocationExpression(InvocationExpression invocationExpression)
		{
			var method = invocationExpression.GetSymbol() as IMethod;
			if (method == null)
				return;
			var arguments = invocationExpression.Arguments.ToArray();

			// Reduce "String.Concat(a, b)" to "a + b"
			if (IsStringConcat(method) && context.Settings.StringConcat)
			{
				if (arguments is [ArrayCreateExpression { Initializer: { } aceInitializer }] && method.Parameters is [{ Type: ArrayType }])
				{
					arguments = aceInitializer.Elements.ToArray();
				}

				if (!CheckArgumentsForStringConcat(arguments))
				{
					return;
				}

				bool isInExpressionTree = invocationExpression.Ancestors.OfType<LambdaExpression>().Any(
					lambda => lambda.Annotation<IL.ILFunction>()?.Kind == IL.ILFunctionKind.ExpressionTree);
				context.Step("Replace String.Concat with +", invocationExpression);
				Expression arg0 = arguments[0].Detach();
				Expression arg1 = arguments[1].Detach();
				if (!isInExpressionTree)
				{
					arg1 = RemoveRedundantToStringInConcat(arg1, method, isLastArgument: arguments.Length == 2).Detach();
					if (arg1.GetResolveResult().Type.IsKnownType(KnownTypeCode.String))
					{
						arg0 = RemoveRedundantToStringInConcat(arg0, method, isLastArgument: false).Detach();
					}
				}
				var expr = new BinaryOperatorExpression(arg0, BinaryOperatorType.Add, arg1);
				for (int i = 2; i < arguments.Length; i++)
				{
					var arg = arguments[i].Detach();
					if (!isInExpressionTree)
					{
						arg = RemoveRedundantToStringInConcat(arg, method, isLastArgument: i == arguments.Length - 1).Detach();
					}
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.Add, arg);
				}
				expr.CopyAnnotationsFrom(invocationExpression);
				invocationExpression.ReplaceWith(expr);
				context.EndStep(expr);
				return;
			}

			switch (method.FullName)
			{
				case "System.Type.GetTypeFromHandle":
					if (arguments.Length == 1)
					{
						if (typeHandleOnTypeOfPattern.IsMatch(arguments[0]))
						{
							context.Step("Replace GetTypeFromHandle with typeof", invocationExpression);
							Expression target = ((MemberReferenceExpression)arguments[0]).Target;
							target.CopyInstructionsFrom(invocationExpression);
							invocationExpression.ReplaceWith(target);
							context.EndStep(target);
							return;
						}
					}
					break;
				/*
			case "System.Reflection.FieldInfo.GetFieldFromHandle":
				// TODO : This is dead code because LdTokenAnnotation is not added anywhere:
				if (arguments.Length == 1) {
					MemberReferenceExpression mre = arguments[0] as MemberReferenceExpression;
					if (mre != null && mre.MemberName == "FieldHandle" && mre.Target.Annotation<LdTokenAnnotation>() != null) {
						invocationExpression.ReplaceWith(mre.Target);
						return;
					}
				} else if (arguments.Length == 2) {
					MemberReferenceExpression mre1 = arguments[0] as MemberReferenceExpression;
					MemberReferenceExpression mre2 = arguments[1] as MemberReferenceExpression;
					if (mre1 != null && mre1.MemberName == "FieldHandle" && mre1.Target.Annotation<LdTokenAnnotation>() != null) {
						if (mre2 != null && mre2.MemberName == "TypeHandle" && mre2.Target is TypeOfExpression) {
							Expression oldArg = ((InvocationExpression)mre1.Target).Arguments.Single();
							FieldReference field = oldArg.Annotation<FieldReference>();
							if (field != null) {
								AstType declaringType = ((TypeOfExpression)mre2.Target).Type.Detach();
								oldArg.ReplaceWith(new MemberReferenceExpression(new TypeReferenceExpression(declaringType), field.Name).CopyAnnotationsFrom(oldArg));
								invocationExpression.ReplaceWith(mre1.Target);
								return;
							}
						}
					}
				}
				break;
				*/
				case "System.Activator.CreateInstance":
					if (context.Settings.UseObjectCreationOfGenericTypeParameter &&
						arguments.Length == 0 &&
						method.TypeArguments.Count == 1 &&
						IsInstantiableTypeParameter(method.TypeArguments[0]))
					{
						context.Step("Replace Activator.CreateInstance with new", invocationExpression);
						var objectCreate = new ObjectCreateExpression(context.TypeSystemAstBuilder.ConvertType(method.TypeArguments.First()));
						invocationExpression.ReplaceWith(objectCreate);
						context.EndStep(objectCreate);
					}
					break;
				case "System.Runtime.CompilerServices.RuntimeHelpers.GetSubArray":
					if (arguments.Length == 2 && context.Settings.Ranges)
					{
						context.Step("Replace RuntimeHelpers.GetSubArray with range indexer", invocationExpression);
						var slicing = new IndexerExpression(arguments[0].Detach(), arguments[1].Detach());
						slicing.CopyAnnotationsFrom(invocationExpression);
						invocationExpression.ReplaceWith(slicing);
						context.EndStep(slicing);
					}
					break;
			}

			bool isChecked;
			BinaryOperatorType? bop = GetBinaryOperatorTypeFromMetadataName(method.Name, out isChecked, context.Settings);
			if (bop != null && arguments.Length == 2)
			{
				context.Step("Replace operator method with binary operator", invocationExpression);
				invocationExpression.Arguments.Clear(); // detach arguments from invocationExpression
				if (isChecked)
				{
					invocationExpression.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
				}
				else if (HasCheckedEquivalent(method))
				{
					invocationExpression.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
				}
				var binaryOperator = new BinaryOperatorExpression(
					arguments[0].UnwrapInDirectionExpression(),
					bop.Value,
					arguments[1].UnwrapInDirectionExpression()
				).CopyAnnotationsFrom(invocationExpression);
				invocationExpression.ReplaceWith(binaryOperator);
				context.EndStep(binaryOperator);
				return;
			}
			// Accessibility is part of whether "x op= y" can bind here at all: C# requires a
			// user-defined operator to be public, so a call to one that is not has to stay a call.
			if (context.Settings.UserDefinedCompoundAssignmentOperators
				&& method is { IsOperator: true, IsStatic: false, Accessibility: Accessibility.Public }
				&& invocationExpression.Target is MemberReferenceExpression or PointerReferenceExpression)
			{
				// A pointer indirection is always an assignable variable; every other receiver
				// has to pass the target checks.
				bool receiverIsPointer = invocationExpression.Target is PointerReferenceExpression;
				Expression receiver = invocationExpression.Target switch {
					MemberReferenceExpression mre => mre.Target,
					_ => ((PointerReferenceExpression)invocationExpression.Target).Target,
				};
				if (!receiverIsPointer && (!IsValidAssignmentTarget(receiver) || !IsAssignableTarget(receiver)))
					return;
				if (!receiverIsPointer && method.DeclaringType.Kind == TypeKind.Interface
					&& receiver.GetResolveResult().Type.Kind is not (TypeKind.Interface or TypeKind.TypeParameter))
				{
					// An operator declared in an interface (an explicit implementation, say)
					// can only be bound through a receiver of interface or type-parameter type.
					return;
				}
				Expression MakeAssignmentTarget()
				{
					Expression target = receiver.Detach();
					return receiverIsPointer
						? new UnaryOperatorExpression(UnaryOperatorType.Dereference, target)
						: target;
				}
				AssignmentOperatorType? aop = GetCompoundAssignmentOperatorTypeFromMetadataName(method.Name, out isChecked, context.Settings);
				if (aop != null && arguments.Length == 1)
				{
					// "x op= y" takes its operator from the static type of x and the type of y; it
					// has no way to select an overload by parameter modifier, so a call the form
					// would bind differently (an "in" overload beside an applicable by-value one)
					// has to stay a call.
					Expression value = arguments[0] is DirectionExpression direction ? direction.Expression : arguments[0];
					IType receiverType = receiverIsPointer
						? ((PointerType)receiver.GetResolveResult().Type).ElementType
						: receiver.GetResolveResult().Type;
					if (CSharpResolver.WouldRebindOperator(method, receiverType, [value.GetResolveResult()], context.TypeSystem))
						return;
					context.Step("Replace instance operator method with compound assignment", invocationExpression);
					if (isChecked)
					{
						invocationExpression.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
					}
					else if (HasCheckedEquivalent(method))
					{
						invocationExpression.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
					}
					var assignment = new AssignmentExpression(
						MakeAssignmentTarget(),
						aop.Value,
						arguments[0].Detach().UnwrapInDirectionExpression()
					).CopyAnnotationsFrom(invocationExpression);
					invocationExpression.ReplaceWith(assignment);
					context.EndStep(assignment);
					return;
				}
				UnaryOperatorType? incDecOp = method.Name switch {
					"op_IncrementAssignment" => UnaryOperatorType.PostIncrement,
					"op_DecrementAssignment" => UnaryOperatorType.PostDecrement,
					"op_CheckedIncrementAssignment" when context.Settings.CheckedOperators => UnaryOperatorType.PostIncrement,
					"op_CheckedDecrementAssignment" when context.Settings.CheckedOperators => UnaryOperatorType.PostDecrement,
					_ => null,
				};
				if (incDecOp != null && arguments.Length == 0)
				{
					context.Step("Replace instance operator method with increment/decrement", invocationExpression);
					if (method.Name is "op_CheckedIncrementAssignment" or "op_CheckedDecrementAssignment")
					{
						invocationExpression.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
					}
					else if (HasCheckedEquivalent(method))
					{
						invocationExpression.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
					}
					var incDec = new UnaryOperatorExpression(incDecOp.Value, MakeAssignmentTarget())
						.CopyAnnotationsFrom(invocationExpression);
					invocationExpression.ReplaceWith(incDec);
					context.EndStep(incDec);
					return;
				}
			}
			UnaryOperatorType? uop = GetUnaryOperatorTypeFromMetadataName(method.Name, out isChecked, context.Settings);
			if (uop != null && arguments.Length == 1)
			{
				if (isChecked)
				{
					invocationExpression.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
				}
				else if (HasCheckedEquivalent(method))
				{
					invocationExpression.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
				}
				if (uop == UnaryOperatorType.Increment || uop == UnaryOperatorType.Decrement)
				{
					// `op_Increment(a)` is not equivalent to `++a`,
					// because it doesn't assign the incremented value to a.
					if (method.DeclaringType.IsKnownType(KnownTypeCode.Decimal))
					{
						context.Step("Replace decimal increment method with arithmetic", invocationExpression);
						// Legacy csc optimizes "d + 1m" to "op_Increment(d)",
						// so reverse that optimization here:
						var arithmetic = new BinaryOperatorExpression(
							arguments[0].UnwrapInDirectionExpression().Detach(),
							(uop == UnaryOperatorType.Increment ? BinaryOperatorType.Add : BinaryOperatorType.Subtract),
							new PrimitiveExpression(1m)
						).CopyAnnotationsFrom(invocationExpression);
						invocationExpression.ReplaceWith(arithmetic);
						context.EndStep(arithmetic);
					}
				}
				else
				{
					context.Step("Replace operator method with unary operator", invocationExpression);
					arguments[0].Remove(); // detach argument
					var unaryOperator = new UnaryOperatorExpression(uop.Value, arguments[0].UnwrapInDirectionExpression()).CopyAnnotationsFrom(invocationExpression);
					invocationExpression.ReplaceWith(unaryOperator);
					context.EndStep(unaryOperator);
				}
				return;
			}
			if (method.Name is "op_Explicit" or "op_CheckedExplicit" && arguments.Length == 1)
			{
				context.Step("Replace conversion operator method with cast", invocationExpression);
				arguments[0].Remove(); // detach argument
				if (method.Name == "op_CheckedExplicit")
				{
					invocationExpression.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
				}
				else if (HasCheckedEquivalent(method))
				{
					invocationExpression.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
				}
				var cast = new CastExpression(context.TypeSystemAstBuilder.ConvertType(method.ReturnType), arguments[0].UnwrapInDirectionExpression())
					.CopyAnnotationsFrom(invocationExpression);
				invocationExpression.ReplaceWith(cast);
				context.EndStep(cast);
				return;
			}
			if (method.Name == "op_True" && arguments.Length == 1 && invocationExpression.Slot?.Kind == Slots.Condition)
			{
				context.Step("Remove op_True from condition", invocationExpression);
				var condition = arguments[0].UnwrapInDirectionExpression();
				invocationExpression.ReplaceWith(condition);
				context.EndStep(condition);
				return;
			}

			return;
		}

		internal static bool HasCheckedEquivalent(IMethod method)
		{
			string name = method.Name;
			if (name.StartsWith("op_", StringComparison.Ordinal))
				name = "op_Checked" + name.Substring(3);
			return method.DeclaringType.GetMethods(m => m.IsOperator && m.Name == name).Any();
		}

		bool IsInstantiableTypeParameter(IType type)
		{
			return type is ITypeParameter tp && tp.HasDefaultConstructorConstraint;
		}

		bool CheckArgumentsForStringConcat(Expression[] arguments)
		{
			if (arguments.Length < 2)
				return false;

			if (arguments.Any(arg => arg is NamedArgumentExpression))
				return false;

			// The evaluation order when the object.ToString() calls happen is a mess:
			// The C# spec says the evaluation for order for each individual string + should be:
			//   * evaluate left argument
			//   * evaluate right argument
			//   * call ToString() on object argument
			// What actually happens pre-VS2019.3:
			//   * evaluate all arguments in chain of + operators from left to right
			//   * call ToString() on all object arguments from left to right
			// What happens in VS2019.3:
			//   * for each argument in chain of + operators fom left to right:
			//       * evaluate argument
			//       * call ToString() on object argument
			// See https://github.com/dotnet/roslyn/issues/38641 for details.
			// To ensure the decompiled code's behavior matches the original IL behavior,
			// no matter which compiler is used to recompile it, we require that all
			// implicit ToString() calls except for the last are free of side effects.
			foreach (var arg in arguments.SkipLast(1))
			{
				if (!ToStringIsKnownEffectFree(arg.GetResolveResult().Type))
				{
					return false;
				}
			}
			foreach (var arg in arguments)
			{
				var rr = arg.GetResolveResult();
				if (rr is InvocationResolveResult irr && IsStringConcat(irr.Member))
				{
					// Roslyn + mcs also flatten nested string.Concat() invocations within a operator+ use,
					// which causes it to use the incorrect evaluation order despite the code using an
					// explicit string.Concat() call.
					// This problem is avoided if the outer call remains string.Concat() as well.
					return false;
				}
				if (rr.Type.IsByRefLike)
				{
					// ref structs cannot be converted to object for use with +
					return false;
				}
			}

			// One of the first two arguments must be string, otherwise the + operator
			// won't resolve to a string concatenation.
			return arguments[0].GetResolveResult().Type.IsKnownType(KnownTypeCode.String)
				|| arguments[1].GetResolveResult().Type.IsKnownType(KnownTypeCode.String);
		}

		private bool IsStringConcat(IParameterizedMember member)
		{
			return member is IMethod method
				&& method.Name == "Concat"
				&& method.DeclaringType.IsKnownType(KnownTypeCode.String);
		}

		static readonly Pattern ToStringCallPattern = new Choice {
			// target.ToString()
			new InvocationExpression(new MemberReferenceExpression(new AnyNode("target"), "ToString")).WithName("call"),
			// target?.ToString()
			new UnaryOperatorExpression(
				UnaryOperatorType.NullConditionalRewrap,
				new InvocationExpression(
					new MemberReferenceExpression(
						new UnaryOperatorExpression(UnaryOperatorType.NullConditional, new AnyNode("target")),
						"ToString")
				).WithName("call")
			).WithName("nullConditional")
		};

		internal static Expression RemoveRedundantToStringInConcat(Expression expr, IMethod concatMethod, bool isLastArgument)
		{
			var m = ToStringCallPattern.Match(expr);
			if (!m.Success)
				return expr;

			if (!concatMethod.Parameters.All(IsStringParameter))
			{
				// If we're using a string.Concat() overload involving object parameters,
				// string.Concat() itself already calls ToString() so the C# compiler shouldn't
				// generate additional ToString() calls in this case.
				return expr;
			}
			var toStringMethod = m.Get<Expression>("call").Single().GetSymbol() as IMethod;
			var target = m.Get<Expression>("target").Single();
			var type = target.GetResolveResult().Type;
			if (type.IsByRefLike)
			{
				// ref structs cannot be converted to object for use with +
				return expr;
			}
			if (!(isLastArgument || ToStringIsKnownEffectFree(type)))
			{
				// ToString() order of evaluation matters, see CheckArgumentsForStringConcat().
				return expr;
			}
			if (type.IsReferenceType != false && !m.Has("nullConditional"))
			{
				// ToString() might throw NullReferenceException, but the builtin operator+ doesn't.
				return expr;
			}
			if (!ToStringIsKnownEffectFree(type) && toStringMethod != null && IL.Transforms.ILInlining.MethodRequiresCopyForReadonlyLValue(toStringMethod))
			{
				// ToString() on a struct may mutate the struct.
				// For operator+ the C# compiler creates a temporary copy before implicitly calling ToString(),
				// whereas an explicit ToString() call would mutate the original lvalue.
				// So we can't remove the compiler-generated ToString() call in cases where this might make a difference.
				return expr;
			}

			// All checks succeeded, we can eliminate the ToString() call.
			// The C# compiler will generate an equivalent call if the code is recompiled.
			return target;

			bool IsStringParameter(IParameter p)
			{
				IType ty = p.Type;
				if (p.IsParams && ty.Kind == TypeKind.Array)
					ty = ((ArrayType)ty).ElementType;
				return ty.IsKnownType(KnownTypeCode.String);
			}
		}

		static bool ToStringIsKnownEffectFree(IType type)
		{
			type = NullableType.GetUnderlyingType(type);
			switch (type.GetDefinition()?.KnownTypeCode)
			{
				case KnownTypeCode.Boolean:
				case KnownTypeCode.Char:
				case KnownTypeCode.SByte:
				case KnownTypeCode.Byte:
				case KnownTypeCode.Int16:
				case KnownTypeCode.UInt16:
				case KnownTypeCode.Int32:
				case KnownTypeCode.UInt32:
				case KnownTypeCode.Int64:
				case KnownTypeCode.UInt64:
				case KnownTypeCode.Single:
				case KnownTypeCode.Double:
				case KnownTypeCode.Decimal:
				case KnownTypeCode.IntPtr:
				case KnownTypeCode.UIntPtr:
				case KnownTypeCode.String:
					return true;
				default:
					return false;
			}
		}

		static BinaryOperatorType? GetBinaryOperatorTypeFromMetadataName(string name, out bool isChecked, DecompilerSettings settings)
		{
			isChecked = false;
			switch (name)
			{
				case "op_Addition":
					return BinaryOperatorType.Add;
				case "op_Subtraction":
					return BinaryOperatorType.Subtract;
				case "op_Multiply":
					return BinaryOperatorType.Multiply;
				case "op_Division":
					return BinaryOperatorType.Divide;
				case "op_CheckedAddition" when settings.CheckedOperators:
					isChecked = true;
					return BinaryOperatorType.Add;
				case "op_CheckedSubtraction" when settings.CheckedOperators:
					isChecked = true;
					return BinaryOperatorType.Subtract;
				case "op_CheckedMultiply" when settings.CheckedOperators:
					isChecked = true;
					return BinaryOperatorType.Multiply;
				case "op_CheckedDivision" when settings.CheckedOperators:
					isChecked = true;
					return BinaryOperatorType.Divide;
				case "op_Modulus":
					return BinaryOperatorType.Modulus;
				case "op_BitwiseAnd":
					return BinaryOperatorType.BitwiseAnd;
				case "op_BitwiseOr":
					return BinaryOperatorType.BitwiseOr;
				case "op_ExclusiveOr":
					return BinaryOperatorType.ExclusiveOr;
				case "op_LeftShift":
					return BinaryOperatorType.ShiftLeft;
				case "op_RightShift":
					return BinaryOperatorType.ShiftRight;
				case "op_UnsignedRightShift" when settings.UnsignedRightShift:
					return BinaryOperatorType.UnsignedShiftRight;
				case "op_Equality":
					return BinaryOperatorType.Equality;
				case "op_Inequality":
					return BinaryOperatorType.InEquality;
				case "op_LessThan":
					return BinaryOperatorType.LessThan;
				case "op_LessThanOrEqual":
					return BinaryOperatorType.LessThanOrEqual;
				case "op_GreaterThan":
					return BinaryOperatorType.GreaterThan;
				case "op_GreaterThanOrEqual":
					return BinaryOperatorType.GreaterThanOrEqual;
				default:
					return null;
			}
		}

		static UnaryOperatorType? GetUnaryOperatorTypeFromMetadataName(string name, out bool isChecked, DecompilerSettings settings)
		{
			isChecked = false;
			switch (name)
			{
				case "op_LogicalNot":
					return UnaryOperatorType.Not;
				case "op_OnesComplement":
					return UnaryOperatorType.BitNot;
				case "op_UnaryNegation":
					return UnaryOperatorType.Minus;
				case "op_CheckedUnaryNegation" when settings.CheckedOperators:
					isChecked = true;
					return UnaryOperatorType.Minus;
				case "op_UnaryPlus":
					return UnaryOperatorType.Plus;
				case "op_Increment":
					return UnaryOperatorType.Increment;
				case "op_Decrement":
					return UnaryOperatorType.Decrement;
				case "op_CheckedIncrement" when settings.CheckedOperators:
					isChecked = true;
					return UnaryOperatorType.Increment;
				case "op_CheckedDecrement" when settings.CheckedOperators:
					isChecked = true;
					return UnaryOperatorType.Decrement;
				default:
					return null;
			}
		}

		static AssignmentOperatorType? GetCompoundAssignmentOperatorTypeFromMetadataName(string name, out bool isChecked, DecompilerSettings settings)
		{
			isChecked = false;
			switch (name)
			{
				case "op_AdditionAssignment":
					return AssignmentOperatorType.Add;
				case "op_CheckedAdditionAssignment" when settings.CheckedOperators:
					isChecked = true;
					return AssignmentOperatorType.Add;
				case "op_SubtractionAssignment":
					return AssignmentOperatorType.Subtract;
				case "op_CheckedSubtractionAssignment" when settings.CheckedOperators:
					isChecked = true;
					return AssignmentOperatorType.Subtract;
				case "op_MultiplicationAssignment":
					return AssignmentOperatorType.Multiply;
				case "op_CheckedMultiplicationAssignment" when settings.CheckedOperators:
					isChecked = true;
					return AssignmentOperatorType.Multiply;
				case "op_DivisionAssignment":
					return AssignmentOperatorType.Divide;
				case "op_CheckedDivisionAssignment" when settings.CheckedOperators:
					isChecked = true;
					return AssignmentOperatorType.Divide;
				case "op_ModulusAssignment":
					return AssignmentOperatorType.Modulus;
				case "op_BitwiseAndAssignment":
					return AssignmentOperatorType.BitwiseAnd;
				case "op_BitwiseOrAssignment":
					return AssignmentOperatorType.BitwiseOr;
				case "op_ExclusiveOrAssignment":
					return AssignmentOperatorType.ExclusiveOr;
				case "op_LeftShiftAssignment":
					return AssignmentOperatorType.ShiftLeft;
				case "op_RightShiftAssignment":
					return AssignmentOperatorType.ShiftRight;
				case "op_UnsignedRightShiftAssignment" when settings.UnsignedRightShift:
					return AssignmentOperatorType.UnsignedShiftRight;
				default:
					return null;
			}
		}

		/// <summary>
		/// Gets whether the expression is classified as a variable, the only thing a user-defined
		/// compound assignment operator can be applied to. Instance operator calls carry their
		/// receiver as the call target, which is under no such restriction: properties and indexers
		/// route through the static operator instead, and hand-written IL can call the operator on
		/// any value at all.
		/// </summary>
		static bool IsValidAssignmentTarget(Expression expression)
		{
			if (IsWritableRefReturn(expression))
				return true;
			return expression switch {
				BaseReferenceExpression => false,
				// A pointer indirection is a variable.
				UnaryOperatorExpression { Operator: UnaryOperatorType.Dereference } => true,
				IndexerExpression => expression.GetSymbol() is not IProperty,
				_ => expression.GetResolveResult() switch {
					ILVariableResolveResult => true,
					MemberResolveResult mrr => mrr.Member is IField,
					_ => false,
				}
			};
		}

		/// <summary>
		/// Gets whether the expression is a variable C# accepts as the target of "x op= y". The
		/// target has to be assignable even though an instance operator never assigns to it:
		/// "this" in a class, foreach, using and fixed variables, "in" parameters and readonly
		/// fields outside the matching constructor are all rejected by the compiler.
		/// </summary>
		static bool IsAssignableTarget(Expression expression)
		{
			if (IsWritableRefReturn(expression))
				return true;
			if (expression is UnaryOperatorExpression { Operator: UnaryOperatorType.Dereference })
			{
				// A pointer indirection is never read-only.
				return true;
			}
			if (expression is IndexerExpression)
			{
				// An array element (property indexers never get this far).
				return true;
			}
			switch (expression.GetResolveResult())
			{
				case ThisResolveResult thisResolveResult:
					// "this" is an assignable variable only in a struct, and only where the
					// method does not make it read-only.
					return thisResolveResult.Type.IsReferenceType != true
						&& !thisResolveResult.Variable.IsRefReadOnly;
				case ILVariableResolveResult vrr:
					return !vrr.Variable.IsRefReadOnly
						&& vrr.Variable.Kind is not (IL.VariableKind.ForeachLocal or IL.VariableKind.UsingLocal or IL.VariableKind.PinnedLocal);
				case MemberResolveResult mrr when mrr.Member is IField field:
					if (!field.IsReadOnly)
						return true;
					// A readonly field is a variable inside the matching constructor of its
					// declaring type; an instance field additionally has to be accessed
					// through "this".
					return GetEnclosingMember(expression) is IMethod { IsConstructor: true } ctor
						&& ctor.IsStatic == field.IsStatic
						&& field.DeclaringTypeDefinition == ctor.DeclaringTypeDefinition
						&& (field.IsStatic || mrr.TargetResult is ThisResolveResult);
				default:
					return false;
			}
		}

		/// <summary>
		/// Gets whether the expression denotes a writable ref-returning invocation or member
		/// access. C# classifies those as variables, so they can take the place of x in
		/// "x op= y"; a "ref readonly" return stays a read-only variable.
		/// </summary>
		static bool IsWritableRefReturn(Expression expression)
		{
			return expression.GetSymbol() is IMember { ReturnType: ByReferenceType } member
				&& member switch {
					IMethod method => !method.ReturnTypeIsRefReadOnly,
					IProperty property => !property.ReturnTypeIsRefReadOnly,
					_ => false,
				};
		}

		static IMember? GetEnclosingMember(Expression expression)
		{
			return expression.Ancestors.OfType<EntityDeclaration>().FirstOrDefault()?.GetSymbol() as IMember;
		}

		static readonly Expression getMethodOrConstructorFromHandlePattern =
			new CastExpression(new Choice {
					 new TypePattern(typeof(MethodInfo)),
					 new TypePattern(typeof(ConstructorInfo))
				 }, new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(MethodBase)).ToType()), "GetMethodFromHandle"),
				new NamedNode("ldtokenNode", new MemberReferenceExpression(new LdTokenPattern("method").ToExpression(), "MethodHandle")),
				new OptionalNode(new MemberReferenceExpression(new TypeOfExpression(new AnyNode("declaringType")), "TypeHandle"))
			));

		public override void VisitCastExpression(CastExpression castExpression)
		{
			base.VisitCastExpression(castExpression);
			// Handle methodof
			Match m = getMethodOrConstructorFromHandlePattern.Match(castExpression);
			if (m.Success)
			{
				IMethod? method = m.Get<AstNode>("method").Single().GetSymbol() as IMethod;
				if (m.Has("declaringType") && method != null)
				{
					Expression newNode = new MemberReferenceExpression(new TypeReferenceExpression(m.Get<AstType>("declaringType").Single().Detach()), method.Name);
					newNode = new InvocationExpression(newNode, method.Parameters.Select(p => new TypeReferenceExpression(context.TypeSystemAstBuilder.ConvertType(p.Type))));
					m.Get<AstNode>("method").Single().ReplaceWith(newNode);
				}
				castExpression.ReplaceWith(m.Get<AstNode>("ldtokenNode").Single().CopyAnnotationsFrom(castExpression));
			}
		}

		void IAstTransform.Run(AstNode rootNode, TransformContext context)
		{
			try
			{
				this.context = context;
				rootNode.AcceptVisitor(this);
			}
			finally
			{
				this.context = null;
			}
		}
	}
}
