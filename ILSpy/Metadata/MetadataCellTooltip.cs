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
using System.Reflection;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Resolves per-cell tooltips for a metadata-table row. Entry classes opt in by exposing
	/// a public <c>{ColumnName}Tooltip</c> property — <c>NameTooltip</c>, <c>FlagsTooltip</c>,
	/// <c>BaseTypeTooltip</c>, and so on. A <see cref="FlagsTooltip"/> value renders as the rich
	/// per-bit breakdown control; anything else is stringified, so callers can flow the result
	/// straight into <c>ToolTip.SetTip</c>.
	/// </summary>
	public static class MetadataCellTooltip
	{
		static readonly ConcurrentDictionary<(Type Type, string Column), PropertyInfo?> propertyCache = new();

		/// <summary>
		/// Looks up <paramref name="columnName"/><c>Tooltip</c> on <paramref name="item"/>'s
		/// runtime type. Returns a rich control for a <see cref="FlagsTooltip"/> value, the
		/// stringified value otherwise, or <see langword="null"/> if the item is null, the tooltip
		/// property is absent, or the value is null / blank.
		/// </summary>
		public static object? Resolve(object item, string columnName)
		{
			if (item is null)
				return null;
			var prop = propertyCache.GetOrAdd((item.GetType(), columnName),
				static key => key.Type.GetProperty(key.Column + "Tooltip",
					BindingFlags.Public | BindingFlags.Instance));
			if (prop is null)
				return null;
			object? value;
			try
			{ value = prop.GetValue(item); }
			catch { return null; }
			if (value is FlagsTooltip flags)
				return flags.Build();
			var s = value?.ToString();
			return string.IsNullOrWhiteSpace(s) ? null : s;
		}
	}
}
