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
	internal class TypeRefTableTreeNode : MetadataTableTreeNode
	{
		public TypeRefTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.TypeRef, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<TypeRefEntry>();
			TypeRefEntry scrollTargetEntry = default;

			foreach (var row in metadata.TypeReferences)
			{
				TypeRefEntry entry = new TypeRefEntry(metadataFile, row);
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

		struct TypeRefEntry
		{
			readonly MetadataFile metadataFile;
			readonly TypeReferenceHandle handle;
			readonly TypeReference typeRef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.TypeRef)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.TypeRef) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ResolutionScope => MetadataTokens.GetToken(typeRef.ResolutionScope);

			public void OnResolutionScopeClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, typeRef.ResolutionScope, protocol: "metadata")));
			}

			string resolutionScopeTooltip;
			public string ResolutionScopeTooltip => GenerateTooltip(ref resolutionScopeTooltip, metadataFile, typeRef.ResolutionScope);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(typeRef.Name):X} \"{Name}\"";

			public string Name => metadataFile.Metadata.GetString(typeRef.Name);

			public string NamespaceTooltip => $"{MetadataTokens.GetHeapOffset(typeRef.Namespace):X} \"{Namespace}\"";

			public string Namespace => metadataFile.Metadata.GetString(typeRef.Namespace);

			public TypeRefEntry(MetadataFile metadataFile, TypeReferenceHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.typeRef = metadataFile.Metadata.GetTypeReference(handle);
				this.resolutionScopeTooltip = null;
			}
		}
	}
}