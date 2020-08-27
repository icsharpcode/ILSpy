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
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class StateMachineMethodTableTreeNode : DebugMetadataTableTreeNode
	{
		readonly bool isEmbedded;

		public StateMachineMethodTableTreeNode(PEFile module, MetadataReader metadata, bool isEmbedded)
			: base((HandleKind)0x36, module, metadata)
		{
			this.isEmbedded = isEmbedded;
		}

		public override object Text => $"36 StateMachineMethod ({metadata.GetTableRowCount(TableIndex.StateMachineMethod)})";

		public override object Icon => Images.Literal;

		public unsafe override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<StateMachineMethodEntry>();
			StateMachineMethodEntry scrollTargetEntry = default;
			var length = metadata.GetTableRowCount(TableIndex.StateMachineMethod);
			var reader = new BlobReader(metadata.MetadataPointer, metadata.MetadataLength);
			reader.Offset = +metadata.GetTableMetadataOffset(TableIndex.StateMachineMethod);

			for (int rid = 1; rid <= length; rid++)
			{
				StateMachineMethodEntry entry = new StateMachineMethodEntry(module, ref reader, isEmbedded, rid);
				if (scrollTarget == rid)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 1)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct StateMachineMethodEntry
		{
			readonly int? offset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly MethodDefinitionHandle moveNextMethod;
			readonly MethodDefinitionHandle kickoffMethod;

			public int RID { get; }

			public object Offset => offset == null ? "n/a" : (object)offset;

			[StringFormat("X8")]
			public int MoveNextMethod => MetadataTokens.GetToken(moveNextMethod);

			public string MoveNextMethodTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)moveNextMethod).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public int KickoffMethod => MetadataTokens.GetToken(kickoffMethod);

			public string KickoffMethodTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)kickoffMethod).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public StateMachineMethodEntry(PEFile module, ref BlobReader reader, bool isEmbedded, int row)
			{
				this.module = module;
				this.metadata = module.Metadata;
				this.RID = row;
				int rowOffset = metadata.GetTableMetadataOffset(TableIndex.StateMachineMethod)
					+ metadata.GetTableRowSize(TableIndex.StateMachineMethod) * (row - 1);
				this.offset = isEmbedded ? null : (int?)rowOffset;

				int methodDefSize = metadata.GetTableRowCount(TableIndex.MethodDef) < ushort.MaxValue ? 2 : 4;
				this.moveNextMethod = MetadataTokens.MethodDefinitionHandle(methodDefSize == 2 ? reader.ReadInt16() : reader.ReadInt32());
				this.kickoffMethod = MetadataTokens.MethodDefinitionHandle(methodDefSize == 2 ? reader.ReadInt16() : reader.ReadInt32());
			}

		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "StateMachineMethod");
		}
	}
}