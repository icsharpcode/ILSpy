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

using ILSpy.ViewModels;

namespace ILSpy.Metadata
{
	/// <summary>
	/// View of #Strings — the heap that holds every UTF-8 type/member/namespace name
	/// referenced by the metadata tables. Lazy materialisation: handles are walked on
	/// demand via <see cref="MetadataReader.GetNextHandle(StringHandle)"/> the first time a
	/// caller asks for the count or a preview row.
	/// </summary>
	public sealed class StringHeapTreeNode : MetadataHeapTreeNode
	{
		List<StringHeapEntry>? entries;

		public StringHeapTreeNode(MetadataFile metadataFile)
			: base(HandleKind.String, metadataFile)
		{
		}

		public override object Text => $"String Heap ({EnsureEntries().Count})";
		public override string ToString() => "String Heap";

		public override TabPageModel CreateTab()
		{
			var page = new MetadataTablePageModel {
				Title = "String Heap",
				Items = EnsureEntries(),
			};
			MetadataColumnBuilder.Populate<StringHeapEntry>(page);
			return page;
		}

		List<StringHeapEntry> EnsureEntries()
		{
			if (entries is { } cached)
				return cached;
			var list = new List<StringHeapEntry>();
			var metadata = metadataFile.Metadata;
			var handle = MetadataTokens.StringHandle(0);
			do
			{
				list.Add(new StringHeapEntry(metadata, handle));
				handle = metadata.GetNextHandle(handle);
			} while (!handle.IsNil);
			entries = list;
			return list;
		}

		public sealed class StringHeapEntry
		{
			readonly MetadataReader metadata;
			readonly StringHandle handle;

			[ColumnInfo("X8")]
			public int Offset => metadata.GetHeapOffset(handle);
			public int Length => metadata.GetString(handle).Length;
			public string Value => metadata.GetString(handle);

			public StringHeapEntry(MetadataReader metadata, StringHandle handle)
			{
				this.metadata = metadata;
				this.handle = handle;
			}
		}
	}
}
