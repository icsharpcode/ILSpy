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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	class EventMapTableTreeNode : MetadataTableTreeNode
	{
		public EventMapTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.EventMap, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<EventMapEntry>();
			EventMapEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.EventMap);
			ReadOnlySpan<byte> ptr = metadata.AsReadOnlySpan();
			for (int rid = 1; rid <= length; rid++)
			{
				EventMapEntry entry = new EventMapEntry(metadataFile, ptr, rid);
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

		readonly struct EventMap
		{
			public readonly TypeDefinitionHandle Parent;
			public readonly EventDefinitionHandle EventList;

			public EventMap(ReadOnlySpan<byte> ptr, int typeDefSize, int eventDefSize)
			{
				Parent = MetadataTokens.TypeDefinitionHandle(Helpers.GetValueLittleEndian(ptr.Slice(0, typeDefSize)));
				EventList = MetadataTokens.EventDefinitionHandle(Helpers.GetValueLittleEndian(ptr.Slice(typeDefSize, eventDefSize)));
			}
		}

		struct EventMapEntry
		{
			readonly MetadataFile metadataFile;
			readonly EventMap eventMap;

			public int RID { get; }

			public int Token => 0x12000000 | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(eventMap.Parent);

			public void OnParentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, eventMap.Parent, protocol: "metadata")));
			}

			string parentTooltip;
			public string ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, eventMap.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int EventList => MetadataTokens.GetToken(eventMap.EventList);

			public void OnEventListClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, eventMap.EventList, protocol: "metadata")));
			}

			string eventListTooltip;
			public string EventListTooltip => GenerateTooltip(ref eventListTooltip, metadataFile, eventMap.EventList);

			public EventMapEntry(MetadataFile metadataFile, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.EventMap)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.EventMap) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				int typeDefSize = metadataFile.Metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
				int eventDefSize = metadataFile.Metadata.GetTableRowCount(TableIndex.Event) < ushort.MaxValue ? 2 : 4;
				this.eventMap = new EventMap(ptr.Slice(rowOffset), typeDefSize, eventDefSize);
				this.parentTooltip = null;
				this.eventListTooltip = null;
			}
		}
	}
}
