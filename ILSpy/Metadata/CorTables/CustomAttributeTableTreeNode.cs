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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	class CustomAttributeTableTreeNode : MetadataTableTreeNode
	{
		public CustomAttributeTableTreeNode(PEFile module)
			: base(HandleKind.CustomAttribute, module)
		{
		}

		public override object Text => $"0C CustomAttribute ({module.Metadata.GetTableRowCount(TableIndex.CustomAttribute)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;

			var list = new List<CustomAttributeEntry>();
			CustomAttributeEntry scrollTargetEntry = default;

			foreach (var row in metadata.CustomAttributes) {
				CustomAttributeEntry entry = new CustomAttributeEntry(module, row);
				if (scrollTarget == MetadataTokens.GetRowNumber(row)) {
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0) {
				view.ScrollIntoView(scrollTargetEntry);
				this.scrollTarget = default;
			}

			return true;
		}

		struct CustomAttributeEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly CustomAttributeHandle handle;
			readonly CustomAttribute customAttr;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.CustomAttribute)
				+ metadata.GetTableRowSize(TableIndex.CustomAttribute) * (RID - 1);

			[StringFormat("X8")]
			public int Parent => MetadataTokens.GetToken(customAttr.Parent);

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					customAttr.Parent.WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public int Constructor => MetadataTokens.GetToken(customAttr.Constructor);

			public string ConstructorTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					customAttr.Constructor.WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X")]
			public int Value => MetadataTokens.GetHeapOffset(customAttr.Value);

			public string ValueTooltip {
				get {
					return null;
				}
			}

			public CustomAttributeEntry(PEFile module, CustomAttributeHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.customAttr = metadata.GetCustomAttribute(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "CustomAttributes");
		}
	}
}
