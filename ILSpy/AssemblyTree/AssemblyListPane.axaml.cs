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

using ILSpy.AppEnv;
using ILSpy.Controls.TreeView;
using ILSpy.TreeNodes;

namespace ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		// Where a dropped set of assemblies lands relative to the target row.
		internal enum DropPosition { Before, After, Append }

		ILSpy.Controls.TreeView.TreeSelectionBinder? selectionBinder;
		LanguageSettings? languageSettings;
		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();

		// Thunderbird-style right-click target: the row whose context menu is open, highlighted
		// without moving the real selection.
		SharpTreeViewItem? contextTargetItem;
		SharpTreeViewItem? contextMenuOpenItem;
		SharpTreeNode? contextMenuTargetNode;

		// Assembly drag-reorder. It's an internal move, so the dragged set is kept in a field rather
		// than serialised; the DataTransfer just carries a marker so DragOver/Drop can tell a reorder
		// drag from an Explorer file drop.
		static readonly DataFormat<string> AssemblyReorderFormat =
			DataFormat.CreateStringApplicationFormat("ilspy-assembly-reorder");
		IReadOnlyList<AssemblyTreeNode>? draggingAssemblies;
		AssemblyTreeNode? pressedAssembly;
		PointerPressedEventArgs? dragPress;
		Point dragStartPos;

		public AssemblyListPane()
		{
			InitializeComponent();
			Loaded += (_, _) => {
				if (DataContext is AssemblyTreeModel m)
					m.MarkTreeReady();
			};
			// Right-press marks the context target without moving selection; MMB opens a new tab.
			Tree.AddHandler(PointerPressedEvent, OnTreePointerPressed, RoutingStrategies.Tunnel);
			Tree.AddHandler(ContextRequestedEvent, OnTreeContextRequested, RoutingStrategies.Bubble, handledEventsToo: true);
			Tree.AddHandler(PointerMovedEvent, OnTreePointerMoved, RoutingStrategies.Bubble, handledEventsToo: true);
			Tree.AddHandler(PointerReleasedEvent, OnTreePointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
			Tree.KeyDown += OnTreeKeyDown;

			// Explorer file drop AND internal assembly drag-reorder both arrive through Avalonia's
			// DragDrop pipeline; OnTreeDragOver/OnTreeDrop dispatch on the data format.
			DragDrop.SetAllowDrop(Tree, true);
			Tree.AddHandler(DragDrop.DragOverEvent, OnTreeDragOver);
			Tree.AddHandler(DragDrop.DropEvent, OnTreeDrop);
			Tree.AddHandler(DragDrop.DragLeaveEvent, (_, _) => HideInsertMarker());

			var registry = TryGetContextMenuRegistry();
			AttachContextMenu(registry?.Entries ?? Array.Empty<IContextMenuEntryExport>());

			languageSettings = TryGetLanguageSettings();
			if (languageSettings != null)
				languageSettings.PropertyChanged += OnLanguageSettingsChanged;
		}

		static LanguageSettings? TryGetLanguageSettings()
		{
			try
			{
				return AppComposition.Current.GetExport<SettingsService>().SessionSettings.LanguageSettings;
			}
			catch
			{
				return null;
			}
		}

		void OnLanguageSettingsChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(LanguageSettings.ShowApiLevel))
				return;
			// Re-apply the API-level filter in place; the flattener drops anything newly hidden.
			if (DataContext is AssemblyTreeModel { Root: ILSpyTreeNode root })
				root.RefreshRealizedFilter();
		}

		static ContextMenuEntryRegistry? TryGetContextMenuRegistry()
		{
			try
			{
				return AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
			}
			catch
			{
				return null;
			}
		}

		static ILSpy.Analyzers.AnalyzerTreeViewModel? TryGetAnalyzerTreeViewModel()
		{
			try
			{
				return AppComposition.Current.GetExport<ILSpy.Analyzers.AnalyzerTreeViewModel>();
			}
			catch
			{
				return null;
			}
		}

		#region Context menu

		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			contextMenuEntries = entries;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			menu.Closed += (_, _) => {
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
			contextMenuOpenItem = contextTargetItem;
			var built = BuildContextMenuForCurrentState(contextMenuEntries);
			if (built == null)
			{
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
			if (e.TryGetPosition(Tree, out var pos) && Tree.InputHitTest(pos) is Visual hit)
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
			pressedAssembly = null;
			dragPress = null;
			var pressedNode = hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node;
			if (point.IsMiddleButtonPressed && pressedNode is ILSpyTreeNode node)
			{
				OpenNodeInNewTab(node);
				e.Handled = true;
				return;
			}
			// Remember a top-level assembly row so a subsequent drag can reorder it.
			if (point.IsLeftButtonPressed && pressedNode is AssemblyTreeNode { Parent: AssemblyListTreeNode, PackageEntry: null } asm)
			{
				pressedAssembly = asm;
				dragPress = e;
				dragStartPos = e.GetPosition(Tree);
			}
		}

		void OnTreePointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			pressedAssembly = null;
			dragPress = null;
		}

		async void OnTreePointerMoved(object? sender, PointerEventArgs e)
		{
			if (pressedAssembly is not { } pressed || dragPress is not { } press)
				return;
			if (!e.GetCurrentPoint(Tree).Properties.IsLeftButtonPressed)
			{
				pressedAssembly = null;
				dragPress = null;
				return;
			}
			var delta = e.GetPosition(Tree) - dragStartPos;
			if (Math.Abs(delta.X) < 4 && Math.Abs(delta.Y) < 4)
				return;

			var dragged = ResolveDraggedAssemblies(pressed);
			pressedAssembly = null;
			dragPress = null;
			if (dragged.Count == 0)
				return;

			draggingAssemblies = dragged;
			try
			{
				var data = new DataTransfer();
				data.Add(DataTransferItem.Create(AssemblyReorderFormat, "1"));
				await DragDrop.DoDragDropAsync(press, data, DragDropEffects.Move);
			}
			finally
			{
				draggingAssemblies = null;
			}
		}

		// The dragged set: the whole selection when the pressed row is part of it, otherwise just
		// the pressed row -- filtered to movable top-level assemblies.
		IReadOnlyList<AssemblyTreeNode> ResolveDraggedAssemblies(AssemblyTreeNode pressed)
		{
			IEnumerable<AssemblyTreeNode> candidates =
				DataContext is AssemblyTreeModel model && model.SelectedItems.Contains(pressed)
					? model.SelectedItems.OfType<AssemblyTreeNode>()
					: new[] { pressed };
			return candidates
				.Where(n => n.Parent is AssemblyListTreeNode && n.PackageEntry == null)
				.ToList();
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
				var analyzerVm = TryGetAnalyzerTreeViewModel();
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
					Tree.Root = model.Root;
				selectionBinder = new ILSpy.Controls.TreeView.TreeSelectionBinder(Tree, model.SelectedItems);
			}
		}

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is AssemblyTreeModel model
				&& e.PropertyName == nameof(AssemblyTreeModel.Root) && model.Root != null)
			{
				Tree.Root = model.Root;
				// The flattener rebuilt; re-apply the model selection to the new rows.
				selectionBinder?.Refresh();
			}
		}

		#endregion

		#region Drag-reorder + file drop

		void OnTreeDragOver(object? sender, DragEventArgs e)
		{
			if (draggingAssemblies is { } dragged && e.DataTransfer.Contains(AssemblyReorderFormat))
			{
				var (target, position) = HitTestTopLevelRow(e);
				bool valid = target != null && CanReorder(dragged, target, position);
				e.DragEffects = valid ? DragDropEffects.Move : DragDropEffects.None;
				UpdateInsertMarker(e, valid ? position : null);
				e.Handled = true;
				return;
			}
			if (!e.DataTransfer.Contains(DataFormat.File))
				return;
			e.DragEffects = DragDropEffects.Copy;
			e.Handled = true;
		}

		void OnTreeDrop(object? sender, DragEventArgs e)
		{
			if (draggingAssemblies is { } dragged && e.DataTransfer.Contains(AssemblyReorderFormat))
			{
				HideInsertMarker();
				var (reorderTarget, reorderPosition) = HitTestTopLevelRow(e);
				if (reorderTarget != null)
					ReorderAssemblies(dragged, reorderTarget, reorderPosition);
				e.DragEffects = DragDropEffects.Move;
				e.Handled = true;
				return;
			}
			if (!e.DataTransfer.Contains(DataFormat.File))
				return;
			var storageItems = e.DataTransfer.TryGetFiles();
			if (storageItems == null || storageItems.Length == 0)
				return;
			var files = storageItems
				.Select(f => f.TryGetLocalPath())
				.Where(p => !string.IsNullOrEmpty(p))
				.Select(p => p!)
				.ToList();
			if (files.Count == 0)
				return;
			var (target, position) = HitTestTopLevelRow(e);
			HandleFileDrop(files, target, position);
			e.DragEffects = DragDropEffects.Copy;
			e.Handled = true;
		}

		(AssemblyTreeNode? target, DropPosition position) HitTestTopLevelRow(DragEventArgs e)
		{
			if (e.Source is not Visual hit)
				return (null, DropPosition.Append);
			var item = hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true);
			if (item?.Node is not AssemblyTreeNode atn || atn.Parent is not AssemblyListTreeNode)
				return (null, DropPosition.Append);
			var pointer = e.GetPosition(item);
			var position = pointer.Y < item.Bounds.Height / 2 ? DropPosition.Before : DropPosition.After;
			return (atn, position);
		}

		// Positions the drop indicator at the top (Before) or bottom (After) edge of the target row.
		void UpdateInsertMarker(DragEventArgs e, DropPosition? position)
		{
			if (position is not { } pos || pos == DropPosition.Append
				|| (e.Source as Visual)?.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true) is not { } item)
			{
				HideInsertMarker();
				return;
			}
			var topLeft = item.TranslatePoint(new Point(0, 0), this) ?? default;
			double y = topLeft.Y + (pos == DropPosition.After ? item.Bounds.Height : 0);
			InsertMarker.Margin = new Thickness(topLeft.X, y - 1, 0, 0);
			InsertMarker.Width = item.Bounds.Width;
			InsertMarker.IsVisible = true;
		}

		void HideInsertMarker() => InsertMarker.IsVisible = false;

		/// <summary>
		/// Whether <paramref name="dragged"/> can be reordered to land Before/After
		/// <paramref name="target"/>. Only top-level (non-package) assemblies reorder, never onto a
		/// child or onto themselves. Mirrors the old AssemblyRowDropHandler validation.
		/// </summary>
		internal bool CanReorder(IReadOnlyList<AssemblyTreeNode> dragged, AssemblyTreeNode? target, DropPosition position)
		{
			if (position == DropPosition.Append || target is not { Parent: AssemblyListTreeNode } || dragged.Count == 0)
				return false;
			foreach (var node in dragged)
			{
				if (node.Parent is not AssemblyListTreeNode || node.PackageEntry != null || ReferenceEquals(node, target))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Moves <paramref name="dragged"/> to the slot indicated by <paramref name="target"/> /
		/// <paramref name="position"/> via <see cref="ICSharpCode.ILSpyX.AssemblyList.Move"/> (the same
		/// persistence path as the rest of the app). Returns false if the move isn't valid.
		/// </summary>
		internal bool ReorderAssemblies(IReadOnlyList<AssemblyTreeNode> dragged, AssemblyTreeNode target, DropPosition position)
		{
			if (!CanReorder(dragged, target, position)
				|| DataContext is not AssemblyTreeModel model || model.AssemblyList is not { } list)
				return false;
			var ordering = list.GetAssemblies();
			int targetIndex = Array.IndexOf(ordering, target.LoadedAssembly);
			if (targetIndex < 0)
				return false;
			int insertIndex = position == DropPosition.After ? targetIndex + 1 : targetIndex;
			list.Move(dragged.Select(n => n.LoadedAssembly).ToArray(), insertIndex);
			return true;
		}

		internal void HandleFileDrop(IReadOnlyList<string> files, AssemblyTreeNode? target, DropPosition position)
		{
			if (DataContext is not AssemblyTreeModel model || model.AssemblyList is not { } list)
				return;
			var opened = new List<ICSharpCode.ILSpyX.LoadedAssembly>();
			foreach (var path in files)
			{
				if (string.IsNullOrEmpty(path))
					continue;
				var asm = list.OpenAssembly(path);
				if (asm != null && !opened.Contains(asm))
					opened.Add(asm);
			}
			if (opened.Count == 0)
				return;

			if (target != null && position != DropPosition.Append)
			{
				var ordering = list.GetAssemblies();
				int targetIndex = Array.IndexOf(ordering, target.LoadedAssembly);
				if (targetIndex >= 0)
				{
					int insertIndex = position == DropPosition.After ? targetIndex + 1 : targetIndex;
					list.Move(opened.ToArray(), insertIndex);
				}
			}

			if (model.Root is not AssemblyListTreeNode listRoot)
				return;
			var newNodes = opened
				.Select(listRoot.FindAssemblyNode)
				.Where(n => n != null)
				.Cast<SharpTreeNode>()
				.ToList();
			if (newNodes.Count == 0)
				return;
			model.SelectedItems.Clear();
			foreach (var node in newNodes)
				model.SelectedItems.Add(node);
		}

		#endregion

		internal void OpenNodeInNewTab(ILSpyTreeNode node)
		{
			try
			{
				AppComposition.Current.GetExport<ILSpy.Docking.DockWorkspace>().OpenNodeInNewTab(node);
			}
			catch
			{
				// Composition unavailable in design-time previews.
			}
		}
	}
}
