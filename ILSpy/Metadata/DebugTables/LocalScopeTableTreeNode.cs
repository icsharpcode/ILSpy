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
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class LocalScopeTableTreeNode : DebugMetadataTableTreeNode
	{
		public LocalScopeTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.LocalScope, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<LocalScopeEntry>();
			LocalScopeEntry scrollTargetEntry = default;

			foreach (var row in metadataFile.Metadata.LocalScopes)
			{
				LocalScopeEntry entry = new LocalScopeEntry(metadataFile, row);
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

		struct LocalScopeEntry
		{
			readonly int? offset;
			readonly MetadataFile metadataFile;
			readonly LocalScopeHandle handle;
			readonly LocalScope localScope;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Method => MetadataTokens.GetToken(localScope.Method);

			public void OnMethodClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, localScope.Method, protocol: "metadata")));
			}

			string methodTooltip;
			public string MethodTooltip => GenerateTooltip(ref methodTooltip, metadataFile, localScope.Method);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ImportScope => MetadataTokens.GetToken(localScope.ImportScope);

			public void OnImportScopeClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, localScope.ImportScope, protocol: "metadata")));
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int VariableList => MetadataTokens.GetToken(localScope.GetLocalVariables().FirstOrDefault());

			public void OnVariableListClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, localScope.GetLocalVariables().FirstOrDefault(), protocol: "metadata")));
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ConstantList => MetadataTokens.GetToken(localScope.GetLocalConstants().FirstOrDefault());

			public void OnConstantListClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, localScope.GetLocalConstants().FirstOrDefault(), protocol: "metadata")));
			}

			public int StartOffset => localScope.StartOffset;

			public int Length => localScope.Length;

			public LocalScopeEntry(MetadataFile metadataFile, LocalScopeHandle handle)
			{
				this.metadataFile = metadataFile;
				this.offset = metadataFile.IsEmbedded ? null : (int?)metadataFile.Metadata.GetTableMetadataOffset(TableIndex.LocalScope)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.LocalScope) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.handle = handle;
				this.localScope = metadataFile.Metadata.GetLocalScope(handle);
				this.methodTooltip = null;
			}
		}
	}
}