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
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.CorTables
{
	/// <summary>
	/// View of the TypeSpec table — instantiated generic types and other constructed type
	/// shapes (arrays, pointers, modreq/modopt) referenced from instructions and signatures.
	/// Each row points at a blob signature describing the constructed type.
	/// </summary>
	public sealed class TypeSpecTableTreeNode : MetadataTableTreeNode<TypeSpecTableTreeNode.TypeSpecEntry>
	{
		public TypeSpecTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.TypeSpec, metadataFile)
		{
		}

		protected override IReadOnlyList<TypeSpecEntry> LoadTable()
		{
			var list = new List<TypeSpecEntry>();
			foreach (var row in metadataFile.Metadata.GetTypeSpecifications())
				list.Add(new TypeSpecEntry(metadataFile, row));
			return list;
		}

		public sealed class TypeSpecEntry
		{
			readonly MetadataFile metadataFile;
			readonly TypeSpecificationHandle handle;
			readonly TypeSpecification typeSpec;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.TypeSpec, RID);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(typeSpec.Signature);

			public string? SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					typeSpec.DecodeSignature(new DisassemblerSignatureTypeProvider(metadataFile, output), default)(ILNameSyntax.Signature);
					return output.ToString();
				}
			}

			public TypeSpecEntry(MetadataFile metadataFile, TypeSpecificationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				typeSpec = metadataFile.Metadata.GetTypeSpecification(handle);
			}
		}
	}
}
