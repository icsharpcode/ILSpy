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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	class ClassLayoutTableTreeNode : MetadataTableTreeNode
	{
		public ClassLayoutTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ClassLayout, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			var list = new List<ClassLayoutEntry>();

			var length = metadataFile.Metadata.GetTableRowCount(TableIndex.ClassLayout);
			ReadOnlySpan<byte> ptr = metadataFile.Metadata.AsReadOnlySpan();
			ClassLayoutEntry scrollTargetEntry = default;

			for (int rid = 1; rid <= length; rid++)
			{
				ClassLayoutEntry entry = new ClassLayoutEntry(metadataFile, ptr, rid);
				if (scrollTarget == rid)
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

		readonly struct ClassLayout
		{
			public readonly ushort PackingSize;
			public readonly EntityHandle Parent;
			public readonly uint ClassSize;

			public ClassLayout(ReadOnlySpan<byte> ptr, int typeDefSize)
			{
				PackingSize = BinaryPrimitives.ReadUInt16LittleEndian(ptr);
				ClassSize = BinaryPrimitives.ReadUInt32LittleEndian(ptr.Slice(2, 4));
				Parent = MetadataTokens.TypeDefinitionHandle(Helpers.GetValueLittleEndian(ptr.Slice(6, typeDefSize)));
			}
		}

		struct ClassLayoutEntry
		{
			readonly MetadataFile metadataFile;
			readonly ClassLayout classLayout;

			public int RID { get; }

			public int Token => 0x0F000000 | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(classLayout.Parent);

			public void OnParentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference("metadata", classLayout.Parent)));
			}

			string parentTooltip;
			public string ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, classLayout.Parent);

			[ColumnInfo("X4", Kind = ColumnKind.Other)]
			public ushort PackingSize => classLayout.PackingSize;

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public uint ClassSize => classLayout.ClassSize;

			public ClassLayoutEntry(MetadataFile metadataFile, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var metadata = metadataFile.Metadata;
				var rowOffset = metadata.GetTableMetadataOffset(TableIndex.ClassLayout)
					+ metadata.GetTableRowSize(TableIndex.ClassLayout) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				this.classLayout = new ClassLayout(ptr.Slice(rowOffset), metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4);
				this.parentTooltip = null;
			}
		}
	}
}
