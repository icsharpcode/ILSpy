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
using System.Windows.Controls;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class ConstantTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public ConstantTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"0B Constant ({module.Metadata.GetTableRowCount(TableIndex.Constant)})";

		public override object Icon => Images.Literal;

		public override bool View(DecompilerTextView textView)
		{
			ListView view = Helpers.CreateListView("ConstantsView");
			var metadata = module.Metadata;

			var list = new List<ConstantEntry>();

			for (int row = 1; row <= module.Metadata.GetTableRowCount(TableIndex.Constant); row++) {
				list.Add(new ConstantEntry(module, MetadataTokens.ConstantHandle(row)));
			}

			view.ItemsSource = list;

			textView.ShowContent(new[] { this }, view);
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

			public int Type => (int)constant.TypeCode;

			public string TypeTooltip => constant.TypeCode.ToString();

			public int ParentHandle => MetadataTokens.GetToken(constant.Parent);

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module);
					constant.Parent.WriteTo(module, output, context);
					return output.ToString();
				}
			}

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
