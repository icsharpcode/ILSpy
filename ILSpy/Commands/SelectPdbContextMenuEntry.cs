﻿// Copyright (c) 2018 Siegfried Pammer
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

using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

using Microsoft.Win32;
namespace ICSharpCode.ILSpy
{
	[ExportContextMenuEntry(Header = nameof(Resources.SelectPDB))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class SelectPdbContextMenuEntry : IContextMenuEntry
	{
		public async void Execute(TextViewContext context)
		{
			var assembly = (context.SelectedTreeNodes?.FirstOrDefault() as AssemblyTreeNode)?.LoadedAssembly;
			if (assembly == null)
				return;
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.FileName = WholeProjectDecompiler.CleanUpFileName(assembly.ShortName) + ".pdb";
			dlg.Filter = Resources.PortablePDBPdbAllFiles;
			dlg.InitialDirectory = Path.GetDirectoryName(assembly.FileName);
			if (dlg.ShowDialog() != true)
				return;

			using (context.TreeView.LockUpdates())
			{
				await assembly.LoadDebugInfo(dlg.FileName);
			}

			var node = (AssemblyTreeNode)MainWindow.Instance.AssemblyTreeModel.FindNodeByPath(new[] { assembly.FileName }, true);
			node.UpdateToolTip();
			MainWindow.Instance.AssemblyTreeModel.SelectNode(node);
			MainWindow.Instance.AssemblyTreeModel.RefreshDecompiledView();
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1
				&& context.SelectedTreeNodes?.FirstOrDefault() is AssemblyTreeNode asm
				&& asm.LoadedAssembly.IsLoadedAsValidAssembly;
		}
	}
}
