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
using ICSharpCode.Decompiler.Metadata;

using ILSpy.Languages;

namespace ILSpy.Metadata
{
	/// <summary>
	/// View of #GUID — a contiguous array of 16-byte GUIDs indexed (1-based) by ModuleDef
	/// rows and a handful of other tables. Indexing starts at 1 because index 0 is reserved
	/// for "no GUID".
	/// </summary>
	public sealed class GuidHeapTreeNode : MetadataHeapTreeNode
	{
		List<GuidHeapEntry>? entries;

		public GuidHeapTreeNode(MetadataFile metadataFile)
			: base(HandleKind.Guid, metadataFile)
		{
		}

		public override object Text => $"Guid Heap ({EnsureEntries().Count})";
		public override string ToString() => "Guid Heap";

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			var all = EnsureEntries();
			var preview = all.Count > PreviewLimit
				? (IReadOnlyList<GuidHeapEntry>)all.GetRange(0, PreviewLimit)
				: all;
			language.WriteCommentLine(output, $"Guid Heap ({all.Count} entries)");
			MetadataTextWriter.WriteTable(language, output, preview);
			if (all.Count > preview.Count)
				language.WriteCommentLine(output, $"... ({all.Count - preview.Count} more entries; full view ships with the metadata DataGrid in a future release)");
		}

		List<GuidHeapEntry> EnsureEntries()
		{
			if (entries is { } cached)
				return cached;
			var list = new List<GuidHeapEntry>();
			var metadata = metadataFile.Metadata;
			int count = metadata.GetHeapSize(HeapIndex.Guid) >> 4;
			for (int i = 1; i <= count; i++)
				list.Add(new GuidHeapEntry(metadata, MetadataTokens.GuidHandle(i)));
			entries = list;
			return list;
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
