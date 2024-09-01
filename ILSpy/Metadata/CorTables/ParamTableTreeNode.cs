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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ParamTableTreeNode : MetadataTableTreeNode
	{
		public ParamTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Param, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			var list = new List<ParamEntry>();
			ParamEntry scrollTargetEntry = default;

			for (int row = 1; row <= metadataFile.Metadata.GetTableRowCount(TableIndex.Param); row++)
			{
				ParamEntry entry = new ParamEntry(metadataFile, MetadataTokens.ParameterHandle(row));
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

		struct ParamEntry
		{
			readonly MetadataFile metadataFile;
			readonly ParameterHandle handle;
			readonly Parameter param;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.Param)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.Param) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public ParameterAttributes Attributes => param.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(ParameterAttributes), selectedValue: (int)param.Attributes, includeAll: false)
			};

			public string Name => metadataFile.Metadata.GetString(param.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(param.Name):X} \"{Name}\"";

			public int Sequence => param.SequenceNumber;

			public ParamEntry(MetadataFile metadataFile, ParameterHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.param = metadataFile.Metadata.GetParameter(handle);
			}
		}
	}
}