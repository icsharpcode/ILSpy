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
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class EventTableTreeNode : MetadataTableTreeNode
	{
		public EventTableTreeNode(PEFile module)
			: base(HandleKind.EventDefinition, module)
		{
		}

		public override object Text => $"14 Event ({module.Metadata.GetTableRowCount(TableIndex.Event)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<EventDefEntry>();
			EventDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.EventDefinitions)
			{
				EventDefEntry entry = new EventDefEntry(module, row);
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

		struct EventDefEntry : IMemberTreeNode
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly EventDefinitionHandle handle;
			readonly EventDefinition eventDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Event)
				+ metadata.GetTableRowSize(TableIndex.Event) * (RID - 1);

			[StringFormat("X8")]
			public EventAttributes Attributes => eventDef.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(EventAttributes), selectedValue: (int)eventDef.Attributes, includeAll: false),
			};

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(eventDef.Name):X} \"{Name}\"";

			public string Name => metadata.GetString(eventDef.Name);

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemWithCurrentOptionsOrNull()?.MainModule).GetDefinition(handle);

			[StringFormat("X8")]
			[LinkToTable]
			public int Type => MetadataTokens.GetToken(eventDef.Type);

			public void OnTypeClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, eventDef.Type, protocol: "metadata"));
			}

			public string TypeTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					eventDef.Type.WriteTo(module, output, Decompiler.Metadata.GenericContext.Empty);
					return output.ToString();
				}
			}

			public EventDefEntry(PEFile module, EventDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.eventDef = metadata.GetEventDefinition(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "EventDefs");
		}
	}
}
