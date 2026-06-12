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

using System.Composition;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Compare;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Compare…" on exactly two valid managed assemblies in the tree.
	/// Opens a fresh tab whose content is a <see cref="CompareTabPageModel"/> driven by
	/// <see cref="CompareEngine"/>. Order matters: the first-selected node becomes the
	/// "left" side, the second becomes the "right".
	/// </summary>
	[ExportContextMenuEntry(Header = "Compare...", Category = nameof(Resources.Analyze), Order = 220)]
	[Shared]
	public sealed class CompareContextMenuEntry : IContextMenuEntry
	{
		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public bool IsVisible(TextViewContext context)
			=> context.SelectedTreeNodes is
				[AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true },
				AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true }];

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not
				[AssemblyTreeNode left, AssemblyTreeNode right])
				return;

			var dockWorkspace = AppComposition.TryGetExport<DockWorkspace>();
			if (dockWorkspace == null)
				return;
			var pane = new CompareTabPageModel(left.LoadedAssembly, right.LoadedAssembly);
			// OpenNewTab wraps the content viewmodel in a ContentTabPage; the DataTemplate
			// for CompareTabPageModel routes through CompareView. Pass null for sourceNode
			// so the new tab isn't anchored to either assembly's tree node — the compare
			// view is a pair, not a single-node decompile result.
			dockWorkspace.OpenNewTab(pane);
		}
	}
}
