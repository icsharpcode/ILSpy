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

using System;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;

using ILSpy.TreeNodes;

namespace ILSpy.Metadata
{
	/// <summary>
	/// Common parent for the per-table leaves under "Tables". Each subclass corresponds to
	/// one CLI <see cref="TableIndex"/> and surfaces its rows. Phase 1 emits a fixed-width
	/// text dump via <see cref="ILSpyTreeNode.Decompile"/>; Phase 2 swaps to a DataGrid tab
	/// via the per-node <c>CreateTab</c> override added in that phase.
	/// </summary>
	public abstract class MetadataTableTreeNode : ILSpyTreeNode
	{
		protected readonly MetadataFile metadataFile;

		public TableIndex Kind { get; }

		public int RowCount => metadataFile.Metadata.GetTableRowCount(Kind);

		protected MetadataTableTreeNode(TableIndex kind, MetadataFile metadataFile)
		{
			Kind = kind;
			this.metadataFile = metadataFile ?? throw new ArgumentNullException(nameof(metadataFile));
		}

		public override object Icon => Images.Images.MetadataTable;
	}
}
