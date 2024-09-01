// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.ILSpy.Metadata
{
	internal class FieldMarshalTableTreeNode : MetadataTableTreeNode
	{
		public FieldMarshalTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.FieldMarshal, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<FieldMarshalEntry>();
			FieldMarshalEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.FieldMarshal);
			ReadOnlySpan<byte> ptr = metadata.AsReadOnlySpan();
			for (int rid = 1; rid <= length; rid++)
			{
				FieldMarshalEntry entry = new FieldMarshalEntry(metadataFile, ptr, rid);
				if (entry.RID == this.scrollTarget)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		readonly struct FieldMarshal
		{
			public readonly BlobHandle NativeType;
			public readonly EntityHandle Parent;

			public FieldMarshal(ReadOnlySpan<byte> ptr, int blobHeapSize, int hasFieldMarshalRefSize)
			{
				Parent = Helpers.FromHasFieldMarshalTag((uint)Helpers.GetValueLittleEndian(ptr, hasFieldMarshalRefSize));
				NativeType = MetadataTokens.BlobHandle(Helpers.GetValueLittleEndian(ptr.Slice(hasFieldMarshalRefSize, blobHeapSize)));
			}
		}

		struct FieldMarshalEntry
		{
			readonly MetadataFile metadataFile;
			readonly FieldMarshal fieldMarshal;

			public int RID { get; }

			public int Token => 0x0D000000 | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(fieldMarshal.Parent);

			public void OnParentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, fieldMarshal.Parent, protocol: "metadata")));
			}

			string parentTooltip;
			public string ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, fieldMarshal.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int NativeType => MetadataTokens.GetHeapOffset(fieldMarshal.NativeType);

			public FieldMarshalEntry(MetadataFile metadataFile, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.FieldMarshal)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.FieldMarshal) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				int hasFieldMarshalRefSize = metadataFile.Metadata.ComputeCodedTokenSize(32768, TableMask.Field | TableMask.Param);
				int blobHeapSize = metadataFile.Metadata.GetHeapSize(HeapIndex.Blob) < ushort.MaxValue ? 2 : 4;
				this.fieldMarshal = new FieldMarshal(ptr.Slice(rowOffset), blobHeapSize, hasFieldMarshalRefSize);
				this.parentTooltip = null;
			}
		}
	}
}
