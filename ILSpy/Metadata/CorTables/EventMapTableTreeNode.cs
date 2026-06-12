// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the EventMap table — pairs each TypeDef that declares events with the first
	/// row of its event list (later rows belong to the same type until the next EventMap row).
	/// </summary>
	public sealed class EventMapTableTreeNode : MetadataTableTreeNode<EventMapTableTreeNode.EventMapEntry>
	{
		public EventMapTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.EventMap, metadataFile)
		{
		}

		protected override IReadOnlyList<EventMapEntry> LoadTable()
		{
			var list = new List<EventMapEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.EventMap);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.EventMap);
			int typeDefSize = metadata.GetTableRowCount(TableIndex.TypeDef) < ushort.MaxValue ? 2 : 4;
			int eventDefSize = metadata.GetTableRowCount(TableIndex.Event) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				int parentRow = typeDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int eventListRow = eventDefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new EventMapEntry(metadataFile, rid, MetadataTokens.TypeDefinitionHandle(parentRow), MetadataTokens.EventDefinitionHandle(eventListRow)));
			}
			return list;
		}

		public sealed class EventMapEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x12000000 | RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, parent);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int EventList => MetadataTokens.GetToken(eventList);

			string? eventListTooltip;
			public string? EventListTooltip => GenerateTooltip(ref eventListTooltip, metadataFile, eventList);

			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle parent;
			readonly EventDefinitionHandle eventList;

			public EventMapEntry(MetadataFile metadataFile, int rid, TypeDefinitionHandle parent, EventDefinitionHandle eventList)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				this.parent = parent;
				this.eventList = eventList;
			}
		}
	}
}
