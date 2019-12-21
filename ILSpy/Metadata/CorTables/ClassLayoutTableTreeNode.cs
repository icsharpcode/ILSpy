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
	class ClassLayoutTableTreeNode : MetadataTableTreeNode
	{
		public ClassLayoutTableTreeNode(PEFile module)
			: base((HandleKind)0x0F, module)
		{
		}

		public override object Text => $"0F ClassLayout ({module.Metadata.GetTableRowCount(TableIndex.ClassLayout)})";

		public override object Icon => Images.Literal;

		public unsafe override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;

			var list = new List<ClassLayoutEntry>();

			var length = metadata.GetTableRowCount(TableIndex.ClassLayout);
			byte* ptr = metadata.MetadataPointer;
			int metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
			for (int rid = 1; rid <= length; rid++) {
				list.Add(new ClassLayoutEntry(module, ptr, metadataOffset, rid));
			}

			view.ItemsSource = list;

			tabPage.Content = view;
			return true;
		}

		readonly struct ClassLayout
		{
			public readonly ushort PackingSize;
			public readonly EntityHandle Parent;
			public readonly uint ClassSize;

			public unsafe ClassLayout(byte *ptr, int typeDefSize)
			{
				PackingSize = (ushort)Helpers.GetValue(ptr, 2);
				ClassSize = (uint)Helpers.GetValue(ptr + 2, 4);
				Parent = MetadataTokens.TypeDefinitionHandle(Helpers.GetValue(ptr + 6, typeDefSize));
			}
		}

		unsafe struct ClassLayoutEntry
		{
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly ClassLayout classLayout;

			public int RID { get; }

			public int Token => 0x0F000000 | RID;

			public int Offset { get; }

			[StringFormat("X8")]
			public int Parent => MetadataTokens.GetToken(classLayout.Parent);

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)classLayout.Parent).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X4")]
			public ushort PackingSize => classLayout.PackingSize;

			[StringFormat("X8")]
			public uint ClassSize => classLayout.ClassSize;

			public ClassLayoutEntry(PEFile module, byte* ptr, int metadataOffset, int row)
			{
				this.module = module;
				this.metadata = module.Metadata;
				this.RID = row;
				var rowOffset = metadata.GetTableMetadataOffset(TableIndex.ClassLayout)
					+ metadata.GetTableRowSize(TableIndex.ClassLayout) * (row - 1);
				this.Offset = metadataOffset + rowOffset;
				this.classLayout = new ClassLayout(ptr + rowOffset, metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "ClassLayouts");
		}
	}
}
