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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the GenericParamConstraint table — bounds applied to generic parameters
	/// (<c>where T : SomeBase, IEnumerable&lt;X&gt;</c>). Each row pairs a GenericParam
	/// owner with a TypeDefOrRef coded token naming the constraining type.
	/// </summary>
	public sealed class GenericParamConstraintTableTreeNode : MetadataTableTreeNode<GenericParamConstraintTableTreeNode.GenericParamConstraintEntry>
	{
		public GenericParamConstraintTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.GenericParamConstraint, metadataFile)
		{
		}

		protected override IReadOnlyList<GenericParamConstraintEntry> LoadTable()
		{
			var list = new List<GenericParamConstraintEntry>();
			var metadata = metadataFile.Metadata;
			for (int row = 1; row <= metadata.GetTableRowCount(TableIndex.GenericParamConstraint); row++)
				list.Add(new GenericParamConstraintEntry(metadataFile, MetadataTokens.GenericParameterConstraintHandle(row)));
			return list;
		}

		public sealed class GenericParamConstraintEntry
		{
			readonly MetadataFile metadataFile;
			readonly GenericParameterConstraintHandle handle;
			readonly GenericParameterConstraint genericParamConstraint;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.GenericParamConstraint, RID);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Owner => MetadataTokens.GetToken(genericParamConstraint.Parameter);

			string? ownerTooltip;
			public string? OwnerTooltip {
				get {
					if (ownerTooltip == null)
					{
						ITextOutput output = new PlainTextOutput();
						var p = metadataFile.Metadata.GetGenericParameter(genericParamConstraint.Parameter);
						output.Write("parameter " + p.Index + (p.Name.IsNil ? "" : " (" + metadataFile.Metadata.GetString(p.Name) + ")") + " of ");
						p.Parent.WriteTo(metadataFile, output, default);
						ownerTooltip = output.ToString();
					}
					return ownerTooltip;
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Type => MetadataTokens.GetToken(genericParamConstraint.Type);

			string? typeTooltip;
			public string? TypeTooltip => GenerateTooltip(ref typeTooltip, metadataFile, genericParamConstraint.Type);

			public GenericParamConstraintEntry(MetadataFile metadataFile, GenericParameterConstraintHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				genericParamConstraint = metadataFile.Metadata.GetGenericParameterConstraint(handle);
			}
		}
	}
}
