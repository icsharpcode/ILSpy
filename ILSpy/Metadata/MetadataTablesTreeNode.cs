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

		public override object Icon => Images.Literal;

		protected override void LoadChildren()
		{
			if (ShowTable(TableIndex.Module, metadataFile.Metadata))
				this.Children.Add(new ModuleTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.TypeRef, metadataFile.Metadata))
				this.Children.Add(new TypeRefTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.TypeDef, metadataFile.Metadata))
				this.Children.Add(new TypeDefTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.Field, metadataFile.Metadata))
				this.Children.Add(new FieldTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.MethodDef, metadataFile.Metadata))
				this.Children.Add(new MethodTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.Param, metadataFile.Metadata))
				this.Children.Add(new ParamTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.InterfaceImpl, metadataFile.Metadata))
				this.Children.Add(new InterfaceImplTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.MemberRef, metadataFile.Metadata))
				this.Children.Add(new MemberRefTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.Constant, metadataFile.Metadata))
				this.Children.Add(new ConstantTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.CustomAttribute, metadataFile.Metadata))
				this.Children.Add(new CustomAttributeTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.FieldMarshal, metadataFile.Metadata))
				this.Children.Add(new FieldMarshalTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.DeclSecurity, metadataFile.Metadata))
				this.Children.Add(new DeclSecurityTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.ClassLayout, metadataFile.Metadata))
				this.Children.Add(new ClassLayoutTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.FieldLayout, metadataFile.Metadata))
				this.Children.Add(new FieldLayoutTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.StandAloneSig, metadataFile.Metadata))
				this.Children.Add(new StandAloneSigTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.EventMap, metadataFile.Metadata))
				this.Children.Add(new EventMapTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.Event, metadataFile.Metadata))
				this.Children.Add(new EventTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.PropertyMap, metadataFile.Metadata))
				this.Children.Add(new PropertyMapTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.Property, metadataFile.Metadata))
				this.Children.Add(new PropertyTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.MethodSemantics, metadataFile.Metadata))
				this.Children.Add(new MethodSemanticsTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.MethodImpl, metadataFile.Metadata))
				this.Children.Add(new MethodImplTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.ModuleRef, metadataFile.Metadata))
				this.Children.Add(new ModuleRefTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.TypeSpec, metadataFile.Metadata))
				this.Children.Add(new TypeSpecTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.ImplMap, metadataFile.Metadata))
				this.Children.Add(new ImplMapTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.FieldRva, metadataFile.Metadata))
				this.Children.Add(new FieldRVATableTreeNode(metadataFile));
			if (ShowTable(TableIndex.Assembly, metadataFile.Metadata))
				this.Children.Add(new AssemblyTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.AssemblyRef, metadataFile.Metadata))
				this.Children.Add(new AssemblyRefTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.File, metadataFile.Metadata))
				this.Children.Add(new FileTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.ExportedType, metadataFile.Metadata))
				this.Children.Add(new ExportedTypeTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.ManifestResource, metadataFile.Metadata))
				this.Children.Add(new ManifestResourceTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.NestedClass, metadataFile.Metadata))
				this.Children.Add(new NestedClassTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.GenericParam, metadataFile.Metadata))
				this.Children.Add(new GenericParamTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.MethodSpec, metadataFile.Metadata))
				this.Children.Add(new MethodSpecTableTreeNode(metadataFile));
			if (ShowTable(TableIndex.GenericParamConstraint, metadataFile.Metadata))
				this.Children.Add(new GenericParamConstraintTableTreeNode(metadataFile));
		}

		internal static bool ShowTable(TableIndex table, MetadataReader metadata) => !MainWindow.Instance.CurrentDisplaySettings.HideEmptyMetadataTables || metadata.GetTableRowCount(table) > 0;

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
				default:
					throw new ArgumentException($"Unsupported table index: {table}");
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
