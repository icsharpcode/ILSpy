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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class TypeSpecTableTreeNode : MetadataTableTreeNode<TypeSpecTableTreeNode.TypeSpecEntry>
	{
		public TypeSpecTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.TypeSpec, metadataFile)
		{
		}

		protected override IReadOnlyList<TypeSpecEntry> LoadTable()
		{
			var list = new List<TypeSpecEntry>();
			foreach (var row in metadataFile.Metadata.GetTypeSpecifications())
			{
				list.Add(new TypeSpecEntry(metadataFile, row));
			}
			return list;
		}

		internal struct TypeSpecEntry
		{
			readonly int metadataOffset;
			readonly MetadataFile module;
			readonly MetadataReader metadata;
			readonly TypeSpecificationHandle handle;
			readonly TypeSpecification typeSpec;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.TypeSpec)
				+ metadata.GetTableRowSize(TableIndex.TypeSpec) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(typeSpec.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					typeSpec.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), default)(ILNameSyntax.Signature);
					return output.ToString();
				}
			}

			public TypeSpecEntry(MetadataFile metadataFile, TypeSpecificationHandle handle)
			{
				this.module = metadataFile;
				this.metadataOffset = metadataFile.MetadataOffset;
				this.metadata = metadataFile.Metadata;
				this.handle = handle;
				this.typeSpec = metadata.GetTypeSpecification(handle);
			}
		}
	}
}