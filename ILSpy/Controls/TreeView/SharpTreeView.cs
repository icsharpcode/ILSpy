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
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;

namespace ILSpy.Controls.TreeView
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
			SelectionChanged += OnSelectionChanged;
			DoubleTapped += OnDoubleTapped;
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

		/// <summary>Scrolls the node into view and gives it keyboard focus.</summary>
		public void FocusNode(SharpTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);
			ScrollIntoNodeView(node);
			if (ContainerFromItem(node) is { } container)
				container.Focus();
			else
				Dispatcher.UIThread.Post(() => (ContainerFromItem(node))?.Focus(), DispatcherPriority.Loaded);
		}

		public void ScrollIntoNodeView(SharpTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);
			doNotScrollOnExpanding = true;
			foreach (var ancestor in node.Ancestors())
				ancestor.IsExpanded = true;
			doNotScrollOnExpanding = false;
			ScrollIntoView(node);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
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
							FocusNode(node.Parent);
						else
							break;
						e.Handled = true;
						break;
					case Key.Right:
						if (!node.IsExpanded && node.ShowExpander)
							node.IsExpanded = true;
						else if (node.Children.Count > 0)
							FocusNode(node.Children.First(c => c.IsVisible));
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
	}
}
