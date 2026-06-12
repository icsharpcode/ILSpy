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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		ICSharpCode.ILSpy.Controls.TreeView.TreeSelectionBinder? selectionBinder;
		LanguageSettings? languageSettings;
		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();

		// Thunderbird-style right-click target: the row whose context menu is open, highlighted
		// without moving the real selection.
		SharpTreeViewItem? contextTargetItem;
		SharpTreeViewItem? contextMenuOpenItem;
		SharpTreeNode? contextMenuTargetNode;
		// For a keyboard-invoked menu (Shift+F10 / Apps), the row to re-focus when the menu closes
		// (closing the popup otherwise drops the keyboard focus and its focus adorner). Null for
		// pointer-invoked menus.
		SharpTreeViewItem? focusToRestoreAfterMenu;
		// Whether the last ContextRequested came from the keyboard (no pointer position). The keyboard
		// path carries no target row, so the menu adopts the selected row (see OnContextMenuOpening).
		bool lastContextRequestWasKeyboard;

		public AssemblyListPane()
		{
			InitializeComponent();
			Loaded += (_, _) => {
				if (DataContext is AssemblyTreeModel m)
					m.MarkTreeReady();
			};
			// Right-press marks the context target without moving selection; MMB opens a new tab.
			// Drag-reorder + file drop are owned by SharpTreeView (delegated to the tree nodes).
			Tree.AddHandler(PointerPressedEvent, OnTreePointerPressed, RoutingStrategies.Tunnel);
			Tree.AddHandler(ContextRequestedEvent, OnTreeContextRequested, RoutingStrategies.Bubble, handledEventsToo: true);
			Tree.KeyDown += OnTreeKeyDown;

			var registry = AppComposition.TryGetExport<ContextMenuEntryRegistry>();
			AttachContextMenu(registry?.Entries ?? Array.Empty<IContextMenuEntryExport>());

			languageSettings = AppComposition.TryGetExport<SettingsService>()?.SessionSettings.LanguageSettings;
			if (languageSettings != null)
				languageSettings.PropertyChanged += OnLanguageSettingsChanged;
		}


		void OnLanguageSettingsChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(LanguageSettings.ShowApiLevel))
				return;
			// Re-apply the API-level filter in place; the flattener drops anything newly hidden.
			if (DataContext is AssemblyTreeModel { Root: ILSpyTreeNode root })
				root.RefreshRealizedFilter();
		}



		#region Context menu

		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			contextMenuEntries = entries;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			menu.Closed += (_, _) => {
				RestoreFocusAfterKeyboardMenu();
				if (!ReferenceEquals(contextTargetItem, contextMenuOpenItem))
					return;
				contextMenuTargetNode = null;
				SetContextTargetItem(null);
			};
			Tree.ContextMenu = menu;
		}

		void OnContextMenuOpening(object? sender, CancelEventArgs e)
		{
			if (sender is not ContextMenu menu)
				return;
			// A keyboard-invoked menu (Shift+F10 / Apps) carries no pointer position, so OnTreeContextRequested
			// set no transient target. Adopt the keyboard-FOCUSED row (which may differ from the selection
			// after Ctrl+Arrow) as the target: opening the popup steals focus and drops the row's focus
			// adorner, so we mark that row with the same context-target highlight the mouse gives the
			// right-clicked row, and restore its focus + adorner on close (Avalonia's ContextMenu does not).
			// Captured here, before the popup opens and takes focus (Opening fires ahead of it), and before
			// contextMenuOpenItem is latched so the Closed handler still clears the highlight.
			var focusedRow = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() as SharpTreeViewItem;
			if (lastContextRequestWasKeyboard && focusedRow?.Node != null)
			{
				contextMenuTargetNode = focusedRow.Node;
				SetContextTargetItem(focusedRow);
				focusToRestoreAfterMenu = focusedRow;
			}
			contextMenuOpenItem = contextTargetItem;
			var built = BuildContextMenuForCurrentState(contextMenuEntries);
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
			global::Avalonia.Threading.Dispatcher.UIThread.Post(
				() => toFocus.Focus(NavigationMethod.Tab));
		}

		internal ContextMenu? BuildContextMenuForCurrentState(IReadOnlyList<IContextMenuEntryExport> entries)
			=> ContextMenuProvider.Build(entries, CreateContextMenuContext());

		internal ContextMenu? BuildContextMenuForCurrentState(
			IReadOnlyList<IContextMenuEntryExport> entries, SharpTreeNode? rightClickedNode)
		{
			contextMenuTargetNode = rightClickedNode;
			try
			{
				return ContextMenuProvider.Build(entries, CreateContextMenuContext());
			}
			finally
			{
				contextMenuTargetNode = null;
			}
		}

		TextViewContext CreateContextMenuContext()
		{
			var selection = (DataContext as AssemblyTreeModel)?.SelectedItems.ToArray()
				?? Array.Empty<SharpTreeNode>();
			// A right-click outside the selection targets just the clicked row; inside the selection
			// (or a keyboard-invoked menu with no target) acts on the whole selection.
			var target = contextMenuTargetNode;
			var nodes = target != null && !Array.Exists(selection, n => ReferenceEquals(n, target))
				? new[] { target }
				: selection;
			return new TextViewContext {
				TreeGrid = Tree,
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
			lastContextRequestWasKeyboard = !e.TryGetPosition(Tree, out var pos);
			if (!lastContextRequestWasKeyboard && Tree.InputHitTest(pos) is Visual hit)
				item = hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true);
			contextMenuTargetNode = item?.Node;
			SetContextTargetItem(item?.Node != null ? item : null);
		}

		void OnTreePointerPressed(object? sender, PointerPressedEventArgs e)
		{
			if (e.Source is not Visual hit)
				return;
			var point = e.GetCurrentPoint(hit).Properties;
			if (point.IsRightButtonPressed)
			{
				// Swallow the right press so the ListBox doesn't move the selection to the row.
				if (hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node != null)
					e.Handled = true;
				return;
			}
			// Any non-right press starts a fresh gesture -- drop a stale right-click target.
			contextMenuTargetNode = null;
			SetContextTargetItem(null);
			if (point.IsMiddleButtonPressed
				&& hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node is ILSpyTreeNode node)
			{
				OpenNodeInNewTab(node);
				e.Handled = true;
			}
		}

		#endregion

		#region Keyboard (assembly-specific: Delete, Ctrl+R)

		void OnTreeKeyDown(object? sender, KeyEventArgs e)
		{
			if (DataContext is not AssemblyTreeModel model)
				return;
			if (e.Key == Key.Delete && e.KeyModifiers == KeyModifiers.None && model.AssemblyList is { } list)
			{
				var selectedAssemblyNodes = model.SelectedItems.OfType<AssemblyTreeNode>().ToList();
				if (selectedAssemblyNodes.Count == 0)
					return;
				int reselectIndex = FlattenedIndexOf(selectedAssemblyNodes[0]);
				foreach (var node in selectedAssemblyNodes)
					list.Unload(node.LoadedAssembly);
				e.Handled = true;
				global::Avalonia.Threading.Dispatcher.UIThread.Post(
					() => ReselectAfterDelete(reselectIndex),
					global::Avalonia.Threading.DispatcherPriority.Background);
				return;
			}
			if (e.Key == Key.R && e.KeyModifiers == KeyModifiers.Control)
			{
				var members = model.SelectedItems.OfType<IMemberTreeNode>()
					.Select(n => n.Member)
					.Where(m => m is not null and not ICSharpCode.Decompiler.TypeSystem.IField { IsConst: true })
					.ToList();
				if (members.Count == 0)
					return;
				var analyzerVm = AppComposition.TryGetExport<ICSharpCode.ILSpy.Analyzers.AnalyzerTreeViewModel>();
				if (analyzerVm == null)
					return;
				foreach (var member in members)
					analyzerVm.Analyze(member!);
				e.Handled = true;
			}
		}

		System.Collections.IList? Flattened => Tree.ItemsSource as System.Collections.IList;

		int FlattenedIndexOf(SharpTreeNode node) => Flattened?.IndexOf(node) ?? -1;

		void ReselectAfterDelete(int index)
		{
			if (DataContext is not AssemblyTreeModel model)
				return;
			var flattened = Flattened;
			if (flattened == null || flattened.Count == 0 || index < 0)
			{
				model.SelectNode(null);
				return;
			}
			model.SelectNode(flattened[Math.Clamp(index, 0, flattened.Count - 1)] as SharpTreeNode);
		}

		#endregion

		#region Selection sync

		protected override void OnDataContextChanged(EventArgs e)
		{
			base.OnDataContextChanged(e);
			selectionBinder?.Dispose();
			selectionBinder = null;
			if (DataContext is AssemblyTreeModel model)
			{
				model.PropertyChanged += Model_PropertyChanged;
				if (model.Root != null)
				{
					Tree.Root = model.Root;
					WireDropSelection(model);
				}
				selectionBinder = new ICSharpCode.ILSpy.Controls.TreeView.TreeSelectionBinder(Tree, model.SelectedItems);
			}
		}

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is AssemblyTreeModel model
				&& e.PropertyName == nameof(AssemblyTreeModel.Root) && model.Root != null)
			{
				Tree.Root = model.Root;
				WireDropSelection(model);
				// The flattener rebuilt; re-apply the model selection to the new rows.
				selectionBinder?.Refresh();
			}
		}

		// The drop logic lives on AssemblyListTreeNode (open + move); selecting the result is a view
		// concern, so the node delegates it back here, where we resolve the assemblies to tree nodes
		// and push them through the model selection (which the TreeSelectionBinder reflects).
		void WireDropSelection(AssemblyTreeModel model)
		{
			if (model.Root is not AssemblyListTreeNode listRoot)
				return;
			listRoot.SelectAssembliesAfterDrop = assemblies => {
				var nodes = assemblies
					.Select(listRoot.FindAssemblyNode)
					.Where(n => n != null)
					.Cast<SharpTreeNode>()
					.ToList();
				if (nodes.Count == 0)
					return;
				// Go through the batched setter, not a raw Clear()+Add(): the latter flashes a
				// transient empty selection that poisons the grid-sync deferred guard (the tree
				// stops following tab activation).
				model.SelectNodes(nodes);
			};
		}

		#endregion


		internal void OpenNodeInNewTab(ILSpyTreeNode node)
		{
			// Composition unavailable in design-time previews -> no-op.
			AppComposition.TryGetExport<ICSharpCode.ILSpy.Docking.DockWorkspace>()?.OpenNodeInNewTab(node);
		}
	}
}
