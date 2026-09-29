// Copyright (c) 2018 Siegfried Pammer
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
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler
{
	internal class DecompileRun
	{
		public HashSet<string> DefinedSymbols { get; } = new HashSet<string>();
		public HashSet<string> Namespaces { get; set; }
		public CancellationToken CancellationToken { get; set; }
		public DecompilerSettings Settings { get; }
		public IDocumentationProvider DocumentationProvider { get; set; }
		public Dictionary<ITypeDefinition, RecordDecompiler> RecordDecompilers { get; } = new Dictionary<ITypeDefinition, RecordDecompiler>();

#nullable enable
		/// <summary>
		/// Memoized AutoEventDecompiler verdicts (null = the event is not automatic). Shared so
		/// that every consumer of the verdict (member hiding, the event declaration, reference
		/// translation) decides from the same analysis.
		/// </summary>
		public Dictionary<IEvent, IField?> AutomaticEvents { get; } = new Dictionary<IEvent, IField?>();
#nullable restore

		public Dictionary<ITypeDefinition, bool> TypeHierarchyIsKnown { get; } = new();

		public UsingScope UsingScope { get; }

		readonly Dictionary<string, UsingScope> nestedUsingScopes = new Dictionary<string, UsingScope>();

		/// <summary>
		/// <see cref="UsingScope"/> as seen from inside <paramref name="namespaceName"/>.
		/// C# looks for extension methods one namespace at a time, innermost first, so a scope that
		/// is not nested into the namespace the code is written in lets an extension method declared
		/// there compete with merely imported ones instead of beating them. Memoized, because the
		/// list of extension methods a scope can reach is built once per scope.
		/// </summary>
		public UsingScope GetUsingScopeFor(string namespaceName)
		{
			if (string.IsNullOrEmpty(namespaceName))
				return UsingScope;
			if (!nestedUsingScopes.TryGetValue(namespaceName, out var scope))
			{
				scope = UsingScope;
				foreach (string part in namespaceName.Split('.'))
				{
					scope = scope.WithNestedNamespace(part);
				}
				nestedUsingScopes.Add(namespaceName, scope);
			}
			return scope;
		}

		public DecompileRun(DecompilerSettings settings, UsingScope usingScope)
		{
			this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
			this.UsingScope = usingScope ?? throw new ArgumentNullException(nameof(usingScope));
		}
	}

	enum EnumValueDisplayMode
	{
		None,
		All,
		AllHex,
		FirstOnly
	}
}
