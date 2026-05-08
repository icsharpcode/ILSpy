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
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

using ILSpy.AppEnv;
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
			AttachContextMenu(TryGetContextMenuEntries());
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
			var built = ContextMenuProvider.Build(contextMenuEntries, context);
			if (built == null)
			{
				e.Cancel = true;
				return;
			}
			menu.Items.Clear();
			foreach (var item in built.Items.Cast<Control>().ToArray())
			{
				built.Items.Remove(item);
				menu.Items.Add(item);
			}
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
			ClearFilterRow();
			itemsView = null;
			if (boundModel == null)
				return;
			foreach (var c in boundModel.Columns)
				grid.Columns.Add(c);
			BuildFilterRow(grid, boundModel);
			var model = boundModel;
			itemsView = new DataGridCollectionView(model.Items) {
				Filter = item => MetadataTablePageModel.MatchesFilters(item, model.ColumnFilters),
			};
			grid.ItemsSource = itemsView;
		}

		void ClearFilterRow()
		{
			var grid = this.FindControl<DataGrid>("Grid");
			if (grid != null)
				grid.LayoutUpdated -= SyncFilterRowWidths;
			var row = this.FindControl<StackPanel>("FilterRow");
			row?.Children.Clear();
		}

		void BuildFilterRow(DataGrid grid, MetadataTablePageModel model)
		{
			var row = this.FindControl<StackPanel>("FilterRow");
			if (row is null)
				return;
			for (int i = 0; i < grid.Columns.Count && i < model.ColumnFilters.Count; i++)
			{
				var filter = model.ColumnFilters[i];
				var box = new TextBox {
					MinHeight = 0,
					Padding = new Thickness(2, 1),
					Margin = new Thickness(0),
					HorizontalAlignment = HorizontalAlignment.Stretch,
					Tag = grid.Columns[i],
					PlaceholderText = filter.ColumnName,
					Text = filter.Text,
				};
				// Two-way bridge between the box and the filter via plain events. The filter's
				// PropertyChanged handler is hooked from the page model side and re-raises a
				// single ColumnFilterChanged the view uses to refresh the collection view.
				box.TextChanged += (_, _) => {
					if (filter.Text != box.Text)
						filter.Text = box.Text;
				};
				filter.PropertyChanged += (_, e) => {
					if (e.PropertyName == nameof(ColumnFilter.Text) && box.Text != filter.Text)
						box.Text = filter.Text;
				};
				row.Children.Add(box);
			}
			// Snap each input to the corresponding column's ActualWidth on every layout
			// pass — DataGridColumn.ActualWidth isn't a styled property, so we can't
			// subscribe to it; piggy-backing on LayoutUpdated is cheap and reliable.
			grid.LayoutUpdated += SyncFilterRowWidths;
			SyncFilterRowWidths(grid, EventArgs.Empty);
		}

		void SyncFilterRowWidths(object? sender, EventArgs e)
		{
			var row = this.FindControl<StackPanel>("FilterRow");
			if (row is null)
				return;
			foreach (var child in row.Children)
			{
				if (child is TextBox box && box.Tag is DataGridColumn column)
					box.Width = column.ActualWidth;
			}
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
