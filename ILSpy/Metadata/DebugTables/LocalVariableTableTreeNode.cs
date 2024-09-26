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
	internal class LocalVariableTableTreeNode : DebugMetadataTableTreeNode
	{
		public LocalVariableTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.LocalVariable, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<LocalVariableEntry>();
			LocalVariableEntry scrollTargetEntry = default;

			foreach (var row in metadataFile.Metadata.LocalVariables)
			{
				LocalVariableEntry entry = new LocalVariableEntry(metadataFile, row);
				if (entry.RID == scrollTarget)
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

		struct LocalVariableEntry
		{
			readonly int? offset;
			readonly MetadataFile metadataFile;
			readonly LocalVariableHandle handle;
			readonly LocalVariable localVar;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public LocalVariableAttributes Attributes => localVar.Attributes;

			public object AttributesTooltip => new FlagsTooltip() {
				FlagGroup.CreateMultipleChoiceGroup(typeof(LocalVariableAttributes)),
			};

			public int Index => localVar.Index;

			public string Name => metadataFile.Metadata.GetString(localVar.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(localVar.Name):X} \"{Name}\"";

			public LocalVariableEntry(MetadataFile metadataFile, LocalVariableHandle handle)
			{
				this.metadataFile = metadataFile;
				this.offset = metadataFile.IsEmbedded ? null : (int?)metadataFile.Metadata.GetTableMetadataOffset(TableIndex.LocalVariable)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.LocalVariable) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.handle = handle;
				this.localVar = metadataFile.Metadata.GetLocalVariable(handle);
			}
		}
	}
}