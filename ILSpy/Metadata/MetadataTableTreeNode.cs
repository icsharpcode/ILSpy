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
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ILSpy.Languages;
using ILSpy.TreeNodes;

namespace ILSpy.Metadata
{
	/// <summary>
	/// Common parent for the per-table leaves under "Tables". Each subclass corresponds to
	/// one CLI <see cref="TableIndex"/> and surfaces its rows. Phase 1 emits a fixed-width
	/// text dump via <see cref="ILSpyTreeNode.Decompile"/>; Phase 2 swaps to a DataGrid tab
	/// via the per-node <c>CreateTab</c> override added in that phase.
	/// </summary>
	public abstract class MetadataTableTreeNode : ILSpyTreeNode
	{
		protected readonly MetadataFile metadataFile;

		public TableIndex Kind { get; }

		public int RowCount => metadataFile.Metadata.GetTableRowCount(Kind);

		protected MetadataTableTreeNode(TableIndex kind, MetadataFile metadataFile)
		{
			Kind = kind;
			this.metadataFile = metadataFile ?? throw new ArgumentNullException(nameof(metadataFile));
		}

		public override object Icon => Images.Images.MetadataTable;
	}

	/// <summary>
	/// Typed companion: holds the row materialiser and the shared text-render path. Rows are
	/// loaded lazily and cached so repeated decompiles don't re-walk the metadata. Phase 2
	/// adds a <c>CreateTab</c> override that hands the same <c>LoadTable()</c> result to a
	/// DataGrid view; for now <see cref="ITextOutput"/> is the only renderer.
	/// </summary>
	public abstract class MetadataTableTreeNode<TEntry> : MetadataTableTreeNode
	{
		public const int PreviewLimit = 200;

		IReadOnlyList<TEntry>? cached;

		protected MetadataTableTreeNode(TableIndex kind, MetadataFile metadataFile)
			: base(kind, metadataFile)
		{
		}

		public override object Text => $"{(byte)Kind:X2} {Kind} ({RowCount})";
		public override string ToString() => Kind.ToString();

		protected abstract IReadOnlyList<TEntry> LoadTable();

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var rows = cached ??= LoadTable();
			var preview = rows.Count > PreviewLimit
				? (IReadOnlyList<TEntry>)rows.Take(PreviewLimit).ToList()
				: rows;
			language.WriteCommentLine(output, $"{Kind} ({rows.Count} rows)");
			MetadataTextWriter.WriteTable(language, output, preview);
			if (rows.Count > preview.Count)
				language.WriteCommentLine(output, $"... ({rows.Count - preview.Count} more rows; full view ships with the metadata DataGrid in a future release)");
		}
	}
}
