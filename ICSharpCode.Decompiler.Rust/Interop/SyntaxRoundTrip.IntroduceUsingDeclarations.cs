// Copyright (c) 2026 Christoph Wille
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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answers and annotation kinds of the engine's <c>IntroduceUsingDeclarations</c>
	/// (<c>csharp/transforms/introduce_using_declarations.rs</c>, which documents each kind).
	/// <para>
	/// The engine walks the tree; the resolver stays here. A resolver scope (the using scope, the
	/// current type definition, the current member and its variables) is kept in
	/// <see cref="usingScopes"/> and named by its index; the engine derives one scope from another
	/// at each namespace, type and member declaration and asks for the short form of a type
	/// reference in a scope, which travels as a subtree in the wire format whose handles index
	/// <see cref="minted"/> (annotation kind <see cref="AnnotationMinted"/>).
	/// </para>
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte IntroduceUsingDeclarationsFirstQuery = 140;
		const byte QueryIudCurrentNamespace = 140;
		const byte QueryIudImport = 141;
		const byte QueryIudUsingScope = 142;
		const byte QueryIudScope = 143;
		const byte QueryIudConvert = 144;
		const byte IntroduceUsingDeclarationsLastQuery = 144;

		const byte AnnotationUsingScope = 140;

		UsingScope? usingScope;
		readonly List<(CSharpResolver Resolver, TypeSystemAstBuilder Builder)> usingScopes = new();

		TransformContext IudContext => context ?? throw new RustEngineException("IntroduceUsingDeclarations needs the transform context");

		object? CreateIntroduceUsingDeclarationsAnnotation(byte kind)
		{
			if (kind != AnnotationUsingScope)
				return null;
			return usingScope ?? throw new RustEngineException("the engine attached a using scope before asking for it");
		}

		byte[]? AnswerIntroduceUsingDeclarations(byte[] query)
		{
			int pos = 1;
			var s = new MemoryStream();
			switch (query[0])
			{
				case QueryIudCurrentNamespace:
					WriteUtf8(s, IudContext.CurrentTypeDefinition?.Namespace ?? string.Empty);
					break;
				case QueryIudImport:
				{
					int handle = (int)ReadUVar(query, ref pos);
					if (handle < 0 || handle >= handles.Count)
						return null;
					if (!ImportedType(handles[handle], query[pos], out var type))
						return null;
					if (type == null)
					{
						s.WriteByte(0);
					}
					else
					{
						s.WriteByte(1);
						WriteUtf8(s, type.Namespace);
					}
					break;
				}
				case QueryIudUsingScope:
				{
					var context = IudContext;
					int count = (int)ReadUVar(query, ref pos);
					var imports = new List<string>();
					for (int i = 0; i < count; i++)
						imports.Add(ReadUtf8(query, ref pos));
					var resolvedNamespaces = new List<INamespace>();
					var sorted = imports
						.OrderBy(n => n.StartsWith("System", StringComparison.Ordinal))
						.ThenByDescending(n => n)
						.ToList();
					UVar(s, (ulong)sorted.Count);
					foreach (string ns in sorted)
					{
						DecompilerAssert.That(context.RequiredNamespacesSuperset.Contains(ns), $"Should not insert using declaration for namespace that is missing from the superset: {ns}");
						if (context.TypeSystem.GetNamespaceByFullName(ns) is { } resolvedNamespace)
							resolvedNamespaces.Add(resolvedNamespace);
						WriteUtf8(s, ns);
					}
					usingScope = new UsingScope(
						new CSharpTypeResolveContext(context.TypeSystem.MainModule),
						context.TypeSystem.RootNamespace,
						resolvedNamespaces.ToImmutableArray()
					);
					usingScopes.Clear();
					var resolver = new CSharpResolver(new CSharpTypeResolveContext(context.TypeSystem.MainModule));
					if (context.Settings.UsingDeclarations)
					{
						var scope = usingScope;
						if (!string.IsNullOrEmpty(context.CurrentTypeDefinition?.Namespace))
						{
							foreach (string ns in context.CurrentTypeDefinition.Namespace.Split('.'))
								scope = scope.WithNestedNamespace(ns);
						}
						resolver = resolver.WithCurrentUsingScope(scope)
							.WithCurrentTypeDefinition(context.CurrentTypeDefinition);
					}
					usingScopes.Add((resolver, CreateIudAstBuilder(resolver)));
					break;
				}
				case QueryIudScope:
				{
					var (resolver, builder) = IudScopeAt((int)ReadUVar(query, ref pos));
					byte frame = query[pos++];
					int count = (int)ReadUVar(query, ref pos);
					if (frame == 0)
					{
						var scope = resolver.CurrentUsingScope;
						for (int i = 0; i < count; i++)
							scope = scope.WithNestedNamespace(ReadUtf8(query, ref pos));
						resolver = resolver.WithCurrentUsingScope(scope);
						builder = CreateIudAstBuilder(resolver);
					}
					else
					{
						var annotations = new List<object>();
						for (int i = 0; i < count; i++)
						{
							int h = (int)ReadUVar(query, ref pos);
							if (h < handles.Count)
								annotations.Add(handles[h]);
						}
						var rr = annotations.OfType<ResolveResult>().FirstOrDefault();
						var symbol = rr is MethodGroupResolveResult mgrr ? mgrr.ChosenMethod : rr?.GetSymbol();
						if (frame == 1)
						{
							resolver = resolver.WithCurrentTypeDefinition(symbol as ITypeDefinition);
							builder = CreateIudAstBuilder(resolver);
						}
						else if (symbol is IMethod method)
						{
							resolver = CSharpDecompiler.IsWindowsFormsInitializeComponentMethod(method)
								? new CSharpResolver(new CSharpTypeResolveContext(resolver.Compilation.MainModule))
								: resolver.WithCurrentMember(method);
							builder = CreateIudAstBuilder(resolver, annotations.OfType<ILFunction>().FirstOrDefault());
						}
					}
					usingScopes.Add((resolver, builder));
					UVar(s, (ulong)(usingScopes.Count - 1));
					break;
				}
				case QueryIudConvert:
				{
					var (_, builder) = IudScopeAt((int)ReadUVar(query, ref pos));
					int handle = (int)ReadUVar(query, ref pos);
					if (handle < 0 || handle >= handles.Count || handles[handle] is not TypeResolveResult rr)
						return null;
					builder.NameLookupMode = (NameLookupMode)query[pos++];
					bool attribute = query[pos++] != 0;
					WriteMinted(s, attribute ? builder.ConvertAttributeType(rr.Type) : builder.ConvertType(rr.Type));
					break;
				}
				default:
					return null;
			}
			return s.ToArray();
		}

		(CSharpResolver Resolver, TypeSystemAstBuilder Builder) IudScopeAt(int id)
			=> id >= 0 && id < usingScopes.Count ? usingScopes[id] : throw new RustEngineException($"no resolver scope {id}");

		/// <summary>
		/// The type a <see cref="QueryIudImport"/> rule takes from <paramref name="annotation"/>;
		/// false where the annotation is not of the class the rule reads (so the engine looks at the
		/// node's next annotation, as <c>Annotation&lt;T&gt;</c> does), null where it is but no
		/// namespace is imported.
		/// </summary>
		static bool ImportedType(object annotation, byte rule, out IType? type)
		{
			type = null;
			switch (rule)
			{
				case 0:
					if (annotation is not TypeResolveResult trr)
						return false;
					type = trr.Type;
					return true;
				case 1:
					if (annotation is not ResolveResult rr)
						return false;
					if (rr is CSharpInvocationResolveResult { IsExtensionMethodInvocation: true, Member: var extensionMethod })
						type = extensionMethod.DeclaringType;
					return true;
				case 2:
					if (annotation is not ForeachAnnotation foreachAnnotation)
						return false;
					if (foreachAnnotation.GetEnumeratorCall is CallInstruction { Method: { IsExtensionMethod: true, DeclaringType: var declaringType } })
						type = declaringType;
					return true;
				case 3:
					if (annotation is not MatchInstruction match)
						return false;
					if (match.Method is IMethod { DeclaringType: var matchType })
						type = matchType;
					return true;
				case 4:
					if (annotation is not CallInstruction call)
						return false;
					if (call.Method is { IsExtensionMethod: true, Name: "Add" })
						type = call.Method.DeclaringType;
					return true;
				default:
					throw new RustEngineException($"unknown import rule {rule}");
			}
		}

		TypeSystemAstBuilder CreateIudAstBuilder(CSharpResolver resolver, ILFunction? function = null)
		{
			var settings = IudContext.Settings;
			if (function != null)
			{
				var variables = new Dictionary<string, IVariable>();
				foreach (var v in function.Variables)
				{
					if (v.Kind != VariableKind.Parameter && v.Name != null && !variables.ContainsKey(v.Name))
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
	}
}
