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

using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Controls.Omnibar
{
	/// <summary>
	/// One crumb in the omnibar breadcrumb: a tree node rendered as icon + label. The
	/// <see cref="Node"/> is the live <see cref="SharpTreeNode"/> the crumb stands for, so clicking
	/// the crumb (or one of its <see cref="Children"/> from the chevron dropdown) navigates the
	/// assembly tree straight there. Used both for the trail itself and for the sibling entries the
	/// chevron lists.
	/// </summary>
	public sealed class BreadcrumbSegment
	{
		public BreadcrumbSegment(SharpTreeNode node)
		{
			Node = node;
			Text = node.Text?.ToString() ?? string.Empty;
			Icon = node.Icon;
		}

		/// <summary>The tree node this crumb represents; navigating selects it.</summary>
		public SharpTreeNode Node { get; }

		/// <summary>Display label, taken from <see cref="SharpTreeNode.Text"/>.</summary>
		public string Text { get; }

		/// <summary>Display icon (an <c>IImage</c>), taken from <see cref="SharpTreeNode.Icon"/>.</summary>
		public object? Icon { get; }

		/// <summary>
		/// Whether the chevron dropdown has anything to show. Reuses the same predicate the tree uses
		/// for its expander (<see cref="SharpTreeNode.ShowExpander"/>: lazy-loadable, or has visible
		/// children), so leaf crumbs such as methods and fields drop the chevron instead of opening an
		/// empty list.
		/// </summary>
		public bool HasChildren => Node.ShowExpander;
	}
}
