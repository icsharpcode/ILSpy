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
	/// View of the FieldLayout table — explicit per-field byte offsets for fields whose
	/// owning TypeDef has <see cref="System.Reflection.TypeAttributes.ExplicitLayout"/> set.
	/// </summary>
	public sealed class FieldLayoutTableTreeNode : MetadataTableTreeNode<FieldLayoutTableTreeNode.FieldLayoutEntry>
	{
		public FieldLayoutTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.FieldLayout, metadataFile)
		{
		}

		protected override IReadOnlyList<FieldLayoutEntry> LoadTable()
		{
			var list = new List<FieldLayoutEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.FieldLayout);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.FieldLayout);
			int fieldDefSize = metadata.GetTableRowCount(TableIndex.Field) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				int offset = reader.ReadInt32();
				int fieldRow = fieldDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new FieldLayoutEntry(metadataFile, rid, offset, MetadataTokens.FieldDefinitionHandle(fieldRow)));
			}
			return list;
		}

		public sealed class FieldLayoutEntry
		{
			// Use a verbatim fieldHandle local because C# 14 made `field` contextual inside property
			// accessors — referencing it without `@` would name the auto-property's hidden
			// backing field instead of this member.
			readonly MetadataFile metadataFile;
			readonly FieldDefinitionHandle fieldHandle;

			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x10000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Field => MetadataTokens.GetToken(fieldHandle);

			string? fieldTooltip;
			public string? FieldTooltip => GenerateTooltip(ref fieldTooltip, metadataFile, fieldHandle);

			[ColumnInfo("X8")]
			public int FieldOffset { get; }

			public FieldLayoutEntry(MetadataFile metadataFile, int rid, int fieldOffset, FieldDefinitionHandle field)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				FieldOffset = fieldOffset;
				fieldHandle = field;
			}
		}
	}
}
