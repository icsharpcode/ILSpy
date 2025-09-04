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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

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

			List<INamespace> resolvedNamespaces = new List<INamespace>();

			if (context.Settings.UsingDeclarations)
			{
				var insertionPoint = rootNode.Children.LastOrDefault(n => n is PreProcessorDirective p && p.Type == PreProcessorDirectiveType.Define);

				// Now add using declarations for those namespaces:
				IOrderedEnumerable<string> sortedImports = requiredImports.ImportedNamespaces
					.OrderBy(n => n.StartsWith("System", StringComparison.Ordinal))
					.ThenByDescending(n => n);
				foreach (string ns in sortedImports)
				{
					Debug.Assert(context.RequiredNamespacesSuperset.Contains(ns), $"Should not insert using declaration for namespace that is missing from the superset: {ns}");
					// we go backwards (OrderByDescending) through the list of namespaces because we insert them backwards
					// (always inserting at the start of the list)
					string[] parts = ns.Split('.');
					AstType nsType = new SimpleType(parts[0]);
					for (int i = 1; i < parts.Length; i++)
					{
						nsType = new MemberType { Target = nsType, MemberName = parts[i] };
					}
					var resolvedNamespace = context.TypeSystem.GetNamespaceByFullName(ns);
					if (resolvedNamespace != null)
					{
						resolvedNamespaces.Add(resolvedNamespace);
					}
					rootNode.InsertChildAfter(insertionPoint, new UsingDeclaration { Import = nsType }, SyntaxTree.MemberRole);
				}
			}

			var usingScope = new UsingScope(
				new CSharpTypeResolveContext(context.TypeSystem.MainModule),
				context.TypeSystem.RootNamespace,
				resolvedNamespaces.ToImmutableArray()
			);
			rootNode.AddAnnotation(usingScope);

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
				if (currentNamespace.StartsWith(ns, StringComparison.Ordinal))
				{
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
				AddImportedNamespace(trr?.Type);
				base.VisitSimpleType(simpleType); // also visit type arguments
			}

			private void AddImportedNamespace(IType type)
			{
				if (type != null && !IsParentOfCurrentNamespace(type.Namespace))
				{
					ImportedNamespaces.Add(type.Namespace);
				}
			}

			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				string oldNamespace = currentNamespace;
				foreach (string ident in namespaceDeclaration.Identifiers)
				{
					currentNamespace = NamespaceDeclaration.BuildQualifiedName(currentNamespace, ident);
					DeclaredNamespaces.Add(currentNamespace);
				}
				base.VisitNamespaceDeclaration(namespaceDeclaration);
				currentNamespace = oldNamespace;
			}

			public override void VisitForeachStatement(ForeachStatement foreachStatement)
			{
				var annotation = foreachStatement.Annotation<ForeachAnnotation>();
				if (annotation?.GetEnumeratorCall is CallInstruction { Method: { IsExtensionMethod: true, DeclaringType: var type } })
				{
					AddImportedNamespace(type);
				}
				base.VisitForeachStatement(foreachStatement);
			}

			public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignation parenthesizedVariableDesignation)
			{
				var annotation = parenthesizedVariableDesignation.Annotation<MatchInstruction>();
				if (annotation?.Method is IMethod { DeclaringType: var type })
				{
					AddImportedNamespace(type);
				}
				base.VisitParenthesizedVariableDesignation(parenthesizedVariableDesignation);
			}

			public override void VisitTupleExpression(TupleExpression tupleExpression)
			{
				var annotation = tupleExpression.Annotation<MatchInstruction>();
				if (annotation?.Method is IMethod { DeclaringType: var type })
				{
					AddImportedNamespace(type);
				}
				base.VisitTupleExpression(tupleExpression);
			}

			public override void VisitArrayInitializerExpression(ArrayInitializerExpression arrayInitializerExpression)
			{
				foreach (var item in arrayInitializerExpression.Elements)
				{
					var optionalCall = item.Annotation<CallInstruction>();
					if (optionalCall?.Method is { IsExtensionMethod: true, Name: "Add" })
					{
						AddImportedNamespace(optionalCall.Method.DeclaringType);
					}
				}
				base.VisitArrayInitializerExpression(arrayInitializerExpression);
			}
		}

		sealed class FullyQualifyAmbiguousTypeNamesVisitor : DepthFirstAstVisitor
		{
			readonly bool ignoreUsingScope;
			readonly DecompilerSettings settings;

			CSharpResolver resolver;
			TypeSystemAstBuilder astBuilder;

			public FullyQualifyAmbiguousTypeNamesVisitor(TransformContext context, UsingScope usingScope)
			{
				this.ignoreUsingScope = !context.Settings.UsingDeclarations;
				this.settings = context.Settings;
				this.resolver = new CSharpResolver(new CSharpTypeResolveContext(context.TypeSystem.MainModule));

				if (!ignoreUsingScope)
				{
					if (!string.IsNullOrEmpty(context.CurrentTypeDefinition?.Namespace))
					{
						foreach (string ns in context.CurrentTypeDefinition.Namespace.Split('.'))
						{
							usingScope = usingScope.WithNestedNamespace(ns);
						}
					}
					this.resolver = this.resolver.WithCurrentUsingScope(usingScope)
						.WithCurrentTypeDefinition(context.CurrentTypeDefinition);
				}
				this.astBuilder = CreateAstBuilder(resolver);
			}

			TypeSystemAstBuilder CreateAstBuilder(CSharpResolver resolver, IL.ILFunction function = null)
			{
				if (function != null)
				{
					var variables = new Dictionary<string, IVariable>();
					foreach (var v in function.Variables)
					{
						if (v.Kind != IL.VariableKind.Parameter && v.Name != null && !variables.ContainsKey(v.Name))
							variables.Add(v.Name, new DefaultVariable(v.Type, v.Name));
					}
					resolver = resolver.AddVariables(variables);
				}

				return new TypeSystemAstBuilder(resolver) {
					UseNullableSpecifierForValueTypes = settings.LiftNullables,
					AlwaysUseGlobal = settings.AlwaysUseGlobal,
					AddResolveResultAnnotations = true,
					UseAliases = true
				};
			}

			public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
			{
				if (ignoreUsingScope)
				{
					base.VisitNamespaceDeclaration(namespaceDeclaration);
					return;
				}
				var previousResolver = resolver;
				var previousAstBuilder = astBuilder;
				var usingScope = resolver.CurrentUsingScope;
				foreach (string ident in namespaceDeclaration.Identifiers)
				{
					usingScope = usingScope.WithNestedNamespace(ident);
				}
				resolver = resolver.WithCurrentUsingScope(usingScope);
				try
				{
					astBuilder = CreateAstBuilder(resolver);
					base.VisitNamespaceDeclaration(namespaceDeclaration);
				}
				finally
				{
					astBuilder = previousAstBuilder;
					resolver = previousResolver;
				}
			}

			public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			{
				if (ignoreUsingScope)
				{
					base.VisitTypeDeclaration(typeDeclaration);
					return;
				}
				var previousResolver = resolver;
				var previousAstBuilder = astBuilder;
				resolver = resolver.WithCurrentTypeDefinition(typeDeclaration.GetSymbol() as ITypeDefinition);
				try
				{
					astBuilder = CreateAstBuilder(resolver);
					base.VisitTypeDeclaration(typeDeclaration);
				}
				finally
				{
					astBuilder = previousAstBuilder;
					resolver = previousResolver;
				}
			}

			public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			{
				Visit(methodDeclaration, base.VisitMethodDeclaration);
			}

			public override void VisitAccessor(Accessor accessor)
			{
				Visit(accessor, base.VisitAccessor);
			}

			public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			{
				Visit(constructorDeclaration, base.VisitConstructorDeclaration);
			}

			public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			{
				Visit(destructorDeclaration, base.VisitDestructorDeclaration);
			}

			public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			{
				Visit(operatorDeclaration, base.VisitOperatorDeclaration);
			}

			void Visit<T>(T entityDeclaration, Action<T> baseCall) where T : EntityDeclaration
			{
				if (ignoreUsingScope)
				{
					baseCall(entityDeclaration);
					return;
				}
				if (entityDeclaration.GetSymbol() is IMethod method)
				{
					var previousResolver = resolver;
					var previousAstBuilder = astBuilder;
					if (CSharpDecompiler.IsWindowsFormsInitializeComponentMethod(method))
					{
						var currentContext = new CSharpTypeResolveContext(previousResolver.Compilation.MainModule);
						resolver = new CSharpResolver(currentContext);
					}
					else
					{
						resolver = resolver.WithCurrentMember(method);
					}
					try
					{
						var function = entityDeclaration.Annotation<IL.ILFunction>();
						astBuilder = CreateAstBuilder(resolver, function);
						baseCall(entityDeclaration);
					}
					finally
					{
						resolver = previousResolver;
						astBuilder = previousAstBuilder;
					}
				}
				else
				{
					baseCall(entityDeclaration);
				}
			}

			public override void VisitSimpleType(SimpleType simpleType)
			{
				TypeResolveResult rr;
				if ((rr = simpleType.Annotation<TypeResolveResult>()) == null)
				{
					base.VisitSimpleType(simpleType);
					return;
				}
				astBuilder.NameLookupMode = simpleType.GetNameLookupMode();
				if (astBuilder.NameLookupMode == NameLookupMode.Type)
				{
					AstType outermostType = simpleType;
					while (outermostType.Parent is AstType)
						outermostType = (AstType)outermostType.Parent;
					if (outermostType.Parent is TypeReferenceExpression)
					{
						// ILSpy uses TypeReferenceExpression in expression context even when the C# parser
						// wouldn't know that it's a type reference.
						// Fall back to expression-mode lookup in these cases:
						astBuilder.NameLookupMode = NameLookupMode.Expression;
					}
				}
				if (simpleType.Parent is Syntax.Attribute)
				{
					simpleType.ReplaceWith(astBuilder.ConvertAttributeType(rr.Type));
				}
				else
				{
					simpleType.ReplaceWith(astBuilder.ConvertType(rr.Type));
				}
			}
		}
	}
}
