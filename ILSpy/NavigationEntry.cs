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

using System;
using System.Runtime.CompilerServices;

using Avalonia.Media;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>
	/// One stop on the back/forward stack: a (tab, target) pair where the target is either a
	/// tree-node selection or a static-page URI. The tab pointer lets navigation jump back to
	/// the document the user was viewing when this entry was recorded.
	/// </summary>
	public abstract class NavigationEntry : IEquatable<NavigationEntry?>
	{
		public TabPageModel Tab { get; }

		protected NavigationEntry(TabPageModel tab)
		{
			Tab = tab ?? throw new ArgumentNullException(nameof(tab));
		}

		/// <summary>Header text shown in the back/forward dropdown.</summary>
		public abstract object DisplayText { get; }

		/// <summary>Icon shown next to the entry in the back/forward dropdown.</summary>
		public virtual IImage? DisplayIcon => null;

		public abstract bool Equals(NavigationEntry? other);

		public override bool Equals(object? obj) => Equals(obj as NavigationEntry);

		public abstract override int GetHashCode();
	}

	/// <summary>
	/// History entry for a tree-node selection. Replaying activates the recorded tab and sets
	/// the assembly-tree selection to <see cref="Node"/>. The optional caret + scroll fields
	/// capture the editor's view state when the user navigated AWAY from this entry, so
	/// Back/Forward restores cursor position and scroll position alongside the selection.
	/// </summary>
	public sealed class TreeNodeEntry : NavigationEntry
	{
		public SharpTreeNode Node { get; }

		/// <summary>
		/// Caret offset (character index into the decompiled text) captured when the user
		/// navigated away from this entry. <c>null</c> means "no capture yet" — entries
		/// land with null and are filled in by <see cref="Docking.DockWorkspace"/>'s
		/// record-history hook just before a new entry takes over.
		/// </summary>
		public int? CaretOffset { get; set; }

		/// <summary>Vertical scroll offset captured at navigate-away time.</summary>
		public double? VerticalOffset { get; set; }

		/// <summary>Horizontal scroll offset captured at navigate-away time.</summary>
		public double? HorizontalOffset { get; set; }

		/// <summary>
		/// Snapshot of which code-folding regions the user had expanded when navigating
		/// away from this entry. <c>null</c> means "no capture yet" (entries land null and
		/// are populated by the record-history hook). Restoration is checksum-gated, so a
		/// snapshot saved against a different document is safely ignored at Back-apply time.
		/// </summary>
		public FoldingsViewState.Snapshot? Foldings { get; set; }

		/// <summary>
		/// For a metadata-table node reached via "Go to token", the 0-based row the grid was
		/// scrolled to. Restored on Back/Forward so returning to this entry lands on the exact
		/// token, not just the top of the table. <c>null</c> for ordinary tree-node selections.
		/// </summary>
		public int? MetadataRow { get; set; }

		public TreeNodeEntry(TabPageModel tab, SharpTreeNode node)
			: base(tab)
		{
			Node = node ?? throw new ArgumentNullException(nameof(node));
		}

		// ILSpyTreeNode exposes a richer NavigationText for nodes whose generic Text ("Base
		// Types", "Derived Types", "References", …) reads ambiguously without context. Plain
		// SharpTreeNode entries fall back to Text.
		public override object DisplayText => (Node is ILSpyTreeNode ilspy ? ilspy.NavigationText : Node.Text) ?? string.Empty;

		public override IImage? DisplayIcon => Node.Icon as IImage;

		public override bool Equals(NavigationEntry? other) =>
			other is TreeNodeEntry t
			&& ReferenceEquals(t.Tab, Tab)
			&& ReferenceEquals(t.Node, Node);

		public override int GetHashCode() => HashCode.Combine(
			RuntimeHelpers.GetHashCode(Tab),
			RuntimeHelpers.GetHashCode(Node));
	}

	/// <summary>
	/// History entry for a static page (e.g. the About tab). Replaying just reactivates the
	/// recorded tab — the tab keeps its content because static pages opt out of being
	/// targeted by tree-node decompilation.
	/// </summary>
	public sealed class StaticPageEntry : NavigationEntry
	{
		public Uri Uri { get; }

		public StaticPageEntry(TabPageModel tab, Uri uri)
			: base(tab)
		{
			Uri = uri ?? throw new ArgumentNullException(nameof(uri));
		}

		public override object DisplayText => Tab.Title ?? Uri.ToString();

		public override bool Equals(NavigationEntry? other) =>
			other is StaticPageEntry s
			&& ReferenceEquals(s.Tab, Tab)
			&& Equals(s.Uri, Uri);

		public override int GetHashCode() => HashCode.Combine(
			RuntimeHelpers.GetHashCode(Tab),
			Uri);
	}
}
