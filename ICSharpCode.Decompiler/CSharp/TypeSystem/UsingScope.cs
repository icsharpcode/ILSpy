// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.TypeSystem
{
	/// <summary>
	/// Represents a scope that contains "using" statements.
	/// This is either the mo itself, or a namespace declaration.
	/// </summary>
	public class UsingScope
	{
		readonly CSharpTypeResolveContext parentContext;

		internal readonly ConcurrentDictionary<string, ResolveResult> ResolveCache = new ConcurrentDictionary<string, ResolveResult>();
		internal List<List<IMethod>>? AllExtensionMethods;

		public UsingScope(CSharpTypeResolveContext context, INamespace @namespace, ImmutableArray<INamespace> usings)
		{
			this.parentContext = context ?? throw new ArgumentNullException(nameof(context));
			this.Usings = usings;
			this.Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
		}

		public INamespace Namespace { get; }

		public UsingScope Parent {
			get { return parentContext.CurrentUsingScope; }
		}

		public ImmutableArray<INamespace> Usings { get; }

		public IReadOnlyList<KeyValuePair<string, ResolveResult>> UsingAliases => [];

		public IReadOnlyList<string> ExternAliases => [];

		/// <summary>
		/// Gets whether this using scope has an alias (either using or extern)
		/// with the specified name.
		/// </summary>
		public bool HasAlias(string identifier) => false;

		internal UsingScope WithNestedNamespace(string simpleName)
		{
			var ns = Namespace.GetChildNamespace(simpleName) ?? new DummyNamespace(Namespace, simpleName);
			return new UsingScope(
				parentContext.WithUsingScope(this),
				ns,
				[]);
		}

		sealed class DummyNamespace : INamespace
		{
			readonly INamespace parentNamespace;
			readonly string name;

			public DummyNamespace(INamespace parentNamespace, string name)
			{
				this.parentNamespace = parentNamespace;
				this.name = name;
			}

			string INamespace.ExternAlias => "";

			string INamespace.FullName {
				get { return NamespaceDeclaration.BuildQualifiedName(parentNamespace.FullName, name); }
			}

			public string Name {
				get { return name; }
			}

			SymbolKind ISymbol.SymbolKind {
				get { return SymbolKind.Namespace; }
			}

			INamespace INamespace.ParentNamespace {
				get { return parentNamespace; }
			}

			IEnumerable<INamespace> INamespace.ChildNamespaces {
				get { return EmptyList<INamespace>.Instance; }
			}

			IEnumerable<ITypeDefinition> INamespace.Types {
				get { return EmptyList<ITypeDefinition>.Instance; }
			}

			IEnumerable<IModule> INamespace.ContributingModules {
				get { return EmptyList<IModule>.Instance; }
			}

			ICompilation ICompilationProvider.Compilation {
				get { return parentNamespace.Compilation; }
			}

			INamespace? INamespace.GetChildNamespace(string name)
			{
				return null;
			}

			ITypeDefinition? INamespace.GetTypeDefinition(string name, int typeParameterCount)
			{
				return null;
			}
		}
	}
}
