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
using System.Reflection;

using Avalonia.Controls;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

using ICSharpCode.ILSpyX.TreeView;

namespace ILSpy.Controls
{
	/// <summary>
	/// A tree whose keyboard gestures the <see cref="TreeKeyboardController"/> drives. The
	/// controller reads the focused node and asks the model to move the selection, so each tree
	/// keeps its own select-and-reveal behaviour (decompile, navigate, scroll-into-view).
	/// </summary>
	public interface ITreeKeyboardTarget
	{
		SharpTreeNode? PrimarySelectedNode { get; }
		void SelectNode(SharpTreeNode? node);
	}

	/// <summary>
	/// Adds the standard tree keyboard gestures to a hierarchical <see cref="DataGrid"/>:
	/// Left/Right collapse-expand + parent/child navigation, Numpad +/-/* (including recursive
	/// expand), and type-ahead incremental search. Expansion is driven through the grid's
	/// <see cref="IHierarchicalModel"/>; selection moves delegate to the
	/// <see cref="ITreeKeyboardTarget"/>. Attach one per tree (assembly tree + analyzer tree)
	/// for consistent behaviour.
	/// </summary>
	public sealed class TreeKeyboardController
	{
		readonly DataGrid grid;
		readonly DispatcherTimer searchResetTimer;
		string searchBuffer = string.Empty;

		// ProDataGrid internals reached reflectively to fix shift-range shrinking (see OnShiftRangeKey).
		static readonly MethodInfo? setRowSelectionMethod =
			typeof(DataGrid).GetMethod("SetRowSelection", BindingFlags.NonPublic | BindingFlags.Instance);

		public TreeKeyboardController(DataGrid grid)
		{
			this.grid = grid ?? throw new ArgumentNullException(nameof(grid));
			searchResetTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
			searchResetTimer.Tick += (_, _) => { searchResetTimer.Stop(); searchBuffer = string.Empty; };
			grid.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble);
			grid.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Bubble);
			// Runs even after the DataGrid marks the key handled, so we can repair its selection.
			grid.AddHandler(InputElement.KeyDownEvent, OnShiftRangeKey, RoutingStrategies.Bubble, handledEventsToo: true);
		}

		// The model is the grid's DataContext (the tree's UserControl sets it). Resolved per
		// gesture so the controller can be created before the DataContext is assigned.
		ITreeKeyboardTarget? Target => grid.DataContext as ITreeKeyboardTarget;

		IHierarchicalModel? Model => grid.HierarchicalModel as IHierarchicalModel;

		void OnKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.KeyModifiers != KeyModifiers.None)
				return;
			if (Target is not { } target
				|| target.PrimarySelectedNode is not { } current
				|| Model is not { } hm
				|| hm.FindNode(current) is not { } node)
				return;

			switch (e.Key)
			{
				case Key.Left:
					// Collapse if open; else step out to the parent (Parent is null on a root row).
					if (node.IsExpanded)
						hm.Collapse(node);
					else if (node.Parent?.Item is SharpTreeNode parent)
						target.SelectNode(parent);
					else
						return;
					break;
				case Key.Right:
					// Expand if closed and not a leaf; else step into the first child.
					if (!node.IsExpanded && !node.IsLeaf)
						hm.Expand(node);
					else if (node.IsExpanded && node.Children.Count > 0 && node.Children[0].Item is SharpTreeNode child)
						target.SelectNode(child);
					else
						return;
					break;
				case Key.Add:
					if (node.IsLeaf)
						return;
					hm.Expand(node);
					break;
				case Key.Subtract:
					hm.Collapse(node);
					break;
				case Key.Multiply:
					ExpandRecursively(hm, node);
					break;
				default:
					return;
			}
			e.Handled = true;
		}

		void OnTextInput(object? sender, TextInputEventArgs e)
		{
			var text = e.Text;
			if (string.IsNullOrEmpty(text) || char.IsControl(text[0]))
				return;
			if (Target is not { } target || Model is not { } hm)
				return;
			var flattened = hm.Flattened;
			if (flattened.Count == 0)
				return;

			searchBuffer += text;
			searchResetTimer.Stop();
			searchResetTimer.Start();

			// Anchor the search at the current selection. A fresh single keystroke advances past it
			// (so repeating a letter cycles through matches); accumulating a longer prefix re-matches
			// from the current row so a settled selection that still matches stays put.
			int anchor = IndexOf(flattened, target.PrimarySelectedNode);
			int from = searchBuffer.Length <= 1 ? anchor + 1 : anchor;

			var match = FindPrefixMatch(flattened, searchBuffer, from);
			if (match?.Item is SharpTreeNode node)
			{
				target.SelectNode(node);
				e.Handled = true;
			}
		}

		static int IndexOf(IReadOnlyList<HierarchicalNode> flattened, SharpTreeNode? node)
		{
			if (node is null)
				return -1;
			for (int i = 0; i < flattened.Count; i++)
			{
				if (ReferenceEquals(flattened[i].Item, node))
					return i;
			}
			return -1;
		}

		static HierarchicalNode? FindPrefixMatch(IReadOnlyList<HierarchicalNode> flattened, string prefix, int from)
		{
			if (from < 0)
				from = 0;
			for (int k = 0; k < flattened.Count; k++)
			{
				var candidate = flattened[(from + k) % flattened.Count];
				if (candidate.Item is SharpTreeNode stn
					&& stn.Text?.ToString() is { } textValue
					&& textValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
				{
					return candidate;
				}
			}
			return null;
		}

		// Workaround for a ProDataGrid bug: a shift+nav key moves the grid's current row and anchor
		// correctly, but SelectFromAnchorToCurrent only ADDS the [anchor..current] range and never
		// deselects rows outside it -- so shrinking a shift-selection (Shift+Up after extending down)
		// is a no-op and the dropped rows stay selected. After the grid has processed the key, prune
		// the selection back to exactly the [anchor..current] range.
		//
		// The prune uses the internal SetRowSelection(slot, isSelected:false, setAnchorSlot:false),
		// NOT SelectedItems.Remove: removing an item makes the grid re-derive its current row (which
		// corrupts the anchor for the next key), whereas SetRowSelection deselects a slot in place
		// and leaves current/anchor untouched.
		void OnShiftRangeKey(object? sender, KeyEventArgs e)
		{
			if ((e.KeyModifiers & KeyModifiers.Shift) == 0
				|| grid.SelectionMode != DataGridSelectionMode.Extended
				|| setRowSelectionMethod is null
				|| e.Key is not (Key.Up or Key.Down or Key.Home or Key.End or Key.PageUp or Key.PageDown))
				return;
			Dispatcher.UIThread.Post(PruneToShiftRange);
		}

		void PruneToShiftRange()
		{
			if (Model is not { } hm || setRowSelectionMethod is null)
				return;
			int anchor = ReadGridSlot("AnchorSlot");
			int current = ReadGridSlot("CurrentSlot");
			if (anchor < 0 || current < 0)
				return;
			int lo = Math.Min(anchor, current), hi = Math.Max(anchor, current);
			var flattened = hm.Flattened;
			if (hi >= flattened.Count)
				return;
			// Collect first (don't mutate the selection while iterating it), then deselect.
			var slotsToDrop = new List<int>();
			foreach (var item in grid.SelectedItems)
			{
				var node = item is HierarchicalNode hn ? hn.Item as SharpTreeNode : item as SharpTreeNode;
				if (node is null)
					continue;
				int slot = IndexOf(flattened, node);
				if (slot >= 0 && (slot < lo || slot > hi))
					slotsToDrop.Add(slot);
			}
			foreach (int slot in slotsToDrop)
				setRowSelectionMethod.Invoke(grid, new object[] { slot, false, false });
		}

		// Reads a DataGrid slot property/field by name; -1 if unavailable (defensive against a
		// future ProDataGrid that renames or removes it -- the worst case is the pre-fix behaviour).
		int ReadGridSlot(string name)
		{
			var type = grid.GetType();
			var member = (object?)type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
				?? type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
			object? value = member switch {
				PropertyInfo pi => pi.GetValue(grid),
				FieldInfo fi => fi.GetValue(grid),
				_ => null,
			};
			return value is int slot ? slot : -1;
		}

		// Numpad-* recursive expand. Recurses only into children that opt in via
		// SharpTreeNode.CanExpandRecursively (false for lazy-loading nodes), so it stays bounded.
		static void ExpandRecursively(IHierarchicalModel hm, HierarchicalNode node)
		{
			if (node.IsLeaf)
				return;
			hm.Expand(node);
			foreach (var child in node.Children)
			{
				if (child.Item is SharpTreeNode { CanExpandRecursively: false })
					continue;
				ExpandRecursively(hm, child);
			}
		}
	}
}
