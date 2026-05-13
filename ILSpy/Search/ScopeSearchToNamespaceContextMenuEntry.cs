// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.Properties;

using ILSpy.TreeNodes;

namespace ILSpy.Search
{
	/// <summary>
	/// Right-click a <see cref="NamespaceTreeNode"/> → "Search within this namespace".
	/// Prepends <c>innamespace:&lt;namespace&gt;</c> to the live search term so subsequent
	/// matches are restricted to that namespace. Same parser/filter path as
	/// <see cref="ScopeSearchToAssemblyContextMenuEntry"/>.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ScopeSearchToThisNamespace), Order = 1101)]
	[Shared]
	public sealed class ScopeSearchToNamespaceContextMenuEntry : IContextMenuEntry
	{
		readonly SearchPaneModel searchPane;

		[ImportingConstructor]
		public ScopeSearchToNamespaceContextMenuEntry(SearchPaneModel searchPane)
		{
			this.searchPane = searchPane;
		}

		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.All(n => n is NamespaceTreeNode);
		}

		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return;
			var ns = nodes.OfType<NamespaceTreeNode>().FirstOrDefault();
			if (ns == null || string.IsNullOrEmpty(ns.Name))
				return;
			searchPane.SearchTerm = ScopeSearchToAssemblyContextMenuEntry.MergeScopePrefix(
				searchPane.SearchTerm, "innamespace", ns.Name);
		}
	}
}
