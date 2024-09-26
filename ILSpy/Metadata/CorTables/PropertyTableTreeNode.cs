﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class PropertyTableTreeNode : MetadataTableTreeNode
	{
		public PropertyTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Property, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<PropertyDefEntry>();
			PropertyDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.PropertyDefinitions)
			{
				PropertyDefEntry entry = new PropertyDefEntry(metadataFile, row);
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

		struct PropertyDefEntry : IMemberTreeNode
		{
			readonly MetadataFile metadataFile;
			readonly PropertyDefinitionHandle handle;
			readonly PropertyDefinition propertyDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.Property)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.Property) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public PropertyAttributes Attributes => propertyDef.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(PropertyAttributes), selectedValue: (int)propertyDef.Attributes, includeAll: false),
			};

			public string Name => metadataFile.Metadata.GetString(propertyDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(propertyDef.Name):X} \"{Name}\"";

			IEntity IMemberTreeNode.Member => ((MetadataModule)metadataFile.GetTypeSystemWithCurrentOptionsOrNull()?.MainModule).GetDefinition(handle);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(propertyDef.Signature);

			string signatureTooltip;
			public string SignatureTooltip => GenerateTooltip(ref signatureTooltip, metadataFile, handle);

			public PropertyDefEntry(MetadataFile metadataFile, PropertyDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.propertyDef = metadataFile.Metadata.GetPropertyDefinition(handle);
				this.signatureTooltip = null;
			}
		}
	}
}
