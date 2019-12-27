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
	class EventMapTableTreeNode : MetadataTableTreeNode
	{
		public EventMapTableTreeNode(PEFile module)
			: base((HandleKind)0x12, module)
		{
		}

		public override object Text => $"12 EventMap ({module.Metadata.GetTableRowCount(TableIndex.EventMap)})";

		public override object Icon => Images.Literal;

		public unsafe override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;

			var list = new List<EventMapEntry>();
			EventMapEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.EventMap);
			byte* ptr = metadata.MetadataPointer;
			int metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
			for (int rid = 1; rid <= length; rid++) {
				EventMapEntry entry = new EventMapEntry(module, ptr, metadataOffset, rid);
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

		readonly struct EventMap
		{
			public readonly TypeDefinitionHandle Parent;
			public readonly EventDefinitionHandle EventList;

			public unsafe EventMap(byte *ptr, int typeDefSize, int eventDefSize)
			{
				Parent = MetadataTokens.TypeDefinitionHandle(Helpers.GetValue(ptr, typeDefSize));
				EventList = MetadataTokens.EventDefinitionHandle(Helpers.GetValue(ptr + typeDefSize, eventDefSize));
			}
		}

		unsafe struct EventMapEntry
		{
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly EventMap eventMap;

			public int RID { get; }

			public int Token => 0x12000000 | RID;

			public int Offset { get; }

			[StringFormat("X8")]
			public int Parent => MetadataTokens.GetToken(eventMap.Parent);

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)eventMap.Parent).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public int EventList => MetadataTokens.GetToken(eventMap.EventList);

			public string EventListTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)eventMap.EventList).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public EventMapEntry(PEFile module, byte* ptr, int metadataOffset, int row)
			{
				this.module = module;
				this.metadata = module.Metadata;
				this.RID = row;
				var rowOffset = metadata.GetTableMetadataOffset(TableIndex.EventMap)
					+ metadata.GetTableRowSize(TableIndex.EventMap) * (row - 1);
				this.Offset = metadataOffset + rowOffset;
				int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
				int eventDefSize = metadata.GetTableRowCount(TableIndex.Event) < ushort.MaxValue ? 2 : 4;
				this.eventMap = new EventMap(ptr + rowOffset, typeDefSize, eventDefSize);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "EventMap");
		}
	}
}
