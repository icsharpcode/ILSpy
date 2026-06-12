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
	/// View of the Constant table — compile-time constants attached to fields, parameters,
	/// or properties. The Type column is the primitive type code; Value points at a blob
	/// holding the literal payload.
	/// </summary>
	public sealed class ConstantTableTreeNode : MetadataTableTreeNode<ConstantTableTreeNode.ConstantEntry>
	{
		public ConstantTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Constant, metadataFile)
		{
		}

		protected override IReadOnlyList<ConstantEntry> LoadTable()
		{
			var list = new List<ConstantEntry>();
			for (int row = 1; row <= metadataFile.Metadata.GetTableRowCount(TableIndex.Constant); row++)
				list.Add(new ConstantEntry(metadataFile, MetadataTokens.ConstantHandle(row)));
			return list;
		}

		public sealed class ConstantEntry
		{
			readonly MetadataFile metadataFile;
			readonly ConstantHandle handle;
			readonly Constant constant;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.Constant, RID);

			[ColumnInfo("X8")]
			public ConstantTypeCode Type => constant.TypeCode;

			public string TypeTooltip => constant.TypeCode.ToString();

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(constant.Parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, constant.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(constant.Value);

			public string? ValueTooltip {
				get {
					return null;
				}
			}

			public ConstantEntry(MetadataFile metadataFile, ConstantHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				constant = metadataFile.Metadata.GetConstant(handle);
			}
		}
	}
}
