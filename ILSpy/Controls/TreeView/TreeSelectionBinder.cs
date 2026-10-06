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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>
	/// Two-way binds a <see cref="SharpTreeView"/>'s selection to a view-model's
	/// <see cref="ObservableCollection{SharpTreeNode}"/>: user selection flows into the model, and a
	/// model-driven change (restore, navigate, freshly-opened nodes) reveals the primary in the tree
	/// and focuses it when the tree already owns the keyboard focus (see the focusOnSelect parameter).
	/// One implementation shared by every tree pane, replacing the per-pane sync code.
	/// </summary>
	public sealed class TreeSelectionBinder : IDisposable
	{
		readonly SharpTreeView tree;
		readonly ObservableCollection<SharpTreeNode> modelSelection;
		readonly Func<IDisposable>? batchSelectionChange;
		readonly bool focusOnSelect;
		bool syncing;

		/// <param name="batchSelectionChange">
		/// Optional: opens a scope over which the view-model coalesces its selection fan-out, so a
		/// sync that touches many rows costs one notification instead of one per row. Panes whose
		/// model has no such scope pass null and get the per-item behaviour.
		/// </param>
		/// <param name="focusOnSelect">
		/// True: every model-driven selection moves the keyboard focus to the selected row (the
		/// Analyzer pane, where an Analyze request lands the user in the pane). False: the row is
		/// only scrolled into view unless the tree already owns the keyboard focus, so Back/Forward,
		/// search-result jumps, go-to-definition and tab activation leave the focus where it is.
		/// </param>
		public TreeSelectionBinder(SharpTreeView tree, ObservableCollection<SharpTreeNode> modelSelection,
			Func<IDisposable>? batchSelectionChange = null, bool focusOnSelect = false)
		{
			this.tree = tree ?? throw new ArgumentNullException(nameof(tree));
			this.modelSelection = modelSelection ?? throw new ArgumentNullException(nameof(modelSelection));
			this.batchSelectionChange = batchSelectionChange;
			this.focusOnSelect = focusOnSelect;
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
				// One batch for the whole reconciliation: each add/remove otherwise fans out into a
				// full command re-query and a decompile of the intermediate selection.
				using var batch = batchSelectionChange?.Invoke();
				// Membership comes from a set, not a scan of modelSelection per node -- with every
				// row selected the linear scan made this quadratic.
				var kept = new HashSet<SharpTreeNode>();
				for (int i = modelSelection.Count - 1; i >= 0; i--)
				{
					if (current.Contains(modelSelection[i]))
						kept.Add(modelSelection[i]);
					else
						modelSelection.RemoveAt(i);
				}
				foreach (var node in current)
				{
					if (!kept.Contains(node))
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
				// Reveal (+ focus) the primary AFTER layout settles -- a model change that also reshapes
				// the tree (a reorder rebuilds the flattener) leaves the panel mid-arrange, and a
				// synchronous ScrollIntoView would throw "Invalid Arrange rectangle". Whether to focus is
				// decided now, from where the keyboard focus sits at the time of the selection change.
				if (primary is { } toReveal)
				{
					bool wasVisible = visibleBefore.Contains(toReveal);
					bool focus = focusOnSelect || TreeOwnsKeyboardFocus();
					Dispatcher.UIThread.Post(() => {
						if (!wasVisible)
							tree.ScrollIntoNodeView(toReveal);
						if (focus)
							tree.FocusNode(toReveal, scroll: !wasVisible);
					});
				}
			}
			finally
			{
				syncing = false;
			}
		}

		// WPF's SelectNodes scrolled the row into view without focusing it; the row took the focus
		// only when the pane itself was activated. The equivalent here: a tree that already holds
		// the keyboard focus (or a window where nothing holds it yet, e.g. the selection restored at
		// startup) follows the selection, any other focused control keeps the focus.
		bool TreeOwnsKeyboardFocus()
		{
			var focused = TopLevel.GetTopLevel(tree)?.FocusManager?.GetFocusedElement();
			return focused is null or TopLevel
				|| (focused is Visual visual && (ReferenceEquals(visual, tree) || tree.IsVisualAncestorOf(visual)));
		}
	}
}
