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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

using Mono.Cecil;

namespace ICSharpCode.ILSpy.Metadata
{
	class ImplMapTableTreeNode : MetadataTableTreeNode
	{
		public ImplMapTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ImplMap, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<ImplMapEntry>();
			ImplMapEntry scrollTargetEntry = default;

			var length = metadata.GetTableRowCount(TableIndex.ImplMap);
			var span = metadata.AsReadOnlySpan();
			for (int rid = 1; rid <= length; rid++)
			{
				ImplMapEntry entry = new ImplMapEntry(metadataFile, span, rid);
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

		readonly struct ImplMap
		{
			public readonly PInvokeAttributes MappingFlags;
			public readonly EntityHandle MemberForwarded;
			public readonly StringHandle ImportName;
			public readonly ModuleReferenceHandle ImportScope;

			public ImplMap(ReadOnlySpan<byte> span, int moduleRefSize, int memberForwardedTagRefSize, int stringHandleSize)
			{
				MappingFlags = (PInvokeAttributes)BinaryPrimitives.ReadUInt16LittleEndian(span);
				MemberForwarded = Helpers.FromMemberForwardedTag((uint)Helpers.GetValueLittleEndian(span.Slice(2, memberForwardedTagRefSize)));
				ImportName = MetadataTokens.StringHandle(Helpers.GetValueLittleEndian(span.Slice(2 + memberForwardedTagRefSize, stringHandleSize)));
				ImportScope = MetadataTokens.ModuleReferenceHandle(Helpers.GetValueLittleEndian(span.Slice(2 + memberForwardedTagRefSize + stringHandleSize, moduleRefSize)));
			}
		}

		struct ImplMapEntry
		{
			readonly MetadataFile metadataFile;
			readonly ImplMap implMap;

			public int RID { get; }

			public int Token => 0x1C000000 | RID;

			public int Offset { get; }

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public PInvokeAttributes MappingFlags => implMap.MappingFlags;

			const PInvokeAttributes otherFlagsMask = ~(PInvokeAttributes.CallConvMask | PInvokeAttributes.CharSetMask);

			public object MappingFlagsTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(PInvokeAttributes), "Character set: ", (int)PInvokeAttributes.CharSetMask, (int)(implMap.MappingFlags & PInvokeAttributes.CharSetMask), new Flag("CharSetNotSpec (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(PInvokeAttributes), "Calling convention: ", (int)PInvokeAttributes.CallConvMask, (int)(implMap.MappingFlags & PInvokeAttributes.CallConvMask), new Flag("Invalid (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(PInvokeAttributes), "Flags:", (int)otherFlagsMask, (int)(implMap.MappingFlags & otherFlagsMask), includeAll: false),
			};

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MemberForwarded => MetadataTokens.GetToken(implMap.MemberForwarded);

			public void OnMemberForwardedClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, implMap.MemberForwarded, protocol: "metadata")));
			}

			string memberForwardedTooltip;
			public string MemberForwardedTooltip => GenerateTooltip(ref memberForwardedTooltip, metadataFile, implMap.MemberForwarded);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ImportScope => MetadataTokens.GetToken(implMap.ImportScope);

			public void OnImportScopeClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, implMap.ImportScope, protocol: "metadata")));
			}

			string importScopeTooltip;
			public string ImportScopeTooltip => GenerateTooltip(ref importScopeTooltip, metadataFile, implMap.ImportScope);

			public string ImportName => metadataFile.Metadata.GetString(implMap.ImportName);

			public string ImportNameTooltip => $"{MetadataTokens.GetHeapOffset(implMap.ImportName):X} \"{ImportName}\"";

			public ImplMapEntry(MetadataFile metadataFile, ReadOnlySpan<byte> span, int row)
			{
				this.metadataFile = metadataFile;
				this.RID = row;
				var rowOffset = metadataFile.Metadata.GetTableMetadataOffset(TableIndex.ImplMap)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.ImplMap) * (row - 1);
				this.Offset = metadataFile.MetadataOffset + rowOffset;
				int moduleRefSize = metadataFile.Metadata.GetTableRowCount(TableIndex.ModuleRef) < ushort.MaxValue ? 2 : 4;
				int memberForwardedTagRefSize = metadataFile.Metadata.ComputeCodedTokenSize(32768, TableMask.MethodDef | TableMask.Field);
				int stringHandleSize = metadataFile.Metadata.GetHeapSize(HeapIndex.String) < ushort.MaxValue ? 2 : 4;
				this.implMap = new ImplMap(span.Slice(rowOffset), moduleRefSize, memberForwardedTagRefSize, stringHandleSize);
				this.importScopeTooltip = null;
				this.memberForwardedTooltip = null;
			}
		}
	}
}
