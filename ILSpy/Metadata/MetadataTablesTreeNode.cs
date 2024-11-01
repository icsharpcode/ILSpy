// Copyright (c) 2021 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	class MetadataTablesTreeNode : ILSpyTreeNode
	{
		readonly MetadataFile metadataFile;

		public MetadataTablesTreeNode(MetadataFile metadataFile)
		{
			this.metadataFile = metadataFile;
			this.LazyLoading = true;
		}

		public override object Text => "Tables";

		public override object Icon => Images.MetadataTableGroup;

		protected override void LoadChildren()
		{
			foreach (var table in Enum.GetValues<TableIndex>())
			{
				if (ShowTable(table, metadataFile.Metadata))
					this.Children.Add(CreateTableTreeNode(table, metadataFile));
			}
		}

		internal static bool ShowTable(TableIndex table, MetadataReader metadata) => !SettingsService.DisplaySettings.HideEmptyMetadataTables || metadata.GetTableRowCount(table) > 0;

		internal static MetadataTableTreeNode CreateTableTreeNode(TableIndex table, MetadataFile metadataFile)
		{
			switch (table)
			{
				case TableIndex.Module:
					return new ModuleTableTreeNode(metadataFile);
				case TableIndex.TypeRef:
					return new TypeRefTableTreeNode(metadataFile);
				case TableIndex.TypeDef:
					return new TypeDefTableTreeNode(metadataFile);
				case TableIndex.Field:
					return new FieldTableTreeNode(metadataFile);
				case TableIndex.MethodDef:
					return new MethodTableTreeNode(metadataFile);
				case TableIndex.Param:
					return new ParamTableTreeNode(metadataFile);
				case TableIndex.InterfaceImpl:
					return new InterfaceImplTableTreeNode(metadataFile);
				case TableIndex.MemberRef:
					return new MemberRefTableTreeNode(metadataFile);
				case TableIndex.Constant:
					return new ConstantTableTreeNode(metadataFile);
				case TableIndex.CustomAttribute:
					return new CustomAttributeTableTreeNode(metadataFile);
				case TableIndex.FieldMarshal:
					return new FieldMarshalTableTreeNode(metadataFile);
				case TableIndex.DeclSecurity:
					return new DeclSecurityTableTreeNode(metadataFile);
				case TableIndex.ClassLayout:
					return new ClassLayoutTableTreeNode(metadataFile);
				case TableIndex.FieldLayout:
					return new FieldLayoutTableTreeNode(metadataFile);
				case TableIndex.StandAloneSig:
					return new StandAloneSigTableTreeNode(metadataFile);
				case TableIndex.EventMap:
					return new EventMapTableTreeNode(metadataFile);
				case TableIndex.Event:
					return new EventTableTreeNode(metadataFile);
				case TableIndex.PropertyMap:
					return new PropertyMapTableTreeNode(metadataFile);
				case TableIndex.Property:
					return new PropertyTableTreeNode(metadataFile);
				case TableIndex.MethodSemantics:
					return new MethodSemanticsTableTreeNode(metadataFile);
				case TableIndex.MethodImpl:
					return new MethodImplTableTreeNode(metadataFile);
				case TableIndex.ModuleRef:
					return new ModuleRefTableTreeNode(metadataFile);
				case TableIndex.TypeSpec:
					return new TypeSpecTableTreeNode(metadataFile);
				case TableIndex.ImplMap:
					return new ImplMapTableTreeNode(metadataFile);
				case TableIndex.FieldRva:
					return new FieldRVATableTreeNode(metadataFile);
				case TableIndex.Assembly:
					return new AssemblyTableTreeNode(metadataFile);
				case TableIndex.AssemblyRef:
					return new AssemblyRefTableTreeNode(metadataFile);
				case TableIndex.File:
					return new FileTableTreeNode(metadataFile);
				case TableIndex.ExportedType:
					return new ExportedTypeTableTreeNode(metadataFile);
				case TableIndex.ManifestResource:
					return new ManifestResourceTableTreeNode(metadataFile);
				case TableIndex.NestedClass:
					return new NestedClassTableTreeNode(metadataFile);
				case TableIndex.GenericParam:
					return new GenericParamTableTreeNode(metadataFile);
				case TableIndex.MethodSpec:
					return new MethodSpecTableTreeNode(metadataFile);
				case TableIndex.GenericParamConstraint:
					return new GenericParamConstraintTableTreeNode(metadataFile);
				case TableIndex.Document:
					return new DocumentTableTreeNode(metadataFile);
				case TableIndex.MethodDebugInformation:
					return new MethodDebugInformationTableTreeNode(metadataFile);
				case TableIndex.LocalScope:
					return new LocalScopeTableTreeNode(metadataFile);
				case TableIndex.LocalVariable:
					return new LocalVariableTableTreeNode(metadataFile);
				case TableIndex.LocalConstant:
					return new LocalConstantTableTreeNode(metadataFile);
				case TableIndex.ImportScope:
					return new ImportScopeTableTreeNode(metadataFile);
				case TableIndex.StateMachineMethod:
					return new StateMachineMethodTableTreeNode(metadataFile);
				case TableIndex.CustomDebugInformation:
					return new CustomDebugInformationTableTreeNode(metadataFile);
				case TableIndex.FieldPtr:
				case TableIndex.EventPtr:
				case TableIndex.MethodPtr:
				case TableIndex.ParamPtr:
				case TableIndex.PropertyPtr:
					return new PtrTableTreeNode(table, metadataFile);
				default:
					return new UnsupportedMetadataTableTreeNode(table, metadataFile);
			}
		}

		public override bool View(TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			return false;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Metadata Tables");
		}
	}
}
