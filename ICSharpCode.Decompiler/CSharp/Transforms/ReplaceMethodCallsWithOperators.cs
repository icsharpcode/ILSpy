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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;

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
			if (method.Name == "Concat" && method.DeclaringType.FullName == "System.String" && CheckArgumentsForStringConcat(arguments)) {
				invocationExpression.Arguments.Clear(); // detach arguments from invocationExpression
				Expression expr = arguments[0];
				for (int i = 1; i < arguments.Length; i++) {
					expr = new BinaryOperatorExpression(expr, BinaryOperatorType.Add, arguments[i].UnwrapInDirectionExpression());
				}
				expr.CopyAnnotationsFrom(invocationExpression);
				invocationExpression.ReplaceWith(expr);
				return;
			}

			switch (method.FullName) {
				case "System.Type.GetTypeFromHandle":
					if (arguments.Length == 1) {
						if (typeHandleOnTypeOfPattern.IsMatch(arguments[0])) {
							Expression target = ((MemberReferenceExpression)arguments[0]).Target;
							target.CopyInstructionsFrom(invocationExpression);
							invocationExpression.ReplaceWith(target);
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
					if (arguments.Length == 0 && method.TypeArguments.Count == 1 && IsInstantiableTypeParameter(method.TypeArguments[0])) {
						invocationExpression.ReplaceWith(new ObjectCreateExpression(context.TypeSystemAstBuilder.ConvertType(method.TypeArguments.First())));
					}
					break;
			}

			BinaryOperatorType? bop = GetBinaryOperatorTypeFromMetadataName(method.Name);
			if (bop != null && arguments.Length == 2) {
				invocationExpression.Arguments.Clear(); // detach arguments from invocationExpression
				invocationExpression.ReplaceWith(
					new BinaryOperatorExpression(
						arguments[0].UnwrapInDirectionExpression(),
						bop.Value,
						arguments[1].UnwrapInDirectionExpression()
					).CopyAnnotationsFrom(invocationExpression)
				);
				return;
			}
			UnaryOperatorType? uop = GetUnaryOperatorTypeFromMetadataName(method.Name);
			if (uop != null && arguments.Length == 1) {
				if (uop == UnaryOperatorType.Increment || uop == UnaryOperatorType.Decrement) {
					// `op_Increment(a)` is not equivalent to `++a`,
					// because it doesn't assign the incremented value to a.
					if (method.DeclaringType.IsKnownType(KnownTypeCode.Decimal)) {
						// Legacy csc optimizes "d + 1m" to "op_Increment(d)",
						// so reverse that optimization here:
						invocationExpression.ReplaceWith(
							new BinaryOperatorExpression(
								arguments[0].UnwrapInDirectionExpression().Detach(),
								(uop == UnaryOperatorType.Increment ? BinaryOperatorType.Add : BinaryOperatorType.Subtract),
								new PrimitiveExpression(1m)
							).CopyAnnotationsFrom(invocationExpression)
						);
					}
					return;
				}
				arguments[0].Remove(); // detach argument
				invocationExpression.ReplaceWith(
					new UnaryOperatorExpression(uop.Value, arguments[0].UnwrapInDirectionExpression()).CopyAnnotationsFrom(invocationExpression)
				);
				return;
			}
			if (method.Name == "op_Explicit" && arguments.Length == 1) {
				arguments[0].Remove(); // detach argument
				invocationExpression.ReplaceWith(
					new CastExpression(context.TypeSystemAstBuilder.ConvertType(method.ReturnType), arguments[0].UnwrapInDirectionExpression())
						.CopyAnnotationsFrom(invocationExpression)
				);
				return;
			}
			if (method.Name == "op_True" && arguments.Length == 1 && invocationExpression.Role == Roles.Condition) {
				invocationExpression.ReplaceWith(arguments[0].UnwrapInDirectionExpression());
				return;
			}

			return;
		}

		bool IsInstantiableTypeParameter(IType type)
		{
			return type is ITypeParameter tp && tp.HasDefaultConstructorConstraint;
		}

		bool CheckArgumentsForStringConcat(Expression[] arguments)
		{
			if (arguments.Length < 2)
				return false;

			return !arguments.Any(arg => arg is NamedArgumentExpression) &&
				(arguments[0].GetResolveResult().Type.IsKnownType(KnownTypeCode.String) ||
				arguments[1].GetResolveResult().Type.IsKnownType(KnownTypeCode.String));
		}

		static BinaryOperatorType? GetBinaryOperatorTypeFromMetadataName(string name)
		{
			switch (name) {
				case "op_Addition":
					return BinaryOperatorType.Add;
				case "op_Subtraction":
					return BinaryOperatorType.Subtract;
				case "op_Multiply":
					return BinaryOperatorType.Multiply;
				case "op_Division":
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

		static UnaryOperatorType? GetUnaryOperatorTypeFromMetadataName(string name)
		{
			switch (name) {
				case "op_LogicalNot":
					return UnaryOperatorType.Not;
				case "op_OnesComplement":
					return UnaryOperatorType.BitNot;
				case "op_UnaryNegation":
					return UnaryOperatorType.Minus;
				case "op_UnaryPlus":
					return UnaryOperatorType.Plus;
				case "op_Increment":
					return UnaryOperatorType.Increment;
				case "op_Decrement":
					return UnaryOperatorType.Decrement;
				default:
					return null;
			}
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
			if (m.Success) {
				IMethod method = m.Get<AstNode>("method").Single().GetSymbol() as IMethod;
				if (m.Has("declaringType") && method != null) {
					Expression newNode = new MemberReferenceExpression(new TypeReferenceExpression(m.Get<AstType>("declaringType").Single().Detach()), method.Name);
					newNode = new InvocationExpression(newNode, method.Parameters.Select(p => new TypeReferenceExpression(context.TypeSystemAstBuilder.ConvertType(p.Type))));
					m.Get<AstNode>("method").Single().ReplaceWith(newNode);
				}
				castExpression.ReplaceWith(m.Get<AstNode>("ldtokenNode").Single().CopyAnnotationsFrom(castExpression));
			}
		}

		void IAstTransform.Run(AstNode rootNode, TransformContext context)
		{
			try {
				this.context = context;
				rootNode.AcceptVisitor(this);
			} finally {
				this.context = null;
			}
		}
	}
}
