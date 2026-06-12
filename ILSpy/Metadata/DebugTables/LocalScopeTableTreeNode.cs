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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata.DebugTables
{
	/// <summary>
	/// View of the LocalScope table — every lexical scope inside every method body's
	/// debug info. Each row points back at the MethodDef + import-scope + local-variable
	/// list and carries the scope's IL start offset and length.
	/// </summary>
	public sealed class LocalScopeTableTreeNode : MetadataTableTreeNode<LocalScopeTableTreeNode.LocalScopeEntry>
	{
		public LocalScopeTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.LocalScope, metadataFile)
		{
		}

		protected override IReadOnlyList<LocalScopeEntry> LoadTable()
		{
			var list = new List<LocalScopeEntry>();
			foreach (var row in metadataFile.Metadata.LocalScopes)
				list.Add(new LocalScopeEntry(metadataFile, row));
			return list;
		}

		public sealed class LocalScopeEntry
		{
			readonly MetadataFile metadataFile;
			readonly LocalScopeHandle handle;
			readonly LocalScope localScope;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Method => MetadataTokens.GetToken(localScope.Method);

			string? methodTooltip;
			public string? MethodTooltip => GenerateTooltip(ref methodTooltip, metadataFile, localScope.Method);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ImportScope => MetadataTokens.GetToken(localScope.ImportScope);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int VariableList => MetadataTokens.GetToken(localScope.GetLocalVariables().FirstOrDefault());

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int ConstantList => MetadataTokens.GetToken(localScope.GetLocalConstants().FirstOrDefault());

			public int StartOffset => localScope.StartOffset;

			public int Length => localScope.Length;

			public LocalScopeEntry(MetadataFile metadataFile, LocalScopeHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				localScope = metadataFile.Metadata.GetLocalScope(handle);
			}
		}
	}
}
