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

using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Right-click on a top-level analysed entity → "Remove" — drops the row from the
	/// pane root. Only visible on rows whose parent is the analyzer root (the per-entity
	/// rows); search-tree-node headers and result rows can't be removed individually.
	/// </summary>
	[ExportContextMenuEntry(Header = "Remove", Icon = "Images/Delete", Order = 9200)]
	[Shared]
	public sealed class RemoveAnalyzeContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.All(IsRemovableRoot);
		}

		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return;
			foreach (var node in nodes.Where(IsRemovableRoot).ToArray())
				node.Parent?.Children.Remove(node);
		}

		static bool IsRemovableRoot(SharpTreeNode node)
			=> node is AnalyzerTreeNode and AnalyzerEntityTreeNode
				&& node.Parent is { IsRoot: true };
	}
}
