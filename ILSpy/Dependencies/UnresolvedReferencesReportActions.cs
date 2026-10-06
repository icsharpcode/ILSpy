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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Dependencies;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Dependencies
{
	/// <summary>
	/// The action behind the unresolved-references report: collect the assembly references that do
	/// not resolve (off the UI thread) and show them, with their probe logs, in a new document tab.
	/// </summary>
	public static class UnresolvedReferencesReportActions
	{
		/// <summary>
		/// The assemblies the report applies to: the selected assembly nodes (with
		/// <paramref name="includeOwnersOfSelection"/>, also the assemblies owning any other selected
		/// node), or every successfully loaded assembly of <paramref name="list"/> when that yields none.
		/// </summary>
		public static IReadOnlyList<LoadedAssembly> GetTargetAssemblies(IEnumerable<SharpTreeNode>? selection, AssemblyList? list,
			bool includeOwnersOfSelection = true)
		{
			var selected = (selection ?? Enumerable.Empty<SharpTreeNode>())
				.Select(n => includeOwnersOfSelection
					? n.AncestorsAndSelf().OfType<AssemblyTreeNode>().FirstOrDefault()
					: n as AssemblyTreeNode)
				.Where(n => n is { LoadedAssembly.IsLoadedAsValidAssembly: true })
				.Select(n => n!.LoadedAssembly)
				.Distinct()
				.ToList();
			if (selected.Count > 0)
				return selected;
			return list?.GetAssemblies().Where(a => a.IsLoadedAsValidAssembly).ToList()
				?? (IReadOnlyList<LoadedAssembly>)Array.Empty<LoadedAssembly>();
		}

		/// <summary>Opens the unresolved-references report for <paramref name="assemblies"/> in a new tab.</summary>
		public static Task ShowUnresolvedReferencesReportAsync(IReadOnlyList<LoadedAssembly> assemblies)
		{
			ArgumentNullException.ThrowIfNull(assemblies);
			var dockWorkspace = AppComposition.TryGetExport<DockWorkspace>();
			if (dockWorkspace == null)
				return Task.CompletedTask;
			return dockWorkspace.RunInNewTabAsync(Resources.UnresolvedReferencesReport,
				token => Task.Run(() => CreateUnresolvedReferencesReport(assemblies, token), token));
		}

		/// <summary>Renders the unresolved-references report, grouped by referencing assembly.</summary>
		internal static AvaloniaEditTextOutput CreateUnresolvedReferencesReport(IReadOnlyList<LoadedAssembly> assemblies, CancellationToken token)
		{
			var groups = UnresolvedReferencesReport.Collect(assemblies, token);
			var output = new AvaloniaEditTextOutput { Title = Resources.UnresolvedReferencesReport };
			int total = groups.Sum(g => g.References.Count);
			output.WriteLine($"{total} unresolved assembly reference(s) in {groups.Count} of {assemblies.Count} assemblies.");
			output.WriteLine();
			foreach (var group in groups)
			{
				output.WriteLine($"{group.Assembly.ShortName} ({group.Assembly.FileName})");
				output.Indent();
				foreach (var unresolved in group.References)
				{
					output.WriteLine(unresolved.Reference.FullName);
					output.Indent();
					var log = new UnresolvedAssemblyNameReference(unresolved.Reference.FullName);
					log.Messages.AddRange(unresolved.Messages.Select(m => (m.Kind, m.Message)));
					AssemblyReferenceTreeNode.PrintAssemblyLoadLogMessages(output, log);
					output.Unindent();
				}
				output.Unindent();
				output.WriteLine();
			}
			return output;
		}
	}
}
