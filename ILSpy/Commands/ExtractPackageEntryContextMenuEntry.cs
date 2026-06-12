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

using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;

using static ICSharpCode.ILSpyX.LoadedPackage;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Extract Package Entry". Saves a selection of bundle items
	/// (assemblies, resource entries, package folders) to disk. Single-item selection
	/// pops a file-save dialog; multi-item or folder selection pops a folder-pick.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ExtractPackageEntry), Category = nameof(Resources.Save), Icon = "Images/FolderOpen", Order = 320)]
	[Shared]
	public sealed class ExtractPackageEntryContextMenuEntry : IContextMenuEntry
	{
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public ExtractPackageEntryContextMenuEntry(DockWorkspace dockWorkspace)
		{
			this.dockWorkspace = dockWorkspace;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
			=> context.SelectedTreeNodes?.Any(IsBundleItem) == true;

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return;
			var selectedNodes = Array.FindAll(context.SelectedTreeNodes, IsBundleItem);
			if (selectedNodes.Length == 0)
				return;
			// Initial directory for the picker — the path of the outer bundle assembly.
			var bundleNode = selectedNodes.FirstOrDefault()?.Ancestors().OfType<AssemblyTreeNode>()
				.FirstOrDefault(asm => asm.PackageEntry == null);
			if (bundleNode == null)
				return;
			ExecuteAsync(selectedNodes).HandleExceptions();
		}

		async Task ExecuteAsync(SharpTreeNode[] selectedNodes)
		{
			if (selectedNodes is [AssemblyTreeNode { PackageEntry: { } assembly }])
			{
				var defaultName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(assembly.Name));
				var path = await FilePickers.SaveAsync(
					".NET assemblies (*.dll, *.exe, *.winmd)|*.dll;*.exe;*.winmd|All files|*.*",
					defaultName);
				if (string.IsNullOrEmpty(path))
					return;
				await SaveAsync(selectedNodes, path, isFile: true);
			}
			else if (selectedNodes is [ResourceTreeNode { Resource: { } resource }])
			{
				var defaultName = Path.GetFileName(WholeProjectDecompiler.SanitizeFileName(resource.Name));
				var path = await FilePickers.SaveAsync("All files|*.*", defaultName);
				if (string.IsNullOrEmpty(path))
					return;
				await SaveAsync(selectedNodes, path, isFile: true);
			}
			else
			{
				var folder = await FilePickers.PickFolderAsync("Select target folder");
				if (string.IsNullOrEmpty(folder))
					return;
				await SaveAsync(selectedNodes, folder, isFile: false);
			}
		}

		internal Task SaveAsync(IReadOnlyList<SharpTreeNode> nodes, string path, bool isFile)
			=> SaveAsync(dockWorkspace, nodes, path, isFile);

		internal static async Task SaveAsync(DockWorkspace dockWorkspace, IReadOnlyList<SharpTreeNode> nodes, string path, bool isFile)
		{
			// Run in a dedicated frozen tab so browsing the tree while extraction runs can't cancel it.
			await dockWorkspace.RunInNewTabAsync("Extracting…", token => Task.Run(() => {
				var output = new AvaloniaEditTextOutput { Title = "Extract package entry" };
				var stopwatch = Stopwatch.StartNew();
				var fileNameCounts = new Dictionary<string, int>(Platform.FileNameComparer);
				foreach (var (entry, fileName) in CollectAllFiles(nodes))
				{
					var actualFileName = WholeProjectDecompiler.SanitizeFileName(fileName);
					while (fileNameCounts.TryGetValue(actualFileName, out var index))
					{
						index++;
						fileNameCounts[actualFileName] = index;
						actualFileName = Path.ChangeExtension(actualFileName, index + Path.GetExtension(actualFileName));
					}
					if (!fileNameCounts.ContainsKey(actualFileName))
						fileNameCounts[actualFileName] = 1;
					SaveEntry(output, entry, isFile ? path : Path.Combine(path, actualFileName));
				}
				stopwatch.Stop();
				output.Write(string.Format(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1")));
				output.WriteLine();
				output.WriteLine();
				if (isFile)
					output.AddRevealFileButton(path);
				else
					output.AddOpenFolderButton(path);
				return output;
			}, token)).ConfigureAwait(true);
		}

		static IEnumerable<(PackageEntry Entry, string TargetFileName)> CollectAllFiles(IReadOnlyList<SharpTreeNode> nodes)
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
					var package = asm.LoadedAssembly.GetLoadResultAsync().GetAwaiter().GetResult().Package!;
					foreach (var entry in package.Entries)
						yield return (entry, entry.FullName);
				}
				else if (node is PackageFolderTreeNode folder)
				{
					int prefixLength = 0;
					var current = folder.Folder;
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

		static void SaveEntry(ITextOutput output, PackageEntry entry, string targetFileName)
		{
			output.Write(entry.Name + ": ");
			using var stream = entry.TryOpenStream();
			if (stream == null)
			{
				output.WriteLine("Could not open stream!");
				return;
			}
			Directory.CreateDirectory(Path.GetDirectoryName(targetFileName)!);
			stream.Position = 0;
			using var fileStream = new FileStream(targetFileName, FileMode.OpenOrCreate);
			stream.CopyTo(fileStream);
			output.Write("Written to ");
			output.Write(targetFileName);
			output.WriteLine();
		}


		static bool IsBundleItem(SharpTreeNode node)
		{
			if (node is AssemblyTreeNode { PackageEntry: { } } or PackageFolderTreeNode)
				return true;
			if (node is ResourceTreeNode { Resource: PackageEntry { } resource } && resource.PackageQualifiedFileName.StartsWith("bundle://"))
				return true;
			return false;
		}
	}

	/// <summary>
	/// Right-click an unpacked bundle assembly → "Extract All Package Entries". Saves
	/// every entry inside the bundle into the picked folder. Shortcut for the multi-
	/// select case in <see cref="ExtractPackageEntryContextMenuEntry"/>.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ExtractAllPackageEntries), Category = nameof(Resources.Save), Icon = "Images/FolderOpen", Order = 330)]
	[Shared]
	public sealed class ExtractAllPackageEntriesContextMenuEntry : IContextMenuEntry
	{
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public ExtractAllPackageEntriesContextMenuEntry(DockWorkspace dockWorkspace)
		{
			this.dockWorkspace = dockWorkspace;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes is [AssemblyTreeNode { PackageEntry: null, PackageKind: PackageKind.Bundle }];
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not [AssemblyTreeNode { PackageEntry: null } asm])
				return;
			ExecuteAsync(asm).HandleExceptions();
		}

		async Task ExecuteAsync(AssemblyTreeNode asm)
		{
			var folder = await FilePickers.PickFolderAsync("Select target folder");
			if (string.IsNullOrEmpty(folder))
				return;
			// Reuses the shared Save pipeline from ExtractPackageEntryContextMenuEntry.
			await ExtractPackageEntryContextMenuEntry.SaveAsync(dockWorkspace, new[] { (SharpTreeNode)asm }, folder, isFile: false);
		}
	}
}
