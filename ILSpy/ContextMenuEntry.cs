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
using System.Composition;

using Avalonia;
using Avalonia.Controls;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// MEF-exported entry that contributes a single item to a context menu. Exports are
	/// discovered via <see cref="ExportContextMenuEntryAttribute"/> and surfaced through
	/// <see cref="ContextMenuProvider.Build"/>, which calls back into <see cref="IsVisible"/>
	/// (filters the menu), <see cref="IsEnabled"/> (greys the item out), and
	/// <see cref="Execute"/> (runs the action) with the live <see cref="TextViewContext"/>.
	/// </summary>
	public interface IContextMenuEntry
	{
		bool IsVisible(TextViewContext context);
		bool IsEnabled(TextViewContext context);
		void Execute(TextViewContext context);
	}

	/// <summary>
	/// Snapshot of the surface that opened a context menu (which control, the selection in it,
	/// the reference under the pointer if any). Entries inspect the populated fields to decide
	/// visibility / enabled state and to scope their action.
	/// </summary>
	public class TextViewContext
	{
		/// <summary>The selected nodes in the assembly-tree DataGrid, or null when the menu wasn't opened on the tree.</summary>
		public SharpTreeNode[]? SelectedTreeNodes { get; init; }

		/// <summary>The tree control the menu was opened on (a DataGrid or the SharpTreeView), or null otherwise.</summary>
		public Control? TreeGrid { get; init; }

		/// <summary>The decompiler text view the menu was opened on, or null otherwise.</summary>
		public DecompilerTextView? TextView { get; init; }

		/// <summary>The list box (e.g. search results) the menu was opened on, or null otherwise.</summary>
		public ListBox? ListBox { get; init; }

		/// <summary>The data grid (e.g. metadata table) the menu was opened on, or null otherwise.</summary>
		public DataGrid? DataGrid { get; init; }

		/// <summary>The reference under the pointer in the text view, or the focused list/grid item, when applicable.</summary>
		public ReferenceSegment? Reference { get; init; }

		/// <summary>The document offset under the pointer at the time the text-view menu opened, or null
		/// when the menu wasn't opened on the text view (or the click missed the text). Entries that act on
		/// a position (e.g. toggle the fold under the click) use this rather than the caret, which may sit
		/// on an entirely different line than the one the user right-clicked.</summary>
		public int? TextLocation { get; init; }

		/// <summary>The visual element the original event came from — useful for context-aware entries that need to inspect the click target.</summary>
		public Visual? OriginalSource { get; init; }
	}

	/// <summary>
	/// Concrete metadata view for context-menu entries. Mirrors WPF's
	/// <c>IContextMenuEntryMetadata</c>; <see cref="System.Composition"/> requires a class
	/// (not an interface) for metadata views.
	/// </summary>
	public class ContextMenuEntryMetadata
	{
		public string? MenuID { get; set; }
		public string? ParentMenuID { get; set; }
		public string? Icon { get; set; }
		public string? Header { get; set; }
		public string? Category { get; set; }
		public string? InputGestureText { get; set; }
		public double Order { get; set; } = double.MaxValue;
	}

	/// <summary>
	/// Marks a class as a context-menu entry discoverable through MEF. The metadata properties
	/// drive where the entry appears in the menu hierarchy and what it looks like.
	/// </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ExportContextMenuEntryAttribute : ExportAttribute
	{
		public ExportContextMenuEntryAttribute()
			: base("ContextMenuEntry", typeof(IContextMenuEntry))
		{
			// Entries default to end-of-menu unless explicitly ordered.
			Order = double.MaxValue;
		}

		/// <summary>
		/// Optional ID that identifies this entry as a sub-menu parent. Other entries can set
		/// <see cref="ParentMenuID"/> to this value to nest under it. Avoid cycles
		/// (<c>MenuID == ParentMenuID</c>); they would loop forever during menu construction.
		/// </summary>
		public string? MenuID { get; set; }

		/// <summary>
		/// Optional ID of the parent sub-menu. <see langword="null"/> places the entry at the
		/// top level of the context menu.
		/// </summary>
		public string? ParentMenuID { get; set; }

		public string? Icon { get; set; }
		public string? Header { get; set; }
		public string? Category { get; set; }
		public string? InputGestureText { get; set; }
		public double Order { get; set; }
	}
}
