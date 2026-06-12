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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click an assembly → "Select PDB". File-picks a portable / Windows PDB and
	/// associates it with the assembly's debug-info provider, then refreshes the
	/// decompiled view so the new symbols feed back into the output.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.SelectPDB), Category = "Debug", Icon = "Images/ProgramDebugDatabase", Order = 400)]
	[Shared]
	public sealed class SelectPdbContextMenuEntry : IContextMenuEntry
	{
		readonly AssemblyTreeModel assemblyTreeModel;

		[ImportingConstructor]
		public SelectPdbContextMenuEntry(AssemblyTreeModel assemblyTreeModel)
		{
			this.assemblyTreeModel = assemblyTreeModel;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
			=> context.SelectedTreeNodes?.Length == 1
				&& context.SelectedTreeNodes[0] is AssemblyTreeNode asm
				&& asm.LoadedAssembly.IsLoadedAsValidAssembly;

		public void Execute(TextViewContext context)
		{
			var assembly = (context.SelectedTreeNodes?.FirstOrDefault() as AssemblyTreeNode)?.LoadedAssembly;
			if (assembly == null)
				return;
			ExecuteAsync(assembly).HandleExceptions();
		}

		async Task ExecuteAsync(ICSharpCode.ILSpyX.LoadedAssembly assembly)
		{
			var owner = UiContext.MainWindow;
			if (owner == null)
				return;
			var initialDir = Path.GetDirectoryName(assembly.FileName);
			var suggested = WholeProjectDecompiler.CleanUpFileName(assembly.ShortName, ".pdb");
			IStorageFolder? startLocation = null;
			if (!string.IsNullOrEmpty(initialDir))
			{
				try
				{ startLocation = await owner.StorageProvider.TryGetFolderFromPathAsync(initialDir); }
				catch { /* picker accepts a null start location */ }
			}
			var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title = Resources.SelectPDB,
				FileTypeFilter = new[] {
					new FilePickerFileType("Portable PDB / .pdb / All files") {
						Patterns = new[] { "*.pdb", "*.*" },
					},
				},
				SuggestedStartLocation = startLocation,
				SuggestedFileName = suggested,
			});
			if (files.Count == 0)
				return;
			var path = files[0].TryGetLocalPath();
			if (string.IsNullOrEmpty(path))
				return;
			await assembly.LoadDebugInfo(path);
			// Refresh the tree node + re-decompile the current selection so the new debug
			// symbols feed back into the output. Mirrors WPF's RefreshDecompiledView call.
			if (assemblyTreeModel.SelectedItem is { } current)
			{
				assemblyTreeModel.SelectedItem = null;
				assemblyTreeModel.SelectedItem = current;
			}
		}
	}
}
