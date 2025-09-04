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

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	class StandAloneSigTableTreeNode : MetadataTableTreeNode<StandAloneSigTableTreeNode.StandAloneSigEntry>
	{
		public StandAloneSigTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.StandAloneSig, metadataFile)
		{
		}

		protected override IReadOnlyList<StandAloneSigEntry> LoadTable()
		{
			var list = new List<StandAloneSigEntry>();
			for (int row = 1; row <= metadataFile.Metadata.GetTableRowCount(TableIndex.StandAloneSig); row++)
			{
				list.Add(new StandAloneSigEntry(metadataFile, MetadataTokens.StandaloneSignatureHandle(row)));
			}

			return list;
		}

		internal struct StandAloneSigEntry
		{
			readonly MetadataFile metadataFile;
			readonly StandaloneSignatureHandle handle;
			readonly StandaloneSignature standaloneSig;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataFile.MetadataOffset
				+ metadataFile.Metadata.GetTableMetadataOffset(TableIndex.StandAloneSig)
				+ metadataFile.Metadata.GetTableRowSize(TableIndex.StandAloneSig) * (RID - 1);

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Signature => MetadataTokens.GetHeapOffset(standaloneSig.Signature);

			string signatureTooltip;
			public string SignatureTooltip => GenerateTooltip(ref signatureTooltip, metadataFile, handle);

			public StandAloneSigEntry(MetadataFile metadataFile, StandaloneSignatureHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				this.standaloneSig = metadataFile.Metadata.GetStandaloneSignature(handle);
				this.signatureTooltip = null;
			}
		}
	}
}
