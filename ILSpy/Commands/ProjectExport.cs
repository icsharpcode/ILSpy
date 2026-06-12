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
			// Run in a dedicated frozen tab so browsing the tree while the export runs can't cancel it.
			await dockWorkspace.RunInNewTabAsync(ICSharpCode.ILSpy.Properties.Resources.ExportProjectSolution, async (token, progress) => {
				var result = await ProjectExporter.ExportAsync(assemblies, solutionMode, options, settingsClone, language, progress, token)
					.ConfigureAwait(false);
				var o = new AvaloniaEditTextOutput { Title = ICSharpCode.ILSpy.Properties.Resources.ExportProjectSolution };
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
