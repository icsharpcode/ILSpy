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
	/// View of the CustomAttribute table — every <c>[Attr(...)]</c> annotation attached to
	/// every metadata row. Parent is a HasCustomAttribute coded token; Constructor points at
	/// the attribute class's ctor (MethodDef or MemberRef); Value blob holds the constructor
	/// arguments + named properties as a serialised payload.
	/// </summary>
	public sealed class CustomAttributeTableTreeNode : MetadataTableTreeNode<CustomAttributeTableTreeNode.CustomAttributeEntry>
	{
		public CustomAttributeTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.CustomAttribute, metadataFile)
		{
		}

		protected override IReadOnlyList<CustomAttributeEntry> LoadTable()
		{
			var list = new List<CustomAttributeEntry>();
			foreach (var row in metadataFile.Metadata.CustomAttributes)
				list.Add(new CustomAttributeEntry(metadataFile, row));
			return list;
		}

		public sealed class CustomAttributeEntry
		{
			readonly MetadataFile metadataFile;
			readonly CustomAttributeHandle handle;
			readonly CustomAttribute customAttr;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.CustomAttribute, RID);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(customAttr.Parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, customAttr.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Constructor => MetadataTokens.GetToken(customAttr.Constructor);

			string? constructorTooltip;
			public string? ConstructorTooltip => GenerateTooltip(ref constructorTooltip, metadataFile, customAttr.Constructor);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(customAttr.Value);

			public string? ValueTooltip {
				get {
					return null;
				}
			}

			public CustomAttributeEntry(MetadataFile metadataFile, CustomAttributeHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				customAttr = metadataFile.Metadata.GetCustomAttribute(handle);
			}
		}
	}
}
