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
using System.Text;

using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Renders a list of row objects to <see cref="ITextOutput"/> as a fixed-width table —
	/// the Phase 1 stand-in for the DataGrid view. Column names come from
	/// <see cref="PropertyInfo.Name"/>; cell formatting respects
	/// <see cref="ColumnInfoAttribute.Format"/> and, for <see cref="Entry"/> rows, the
	/// per-row hex-width implied by <see cref="Entry.Size"/>. Each emitted line goes through
	/// <see cref="Language.WriteCommentLine"/> so the output sits cleanly inside the
	/// existing comment-prefixed text pipeline.
	/// </summary>
	public static class MetadataTextWriter
	{
		public static void WriteTable<TRow>(Language language, ITextOutput output, IReadOnlyList<TRow> rows)
		{
			ArgumentNullException.ThrowIfNull(language);
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(rows);

			var props = typeof(TRow).GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.GetIndexParameters().Length == 0
					&& p.PropertyType != typeof(IList<BitEntry>)
					&& p.Name != nameof(Entry.RowDetails))
				.ToArray();
			if (props.Length == 0)
				return;

			var formats = props.Select(p => p.GetCustomAttribute<ColumnInfoAttribute>()?.Format).ToArray();

			// Materialise every cell as a string, so column widths can be computed in one pass.
			var cells = new string[rows.Count][];
			for (int r = 0; r < rows.Count; r++)
			{
				cells[r] = new string[props.Length];
				for (int c = 0; c < props.Length; c++)
					cells[r][c] = FormatCell(rows[r], props[c], formats[c]);
			}

			var widths = new int[props.Length];
			for (int c = 0; c < props.Length; c++)
			{
				widths[c] = props[c].Name.Length;
				for (int r = 0; r < rows.Count; r++)
					widths[c] = Math.Max(widths[c], cells[r][c].Length);
			}

			language.WriteCommentLine(output, BuildLine(widths, props.Select(p => p.Name).ToArray()));
			language.WriteCommentLine(output, BuildLine(widths, widths.Select(w => new string('-', w)).ToArray()));
			for (int r = 0; r < rows.Count; r++)
				language.WriteCommentLine(output, BuildLine(widths, cells[r]));
		}

		static string BuildLine(int[] widths, string[] cells)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < cells.Length; i++)
			{
				if (i > 0)
					sb.Append("  ");
				sb.Append(cells[i].PadRight(widths[i]));
			}
			return sb.ToString().TrimEnd();
		}

		static string FormatCell(object? row, PropertyInfo prop, string? format)
		{
			var value = prop.GetValue(row);
			if (value is null)
				return "";

			// Entry-shaped rows get special handling: Offset → 8-digit hex; Value → hex padded
			// to (Size * 2) digits. Phase 2 will hoist this onto a converter; Phase 1 keeps it
			// inline because the only consumer is this writer.
			if (row is Entry entry)
			{
				if (prop.Name == nameof(Entry.Offset))
					return ((int)value).ToString("X8", CultureInfo.InvariantCulture);
				if (prop.Name == nameof(Entry.Value) && entry.Size > 0)
					return FormatHex(value, entry.Size * 2);
			}

			if (!string.IsNullOrEmpty(format))
			{
				// Enum.ToString rejects digit-bearing format strings ("X8" etc.); coerce to the
				// underlying integer first so the column lines up with neighbouring hex cells.
				if (value is Enum enumValue)
					value = Convert.ChangeType(enumValue, enumValue.GetTypeCode(), CultureInfo.InvariantCulture)!;
				if (value is IFormattable formattable)
					return formattable.ToString(format, CultureInfo.InvariantCulture);
			}

			return value.ToString() ?? "";
		}

		static string FormatHex(object value, int width)
		{
			string spec = "X" + width.ToString(CultureInfo.InvariantCulture);
			return value switch {
				ulong u => u.ToString(spec, CultureInfo.InvariantCulture),
				long l => l.ToString(spec, CultureInfo.InvariantCulture),
				uint ui => ui.ToString(spec, CultureInfo.InvariantCulture),
				int i => i.ToString(spec, CultureInfo.InvariantCulture),
				ushort us => us.ToString(spec, CultureInfo.InvariantCulture),
				short s => s.ToString(spec, CultureInfo.InvariantCulture),
				byte b => b.ToString(spec, CultureInfo.InvariantCulture),
				sbyte sb => sb.ToString(spec, CultureInfo.InvariantCulture),
				_ => value.ToString() ?? "",
			};
		}
	}
}
