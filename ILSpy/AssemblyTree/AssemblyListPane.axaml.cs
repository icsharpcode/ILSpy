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

		bool syncingSelection;
		LanguageSettings? languageSettings;
		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();

		// Thunderbird-style right-click target: the row whose context menu is open, highlighted
		// without moving the real selection.
		SharpTreeViewItem? contextTargetItem;
		SharpTreeViewItem? contextMenuOpenItem;
		SharpTreeNode? contextMenuTargetNode;

		public AssemblyListPane()
		{
			InitializeComponent();
			Loaded += (_, _) => {
				if (DataContext is AssemblyTreeModel m)
					m.MarkTreeReady();
			};
			Tree.SelectionChanged += OnTreeSelectionChanged;
			// Right-press marks the context target without moving selection; MMB opens a new tab.
			Tree.AddHandler(PointerPressedEvent, OnTreePointerPressed, RoutingStrategies.Tunnel);
			Tree.AddHandler(ContextRequestedEvent, OnTreeContextRequested, RoutingStrategies.Bubble, handledEventsToo: true);
			Tree.KeyDown += OnTreeKeyDown;

			// Explorer -> tree file drop (the tree only receives drops; assembly reorder is a
			// separate follow-up on the new control).
			DragDrop.SetAllowDrop(Tree, true);
			Tree.AddHandler(DragDrop.DragOverEvent, OnTreeDragOver);
			Tree.AddHandler(DragDrop.DropEvent, OnTreeDrop);

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
			if (DataContext is AssemblyTreeModel model)
			{
				model.PropertyChanged += Model_PropertyChanged;
				if (model.Root != null)
				{
					Tree.Root = model.Root;
					SyncModelSelectionToTree();
				}
			}
		}

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is not AssemblyTreeModel model)
				return;
			if (e.PropertyName == nameof(AssemblyTreeModel.Root) && model.Root != null)
			{
				Tree.Root = model.Root;
				SyncModelSelectionToTree();
			}
			else if (e.PropertyName == nameof(AssemblyTreeModel.SelectedItem))
			{
				if (syncingSelection)
					return;
				SyncModelSelectionToTree();
			}
		}

		// Tree -> model: mirror the ListBox selection (already SharpTreeNodes) into the model.
		void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (syncingSelection || DataContext is not AssemblyTreeModel model)
				return;
			syncingSelection = true;
			try
			{
				var current = Tree.SelectedItems!.OfType<SharpTreeNode>().ToHashSet();
				for (int i = model.SelectedItems.Count - 1; i >= 0; i--)
				{
					if (!current.Contains(model.SelectedItems[i]))
						model.SelectedItems.RemoveAt(i);
				}
				foreach (var node in current)
				{
					if (!model.SelectedItems.Contains(node))
						model.SelectedItems.Add(node);
				}
			}
			finally
			{
				syncingSelection = false;
			}
		}

		// Model -> tree: reveal + select the model's nodes (e.g. restored / navigated selection).
		void SyncModelSelectionToTree()
		{
			if (DataContext is not AssemblyTreeModel model)
				return;
			syncingSelection = true;
			try
			{
				Tree.SelectedItems!.Clear();
				SharpTreeNode? primary = null;
				foreach (var node in model.SelectedItems)
				{
					Tree.ScrollIntoNodeView(node);
					// Only add rows the flattener actually contains; adding an off-list node
					// corrupts the ListBox SelectionModel (it throws on later enumeration).
					if (Flattened is { } items && items.Contains(node))
					{
						Tree.SelectedItems.Add(node);
						primary = node;
					}
				}
				if (primary != null)
					Tree.FocusNode(primary);
			}
			finally
			{
				syncingSelection = false;
			}
		}

		#endregion

		#region File drop

		void OnTreeDragOver(object? sender, DragEventArgs e)
		{
			if (!e.DataTransfer.Contains(DataFormat.File))
				return;
			e.DragEffects = DragDropEffects.Copy;
			e.Handled = true;
		}

		void OnTreeDrop(object? sender, DragEventArgs e)
		{
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
