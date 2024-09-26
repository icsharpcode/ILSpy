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

using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ConstantTableTreeNode : MetadataTableTreeNode
	{
		public ConstantTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Constant, metadataFile)
		{
		}

		public override bool View(TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<ConstantEntry>();
			ConstantEntry scrollTargetEntry = default;

			for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.Constant); row++)
			{
				ConstantEntry entry = new ConstantEntry(metadataFile, MetadataTokens.ConstantHandle(row));
				if (scrollTarget == row)
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

		struct ConstantEntry
		{
			readonly MetadataFile metadataFile;
			readonly EntityHandle handle;
			readonly Constant constant;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.Constant)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.Constant) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public ConstantTypeCode Type => constant.TypeCode;

			public string TypeTooltip => constant.TypeCode.ToString();

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(constant.Parent);

			public void OnParentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, constant.Parent, protocol: "metadata")));
			}

			string parentTooltip;
			public string ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, constant.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(constant.Value);

			public string ValueTooltip {
				get {
					return null;
				}
			}

			public ConstantEntry(MetadataFile metadataFile, ConstantHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.constant = metadataFile.Metadata.GetConstant(handle);
				this.parentTooltip = null;
			}
		}
	}
}
