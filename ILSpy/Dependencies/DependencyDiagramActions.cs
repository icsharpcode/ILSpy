// Copyright (c) 2026 Piero Viano
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
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Dependencies;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>
	/// The actions behind the dependency diagram menu entries and commands: open the in-app
	/// dependency diagram and export it as Mermaid HTML. The heavy part of each (resolving
	/// references) runs off the UI thread.
	/// </summary>
	public static class DependencyDiagramActions
	{
		/// <summary>
		/// The assemblies an action applies to: the assemblies owning the selected nodes, or every
		/// successfully loaded assembly of <paramref name="list"/> when that yields none.
		/// </summary>
		public static IReadOnlyList<LoadedAssembly> GetTargetAssemblies(IEnumerable<SharpTreeNode>? selection, AssemblyList? list)
		{
			var selected = (selection ?? Enumerable.Empty<SharpTreeNode>())
				.Select(n => n.AncestorsAndSelf().OfType<AssemblyTreeNode>().FirstOrDefault())
				.Where(n => n is { LoadedAssembly.IsLoadedAsValidAssembly: true })
				.Select(n => n!.LoadedAssembly)
				.Distinct()
				.ToList();
			if (selected.Count > 0)
				return selected;
			return list?.GetAssemblies().Where(a => a.IsLoadedAsValidAssembly).ToList()
				?? (IReadOnlyList<LoadedAssembly>)Array.Empty<LoadedAssembly>();
		}

		/// <summary>Title of a diagram over <paramref name="roots"/>.</summary>
		public static string DescribeRoots(IReadOnlyList<LoadedAssembly> roots)
		{
			if (roots.Count == 1)
				return roots[0].ShortName;
			var list = roots.FirstOrDefault()?.AssemblyList;
			return list != null && roots.Count == list.GetAssemblies().Count(a => a.IsLoadedAsValidAssembly)
				? list.ListName
				: $"{roots.Count} assemblies";
		}

		/// <summary>
		/// Builds the transitive dependency graph of <paramref name="roots"/> off the UI thread and
		/// opens it in a new document tab. Returns the tab content, or null when there is nothing to show.
		/// </summary>
		public static async Task<DependencyGraphViewModel?> ShowDiagramAsync(IReadOnlyList<LoadedAssembly> roots)
		{
			ArgumentNullException.ThrowIfNull(roots);
			var dockWorkspace = AppComposition.TryGetExport<DockWorkspace>();
			if (roots.Count == 0 || dockWorkspace == null)
				return null;
			var graph = await Task.Run(() => AssemblyDependencyGraph.Build(roots, transitive: true)).ConfigureAwait(true);
			var content = new DependencyGraphViewModel(graph, roots, $"Dependencies: {DescribeRoots(roots)}");
			content.Tab = dockWorkspace.OpenNewTab(content);
			return content;
		}

		/// <summary>
		/// Writes the dependency graph of <paramref name="roots"/> as a Mermaid HTML page to
		/// <paramref name="path"/> (asking for a file when it is null), then shows a short report
		/// tab offering to open the page. Returns the written path, or null when cancelled.
		/// </summary>
		public static async Task<string?> ExportMermaidAsync(IReadOnlyList<LoadedAssembly> roots, string? path)
		{
			ArgumentNullException.ThrowIfNull(roots);
			if (roots.Count == 0)
				return null;
			string title = $"Assembly dependencies: {DescribeRoots(roots)}";
			if (path == null)
			{
				path = await FilePickers.SaveAsync("HTML files|*.html", DescribeRoots(roots) + ".dependencies.html").ConfigureAwait(true);
				if (string.IsNullOrEmpty(path))
					return null;
			}
			string target = path;
			await Task.Run(() => {
				var graph = AssemblyDependencyGraph.Build(roots, transitive: true);
				// Written next to the target and moved into place, so the page never appears half-written.
				string temp = target + ".tmp";
				File.WriteAllText(temp, DependencyGraphMermaidWriter.ToHtml(graph, title), new UTF8Encoding(false));
				File.Move(temp, target, overwrite: true);
			}).ConfigureAwait(true);

			var dockWorkspace = AppComposition.TryGetExport<DockWorkspace>();
			if (dockWorkspace != null)
			{
				var output = new AvaloniaEditTextOutput { Title = Resources.ExportDependencyDiagramMermaid };
				output.WriteLine("Dependency diagram written to:");
				output.WriteLine(target);
				output.WriteLine();
				output.AddButton(null, "Open in Browser", (_, _) => ShellHelper.OpenWithDefaultApplication(target));
				output.WriteLine();
				output.AddRevealFileButton(target);
				dockWorkspace.ShowTextInNewTab(Resources.ExportDependencyDiagramMermaid, output);
			}
			return target;
		}
	}
}
