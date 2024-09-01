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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class AssemblyTableTreeNode : MetadataTableTreeNode
	{
		public AssemblyTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Assembly, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			if (metadataFile.Metadata.IsAssembly)
			{
				view.ItemsSource = new[] { new AssemblyEntry(metadataFile.Metadata, metadataFile.MetadataOffset) };
			}
			else
			{
				view.ItemsSource = Array.Empty<AssemblyEntry>();
			}

			tabPage.Content = view;
			return true;
		}

		readonly struct AssemblyEntry
		{
			readonly int metadataOffset;
			readonly MetadataReader metadata;
			readonly AssemblyDefinition assembly;

			public int RID => MetadataTokens.GetRowNumber(EntityHandle.AssemblyDefinition);

			public int Token => MetadataTokens.GetToken(EntityHandle.AssemblyDefinition);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Assembly)
				+ metadata.GetTableRowSize(TableIndex.Assembly) * (RID - 1);

			[ColumnInfo("X4", Kind = ColumnKind.Other)]
			public AssemblyHashAlgorithm HashAlgorithm => assembly.HashAlgorithm;

			public object HashAlgorithmTooltip => new FlagsTooltip() {
				FlagGroup.CreateSingleChoiceGroup(typeof(AssemblyHashAlgorithm), selectedValue: (int)assembly.HashAlgorithm, defaultFlag: new Flag("None (0000)", 0, false), includeAny: false)
			};

			[ColumnInfo("X4", Kind = ColumnKind.Other)]
			public AssemblyFlags Flags => assembly.Flags;

			public object FlagsTooltip => new FlagsTooltip() {
				FlagGroup.CreateMultipleChoiceGroup(typeof(AssemblyFlags), selectedValue: (int)assembly.Flags, includeAll: false)
			};

			public Version Version => assembly.Version;

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(assembly.Name):X} \"{Name}\"";

			public string Name => metadata.GetString(assembly.Name);

			public string CultureTooltip => $"{MetadataTokens.GetHeapOffset(assembly.Culture):X} \"{Culture}\"";

			public string Culture => metadata.GetString(assembly.Culture);

			public AssemblyEntry(MetadataReader metadata, int metadataOffset)
			{
				this.metadata = metadata;
				this.metadataOffset = metadataOffset;
				this.assembly = metadata.GetAssemblyDefinition();
			}
		}
	}
}