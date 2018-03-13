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
using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Namespace node. The loading of the type nodes is handled by the parent AssemblyTreeNode.
	/// </summary>
	public sealed class NamespaceTreeNode : ILSpyTreeNode
	{
		public string Name { get; }

		public NamespaceTreeNode(string name)
		{
			this.Name = name ?? throw new ArgumentNullException(nameof(name));
		}
		
		public override object Text => HighlightSearchMatch(Name.Length == 0 ? "-" : Name);

		public override object Icon => Images.Namespace;

		public override FilterResult Filter(FilterSettings settings)
		{
			return settings.SearchTermMatches(Name) ? FilterResult.MatchAndRecurse : FilterResult.Recurse;
		}
		
		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.DecompileNamespace(Name, this.Children.OfType<TypeTreeNode>().Select(t => t.TypeDefinition), output, options);
		}
	}
}
