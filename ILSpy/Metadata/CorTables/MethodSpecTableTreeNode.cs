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
	internal class MethodSpecTableTreeNode : MetadataTableTreeNode
	{
		public MethodSpecTableTreeNode(PEFile module)
			: base(HandleKind.MethodSpecification, module)
		{
		}

		public override object Text => $"2B MethodSpec ({module.Metadata.GetTableRowCount(TableIndex.MethodSpec)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;
			
			var list = new List<MethodSpecEntry>();
			MethodSpecEntry scrollTargetEntry = default;

			foreach (var row in metadata.GetMethodSpecifications()) {
				MethodSpecEntry entry = new MethodSpecEntry(module, row);
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

		struct MethodSpecEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly MethodSpecificationHandle handle;
			readonly MethodSpecification methodSpec;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.MethodSpec)
				+ metadata.GetTableRowSize(TableIndex.MethodSpec) * (RID-1);

			[StringFormat("X8")]
			public int Method => MetadataTokens.GetToken(methodSpec.Method);

			public string MethodTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					methodSpec.Method.WriteTo(module, output, GenericContext.Empty);
					return output.ToString();
				}
			}

			[StringFormat("X")]
			public int Signature => MetadataTokens.GetHeapOffset(methodSpec.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var signature = methodSpec.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), GenericContext.Empty);
					bool first = true;
					foreach (var type in signature) {
						if (first)
							first = false;
						else
							output.Write(", ");
						type(ILNameSyntax.TypeName);
					}
					return output.ToString();
				}
			}

			public MethodSpecEntry(PEFile module, MethodSpecificationHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.methodSpec = metadata.GetMethodSpecification(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "MethodSpecs");
		}
	}
}