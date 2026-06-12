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
	/// View of the GenericParam table — every type or method generic-parameter slot. Owner
	/// is a TypeOrMethodDef coded token; Number is the parameter's 0-based index inside
	/// its owner's type-parameter list.
	/// </summary>
	public sealed class GenericParamTableTreeNode : MetadataTableTreeNode<GenericParamTableTreeNode.GenericParamEntry>
	{
		public GenericParamTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.GenericParam, metadataFile)
		{
		}

		protected override IReadOnlyList<GenericParamEntry> LoadTable()
		{
			var list = new List<GenericParamEntry>();
			for (int row = 1; row <= metadataFile.Metadata.GetTableRowCount(TableIndex.GenericParam); row++)
				list.Add(new GenericParamEntry(metadataFile, MetadataTokens.GenericParameterHandle(row)));
			return list;
		}

		public sealed class GenericParamEntry
		{
			readonly MetadataFile metadataFile;
			readonly GenericParameterHandle handle;
			readonly GenericParameter genericParam;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.GenericParam, RID);

			public int Number => genericParam.Index;

			[ColumnInfo("X8")]
			public GenericParameterAttributes Attributes => genericParam.Attributes;

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(GenericParameterAttributes), "Variance: ", (int)GenericParameterAttributes.VarianceMask, (int)(genericParam.Attributes & GenericParameterAttributes.VarianceMask), new Flag("None (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(GenericParameterAttributes), "Special Constraint: ", (int)GenericParameterAttributes.SpecialConstraintMask, (int)(genericParam.Attributes & GenericParameterAttributes.SpecialConstraintMask), includeAll: false),
			};

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Owner => MetadataTokens.GetToken(genericParam.Parent);

			string? ownerTooltip;
			public string? OwnerTooltip => GenerateTooltip(ref ownerTooltip, metadataFile, genericParam.Parent);

			public string Name => metadataFile.Metadata.GetString(genericParam.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(genericParam.Name):X} \"{Name}\"";

			public GenericParamEntry(MetadataFile metadataFile, GenericParameterHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				genericParam = metadataFile.Metadata.GetGenericParameter(handle);
			}
		}
	}
}
