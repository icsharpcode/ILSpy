﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
	internal class UserStringHeapTreeNode : MetadataHeapTreeNode
	{
		readonly List<UserStringHeapEntry> list;

		public UserStringHeapTreeNode(MetadataFile metadataFile)
			: base(HandleKind.UserString, metadataFile)
		{
			list = new List<UserStringHeapEntry>();
			var metadata = metadataFile.Metadata;
			UserStringHandle handle = MetadataTokens.UserStringHandle(0);
			do
			{
				UserStringHeapEntry entry = new UserStringHeapEntry(metadata, handle);
				list.Add(entry);
				handle = metadata.GetNextHandle(handle);
			} while (!handle.IsNil);
		}

		public override object Text => $"UserString Heap ({list.Count})";

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			view.ItemsSource = list;

			tabPage.Content = view;

			return true;
		}

		class UserStringHeapEntry
		{
			readonly MetadataReader metadata;
			readonly UserStringHandle handle;

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