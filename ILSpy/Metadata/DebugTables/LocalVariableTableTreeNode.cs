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
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class LocalVariableTableTreeNode : DebugMetadataTableTreeNode
	{
		private readonly bool isEmbedded;

		public LocalVariableTableTreeNode(PEFile module, MetadataReader metadata, bool isEmbedded)
			: base(HandleKind.LocalVariable, module, metadata)
		{
			this.isEmbedded = isEmbedded;
		}

		public override object Text => $"33 LocalVariable ({metadata.GetTableRowCount(TableIndex.LocalVariable)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<LocalVariableEntry>();
			LocalVariableEntry scrollTargetEntry = default;

			foreach (var row in metadata.LocalVariables) {
				LocalVariableEntry entry = new LocalVariableEntry(module, metadata, isEmbedded, row);
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

		struct LocalVariableEntry
		{
			readonly int? offset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly LocalVariableHandle handle;
			readonly LocalVariable localVar;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			[StringFormat("X8")]
			public LocalVariableAttributes Attributes => localVar.Attributes;

			public object AttributesTooltip => new FlagsTooltip() {
				FlagGroup.CreateMultipleChoiceGroup(typeof(LocalVariableAttributes)),
			};

			public int Index => localVar.Index;

			public string Name => metadata.GetString(localVar.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(localVar.Name):X} \"{Name}\"";

			public LocalVariableEntry(PEFile module, MetadataReader metadata, bool isEmbedded, LocalVariableHandle handle)
			{
				this.offset = isEmbedded ? null : (int?)metadata.GetTableMetadataOffset(TableIndex.LocalVariable)
					+ metadata.GetTableRowSize(TableIndex.LocalVariable) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.module = module;
				this.metadata = metadata;
				this.handle = handle;
				this.localVar = metadata.GetLocalVariable(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Document");
		}
	}
}