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

using System;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ILSpy.Languages;
using ILSpy.Metadata.CorTables;
using ILSpy.TreeNodes;

namespace ILSpy.Metadata
{
	/// <summary>
	/// Synthetic "Tables" container nested under each MetadataTreeNode. Lazily expands into
	/// one leaf per non-empty CLI metadata table so users can drill into rows without
	/// scrolling past 50 zero-row entries (#Strings &amp; co. live as siblings, not children).
	/// </summary>
	public sealed class MetadataTablesTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile metadataFile;

		public MetadataTablesTreeNode(MetadataFile metadataFile)
		{
			this.metadataFile = metadataFile ?? throw new ArgumentNullException(nameof(metadataFile));
			LazyLoading = true;
		}

		public override object Text => "Tables";
		public override object Icon => Images.Images.MetadataTable;
		public override string ToString() => "Tables";

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Tables");
			var metadata = metadataFile.Metadata;
			foreach (var table in Enum.GetValues<TableIndex>())
			{
				int count = metadata.GetTableRowCount(table);
				if (count > 0)
					language.WriteCommentLine(output, $"{(byte)table:X2} {table}: {count} rows");
			}
		}

		protected override void LoadChildren()
		{
			var metadata = metadataFile.Metadata;
			foreach (var table in Enum.GetValues<TableIndex>())
			{
				if (metadata.GetTableRowCount(table) > 0)
					Children.Add(CreateTableTreeNode(table, metadataFile));
			}
		}

		// Typed leaves are added in passes (1e-i, 1e-ii, 1e-iii); any table not yet ported
		// falls through to the universal placeholder so the navigation surface stays whole.
		internal static MetadataTableTreeNode CreateTableTreeNode(TableIndex table, MetadataFile metadataFile)
			=> table switch {
				TableIndex.Module => new ModuleTableTreeNode(metadataFile),
				TableIndex.TypeRef => new TypeRefTableTreeNode(metadataFile),
				TableIndex.TypeDef => new TypeDefTableTreeNode(metadataFile),
				TableIndex.Field => new FieldTableTreeNode(metadataFile),
				TableIndex.MethodDef => new MethodTableTreeNode(metadataFile),
				TableIndex.Param => new ParamTableTreeNode(metadataFile),
				TableIndex.MemberRef => new MemberRefTableTreeNode(metadataFile),
				TableIndex.Constant => new ConstantTableTreeNode(metadataFile),
				TableIndex.CustomAttribute => new CustomAttributeTableTreeNode(metadataFile),
				TableIndex.Event => new EventTableTreeNode(metadataFile),
				TableIndex.Property => new PropertyTableTreeNode(metadataFile),
				TableIndex.ModuleRef => new ModuleRefTableTreeNode(metadataFile),
				TableIndex.TypeSpec => new TypeSpecTableTreeNode(metadataFile),
				TableIndex.Assembly => new AssemblyTableTreeNode(metadataFile),
				TableIndex.AssemblyRef => new AssemblyRefTableTreeNode(metadataFile),
				TableIndex.ExportedType => new ExportedTypeTableTreeNode(metadataFile),
				TableIndex.ManifestResource => new ManifestResourceTableTreeNode(metadataFile),
				TableIndex.GenericParam => new GenericParamTableTreeNode(metadataFile),
				TableIndex.MethodSpec => new MethodSpecTableTreeNode(metadataFile),
				TableIndex.GenericParamConstraint => new GenericParamConstraintTableTreeNode(metadataFile),
				TableIndex.StandAloneSig => new StandAloneSigTableTreeNode(metadataFile),
				TableIndex.DeclSecurity => new DeclSecurityTableTreeNode(metadataFile),
				TableIndex.File => new FileTableTreeNode(metadataFile),
				_ => new UnsupportedMetadataTableTreeNode(table, metadataFile),
			};
	}
}
