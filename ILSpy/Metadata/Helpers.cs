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

namespace ILSpy.Metadata
{
	/// <summary>
	/// Classifies a column for the DataGrid view (Phase 2+). Heap-offset cells render with a
	/// hex-padded width and resolve their string content via the relevant heap reader; token
	/// cells render as a hyperlink that navigates to the target table row. The text path
	/// (Phase 1) ignores everything except <c>Format</c>.
	/// </summary>
	public enum ColumnKind
	{
		Other,
		HeapOffset,
		Token,
	}

	/// <summary>
	/// Annotates a row-shape property with rendering hints. <c>Format</c> is consumed by the
	/// text writer; <c>Kind</c> and <c>LinkToTable</c> drive the DataGrid column factory in
	/// Phase 2+ and are inert in Phase 1.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ColumnInfoAttribute : Attribute
	{
		public string Format { get; }
		public ColumnKind Kind { get; set; }
		public bool LinkToTable { get; set; }

		public ColumnInfoAttribute(string format)
		{
			Format = format;
		}
	}

	/// <summary>
	/// One row in a PE-header / metadata-table dump. <c>Value</c> is rendered hex-formatted
	/// at <c>Size * 2</c> digits when numeric; <c>RowDetails</c> carries optional flag-bit
	/// breakdowns that the DataGrid view (Phase 2) uses to populate row details. The text
	/// path (Phase 1) ignores <c>RowDetails</c>.
	/// </summary>
	public sealed class Entry
	{
		public string Member { get; }
		public int Offset { get; }
		public int Size { get; }
		public object? Value { get; }
		public string Meaning { get; }
		public IList<BitEntry>? RowDetails { get; }

		public Entry(int offset, object? value, int size, string member, string meaning, IList<BitEntry>? rowDetails = null)
		{
			Member = member;
			Offset = offset;
			Size = size;
			Value = value;
			Meaning = meaning;
			RowDetails = rowDetails;
		}
	}

	/// <summary>One bit (or bit-group) entry inside a flags-Entry's row-details strip.</summary>
	public sealed class BitEntry
	{
		public bool Value { get; }
		public string Meaning { get; }

		public BitEntry(bool value, string meaning)
		{
			Value = value;
			Meaning = meaning;
		}
	}
}
