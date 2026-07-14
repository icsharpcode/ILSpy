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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using global::Avalonia.Controls.ApplicationLifetimes;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// The shared launcher behind the dedicated "Export Project/Solution..." entry (File menu +
	/// assembly context menu): recognises an exportable selection, shows
	/// <see cref="ExportProjectDialog"/>, then runs <see cref="ProjectExporter"/> on a settings
	/// clone behind the tab's cancellable progress UI and reports into the active decompiler tab.
	/// </summary>
	internal static class ProjectExport
	{
		/// <summary>
		/// True when <paramref name="nodes"/> is one or more assembly nodes that all loaded as valid
		/// assemblies. <paramref name="solutionMode"/> is set when more than one is selected.
		/// </summary>
		public static bool TryGetExportableAssemblies(IReadOnlyList<SharpTreeNode>? nodes,
			out List<LoadedAssembly> assemblies, out bool solutionMode)
		{
			assemblies = new List<LoadedAssembly>();
			solutionMode = false;
			if (nodes is not { Count: > 0 })
				return false;
			if (!nodes.All(n => n is AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true }))
				return false;
			assemblies = nodes.OfType<AssemblyTreeNode>().Select(n => n.LoadedAssembly).ToList();
			solutionMode = assemblies.Count > 1;
			return true;
		}

		public static async Task PromptAndExportAsync(IReadOnlyList<LoadedAssembly> assemblies,
			bool solutionMode, Language language, DockWorkspace dockWorkspace, SettingsService settingsService)
		{
			var owner = UiContext.MainWindow;
			if (owner == null)
				return;

			var dialog = new ExportProjectDialog(settingsService, assemblies, solutionMode);
			var options = await dialog.ShowDialog<ProjectExportOptions?>(owner).ConfigureAwait(true);
			if (options is null || string.IsNullOrEmpty(options.OutputDirectory))
				return;

			var settingsClone = settingsService.DecompilerSettings.Clone();
			await RunExportAsync(assemblies, solutionMode, options, settingsClone, language, dockWorkspace).ConfigureAwait(true);
		}

		/// <summary>
		/// Exports a single assembly as a decompiled project into <paramref name="outputDirectory"/>, using
		/// the current decompiler settings (no export-dialog overrides, no PDB, no strong-name key). This is
		/// the File -> Save Code -> .csproj path: it reuses the same <see cref="ProjectExporter"/> +
		/// frozen-progress-tab machinery as the Export Project command, so a large assembly reports real
		/// progress and can be cancelled, instead of running silently. The tab is titled the same way as the
		/// Export Project command (see <see cref="RunExportAsync"/>) so the same operation reads the same
		/// however it was started.
		/// </summary>
		public static Task ExportSingleAssemblyAsync(LoadedAssembly assembly, string outputDirectory,
			DecompilerSettings settings, Language language, DockWorkspace dockWorkspace)
		{
			ArgumentNullException.ThrowIfNull(assembly);
			ArgumentNullException.ThrowIfNull(settings);
			ArgumentNullException.ThrowIfNull(dockWorkspace);

			// Mirror the live settings into the options so ProjectExporter.ApplyOverrides is a no-op and the
			// output matches the plain "Save Code" behaviour exactly, just with progress now surfaced.
			var options = new ProjectExportOptions(
				OutputDirectory: outputDirectory,
				UseSdkStyleProjectFormat: settings.UseSdkStyleProjectFormat,
				UseNestedDirectoriesForNamespaces: settings.UseNestedDirectoriesForNamespaces,
				RemoveDeadCode: settings.RemoveDeadCode,
				RemoveDeadStores: settings.RemoveDeadStores,
				UseDebugSymbols: settings.UseDebugSymbols,
				StrongNameKeyFile: null,
				GeneratePdb: false,
				EmbedSourceFilesInPdb: false);
			return RunExportAsync(new List<LoadedAssembly> { assembly }, solutionMode: false, options,
				settings.Clone(), language, dockWorkspace);
		}

		// Runs the export behind a dedicated frozen tab (so browsing the tree while it runs can't cancel it)
		// and reports the result there, with an Open-folder button on success. Shared by the Export Project
		// command and the Save Code -> .csproj path. The tab is titled after the assemblies being exported,
		// joining their full tree-node labels the same way DecompilerTabPageModel.ComposeBaseTitle titles a
		// multi-node decompile tab -- so a single-assembly export reads as that assembly and a solution
		// export as its members, ellipsised on the tab with the full list shown as a tooltip.
		static Task RunExportAsync(IReadOnlyList<LoadedAssembly> assemblies, bool solutionMode,
			ProjectExportOptions options, DecompilerSettings settingsClone, Language language,
			DockWorkspace dockWorkspace)
		{
			var title = assemblies.Count > 0
				? string.Join(", ", assemblies.Select(a => a.Text))
				: ICSharpCode.ILSpy.Properties.Resources.ExportProjectSolution;
			return dockWorkspace.RunInNewTabAsync(title, async (token, progress) => {
				var result = await ProjectExporter.ExportAsync(assemblies, solutionMode, options, settingsClone, language, progress, token)
					.ConfigureAwait(false);
				var o = new AvaloniaEditTextOutput { Title = title };
				o.Write(result.StatusText);
				o.WriteLine();
				if (result.Success && Directory.Exists(options.OutputDirectory))
				{
					o.AddOpenFolderButton(options.OutputDirectory);
				}
				return o;
			});
		}

	}
}
