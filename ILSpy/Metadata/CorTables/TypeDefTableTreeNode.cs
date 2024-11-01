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
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class TypeDefTableTreeNode : MetadataTableTreeNode
	{
		public TypeDefTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.TypeDef, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = metadataFile.Metadata;

			var list = new List<TypeDefEntry>();
			TypeDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.TypeDefinitions)
			{
				TypeDefEntry entry = new TypeDefEntry(metadataFile, row);
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

		struct TypeDefEntry : IMemberTreeNode
		{
			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle handle;
			readonly TypeDefinition typeDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.TypeDef)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.TypeDef) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.Other)]
			public TypeAttributes Attributes => typeDef.Attributes;

			const TypeAttributes otherFlagsMask = ~(TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.ClassSemanticsMask | TypeAttributes.StringFormatMask | TypeAttributes.CustomFormatMask);

			public object AttributesTooltip => new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Visibility: ", (int)TypeAttributes.VisibilityMask, (int)(typeDef.Attributes & TypeAttributes.VisibilityMask), new Flag("NotPublic (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Class layout: ", (int)TypeAttributes.LayoutMask, (int)(typeDef.Attributes & TypeAttributes.LayoutMask), new Flag("AutoLayout (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Class semantics: ", (int)TypeAttributes.ClassSemanticsMask, (int)(typeDef.Attributes & TypeAttributes.ClassSemanticsMask), new Flag("Class (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "String format: ", (int)TypeAttributes.StringFormatMask, (int)(typeDef.Attributes & TypeAttributes.StringFormatMask), new Flag("AnsiClass (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Custom format: ", (int)TypeAttributes.CustomFormatMask, (int)(typeDef.Attributes & TypeAttributes.CustomFormatMask), new Flag("Value0 (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(TypeAttributes), "Flags:", (int)otherFlagsMask, (int)(typeDef.Attributes & otherFlagsMask), includeAll: false),
			};

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(typeDef.Name):X} \"{Name}\"";

			public string Name => metadataFile.Metadata.GetString(typeDef.Name);

			public string NamespaceTooltip => $"{MetadataTokens.GetHeapOffset(typeDef.Namespace):X} \"{Namespace}\"";

			public string Namespace => metadataFile.Metadata.GetString(typeDef.Namespace);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int BaseType => MetadataTokens.GetToken(typeDef.BaseType);

			public void OnBaseTypeClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, typeDef.BaseType, protocol: "metadata")));
			}

			public string BaseTypeTooltip {
				get {
					var output = new PlainTextOutput();
					var provider = new DisassemblerSignatureTypeProvider(metadataFile, output);
					if (typeDef.BaseType.IsNil)
						return null;
					switch (typeDef.BaseType.Kind)
					{
						case HandleKind.TypeDefinition:
							provider.GetTypeFromDefinition(metadataFile.Metadata, (TypeDefinitionHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeReference:
							provider.GetTypeFromReference(metadataFile.Metadata, (TypeReferenceHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeSpecification:
							provider.GetTypeFromSpecification(metadataFile.Metadata, new Decompiler.Metadata.MetadataGenericContext(default(TypeDefinitionHandle), metadataFile.Metadata), (TypeSpecificationHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						default:
							return null;
					}
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int FieldList => MetadataTokens.GetToken(typeDef.GetFields().FirstOrDefault());

			public void OnFieldListClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, typeDef.GetFields().FirstOrDefault(), protocol: "metadata")));
			}

			string fieldListTooltip;
			public string FieldListTooltip {
				get {
					var field = typeDef.GetFields().FirstOrDefault();
					if (field.IsNil)
						return null;
					return GenerateTooltip(ref fieldListTooltip, metadataFile, field);
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MethodList => MetadataTokens.GetToken(typeDef.GetMethods().FirstOrDefault());

			public void OnMethodListClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, typeDef.GetMethods().FirstOrDefault(), protocol: "metadata")));
			}

			string methodListTooltip;
			public string MethodListTooltip {
				get {
					var method = typeDef.GetMethods().FirstOrDefault();
					if (method.IsNil)
						return null;
					return GenerateTooltip(ref methodListTooltip, metadataFile, method);
				}
			}

			IEntity IMemberTreeNode.Member => ((MetadataModule)metadataFile.GetTypeSystemWithCurrentOptionsOrNull(SettingsService)?.MainModule)?.GetDefinition(handle);

			public TypeDefEntry(MetadataFile metadataFile, TypeDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.typeDef = metadataFile.Metadata.GetTypeDefinition(handle);
				this.methodListTooltip = null;
				this.fieldListTooltip = null;
			}
		}
	}
}