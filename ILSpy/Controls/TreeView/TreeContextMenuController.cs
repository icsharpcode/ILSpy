// Copyright (c) 2026 Christoph Wille
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
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>
	/// Hosts the registry-built context menu of a <see cref="SharpTreeView"/> with Thunderbird-style
	/// targeting, shared by every tree pane: a right-click on a row outside the selection opens the
	/// menu for that row alone without moving the selection (the right press is swallowed so the
	/// ListBox does not select the row), the target row carries the <c>contextTarget</c> highlight
	/// while the menu is open, and a keyboard-invoked menu (Shift+F10 / Apps) adopts the focused row
	/// and hands the focus back to it when the popup closes.
	/// </summary>
	public sealed class TreeContextMenuController
	{
		readonly SharpTreeView tree;
		readonly Func<IReadOnlyList<SharpTreeNode>> modelSelection;
		IReadOnlyList<IContextMenuEntryExport> entries = Array.Empty<IContextMenuEntryExport>();

		// The row whose context menu is open, highlighted without moving the real selection.
		SharpTreeViewItem? contextTargetItem;
		SharpTreeViewItem? contextMenuOpenItem;
		SharpTreeNode? contextMenuTargetNode;
		// For a keyboard-invoked menu, the row to re-focus when the menu closes (closing the popup
		// otherwise drops the keyboard focus and its focus adorner). Null for pointer-invoked menus.
		SharpTreeViewItem? focusToRestoreAfterMenu;
		// Whether the last ContextRequested came from the keyboard (no pointer position). The keyboard
		// path carries no target row, so the menu adopts the focused row (see OnContextMenuOpening).
		bool lastContextRequestWasKeyboard;

		/// <param name="modelSelection">The pane's model selection, read when a menu is built.</param>
		public TreeContextMenuController(SharpTreeView tree, Func<IReadOnlyList<SharpTreeNode>> modelSelection)
		{
			this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
			this.modelSelection = modelSelection ?? throw new ArgumentNullException(nameof(modelSelection));
			tree.AddHandler(InputElement.PointerPressedEvent, OnTreePointerPressed, RoutingStrategies.Tunnel);
			tree.AddHandler(Control.ContextRequestedEvent, OnTreeContextRequested, RoutingStrategies.Bubble, handledEventsToo: true);
		}

		/// <summary>Installs a context menu on the tree that is rebuilt from <paramref name="entries"/> every time it opens.</summary>
		public void Attach(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			this.entries = entries;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			menu.Closed += (_, _) => {
				RestoreFocusAfterKeyboardMenu();
				if (!ReferenceEquals(contextTargetItem, contextMenuOpenItem))
					return;
				contextMenuTargetNode = null;
				SetContextTargetItem(null);
			};
			tree.ContextMenu = menu;
		}

		void OnContextMenuOpening(object? sender, CancelEventArgs e)
		{
			if (sender is not ContextMenu menu)
				return;
			// A keyboard-invoked menu carries no pointer position, so OnTreeContextRequested set no
			// transient target. Adopt the keyboard-FOCUSED row (which may differ from the selection
			// after Ctrl+Arrow) as the target: opening the popup steals focus and drops the row's focus
			// adorner, so we mark that row with the same context-target highlight the mouse gives the
			// right-clicked row, and restore its focus + adorner on close (Avalonia's ContextMenu does not).
			// Captured here, before the popup opens and takes focus (Opening fires ahead of it), and before
			// contextMenuOpenItem is latched so the Closed handler still clears the highlight.
			var focusedRow = TopLevel.GetTopLevel(tree)?.FocusManager?.GetFocusedElement() as SharpTreeViewItem;
			if (lastContextRequestWasKeyboard && focusedRow?.Node != null)
			{
				contextMenuTargetNode = focusedRow.Node;
				SetContextTargetItem(focusedRow);
				focusToRestoreAfterMenu = focusedRow;
			}
			contextMenuOpenItem = contextTargetItem;
			var built = Build(entries);
			if (built == null)
			{
				// Menu won't open (so Closed won't fire) -- undo the transient target + captured focus.
				focusToRestoreAfterMenu = null;
				contextMenuTargetNode = null;
				SetContextTargetItem(null);
				e.Cancel = true;
				return;
			}
			menu.Items.Clear();
			foreach (var item in built.Items.OfType<Control>().ToArray())
			{
				built.Items.Remove(item);
				menu.Items.Add(item);
			}
		}

		void RestoreFocusAfterKeyboardMenu()
		{
			if (focusToRestoreAfterMenu is not { } toFocus)
				return;
			focusToRestoreAfterMenu = null;
			// Re-focus with a keyboard navigation method so the focus visual (the adorner) comes back,
			// not just the logical focus. Posted so it runs after the popup has fully torn down.
			Dispatcher.UIThread.Post(() => toFocus.Focus(NavigationMethod.Tab));
		}

		/// <summary>Builds the menu for the current selection, as the live Opening event would.</summary>
		public ContextMenu? Build(IReadOnlyList<IContextMenuEntryExport> entries)
			=> ContextMenuProvider.Build(entries, CreateContext());

		/// <summary>Builds the menu as a right-click on <paramref name="rightClickedNode"/> would.</summary>
		public ContextMenu? Build(IReadOnlyList<IContextMenuEntryExport> entries, SharpTreeNode? rightClickedNode)
		{
			contextMenuTargetNode = rightClickedNode;
			try
			{
				return ContextMenuProvider.Build(entries, CreateContext());
			}
			finally
			{
				contextMenuTargetNode = null;
			}
		}

		TextViewContext CreateContext()
		{
			var selection = modelSelection().ToArray();
			// A right-click outside the selection targets just the clicked row; inside the selection
			// (or a keyboard-invoked menu with no target) acts on the whole selection.
			var target = contextMenuTargetNode;
			var nodes = target != null && !Array.Exists(selection, n => ReferenceEquals(n, target))
				? new[] { target }
				: selection;
			return new TextViewContext {
				TreeGrid = tree,
				SelectedTreeNodes = nodes,
			};
		}

		void SetContextTargetItem(SharpTreeViewItem? item)
		{
			if (ReferenceEquals(contextTargetItem, item))
				return;
			contextTargetItem?.Classes.Remove("contextTarget");
			contextTargetItem = item;
			contextTargetItem?.Classes.Add("contextTarget");
		}

		void OnTreeContextRequested(object? sender, ContextRequestedEventArgs e)
		{
			SharpTreeViewItem? item = null;
			// Keyboard invocation (Shift+F10 / Apps) raises ContextRequested with no pointer position.
			lastContextRequestWasKeyboard = !e.TryGetPosition(tree, out var pos);
			if (!lastContextRequestWasKeyboard && tree.InputHitTest(pos) is Visual hit)
				item = hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true);
			contextMenuTargetNode = item?.Node;
			SetContextTargetItem(item?.Node != null ? item : null);
		}

		void OnTreePointerPressed(object? sender, PointerPressedEventArgs e)
		{
			if (e.Source is not Visual hit)
				return;
			if (e.GetCurrentPoint(hit).Properties.IsRightButtonPressed)
			{
				// Swallow the right press so the ListBox doesn't move the selection to the row.
				if (hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node != null)
					e.Handled = true;
				return;
			}
			// Any non-right press starts a fresh gesture -- drop a stale right-click target.
			contextMenuTargetNode = null;
			SetContextTargetItem(null);
		}
	}
}
