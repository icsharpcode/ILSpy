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

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class StateMachineMethodTableTreeNode : DebugMetadataTableTreeNode
	{
		public StateMachineMethodTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.StateMachineMethod, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<StateMachineMethodEntry>();
			StateMachineMethodEntry scrollTargetEntry = default;
			var length = metadataFile.Metadata.GetTableRowCount(TableIndex.StateMachineMethod);
			var reader = metadataFile.Metadata.AsBlobReader();
			reader.Offset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.StateMachineMethod);

			for (int rid = 1; rid <= length; rid++)
			{
				StateMachineMethodEntry entry = new StateMachineMethodEntry(metadataFile, ref reader, rid);
				if (scrollTarget == rid)
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

		struct StateMachineMethodEntry
		{
			readonly int? offset;
			readonly MetadataFile metadataFile;
			readonly MethodDefinitionHandle moveNextMethod;
			readonly MethodDefinitionHandle kickoffMethod;

			public int RID { get; }

			public int Token => 0x36000000 + RID;

			public object Offset => offset == null ? "n/a" : (object)offset;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MoveNextMethod => MetadataTokens.GetToken(moveNextMethod);

			public void OnMoveNextMethodClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, moveNextMethod, protocol: "metadata")));
			}

			string moveNextMethodTooltip;
			public string MoveNextMethodTooltip => GenerateTooltip(ref moveNextMethodTooltip, metadataFile, moveNextMethod);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int KickoffMethod => MetadataTokens.GetToken(kickoffMethod);

			public void OnKickofMethodClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, kickoffMethod, protocol: "metadata")));
			}

			string kickoffMethodTooltip;
			public string KickoffMethodTooltip => GenerateTooltip(ref kickoffMethodTooltip, metadataFile, kickoffMethod);

			public StateMachineMethodEntry(MetadataFile metadataFile, ref BlobReader reader, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				int rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.StateMachineMethod)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.StateMachineMethod) * (row - 1);
				this.offset = metadataFile.IsEmbedded ? null : (int?)rowOffset;

				int methodDefSize = metadataFile.Metadata.GetTableRowCount(TableIndex.MethodDef) < ushort.MaxValue ? 2 : 4;
				this.moveNextMethod = MetadataTokens.MethodDefinitionHandle(methodDefSize == 2 ? reader.ReadInt16() : reader.ReadInt32());
				this.kickoffMethod = MetadataTokens.MethodDefinitionHandle(methodDefSize == 2 ? reader.ReadInt16() : reader.ReadInt32());
				this.kickoffMethodTooltip = null;
				this.moveNextMethodTooltip = null;
			}

		}
	}
}