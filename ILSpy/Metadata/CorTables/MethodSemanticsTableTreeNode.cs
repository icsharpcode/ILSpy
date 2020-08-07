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

namespace ICSharpCode.ILSpy.Metadata
{
	internal class MethodSemanticsTableTreeNode : MetadataTableTreeNode
	{
		public MethodSemanticsTableTreeNode(PEFile module)
			: base((HandleKind)0x18, module)
		{
		}

		public override object Text => $"18 MethodSemantics ({module.Metadata.GetTableRowCount(TableIndex.MethodSemantics)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<MethodSemanticsEntry>();
			MethodSemanticsEntry scrollTargetEntry = default;

			foreach (var row in metadata.GetMethodSemantics()) {
				MethodSemanticsEntry entry = new MethodSemanticsEntry(module, row.Handle, row.Semantics, row.Method, row.Association);
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

		struct MethodSemanticsEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly Handle handle;
			readonly MethodSemanticsAttributes semantics;
			readonly MethodDefinitionHandle method;
			readonly EntityHandle association;

			public int RID => MetadataTokens.GetToken(handle) & 0xFFFFFF;

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.MethodDef)
				+ metadata.GetTableRowSize(TableIndex.MethodDef) * (RID - 1);

			[StringFormat("X8")]
			public MethodSemanticsAttributes Semantics => semantics;

			public string SemanticsTooltip => semantics.ToString();

			[StringFormat("X8")]
			public int Method => MetadataTokens.GetToken(method);

			public string MethodTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					((EntityHandle)method).WriteTo(module, output, Decompiler.Metadata.GenericContext.Empty);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public int Association => MetadataTokens.GetToken(association);

			public string AssociationTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					association.WriteTo(module, output, Decompiler.Metadata.GenericContext.Empty);
					return output.ToString();
				}
			}

			public MethodSemanticsEntry(PEFile module, Handle handle, MethodSemanticsAttributes semantics, MethodDefinitionHandle method, EntityHandle association)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.semantics = semantics;
				this.method = method;
				this.association = association;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "MethodDefs");
		}
	}
}
