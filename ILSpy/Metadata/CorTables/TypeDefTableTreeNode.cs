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
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class TypeDefTableTreeNode : MetadataTableTreeNode
	{
		public TypeDefTableTreeNode(PEFile module)
			: base(HandleKind.TypeDefinition, module)
		{
		}

		public override object Text => $"02 TypeDef ({module.Metadata.GetTableRowCount(TableIndex.TypeDef)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var metadata = module.Metadata;

			var list = new List<TypeDefEntry>();
			TypeDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.TypeDefinitions)
			{
				TypeDefEntry entry = new TypeDefEntry(module, row);
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
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly TypeDefinitionHandle handle;
			readonly TypeDefinition typeDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.TypeDef)
				+ metadata.GetTableRowSize(TableIndex.TypeDef) * (RID - 1);

			[StringFormat("X8")]
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

			public string Name => metadata.GetString(typeDef.Name);

			public string NamespaceTooltip => $"{MetadataTokens.GetHeapOffset(typeDef.Namespace):X} \"{Namespace}\"";

			public string Namespace => metadata.GetString(typeDef.Namespace);

			[StringFormat("X8")]
			[LinkToTable]
			public int BaseType => MetadataTokens.GetToken(typeDef.BaseType);

			public void OnBaseTypeClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, typeDef.BaseType, protocol: "metadata"));
			}

			public string BaseTypeTooltip {
				get {
					var output = new PlainTextOutput();
					var provider = new DisassemblerSignatureTypeProvider(module, output);
					if (typeDef.BaseType.IsNil)
						return null;
					switch (typeDef.BaseType.Kind)
					{
						case HandleKind.TypeDefinition:
							provider.GetTypeFromDefinition(module.Metadata, (TypeDefinitionHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeReference:
							provider.GetTypeFromReference(module.Metadata, (TypeReferenceHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeSpecification:
							provider.GetTypeFromSpecification(module.Metadata, new Decompiler.Metadata.MetadataGenericContext(default(TypeDefinitionHandle), module), (TypeSpecificationHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						default:
							return null;
					}
				}
			}

			[StringFormat("X8")]
			[LinkToTable]
			public int FieldList => MetadataTokens.GetToken(typeDef.GetFields().FirstOrDefault());

			public void OnFieldListClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, typeDef.GetFields().FirstOrDefault(), protocol: "metadata"));
			}

			public string FieldListTooltip {
				get {
					var field = typeDef.GetFields().FirstOrDefault();
					if (field.IsNil)
						return null;
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.MetadataGenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)field).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			[LinkToTable]
			public int MethodList => MetadataTokens.GetToken(typeDef.GetMethods().FirstOrDefault());

			public void OnMethodListClick()
			{
				MainWindow.Instance.JumpToReference(new EntityReference(module, typeDef.GetMethods().FirstOrDefault(), protocol: "metadata"));
			}

			public string MethodListTooltip {
				get {
					var method = typeDef.GetMethods().FirstOrDefault();
					if (method.IsNil)
						return null;
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.MetadataGenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)method).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemWithCurrentOptionsOrNull()?.MainModule).GetDefinition(handle);

			public TypeDefEntry(PEFile module, TypeDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.typeDef = metadata.GetTypeDefinition(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "TypeDefs");
		}
	}
}