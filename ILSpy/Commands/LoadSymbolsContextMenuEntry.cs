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

using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.Symbols;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click one or more assemblies -> "Load Symbols from Symbol Server". Looks up each
	/// assembly's PDB along the configured symbol path (downloading it if needed), then
	/// re-decompiles the current selection so the symbols feed into the output. Assemblies whose
	/// PDB is not found are listed in a report.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.LoadSymbolsFromSymbolServer), Category = "Debug", Icon = "Images/ProgramDebugDatabase", Order = 405)]
	[Shared]
	public sealed class LoadSymbolsContextMenuEntry : IContextMenuEntry
	{
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly DockWorkspace dockWorkspace;
		readonly SymbolService symbolService;

		[ImportingConstructor]
		public LoadSymbolsContextMenuEntry(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace, SymbolService symbolService)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.dockWorkspace = dockWorkspace;
			this.symbolService = symbolService;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
			=> context.SelectedTreeNodes is { Length: > 0 } nodes
				&& nodes.All(n => n is AssemblyTreeNode asm && asm.LoadedAssembly.IsLoadedAsValidAssembly);

		public void Execute(TextViewContext context)
		{
			var assemblies = context.SelectedTreeNodes?.OfType<AssemblyTreeNode>().Select(n => n.LoadedAssembly).ToArray();
			if (assemblies is not { Length: > 0 })
				return;
			ExecuteAsync(assemblies).HandleExceptions();
		}

		async Task ExecuteAsync(IReadOnlyList<LoadedAssembly> assemblies)
		{
			var locator = symbolService.Locator;
			// An explicit request re-probes locations that missed earlier (e.g. a server that was down).
			locator.ClearCache();
			var notFound = new List<LoadedAssembly>();
			foreach (var assembly in assemblies)
			{
				if (await assembly.LoadDebugInfoFromSymbolPathAsync(locator).ConfigureAwait(true) == null)
					notFound.Add(assembly);
			}
			if (notFound.Count < assemblies.Count && assemblyTreeModel.SelectedItem is { } current)
			{
				assemblyTreeModel.SelectedItem = null;
				assemblyTreeModel.SelectedItem = current;
			}
			if (notFound.Count > 0)
			{
				var output = new AvaloniaEditTextOutput { Title = Resources.LoadSymbolsFromSymbolServer };
				output.WriteLine("No matching symbols were found for:");
				foreach (var assembly in notFound)
					output.WriteLine("  " + Path.GetFileName(assembly.FileName));
				output.WriteLine();
				output.WriteLine("Searched:");
				foreach (var element in locator.SymbolPath.Elements)
					output.WriteLine("  " + element.Location + (element.IsHttp ? " (cache: " + element.CacheDirectory + ")" : ""));
				dockWorkspace.ShowTextInNewTab(Resources.LoadSymbolsFromSymbolServer, output);
			}
		}
	}
}
