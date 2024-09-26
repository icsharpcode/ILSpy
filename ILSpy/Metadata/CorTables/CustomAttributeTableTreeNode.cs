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

namespace ICSharpCode.ILSpy.Metadata
{
	class CustomAttributeTableTreeNode : MetadataTableTreeNode
	{
		public CustomAttributeTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.CustomAttribute, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<CustomAttributeEntry>();
			CustomAttributeEntry scrollTargetEntry = default;

			foreach (var row in metadata.CustomAttributes)
			{
				CustomAttributeEntry entry = new CustomAttributeEntry(metadataFile, row);
				if (scrollTarget == MetadataTokens.GetRowNumber(row))
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

		struct CustomAttributeEntry
		{
			readonly MetadataFile metadataFile;
			readonly CustomAttributeHandle handle;
			readonly CustomAttribute customAttr;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.CustomAttribute)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.CustomAttribute) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(customAttr.Parent);

			public void OnParentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, customAttr.Parent, protocol: "metadata")));
			}

			string parentTooltip;
			public string ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, customAttr.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Constructor => MetadataTokens.GetToken(customAttr.Constructor);

			public void OnConstructorClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, customAttr.Constructor, protocol: "metadata")));
			}

			string constructorTooltip;
			public string ConstructorTooltip => GenerateTooltip(ref constructorTooltip, metadataFile, customAttr.Constructor);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(customAttr.Value);

			public string ValueTooltip {
				get {
					return null;
				}
			}

			public CustomAttributeEntry(MetadataFile metadataFile, CustomAttributeHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.customAttr = metadataFile.Metadata.GetCustomAttribute(handle);
				this.parentTooltip = null;
				this.constructorTooltip = null;
			}
		}
	}
}
