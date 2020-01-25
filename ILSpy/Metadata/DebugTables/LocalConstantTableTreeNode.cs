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
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class LocalConstantTableTreeNode : DebugMetadataTableTreeNode
	{
		private readonly bool isEmbedded;

		public LocalConstantTableTreeNode(PEFile module, MetadataReader metadata, bool isEmbedded)
			: base(HandleKind.LocalConstant, module, metadata)
		{
			this.isEmbedded = isEmbedded;
		}

		public override object Text => $"34 LocalConstant ({metadata.GetTableRowCount(TableIndex.LocalConstant)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<LocalConstantEntry>();
			LocalConstantEntry scrollTargetEntry = default;

			foreach (var row in metadata.LocalConstants) {
				LocalConstantEntry entry = new LocalConstantEntry(module, metadata, isEmbedded, row);
				if (entry.RID == scrollTarget) {
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 1) {
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct LocalConstantEntry
		{
			readonly int? offset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly LocalConstantHandle handle;
			readonly LocalConstant localConst;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			public string Name => metadata.GetString(localConst.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(localConst.Name):X} \"{Name}\"";

			[StringFormat("X")]
			public int Signature => MetadataTokens.GetToken(localConst.Signature);

			public LocalConstantEntry(PEFile module, MetadataReader metadata, bool isEmbedded, LocalConstantHandle handle)
			{
				this.offset = isEmbedded ? null : (int?)metadata.GetTableMetadataOffset(TableIndex.LocalConstant)
					+ metadata.GetTableRowSize(TableIndex.LocalConstant) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.module = module;
				this.metadata = metadata;
				this.handle = handle;
				this.localConst = metadata.GetLocalConstant(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Document");
		}
	}
}