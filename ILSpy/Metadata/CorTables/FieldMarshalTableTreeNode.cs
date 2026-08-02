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
	/// View of the FieldMarshal table — native-marshalling descriptors attached to fields
	/// or parameters. Parent is a HasFieldMarshal coded token (Field / Param); NativeType
	/// is the heap-offset of a marshal-spec blob.
	/// </summary>
	public sealed class FieldMarshalTableTreeNode : MetadataTableTreeNode<FieldMarshalTableTreeNode.FieldMarshalEntry>
	{
		public FieldMarshalTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.FieldMarshal, metadataFile)
		{
		}

		protected override IReadOnlyList<FieldMarshalEntry> LoadTable()
		{
			var list = new List<FieldMarshalEntry>();
			int rid = 0;
			foreach (var (parent, nativeType) in metadataFile.Metadata.GetFieldMarshals())
			{
				list.Add(new FieldMarshalEntry(metadataFile, ++rid, parent, nativeType));
			}
			return list;
		}

		public sealed class FieldMarshalEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x0D000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, parent);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int NativeType => MetadataTokens.GetHeapOffset(nativeType);

			readonly MetadataFile metadataFile;
			readonly EntityHandle parent;
			readonly BlobHandle nativeType;

			public FieldMarshalEntry(MetadataFile metadataFile, int rid, EntityHandle parent, BlobHandle nativeType)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				this.parent = parent;
				this.nativeType = nativeType;
			}
		}
	}
}
