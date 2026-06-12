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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Search
{
	/// <summary>
	/// Right-click a <see cref="NamespaceTreeNode"/> → "Search within this namespace".
	/// Prepends <c>innamespace:&lt;namespace&gt;</c> to the live search term so subsequent
	/// matches are restricted to that namespace. Same parser/filter path as
	/// <see cref="ScopeSearchToAssemblyContextMenuEntry"/>.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ScopeSearchToThisNamespace), Category = nameof(Resources.Analyze), Icon = "Images/Namespace", Order = 220)]
	[Shared]
	public sealed class ScopeSearchToNamespaceContextMenuEntry : IContextMenuEntry
	{
		readonly SearchPaneModel searchPane;

		[ImportingConstructor]
		public ScopeSearchToNamespaceContextMenuEntry(SearchPaneModel searchPane)
		{
			this.searchPane = searchPane;
		}

		public bool IsVisible(TextViewContext context) => !string.IsNullOrEmpty(Namespace(context));

		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public void Execute(TextViewContext context)
		{
			if (Namespace(context) is { Length: > 0 } ns)
				searchPane.SearchTerm = ScopeSearchToAssemblyContextMenuEntry.MergeScopePrefix(
					searchPane.SearchTerm, "innamespace", ns);
		}

		// The namespace to scope to: a selected NamespaceTreeNode in the tree, or the namespace of the
		// symbol under a right-clicked code reference. The empty (global) namespace doesn't scope.
		static string? Namespace(TextViewContext context)
		{
			if (context.SelectedTreeNodes is { Length: > 0 } nodes && nodes.All(n => n is NamespaceTreeNode))
				return nodes.OfType<NamespaceTreeNode>().FirstOrDefault()?.Name;
			return (context.Reference?.Reference as IEntity)?.Namespace;
		}
	}
}
