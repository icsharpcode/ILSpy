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

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// View of #GUID — a contiguous array of 16-byte GUIDs indexed (1-based) by ModuleDef
	/// rows and a handful of other tables. Indexing starts at 1 because index 0 is reserved
	/// for "no GUID".
	/// </summary>
	public sealed class GuidHeapTreeNode : MetadataHeapTreeNode<GuidHeapTreeNode.GuidHeapEntry>
	{
		public GuidHeapTreeNode(MetadataFile metadataFile)
			: base(HandleKind.Guid, metadataFile)
		{
		}

		protected override string HeapName => "Guid Heap";

		protected override void LoadEntries(MetadataReader metadata, List<GuidHeapEntry> list)
		{
			int count = metadata.GetHeapSize(HeapIndex.Guid) >> 4;
			for (int i = 1; i <= count; i++)
				list.Add(new GuidHeapEntry(metadata, MetadataTokens.GuidHandle(i)));
		}

		public sealed class GuidHeapEntry
		{
			readonly MetadataReader metadata;
			readonly GuidHandle handle;

			public int Index => metadata.GetHeapOffset(handle);
			public int Length => 16;
			public string Value => metadata.GetGuid(handle).ToString();

			public GuidHeapEntry(MetadataReader metadata, GuidHandle handle)
			{
				this.metadata = metadata;
				this.handle = handle;
			}
		}
	}
}
