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
using ICSharpCode.ILSpy.Properties;
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
	/// The shared launcher for every flow that decompiles whole assemblies to disk. All of them
	/// recognise their selection with <see cref="TryGetExportableAssemblies"/> and run
	/// <see cref="ProjectExporter"/> on a settings clone behind a frozen progress tab
	/// (<see cref="RunExportAsync"/>), differing only in how the target and options are chosen:
	/// <list type="bullet">
	/// <item>"Export Project/Solution..." (File menu + assembly context menu) asks
	/// <see cref="ExportProjectDialog"/> for an output folder and per-run overrides.</item>
	/// <item>Save Code on one assembly, with a project extension picked in the file dialog, exports
	/// that project with the live settings (<see cref="ExportSingleAssemblyAsync"/>).</item>
	/// <item>Save Code on several assemblies exports a solution to a picked <c>.sln</c> path, again
	/// with the live settings (<see cref="PromptAndExportSolutionAsync"/>).</item>
	/// </list>
	/// </summary>
	internal static class ProjectExport
	{
		/// <summary>
		/// True when <paramref name="nodes"/> is one or more assembly nodes, at least one of which loaded
		/// as a valid assembly. Ones that failed to load stay in <paramref name="assemblies"/>: the dialog
		/// badges them and <see cref="ProjectExporter"/> skips them with a line in the report, so a mixed
		/// selection exports what it can instead of being turned away here.
		/// <paramref name="solutionMode"/> is set when more than one node is selected.
		/// </summary>
		public static bool TryGetExportableAssemblies(IReadOnlyList<SharpTreeNode>? nodes,
			out List<LoadedAssembly> assemblies, out bool solutionMode)
		{
			assemblies = new List<LoadedAssembly>();
			solutionMode = false;
			if (nodes is not { Count: > 0 })
				return false;
			if (!nodes.All(n => n is AssemblyTreeNode))
				return false;

			var selected = nodes.OfType<AssemblyTreeNode>().Select(n => n.LoadedAssembly).ToList();
			// Nothing to write when every selected assembly failed to load.
			if (!selected.Any(a => a.IsLoadedAsValidAssembly))
				return false;

			assemblies = selected;
			solutionMode = selected.Count > 1;
			return true;
		}

		/// <summary>
		/// True when <paramref name="nodes"/> is the selection shape Save Code maps onto a solution:
		/// several assembly nodes, on the terms <see cref="TryGetExportableAssemblies"/> sets out -- the
		/// ones that did not load ride along in <paramref name="assemblies"/> to be skipped and reported
		/// by the exporter, rather than costing the whole selection its solution export.
		/// </summary>
		public static bool TryGetSolutionAssemblies(IReadOnlyList<SharpTreeNode>? nodes,
			out List<LoadedAssembly> assemblies)
			=> TryGetExportableAssemblies(nodes, out assemblies, out var solutionMode) && solutionMode;

		/// <summary>
		/// Awaits every assembly's load so a caller can read settled load state -- validity, metadata,
		/// PDB eligibility -- without triggering or blocking on the lazy load itself. Uses the load
		/// accessor that swallows load failures, so a broken assembly in the selection completes here
		/// rather than faulting the whole batch.
		/// </summary>
		internal static Task EnsureAssembliesLoadedAsync(IReadOnlyList<LoadedAssembly> assemblies)
			=> Task.WhenAll(assemblies.Select(a => a.GetMetadataFileOrNullAsync()));

		public static async Task PromptAndExportAsync(IReadOnlyList<LoadedAssembly> assemblies,
			bool solutionMode, Language language, DockWorkspace dockWorkspace, SettingsService settingsService)
		{
			var owner = UiContext.MainWindow;
			if (owner == null)
				return;

			// Settle every selected assembly's load before building the dialog: its preview rows badge
			// each one (valid / not a valid assembly / PDB-eligible) off the load result. Awaiting here
			// means those reads see a completed load, instead of the dialog blocking the UI thread to
			// force one as it builds the rows.
			await EnsureAssembliesLoadedAsync(assemblies).ConfigureAwait(true);

			var dialog = new ExportProjectDialog(settingsService, assemblies, solutionMode);
			var options = await dialog.ShowDialog<ProjectExportOptions?>(owner).ConfigureAwait(true);
			if (options is null || string.IsNullOrEmpty(options.OutputDirectory))
				return;

			var settingsClone = settingsService.DecompilerSettings.Clone();
			await RunExportAsync(assemblies, solutionMode, options, settingsClone, language, dockWorkspace).ConfigureAwait(true);
		}

		/// <summary>
		/// Exports a single assembly as a decompiled project into <paramref name="outputDirectory"/>.
		/// This is the File -> Save Code path for a project extension.
		/// </summary>
		public static Task ExportSingleAssemblyAsync(LoadedAssembly assembly, string outputDirectory,
			DecompilerSettings settings, Language language, DockWorkspace dockWorkspace)
		{
			ArgumentNullException.ThrowIfNull(assembly);
			ArgumentNullException.ThrowIfNull(settings);
			ArgumentNullException.ThrowIfNull(dockWorkspace);

			return RunExportAsync(new List<LoadedAssembly> { assembly }, solutionMode: false,
				OptionsFrom(settings, outputDirectory), settings.Clone(), language, dockWorkspace);
		}

		/// <summary>
		/// Prompts for a target <c>.sln</c> file and exports <paramref name="assemblies"/> into it, one
		/// decompiled project each. This is the File -> Save Code path for a multi-assembly selection.
		/// Does nothing if the user cancels the picker.
		/// </summary>
		public static async Task PromptAndExportSolutionAsync(IReadOnlyList<LoadedAssembly> assemblies,
			Language language, DockWorkspace dockWorkspace)
		{
			var path = await FilePickers.SaveAsync(
				Resources.VisualStudioSolutionFileSlnAllFiles, "Solution.sln",
				Resources._SaveCode).ConfigureAwait(true);
			if (string.IsNullOrEmpty(path))
				return;

			var settings = AppComposition.TryGetExport<SettingsService>()?.CreateEffectiveDecompilerSettings()
				?? new DecompilerSettings();
			await ExportSolutionAsync(assemblies, path, settings, language, dockWorkspace).ConfigureAwait(true);
		}

		/// <summary>
		/// Exports <paramref name="assemblies"/> as a solution written to <paramref name="solutionPath"/>,
		/// one decompiled project each. Public so tests (and scripted callers) can bypass the file picker.
		/// </summary>
		public static Task ExportSolutionAsync(IReadOnlyList<LoadedAssembly> assemblies, string solutionPath,
			DecompilerSettings settings, Language language, DockWorkspace dockWorkspace)
		{
			ArgumentNullException.ThrowIfNull(assemblies);
			ArgumentNullException.ThrowIfNull(settings);
			ArgumentNullException.ThrowIfNull(dockWorkspace);

			var options = OptionsFrom(settings, Path.GetDirectoryName(solutionPath) ?? string.Empty,
				Path.GetFileName(solutionPath));
			return RunExportAsync(assemblies, solutionMode: true, options, settings.Clone(), language, dockWorkspace);
		}

		// The Save Code paths take the settings as they stand instead of asking for per-run overrides, so
		// mirror those settings into the options: ProjectExporter.ApplyOverrides then leaves the settings
		// clone alone and the output matches a plain decompile, only with progress surfaced. No PDB and no
		// strong-name key either -- both are dialog-only features.
		static ProjectExportOptions OptionsFrom(DecompilerSettings settings, string outputDirectory,
			string? solutionFileName = null)
			=> new(
				OutputDirectory: outputDirectory,
				UseSdkStyleProjectFormat: settings.UseSdkStyleProjectFormat,
				UseNestedDirectoriesForNamespaces: settings.UseNestedDirectoriesForNamespaces,
				RemoveDeadCode: settings.RemoveDeadCode,
				RemoveDeadStores: settings.RemoveDeadStores,
				UseDebugSymbols: settings.UseDebugSymbols,
				StrongNameKeyFile: null,
				GeneratePdb: false,
				EmbedSourceFilesInPdb: false,
				SolutionFileName: solutionFileName);

		// Runs the export behind a dedicated frozen tab (so browsing the tree while it runs can't cancel it)
		// and reports the result there, with an Open-folder button on success. The tab is titled after the
		// assemblies being exported, joining their full tree-node labels the same way
		// DecompilerTabPageModel.ComposeBaseTitle titles a multi-node decompile tab -- so a single-assembly
		// export reads as that assembly and a solution export as its members, ellipsised on the tab with the
		// full list shown as a tooltip.
		static Task RunExportAsync(IReadOnlyList<LoadedAssembly> assemblies, bool solutionMode,
			ProjectExportOptions options, DecompilerSettings settingsClone, Language language,
			DockWorkspace dockWorkspace)
		{
			var title = assemblies.Count > 0
				? string.Join(", ", assemblies.Select(a => a.Text))
				: Resources.ExportProjectSolution;
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
