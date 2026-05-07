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
		/// Free-text filter applied to the visible rows. Empty / null shows every row;
		/// otherwise the view shows the rows where any property's stringified value contains
		/// the filter (case-insensitive). Bound two-way to a TextBox above the grid.
		/// </summary>
		[ObservableProperty]
		private string? filterText;

		/// <summary>
		/// Raised when the user clicks a hyperlink-styled token cell. The host (the dock
		/// workspace) resolves the (row, columnName) pair to a metadata token and navigates
		/// to the target table row.
		/// </summary>
		public event Action<MetadataCellNavigationEventArgs>? NavigateToCellRequested;

		internal void RaiseNavigateToCell(object row, string columnName)
			=> NavigateToCellRequested?.Invoke(new MetadataCellNavigationEventArgs(row, columnName));

		static readonly ConcurrentDictionary<Type, PropertyInfo[]> filterPropertyCache = new();

		/// <summary>
		/// Predicate used by both the view's <c>DataGridCollectionView.Filter</c> and tests:
		/// returns <see langword="true"/> when <paramref name="filter"/> is empty or any
		/// public instance property's stringified value on <paramref name="item"/> contains
		/// the filter case-insensitively.
		/// </summary>
		public static bool MatchesFilter(object item, string? filter)
		{
			ArgumentNullException.ThrowIfNull(item);
			if (string.IsNullOrEmpty(filter))
				return true;
			var props = filterPropertyCache.GetOrAdd(item.GetType(),
				static t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));
			foreach (var prop in props)
			{
				object? value;
				try
				{ value = prop.GetValue(item); }
				catch { continue; }
				var s = value?.ToString();
				if (s is not null && s.Contains(filter, StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}
	}

	/// <summary>The (row, column) pair clicked in a token cell.</summary>
	public sealed record MetadataCellNavigationEventArgs(object Row, string ColumnName);
}
