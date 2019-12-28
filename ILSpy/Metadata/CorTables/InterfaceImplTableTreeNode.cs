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
	class InterfaceImplTableTreeNode : MetadataTableTreeNode
	{
		public InterfaceImplTableTreeNode(PEFile module)
			: base((HandleKind)0x09, module)
		{
		}

		public override object Text => $"09 InterfaceImpl ({module.Metadata.GetTableRowCount(TableIndex.InterfaceImpl)})";

		public override object Icon => Images.Literal;

		public unsafe override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<InterfaceImplEntry>();
			InterfaceImplEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.InterfaceImpl);
			byte* ptr = metadata.MetadataPointer;
			int metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
			for (int rid = 1; rid <= length; rid++) {
				InterfaceImplEntry entry = new InterfaceImplEntry(module, ptr, metadataOffset, rid);
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

		readonly struct InterfaceImpl
		{
			public readonly EntityHandle Class;
			public readonly EntityHandle Interface;

			public unsafe InterfaceImpl(byte *ptr, int classSize, int interfaceSize)
			{
				Class = MetadataTokens.TypeDefinitionHandle(Helpers.GetValue(ptr, classSize));
				Interface = Helpers.FromTypeDefOrRefTag((uint)Helpers.GetValue(ptr + classSize, interfaceSize));
			}
		}

		unsafe struct InterfaceImplEntry
		{
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly InterfaceImpl interfaceImpl;

			public int RID { get; }

			public int Token => 0x09000000 | RID;

			public int Offset { get; }

			[StringFormat("X8")]
			public int Class => MetadataTokens.GetToken(interfaceImpl.Class);

			public string ClassTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)interfaceImpl.Class).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public int Interface => MetadataTokens.GetToken(interfaceImpl.Interface);

			public string InterfaceTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)interfaceImpl.Interface).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public InterfaceImplEntry(PEFile module, byte* ptr, int metadataOffset, int row)
			{
				this.module = module;
				this.metadata = module.Metadata;
				this.RID = row;
				var rowOffset = metadata.GetTableMetadataOffset(TableIndex.InterfaceImpl)
					+ metadata.GetTableRowSize(TableIndex.InterfaceImpl) * (row - 1);
				this.Offset = metadataOffset + rowOffset;
				this.interfaceImpl = new InterfaceImpl(ptr + rowOffset, metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4, metadata.ComputeCodedTokenSize(16384, TableMask.TypeDef | TableMask.TypeRef | TableMask.TypeSpec));
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "InterfaceImpls");
		}
	}
}
