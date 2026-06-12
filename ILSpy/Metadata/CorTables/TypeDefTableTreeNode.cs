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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the TypeDef table — every type the module defines. The first row is the
	/// pseudo-type &lt;Module&gt;, owning module-scoped fields and methods. Each row carries
	/// attributes (visibility, layout, semantics), the optional base type, and pointers
	/// into the FieldList / MethodList for the type's members.
	/// </summary>
	public sealed class TypeDefTableTreeNode : MetadataTableTreeNode<TypeDefTableTreeNode.TypeDefEntry>
	{
		public TypeDefTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.TypeDef, metadataFile)
		{
		}

		protected override IReadOnlyList<TypeDefEntry> LoadTable()
		{
			var list = new List<TypeDefEntry>();
			foreach (var row in metadataFile.Metadata.TypeDefinitions)
				list.Add(new TypeDefEntry(metadataFile, row));
			return list;
		}

		public sealed class TypeDefEntry
		{
			readonly MetadataFile metadataFile;
			readonly TypeDefinitionHandle handle;
			readonly TypeDefinition typeDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.TypeDef, RID);

			[ColumnInfo("X8")]
			public TypeAttributes Attributes => typeDef.Attributes;

			public object AttributesTooltip => FlagsTooltip.ForTypeAttributes(typeDef.Attributes);

			public string Name => metadataFile.Metadata.GetString(typeDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(typeDef.Name):X} \"{Name}\"";

			public string Namespace => metadataFile.Metadata.GetString(typeDef.Namespace);

			public string NamespaceTooltip => $"{MetadataTokens.GetHeapOffset(typeDef.Namespace):X} \"{Namespace}\"";

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int BaseType => MetadataTokens.GetToken(typeDef.BaseType);

			public string? BaseTypeTooltip {
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
							provider.GetTypeFromSpecification(metadataFile.Metadata, new MetadataGenericContext(default(TypeDefinitionHandle), metadataFile.Metadata), (TypeSpecificationHandle)typeDef.BaseType, 0)(ILNameSyntax.Signature);
							return output.ToString();
						default:
							return null;
					}
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int FieldList => MetadataTokens.GetToken(typeDef.GetFields().FirstOrDefault());

			string? fieldListTooltip;
			public string? FieldListTooltip {
				get {
					var @field = typeDef.GetFields().FirstOrDefault();
					if (@field.IsNil)
						return null;
					return GenerateTooltip(ref fieldListTooltip, metadataFile, @field);
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int MethodList => MetadataTokens.GetToken(typeDef.GetMethods().FirstOrDefault());

			string? methodListTooltip;
			public string? MethodListTooltip {
				get {
					var method = typeDef.GetMethods().FirstOrDefault();
					if (method.IsNil)
						return null;
					return GenerateTooltip(ref methodListTooltip, metadataFile, method);
				}
			}

			public TypeDefEntry(MetadataFile metadataFile, TypeDefinitionHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				typeDef = metadataFile.Metadata.GetTypeDefinition(handle);
			}
		}
	}
}
