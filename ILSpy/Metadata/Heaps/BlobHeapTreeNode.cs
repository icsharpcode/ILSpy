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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class BlobHeapTreeNode : MetadataHeapTreeNode
	{
		readonly List<BlobHeapEntry> list;

		public BlobHeapTreeNode(PEFile module, MetadataReader metadata)
			: base(HandleKind.Blob, module, metadata)
		{
			list = new List<BlobHeapEntry>();

			BlobHandle handle = MetadataTokens.BlobHandle(0);
			do
			{
				BlobHeapEntry entry = new BlobHeapEntry(metadata, handle);
				list.Add(entry);
				handle = metadata.GetNextHandle(handle);
			} while (!handle.IsNil);
		}

		public override object Text => $"Blob Heap ({list.Count})";

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

		class BlobHeapEntry
		{
			readonly MetadataReader metadata;
			readonly BlobHandle handle;

			public int Offset => metadata.GetHeapOffset(handle);

			public int Length => metadata.GetBlobReader(handle).Length;

			public string Value => metadata.GetBlobReader(handle).ToHexString();

			public BlobHeapEntry(MetadataReader metadata, BlobHandle handle)
			{
				this.metadata = metadata;
				this.handle = handle;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Blob Heap");
		}
	}
}