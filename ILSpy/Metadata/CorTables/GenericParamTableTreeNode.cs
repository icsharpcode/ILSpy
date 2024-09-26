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
	internal class GenericParamTableTreeNode : MetadataTableTreeNode
	{
		public GenericParamTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.GenericParam, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			var list = new List<GenericParamEntry>();
			GenericParamEntry scrollTargetEntry = default;

			for (int row = 1; row <= metadataFile.Metadata.GetTableRowCount(TableIndex.GenericParam); row++)
			{
				GenericParamEntry entry = new GenericParamEntry(metadataFile, MetadataTokens.GenericParameterHandle(row));
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

		struct GenericParamEntry
		{
			readonly MetadataFile metadataFile;
			readonly GenericParameterHandle handle;
			readonly GenericParameter genericParam;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.GenericParam)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.GenericParam) * (RID - 1);

			public int Number => genericParam.Index;

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public GenericParameterAttributes Attributes => genericParam.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(GenericParameterAttributes), "Code type: ", (int)GenericParameterAttributes.VarianceMask, (int)(genericParam.Attributes & GenericParameterAttributes.VarianceMask), new Flag("None (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(GenericParameterAttributes), "Managed type: ", (int)GenericParameterAttributes.SpecialConstraintMask, (int)(genericParam.Attributes & GenericParameterAttributes.SpecialConstraintMask), new Flag("None (0000)", 0, false), includeAny: false),
			};

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Owner => MetadataTokens.GetToken(genericParam.Parent);

			public void OnOwnerClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, genericParam.Parent, protocol: "metadata")));
			}

			string ownerTooltip;
			public string OwnerTooltip => GenerateTooltip(ref ownerTooltip, metadataFile, genericParam.Parent);

			public string Name => metadataFile.Metadata.GetString(genericParam.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(genericParam.Name):X} \"{Name}\"";

			public GenericParamEntry(MetadataFile metadataFile, GenericParameterHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.genericParam = metadataFile.Metadata.GetGenericParameter(handle);
				this.ownerTooltip = null;
			}
		}
	}
}