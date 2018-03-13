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
using System.Collections.ObjectModel;
using System.Diagnostics;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.TypeSystem
{
	/// <summary>
	/// Resolved version of using scope.
	/// </summary>
	public class ResolvedUsingScope
	{
		readonly CSharpTypeResolveContext parentContext;

		internal readonly ConcurrentDictionary<string, ResolveResult> ResolveCache = new ConcurrentDictionary<string, ResolveResult>();
		internal List<List<IMethod>> AllExtensionMethods;
		
		public ResolvedUsingScope(CSharpTypeResolveContext context, UsingScope usingScope)
		{
			this.parentContext = context ?? throw new ArgumentNullException("context");
			this.UnresolvedUsingScope = usingScope ?? throw new ArgumentNullException("usingScope");
			if (usingScope.Parent != null) {
				if (context.CurrentUsingScope == null)
					throw new InvalidOperationException();
			} else {
				if (context.CurrentUsingScope != null)
					throw new InvalidOperationException();
			}
		}
		
		public UsingScope UnresolvedUsingScope { get; }

		INamespace @namespace;
		
		public INamespace Namespace {
			get {
				var result = LazyInit.VolatileRead(ref this.@namespace);
				if (result != null) {
					return result;
				} else {
					if (parentContext.CurrentUsingScope != null) {
						result = parentContext.CurrentUsingScope.Namespace.GetChildNamespace(UnresolvedUsingScope.ShortNamespaceName);
						if (result == null)
							result = new DummyNamespace(parentContext.CurrentUsingScope.Namespace, UnresolvedUsingScope.ShortNamespaceName);
					} else {
						result = parentContext.Compilation.RootNamespace;
					}
					Debug.Assert(result != null);
					return LazyInit.GetOrSet(ref this.@namespace, result);
				}
			}
		}
		
		public ResolvedUsingScope Parent => parentContext.CurrentUsingScope;

		IList<INamespace> usings;
		
		public IList<INamespace> Usings {
			get {
				var result = LazyInit.VolatileRead(ref this.usings);
				if (result != null) {
					return result;
				} else {
					result = new List<INamespace>();
					var resolver = new CSharpResolver(parentContext.WithUsingScope(this));
					foreach (var u in UnresolvedUsingScope.Usings) {
						var ns = u.ResolveNamespace(resolver);
						if (ns != null && !result.Contains(ns))
							result.Add(ns);
					}
					return LazyInit.GetOrSet(ref this.usings, new ReadOnlyCollection<INamespace>(result));
				}
			}
		}
		
		IList<KeyValuePair<string, ResolveResult>> usingAliases;
		
		public IList<KeyValuePair<string, ResolveResult>> UsingAliases {
			get {
				var result = LazyInit.VolatileRead(ref this.usingAliases);
				if (result != null) {
					return result;
				} else {
					var resolver = new CSharpResolver(parentContext.WithUsingScope(this));
					result = new KeyValuePair<string, ResolveResult>[UnresolvedUsingScope.UsingAliases.Count];
					for (var i = 0; i < result.Count; i++) {
						var rr = UnresolvedUsingScope.UsingAliases[i].Value.Resolve(resolver);
						if (rr is TypeResolveResult) {
							rr = new AliasTypeResolveResult (UnresolvedUsingScope.UsingAliases[i].Key, (TypeResolveResult)rr);
						} else if (rr is NamespaceResolveResult) {
							rr = new AliasNamespaceResolveResult (UnresolvedUsingScope.UsingAliases[i].Key, (NamespaceResolveResult)rr);
						}
						result[i] = new KeyValuePair<string, ResolveResult>(
							UnresolvedUsingScope.UsingAliases[i].Key,
							rr
						);
					}
					return LazyInit.GetOrSet(ref this.usingAliases, result);
				}
			}
		}
		
		public IList<string> ExternAliases => UnresolvedUsingScope.ExternAliases;

		/// <summary>
		/// Gets whether this using scope has an alias (either using or extern)
		/// with the specified name.
		/// </summary>
		public bool HasAlias(string identifier)
		{
			return UnresolvedUsingScope.HasAlias(identifier);
		}
		
		sealed class DummyNamespace : INamespace
		{
			readonly INamespace parentNamespace;

			public DummyNamespace(INamespace parentNamespace, string name)
			{
				this.parentNamespace = parentNamespace;
				this.Name = name;
			}
			
			public string ExternAlias { get; set; }
			
			string INamespace.FullName => NamespaceDeclaration.BuildQualifiedName(parentNamespace.FullName, Name);

			public string Name { get; }

			SymbolKind ISymbol.SymbolKind => SymbolKind.Namespace;

			INamespace INamespace.ParentNamespace => parentNamespace;

			IEnumerable<INamespace> INamespace.ChildNamespaces => EmptyList<INamespace>.Instance;

			IEnumerable<ITypeDefinition> INamespace.Types => EmptyList<ITypeDefinition>.Instance;

			IEnumerable<IAssembly> INamespace.ContributingAssemblies => EmptyList<IAssembly>.Instance;

			ICompilation ICompilationProvider.Compilation => parentNamespace.Compilation;

			INamespace INamespace.GetChildNamespace(string name)
			{
				return null;
			}
			
			ITypeDefinition INamespace.GetTypeDefinition(string name, int typeParameterCount)
			{
				return null;
			}

			public ISymbolReference ToReference()
			{
				return new MergedNamespaceReference(ExternAlias, ((INamespace)this).FullName);
			}
		}
	}
}
