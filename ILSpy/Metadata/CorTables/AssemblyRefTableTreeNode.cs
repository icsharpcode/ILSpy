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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class AssemblyRefTableTreeNode : MetadataTableTreeNode
	{
		public AssemblyRefTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.AssemblyRef, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<AssemblyRefEntry>();
			AssemblyRefEntry scrollTargetEntry = default;

			foreach (var row in metadataFile.Metadata.AssemblyReferences)
			{
				AssemblyRefEntry entry = new AssemblyRefEntry(metadataFile, row);
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

		struct AssemblyRefEntry
		{
			readonly MetadataFile metadataFile;
			readonly AssemblyReferenceHandle handle;
			readonly System.Reflection.Metadata.AssemblyReference assemblyRef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.AssemblyRef)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.AssemblyRef) * (RID - 1);

			public Version Version => assemblyRef.Version;

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public AssemblyFlags Flags => assemblyRef.Flags;

			public object FlagsTooltip => new FlagsTooltip((int)assemblyRef.Flags, null) {
				FlagGroup.CreateMultipleChoiceGroup(typeof(AssemblyFlags), selectedValue: (int)assemblyRef.Flags, includeAll: false)
			};

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int PublicKeyOrToken => MetadataTokens.GetHeapOffset(assemblyRef.PublicKeyOrToken);

			public string PublicKeyOrTokenTooltip {
				get {
					if (assemblyRef.PublicKeyOrToken.IsNil)
						return null;
					System.Collections.Immutable.ImmutableArray<byte> token = metadataFile.Metadata.GetBlobContent(assemblyRef.PublicKeyOrToken);
					return token.ToHexString(token.Length);
				}
			}

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(assemblyRef.Name):X} \"{Name}\"";

			public string Name => metadataFile.Metadata.GetString(assemblyRef.Name);

			public string CultureTooltip => $"{MetadataTokens.GetHeapOffset(assemblyRef.Culture):X} \"{Culture}\"";

			public string Culture => metadataFile.Metadata.GetString(assemblyRef.Culture);

			public AssemblyRefEntry(MetadataFile metadataFile, AssemblyReferenceHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.assemblyRef = metadataFile.Metadata.GetAssemblyReference(handle);
			}
		}
	}
}