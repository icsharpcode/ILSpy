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
using System.Collections.Generic;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.TreeView;

namespace ICSharpCode.ILSpy
{
	[ExportMainMenuCommand(Menu = nameof(Resources._View),  Header = nameof(Resources.SortAssembly_listName),  MenuIcon = "Images/Sort", MenuCategory = nameof(Resources.View))]
	[ExportToolbarCommand(ToolTip = nameof(Resources.SortAssemblyListName),  ToolbarIcon = "Images/Sort",  ToolbarCategory = nameof(Resources.View))]
	sealed class SortAssemblyListCommand : SimpleCommand, IComparer<LoadedAssembly>
	{
		public override void Execute(object parameter)
		{
			using (MainWindow.Instance.treeView.LockUpdates())
				MainWindow.Instance.CurrentAssemblyList.Sort(this);
		}

		int IComparer<LoadedAssembly>.Compare(LoadedAssembly x, LoadedAssembly y)
		{
			return string.Compare(x.ShortName, y.ShortName, StringComparison.CurrentCulture);
		}
	}

	[ExportMainMenuCommand(Menu = nameof(Resources._View),  Header = nameof(Resources._CollapseTreeNodes),  MenuIcon = "Images/CollapseAll", MenuCategory = nameof(Resources.View))]
	[ExportToolbarCommand(ToolTip = nameof(Resources.CollapseTreeNodes),  ToolbarIcon = "Images/CollapseAll", ToolbarCategory = nameof(Resources.View))]
	sealed class CollapseAllCommand : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			using (MainWindow.Instance.treeView.LockUpdates())
				CollapseChildren(MainWindow.Instance.treeView.Root);

			void CollapseChildren(SharpTreeNode node)
			{
				foreach (var child in node.Children) {
					if (!child.IsExpanded)
						continue;
					CollapseChildren(child);
					child.IsExpanded = false;
				}
			}
		}
	}
}
