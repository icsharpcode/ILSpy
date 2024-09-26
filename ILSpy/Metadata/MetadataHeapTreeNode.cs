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

using System.Reflection.Metadata;
using System.Windows.Controls;
using System.Windows.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal abstract class MetadataHeapTreeNode : ILSpyTreeNode
	{
		protected MetadataFile metadataFile;
		protected int scrollTarget;

		public HandleKind Kind { get; }

		public override object Icon => Images.Heap;

		public MetadataHeapTreeNode(HandleKind kind, MetadataFile metadataFile)
		{
			this.Kind = kind;
			this.metadataFile = metadataFile;
		}

		protected void ScrollItemIntoView(DataGrid view, object item)
		{
			view.Loaded += View_Loaded;
			view.Dispatcher.BeginInvoke(() => view.SelectItem(item), DispatcherPriority.Background);
		}

		private void View_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			DataGrid view = (DataGrid)sender;
			var sv = view.FindVisualChild<ScrollViewer>();
			sv.ScrollToVerticalOffset(scrollTarget - 1);
			view.Loaded -= View_Loaded;
			this.scrollTarget = default;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
		}
	}
}