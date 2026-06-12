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
using System.Reflection;

using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Views
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
			var grid = this.FindControl<DataGrid>("Grid");
			if (grid is not null)
			{
				grid.DoubleTapped += OnGridDoubleTapped;
				grid.LoadingRow += OnGridLoadingRow;
				// Bubble + handledEventsToo: ProDataGrid's row-level pointer handlers mark
				// PointerPressed handled before bubble reaches our subscription, so we have to
				// opt into "see handled events too" to react. Tunnel was tried first but
				// ProDataGrid's hierarchy seemingly intercepts the tunnel pass for non-primary
				// buttons — bubble + handledEventsToo is the reliable path.
				grid.AddHandler(PointerPressedEvent, OnGridPointerPressed,
					global::Avalonia.Interactivity.RoutingStrategies.Bubble,
					handledEventsToo: true);
			}
		}

		void OnGridDoubleTapped(object? sender, global::Avalonia.Input.TappedEventArgs e)
		{
			// Plain double-click reuses the active tab — the dock workspace's row-activation
			// subscriber resolves the row's metadataFile + Token to an IEntity and selects
			// the matching tree node. The "open in new tab" gesture is middle-click, handled
			// in OnGridPointerPressed.
			if (DataContext is not MetadataTablePageModel page)
				return;
			// Resolve the row from the double-tapped element rather than grid.SelectedItem: a
			// double-click on a column header (e.g. rapidly toggling its sort button) must NOT
			// navigate to the currently-selected row — only a double-click on a data row does.
			if (FindActivatableRow(e.Source as Visual) is { DataContext: { } row })
				page.RaiseRowActivated(row);
		}

		/// <summary>
		/// Walks up from <paramref name="source"/> to the data row a click gesture may
		/// activate. Returns <see langword="null"/> when the gesture landed outside any row,
		/// or inside the row's details area — interacting with details content (selecting
		/// text in an embedded-source blob, scrolling a flags sub-grid) must not navigate
		/// away from the metadata view.
		/// </summary>
		internal static DataGridRow? FindActivatableRow(Visual? source)
		{
			var visual = source;
			while (visual is not null and not DataGridRow)
			{
				if (visual is global::Avalonia.Controls.Primitives.DataGridDetailsPresenter)
					return null;
				visual = visual.GetVisualParent();
			}
			return visual as DataGridRow;
		}

		void OnGridLoadingRow(object? sender, DataGridRowEventArgs e)
		{
			// Row containers are recycled with AreDetailsVisible reset to false, so the
			// per-item visibility has to be re-derived every time a container materialises.
			if (boundModel?.IsRowDetailsVisible is not { } isVisible)
				return;
			if (e.Row.DataContext is { } item)
				e.Row.AreDetailsVisible = isVisible(item);
		}

		void OnGridPointerPressed(object? sender, PointerPressedEventArgs e)
		{
			// Middle-click on a row → open in a new decompiler tab. MMB doesn't change
			// selection, so hit-test the row from e.Source rather than relying on
			// SelectedItem (which would point at the previously-clicked row).
			if (DataContext is not MetadataTablePageModel page)
				return;
			if (e.Source is not Visual hit
				|| !e.GetCurrentPoint(hit).Properties.IsMiddleButtonPressed)
				return;
			if (FindActivatableRow(hit) is not { } row || row.DataContext is null)
				return;
			page.RaiseRowActivated(row.DataContext, openInNewTab: true);
			e.Handled = true;
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

			// Always-available "Copy" — copies the single-cell value under the pointer.
			// Disabled when no cell was hovered (e.g. right-click on empty space below the
			// rows). Built before the MEF entries so it stays at the top of the menu.
			var copyItem = new MenuItem {
				Header = "Copy",
				InputGesture = new KeyGesture(Key.C, KeyModifiers.Control),
				IsEnabled = hoveredCell is not null,
			};
			var cellSnapshot = hoveredCell;
			copyItem.Click += async (_, _) => {
				var text = ReadCellText(cellSnapshot);
				if (string.IsNullOrEmpty(text))
					return;
				var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
				if (clipboard is not null)
					await clipboard.SetTextAsync(text);
			};
			menu.Items.Add(copyItem);

			// "Copy Row" — TSV dump of the row that owns the hovered cell. Uses the same
			// format-aware ReadCellText for each column, so hex columns land as hex.
			var copyRowItem = new MenuItem {
				Header = "Copy Row",
				IsEnabled = hoveredCell?.DataContext is not null && grid is not null,
			};
			var rowSnapshot = hoveredCell?.DataContext;
			var gridSnapshot = grid;
			copyRowItem.Click += async (_, _) => {
				if (rowSnapshot is null || gridSnapshot is null)
					return;
				var text = BuildRowText(gridSnapshot, rowSnapshot);
				if (string.IsNullOrEmpty(text))
					return;
				var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
				if (clipboard is not null)
					await clipboard.SetTextAsync(text);
			};
			menu.Items.Add(copyRowItem);

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

		static string ReadCellText(DataGridCell? cell)
		{
			if (cell?.OwningColumn is null || cell.DataContext is null)
				return string.Empty;
			var columnName = cell.OwningColumn.Tag as string
				?? cell.OwningColumn.Header?.ToString();
			return ReadColumnValue(cell.DataContext, columnName);
		}

		static string BuildRowText(DataGrid grid, object row)
		{
			var sb = new System.Text.StringBuilder();
			for (int i = 0; i < grid.Columns.Count; i++)
			{
				if (i > 0)
					sb.Append('\t');
				var name = grid.Columns[i].Tag as string ?? grid.Columns[i].Header?.ToString();
				sb.Append(ReadColumnValue(row, name));
			}
			return sb.ToString();
		}

		static string ReadColumnValue(object row, string? columnName)
		{
			if (string.IsNullOrEmpty(columnName))
				return string.Empty;
			var prop = row.GetType().GetProperty(columnName,
				BindingFlags.Public | BindingFlags.Instance);
			if (prop is null)
				return string.Empty;
			var info = prop.GetCustomAttribute<ColumnInfoAttribute>();
			object? value;
			try
			{ value = prop.GetValue(row); }
			catch { return string.Empty; }
			if (value is null)
				return string.Empty;
			// Mirror the formatting the column itself uses, so what's on the clipboard
			// matches what the user sees ("0x06000001" rather than "100663297"). Enum
			// values must be widened to their underlying integer first — Enum.ToString
			// rejects multi-character format specs like "X8".
			if (!string.IsNullOrEmpty(info?.Format))
			{
				if (value is Enum enumValue)
					value = System.Convert.ChangeType(enumValue, enumValue.GetTypeCode(),
						System.Globalization.CultureInfo.InvariantCulture);
				if (value is IFormattable f)
					return f.ToString(info.Format, System.Globalization.CultureInfo.InvariantCulture);
			}
			return value.ToString() ?? string.Empty;
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
			// Apply the row-details schema before the new items attach so the first row
			// materialisation already sees the right template and visibility mode. The grid's
			// property is annotated non-nullable but null is its documented "no details" value.
			grid.RowDetailsTemplate = boundModel?.RowDetailsTemplate!;
			grid.RowDetailsVisibilityMode = boundModel?.RowDetailsVisibilityMode
				?? DataGridRowDetailsVisibilityMode.Collapsed;
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
