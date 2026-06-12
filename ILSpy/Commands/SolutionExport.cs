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

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// The shared "export several assemblies as a Visual Studio solution" flow behind both the
	/// File &rarr; Save Code command and the Save Code context-menu entry: it recognises a
	/// solution-eligible selection, prompts for the <c>.sln</c> path, runs
	/// <see cref="SolutionWriter"/> behind the tab's cancellable progress UI, and surfaces the
	/// status report in the active decompiler tab.
	/// </summary>
	internal static class SolutionExport
	{
		/// <summary>
		/// True when <paramref name="nodes"/> is several assembly nodes that all loaded as valid
		/// assemblies — the only selection shape that maps onto a multi-project solution.
		/// </summary>
		public static bool TryGetAssemblies(IReadOnlyList<SharpTreeNode>? nodes, out List<LoadedAssembly> assemblies)
		{
			assemblies = new List<LoadedAssembly>();
			if (nodes is not { Count: > 1 })
				return false;
			if (!nodes.All(n => n is AssemblyTreeNode { LoadedAssembly.IsLoadedAsValidAssembly: true }))
				return false;
			assemblies = nodes.OfType<AssemblyTreeNode>().Select(n => n.LoadedAssembly).ToList();
			return true;
		}

		/// <summary>
		/// Prompts for a target <c>.sln</c> file and exports <paramref name="assemblies"/> into it,
		/// one decompiled project each. The export runs in its own frozen tab so browsing the tree
		/// can't cancel it, and the status report lands there. Does nothing if the user cancels the picker.
		/// </summary>
		public static async Task PromptAndExportAsync(IReadOnlyList<LoadedAssembly> assemblies,
			Language language, DockWorkspace dockWorkspace)
		{
			var path = await FilePickers.SaveAsync(
				Resources.VisualStudioSolutionFileSlnAllFiles, "Solution.sln", Resources._SaveCode)
				.ConfigureAwait(true);
			if (string.IsNullOrEmpty(path))
				return;

			// Snapshot the user's current decompiler settings for the whole export, like the
			// per-project export path does.
			var settings = AppEnv.AppComposition.TryGetExport<SettingsService>()?.CreateEffectiveDecompilerSettings()
				?? new ICSharpCode.Decompiler.DecompilerSettings();

			// Run in a dedicated frozen tab so browsing the tree while the export runs can't cancel it.
			await dockWorkspace.RunInNewTabAsync("Exporting solution", async (token, progress) => {
				var result = await SolutionWriter.CreateSolutionAsync(path, language, assemblies, token,
						settings, strongNameKeyFile: null, progress: progress)
					.ConfigureAwait(false);
				var o = new AvaloniaEditTextOutput { Title = Resources._SaveCode };
				o.Write(result.StatusText);
				o.WriteLine();
				if (result.Success && Path.GetDirectoryName(path) is { Length: > 0 } directory)
				{
					o.AddOpenFolderButton(directory);
				}
				return o;
			}).ConfigureAwait(true);
		}

	}
}
