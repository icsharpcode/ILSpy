// Copyright (c) 2025 AlphaSierraPapa for the SharpDevelop Team
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

using System.Composition;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy
{
	[ExportContextMenuEntry(Header = "Compare...", Order = 9999)]
	[Shared]
	internal sealed class CompareContextMenuEntry(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			var left = ((AssemblyTreeNode)context.SelectedTreeNodes[0]).LoadedAssembly;
			var right = ((AssemblyTreeNode)context.SelectedTreeNodes[1]).LoadedAssembly;

			var tabPage = dockWorkspace.AddTabPage();
			CompareViewModel.Show(tabPage, left, right, assemblyTreeModel);
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes is [AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true }, AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true }];
		}
	}
}