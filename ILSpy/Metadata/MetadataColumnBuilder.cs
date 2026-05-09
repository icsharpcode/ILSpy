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
using System.Globalization;
using System.Linq;
using System.Reflection;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

using ILSpy.ViewModels;

namespace ILSpy.Metadata
{
	/// <summary>
	/// Builds DataGrid columns for a metadata-row type by reflecting over its public
	/// properties. Reflection metadata is cached as <c>ColumnDescriptor</c>s; live
	/// <see cref="DataGridColumn"/> instances are constructed fresh on every call so each
	/// page can carry its own per-column filter inputs in the headers without sharing state
	/// with sibling pages.
	/// </summary>
	public static class MetadataColumnBuilder
	{
		static readonly ConcurrentDictionary<Type, ColumnDescriptor[]> descriptorCache = new();

		/// <summary>
		/// Convenience overload for tests and simple consumers — returns columns with plain
		/// string headers and no per-column filter wiring. Production code should call
		/// <see cref="Populate{TEntry}(MetadataTablePageModel)"/>, which also seeds the page's
		/// <c>ColumnFilters</c> collection and bakes filter inputs into each header.
		/// </summary>
		public static IReadOnlyList<DataGridColumn> For<TEntry>() => For(typeof(TEntry));

		public static IReadOnlyList<DataGridColumn> For(Type entryType)
		{
			ArgumentNullException.ThrowIfNull(entryType);
			return descriptorCache.GetOrAdd(entryType, BuildDescriptors)
				.Select(d => BuildColumn(d, filter: null))
				.ToArray();
		}

		/// <summary>
		/// Populates <paramref name="page"/> with both <c>Columns</c> and
		/// <c>ColumnFilters</c> in matched order. Each column's header is a vertical pair —
		/// the column name above a small TextBox bound two-way to the column's filter — so
		/// the user can filter every column independently and the predicate ANDs the lot.
		/// </summary>
		public static void Populate<TEntry>(MetadataTablePageModel page) => Populate(page, typeof(TEntry));

		public static void Populate(MetadataTablePageModel page, Type entryType)
		{
			ArgumentNullException.ThrowIfNull(page);
			ArgumentNullException.ThrowIfNull(entryType);
			var descriptors = descriptorCache.GetOrAdd(entryType, BuildDescriptors);
			page.ColumnFilters.Clear();
			var columns = new List<DataGridColumn>(descriptors.Length);
			foreach (var d in descriptors)
			{
				var filter = new ColumnFilter(d.Name);
				page.ColumnFilters.Add(filter);
				columns.Add(BuildColumn(d, filter));
			}
			page.Columns = columns;
		}

		static ColumnDescriptor[] BuildDescriptors(Type entryType)
		{
			var list = new List<ColumnDescriptor>();
			foreach (var prop in entryType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (prop.GetIndexParameters().Length > 0)
					continue;
				if (prop.Name == nameof(Entry.RowDetails))
					continue;
				if (prop.PropertyType == typeof(IList<BitEntry>))
					continue;
				list.Add(new ColumnDescriptor(prop, prop.GetCustomAttribute<ColumnInfoAttribute>()));
			}
			return list.ToArray();
		}

		static DataGridColumn BuildColumn(ColumnDescriptor d, ColumnFilter? filter)
		{
			var column = d.Info?.Kind == ColumnKind.Token
				? (DataGridColumn)BuildTokenColumn(d.Property, d.Info)
				: BuildTextColumn(d.Property, d.Info);
			column.Header = filter is null ? d.Name : (object)BuildHeader(d.Name, filter, d.Property.PropertyType);
			// Header is a Panel when filters are wired, so callers can't recover the column
			// name via Header.ToString(). Stash the name on Tag for hover-tooltip lookup,
			// "Go to token" context-menu resolution, and any future cell-level navigation.
			column.Tag = d.Name;
			return column;
		}

		static Control BuildHeader(string columnName, ColumnFilter filter, Type propertyType)
		{
			var label = new TextBlock {
				Text = columnName,
				FontWeight = FontWeight.SemiBold,
				HorizontalAlignment = HorizontalAlignment.Stretch,
			};
			var box = new TextBox {
				MinHeight = 0,
				Padding = new Thickness(2, 1),
				Margin = new Thickness(0, 2, 0, 0),
				FontWeight = FontWeight.Normal,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Text = filter.Text,
			};
			// Subscribe to the AvaloniaProperty change notification directly. GetObservable
			// also pushes the initial value at subscribe time, which can race with the
			// header's visual-tree-attach pass; AvaloniaObject.PropertyChanged only fires on
			// actual changes and reaches the same handler regardless of when the box joins
			// or leaves the visual tree.
			box.PropertyChanged += (_, e) => {
				if (e.Property == TextBox.TextProperty && filter.Text != box.Text)
					filter.Text = box.Text;
			};
			filter.PropertyChanged += (_, e) => {
				if (e.PropertyName == nameof(ColumnFilter.Text) && box.Text != filter.Text)
					box.Text = filter.Text;
			};

			Control inputRow = box;
			if (propertyType.IsEnum && Attribute.IsDefined(propertyType, typeof(FlagsAttribute)))
				inputRow = WrapWithFlagsDropdown(box, filter, propertyType);

			return new StackPanel {
				Orientation = Orientation.Vertical,
				Children = { label, inputRow },
			};
		}

		static Control WrapWithFlagsDropdown(TextBox box, ColumnFilter filter, Type enumType)
		{
			// Schema-driven dropdown: a FlagsFilterPopup that distinguishes mutex
			// sub-ranges (multi-select chips) from independent flags (tri-state pills),
			// driving ColumnFilter.FlagsState. The predicate path runs the compiled
			// CompiledFilter alongside the TextBox-driven substring / regex / numeric
			// path; both AND together.
			var schema = ILSpy.Metadata.Filters.FlagsSchemaInferer.For(enumType);
			filter.FlagsState ??= new ILSpy.Metadata.Filters.FilterState(schema);
			var popupContent = new ILSpy.Views.Filters.FlagsFilterPopup(filter.FlagsState);

			var openButton = new Button {
				Content = "▾",
				Padding = new Thickness(4, 0),
				Margin = new Thickness(2, 2, 0, 0),
				MinHeight = 0,
				VerticalAlignment = VerticalAlignment.Stretch,
			};
			var popupRoot = new Border {
				BorderBrush = Brushes.Gray,
				BorderThickness = new Thickness(1),
				Background = Brushes.White,
				// Force the arrow cursor on the popup surface — without this, the
				// EW-resize cursor set on the DataGrid column header's drag-grip can
				// leak into the popup if the pointer enters from there before
				// Avalonia recomputes the cursor for the new hit-test target.
				Cursor = new global::Avalonia.Input.Cursor(global::Avalonia.Input.StandardCursorType.Arrow),
				Child = new ScrollViewer {
					MaxHeight = 400,
					Content = popupContent,
				},
			};
			// Stop wheel events from bubbling out of the popup. Without this, scrolling
			// inside the dropdown also scrolls the underlying DataGrid because the
			// PointerWheelChanged event keeps bubbling up the routed-event tree once the
			// inner ScrollViewer has consumed (or ignored) it.
			popupRoot.AddHandler(global::Avalonia.Input.InputElement.PointerWheelChangedEvent,
				(_, e) => e.Handled = true,
				handledEventsToo: true);
			var popup = new Popup {
				PlacementTarget = openButton,
				Placement = PlacementMode.BottomEdgeAlignedLeft,
				IsLightDismissEnabled = true,
				Child = popupRoot,
			};
			openButton.Click += (_, _) => popup.IsOpen = true;
			var row = new DockPanel { LastChildFill = true, Margin = new Thickness(0, 2, 0, 0) };
			DockPanel.SetDock(openButton, global::Avalonia.Controls.Dock.Right);
			row.Children.Add(openButton);
			row.Children.Add(box);
			box.Margin = new Thickness(0);
			return new StackPanel {
				Orientation = Orientation.Vertical,
				Children = { row, popup },
			};
		}

		static DataGridTextColumn BuildTextColumn(PropertyInfo prop, ColumnInfoAttribute? info)
		{
			var binding = new Binding(prop.Name) { Mode = BindingMode.OneWay };
			if (!string.IsNullOrEmpty(info?.Format))
				binding.Converter = new HexFormatConverter(info.Format);
			return new DataGridTextColumn {
				IsReadOnly = true,
				Binding = binding,
				SortMemberPath = prop.Name,
			};
		}

		static DataGridTemplateColumn BuildTokenColumn(PropertyInfo prop, ColumnInfoAttribute? info)
		{
			var format = info?.Format;
			var template = new FuncDataTemplate<object>((row, _) => {
				var btn = new Button {
					Background = Brushes.Transparent,
					BorderThickness = default,
					Padding = new Thickness(2, 0),
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					Foreground = Brushes.Blue,
					Cursor = new global::Avalonia.Input.Cursor(global::Avalonia.Input.StandardCursorType.Hand),
					Content = FormatTokenValue(row, prop, format),
				};
				btn.Click += (_, _) => {
					if (btn.FindAncestorOfType<Views.MetadataTablePage>()?.DataContext is MetadataTablePageModel page)
						page.RaiseNavigateToCell(row, prop.Name);
				};
				return btn;
			}, supportsRecycling: true);
			return new DataGridTemplateColumn {
				IsReadOnly = true,
				CellTemplate = template,
				SortMemberPath = prop.Name,
			};
		}

		static string FormatTokenValue(object row, PropertyInfo prop, string? format)
		{
			var value = prop.GetValue(row);
			if (value is null)
				return string.Empty;
			if (value is Enum enumValue)
				value = System.Convert.ChangeType(enumValue, enumValue.GetTypeCode(), CultureInfo.InvariantCulture);
			if (!string.IsNullOrEmpty(format) && value is IFormattable formattable)
				return formattable.ToString(format, CultureInfo.InvariantCulture);
			return value.ToString() ?? string.Empty;
		}

		sealed record ColumnDescriptor(PropertyInfo Property, ColumnInfoAttribute? Info)
		{
			public string Name => Property.Name;
		}
	}

	/// <summary>
	/// Applies a numeric format spec ("X8", "X4", …) at bind-time. Wraps enum values to their
	/// underlying integer first so digit-bearing hex specs don't trip <see cref="Enum.ToString"/>'s
	/// strict format-character whitelist.
	/// </summary>
	internal sealed class HexFormatConverter : IValueConverter
	{
		readonly string format;

		public HexFormatConverter(string format)
		{
			this.format = format;
		}

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is null)
				return string.Empty;
			if (value is Enum enumValue)
				value = System.Convert.ChangeType(enumValue, enumValue.GetTypeCode(), CultureInfo.InvariantCulture);
			return value is IFormattable formattable
				? formattable.ToString(format, CultureInfo.InvariantCulture)
				: value.ToString();
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
