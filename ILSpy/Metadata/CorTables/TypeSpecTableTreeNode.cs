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
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class TypeSpecTableTreeNode : MetadataTableTreeNode
	{
		public TypeSpecTableTreeNode(PEFile module)
			: base(HandleKind.TypeSpecification, module)
		{
		}

		public override object Text => $"1B TypeSpec ({module.Metadata.GetTableRowCount(TableIndex.TypeSpec)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;
			
			var list = new List<TypeSpecEntry>();
			TypeSpecEntry scrollTargetEntry = default;
			
			foreach (var row in metadata.GetTypeSpecifications()) {
				TypeSpecEntry entry = new TypeSpecEntry(module, row);
				if (scrollTarget.Equals(row)) {
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

		struct TypeSpecEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly TypeSpecificationHandle handle;
			readonly TypeSpecification typeSpec;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.TypeSpec)
				+ metadata.GetTableRowSize(TableIndex.TypeSpec) * (RID-1);

			[StringFormat("X")]
			public int Signature => MetadataTokens.GetHeapOffset(typeSpec.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					typeSpec.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), GenericContext.Empty)(ILNameSyntax.Signature);
					return output.ToString();
				}
			}

			public TypeSpecEntry(PEFile module, TypeSpecificationHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.typeSpec = metadata.GetTypeSpecification(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "TypeSpecs");
		}
	}
}