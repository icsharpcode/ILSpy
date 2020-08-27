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
using System.Diagnostics;
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
		CSharpResolver resolver;
		CSharpConversions conversions;

		public void Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			this.conversions = CSharpConversions.Get(context.TypeSystem);
			InitializeContext(rootNode.Annotation<UsingScope>());
			rootNode.AcceptVisitor(this);
		}

		Stack<CSharpTypeResolveContext> resolveContextStack = new Stack<CSharpTypeResolveContext>();

		void InitializeContext(UsingScope usingScope)
		{
			this.resolveContextStack = new Stack<CSharpTypeResolveContext>();
			if (!string.IsNullOrEmpty(context.CurrentTypeDefinition?.Namespace))
			{
				foreach (string ns in context.CurrentTypeDefinition.Namespace.Split('.'))
				{
					usingScope = new UsingScope(usingScope, ns);
				}
			}
			var currentContext = new CSharpTypeResolveContext(context.TypeSystem.MainModule, usingScope.Resolve(context.TypeSystem), context.CurrentTypeDefinition);
			this.resolveContextStack.Push(currentContext);
			this.resolver = new CSharpResolver(currentContext);
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			var previousContext = resolveContextStack.Peek();
			var usingScope = previousContext.CurrentUsingScope.UnresolvedUsingScope;
			foreach (string ident in namespaceDeclaration.Identifiers)
			{
				usingScope = new UsingScope(usingScope, ident);
			}
			var currentContext = new CSharpTypeResolveContext(previousContext.CurrentModule, usingScope.Resolve(previousContext.Compilation));
			resolveContextStack.Push(currentContext);
			try
			{
				this.resolver = new CSharpResolver(currentContext);
				base.VisitNamespaceDeclaration(namespaceDeclaration);
			}
			finally
			{
				this.resolver = new CSharpResolver(previousContext);
				resolveContextStack.Pop();
			}
		}

		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			var previousContext = resolveContextStack.Peek();
			var currentContext = previousContext.WithCurrentTypeDefinition(typeDeclaration.GetSymbol() as ITypeDefinition);
			resolveContextStack.Push(currentContext);
			try
			{
				this.resolver = new CSharpResolver(currentContext);
				base.VisitTypeDeclaration(typeDeclaration);
			}
			finally
			{
				this.resolver = new CSharpResolver(previousContext);
				resolveContextStack.Pop();
			}
		}

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			base.VisitInvocationExpression(invocationExpression);
			var method = invocationExpression.GetSymbol() as IMethod;
			if (method == null || !method.IsExtensionMethod || !invocationExpression.Arguments.Any())
				return;
			IReadOnlyList<IType> typeArguments;
			MemberReferenceExpression memberRefExpr;
			switch (invocationExpression.Target)
			{
				case MemberReferenceExpression mre:
					typeArguments = mre.TypeArguments.Any() ? method.TypeArguments : EmptyList<IType>.Instance;
					memberRefExpr = mre;
					break;
				case IdentifierExpression ide:
					typeArguments = ide.TypeArguments.Any() ? method.TypeArguments : EmptyList<IType>.Instance;
					memberRefExpr = null;
					break;
				default:
					return;
			}

			var firstArgument = invocationExpression.Arguments.First();
			if (firstArgument is NamedArgumentExpression)
				return;
			var target = firstArgument.GetResolveResult();
			if (target is ConstantResolveResult crr && crr.ConstantValue == null)
			{
				target = new ConversionResolveResult(method.Parameters[0].Type, crr, Conversion.NullLiteralConversion);
			}
			ResolveResult[] args = new ResolveResult[invocationExpression.Arguments.Count - 1];
			string[] argNames = null;
			int pos = 0;
			foreach (var arg in invocationExpression.Arguments.Skip(1))
			{
				if (arg is NamedArgumentExpression nae)
				{
					if (argNames == null)
					{
						argNames = new string[args.Length];
					}
					argNames[pos] = nae.Name;
					args[pos] = nae.Expression.GetResolveResult();
				}
				else
				{
					args[pos] = arg.GetResolveResult();
				}
				pos++;
			}
			if (!CanTransformToExtensionMethodCall(resolver, method, typeArguments, target, args, argNames))
				return;
			if (firstArgument is DirectionExpression dirExpr)
			{
				if (!context.Settings.RefExtensionMethods || dirExpr.FieldDirection == FieldDirection.Out)
					return;
				firstArgument = dirExpr.Expression;
				target = firstArgument.GetResolveResult();
				dirExpr.Detach();
			}
			else if (firstArgument is NullReferenceExpression)
			{
				Debug.Assert(context.RequiredNamespacesSuperset.Contains(method.Parameters[0].Type.Namespace));
				firstArgument = firstArgument.ReplaceWith(expr => new CastExpression(context.TypeSystemAstBuilder.ConvertType(method.Parameters[0].Type), expr.Detach()));
			}
			if (invocationExpression.Target is IdentifierExpression identifierExpression)
			{
				identifierExpression.Detach();
				memberRefExpr = new MemberReferenceExpression(firstArgument.Detach(), method.Name, identifierExpression.TypeArguments.Detach());
				invocationExpression.Target = memberRefExpr;
			}
			else
			{
				memberRefExpr.Target = firstArgument.Detach();
			}
			if (invocationExpression.GetResolveResult() is CSharpInvocationResolveResult irr)
			{
				// do not forget to update the CSharpInvocationResolveResult => set IsExtensionMethodInvocation == true
				invocationExpression.RemoveAnnotations<CSharpInvocationResolveResult>();
				var newResolveResult = new CSharpInvocationResolveResult(
					irr.TargetResult, irr.Member, irr.Arguments, irr.OverloadResolutionErrors,
					isExtensionMethodInvocation: true, irr.IsExpandedForm, irr.IsDelegateInvocation,
					irr.GetArgumentToParameterMap(), irr.InitializerStatements);
				invocationExpression.AddAnnotation(newResolveResult);
			}
		}

		public static bool CanTransformToExtensionMethodCall(CSharpResolver resolver, IMethod method,
			IReadOnlyList<IType> typeArguments, ResolveResult target, ResolveResult[] arguments, string[] argumentNames)
		{
			if (target is LambdaResolveResult)
				return false;
			var rr = resolver.ResolveMemberAccess(target, method.Name, typeArguments, NameLookupMode.InvocationTarget) as MethodGroupResolveResult;
			if (rr == null)
				return false;
			var or = rr.PerformOverloadResolution(resolver.CurrentTypeResolveContext.Compilation, arguments, argumentNames, allowExtensionMethods: true);
			if (or == null || or.IsAmbiguous)
				return false;
			return method.Equals(or.GetBestCandidateWithSubstitutedTypeArguments())
				&& CSharpResolver.IsEligibleExtensionMethod(target.Type, method, useTypeInference: false, out _);
		}

		public static bool CanTransformToExtensionMethodCall(IMethod method, CSharpTypeResolveContext resolveContext, bool ignoreTypeArguments = false, bool ignoreArgumentNames = true)
		{
			if (method.Parameters.Count == 0)
				return false;
			var targetType = method.Parameters.Select(p => new ResolveResult(p.Type)).First();
			var paramTypes = method.Parameters.Skip(1).Select(p => new ResolveResult(p.Type)).ToArray();
			var paramNames = ignoreArgumentNames ? null : method.Parameters.SelectReadOnlyArray(p => p.Name);
			var typeArgs = ignoreTypeArguments ? Empty<IType>.Array : method.TypeArguments.ToArray();
			var resolver = new CSharpResolver(resolveContext);
			return CanTransformToExtensionMethodCall(resolver, method, typeArgs, targetType, paramTypes, argumentNames: paramNames);
		}
	}
}