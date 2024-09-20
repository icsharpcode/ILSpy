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
	class ManifestResourceTableTreeNode : MetadataTableTreeNode
	{
		public ManifestResourceTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ManifestResource, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<ManifestResourceEntry>();
			ManifestResourceEntry scrollTargetEntry = default;

			foreach (var row in metadata.ManifestResources)
			{
				ManifestResourceEntry entry = new ManifestResourceEntry(metadataFile, row);
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

		struct ManifestResourceEntry
		{
			readonly MetadataFile metadataFile;
			readonly ManifestResourceHandle handle;
			readonly ManifestResource manifestResource;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.ManifestResource)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.ManifestResource) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public ManifestResourceAttributes Attributes => manifestResource.Attributes;

			public object AttributesTooltip => manifestResource.Attributes.ToString();

			public string Name => metadataFile.Metadata.GetString(manifestResource.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(manifestResource.Name):X} \"{Name}\"";

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Implementation => MetadataTokens.GetToken(manifestResource.Implementation);

			public void OnImplementationClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, manifestResource.Implementation, protocol: "metadata")));
			}

			string implementationTooltip;
			public string ImplementationTooltip => GenerateTooltip(ref implementationTooltip, metadataFile, manifestResource.Implementation);

			public ManifestResourceEntry(MetadataFile metadataFile, ManifestResourceHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.manifestResource = metadataFile.Metadata.GetManifestResource(handle);
				this.implementationTooltip = null;
			}
		}
	}
}
