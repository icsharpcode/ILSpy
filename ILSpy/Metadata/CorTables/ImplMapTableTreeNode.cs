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
	/// View of the ImplMap table — P/Invoke linkage. Each row maps a managed MethodDef (or
	/// Field, in legacy CIL) to its native entry point: the <c>extern</c> name plus the
	/// ModuleRef holding the DLL identifier.
	/// </summary>
	public sealed class ImplMapTableTreeNode : MetadataTableTreeNode<ImplMapTableTreeNode.ImplMapEntry>
	{
		public ImplMapTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ImplMap, metadataFile)
		{
		}

		protected override IReadOnlyList<ImplMapEntry> LoadTable()
		{
			var list = new List<ImplMapEntry>();
			var metadata = metadataFile.Metadata;
			var length = metadata.GetTableRowCount(TableIndex.ImplMap);
			var reader = metadata.AsBlobReader();
			reader.Offset = metadata.GetTableMetadataOffset(TableIndex.ImplMap);
			int moduleRefSize = metadata.GetTableRowCount(TableIndex.ModuleRef) < ushort.MaxValue ? 2 : 4;
			int memberForwardedTagSize = metadata.ComputeCodedTokenSize(32768, TableMask.MethodDef | TableMask.Field);
			int stringHandleSize = metadata.GetHeapSize(HeapIndex.String) < ushort.MaxValue ? 2 : 4;
			for (int rid = 1; rid <= length; rid++)
			{
				MethodImportAttributes mappingFlags = (MethodImportAttributes)reader.ReadUInt16();
				uint memberForwardedTag = (uint)(memberForwardedTagSize == 2 ? reader.ReadUInt16() : reader.ReadInt32());
				int importNameOffset = stringHandleSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				int importScopeRow = moduleRefSize == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new ImplMapEntry(metadataFile, rid, mappingFlags,
					MetadataReaderHelpers.FromMemberForwardedTag(memberForwardedTag),
					MetadataTokens.StringHandle(importNameOffset),
					MetadataTokens.ModuleReferenceHandle(importScopeRow)));
			}
			return list;
		}

		public sealed class ImplMapEntry
		{
			public int RID { get; }

			[ColumnInfo("X8")]
			public int Token => 0x1C000000 | RID;

			[ColumnInfo("X8")]
			public MethodImportAttributes MappingFlags { get; }

			const MethodImportAttributes otherFlagsMask = ~(MethodImportAttributes.CallingConventionMask | MethodImportAttributes.CharSetMask);

			public object MappingFlagsTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImportAttributes), "Character set: ", (int)MethodImportAttributes.CharSetMask, (int)(MappingFlags & MethodImportAttributes.CharSetMask), new Flag("CharSetNotSpec (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImportAttributes), "Calling convention: ", (int)MethodImportAttributes.CallingConventionMask, (int)(MappingFlags & MethodImportAttributes.CallingConventionMask), new Flag("Invalid (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(MethodImportAttributes), "Flags:", (int)otherFlagsMask, (int)(MappingFlags & otherFlagsMask), includeAll: false),
			};

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MemberForwarded => MetadataTokens.GetToken(memberForwarded);

			string? memberForwardedTooltip;
			public string? MemberForwardedTooltip => GenerateTooltip(ref memberForwardedTooltip, metadataFile, memberForwarded);

			public string ImportName { get; }

			public string ImportNameTooltip => $"{MetadataTokens.GetHeapOffset(importNameHandle):X} \"{ImportName}\"";

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ImportScope => MetadataTokens.GetToken(importScope);

			string? importScopeTooltip;
			public string? ImportScopeTooltip => GenerateTooltip(ref importScopeTooltip, metadataFile, importScope);

			readonly MetadataFile metadataFile;
			readonly EntityHandle memberForwarded;
			readonly StringHandle importNameHandle;
			readonly ModuleReferenceHandle importScope;

			public ImplMapEntry(MetadataFile metadataFile, int rid, MethodImportAttributes mappingFlags, EntityHandle memberForwarded, StringHandle importNameHandle, ModuleReferenceHandle importScope)
			{
				this.metadataFile = metadataFile;
				RID = rid;
				MappingFlags = mappingFlags;
				this.memberForwarded = memberForwarded;
				this.importNameHandle = importNameHandle;
				ImportName = metadataFile.Metadata.GetString(importNameHandle);
				this.importScope = importScope;
			}
		}
	}
}
