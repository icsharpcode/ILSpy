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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Threading;

using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>
	/// Two-way binds a <see cref="SharpTreeView"/>'s selection to a view-model's
	/// <see cref="ObservableCollection{SharpTreeNode}"/>: user selection flows into the model, and a
	/// model-driven change (restore, navigate, freshly-opened nodes) reveals + focuses the primary in
	/// the tree. One implementation shared by every tree pane, replacing the per-pane sync code.
	/// </summary>
	public sealed class TreeSelectionBinder : IDisposable
	{
		readonly SharpTreeView tree;
		readonly ObservableCollection<SharpTreeNode> modelSelection;
		bool syncing;

		public TreeSelectionBinder(SharpTreeView tree, ObservableCollection<SharpTreeNode> modelSelection)
		{
			this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
			this.modelSelection = modelSelection ?? throw new ArgumentNullException(nameof(modelSelection));
			tree.SelectionChanged += OnTreeSelectionChanged;
			tree.Loaded += OnTreeLoaded;
			modelSelection.CollectionChanged += OnModelSelectionChanged;
			// The model may already carry a selection (restored, or set by Analyze, before the view
			// was realised). Apply it now; if the tree isn't attached yet the ListBox drops the
			// SelectedItems add, so OnTreeLoaded re-applies once it is.
			if (modelSelection.Count > 0)
				SyncModelToTree();
		}

		// A selection applied before the tree was attached doesn't stick (the ListBox isn't an
		// initialised ItemsControl yet); re-apply once it loads so the row shows selected/focused.
		void OnTreeLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (modelSelection.Count > 0 && tree.SelectedItems!.Count == 0)
				SyncModelToTree();
		}

		public void Dispose()
		{
			tree.SelectionChanged -= OnTreeSelectionChanged;
			tree.Loaded -= OnTreeLoaded;
			modelSelection.CollectionChanged -= OnModelSelectionChanged;
		}

		/// <summary>Re-applies the model selection to the tree (e.g. after the tree's Root rebinds).</summary>
		public void Refresh() => SyncModelToTree();

		// Tree -> model: mirror the ListBox selection (already SharpTreeNodes) into the view-model.
		void OnTreeSelectionChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (syncing)
				return;
			syncing = true;
			try
			{
				var current = tree.SelectedItems!.OfType<SharpTreeNode>().ToHashSet();
				for (int i = modelSelection.Count - 1; i >= 0; i--)
				{
					if (!current.Contains(modelSelection[i]))
						modelSelection.RemoveAt(i);
				}
				foreach (var node in current)
				{
					if (!modelSelection.Contains(node))
						modelSelection.Add(node);
				}
			}
			finally
			{
				syncing = false;
			}
		}

		void OnModelSelectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (syncing)
				return;
			SyncModelToTree();
		}

		void SyncModelToTree()
		{
			syncing = true;
			try
			{
				// Snapshot which selected rows are already fully visible BEFORE mutating the selection:
				// the change triggers the ListBox's AutoScrollToSelectedItem, which drags an off-screen
				// row to an edge and would make it look "visible" by reveal time. A row that was already
				// on screen must not be revealed/recentred (e.g. Decompile to new tab on a visible row);
				// a genuinely off-screen one still gets centred (Back navigation, go-to-definition).
				var visibleBefore = new HashSet<SharpTreeNode>(ReferenceEqualityComparer.Instance);
				foreach (var node in modelSelection)
					if (tree.IsNodeFullyVisible(node))
						visibleBefore.Add(node);

				tree.SelectedItems!.Clear();
				SharpTreeNode? primary = null;
				var items = tree.ItemsSource as IList;
				foreach (var node in modelSelection)
				{
					// Expand ancestors (no scroll) so the row exists in the flattener; only select
					// rows it actually contains -- adding an off-list node corrupts the ListBox
					// SelectionModel (it throws on later enumeration).
					foreach (var ancestor in node.Ancestors())
						ancestor.IsExpanded = true;
					if (items != null && items.Contains(node))
					{
						tree.SelectedItems.Add(node);
						primary = node;
					}
				}
				// Reveal + focus the primary AFTER layout settles -- a model change that also reshapes
				// the tree (a reorder rebuilds the flattener) leaves the panel mid-arrange, and a
				// synchronous ScrollIntoView would throw "Invalid Arrange rectangle".
				if (primary is { } toReveal)
				{
					bool wasVisible = visibleBefore.Contains(toReveal);
					Dispatcher.UIThread.Post(() => {
						if (!wasVisible)
							tree.ScrollIntoNodeView(toReveal);
						tree.FocusNode(toReveal, scroll: !wasVisible);
					});
				}
			}
			finally
			{
				syncing = false;
			}
		}
	}
}
