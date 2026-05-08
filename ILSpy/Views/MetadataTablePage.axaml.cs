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
using System.ComponentModel;
using System.Linq;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Metadata;
using ILSpy.ViewModels;

namespace ILSpy.Views
{
	/// <summary>
	/// DataGrid-backed view for <see cref="MetadataTablePageModel"/>. Avalonia DataGrid's
	/// <c>Columns</c> isn't a styled / bindable property, so the column list is rebound
	/// imperatively from the model whenever the DataContext or the model's <c>Columns</c>
	/// list changes.
	/// </summary>
	public partial class MetadataTablePage : UserControl
	{
		MetadataTablePageModel? boundModel;
		DataGridCollectionView? itemsView;
		DataGridCell? hoveredCell;

		public MetadataTablePage()
		{
			InitializeComponent();
			DataContextChanged += (_, _) => RebindModel();
			AddHandler(PointerMovedEvent, OnPointerMovedOverGrid);
			AddHandler(KeyDownEvent, OnKeyDown);
			AttachContextMenu(TryGetContextMenuEntries());
		}

		void OnKeyDown(object? sender, KeyEventArgs e)
		{
			// Ctrl+G mirrors the "Go to token" context-menu entry: dispatch through the
			// focused cell (or last-hovered cell as a fallback when focus hasn't landed on
			// the grid, e.g. after the user clicks a column header).
			if (e.Key != Key.G || (e.KeyModifiers & KeyModifiers.Control) == 0)
				return;
			var focusedCell = (TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() as Visual)
				?.FindAncestorOfType<DataGridCell>();
			var cell = focusedCell ?? hoveredCell;
			if (cell is not null && TryNavigateToTokenInCell(cell))
				e.Handled = true;
		}

		/// <summary>
		/// Dispatches the "Go to token" action on <paramref name="cell"/>. Returns
		/// <see langword="true"/> if the cell was a token-kind column on a metadata page,
		/// matching the context-menu entry's visibility test.
		/// </summary>
		internal bool TryNavigateToTokenInCell(DataGridCell cell)
		{
			ArgumentNullException.ThrowIfNull(cell);
			var grid = this.FindControl<DataGrid>("Grid");
			if (grid is null)
				return false;
			var ctx = new TextViewContext { DataGrid = grid, OriginalSource = cell };
			var entry = new GoToTokenContextMenuEntry();
			if (!entry.IsVisible(ctx))
				return false;
			entry.Execute(ctx);
			return true;
		}

		static IReadOnlyList<IContextMenuEntryExport> TryGetContextMenuEntries()
		{
			try
			{ return AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries; }
			catch { return Array.Empty<IContextMenuEntryExport>(); }
		}

		IReadOnlyList<IContextMenuEntryExport> contextMenuEntries = Array.Empty<IContextMenuEntryExport>();

		/// <summary>
		/// Replaces the active context-menu entries. Tests may call this directly to inject
		/// stub entries; at runtime construction registers the MEF-discovered set.
		/// </summary>
		internal void AttachContextMenu(IReadOnlyList<IContextMenuEntryExport> entries)
		{
			contextMenuEntries = entries;
			var grid = this.FindControl<DataGrid>("Grid");
			if (grid == null)
				return;
			var menu = new ContextMenu();
			menu.Opening += OnContextMenuOpening;
			grid.ContextMenu = menu;
		}

		void OnContextMenuOpening(object? sender, CancelEventArgs e)
		{
			if (sender is not ContextMenu menu)
				return;
			var grid = this.FindControl<DataGrid>("Grid");
			var context = new TextViewContext {
				DataGrid = grid,
				OriginalSource = hoveredCell,
			};
			menu.Items.Clear();

			// Always-available Copy. Avalonia DataGrid's built-in Ctrl+C copies the active
			// selection as TSV; mirror that here so right-click in a plain column cell still
			// produces a usable menu (the GoToToken entry shows only on token-kind cells).
			var copyItem = new MenuItem {
				Header = "Copy",
				InputGesture = new KeyGesture(Key.C, KeyModifiers.Control),
				IsEnabled = grid?.SelectedItem != null,
			};
			copyItem.Click += async (_, _) => {
				if (grid is null)
					return;
				var topLevel = TopLevel.GetTopLevel(grid);
				var clipboard = topLevel?.Clipboard;
				if (clipboard is null)
					return;
				var text = BuildClipboardText(grid);
				if (!string.IsNullOrEmpty(text))
					await clipboard.SetTextAsync(text);
			};
			menu.Items.Add(copyItem);

			var built = ContextMenuProvider.Build(contextMenuEntries, context);
			if (built != null)
			{
				menu.Items.Add(new Separator());
				foreach (var item in built.Items.Cast<Control>().ToArray())
				{
					built.Items.Remove(item);
					menu.Items.Add(item);
				}
			}
		}

		static string BuildClipboardText(DataGrid grid)
		{
			// Tab-separated row dump for the selected item — same shape DataGrid's built-in
			// Ctrl+C produces. Walk every column so the user sees the full row state, not
			// only the cell under the pointer.
			if (grid.SelectedItem is not { } row)
				return string.Empty;
			var sb = new System.Text.StringBuilder();
			for (int i = 0; i < grid.Columns.Count; i++)
			{
				if (i > 0)
					sb.Append('\t');
				var col = grid.Columns[i];
				if (col is DataGridTextColumn text && text.Binding is global::Avalonia.Data.Binding b
					&& b.Path is { } path
					&& row.GetType().GetProperty(path) is { } prop)
				{
					var v = prop.GetValue(row);
					sb.Append(v?.ToString());
				}
			}
			return sb.ToString();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		void OnPointerMovedOverGrid(object? sender, PointerEventArgs e)
		{
			// Walk up the visual tree from the pointer's source to the cell. Cells nest
			// arbitrary content (TextBlock for plain columns, Button for token columns), so
			// a per-cell PointerEntered subscription would miss most events.
			var visual = e.Source as Visual;
			while (visual is not null and not DataGridCell)
				visual = visual.GetVisualParent();
			var cell = visual as DataGridCell;
			if (cell == hoveredCell)
				return;
			hoveredCell = cell;
			if (cell is null)
				return;
			var columnName = cell.OwningColumn?.Tag as string
				?? cell.OwningColumn?.Header?.ToString();
			if (columnName is null)
				return;
			var tip = MetadataCellTooltip.Resolve(cell.DataContext!, columnName);
			ToolTip.SetTip(cell, tip);
		}

		void RebindModel()
		{
			if (boundModel != null)
			{
				boundModel.PropertyChanged -= OnModelPropertyChanged;
				boundModel.ColumnFilterChanged -= OnAnyColumnFilterChanged;
			}
			boundModel = DataContext as MetadataTablePageModel;
			if (boundModel != null)
			{
				boundModel.PropertyChanged += OnModelPropertyChanged;
				boundModel.ColumnFilterChanged += OnAnyColumnFilterChanged;
			}
			ApplySchema();
			ApplyScrollTarget();
		}

		void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(MetadataTablePageModel.Columns)
				or nameof(MetadataTablePageModel.Items))
				ApplySchema();
			else if (e.PropertyName == nameof(MetadataTablePageModel.ScrollToRow))
				ApplyScrollTarget();
		}

		void OnAnyColumnFilterChanged() => itemsView?.Refresh();

		void ApplySchema()
		{
			var grid = this.FindControl<DataGrid>("Grid");
			if (grid == null)
				return;
			// Detach items first so the layout pass between Columns.Clear() and the new
			// columns being added doesn't try to render existing rows against a column
			// list that doesn't match their cell collection — that race throws an
			// IndexOutOfRangeException out of DataGridCellsPresenter.MeasureOverride when
			// the new schema has more columns than the old one.
			grid.ItemsSource = Array.Empty<object>();
			grid.Columns.Clear();
			itemsView = null;
			if (boundModel == null)
				return;
			foreach (var c in boundModel.Columns)
				grid.Columns.Add(c);
			var model = boundModel;
			itemsView = new DataGridCollectionView(model.Items);
			grid.ItemsSource = itemsView;
			// Assign Filter AFTER attaching to the grid; the DataGridCollectionView's setter
			// short-circuits on _filter == value and an object-initializer assignment seems to
			// land on a transient state that the grid replaces, so the filter ends up null
			// on the live view by the time refresh requests come in.
			itemsView.Filter = item => MetadataTablePageModel.MatchesFilters(item, model.ColumnFilters);
		}

		void ApplyScrollTarget()
		{
			if (boundModel?.ScrollToRow is not int row)
				return;
			var grid = this.FindControl<DataGrid>("Grid");
			if (grid == null || boundModel.Items.Count == 0)
				return;
			int idx = Math.Clamp(row, 0, boundModel.Items.Count - 1);
			var item = boundModel.Items[idx];
			grid.ScrollIntoView(item, grid.Columns.Count > 0 ? grid.Columns[0] : null);
			grid.SelectedItem = item;
			boundModel.ScrollToRow = null;
		}
	}
}
