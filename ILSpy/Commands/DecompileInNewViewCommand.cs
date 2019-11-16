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
using System.Linq;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	[ExportContextMenuEntry(Header = nameof(Resources.DecompileToNewPanel), Icon = "images/Search", Category = nameof(Resources.Analyze), Order = 90)]
	internal sealed class DecompileInNewViewCommand : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return true;			
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return false;
			return true;			
		}

		public void Execute(TextViewContext context)
		{
			var nodes = context.SelectedTreeNodes.Cast<ILSpyTreeNode>().ToArray();
			var title = string.Join(", ", nodes.Select(x => x.ToString()));
			DockWorkspace.Instance.Documents.Add(new ViewModels.DecompiledDocumentModel(title, title));
			DockWorkspace.Instance.ActiveDocument = DockWorkspace.Instance.Documents.Last();
			MainWindow.Instance.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, (Action)(delegate {
				DockWorkspace.Instance.GetTextView().DecompileAsync(MainWindow.Instance.CurrentLanguage, nodes, new DecompilationOptions());
			}));
		}
	}
}
