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
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ParamTableTreeNode : MetadataTableTreeNode
	{
		public ParamTableTreeNode(PEFile module)
			: base(HandleKind.Parameter, module)
		{
		}

		public override object Text => $"08 Param ({module.Metadata.GetTableRowCount(TableIndex.Param)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;
			
			var list = new List<ParamEntry>();
			ParamEntry scrollTargetEntry = default;

			for (int row = 1; row <= module.Metadata.GetTableRowCount(TableIndex.Param); row++) {
				ParamEntry entry = new ParamEntry(module, MetadataTokens.ParameterHandle(row));
				if (entry.RID == this.scrollTarget) {
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

		struct ParamEntry
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly ParameterHandle handle;
			readonly Parameter param;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Param)
				+ metadata.GetTableRowSize(TableIndex.Param) * (RID-1);

			[StringFormat("X8")]
			public ParameterAttributes Attributes => param.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(ParameterAttributes), selectedValue: (int)param.Attributes, includeAll: false)
			};

			public string Name => metadata.GetString(param.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(param.Name):X} \"{Name}\"";

			public int Sequence => param.SequenceNumber;

			public ParamEntry(PEFile module, ParameterHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.param = metadata.GetParameter(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Params");
		}
	}
}