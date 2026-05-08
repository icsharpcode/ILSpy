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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ILSpy.ViewModels
{
	/// <summary>
	/// Tab content for a metadata-table view: an item list bound to a DataGrid plus the
	/// column descriptors the view rebinds on every <c>DataContext</c> change. The DataGrid's
	/// <c>Columns</c> collection isn't an Avalonia property, so the model can't bind it
	/// declaratively — the view assigns it imperatively from <see cref="Columns"/>.
	/// </summary>
	public sealed partial class MetadataTablePageModel : TabPageModel
	{
		[ObservableProperty]
		private IReadOnlyList<object> items = Array.Empty<object>();

		[ObservableProperty]
		private IReadOnlyList<DataGridColumn> columns = Array.Empty<DataGridColumn>();

		/// <summary>
		/// When set, the view scrolls the grid so the row at this 0-based index is visible
		/// (Phase 3 token navigation). Cleared back to <see langword="null"/> after the view
		/// honours it so back-navigation can re-trigger the same scroll.
		/// </summary>
		[ObservableProperty]
		private int? scrollToRow;

		/// <summary>
		/// One filter input per column, in the same order as <see cref="Columns"/>. The view
		/// renders each <see cref="ColumnFilter.Text"/> as a TextBox baked into that column's
		/// header; the filter predicate ANDs every non-empty entry, requiring each row's
		/// matching property's stringified value to contain its filter (case-insensitive).
		/// </summary>
		public ObservableCollection<ColumnFilter> ColumnFilters { get; } = new();

		/// <summary>
		/// Raised whenever any column's filter text changes. The view subscribes to this so
		/// it can refresh the underlying <c>DataGridCollectionView</c> without each column
		/// having to know how to find the live view instance.
		/// </summary>
		public event Action? ColumnFilterChanged;

		internal void RaiseColumnFilterChanged() => ColumnFilterChanged?.Invoke();

		/// <summary>
		/// Raised when the user clicks a hyperlink-styled token cell. The host (the dock
		/// workspace) resolves the (row, columnName) pair to a metadata token and navigates
		/// to the target table row.
		/// </summary>
		public event Action<MetadataCellNavigationEventArgs>? NavigateToCellRequested;

		internal void RaiseNavigateToCell(object row, string columnName)
			=> NavigateToCellRequested?.Invoke(new MetadataCellNavigationEventArgs(row, columnName));

		static readonly ConcurrentDictionary<(Type Type, string Column), PropertyInfo?> propertyLookupCache = new();

		/// <summary>
		/// Returns true when every <see cref="ColumnFilter"/> in <paramref name="filters"/>
		/// either has an empty value or the matching property on <paramref name="item"/>
		/// stringifies to a value that contains the filter case-insensitively. Used by the
		/// view's <c>DataGridCollectionView.Filter</c> and by tests.
		/// </summary>
		public static bool MatchesFilters(object item, IEnumerable<ColumnFilter> filters)
		{
			ArgumentNullException.ThrowIfNull(item);
			ArgumentNullException.ThrowIfNull(filters);
			foreach (var f in filters)
			{
				if (string.IsNullOrEmpty(f.Text))
					continue;
				var prop = propertyLookupCache.GetOrAdd((item.GetType(), f.ColumnName),
					static key => key.Type.GetProperty(key.Column,
						BindingFlags.Public | BindingFlags.Instance));
				if (prop is null)
					return false;
				object? value;
				try
				{ value = prop.GetValue(item); }
				catch { return false; }
				var s = value?.ToString();
				if (s is null || !s.Contains(f.Text, StringComparison.OrdinalIgnoreCase))
					return false;
			}
			return true;
		}
	}

	/// <summary>One column's filter entry — the column name plus its current filter text.</summary>
	public sealed partial class ColumnFilter : ObservableObject
	{
		public ColumnFilter(string columnName)
		{
			ColumnName = columnName;
		}

		public string ColumnName { get; }

		// Backing field is `text` rather than `value` because the latter collides with C#'s
		// contextual identifier for property setters and trips Avalonia's INPC binding
		// plugin on write (the cached accessor's target reference comes back null).
		[ObservableProperty]
		private string? text;
	}

	/// <summary>The (row, column) pair clicked in a token cell.</summary>
	public sealed record MetadataCellNavigationEventArgs(object Row, string ColumnName);
}
