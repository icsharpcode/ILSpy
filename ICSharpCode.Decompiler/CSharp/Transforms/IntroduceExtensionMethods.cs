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
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.Semantics;
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
		UsingScope rootUsingScope;
		UsingScope usingScope;
		IMember currentMember;
		CSharpTypeResolveContext resolveContext;
		CSharpResolver resolver;

		public void Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			this.usingScope = this.rootUsingScope = rootNode.Annotation<UsingScope>();
			rootNode.AcceptVisitor(this);
		}

		void SetContext()
		{
			this.usingScope = rootUsingScope;
			foreach (var name in currentMember.Namespace.Split('.'))
				usingScope = new UsingScope(usingScope, name);
			resolveContext = new CSharpTypeResolveContext(currentMember.ParentAssembly, usingScope.Resolve(context.TypeSystem.Compilation), currentMember.DeclaringTypeDefinition, currentMember);
			resolver = new CSharpResolver(resolveContext);
		}

		public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			currentMember = methodDeclaration.GetSymbol() as IMember;
			if (currentMember == null) return;
			SetContext();
			base.VisitMethodDeclaration(methodDeclaration);
			currentMember = null;
		}

		public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			currentMember = constructorDeclaration.GetSymbol() as IMember;
			if (currentMember == null) return;
			SetContext();
			base.VisitConstructorDeclaration(constructorDeclaration);
			currentMember = null;
		}

		public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			currentMember = destructorDeclaration.GetSymbol() as IMember;
			if (currentMember == null) return;
			SetContext();
			base.VisitDestructorDeclaration(destructorDeclaration);
			currentMember = null;
		}

		public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			currentMember = propertyDeclaration.GetSymbol() as IMember;
			if (currentMember == null) return;
			SetContext();
			base.VisitPropertyDeclaration(propertyDeclaration);
			currentMember = null;
		}

		public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
		{
			currentMember = fieldDeclaration.GetSymbol() as IMember;
			if (currentMember == null) return;
			SetContext();
			base.VisitFieldDeclaration(fieldDeclaration);
			currentMember = null;
		}

		public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
		{
			currentMember = eventDeclaration.GetSymbol() as IMember;
			if (currentMember == null) return;
			SetContext();
			base.VisitEventDeclaration(eventDeclaration);
			currentMember = null;
		}

		public override void VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration)
		{
			currentMember = eventDeclaration.GetSymbol() as IMember;
			if (currentMember == null) return;
			SetContext();
			base.VisitCustomEventDeclaration(eventDeclaration);
			currentMember = null;
		}

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression(invocationExpression);
			var mre = invocationExpression.Target as MemberReferenceExpression;
			var method = invocationExpression.GetSymbol() as IMethod;
			if (method == null || !method.IsExtensionMethod || mre == null || !(mre.Target is TypeReferenceExpression) || !invocationExpression.Arguments.Any())
				return;
			var firstArgument = invocationExpression.Arguments.First();
			var target = firstArgument.GetResolveResult();
			var args = invocationExpression.Arguments.Skip(1).Select(a => a.GetResolveResult()).ToArray();
			var rr = resolver.ResolveMemberAccess(target, method.Name, method.TypeArguments, NameLookupMode.InvocationTarget) as MethodGroupResolveResult;
			if (rr == null)
				return;
			var or = rr.PerformOverloadResolution(resolveContext.Compilation, args, allowExtensionMethods: true);
			if (or == null || or.IsAmbiguous)
				return;
			if (firstArgument is NullReferenceExpression)
				firstArgument = firstArgument.ReplaceWith(expr => new CastExpression(context.TypeSystemAstBuilder.ConvertType(method.Parameters[0].Type), expr.Detach()));
			else
				mre.Target = firstArgument.Detach();
		}
	}
}