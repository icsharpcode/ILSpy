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
	/// View of the MethodSpec table — concrete instantiations of generic methods like
	/// <c>List&lt;int&gt;.Add</c>. Each row points back at the open generic method
	/// (MethodDef or MemberRef) and carries a blob holding the type-argument list.
	/// </summary>
	public sealed class MethodSpecTableTreeNode : MetadataTableTreeNode<MethodSpecTableTreeNode.MethodSpecEntry>
	{
		public MethodSpecTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.MethodSpec, metadataFile)
		{
		}

		protected override IReadOnlyList<MethodSpecEntry> LoadTable()
		{
			var list = new List<MethodSpecEntry>();
			foreach (var row in metadataFile.Metadata.GetMethodSpecifications())
				list.Add(new MethodSpecEntry(metadataFile, row));
			return list;
		}

		public sealed class MethodSpecEntry
		{
			readonly MetadataFile metadataFile;
			readonly MethodSpecificationHandle handle;
			readonly MethodSpecification methodSpec;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.MethodSpec, RID);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Method => MetadataTokens.GetToken(methodSpec.Method);

			string? methodTooltip;
			public string? MethodTooltip => GenerateTooltip(ref methodTooltip, metadataFile, methodSpec.Method);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(methodSpec.Signature);

			public string? SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var signature = methodSpec.DecodeSignature(new DisassemblerSignatureTypeProvider(metadataFile, output), default);
					bool first = true;
					foreach (var type in signature)
					{
						if (first)
							first = false;
						else
							output.Write(", ");
						type(ILNameSyntax.TypeName);
					}
					return output.ToString();
				}
			}

			public MethodSpecEntry(MetadataFile metadataFile, MethodSpecificationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				methodSpec = metadataFile.Metadata.GetMethodSpecification(handle);
			}
		}
	}
}
