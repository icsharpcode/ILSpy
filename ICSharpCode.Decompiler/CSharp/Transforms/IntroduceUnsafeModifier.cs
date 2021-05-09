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

using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	public class IntroduceUnsafeModifier : DepthFirstAstVisitor<bool>, IAstTransform
	{
		public void Run(AstNode compilationUnit, TransformContext context)
		{
			compilationUnit.AcceptVisitor(this);
		}

		public static bool IsUnsafe(AstNode node)
		{
			return node.AcceptVisitor(new IntroduceUnsafeModifier());
		}

		protected override bool VisitChildren(AstNode node)
		{
			bool result = false;
			AstNode next;
			for (AstNode child = node.FirstChild; child != null; child = next)
			{
				// Store next to allow the loop to continue
				// if the visitor removes/replaces child.
				next = child.NextSibling;
				result |= child.AcceptVisitor(this);
			}
			if (result && node is EntityDeclaration && !(node is Accessor))
			{
				((EntityDeclaration)node).Modifiers |= Modifiers.Unsafe;
				return false;
			}
			return result;
		}

		public override bool VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression)
		{
			base.VisitPointerReferenceExpression(pointerReferenceExpression);
			return true;
		}

		public override bool VisitSizeOfExpression(SizeOfExpression sizeOfExpression)
		{
			// C# sizeof(MyStruct) requires unsafe{}
			// (not for sizeof(int), but that gets constant-folded and thus decompiled to 4)
			base.VisitSizeOfExpression(sizeOfExpression);
			return true;
		}

		public override bool VisitComposedType(ComposedType composedType)
		{
			if (composedType.PointerRank > 0)
				return true;
			else
				return base.VisitComposedType(composedType);
		}

		public override bool VisitFunctionPointerType(FunctionPointerAstType functionPointerType)
		{
			return true;
		}

		public override bool VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression)
		{
			bool result = base.VisitUnaryOperatorExpression(unaryOperatorExpression);
			if (unaryOperatorExpression.Operator == UnaryOperatorType.Dereference)
			{
				var bop = unaryOperatorExpression.Expression as BinaryOperatorExpression;
				if (bop != null && bop.Operator == BinaryOperatorType.Add
					&& bop.GetResolveResult() is OperatorResolveResult orr
					&& orr.Operands.FirstOrDefault()?.Type.Kind == TypeKind.Pointer)
				{
					// transform "*(ptr + int)" to "ptr[int]"
					IndexerExpression indexer = new IndexerExpression();
					indexer.Target = bop.Left.Detach();
					indexer.Arguments.Add(bop.Right.Detach());
					indexer.CopyAnnotationsFrom(unaryOperatorExpression);
					indexer.CopyAnnotationsFrom(bop);
					unaryOperatorExpression.ReplaceWith(indexer);
				}
				return true;
			}
			else if (unaryOperatorExpression.Operator == UnaryOperatorType.AddressOf)
			{
				return true;
			}
			else
			{
				return result;
			}
		}

		public override bool VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
		{
			bool result = base.VisitMemberReferenceExpression(memberReferenceExpression);
			UnaryOperatorExpression uoe = memberReferenceExpression.Target as UnaryOperatorExpression;
			if (uoe != null && uoe.Operator == UnaryOperatorType.Dereference)
			{
				PointerReferenceExpression pre = new PointerReferenceExpression();
				pre.Target = uoe.Expression.Detach();
				pre.MemberName = memberReferenceExpression.MemberName;
				memberReferenceExpression.TypeArguments.MoveTo(pre.TypeArguments);
				pre.CopyAnnotationsFrom(uoe);
				pre.RemoveAnnotations<ResolveResult>(); // only copy the ResolveResult from the MRE
				pre.CopyAnnotationsFrom(memberReferenceExpression);
				memberReferenceExpression.ReplaceWith(pre);
			}
			var rr = memberReferenceExpression.GetResolveResult();
			if (rr != null)
			{
				if (IsUnsafeType(rr.Type))
					return true;
			}

			return result;
		}

		public override bool VisitIdentifierExpression(IdentifierExpression identifierExpression)
		{
			bool result = base.VisitIdentifierExpression(identifierExpression);
			var rr = identifierExpression.GetResolveResult();
			if (rr != null)
			{
				if (IsUnsafeType(rr.Type))
					return true;
			}

			return result;
		}

		public override bool VisitStackAllocExpression(StackAllocExpression stackAllocExpression)
		{
			bool result = base.VisitStackAllocExpression(stackAllocExpression);
			var rr = stackAllocExpression.GetResolveResult();
			if (IsUnsafeType(rr?.Type))
				return true;
			return result;
		}

		public override bool VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			bool result = base.VisitInvocationExpression(invocationExpression);
			var rr = invocationExpression.GetResolveResult();
			if (IsUnsafeType(rr?.Type))
				return true;
			if ((rr as MemberResolveResult)?.Member is IParameterizedMember pm)
			{
				if (pm.Parameters.Any(p => IsUnsafeType(p.Type)))
					return true;
			}
			return result;
		}

		public override bool VisitFixedVariableInitializer(FixedVariableInitializer fixedVariableInitializer)
		{
			base.VisitFixedVariableInitializer(fixedVariableInitializer);
			return true;
		}

		private bool IsUnsafeType(IType type)
		{
			if (type?.Kind == TypeKind.Delegate)
			{
				// Using a delegate which involves pointers in its signature needs the unsafe modifier
				// https://github.com/icsharpcode/ILSpy/issues/949
				IMethod invoke = type.GetDelegateInvokeMethod();
				if (invoke != null && (ContainsPointer(invoke.ReturnType) || invoke.Parameters.Any(p => ContainsPointer(p.Type))))
					return true;
			}
			return ContainsPointer(type);
		}

		private bool ContainsPointer(IType type)
		{
			switch (type?.Kind)
			{
				case TypeKind.Pointer:
				case TypeKind.FunctionPointer:
					return true;
				case TypeKind.ByReference:
				case TypeKind.Array:
					return IsUnsafeType(((Decompiler.TypeSystem.Implementation.TypeWithElementType)type).ElementType);
				default:
					return false;
			}
		}
	}
}
