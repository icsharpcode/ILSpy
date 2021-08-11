// Copyright (c) 2021 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class GuidHeapTreeNode : MetadataHeapTreeNode
	{
		readonly List<GuidHeapEntry> list;

		public GuidHeapTreeNode(PEFile module, MetadataReader metadata)
			: base(HandleKind.Guid, module, metadata)
		{
			list = new List<GuidHeapEntry>();
			int count = metadata.GetHeapSize(HeapIndex.Guid) >> 4;
			for (int i = 1; i <= count; i++)
			{
				GuidHeapEntry entry = new GuidHeapEntry(metadata, MetadataTokens.GuidHandle(i));
				list.Add(entry);
			}
		}

		public override object Text => $"Guid Heap ({list.Count})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			view.ItemsSource = list;

			tabPage.Content = view;

			return true;
		}

		class GuidHeapEntry
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

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Guid Heap");
		}
	}
}