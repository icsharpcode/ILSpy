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

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Introduces using declarations.
	/// </summary>
	public class IntroduceUsingDeclarations : IAstTransform
	{
		public void Run(AstNode rootNode, TransformContext context)
		{
			// First determine all the namespaces that need to be imported:
			var requiredImports = new FindRequiredImports(context);
			rootNode.AcceptVisitor(requiredImports);
			
			var usingScope = new UsingScope();
			rootNode.AddAnnotation(usingScope);

			if (context.Settings.UsingDeclarations) {
				var insertionPoint = rootNode.Children.LastOrDefault(n => n is PreProcessorDirective p && p.Type == PreProcessorDirectiveType.Define);

				// Now add using declarations for those namespaces:
				foreach (string ns in requiredImports.ImportedNamespaces.OrderByDescending(n => n)) {
					Debug.Assert(context.RequiredNamespacesSuperset.Contains(ns), $"Should not insert using declaration for namespace that is missing from the superset: {ns}");
					// we go backwards (OrderByDescending) through the list of namespaces because we insert them backwards
					// (always inserting at the start of the list)
					string[] parts = ns.Split('.');
					AstType nsType = new SimpleType(parts[0]);
					for (int i = 1; i < parts.Length; i++) {
						nsType = new MemberType { Target = nsType, MemberName = parts[i] };
					}
					var reference = nsType.ToTypeReference(NameLookupMode.TypeInUsingDeclaration) as TypeOrNamespaceReference;
					if (reference != null)
						usingScope.Usings.Add(reference);
					rootNode.InsertChildAfter(insertionPoint, new UsingDeclaration { Import = nsType }, SyntaxTree.MemberRole);
				}
			}

			// verify that the SimpleTypes refer to the correct type (no ambiguities)
			rootNode.AcceptVisitor(new FullyQualifyAmbiguousTypeNamesVisitor(context, usingScope));
		}
		
		sealed class FindRequiredImports : DepthFirstAstVisitor
		{
			string currentNamespace;

			public readonly HashSet<string> DeclaredNamespaces = new HashSet<string>() { string.Empty };
			public readonly HashSet<string> ImportedNamespaces = new HashSet<string>();
			
			public FindRequiredImports(TransformContext context)
			{
				this.currentNamespace = context.CurrentTypeDefinition?.Namespace ?? string.Empty;
			}
			
			bool IsParentOfCurrentNamespace(string ns)
			{
				if (ns.Length == 0)
					return true;
				if (currentNamespace.StartsWith(ns, StringComparison.Ordinal)) {
					if (currentNamespace.Length == ns.Length)
						return true;
					if (currentNamespace[ns.Length] == '.')
						return true;
				}
				return false;
			}
			
			public override void VisitSimpleType(SimpleType simpleType)
			{
				var trr = simpleType.Annotation<TypeResolveResult>();
				if (trr != null && !IsParentOfCurrentNamespace(trr.Type.Namespace)) {
					ImportedNamespaces.Add(trr.Type.Namespace);
				}
				base.VisitSimpleType(simpleType); // also visit type arguments
			}
			
			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				string oldNamespace = currentNamespace;
				foreach (string ident in namespaceDeclaration.Identifiers) {
					currentNamespace = NamespaceDeclaration.BuildQualifiedName(currentNamespace, ident);
					DeclaredNamespaces.Add(currentNamespace);
				}
				base.VisitNamespaceDeclaration(namespaceDeclaration);
				currentNamespace = oldNamespace;
			}/*

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				string oldNamespace = currentNamespace;
				if (!(typeDeclaration.Parent is NamespaceDeclaration || typeDeclaration.Parent is TypeDeclaration)) {
					var symbol = typeDeclaration.GetSymbol() as ITypeDefinition;
					if (symbol != null) {
						currentNamespace = symbol.Namespace;
						DeclaredNamespaces.Add(currentNamespace);
					}
				}
				base.VisitTypeDeclaration(typeDeclaration);
				currentNamespace = oldNamespace;
			}*/
		}
		
		sealed class FullyQualifyAmbiguousTypeNamesVisitor : DepthFirstAstVisitor
		{
			Stack<CSharpTypeResolveContext> context;
			TypeSystemAstBuilder astBuilder;
			bool ignoreUsingScope;
			
			public FullyQualifyAmbiguousTypeNamesVisitor(TransformContext context, UsingScope usingScope)
			{
				this.ignoreUsingScope = !context.Settings.UsingDeclarations;

				CSharpTypeResolveContext currentContext;
				if (ignoreUsingScope) {
					currentContext = new CSharpTypeResolveContext(context.TypeSystem.MainModule);
				} else {
					this.context = new Stack<CSharpTypeResolveContext>();
					if (!string.IsNullOrEmpty(context.CurrentTypeDefinition?.Namespace)) {
						foreach (string ns in context.CurrentTypeDefinition.Namespace.Split('.')) {
							usingScope = new UsingScope(usingScope, ns);
						}
					}
					currentContext = new CSharpTypeResolveContext(context.TypeSystem.MainModule, usingScope.Resolve(context.TypeSystem), context.CurrentTypeDefinition);
					this.context.Push(currentContext);
				}
				this.astBuilder = CreateAstBuilder(currentContext);
			}

			static TypeSystemAstBuilder CreateAstBuilder(CSharpTypeResolveContext context)
			{
				return new TypeSystemAstBuilder(new CSharpResolver(context)) {
					AddResolveResultAnnotations = true,
					UseAliases = true
				};
			}
			
			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				if (ignoreUsingScope) {
					base.VisitNamespaceDeclaration(namespaceDeclaration);
					return;
				}
				var previousContext = context.Peek();
				var usingScope = previousContext.CurrentUsingScope.UnresolvedUsingScope;
				foreach (string ident in namespaceDeclaration.Identifiers) {
					usingScope = new UsingScope(usingScope, ident);
				}
				var currentContext = new CSharpTypeResolveContext(previousContext.CurrentModule, usingScope.Resolve(previousContext.Compilation));
				context.Push(currentContext);
				try {
					astBuilder = CreateAstBuilder(currentContext);
					base.VisitNamespaceDeclaration(namespaceDeclaration);
				} finally {
					astBuilder = CreateAstBuilder(previousContext);
					context.Pop();
				}
			}
			
			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (ignoreUsingScope) {
					base.VisitTypeDeclaration(typeDeclaration);
					return;
				}
				var previousContext = context.Peek();
				var currentContext = previousContext.WithCurrentTypeDefinition(typeDeclaration.GetSymbol() as ITypeDefinition);
				context.Push(currentContext);
				try {
					astBuilder = CreateAstBuilder(currentContext);
					base.VisitTypeDeclaration(typeDeclaration);
				} finally {
					astBuilder = CreateAstBuilder(previousContext);
					context.Pop();
				}
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				if (ignoreUsingScope) {
					base.VisitMethodDeclaration(methodDeclaration);
					return;
				}
				if (methodDeclaration.GetSymbol() is IMethod method && CSharpDecompiler.IsWindowsFormsInitializeComponentMethod(method)) {
					var previousContext = context.Peek();
					var currentContext = new CSharpTypeResolveContext(previousContext.CurrentModule);
					context.Push(currentContext);
					try {
						astBuilder = CreateAstBuilder(currentContext);
						base.VisitMethodDeclaration(methodDeclaration);
					} finally {
						astBuilder = CreateAstBuilder(previousContext);
						context.Pop();
					}
				} else {
					base.VisitMethodDeclaration(methodDeclaration);
				}
			}

			public override void VisitSimpleType(SimpleType simpleType)
			{
				TypeResolveResult rr;
				if ((rr = simpleType.Annotation<TypeResolveResult>()) == null) {
					base.VisitSimpleType(simpleType);
					return;
				}
				astBuilder.NameLookupMode = simpleType.GetNameLookupMode();
				if (astBuilder.NameLookupMode == NameLookupMode.Type) {
					AstType outermostType = simpleType;
					while (outermostType.Parent is AstType)
						outermostType = (AstType)outermostType.Parent;
					if (outermostType.Parent is TypeReferenceExpression) {
						// ILSpy uses TypeReferenceExpression in expression context even when the C# parser
						// wouldn't know that it's a type reference.
						// Fall back to expression-mode lookup in these cases:
						astBuilder.NameLookupMode = NameLookupMode.Expression;
					}
				}
				if (simpleType.Parent is Syntax.Attribute) {
					simpleType.ReplaceWith(astBuilder.ConvertAttributeType(rr.Type));
				} else {
					simpleType.ReplaceWith(astBuilder.ConvertType(rr.Type));
				}
			}
		}
	}
}
