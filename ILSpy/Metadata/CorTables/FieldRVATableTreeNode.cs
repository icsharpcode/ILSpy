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

using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the FieldRVA table — for static fields backed by a fixed PE-image RVA, the
	/// table maps each FieldDef to its data location. Common with <c>fixed</c> array initializers
	/// (the compiler emits a synthesised type whose static fields point at the .rdata bytes).
	/// </summary>
	public sealed class FieldRVATableTreeNode : MetadataTableTreeNode<FieldRVATableTreeNode.FieldRVAEntry>
	{
		public FieldRVATableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.FieldRva, metadataFile)
		{
		}

		protected override IReadOnlyList<FieldRVAEntry> LoadTable()
		{
			var list = new List<FieldRVAEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.FieldRva);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.FieldRva);
			int fieldDefSize = metadata.GetTableRowCount(TableIndex.Field) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				int rva = reader.ReadInt32();
				int fieldRow = fieldDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new FieldRVAEntry(metadataFile, rid, rva, MetadataTokens.FieldDefinitionHandle(fieldRow)));
			}
			return list;
		}

		public sealed class FieldRVAEntry
		{
			// Verbatim fieldHandle — see FieldLayoutTableTreeNode for the C# 14 keyword note.
			readonly MetadataFile metadataFile;
			readonly FieldDefinitionHandle fieldHandle;

			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x1D000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Field => MetadataTokens.GetToken(fieldHandle);

			string? fieldTooltip;
			public string? FieldTooltip => GenerateTooltip(ref fieldTooltip, metadataFile, fieldHandle);

			[ColumnInfo("X8")]
			public int RVA { get; }

			public FieldRVAEntry(MetadataFile metadataFile, int rid, int rva, FieldDefinitionHandle field)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				RVA = rva;
				fieldHandle = field;
			}
		}
	}
}
