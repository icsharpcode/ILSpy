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
	/// View of the CustomDebugInformation table — extensible per-entity payloads used for
	/// async/iterator state-machine info, embedded source, source-link JSON, and similar
	/// debug-time data. Each row carries a Parent token, a Kind GUID, and an opaque Value
	/// blob. The friendly-name decoding of well-known Kind GUIDs (StateMachineHoistedLocalScopes,
	/// SourceLink, etc.) is deferred until the Phase 4 cell-tooltip work.
	/// </summary>
	public sealed class CustomDebugInformationTableTreeNode : MetadataTableTreeNode<CustomDebugInformationTableTreeNode.CustomDebugInformationEntry>
	{
		public CustomDebugInformationTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.CustomDebugInformation, metadataFile)
		{
		}

		protected override IReadOnlyList<CustomDebugInformationEntry> LoadTable()
		{
			var list = new List<CustomDebugInformationEntry>();
			foreach (var row in metadataFile.Metadata.CustomDebugInformation)
				list.Add(new CustomDebugInformationEntry(metadataFile, row));
			return list;
		}

		public sealed class CustomDebugInformationEntry
		{
			readonly MetadataFile metadataFile;
			readonly CustomDebugInformationHandle handle;
			readonly CustomDebugInformation debugInfo;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(debugInfo.Parent);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Kind => MetadataTokens.GetHeapOffset(debugInfo.Kind);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(debugInfo.Value);

			public CustomDebugInformationEntry(MetadataFile metadataFile, CustomDebugInformationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				debugInfo = metadataFile.Metadata.GetCustomDebugInformation(handle);
			}
		}
	}
}
