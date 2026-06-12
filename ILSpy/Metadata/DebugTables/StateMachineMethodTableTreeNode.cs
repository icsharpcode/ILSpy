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

namespace ICSharpCode.ILSpy.Metadata.DebugTables
{
	/// <summary>
	/// View of the StateMachineMethod table — pairs each compiler-generated state-machine
	/// MoveNext method with the user-written kickoff method (the original async / iterator
	/// declaration). Read directly from the metadata table because System.Reflection.Metadata
	/// doesn't surface these rows through a typed enumeration.
	/// </summary>
	public sealed class StateMachineMethodTableTreeNode : MetadataTableTreeNode<StateMachineMethodTableTreeNode.StateMachineMethodEntry>
	{
		public StateMachineMethodTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.StateMachineMethod, metadataFile)
		{
		}

		protected override IReadOnlyList<StateMachineMethodEntry> LoadTable()
		{
			var list = new List<StateMachineMethodEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.StateMachineMethod);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.StateMachineMethod);
			int methodDefSize = metadata.GetTableRowCount(TableIndex.MethodDef) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				var moveNext = MetadataTokens.MethodDefinitionHandle(methodDefSize == 2 ? reader.ReadInt16() : reader.ReadInt32());
				var kickoff = MetadataTokens.MethodDefinitionHandle(methodDefSize == 2 ? reader.ReadInt16() : reader.ReadInt32());
				list.Add(new StateMachineMethodEntry(metadataFile, rid, moveNext, kickoff));
			}
			return list;
		}

		public sealed class StateMachineMethodEntry
		{
			readonly MetadataFile metadataFile;
			readonly MethodDefinitionHandle moveNextMethod;
			readonly MethodDefinitionHandle kickoffMethod;

			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x36000000 + RID;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MoveNextMethod => MetadataTokens.GetToken(moveNextMethod);

			string? moveNextMethodTooltip;
			public string? MoveNextMethodTooltip => GenerateTooltip(ref moveNextMethodTooltip, metadataFile, moveNextMethod);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int KickoffMethod => MetadataTokens.GetToken(kickoffMethod);

			string? kickoffMethodTooltip;
			public string? KickoffMethodTooltip => GenerateTooltip(ref kickoffMethodTooltip, metadataFile, kickoffMethod);

			public StateMachineMethodEntry(MetadataFile metadataFile, int rid, MethodDefinitionHandle moveNext, MethodDefinitionHandle kickoff)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				moveNextMethod = moveNext;
				kickoffMethod = kickoff;
			}
		}
	}
}
