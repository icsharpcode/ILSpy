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
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>
	/// A virtualizing tree control built on <see cref="ListBox"/>, mirroring the WPF SharpTreeView.
	/// The tree of <see cref="SharpTreeNode"/>s is flattened to the visible-node list by
	/// <see cref="TreeFlattener"/> (an <see cref="System.Collections.IList"/> +
	/// <see cref="INotifyCollectionChanged"/>) which is bound straight to the ListBox ItemsSource --
	/// so the ListBox virtualizes it and provides anchor-based extend/shrink selection natively.
	/// </summary>
	public class SharpTreeView : ListBox
	{
		public static readonly StyledProperty<SharpTreeNode?> RootProperty =
			AvaloniaProperty.Register<SharpTreeView, SharpTreeNode?>(nameof(Root));

		public static readonly StyledProperty<bool> ShowRootProperty =
			AvaloniaProperty.Register<SharpTreeView, bool>(nameof(ShowRoot), defaultValue: true);

		public static readonly StyledProperty<bool> ShowRootExpanderProperty =
			AvaloniaProperty.Register<SharpTreeView, bool>(nameof(ShowRootExpander), defaultValue: false);

		public static readonly StyledProperty<bool> ShowLinesProperty =
			AvaloniaProperty.Register<SharpTreeView, bool>(nameof(ShowLines), defaultValue: true);

		TreeFlattener? flattener;
		bool doNotScrollOnExpanding;
		string searchBuffer = string.Empty;
		DispatcherTimer? searchResetTimer;

		static SharpTreeView()
		{
			// WPF "Extended": plain click selects one, Shift extends a range from the anchor (and
			// shrinks it), Ctrl toggles. Avalonia's ListBox provides this with Multiple selection --
			// the behaviour ProDataGrid could not do correctly.
			SelectionModeProperty.OverrideDefaultValue<SharpTreeView>(SelectionMode.Multiple);
		}

		public SharpTreeView()
		{
			// Take keyboard focus directly so gestures like Ctrl+A work the moment the user tabs
			// or clicks into the pane, before any row becomes the current item.
			Focusable = true;
			// The app reveals the selection explicitly on cross-control navigation (search results,
			// code/metadata links, analyzer nodes) via TreeSelectionBinder -> CenterNodeInView. The
			// ListBox's own AutoScrollToSelectedItem additionally chases the selected row whenever its
			// index shifts -- e.g. when the user expands an unrelated node -- yanking the viewport away
			// from what the user is doing. Disable it so in-tree gestures never move the view.
			AutoScrollToSelectedItem = false;
			SelectionChanged += OnSelectionChanged;
			DoubleTapped += OnDoubleTapped;
			// Drag-drop is handled generically here and delegated to SharpTreeNode.CanDrop/Drop.
			AddHandler(PointerPressedEvent, OnDragPointerPressed, RoutingStrategies.Tunnel);
			AddHandler(PointerMovedEvent, OnDragPointerMoved, RoutingStrategies.Bubble, handledEventsToo: true);
			AddHandler(PointerReleasedEvent, OnDragPointerReleased, RoutingStrategies.Bubble, handledEventsToo: true);
			DragDrop.SetAllowDrop(this, true);
			AddHandler(DragDrop.DragOverEvent, OnDragOver);
			AddHandler(DragDrop.DropEvent, OnDrop);
			AddHandler(DragDrop.DragLeaveEvent, (_, _) => HideInsertMarker());
		}

		public SharpTreeNode? Root {
			get => GetValue(RootProperty);
			set => SetValue(RootProperty, value);
		}

		public bool ShowRoot {
			get => GetValue(ShowRootProperty);
			set => SetValue(ShowRootProperty, value);
		}

		public bool ShowRootExpander {
			get => GetValue(ShowRootExpanderProperty);
			set => SetValue(ShowRootExpanderProperty, value);
		}

		public bool ShowLines {
			get => GetValue(ShowLinesProperty);
			set => SetValue(ShowLinesProperty, value);
		}

		protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
		{
			base.OnPropertyChanged(change);
			if (change.Property == RootProperty || change.Property == ShowRootProperty || change.Property == ShowRootExpanderProperty)
				Reload();
		}

		void Reload()
		{
			if (flattener != null)
			{
				flattener.Stop();
				flattener = null;
			}
			if (Root != null)
			{
				if (!(ShowRoot && ShowRootExpander))
					Root.IsExpanded = true;
				flattener = new TreeFlattener(Root, ShowRoot);
				ItemsSource = flattener;
			}
			else
			{
				ItemsSource = null;
			}
			// Avalonia's ListBox removes items from the selection automatically when they leave the
			// source (a collapsed ancestor hides them), so no manual deselect-on-hide is needed.
		}

		protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
			=> new SharpTreeViewItem();

		protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
			=> NeedsContainer<SharpTreeViewItem>(item, out recycleKey);

		protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
		{
			base.PrepareContainerForItemOverride(container, item, index);
			if (container is SharpTreeViewItem treeItem)
				treeItem.ParentTreeView = this;
		}

		void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			foreach (SharpTreeNode node in e.RemovedItems)
				node.IsSelected = false;
			foreach (SharpTreeNode node in e.AddedItems)
				node.IsSelected = true;
		}

		void OnDoubleTapped(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			var node = (e.Source as Visual)?.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node;
			if (node == null)
				return;
			// Let the node act first (e.g. an analyzer entity row navigates); only when it doesn't
			// handle the gesture do we fall back to toggling expansion.
			var args = new AvaloniaPlatformRoutedEventArgs(e);
			node.ActivateItem(args);
			if (!e.Handled && node.ShowExpander)
			{
				node.IsExpanded = !node.IsExpanded;
				e.Handled = true;
			}
		}

		/// <summary>
		/// Called when a visible node expands so its newly shown children are scrolled into view
		/// (without scrolling the node itself off the top).
		/// </summary>
		internal void HandleExpanding(SharpTreeNode node)
		{
			if (doNotScrollOnExpanding)
				return;
			SharpTreeNode lastVisibleChild = node;
			while (true)
			{
				var child = lastVisibleChild.Children.LastOrDefault(c => c.IsVisible);
				if (child == null)
					break;
				lastVisibleChild = child;
			}
			if (lastVisibleChild != node)
			{
				ScrollIntoView(lastVisibleChild);
				Dispatcher.UIThread.Post(() => ScrollIntoView(node), DispatcherPriority.Loaded);
			}
		}

		/// <summary>Scrolls the node into view (unless <paramref name="scroll"/> is false) and gives it
		/// keyboard focus. Pass <c>scroll: false</c> to focus a row that is already visible without
		/// disturbing the scroll position.</summary>
		public void FocusNode(SharpTreeNode node, bool scroll = true)
		{
			ArgumentNullException.ThrowIfNull(node);
			if (scroll)
				ScrollIntoNodeView(node);
			if (ContainerFromItem(node) is { } container)
				container.Focus();
			else
				Dispatcher.UIThread.Post(() => (ContainerFromItem(node))?.Focus(), DispatcherPriority.Loaded);
		}

		/// <summary>True when the node's row is realised and lies fully within the scroll viewport.
		/// Used to decide, before a selection change scrolls the list, whether a reveal is needed at
		/// all -- an already-visible row should not be pulled to the centre.</summary>
		public bool IsNodeFullyVisible(SharpTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);
			var scrollViewer = this.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
			if (scrollViewer is null)
				return false;
			if (ContainerFromItem(node) is Control row && row.IsVisible
				&& row.TranslatePoint(new Point(0, 0), scrollViewer) is { } top)
				return top.Y >= 0 && top.Y + row.Bounds.Height <= scrollViewer.Viewport.Height;
			return false;
		}

		/// <summary>Moves the (single) selection to <paramref name="node"/> and focuses it.
		/// Used by keyboard navigation where selection must follow the caret.</summary>
		void SelectAndFocus(SharpTreeNode node)
		{
			SelectedItem = node;
			FocusNode(node);
		}

		public void ScrollIntoNodeView(SharpTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);
			doNotScrollOnExpanding = true;
			foreach (var ancestor in node.Ancestors())
				ancestor.IsExpanded = true;
			doNotScrollOnExpanding = false;
			CenterNodeInView(node);
		}

		/// <summary>
		/// Reveals <paramref name="node"/> centred in the viewport so the user's eye lands on a
		/// newly selected row, rather than at the nearest edge (where <see cref="ListBox.ScrollIntoView"/>
		/// leaves it). Skips the move when the row is already fully visible, so clicking a visible
		/// row -- or selecting a freshly-loaded top-level entry that's already on screen -- never
		/// yanks the viewport.
		/// </summary>
		void CenterNodeInView(SharpTreeNode node)
		{
			var scrollViewer = this.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
			if (scrollViewer is null)
			{
				ScrollIntoView(node);
				return;
			}

			// If the row is already realised and roughly centred, leave the viewport alone -- this
			// keeps a re-selection of an already-centred row from twitching. We deliberately do NOT
			// skip a merely-visible row sitting at an edge: the ListBox's AutoScrollToSelectedItem
			// drags the selected row to the nearest edge first, and a reveal should still pull it to
			// the centre from there. (Skipping an already-visible row is decided one level up, before
			// AutoScroll runs, in the model->tree sync -- see TreeSelectionBinder.SyncModelToTree.)
			if (ContainerFromItem(node) is Control visible && visible.IsVisible
				&& visible.TranslatePoint(new Point(0, 0), scrollViewer) is { } top)
			{
				var rowMid = top.Y + visible.Bounds.Height / 2;
				var viewportMid = scrollViewer.Viewport.Height / 2;
				if (Math.Abs(rowMid - viewportMid) <= visible.Bounds.Height)
					return;
			}

			// Bring it on screen (edge), force layout so the container realises, then offset so the
			// row sits at the vertical centre.
			ScrollIntoView(node);
			UpdateLayout();
			if (ContainerFromItem(node) is not Control row)
				return;
			if (row.TranslatePoint(new Point(0, 0), scrollViewer) is not { } rowTop)
				return;
			var desiredTop = (scrollViewer.Viewport.Height - row.Bounds.Height) / 2;
			var newOffsetY = scrollViewer.Offset.Y + (rowTop.Y - desiredTop);
			var maxOffset = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
			newOffsetY = Math.Clamp(newOffsetY, 0, maxOffset);
			scrollViewer.Offset = new Vector(scrollViewer.Offset.X, newOffsetY);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			// Ctrl+A select-all must work on the first press even before a current item is
			// established (right after the tree gains focus). The base ListBox only selects all
			// when a focused item lives inside it, so drive it explicitly here.
			if (e.Key == Key.A && e.KeyModifiers == KeyModifiers.Control
				&& SelectionMode.HasFlag(SelectionMode.Multiple))
			{
				SelectAll();
				e.Handled = true;
				return;
			}
			var node = (e.Source as Visual)?.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node
				?? SelectedItem as SharpTreeNode;
			if (node != null && e.KeyModifiers == KeyModifiers.None)
			{
				switch (e.Key)
				{
					case Key.Left:
						if (node.IsExpanded)
							node.IsExpanded = false;
						else if (node.Parent != null && !node.Parent.IsRoot)
							SelectAndFocus(node.Parent);
						else
							break;
						e.Handled = true;
						break;
					case Key.Right:
						if (!node.IsExpanded && node.ShowExpander)
							node.IsExpanded = true;
						else if (node.Children.Count > 0)
							SelectAndFocus(node.Children.First(c => c.IsVisible));
						else
							break;
						e.Handled = true;
						break;
					case Key.Add:
						node.IsExpanded = true;
						e.Handled = true;
						break;
					case Key.Subtract:
						node.IsExpanded = false;
						e.Handled = true;
						break;
					case Key.Multiply:
						node.IsExpanded = true;
						ExpandRecursively(node);
						e.Handled = true;
						break;
					case Key.Enter:
					case Key.Space:
						if (SelectedItems!.Count == 1 && ReferenceEquals(SelectedItem, node))
						{
							if (e.Key == Key.Space && node.IsCheckable)
								node.IsChecked = node.IsChecked == null ? true : !node.IsChecked;
							else
								node.ActivateItem(new AvaloniaPlatformRoutedEventArgs(e));
							e.Handled = true;
						}
						break;
				}
			}
			if (!e.Handled)
				base.OnKeyDown(e);
		}

		static void ExpandRecursively(SharpTreeNode node)
		{
			if (!node.CanExpandRecursively)
				return;
			node.IsExpanded = true;
			foreach (var child in node.Children)
				ExpandRecursively(child);
		}

		protected override void OnTextInput(TextInputEventArgs e)
		{
			var text = e.Text;
			if (!string.IsNullOrEmpty(text) && !char.IsControl(text[0]) && flattener is { Count: > 0 })
			{
				searchBuffer += text;
				RestartSearchTimer();
				// A fresh single keystroke advances past the current row (so repeating a letter
				// cycles); a growing prefix re-matches from the current row so a settled match stays.
				int anchor = SelectedItem is SharpTreeNode current ? flattener.IndexOf(current) : -1;
				int from = searchBuffer.Length <= 1 ? anchor + 1 : anchor;
				if (FindPrefixMatch(from) is { } match)
				{
					SelectedItem = match;
					FocusNode(match);
				}
				e.Handled = true;
			}
			if (!e.Handled)
				base.OnTextInput(e);
		}

		SharpTreeNode? FindPrefixMatch(int from)
		{
			if (flattener is null)
				return null;
			int count = flattener.Count;
			if (from < 0)
				from = 0;
			for (int k = 0; k < count; k++)
			{
				if (flattener[(from + k) % count] is SharpTreeNode node
					&& node.Text?.ToString() is { } text
					&& text.StartsWith(searchBuffer, StringComparison.OrdinalIgnoreCase))
				{
					return node;
				}
			}
			return null;
		}

		void RestartSearchTimer()
		{
			searchResetTimer ??= new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
			searchResetTimer.Stop();
			searchResetTimer.Tick -= OnSearchTimeout;
			searchResetTimer.Tick += OnSearchTimeout;
			searchResetTimer.Start();
		}

		void OnSearchTimeout(object? sender, EventArgs e)
		{
			searchResetTimer?.Stop();
			searchBuffer = string.Empty;
		}

		/// <summary>Selected items with no selected ancestor (used by Delete).</summary>
		public IEnumerable<SharpTreeNode> GetTopLevelSelection()
		{
			var selection = SelectedItems!.OfType<SharpTreeNode>().ToHashSet();
			return selection.Where(item => item.Ancestors().All(a => !selection.Contains(a)));
		}

		#region Drag and drop

		// External Explorer drops are presented to the node under this format (the internal reorder
		// payload carries its own node-defined format from SharpTreeNode.Copy).
		const string FileDropFormat = "FileDrop";
		static readonly DataFormat<string> InternalDragFormat =
			DataFormat.CreateStringApplicationFormat("sharptreeview-drag");

		internal enum DropPlace { Before, Inside, After }

		SharpTreeNode[]? draggedNodes;
		IPlatformDataObject? dragData;
		SharpTreeNode? pressedNode;
		PointerPressedEventArgs? dragPress;
		Point dragStartPoint;
		Border? insertMarker;

		void OnDragPointerReleased(object? sender, PointerReleasedEventArgs e)
		{
			pressedNode = null;
			dragPress = null;
		}

		void OnDragPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			pressedNode = null;
			dragPress = null;
			if (e.Source is not Visual hit || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
				return;
			if (hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true)?.Node is { } node)
			{
				pressedNode = node;
				dragPress = e;
				dragStartPoint = e.GetPosition(this);
			}
		}

		async void OnDragPointerMoved(object? sender, PointerEventArgs e)
		{
			if (pressedNode is null || dragPress is not { } press)
				return;
			if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
			{
				pressedNode = null;
				dragPress = null;
				return;
			}
			var delta = e.GetPosition(this) - dragStartPoint;
			if (Math.Abs(delta.X) < 4 && Math.Abs(delta.Y) < 4)
				return;

			var nodes = ResolveDraggedSet(pressedNode);
			pressedNode = null;
			dragPress = null;
			if (nodes.Length == 0 || !nodes[0].CanDrag(nodes))
				return;

			draggedNodes = nodes;
			dragData = nodes[0].Copy(nodes);
			try
			{
				var data = new DataTransfer();
				data.Add(DataTransferItem.Create(InternalDragFormat, "1"));
				await DragDrop.DoDragDropAsync(press, data, DragDropEffects.Move | DragDropEffects.Copy);
			}
			finally
			{
				draggedNodes = null;
				dragData = null;
				HideInsertMarker();
			}
		}

		// The dragged set: the whole selection when the pressed row is part of it, else the pressed row.
		SharpTreeNode[] ResolveDraggedSet(SharpTreeNode pressed)
		{
			var selection = SelectedItems!.OfType<SharpTreeNode>().ToArray();
			return selection.Contains(pressed) ? selection : new[] { pressed };
		}

		void OnDragOver(object? sender, DragEventArgs e)
		{
			var args = new AvaloniaPlatformDragEventArgs(BuildPlatformData(e));
			if (PickAcceptingTarget(args, ResolveDropTarget(e)) is { } target)
			{
				e.DragEffects = args.Effects.ToAvalonia();
				ShowInsertMarker(target.Item, target.Place);
			}
			else
			{
				e.DragEffects = DragDropEffects.None;
				HideInsertMarker();
			}
			e.Handled = true;
		}

		void OnDrop(object? sender, DragEventArgs e)
		{
			HideInsertMarker();
			var args = new AvaloniaPlatformDragEventArgs(BuildPlatformData(e));
			if (PickAcceptingTarget(args, ResolveDropTarget(e)) is { } target)
			{
				target.Node.InternalDrop(args, target.Index);
				e.DragEffects = args.Effects.ToAvalonia();
			}
			e.Handled = true;
		}

		// Dropping onto the middle 50% of a row lands DropPlace.Inside on the row's own node. Most
		// concrete SharpTreeNode subclasses inherit the base CanDrop (returns false), so a literal
		// "drop onto an assembly row" would otherwise show the no-cursor even though the root happily
		// accepts the payload. Fall back to the empty-space (root) target in that case -- same
		// fallback as the empty-space-below-last-row path.
		internal DropTarget? PickAcceptingTarget(IPlatformDragEventArgs args, DropTarget? initial)
		{
			if (initial is { } first && first.Node.CanDrop(args, first.Index))
				return first;
			if (ResolveEmptySpaceDropTarget() is { } fallback
				&& (initial is null || !ReferenceEquals(fallback.Node, initial.Value.Node))
				&& fallback.Node.CanDrop(args, fallback.Index))
				return fallback;
			return null;
		}

		internal readonly record struct DropTarget(SharpTreeNode Node, int Index, DropPlace Place, SharpTreeViewItem? Item);

		DropTarget? ResolveDropTarget(DragEventArgs e)
		{
			// External Explorer drops over the gap below the last row arrive with e.Source pointing at
			// the ListBox-inner ScrollViewer / ItemsPresenter / the tree itself rather than a row.
			// Fall back to "target the root, Inside, at the end of its children" so dropping onto an
			// empty list still calls the root's CanDrop/Drop -- the WPF SharpTreeView did the same.
			if (e.Source is not Visual hit
				|| hit.FindAncestorOfType<SharpTreeViewItem>(includeSelf: true) is not { Node: { } node } item)
				return ResolveEmptySpaceDropTarget();
			double h = item.Bounds.Height;
			double y = e.GetPosition(item).Y;
			DropPlace place = y < h * 0.25 ? DropPlace.Before : y > h * 0.75 ? DropPlace.After : DropPlace.Inside;
			switch (place)
			{
				case DropPlace.Inside:
					return new DropTarget(node, node.Children.Count, place, item);
				default:
					if (node.Parent is not { } parent)
						return new DropTarget(node, node.Children.Count, DropPlace.Inside, item);
					int idx = parent.Children.IndexOf(node);
					return new DropTarget(parent, place == DropPlace.After ? idx + 1 : idx, place, item);
			}
		}

		internal DropTarget? ResolveEmptySpaceDropTarget()
		{
			if (Root is not { } root)
				return null;
			return new DropTarget(root, root.Children.Count, DropPlace.Inside, Item: null);
		}

		IPlatformDataObject BuildPlatformData(DragEventArgs e)
		{
			// Internal drag: the node-built payload from Copy. External: pack the dropped file paths.
			if (dragData != null)
				return dragData;
			var data = new AvaloniaDataObject();
			if (e.DataTransfer.Contains(DataFormat.File) && e.DataTransfer.TryGetFiles() is { } storageItems)
			{
				var files = storageItems
					.Select(f => f.TryGetLocalPath())
					.Where(p => !string.IsNullOrEmpty(p))
					.Select(p => p!)
					.ToArray();
				if (files.Length > 0)
					data.SetData(FileDropFormat, files);
			}
			return data;
		}

		void ShowInsertMarker(SharpTreeViewItem? item, DropPlace place)
		{
			// item is only null for the empty-space fallback target, whose place is always
			// DropPlace.Inside -- the early return below covers that and the marker stays hidden.
			if (place == DropPlace.Inside || item is null || AdornerLayer.GetAdornerLayer(this) is not { } layer)
			{
				HideInsertMarker();
				return;
			}
			if (insertMarker == null)
			{
				insertMarker = new Border { IsHitTestVisible = false, BorderBrush = Brushes.DodgerBlue };
				layer.Children.Add(insertMarker);
			}
			AdornerLayer.SetAdornedElement(insertMarker, item);
			insertMarker.BorderThickness = place == DropPlace.After
				? new Thickness(0, 0, 0, 2)
				: new Thickness(0, 2, 0, 0);
			insertMarker.IsVisible = true;
		}

		void HideInsertMarker()
		{
			if (insertMarker != null)
				insertMarker.IsVisible = false;
		}

		#endregion
	}
}
