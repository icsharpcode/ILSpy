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
	internal class ImportScopeTableTreeNode : DebugMetadataTableTreeNode
	{
		public ImportScopeTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ImportScope, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<ImportScopeEntry>();
			ImportScopeEntry scrollTargetEntry = default;

			foreach (var row in metadataFile.Metadata.ImportScopes)
			{
				ImportScopeEntry entry = new ImportScopeEntry(metadataFile, row);
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

		readonly struct ImportScopeEntry
		{
			readonly int? offset;
			readonly MetadataFile metadataFile;
			readonly ImportScopeHandle handle;
			readonly ImportScope localScope;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public object Offset => offset == null ? "n/a" : offset;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(localScope.Parent);

			public void OnParentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, localScope.Parent, protocol: "metadata")));
			}

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Imports => MetadataTokens.GetHeapOffset(localScope.ImportsBlob);

			public ImportScopeEntry(MetadataFile metadataFile, ImportScopeHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.localScope = metadataFile.Metadata.GetImportScope(handle);
				this.offset = metadataFile.IsEmbedded ? null : (int?)metadataFile.Metadata.GetTableMetadataOffset(TableIndex.ImportScope)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.ImportScope) * (MetadataTokens.GetRowNumber(handle) - 1);
			}
		}
	}
}