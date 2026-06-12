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
	/// View of the InterfaceImpl table — every type/interface implementation pair. Class is
	/// a TypeDef row; Interface is a TypeDefOrRef coded token (TypeDef / TypeRef / TypeSpec).
	/// </summary>
	public sealed class InterfaceImplTableTreeNode : MetadataTableTreeNode<InterfaceImplTableTreeNode.InterfaceImplEntry>
	{
		public InterfaceImplTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.InterfaceImpl, metadataFile)
		{
		}

		protected override IReadOnlyList<InterfaceImplEntry> LoadTable()
		{
			var list = new List<InterfaceImplEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.InterfaceImpl);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.InterfaceImpl);
			int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
			int interfaceTagSize = metadata.ComputeCodedTokenSize(32768, TableMask.TypeDef | TableMask.TypeRef | TableMask.TypeSpec);
			for (int rid = 1; rid <= length; rid++)
			{
				int classRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				uint interfaceTag = (uint)(interfaceTagSize == 2 ? reader.ReadUInt16() : reader.ReadInt32());
				list.Add(new InterfaceImplEntry(metadataFile, rid,
					MetadataTokens.TypeDefinitionHandle(classRow),
					MetadataReaderHelpers.FromTypeDefOrRefTag(interfaceTag)));
			}
			return list;
		}

		public sealed class InterfaceImplEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x09000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Class => MetadataTokens.GetToken(@class);

			string? classTooltip;
			public string? ClassTooltip => GenerateTooltip(ref classTooltip, metadataFile, @class);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Interface => MetadataTokens.GetToken(@interface);

			string? interfaceTooltip;
			public string? InterfaceTooltip => GenerateTooltip(ref interfaceTooltip, metadataFile, @interface);

			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle @class;
			readonly EntityHandle @interface;

			public InterfaceImplEntry(MetadataFile metadataFile, int rid, TypeDefinitionHandle @class, EntityHandle @interface)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				this.@class = @class;
				this.@interface = @interface;
			}
		}
	}
}
