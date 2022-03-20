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
using System.IO;
using System.Linq;
using System.Windows;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.TreeView;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy.TextView
{
	[ExportContextMenuEntry(Header = nameof(Resources._SaveCode), Category = nameof(Resources.Save), Icon = "Images/Save")]
	sealed class SaveCodeContextMenuEntry : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			Execute(context.SelectedTreeNodes);
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			return CanExecute(context.SelectedTreeNodes);
		}

		public static bool CanExecute(IReadOnlyList<SharpTreeNode> selectedNodes)
		{
			if (selectedNodes == null || selectedNodes.Any(n => !(n is ILSpyTreeNode)))
				return false;
			return selectedNodes.Count == 1
				|| (selectedNodes.Count > 1 && (selectedNodes.All(n => n is AssemblyTreeNode) || selectedNodes.All(n => n is IMemberTreeNode)));
		}

		public static void Execute(IReadOnlyList<SharpTreeNode> selectedNodes)
		{
			var currentLanguage = MainWindow.Instance.CurrentLanguage;
			var tabPage = Docking.DockWorkspace.Instance.ActiveTabPage;
			tabPage.ShowTextView(textView => {
				if (selectedNodes.Count == 1 && selectedNodes[0] is ILSpyTreeNode singleSelection)
				{
					// if there's only one treenode selected
					// we will invoke the custom Save logic
					if (singleSelection.Save(tabPage))
						return;
				}
				else if (selectedNodes.Count > 1 && selectedNodes.All(n => n is AssemblyTreeNode))
				{
					var selectedPath = SelectSolutionFile();

					if (!string.IsNullOrEmpty(selectedPath))
					{
						var assemblies = selectedNodes.OfType<AssemblyTreeNode>()
							.Select(n => n.LoadedAssembly)
							.Where(a => a.IsLoadedAsValidAssembly).ToArray();
						SolutionWriter.CreateSolution(textView, selectedPath, currentLanguage, assemblies);
					}
					return;
				}

				// Fallback: if nobody was able to handle the request, use default behavior.
				// try to save all nodes to disk.
				var options = new DecompilationOptions() { FullDecompilation = true };
				textView.SaveToDisk(currentLanguage, selectedNodes.OfType<ILSpyTreeNode>(), options);
			});
		}

		/// <summary>
		/// Shows a File Selection dialog where the user can select the target file for the solution.
		/// </summary>
		/// <param name="path">The initial path to show in the dialog. If not specified, the 'Documents' directory
		/// will be used.</param>
		/// 
		/// <returns>The full path of the selected target file, or <c>null</c> if the user canceled.</returns>
		static string SelectSolutionFile()
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = "Solution.sln";
			dlg.Filter = Resources.VisualStudioSolutionFileSlnAllFiles;

			if (dlg.ShowDialog() != true)
			{
				return null;
			}

			string selectedPath = Path.GetDirectoryName(dlg.FileName);
			bool directoryNotEmpty;
			try
			{
				directoryNotEmpty = Directory.EnumerateFileSystemEntries(selectedPath).Any();
			}
			catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is System.Security.SecurityException)
			{
				MessageBox.Show(
					"The directory cannot be accessed. Please ensure it exists and you have sufficient rights to access it.",
					"Solution directory not accessible",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}

			if (directoryNotEmpty)
			{
				var result = MessageBox.Show(
					Resources.AssemblySaveCodeDirectoryNotEmpty,
					Resources.AssemblySaveCodeDirectoryNotEmptyTitle,
					MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
				if (result == MessageBoxResult.No)
					return null; // -> abort
			}

			return dlg.FileName;
		}
	}
}
