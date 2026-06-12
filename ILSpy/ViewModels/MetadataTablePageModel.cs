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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.Metadata;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Tab content for a metadata-table view: an item list bound to a DataGrid plus the
	/// column descriptors the view rebinds on every <c>DataContext</c> change. The DataGrid's
	/// <c>Columns</c> collection isn't an Avalonia property, so the model can't bind it
	/// declaratively — the view assigns it imperatively from <see cref="Columns"/>.
	/// </summary>
	public sealed partial class MetadataTablePageModel : ContentPageModel
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
		/// Optional template for the expandable details area beneath each row (flag-bit
		/// breakdowns, decoded blob content). The view copies it onto the DataGrid whenever
		/// the schema rebinds; <see langword="null"/> leaves the table without row details.
		/// </summary>
		public IDataTemplate? RowDetailsTemplate { get; set; }

		/// <summary>
		/// When the details area shows: <c>Collapsed</c> tables expand rows individually via
		/// <see cref="IsRowDetailsVisible"/>; <c>VisibleWhenSelected</c> previews the selected
		/// row's details as the user moves through the table.
		/// </summary>
		public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode { get; set; }
			= DataGridRowDetailsVisibilityMode.Collapsed;

		/// <summary>
		/// Per-row initial details visibility, applied by the view each time a row container
		/// materialises (rows are recycled with their visibility reset). Only consulted when
		/// non-null; rows without details should yield <see langword="false"/>.
		/// </summary>
		public Func<object, bool>? IsRowDetailsVisible { get; set; }

		/// <summary>
		/// One filter input per column, in the same order as <see cref="Columns"/>. The view
		/// renders each <see cref="ColumnFilter.Text"/> as a TextBox baked into that column's
		/// header; the filter predicate ANDs every non-empty entry, requiring each row's
		/// matching property's stringified value to contain its filter (case-insensitive).
		/// </summary>
		public ObservableCollection<ColumnFilter> ColumnFilters { get; } = new();

		/// <summary>
		/// Raised whenever any column's filter text changes. The view subscribes here to
		/// refresh its <c>DataGridCollectionView</c>; tests use it as the canonical filter
		/// signal too. The page wires this internally via <c>ColumnFilters.CollectionChanged</c>
		/// + each filter's <see cref="INotifyPropertyChanged"/> so callers don't have to chase
		/// individual filter instances.
		/// </summary>
		public event Action? ColumnFilterChanged;

		public MetadataTablePageModel()
		{
			// Metadata grids render PE-header / table fields straight from the metadata —
			// they're language-agnostic, so the toolbar's Language / Language-Version pickers
			// don't affect what's shown. Mirror WPF's per-tab SupportsLanguageSwitching=false
			// on every metadata tree node.
			SupportsLanguageSwitching = false;
			ColumnFilters.CollectionChanged += OnColumnFiltersCollectionChanged;
		}

		void OnColumnFiltersCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems is not null)
				foreach (ColumnFilter f in e.OldItems)
					f.PropertyChanged -= OnColumnFilterTextChanged;
			if (e.NewItems is not null)
				foreach (ColumnFilter f in e.NewItems)
					f.PropertyChanged += OnColumnFilterTextChanged;
		}

		void OnColumnFilterTextChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(ColumnFilter.Text) or nameof(ColumnFilter.FlagsState))
				ColumnFilterChanged?.Invoke();
		}

		/// <summary>
		/// Raised when the user clicks a hyperlink-styled token cell. The host (the dock
		/// workspace) resolves the (row, columnName) pair to a metadata token and navigates
		/// to the target table row.
		/// </summary>
		public event Action<MetadataCellNavigationEventArgs>? NavigateToCellRequested;

		internal void RaiseNavigateToCell(object row, string columnName)
			=> NavigateToCellRequested?.Invoke(new MetadataCellNavigationEventArgs(row, columnName));

		/// <summary>
		/// Raised when the user double-clicks (or otherwise activates) a metadata grid
		/// row. The dock workspace resolves the row's <c>metadataFile</c> + <c>Token</c>
		/// to an <see cref="ICSharpCode.Decompiler.TypeSystem.IEntity"/>; when
		/// <see cref="MetadataRowActivationEventArgs.OpenInNewTab"/> is true the entity
		/// is decompiled into a fresh tab, otherwise it replaces the active tab via the
		/// regular tree-selection path.
		/// </summary>
		public event Action<MetadataRowActivationEventArgs>? RowActivated;

		internal void RaiseRowActivated(object row, bool openInNewTab = false)
			=> RowActivated?.Invoke(new MetadataRowActivationEventArgs(row, openInNewTab));

		static readonly ConcurrentDictionary<(Type Type, string Column), PropertyInfo?> propertyLookupCache = new();

		static readonly ConcurrentDictionary<string, Regex?> regexCache = new();

		/// <summary>
		/// Returns true when every <see cref="ColumnFilter"/> in <paramref name="filters"/>
		/// either has an empty value or the matching property on <paramref name="item"/>
		/// satisfies the filter. The predicate is type-aware:
		/// <list type="bullet">
		///   <item>regex when the filter is wrapped in <c>/.../</c></item>
		///   <item><see cref="FlagsAttribute"/> enums: comma-separated flag names with
		///     <c>!</c> prefix for negation; all named flags must match (AND)</item>
		///   <item>integer columns: optional comparison operator (<c>&gt;</c>, <c>&lt;</c>,
		///     <c>&gt;=</c>, <c>&lt;=</c>, <c>=</c>) followed by a hex (<c>0x…</c>) or
		///     decimal literal</item>
		///   <item>otherwise: case-insensitive substring on the stringified value</item>
		/// </list>
		/// Type-aware modes silently fall through to the substring path on a parse failure
		/// so a typo can't take down the filter.
		/// </summary>
		public static bool MatchesFilters(object item, IEnumerable<ColumnFilter> filters)
		{
			ArgumentNullException.ThrowIfNull(item);
			ArgumentNullException.ThrowIfNull(filters);
			foreach (var f in filters)
			{
				bool textActive = !string.IsNullOrEmpty(f.Text);
				bool flagsActive = f.FlagsState is { IsEmpty: false };
				if (!textActive && !flagsActive)
					continue;
				var prop = propertyLookupCache.GetOrAdd((item.GetType(), f.ColumnName),
					static key => key.Type.GetProperty(key.Column,
						BindingFlags.Public | BindingFlags.Instance));
				if (prop is null)
					return false;
				if (!MatchesOne(item, prop, f))
					return false;
			}
			return true;
		}

		static bool MatchesOne(object item, PropertyInfo prop, ColumnFilter filter)
		{
			object? value;
			try
			{ value = prop.GetValue(item); }
			catch { return false; }
			if (value is null)
				return false;

			// [Flags] column — apply the schema-driven CompiledFilter when the user has
			// poked at the dropdown popup. The compiled filter encodes mutex sub-ranges
			// (multi-select with bitwise AND on the masked bits) plus required / excluded
			// independent flags, mirroring WPF's FlagsFilterControl + FlagsContentFilter.
			if (prop.PropertyType.IsEnum && Attribute.IsDefined(prop.PropertyType, typeof(FlagsAttribute)))
			{
				if (filter.FlagsState is { IsEmpty: false } flagsState)
				{
					uint actual = Convert.ToUInt32(
						Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture)
							& 0xFFFFFFFFL,
						System.Globalization.CultureInfo.InvariantCulture);
					if (!ICSharpCode.ILSpy.Metadata.Filters.CompiledFilter.Compile(flagsState).Matches(actual))
						return false;
				}
				// Fall through so a free-form substring filter on a [Flags] column still
				// works alongside the dropdown.
			}

			if (string.IsNullOrEmpty(filter.Text))
				return true;

			if (TryGetRegex(filter.Text) is { } rx)
				return rx.IsMatch(value.ToString() ?? string.Empty);

			if ((IsIntegerType(prop.PropertyType) || prop.PropertyType.IsEnum)
				&& TryMatchNumeric(value, filter.Text, out var numericResult))
				return numericResult;

			// Substring matches the formatted display value so a typed "06000010" hits a
			// "06000010" cell — the predicate sees what the user sees, not the raw int.
			var info = prop.GetCustomAttribute<ColumnInfoAttribute>();
			var stringified = FormatForDisplay(value, info?.Format);
			return stringified.Contains(filter.Text, StringComparison.OrdinalIgnoreCase);
		}

		static string FormatForDisplay(object value, string? format)
		{
			if (string.IsNullOrEmpty(format))
				return value.ToString() ?? string.Empty;
			object boxed = value is Enum e
				? Convert.ChangeType(e, e.GetTypeCode(), System.Globalization.CultureInfo.InvariantCulture)
				: value;
			return boxed is IFormattable f
				? f.ToString(format, System.Globalization.CultureInfo.InvariantCulture)
				: (value.ToString() ?? string.Empty);
		}

		static bool TryMatchNumeric(object value, string filter, out bool result)
		{
			result = false;
			var (op, operandText) = SplitComparison(filter);
			if (op is null)
				return false;
			if (!TryParseInt64(operandText, out var operand))
				return false;
			long actual = value is Enum e
				? Convert.ToInt64(e, System.Globalization.CultureInfo.InvariantCulture)
				: Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture);
			result = op switch {
				"=" => actual == operand,
				"<" => actual < operand,
				">" => actual > operand,
				"<=" => actual <= operand,
				">=" => actual >= operand,
				_ => false,
			};
			return true;
		}

		static (string? Op, string Operand) SplitComparison(string filter)
		{
			// Numeric mode requires an explicit comparison operator. A bare hex / decimal
			// literal stays on the substring path so "06000010" still matches the formatted
			// "06000010" the column displays.
			if (filter.StartsWith(">=", StringComparison.Ordinal))
				return (">=", filter[2..].Trim());
			if (filter.StartsWith("<=", StringComparison.Ordinal))
				return ("<=", filter[2..].Trim());
			if (filter.Length > 0 && filter[0] is '>' or '<' or '=')
				return (filter[..1], filter[1..].Trim());
			return (null, filter);
		}

		static bool TryParseInt64(string text, out long value)
		{
			if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
				return long.TryParse(text.AsSpan(2), System.Globalization.NumberStyles.HexNumber,
					System.Globalization.CultureInfo.InvariantCulture, out value);
			return long.TryParse(text, System.Globalization.NumberStyles.Integer,
				System.Globalization.CultureInfo.InvariantCulture, out value);
		}

		static bool IsIntegerType(Type type) => Type.GetTypeCode(type) is
			TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16
			or TypeCode.Int32 or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;

		static Regex? TryGetRegex(string filter)
		{
			// `/pattern/` opts a filter into regex mode (case-insensitive). Anything else,
			// or an unparseable pattern, leaves the predicate on the substring path.
			if (filter.Length < 2 || filter[0] != '/' || filter[^1] != '/')
				return null;
			return regexCache.GetOrAdd(filter, static text => {
				try
				{ return new Regex(text[1..^1], RegexOptions.IgnoreCase | RegexOptions.CultureInvariant); }
				catch { return null; }
			});
		}
	}

	/// <summary>One column's filter entry — the column name plus its current filter state.</summary>
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

		/// <summary>
		/// For <see cref="FlagsAttribute"/> enum columns: schema-driven mutex / required /
		/// excluded state set by the dropdown popup. <c>null</c> means "no flag filter";
		/// any non-empty FilterState compiles into a CompiledFilter and runs alongside
		/// the text predicate.
		/// </summary>
		[ObservableProperty]
		private ICSharpCode.ILSpy.Metadata.Filters.FilterState? flagsState;

		// Forward inner FilterState mutations as our own PropertyChanged so the page's
		// CollectionChanged-driven subscription (which only listens for ColumnFilter
		// property changes) wakes up and refreshes the DataGridCollectionView.
		partial void OnFlagsStateChanged(
			ICSharpCode.ILSpy.Metadata.Filters.FilterState? oldValue,
			ICSharpCode.ILSpy.Metadata.Filters.FilterState? newValue)
		{
			if (oldValue is not null)
				oldValue.PropertyChanged -= OnFlagsStateInnerChanged;
			if (newValue is not null)
				newValue.PropertyChanged += OnFlagsStateInnerChanged;
		}

		void OnFlagsStateInnerChanged(object? sender, PropertyChangedEventArgs e)
			=> OnPropertyChanged(nameof(FlagsState));
	}

	/// <summary>The (row, column) pair clicked in a token cell.</summary>
	public sealed record MetadataCellNavigationEventArgs(object Row, string ColumnName);

	/// <summary>The activated row plus whether the activation was a new-tab gesture
	/// (Shift+double-click or "Decompile to new tab" context-menu entry).</summary>
	public sealed record MetadataRowActivationEventArgs(object Row, bool OpenInNewTab);
}
