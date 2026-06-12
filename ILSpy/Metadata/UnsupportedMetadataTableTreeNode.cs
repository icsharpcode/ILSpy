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

using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Universal placeholder used until a typed leaf is wired up for a given
	/// <see cref="TableIndex"/>. Reports the table name and row count; gives the user
	/// somewhere to land while the per-table viewers are filled in incrementally. Phase 1e
	/// replaces this with one typed <see cref="MetadataTableTreeNode"/> subclass per table.
	/// </summary>
	public sealed class UnsupportedMetadataTableTreeNode : MetadataTableTreeNode
	{
		public UnsupportedMetadataTableTreeNode(TableIndex kind, MetadataFile metadataFile)
			: base(kind, metadataFile)
		{
		}

		public override object Text => $"{(byte)Kind:X2} {Kind} ({RowCount})";
		public override string ToString() => Kind.ToString();

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, $"{Kind}: {RowCount} rows");
			language.WriteCommentLine(output, "Per-row content for this table ships in a future Avalonia release.");
		}
	}
}
