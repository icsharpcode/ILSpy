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
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ConstantTableTreeNode : MetadataTableTreeNode
	{
		public ConstantTableTreeNode(PEFile module)
			: base((HandleKind)0x0B, module)
		{
		}

		public override object Text => $"0B Constant ({module.Metadata.GetTableRowCount(TableIndex.Constant)})";

		public override object Icon => Images.Literal;

		public override bool View(TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;

			var list = new List<ConstantEntry>();
			ConstantEntry scrollTargetEntry = default;

			for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.Constant); row++) {
				ConstantEntry entry = new ConstantEntry(module, MetadataTokens.ConstantHandle(row));
				if (scrollTarget == row) {
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

		struct ConstantEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly EntityHandle handle;
			readonly Constant constant;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Constant)
				+ metadata.GetTableRowSize(TableIndex.Constant) * (RID - 1);

			[StringFormat("X8")]
			public ConstantTypeCode Type => constant.TypeCode;

			public string TypeTooltip => constant.TypeCode.ToString();

			[StringFormat("X8")]
			public int Parent => MetadataTokens.GetToken(constant.Parent);

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module);
					constant.Parent.WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X")]
			public int Value => MetadataTokens.GetHeapOffset(constant.Value);

			public string ValueTooltip {
				get {
					return null;
				}
			}

			public ConstantEntry(PEFile module, ConstantHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.constant = metadata.GetConstant(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Constants");
		}
	}
}
