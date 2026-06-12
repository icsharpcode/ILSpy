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

namespace ICSharpCode.ILSpy.Metadata.DebugTables
{
	/// <summary>
	/// View of the ImportScope table — chains of <c>using</c> / <c>using static</c> / alias
	/// imports active inside a LocalScope. Each row points at its parent ImportScope (or nil
	/// for a root scope) and holds an Imports blob describing the imports in that scope.
	/// </summary>
	public sealed class ImportScopeTableTreeNode : MetadataTableTreeNode<ImportScopeTableTreeNode.ImportScopeEntry>
	{
		public ImportScopeTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.ImportScope, metadataFile)
		{
		}

		protected override IReadOnlyList<ImportScopeEntry> LoadTable()
		{
			var list = new List<ImportScopeEntry>();
			foreach (var row in metadataFile.Metadata.ImportScopes)
				list.Add(new ImportScopeEntry(metadataFile, row));
			return list;
		}

		public sealed class ImportScopeEntry
		{
			readonly MetadataFile metadataFile;
			readonly ImportScopeHandle handle;
			readonly ImportScope scope;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(scope.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Imports => MetadataTokens.GetHeapOffset(scope.ImportsBlob);

			public ImportScopeEntry(MetadataFile metadataFile, ImportScopeHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				scope = metadataFile.Metadata.GetImportScope(handle);
			}
		}
	}
}
