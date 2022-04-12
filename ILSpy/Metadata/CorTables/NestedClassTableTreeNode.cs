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
	class NestedClassTableTreeNode : MetadataTableTreeNode
	{
		public NestedClassTableTreeNode(PEFile module)
			: base((HandleKind)0x29, module)
		{
		}

		public override object Text => $"29 NestedClass ({module.Metadata.GetTableRowCount(TableIndex.NestedClass)})";

		public override object Icon => Images.Literal;

		public unsafe override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<NestedClassEntry>();
			NestedClassEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.NestedClass);
			byte* ptr = metadata.MetadataPointer;
			int metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
			for (int rid = 1; rid <= length; rid++)
			{
				NestedClassEntry entry = new NestedClassEntry(module, ptr, metadataOffset, rid);
				if (entry.RID == this.scrollTarget)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		readonly struct NestedClass
		{
			public readonly TypeDefinitionHandle Nested;
			public readonly TypeDefinitionHandle Enclosing;

			public unsafe NestedClass(byte* ptr, int typeDefSize)
			{
				Nested = MetadataTokens.TypeDefinitionHandle(Helpers.GetValue(ptr, typeDefSize));
				Enclosing = MetadataTokens.TypeDefinitionHandle(Helpers.GetValue(ptr + typeDefSize, typeDefSize));
			}
		}

		unsafe struct NestedClassEntry
		{
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly NestedClass nestedClass;

			public int RID { get; }

			public int Token => 0x29000000 | RID;

			public int Offset { get; }

			[StringFormat("X8")]
			[LinkToTable]
			public int NestedClass => MetadataTokens.GetToken(nestedClass.Nested);

			public void OnNestedClassClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, nestedClass.Nested, protocol: "metadata"));
			}

			public string NestedClassTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.MetadataGenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)nestedClass.Nested).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			[LinkToTable]
			public int EnclosingClass => MetadataTokens.GetToken(nestedClass.Enclosing);

			public void OnEnclosingClassClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, nestedClass.Enclosing, protocol: "metadata"));
			}

			public string EnclosingClassTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.MetadataGenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)nestedClass.Enclosing).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public unsafe NestedClassEntry(PEFile module, byte* ptr, int metadataOffset, int row)
			{
				this.module = module;
				this.metadata = module.Metadata;
				this.RID = row;
				var rowOffset = metadata.GetTableMetadataOffset(TableIndex.NestedClass)
					+ metadata.GetTableRowSize(TableIndex.NestedClass) * (row - 1);
				this.Offset = metadataOffset + rowOffset;
				int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
				this.nestedClass = new NestedClass(ptr + rowOffset, typeDefSize);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "NestedClass");
		}
	}
}
