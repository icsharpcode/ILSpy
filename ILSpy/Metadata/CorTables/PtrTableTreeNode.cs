// Copyright (c) 2024 AlphaSierraPapa for the SharpDevelop Team
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
	class PtrTableTreeNode : MetadataTableTreeNode
	{
		readonly TableIndex referencedTableKind;

		public PtrTableTreeNode(TableIndex kind, MetadataFile metadataFile)
			: base(kind, metadataFile)
		{
			if (kind is not (TableIndex.EventPtr or TableIndex.FieldPtr or TableIndex.MethodPtr or TableIndex.ParamPtr or TableIndex.PropertyPtr))
			{
				throw new ArgumentOutOfRangeException("PtrTable does not support " + kind);
			}

			this.referencedTableKind = kind switch {
				TableIndex.EventPtr => TableIndex.Event,
				TableIndex.FieldPtr => TableIndex.Field,
				TableIndex.MethodPtr => TableIndex.MethodDef,
				TableIndex.ParamPtr => TableIndex.Param,
				TableIndex.PropertyPtr => TableIndex.Property,
				_ => throw new NotImplementedException(), // unreachable
			};
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<PtrEntry>();
			PtrEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(Kind);
			ReadOnlySpan<byte> ptr = metadata.AsReadOnlySpan();

			int handleDefSize = metadataFile.Metadata.GetTableRowCount(referencedTableKind) < ushort.MaxValue ? 2 : 4;

			for (int rid = 1; rid <= length; rid++)
			{
				var entry = new PtrEntry(metadataFile, Kind, referencedTableKind, handleDefSize, ptr, rid);
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

		readonly struct HandlePtr
		{
			public readonly EntityHandle Handle;

			public HandlePtr(ReadOnlySpan<byte> ptr, TableIndex kind, int handleSize)
			{
				Handle = MetadataTokens.EntityHandle(((int)kind << 24) | Helpers.GetValueLittleEndian(ptr, handleSize));
			}
		}

		struct PtrEntry
		{
			readonly MetadataFile metadataFile;
			readonly HandlePtr handlePtr;
			readonly TableIndex kind;

			public int RID { get; }

			public int Token => ((int)kind << 24) | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Handle => MetadataTokens.GetToken(handlePtr.Handle);

			public void OnHandleClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, handlePtr.Handle, protocol: "metadata")));
			}

			string handleTooltip;
			public string HandleTooltip => GenerateTooltip(ref handleTooltip, metadataFile, handlePtr.Handle);

			public PtrEntry(MetadataFile metadataFile, TableIndex kind, TableIndex handleKind, int handleDefSize, ReadOnlySpan<byte> ptr, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				this.kind = kind;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(kind)
					+ metadataFile.Metadata.GetTableRowSize(kind) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;

				this.handlePtr = new HandlePtr(ptr.Slice(rowOffset), handleKind, handleDefSize);
				this.handleTooltip = null;
			}
		}
	}
}
