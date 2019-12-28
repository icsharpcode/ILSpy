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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class PropertyTableTreeNode : MetadataTableTreeNode
	{
		public PropertyTableTreeNode(PEFile module)
			: base(HandleKind.PropertyDefinition, module)
		{
		}

		public override object Text => $"17 Property ({module.Metadata.GetTableRowCount(TableIndex.Property)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<PropertyDefEntry>();
			PropertyDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.PropertyDefinitions) {
				PropertyDefEntry entry = new PropertyDefEntry(module, row);
				if (entry.RID == this.scrollTarget) {
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0) {
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct PropertyDefEntry : IMemberTreeNode
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly PropertyDefinitionHandle handle;
			readonly PropertyDefinition propertyDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Property)
				+ metadata.GetTableRowSize(TableIndex.Property) * (RID - 1);

			[StringFormat("X8")]
			public PropertyAttributes Attributes => propertyDef.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(PropertyAttributes), selectedValue: (int)propertyDef.Attributes, includeAll: false),
			};

			public string Name => metadata.GetString(propertyDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(propertyDef.Name):X} \"{Name}\"";

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemOrNull()?.MainModule).GetDefinition(handle);

			[StringFormat("X")]
			public int Signature => MetadataTokens.GetHeapOffset(propertyDef.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					((EntityHandle)handle).WriteTo(module, output, Decompiler.Metadata.GenericContext.Empty);
					return output.ToString();
				}
			}

			public PropertyDefEntry(PEFile module, PropertyDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.propertyDef = metadata.GetPropertyDefinition(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "PropertyDefs");
		}
	}
}
