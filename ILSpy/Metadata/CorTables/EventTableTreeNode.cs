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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class EventTableTreeNode : MetadataTableTreeNode
	{
		public EventTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Event, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<EventDefEntry>();
			EventDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.EventDefinitions)
			{
				EventDefEntry entry = new EventDefEntry(metadataFile, row);
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
			readonly MetadataFile metadataFile;
			readonly EventDefinitionHandle handle;
			readonly EventDefinition eventDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.Event)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.Event) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public EventAttributes Attributes => eventDef.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(EventAttributes), selectedValue: (int)eventDef.Attributes, includeAll: false),
			};

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(eventDef.Name):X} \"{Name}\"";

			public string Name => metadataFile.Metadata.GetString(eventDef.Name);

			IEntity IMemberTreeNode.Member {
				get {
					return ((MetadataModule)metadataFile.GetTypeSystemWithCurrentOptionsOrNull(SettingsService)?.MainModule)?.GetDefinition(handle);
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Type => MetadataTokens.GetToken(eventDef.Type);

			public void OnTypeClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, eventDef.Type, protocol: "metadata")));
			}

			string typeTooltip;
			public string TypeTooltip => GenerateTooltip(ref typeTooltip, metadataFile, eventDef.Type);

			public EventDefEntry(MetadataFile metadataFile, EventDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.eventDef = metadataFile.Metadata.GetEventDefinition(handle);
				this.typeTooltip = null;
			}
		}
	}
}
