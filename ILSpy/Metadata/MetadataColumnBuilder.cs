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
using System.Globalization;
using System.Linq;
using System.Reflection;

using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace ILSpy.Metadata
{
	/// <summary>
	/// Builds DataGrid column descriptors for a metadata-row type by reflecting over its public
	/// properties. Returns fresh <see cref="DataGridColumn"/> instances per call — Avalonia's
	/// DataGrid tracks an OwningGrid reference on each column, so reusing the same instances
	/// across tabs (or even across rebinds of one tab) throws InvalidOperationException
	/// "Column already belongs to a DataGrid instance and cannot be reassigned." Reflection
	/// cost per tab-open is negligible against the layout pass that follows.
	/// Token / heap-offset columns render as plain hex text in Phase 2; Phase 3 swaps them for
	/// hyperlink-styled buttons that emit a token-navigation request.
	/// </summary>
	public static class MetadataColumnBuilder
	{
		public static IReadOnlyList<DataGridColumn> For<TEntry>() => For(typeof(TEntry));

		public static IReadOnlyList<DataGridColumn> For(Type entryType)
		{
			ArgumentNullException.ThrowIfNull(entryType);
			return BuildColumns(entryType);
		}

		static IReadOnlyList<DataGridColumn> BuildColumns(Type entryType)
		{
			var columns = new List<DataGridColumn>();
			foreach (var prop in entryType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (prop.GetIndexParameters().Length > 0)
					continue;
				if (prop.Name == nameof(Entry.RowDetails))
					continue;
				if (prop.PropertyType == typeof(IList<BitEntry>))
					continue;

				var info = prop.GetCustomAttribute<ColumnInfoAttribute>();
				var binding = new Binding(prop.Name) { Mode = BindingMode.OneWay };

				if (!string.IsNullOrEmpty(info?.Format))
				{
					binding.Converter = new HexFormatConverter(info!.Format);
				}
				columns.Add(new DataGridTextColumn {
					Header = prop.Name,
					IsReadOnly = true,
					Binding = binding,
					SortMemberPath = prop.Name,
				});
			}
			return columns;
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
