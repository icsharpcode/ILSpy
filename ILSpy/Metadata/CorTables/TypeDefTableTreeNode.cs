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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Controls;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class TypeDefTableTreeNode : ILSpyTreeNode
	{
		private PEFile module;

		public TypeDefTableTreeNode(PEFile module)
		{
			this.module = module;
		}

		public override object Text => $"02 TypeDef ({module.Metadata.GetTableRowCount(TableIndex.TypeDef)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;

			var list = new List<TypeDefEntry>();

			foreach (var row in metadata.TypeDefinitions)
				list.Add(new TypeDefEntry(module, row));

			view.ItemsSource = list;

			tabPage.Content = view;
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
				+ metadata.GetTableRowSize(TableIndex.TypeDef) * (RID-1);

			public int Attributes => (int)typeDef.Attributes;

			public string AttributesTooltip => Helpers.AttributesToString(typeDef.Attributes);

			public int NameStringHandle => MetadataTokens.GetHeapOffset(typeDef.Name);

			public string Name => metadata.GetString(typeDef.Name);

			public int NamespaceStringHandle => MetadataTokens.GetHeapOffset(typeDef.Namespace);

			public string Namespace => metadata.GetString(typeDef.Namespace);

			public int BaseType => MetadataTokens.GetToken(typeDef.BaseType);

			public string BaseTypeSignature {
				get {
					var output = new PlainTextOutput();
					var provider = new DisassemblerSignatureTypeProvider(module, output);
					if (typeDef.BaseType.IsNil)
						return null;
					switch (typeDef.BaseType.Kind) {
						case HandleKind.TypeDefinition:
							provider.GetTypeFromDefinition(module.Metadata, (TypeDefinitionHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeReference:
							provider.GetTypeFromReference(module.Metadata, (TypeReferenceHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						case HandleKind.TypeSpecification:
							provider.GetTypeFromSpecification(module.Metadata, new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module), (TypeSpecificationHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						default:
							return null;
					}
				}
			}

			public int FieldList => MetadataTokens.GetToken(typeDef.GetFields().FirstOrDefault());

			public int MethodList => MetadataTokens.GetToken(typeDef.GetMethods().FirstOrDefault());

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemOrNull()?.MainModule).GetDefinition(handle);

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