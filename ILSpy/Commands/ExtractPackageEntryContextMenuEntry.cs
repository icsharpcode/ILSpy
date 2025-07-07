// Copyright (c) 2021 Siegfried Pammer
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
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using Microsoft.Win32;

using static ICSharpCode.ILSpyX.LoadedPackage;

namespace ICSharpCode.ILSpy
{
	[ExportContextMenuEntry(Header = nameof(Resources.ExtractPackageEntry), Category = nameof(Resources.Save), Icon = "Images/Save")]
	[Shared]
	sealed class ExtractPackageEntryContextMenuEntry(DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			var selectedNodes = Array.FindAll(context.SelectedTreeNodes, x => x is AssemblyTreeNode { PackageEntry: { } } or PackageFolderTreeNode);
			// Get root assembly to infer the initial directory for the save dialog.
			var bundleNode = selectedNodes.FirstOrDefault()?.Ancestors().OfType<AssemblyTreeNode>()
				.FirstOrDefault(asm => asm.PackageEntry == null);
			if (bundleNode == null)
				return;
			if (selectedNodes is [AssemblyTreeNode { PackageEntry: { } assembly }])
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.FileName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(assembly.Name));
				dlg.Filter = ".NET assemblies|*.dll;*.exe;*.winmd" + Resources.AllFiles;
				dlg.InitialDirectory = Path.GetDirectoryName(bundleNode.LoadedAssembly.FileName);
				if (dlg.ShowDialog() != true)
					return;

				string fileName = dlg.FileName;
				dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
					AvalonEditTextOutput output = new AvalonEditTextOutput();
					Stopwatch stopwatch = Stopwatch.StartNew();
					SaveEntry(output, assembly, fileName);
					stopwatch.Stop();
					output.WriteLine(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1"));
					output.WriteLine();
					output.AddButton(null, Resources.OpenExplorer, delegate { Process.Start("explorer", "/select,\"" + fileName + "\""); });
					output.WriteLine();
					return output;
				}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
			}
			else
			{
				OpenFolderDialog dlg = new OpenFolderDialog();
				dlg.InitialDirectory = Path.GetDirectoryName(bundleNode.LoadedAssembly.FileName);
				if (dlg.ShowDialog() != true)
					return;

				string folderName = dlg.FolderName;
				if (Directory.EnumerateFileSystemEntries(folderName).Any())
				{
					var result = MessageBox.Show(
						Resources.AssemblySaveCodeDirectoryNotEmpty,
						Resources.AssemblySaveCodeDirectoryNotEmptyTitle,
						MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
					if (result == MessageBoxResult.No)
						return;
				}

				dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
					AvalonEditTextOutput output = new AvalonEditTextOutput();
					Stopwatch stopwatch = Stopwatch.StartNew();
					foreach (var selectedNode in selectedNodes)
					{
						if (selectedNode is AssemblyTreeNode { PackageEntry: { } assembly })
						{
							string fileName = Path.Combine(folderName, GetFullPath(selectedNode.Parent) + WholeProjectDecompiler.SanitizeFileName(assembly.Name));
							SaveEntry(output, assembly, fileName);
						}
						else if (selectedNode is PackageFolderTreeNode)
						{
							selectedNode.EnsureLazyChildren();
							foreach (var node in selectedNode.DescendantsAndSelf())
							{
								if (node is AssemblyTreeNode { PackageEntry: { } asm })
								{
									string fileName = Path.Combine(folderName, GetFullPath(node.Parent) + WholeProjectDecompiler.SanitizeFileName(asm.Name));
									SaveEntry(output, asm, fileName);
								}
								else if (node is PackageFolderTreeNode)
								{
									Directory.CreateDirectory(Path.Combine(folderName, GetFullPath(node)));
								}
							}
						}
					}
					stopwatch.Stop();
					output.WriteLine(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1"));
					output.WriteLine();
					output.AddButton(null, Resources.OpenExplorer, delegate { Process.Start("explorer", "\"" + folderName + "\""); });
					output.WriteLine();
					return output;
				}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
			}
		}

		static string GetFullPath(SharpTreeNode node)
		{
			if (node is PackageFolderTreeNode)
			{
				string name = node.Text + "\\";
				if (GetFullPath(node.Parent) is string parent)
					return parent + "\\" + name;
				return name;
			}
			return null;
		}

		void SaveEntry(ITextOutput output, PackageEntry entry, string targetFileName)
		{
			output.Write(entry.Name + ": ");
			using Stream stream = entry.TryOpenStream();
			if (stream == null)
			{
				output.WriteLine("Could not open stream!");
				return;
			}

			stream.Position = 0;
			using FileStream fileStream = new FileStream(targetFileName, FileMode.OpenOrCreate);
			stream.CopyTo(fileStream);
			output.WriteLine("Written to " + targetFileName);
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context) =>
			context.SelectedTreeNodes?.Any(x => x is AssemblyTreeNode { PackageEntry: { } } or PackageFolderTreeNode) == true;
	}

	[ExportContextMenuEntry(Header = nameof(Resources.ExtractAllPackageEntries), Category = nameof(Resources.Save), Icon = "Images/Save")]
	[Shared]
	sealed class ExtractAllPackageEntriesContextMenuEntry(DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not [AssemblyTreeNode { PackageEntry: null } asm])
				return;
			OpenFolderDialog dlg = new OpenFolderDialog();
			dlg.InitialDirectory = Path.GetDirectoryName(asm.LoadedAssembly.FileName);
			if (dlg.ShowDialog() != true)
				return;

			string folderName = dlg.FolderName;
			if (Directory.EnumerateFileSystemEntries(folderName).Any())
			{
				var result = MessageBox.Show(
					Resources.AssemblySaveCodeDirectoryNotEmpty,
					Resources.AssemblySaveCodeDirectoryNotEmptyTitle,
					MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
				if (result == MessageBoxResult.No)
					return;
			}

			dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				Stopwatch stopwatch = Stopwatch.StartNew();
				asm.EnsureLazyChildren();
				foreach (var node in asm.Descendants())
				{
					if (node is AssemblyTreeNode { PackageEntry: { } assembly })
					{
						string fileName = Path.Combine(folderName, GetFullPath(node.Parent) + WholeProjectDecompiler.SanitizeFileName(assembly.Name));
						SaveEntry(output, assembly, fileName);
					}
					else if (node is PackageFolderTreeNode)
					{
						Directory.CreateDirectory(Path.Combine(folderName, GetFullPath(node)));
					}
				}
				stopwatch.Stop();
				output.WriteLine(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1"));
				output.WriteLine();
				output.AddButton(null, Resources.OpenExplorer, delegate { Process.Start("explorer", "\"" + folderName + "\""); });
				output.WriteLine();
				return output;
			}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
		}

		static string GetFullPath(SharpTreeNode node)
		{
			if (node is PackageFolderTreeNode)
			{
				string name = node.Text + "\\";
				if (GetFullPath(node.Parent) is string parent)
					return parent + "\\" + name;
				return name;
			}
			return null;
		}

		void SaveEntry(ITextOutput output, PackageEntry entry, string targetFileName)
		{
			output.Write(entry.Name + ": ");
			using Stream stream = entry.TryOpenStream();
			if (stream == null)
			{
				output.WriteLine("Could not open stream!");
				return;
			}

			stream.Position = 0;
			using FileStream fileStream = new FileStream(targetFileName, FileMode.OpenOrCreate);
			stream.CopyTo(fileStream);
			output.WriteLine("Written to " + targetFileName);
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is [AssemblyTreeNode { PackageEntry: null } asm])
			{
				try
				{
					if (asm.LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult().Package is { Kind: PackageKind.Bundle })
						return true;
				}
				catch
				{
				}
			}
			return false;
		}
	}
}
