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

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// View of #US — the UTF-16 heap holding every literal string emitted by an <c>ldstr</c>
	/// instruction. Distinct from #Strings (which carries identifier names); user strings
	/// are referenced by metadata tokens, not by member-name resolution.
	/// </summary>
	public sealed class UserStringHeapTreeNode : MetadataHeapTreeNode<UserStringHeapTreeNode.UserStringHeapEntry>
	{
		public UserStringHeapTreeNode(MetadataFile metadataFile)
			: base(HandleKind.UserString, metadataFile)
		{
		}

		protected override string HeapName => "UserString Heap";

		protected override void LoadEntries(MetadataReader metadata, List<UserStringHeapEntry> list)
		{
			WalkHeap(list, MetadataTokens.UserStringHandle(0),
				h => new UserStringHeapEntry(metadata, h), metadata.GetNextHandle, h => h.IsNil);
		}

		public sealed class UserStringHeapEntry
		{
			readonly MetadataReader metadata;
			readonly UserStringHandle handle;

			[ColumnInfo("X8")]
			public int Offset => metadata.GetHeapOffset(handle);
			public int Length => metadata.GetUserString(handle).Length;
			public string Value => metadata.GetUserString(handle);

			public UserStringHeapEntry(MetadataReader metadata, UserStringHandle handle)
			{
				this.metadata = metadata;
				this.handle = handle;
			}
		}
	}
}
