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
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Converts extension method calls into infix syntax.
	/// </summary>
	public class IntroduceExtensionMethods : DepthFirstAstVisitor, IAstTransform
	{
		TransformContext context;
		CSharpResolver resolver;

		public void Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			InitializeContext(rootNode.Annotation<UsingScope>());
			rootNode.AcceptVisitor(this);
		}

		Stack<CSharpTypeResolveContext> resolveContextStack = new Stack<CSharpTypeResolveContext>();

		void InitializeContext(UsingScope usingScope)
		{
			this.resolveContextStack = new Stack<CSharpTypeResolveContext>();
			if (!string.IsNullOrEmpty(context.DecompiledTypeDefinition?.Namespace)) {
				foreach (string ns in context.DecompiledTypeDefinition.Namespace.Split('.')) {
					usingScope = new UsingScope(usingScope, ns);
				}
			}
			var currentContext = new CSharpTypeResolveContext(context.TypeSystem.MainAssembly, usingScope.Resolve(context.TypeSystem.Compilation), context.DecompiledTypeDefinition);
			this.resolveContextStack.Push(currentContext);
			this.resolver = new CSharpResolver(currentContext);
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			var previousContext = resolveContextStack.Peek();
			var usingScope = previousContext.CurrentUsingScope.UnresolvedUsingScope;
			foreach (string ident in namespaceDeclaration.Identifiers) {
				usingScope = new UsingScope(usingScope, ident);
			}
			var currentContext = new CSharpTypeResolveContext(previousContext.CurrentAssembly, usingScope.Resolve(previousContext.Compilation));
			resolveContextStack.Push(currentContext);
			try {
				this.resolver = new CSharpResolver(currentContext);
				base.VisitNamespaceDeclaration(namespaceDeclaration);
			} finally {
				this.resolver = new CSharpResolver(previousContext);
				resolveContextStack.Pop();
			}
		}

		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			var previousContext = resolveContextStack.Peek();
			var currentContext = previousContext.WithCurrentTypeDefinition(typeDeclaration.GetSymbol() as ITypeDefinition);
			resolveContextStack.Push(currentContext);
			try {
				this.resolver = new CSharpResolver(currentContext);
				base.VisitTypeDeclaration(typeDeclaration);
			} finally {
				this.resolver = new CSharpResolver(previousContext);
				resolveContextStack.Pop();
			}
		}

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression(invocationExpression);
			var mre = invocationExpression.Target as MemberReferenceExpression;
			var method = invocationExpression.GetSymbol() as IMethod;
			if (method == null || !method.IsExtensionMethod || mre == null || !(mre.Target is TypeReferenceExpression) || !invocationExpression.Arguments.Any())
				return;
			var typeArguments = mre.TypeArguments.Any() ? method.TypeArguments : EmptyList<IType>.Instance;
			var firstArgument = invocationExpression.Arguments.First();
			var target = firstArgument.GetResolveResult();
			var args = invocationExpression.Arguments.Skip(1).Select(a => a.GetResolveResult()).ToArray();
			var rr = resolver.ResolveMemberAccess(target, method.Name, typeArguments, NameLookupMode.InvocationTarget) as MethodGroupResolveResult;
			if (rr == null)
				return;
			var or = rr.PerformOverloadResolution(resolver.CurrentTypeResolveContext.Compilation, args, allowExtensionMethods: true);
			if (or == null || or.IsAmbiguous || !method.Equals(or.GetBestCandidateWithSubstitutedTypeArguments()))
				return;
			if (firstArgument is NullReferenceExpression)
				firstArgument = firstArgument.ReplaceWith(expr => new CastExpression(context.TypeSystemAstBuilder.ConvertType(method.Parameters[0].Type), expr.Detach()));
			else
				mre.Target = firstArgument.Detach();
		}
	}
}