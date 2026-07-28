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
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the MethodDef table — every method body the module ships. The RVA points
	/// into the .text section's IL stream (0 for abstract / interface methods); the
	/// ParamList token is the first row in the Param table covering this method's parameters.
	/// </summary>
	public sealed class MethodTableTreeNode : MetadataTableTreeNode<MethodTableTreeNode.MethodDefEntry>
	{
		public MethodTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodDef, metadataFile)
		{
		}

		protected override IReadOnlyList<MethodDefEntry> LoadTable()
		{
			var list = new List<MethodDefEntry>();
			var metadata = metadataFile.Metadata;
			// ParamList is read from the raw row: the computed range (GetParameters) is empty
			// for a parameterless method, but the stored value is the running list position
			// (the next method's first Param row, or one past the Param table's end), never 0.
			// It is the last column, so it sits at the end of the row; with a ParamPtr
			// indirection present, the column indexes the pointer table, whose row count also
			// governs the column width.
			var indexedTable = metadata.GetTableRowCount(TableIndex.ParamPtr) > 0 ? TableIndex.ParamPtr : TableIndex.Param;
			int paramListWidth = metadata.GetTableRowCount(indexedTable) <= ushort.MaxValue ? 2 : 4;
			int rowSize = metadata.GetTableRowSize(TableIndex.MethodDef);
			int tableOffset = metadata.GetTableMetadataOffset(TableIndex.MethodDef);
			var reader = metadata.AsBlobReader();
			foreach (var row in metadata.MethodDefinitions)
			{
				reader.Offset = tableOffset + rowSize * MetadataTokens.GetRowNumber(row) - paramListWidth;
				int paramList = paramListWidth == 2 ? reader.ReadUInt16() : reader.ReadInt32();
				list.Add(new MethodDefEntry(metadataFile, row, paramList));
			}
			return list;
		}

		public sealed class MethodDefEntry
		{
			readonly MetadataFile metadataFile;
			readonly MethodDefinitionHandle handle;
			readonly MethodDefinition methodDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.MethodDef, RID);

			[ColumnInfo("X8")]
			public MethodAttributes Attributes => methodDef.Attributes;

			const MethodAttributes otherFlagsMask = ~(MethodAttributes.MemberAccessMask | MethodAttributes.VtableLayoutMask);

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Member access: ", (int)MethodAttributes.MemberAccessMask, (int)(methodDef.Attributes & MethodAttributes.MemberAccessMask), new Flag("CompilerControlled (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodAttributes), "Vtable layout: ", (int)MethodAttributes.VtableLayoutMask, (int)(methodDef.Attributes & MethodAttributes.VtableLayoutMask), new Flag("ReuseSlot (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(MethodAttributes), "Flags:", (int)otherFlagsMask, (int)(methodDef.Attributes & otherFlagsMask), includeAll: false),
			};

			[ColumnInfo("X8")]
			public MethodImplAttributes ImplAttributes => methodDef.ImplAttributes;

			public object ImplAttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImplAttributes), "Code type: ", (int)MethodImplAttributes.CodeTypeMask, (int)(methodDef.ImplAttributes & MethodImplAttributes.CodeTypeMask), new Flag("IL (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(MethodImplAttributes), "Managed type: ", (int)MethodImplAttributes.ManagedMask, (int)(methodDef.ImplAttributes & MethodImplAttributes.ManagedMask), new Flag("Managed (0000)", 0, false), includeAny: false),
			};

			[ColumnInfo("X8")]
			public int RVA => methodDef.RelativeVirtualAddress;

			public string Name => metadataFile.Metadata.GetString(methodDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(methodDef.Name):X} \"{Name}\"";

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(methodDef.Signature);

			string? signatureTooltip;

			public string? SignatureTooltip => GenerateTooltip(ref signatureTooltip, metadataFile, handle);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ParamList => 0x08000000 | paramList;

			string? paramListTooltip;
			public string? ParamListTooltip {
				get {
					var param = methodDef.GetParameters().FirstOrDefault();
					if (param.IsNil)
						return "(method has no Param rows; the stored value is the start of its empty parameter list: the next method's first Param row, or one past the end of the Param table)";
					return GenerateTooltip(ref paramListTooltip, metadataFile, param);
				}
			}

			readonly int paramList;

			public MethodDefEntry(MetadataFile metadataFile, MethodDefinitionHandle handle, int paramList)
			{
				this.paramList = paramList;
				this.metadataFile = metadataFile;
				this.handle = handle;
				methodDef = metadataFile.Metadata.GetMethodDefinition(handle);
			}
		}
	}
}
