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
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Util;
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
			var selectedNodes = Array.FindAll(context.SelectedTreeNodes, IsBundleItem);
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
				if (dlg.ShowDialog() == true)
					Save(dockWorkspace, selectedNodes, dlg.FileName, true);
			}
			else if (selectedNodes is [ResourceTreeNode { Resource: { } resource }])
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.FileName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(resource.Name));
				dlg.Filter = Resources.AllFiles[1..];
				dlg.InitialDirectory = Path.GetDirectoryName(bundleNode.LoadedAssembly.FileName);
				if (dlg.ShowDialog() == true)
					Save(dockWorkspace, selectedNodes, dlg.FileName, true);
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

				Save(dockWorkspace, selectedNodes, folderName, false);
			}
		}

		internal static void Save(DockWorkspace dockWorkspace, ICollection<SharpTreeNode> nodes, string path, bool isFile)
		{
			dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				Stopwatch stopwatch = Stopwatch.StartNew();
				Dictionary<string, int> fileNameCounts = new Dictionary<string, int>(Platform.FileNameComparer);
				foreach (var (entry, fileName) in CollectAllFiles(nodes))
				{
					string actualFileName = WholeProjectDecompiler.SanitizeFileName(fileName);
					while (fileNameCounts.TryGetValue(actualFileName, out int index))
					{
						index++;
						fileNameCounts[actualFileName] = index;
						actualFileName = Path.ChangeExtension(actualFileName, index + Path.GetExtension(actualFileName));
					}
					if (!fileNameCounts.ContainsKey(actualFileName))
					{
						fileNameCounts[actualFileName] = 1;
					}
					SaveEntry(output, entry, Path.Combine(path, actualFileName));
				}
				stopwatch.Stop();
				output.WriteLine(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1"));
				output.WriteLine();
				// If we have written files, open explorer and select them grouped by folder; otherwise fall back to opening containing folder.
				if (isFile && File.Exists(path))
					output.AddButton(null, Resources.OpenExplorer, delegate { ShellHelper.OpenFolderAndSelectItem(path); });
				else
					output.AddButton(null, Resources.OpenExplorer, delegate { ShellHelper.OpenFolder(path); });
				output.WriteLine();
				return output;
			}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();

			static IEnumerable<(PackageEntry Entry, string TargetFileName)> CollectAllFiles(ICollection<SharpTreeNode> nodes)
			{
				foreach (var node in nodes)
				{
					if (node is AssemblyTreeNode { PackageEntry: { } assembly })
					{
						yield return (assembly, Path.GetFileName(assembly.FullName));
					}
					else if (node is ResourceTreeNode { Resource: PackageEntry { } resource })
					{
						yield return (resource, Path.GetFileName(resource.FullName));
					}
					else if (node is AssemblyTreeNode { PackageKind: not null } asm)
					{
						var package = asm.LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult().Package;
						foreach (var entry in package.Entries)
						{
							yield return (entry, entry.FullName);
						}
					}
					else if (node is PackageFolderTreeNode folder)
					{
						int prefixLength = 0;
						PackageFolder current = folder.Folder;
						if (nodes.Count > 1)
							current = current.Parent;
						while (current != null)
						{
							prefixLength += current.Name.Length + 1;
							current = current.Parent;
						}
						if (prefixLength > 0)
							prefixLength--;
						foreach (var item in TreeTraversal.PreOrder(folder.Folder, f => f.Folders).SelectMany(f => f.Entries))
						{
							yield return (item, item.FullName.Substring(prefixLength));
						}
					}
				}
			}
		}

		static void SaveEntry(ITextOutput output, PackageEntry entry, string targetFileName)
		{
			output.Write(entry.Name + ": ");
			using Stream stream = entry.TryOpenStream();
			if (stream == null)
			{
				output.WriteLine("Could not open stream!");
				return;
			}

			Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));

			stream.Position = 0;
			using FileStream fileStream = new FileStream(targetFileName, FileMode.OpenOrCreate);
			stream.CopyTo(fileStream);
			output.WriteLine("Written to " + targetFileName);
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context) => context.SelectedTreeNodes?.Any(IsBundleItem) == true;

		static bool IsBundleItem(SharpTreeNode node)
		{
			if (node is AssemblyTreeNode { PackageEntry: { } } or PackageFolderTreeNode)
				return true;
			if (node is ResourceTreeNode { Resource: PackageEntry { } resource } && resource.PackageQualifiedFileName.StartsWith("bundle://"))
				return true;
			return false;
		}
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

			ExtractPackageEntryContextMenuEntry.Save(dockWorkspace, [asm], folderName, false);
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes is [AssemblyTreeNode { PackageEntry: null, PackageKind: PackageKind.Bundle }];
		}
	}
}
