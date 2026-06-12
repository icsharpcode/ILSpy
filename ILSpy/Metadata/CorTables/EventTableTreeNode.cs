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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the Event table — every CLR event the module's types declare. Each row
	/// references the event's delegate type as a TypeDefOrRef coded token; add/remove/raise
	/// accessor bindings live in MethodSemantics.
	/// </summary>
	public sealed class EventTableTreeNode : MetadataTableTreeNode<EventTableTreeNode.EventDefEntry>
	{
		public EventTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Event, metadataFile)
		{
		}

		protected override IReadOnlyList<EventDefEntry> LoadTable()
		{
			var list = new List<EventDefEntry>();
			foreach (var row in metadataFile.Metadata.EventDefinitions)
				list.Add(new EventDefEntry(metadataFile, row));
			return list;
		}

		public sealed class EventDefEntry
		{
			readonly MetadataFile metadataFile;
			readonly EventDefinitionHandle handle;
			readonly EventDefinition eventDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.Event, RID);

			[ColumnInfo("X8")]
			public EventAttributes Attributes => eventDef.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateMultipleChoiceGroup(typeof(EventAttributes), selectedValue: (int)eventDef.Attributes, includeAll: false),
			};

			public string Name => metadataFile.Metadata.GetString(eventDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(eventDef.Name):X} \"{Name}\"";

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Type => MetadataTokens.GetToken(eventDef.Type);

			string? typeTooltip;
			public string? TypeTooltip => GenerateTooltip(ref typeTooltip, metadataFile, eventDef.Type);

			public EventDefEntry(MetadataFile metadataFile, EventDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				eventDef = metadataFile.Metadata.GetEventDefinition(handle);
			}
		}
	}
}
