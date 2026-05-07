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

using ICSharpCode.Decompiler.Metadata;

namespace ILSpy.Metadata.DebugTables
{
	/// <summary>
	/// View of the Document table — every source file referenced by debug info. Each row
	/// carries the path string, plus heap-offsets for hash algorithm, hash bytes, and
	/// language GUID.
	/// </summary>
	public sealed class DocumentTableTreeNode : MetadataTableTreeNode<DocumentTableTreeNode.DocumentEntry>
	{
		public DocumentTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.Document, metadataFile)
		{
		}

		protected override IReadOnlyList<DocumentEntry> LoadTable()
		{
			var list = new List<DocumentEntry>();
			foreach (var row in metadataFile.Metadata.Documents)
				list.Add(new DocumentEntry(metadataFile, row));
			return list;
		}

		public sealed class DocumentEntry
		{
			readonly MetadataFile metadataFile;
			readonly DocumentHandle handle;
			readonly Document document;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			public string Name => metadataFile.Metadata.GetString(document.Name);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int HashAlgorithm => MetadataTokens.GetHeapOffset(document.HashAlgorithm);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Hash => MetadataTokens.GetHeapOffset(document.Hash);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Language => MetadataTokens.GetHeapOffset(document.Language);

			public DocumentEntry(MetadataFile metadataFile, DocumentHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				document = metadataFile.Metadata.GetDocument(handle);
			}
		}
	}
}
