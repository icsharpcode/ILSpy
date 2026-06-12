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
	/// View of the NestedClass table — pairs each nested type with its enclosing type. Walked
	/// directly from the table bytes because System.Reflection.Metadata only exposes
	/// nested-type discovery as a per-typedef enumeration.
	/// </summary>
	public sealed class NestedClassTableTreeNode : MetadataTableTreeNode<NestedClassTableTreeNode.NestedClassEntry>
	{
		public NestedClassTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.NestedClass, metadataFile)
		{
		}

		protected override IReadOnlyList<NestedClassEntry> LoadTable()
		{
			var list = new List<NestedClassEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.NestedClass);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.NestedClass);
			int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				int nestedRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int enclosingRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new NestedClassEntry(metadataFile, rid, MetadataTokens.TypeDefinitionHandle(nestedRow), MetadataTokens.TypeDefinitionHandle(enclosingRow)));
			}
			return list;
		}

		public sealed class NestedClassEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x29000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int NestedClass => MetadataTokens.GetToken(nested);

			string? nestedClassTooltip;
			public string? NestedClassTooltip => GenerateTooltip(ref nestedClassTooltip, metadataFile, nested);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int EnclosingClass => MetadataTokens.GetToken(enclosing);

			string? enclosingClassTooltip;
			public string? EnclosingClassTooltip => GenerateTooltip(ref enclosingClassTooltip, metadataFile, enclosing);

			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle nested;
			readonly TypeDefinitionHandle enclosing;

			public NestedClassEntry(MetadataFile metadataFile, int rid, TypeDefinitionHandle nested, TypeDefinitionHandle enclosing)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				this.nested = nested;
				this.enclosing = enclosing;
			}
		}
	}
}
