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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.DataGridDragDrop;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpyX.TreeView;

using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.TreeNodes;

namespace ILSpy.AssemblyTree
{
	public partial class AssemblyListPane : UserControl
	{
		// Suppresses the SelectionChanged → model bounce while we're pushing the model's
		// selection into the DataGrid.
		bool syncingSelection;

		LanguageSettings? languageSettings;

		public AssemblyListPane()
		{
			AppEnv.StartupLog.Mark("AssemblyListPane ctor entered");
			InitializeComponent();
			AttachedToVisualTree += (_, _) => AppEnv.StartupLog.Mark("AssemblyListPane attached to visual tree");
			Loaded += (_, _) => {
				AppEnv.StartupLog.Mark("AssemblyListPane Loaded");
				if (DataContext is AssemblyTreeModel m)
					m.MarkTreeReady();
			};
			// One-shot mark on the first DataGridRow being prepared — that's the
			// observable "tree has rows on screen" moment we compare against decompiler
			// output appearing.
			void OnFirstRowLoaded(object? sender, DataGridRowEventArgs args)
			{
				AppEnv.StartupLog.Mark("Assembly tree DataGrid: first row realised");
				TreeGrid.LoadingRow -= OnFirstRowLoaded;
			}
			TreeGrid.LoadingRow += OnFirstRowLoaded;
			TreeGrid.DoubleTapped += OnTreeGridDoubleTapped;
			TreeGrid.KeyDown += OnTreeGridKeyDown;
			// Bubble + handledEventsToo: ProDataGrid's row-level pointer handlers mark
			// PointerPressed handled before bubble reaches our subscription, so we have to
			// opt into "see handled events too" to react.
			TreeGrid.AddHandler(PointerPressedEvent, OnTreeGridPointerPressed,
				global::Avalonia.Interactivity.RoutingStrategies.Bubble,
				handledEventsToo: true);

			// Drag-reorder of top-level assembly rows. The actual reorder lives in
			// AssemblyRowDropHandler (wired to the live AssemblyList in BindTree); the
			// RowDragStarting hook here just cancels drags that originate from non-eligible
			// rows so the user never even sees a pickup cursor on a type or namespace.
			TreeGrid.CanUserReorderRows = true;
			TreeGrid.RowDragHandle = DataGridRowDragHandle.Row;
			TreeGrid.RowDragStarting += OnTreeGridRowDragStarting;

			// Explorer → tree file drop: ProDataGrid's row-drag pipeline doesn't see external
			// drops, so we wire Avalonia's standard DragDrop pipeline alongside it. Drops with
			// a target row honour Before/After to control where the opened assembly lands.
			DragDrop.SetAllowDrop(TreeGrid, true);
			TreeGrid.AddHandler(DragDrop.DragOverEvent, OnTreeGridDragOver);
			TreeGrid.AddHandler(DragDrop.DropEvent, OnTreeGridDrop);

			// Context-menu host. Tests bypass this and re-attach via AttachContextMenu so they
			// can inject stub entries — at app-runtime we resolve the registry through the
			// composition host. Both paths route through the same Opening handler.
			var registry = TryGetContextMenuRegistry();
			AttachContextMenu(registry?.Entries ?? System.Array.Empty<IContextMenuEntryExport>());

			// Subscribe to the active LanguageSettings so flipping ShowApiLevel rebuilds the
			// tree and the new visibility takes effect without a restart. SettingsService is
			// optional (design-time previews don't bootstrap composition).
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
			if (DataContext is AssemblyTreeModel model && model.Root != null)
				BindTree(model.Root);
		}

		static ContextMenuEntryRegistry? TryGetContextMenuRegistry()
		{
			try
			{
				return AppEnv.AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
			}
			catch
			{
				// Composition isn't available in design-time previews; the empty-entries
				// path leaves the menu host in place but cancels Opening, matching the
				// "no entries registered" UX.
				return null;
			}
		}

		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = System.Array.Empty<IContextMenuEntryExport>();

		/// <summary>
		/// Replaces the active context-menu entries. App-runtime call: once at construction
		/// with the registry's entries. Test call: directly with stub entries to exercise the
		/// menu without going through MEF.
		/// </summary>
		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			contextMenuEntries = entries;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			TreeGrid.ContextMenu = menu;
		}

		void OnContextMenuOpening(object? sender, global::System.ComponentModel.CancelEventArgs e)
		{
			if (sender is not ContextMenu menu)
				return;
			var built = BuildContextMenuForCurrentState(contextMenuEntries);
			if (built == null)
			{
				e.Cancel = true;
				return;
			}
			// Move the built menu's items into the live ContextMenu (a single Items collection
			// can't host the same MenuItem twice — copy then drop the donor).
			menu.Items.Clear();
			foreach (var item in System.Linq.Enumerable.ToArray(built.Items.OfType<Control>()))
			{
				built.Items.Remove(item);
				menu.Items.Add(item);
			}
		}

		/// <summary>
		/// Builds the context menu using the supplied entries against the pane's current
		/// selection. Internal so tests can drive the build path without raising the live
		/// Opening event.
		/// </summary>
		internal ContextMenu? BuildContextMenuForCurrentState(IReadOnlyList<IContextMenuEntryExport> entries)
			=> ContextMenuProvider.Build(entries, CreateContextMenuContext());

		TextViewContext CreateContextMenuContext()
		{
			var nodes = (DataContext as AssemblyTreeModel)?.SelectedItems.ToArray()
				?? System.Array.Empty<SharpTreeNode>();
			return new TextViewContext {
				TreeGrid = TreeGrid,
				SelectedTreeNodes = nodes,
			};
		}

		void OnTreeGridKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key != Key.Delete || DataContext is not AssemblyTreeModel model || model.AssemblyList is not { } list)
				return;
			// Snapshot before mutation — Unload mutates the list and indirectly the model's
			// selection.
			var assemblies = model.SelectedItems.OfType<AssemblyTreeNode>()
				.Select(n => n.LoadedAssembly)
				.ToList();
			if (assemblies.Count == 0)
				return;
			foreach (var asm in assemblies)
				list.Unload(asm);
			e.Handled = true;
		}

		void OnTreeGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (syncingSelection || DataContext is not AssemblyTreeModel model)
				return;
			// SelectedItem is a wrapper over SelectedItems on the model — touching it would
			// Clear+Add and clobber Ctrl-click multi-selection. Only mutate the collection.
			BeginSync();
			try
			{
				var current = new System.Collections.Generic.HashSet<SharpTreeNode>();
				foreach (var item in TreeGrid.SelectedItems)
				{
					var node = item is HierarchicalNode hn && hn.Item is SharpTreeNode t ? t
						: item as SharpTreeNode;
					if (node != null)
						current.Add(node);
				}
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
				EndSync();
			}
		}

		void SyncSelectionFromModel(SharpTreeNode? target)
		{
			if (target == null)
			{
				if (TreeGrid.SelectedItem != null)
				{
					BeginSync();
					TreeGrid.SelectedItem = null!;
					EndSync();
				}
				return;
			}
			if (TreeGrid.HierarchicalModel is not IHierarchicalModel hm)
				return;

			var path = new System.Collections.Generic.List<SharpTreeNode>();
			for (var n = target; n.Parent != null; n = n.Parent)
				path.Add(n);
			path.Reverse();

			// Each ancestor must be expanded before FindNode can locate the next level's wrapper.
			BeginSync();
			HierarchicalNode? hNode = null;
			for (int i = 0; i < path.Count; i++)
			{
				hNode = hm.FindNode(path[i]);
				if (hNode == null)
				{
					EndSync();
					return;
				}
				if (i < path.Count - 1 && !hNode.IsExpanded)
					hm.Expand(hNode);
			}

			if (ReferenceEquals(TreeGrid.SelectedItem, target))
			{
				EndSync();
				return;
			}
			TreeGrid.SelectedItem = target;
			// Pass the HierarchicalNode wrapper (DataGrid.IndexOf is against the wrappers, not
			// the underlying SharpTreeNodes), and defer past Expand's pending child-realization
			// notifications — synchronously the wrapper isn't in the visible list yet.
			var scrollTarget = hNode!;
			Dispatcher.UIThread.Post(
				() => CenterRowInView(scrollTarget),
				DispatcherPriority.Background);
			EndSync();
		}

		// DataGrid.ScrollIntoView only brings the row to the nearest viewport edge; we want
		// the row centred so the user's eye lands on it. Skip the move when the row is already
		// fully in view — re-centring an in-view row would yank the viewport on every selection
		// (e.g. user clicks a visible row, or Ctrl+O selects a freshly-loaded top-level entry).
		void CenterRowInView(HierarchicalNode node)
		{
			var scrollViewer = TreeGrid.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
			if (scrollViewer is null)
				return;

			// Cheap pre-check: if the row's already realised AND fully visible, leave the
			// viewport alone. ScrollIntoView (next call) would otherwise drag it to the edge,
			// and our centring step would then drag it again.
			var existingRow = TreeGrid.GetVisualDescendants().OfType<DataGridRow>()
				.FirstOrDefault(r => ReferenceEquals(r.DataContext, node));
			if (existingRow is { IsVisible: true }
				&& existingRow.TranslatePoint(new Point(0, 0), scrollViewer) is { } existingTop
				&& existingTop.Y >= 0
				&& existingTop.Y + existingRow.Bounds.Height <= scrollViewer.Viewport.Height)
				return;

			TreeGrid.ScrollIntoView(node, TreeGrid.Columns[0]);
			// ScrollIntoView only changes ScrollViewer.Offset; the row becomes a realised
			// DataGridRow during the next layout pass. Force it now so we can read the row's
			// bounds for centring.
			TreeGrid.UpdateLayout();

			var row = TreeGrid.GetVisualDescendants().OfType<DataGridRow>()
				.FirstOrDefault(r => ReferenceEquals(r.DataContext, node));
			if (row is null)
				return;

			var rowTopInViewer = row.TranslatePoint(new Point(0, 0), scrollViewer);
			if (rowTopInViewer is null)
				return;

			var desiredTop = (scrollViewer.Viewport.Height - row.Bounds.Height) / 2;
			var newOffsetY = scrollViewer.Offset.Y + (rowTopInViewer.Value.Y - desiredTop);
			var maxOffset = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
			newOffsetY = Math.Clamp(newOffsetY, 0, maxOffset);
			scrollViewer.Offset = new Vector(scrollViewer.Offset.X, newOffsetY);
		}

		void BeginSync() => syncingSelection = true;

		// Released on a Background dispatch tick so any DataGrid SelectionChanged the Expand /
		// SelectedItem / ScrollIntoView calls trigger doesn't bounce back into the model.
		void EndSync() => Dispatcher.UIThread.Post(
			() => syncingSelection = false,
			DispatcherPriority.Background);


		void OnTreeGridDoubleTapped(object? sender, TappedEventArgs e)
		{
			if (TreeGrid.HierarchicalModel is not IHierarchicalModel model)
				return;
			var visual = e.Source as Visual;
			while (visual != null && visual.DataContext is not HierarchicalNode)
				visual = visual.GetVisualParent();
			if (visual?.DataContext is HierarchicalNode node && !node.IsLeaf)
			{
				model.Toggle(node);
				e.Handled = true;
			}
		}

		void OnTreeGridPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			// Middle-click on a tree row → "open in new tab", matching the WPF gesture
			// (DecompileInNewViewCommand advertises InputGestureText = "MMB"). MMB does
			// not move the tree's selection on its own, so hit-test the row from e.Source
			// rather than reading SelectedItem.
			if (e.Source is not Visual hit
				|| !e.GetCurrentPoint(hit).Properties.IsMiddleButtonPressed)
				return;
			var visual = hit;
			while (visual != null && visual.DataContext is not HierarchicalNode)
				visual = visual.GetVisualParent();
			if (visual?.DataContext is not HierarchicalNode hn || hn.Item is not ILSpyTreeNode node)
				return;
			OpenNodeInNewTab(node);
			e.Handled = true;
		}

		/// <summary>
		/// Opens <paramref name="node"/> in a fresh document tab without disturbing the
		/// active one. Delegates to <see cref="DockWorkspace.OpenNodeInNewTab"/>, which
		/// picks decompiler-vs-custom page type based on what the node returns from
		/// <see cref="ILSpyTreeNode.CreateTab"/>. Shared between the MMB handler above and
		/// any test that wants to drive the new-tab path without simulating real pointer input.
		/// </summary>
		internal void OpenNodeInNewTab(ILSpyTreeNode node)
		{
			DockWorkspace? dock;
			try
			{
				dock = AppComposition.Current.GetExport<DockWorkspace>();
			}
			catch
			{
				// Composition isn't available in design-time previews; the gesture is a
				// no-op there.
				return;
			}
			dock.OpenNodeInNewTab(node);
		}

		protected override void OnDataContextChanged(System.EventArgs e)
		{
			using var _ = AppEnv.StartupLog.Phase("AssemblyListPane.OnDataContextChanged");
			base.OnDataContextChanged(e);

			if (DataContext is AssemblyTreeModel model)
			{
				model.PropertyChanged += Model_PropertyChanged;
				AppEnv.StartupLog.Mark($"AssemblyListPane DataContext is AssemblyTreeModel; Root={(model.Root != null ? "set" : "null")}");
				if (model.Root != null)
				{
					BindTree(model.Root);
					// Push any already-set selection (e.g. one restored from SessionSettings
					// before the pane subscribed to PropertyChanged) into the DataGrid.
					if (model.SelectedItem != null)
						SyncSelectionFromModel(model.SelectedItem);
				}
			}
		}

		void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is not AssemblyTreeModel model)
				return;
			if (e.PropertyName == nameof(AssemblyTreeModel.Root) && model.Root != null)
			{
				BindTree(model.Root);
				SyncSelectionFromModel(model.SelectedItem);
			}
			else if (e.PropertyName == nameof(AssemblyTreeModel.SelectedItem))
			{
				SyncSelectionFromModel(model.SelectedItem);
			}
		}

		// Apply the active LanguageSettings filter to a child collection materialised on
		// expansion. Returns a fresh List so iteration is stable; live updates at deeper levels
		// are not preserved (members are realised once per expansion — adds/removes mid-expand
		// don't happen in practice).
		static IEnumerable<SharpTreeNode> FilterChildren(IEnumerable<SharpTreeNode> children, LanguageSettings? settings)
		{
			if (settings == null)
				return children;
			return children
				.Where(c => c is not ILSpyTreeNode it || it.Filter(settings) != FilterResult.Hidden)
				.ToList();
		}

		void BindTree(SharpTreeNode root)
		{
			using var _ = AppEnv.StartupLog.Phase("AssemblyListPane.BindTree");
			var settings = languageSettings;
			var options = new HierarchicalOptions<SharpTreeNode> {
				ChildrenSelector = node => {
					node.EnsureLazyChildren();
					return FilterChildren(node.Children, settings);
				},
				IsLeafSelector = node => !node.ShowExpander,
				VirtualizeChildren = false,
				// Two-way sync of SharpTreeNode.IsExpanded ↔ grid wrapper. ProDataGrid reads the
				// value via reflection, writes it back on chevron-click, and observes
				// INotifyPropertyChanged on the source so model-side mutations propagate to the
				// grid automatically.
				IsExpandedPropertyPath = nameof(SharpTreeNode.IsExpanded),
			};

			var hierarchicalModel = new HierarchicalModel<SharpTreeNode>(options);
			// Pass the live ObservableCollection at the root so the grid observes
			// CollectionChanged when assemblies are unloaded / opened. Filter is not applied
			// at this level (AssemblyTreeNode never reports Hidden); deeper levels filter
			// on expansion via ChildrenSelector.
			hierarchicalModel.SetRoots(root.Children);

			TreeGrid.HierarchicalModel = hierarchicalModel;

			// Re-target the drop handler whenever the active AssemblyList changes (the user
			// can switch lists from the dropdown), so the next reorder mutates the right list.
			if (root is AssemblyListTreeNode listRoot)
				TreeGrid.RowDropHandler = new AssemblyRowDropHandler(listRoot.AssemblyList);
		}

		void OnTreeGridRowDragStarting(object? sender, DataGridRowDragStartingEventArgs e)
		{
			// Refuse the gesture for anything other than a top-level AssemblyTreeNode owned by
			// the user (not a nuget-nested entry). Cancelling here stops the drag visuals from
			// ever showing — the user gets immediate feedback that the row is not movable.
			foreach (var item in e.Items)
			{
				if (!AssemblyRowDropHandler.TryUnwrapTopLevelAssemblyNode(item, out var node)
					|| node.PackageEntry != null)
				{
					e.Cancel = true;
					return;
				}
			}
		}

		void OnTreeGridDragOver(object? sender, DragEventArgs e)
		{
			if (!e.DataTransfer.Contains(DataFormat.File))
				return;
			e.DragEffects = DragDropEffects.Copy;
			e.Handled = true;
		}

		void OnTreeGridDrop(object? sender, DragEventArgs e)
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

		(AssemblyTreeNode? target, DataGridRowDropPosition position) HitTestTopLevelRow(DragEventArgs e)
		{
			// Walk up from the pointer's visual to find the DataGridRow we landed on, then
			// pick Before / After by which half of the row contains the pointer. Drops on
			// empty space or onto deeper rows fall back to "append" — target == null.
			if (e.Source is not Visual hit)
				return (null, DataGridRowDropPosition.After);
			Visual? row = hit;
			while (row != null && row is not DataGridRow)
				row = row.GetVisualParent();
			if (row is not DataGridRow dataRow
				|| dataRow.DataContext is not HierarchicalNode hn
				|| hn.Item is not AssemblyTreeNode atn
				|| atn.Parent is not AssemblyListTreeNode)
				return (null, DataGridRowDropPosition.After);

			var pointer = e.GetPosition(dataRow);
			var position = pointer.Y < dataRow.Bounds.Height / 2
				? DataGridRowDropPosition.Before
				: DataGridRowDropPosition.After;
			return (atn, position);
		}

		/// <summary>
		/// Opens each path through <see cref="AssemblyList.OpenAssembly(string, bool)"/> and
		/// — when a top-level <paramref name="target"/> row is supplied — moves the opened set
		/// to the slot indicated by <paramref name="position"/>. Mirrors the WPF
		/// <c>AssemblyListTreeNode.Drop</c> code path so paths arriving from Explorer behave
		/// the same as <c>File → Open</c> with a follow-up reorder.
		/// </summary>
		internal void HandleFileDrop(IReadOnlyList<string> files,
			AssemblyTreeNode? target, DataGridRowDropPosition position)
		{
			if (DataContext is not AssemblyTreeModel model || model.AssemblyList is not { } list)
				return;
			var opened = new List<LoadedAssembly>();
			foreach (var path in files)
			{
				if (string.IsNullOrEmpty(path))
					continue;
				var asm = list.OpenAssembly(path);
				if (asm != null && !opened.Contains(asm))
					opened.Add(asm);
			}
			if (opened.Count == 0 || target == null)
				return;

			var ordering = list.GetAssemblies();
			int targetIndex = Array.IndexOf(ordering, target.LoadedAssembly);
			if (targetIndex < 0)
				return;
			int insertIndex = position == DataGridRowDropPosition.After
				? targetIndex + 1
				: targetIndex;
			list.Move(opened.ToArray(), insertIndex);
		}
	}
}
