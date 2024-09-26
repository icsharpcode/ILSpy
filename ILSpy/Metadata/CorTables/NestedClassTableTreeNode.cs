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
	class NestedClassTableTreeNode : MetadataTableTreeNode
	{
		public NestedClassTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.NestedClass, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<NestedClassEntry>();
			NestedClassEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.NestedClass);
			ReadOnlySpan<byte> ptr = metadata.AsReadOnlySpan();
			for (int rid = 1; rid <= length; rid++)
			{
				NestedClassEntry entry = new NestedClassEntry(metadataFile, ptr, rid);
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

		readonly struct NestedClass
		{
			public readonly TypeDefinitionHandle Nested;
			public readonly TypeDefinitionHandle Enclosing;

			public NestedClass(ReadOnlySpan<byte> ptr, int typeDefSize)
			{
				Nested = MetadataTokens.TypeDefinitionHandle(Helpers.GetValueLittleEndian(ptr, typeDefSize));
				Enclosing = MetadataTokens.TypeDefinitionHandle(Helpers.GetValueLittleEndian(ptr.Slice(typeDefSize), typeDefSize));
			}
		}

		struct NestedClassEntry
		{
			readonly MetadataFile metadataFile;
			readonly NestedClass nestedClass;

			public int RID { get; }

			public int Token => 0x29000000 | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int NestedClass => MetadataTokens.GetToken(nestedClass.Nested);

			public void OnNestedClassClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, nestedClass.Nested, protocol: "metadata")));
			}

			string nestedClassTooltip;
			public string NestedClassTooltip => GenerateTooltip(ref nestedClassTooltip, metadataFile, nestedClass.Nested);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int EnclosingClass => MetadataTokens.GetToken(nestedClass.Enclosing);

			public void OnEnclosingClassClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, nestedClass.Enclosing, protocol: "metadata")));
			}

			string enclosingClassTooltip;
			public string EnclosingClassTooltip => GenerateTooltip(ref enclosingClassTooltip, metadataFile, nestedClass.Enclosing);

			public NestedClassEntry(MetadataFile metadataFile, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.NestedClass)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.NestedClass) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				int typeDefSize = metadataFile.Metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
				this.nestedClass = new NestedClass(ptr.Slice(rowOffset), typeDefSize);
				this.nestedClassTooltip = null;
				this.enclosingClassTooltip = null;
			}
		}
	}
}
