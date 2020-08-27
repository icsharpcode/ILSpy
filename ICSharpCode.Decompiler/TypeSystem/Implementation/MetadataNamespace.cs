// Copyright (c) 2018 Daniel Grunwald
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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	sealed class MetadataNamespace : INamespace
	{
		readonly MetadataModule module;
		readonly NamespaceDefinition ns;

		public INamespace ParentNamespace { get; }
		public string FullName { get; }
		public string Name { get; }

		public MetadataNamespace(MetadataModule module, INamespace parent, string fullName, NamespaceDefinition ns)
		{
			Debug.Assert(module != null);
			Debug.Assert(fullName != null);
			this.module = module;
			this.ParentNamespace = parent;
			this.ns = ns;
			this.FullName = fullName;
			this.Name = module.GetString(ns.Name);
		}

		string INamespace.ExternAlias => string.Empty;

		INamespace[] childNamespaces;

		public IEnumerable<INamespace> ChildNamespaces {
			get {
				var children = LazyInit.VolatileRead(ref childNamespaces);
				if (children != null)
				{
					return children;
				}
				var nsDefs = ns.NamespaceDefinitions;
				children = new INamespace[nsDefs.Length];
				for (int i = 0; i < children.Length; i++)
				{
					var nsHandle = nsDefs[i];
					string fullName = module.metadata.GetString(nsHandle);
					children[i] = new MetadataNamespace(module, this, fullName,
						module.metadata.GetNamespaceDefinition(nsHandle));
				}
				return LazyInit.GetOrSet(ref childNamespaces, children);
			}
		}

		IEnumerable<ITypeDefinition> INamespace.Types {
			get {
				foreach (var typeHandle in ns.TypeDefinitions)
				{
					var def = module.GetDefinition(typeHandle);
					if (def != null)
						yield return def;
				}
			}
		}

		IEnumerable<IModule> INamespace.ContributingModules => new[] { module };

		SymbolKind ISymbol.SymbolKind => SymbolKind.Namespace;

		ICompilation ICompilationProvider.Compilation => module.Compilation;

		INamespace INamespace.GetChildNamespace(string name)
		{
			foreach (var ns in ChildNamespaces)
			{
				if (ns.Name == name)
					return ns;
			}
			return null;
		}

		ITypeDefinition INamespace.GetTypeDefinition(string name, int typeParameterCount)
		{
			return module.GetTypeDefinition(FullName, name, typeParameterCount);
		}
	}
}
