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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the five Ptr tables (FieldPtr / MethodPtr / ParamPtr / EventPtr / PropertyPtr).
	/// Present only in metadata produced with #~ + indirection (rare; mostly EnC or some
	/// custom emitters), and each row carries a single handle into the table the Ptr points
	/// at. One node type drives all five since the row shape is identical.
	/// </summary>
	public sealed class PtrTableTreeNode : MetadataTableTreeNode<PtrTableTreeNode.PtrEntry>
	{
		readonly TableIndex referencedTableKind;

		public PtrTableTreeNode(TableIndex kind, MetadataFile metadataFile)
			: base(kind, metadataFile)
		{
			referencedTableKind = kind switch {
				TableIndex.EventPtr => TableIndex.Event,
				TableIndex.FieldPtr => TableIndex.Field,
				TableIndex.MethodPtr => TableIndex.MethodDef,
				TableIndex.ParamPtr => TableIndex.Param,
				TableIndex.PropertyPtr => TableIndex.Property,
				_ => throw new ArgumentOutOfRangeException(nameof(kind), $"PtrTable does not support {kind}"),
			};
		}

		protected override IReadOnlyList<PtrEntry> LoadTable()
		{
			var list = new List<PtrEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(Kind);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(Kind);
			int handleSize = metadata.GetTableRowCount(referencedTableKind) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				int handleRow = handleSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new PtrEntry(metadataFile, rid, Kind, MetadataTokens.EntityHandle(((int)referencedTableKind << 24) | handleRow)));
			}
			return list;
		}

		public sealed class PtrEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => ((int)kind << 24) | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Handle => MetadataTokens.GetToken(handle);

			string? handleTooltip;
			public string? HandleTooltip => GenerateTooltip(ref handleTooltip, metadataFile, handle);

			readonly MetadataFile metadataFile;
			readonly TableIndex kind;
			readonly EntityHandle handle;

			public PtrEntry(MetadataFile metadataFile, int rid, TableIndex kind, EntityHandle handle)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				this.kind = kind;
				this.handle = handle;
			}
		}
	}
}
