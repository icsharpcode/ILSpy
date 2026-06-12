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
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.MermaidDiagrammer;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click an assembly → "Create Diagram". Asks for an output folder, invokes the
	/// shared <see cref="GenerateHtmlDiagrammer"/> engine on a background thread, then
	/// pushes a completion report (elapsed time, learn-more link, Open-Explorer button)
	/// into the active decompiler tab via <see cref="DockWorkspace.ShowText"/>. Visible
	/// only when exactly one valid loaded assembly is selected.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources._CreateDiagram), Category = nameof(Resources.Save), Icon = "Images/Save", Order = 310)]
	[Shared]
	public sealed class CreateDiagramContextMenuEntry : IContextMenuEntry
	{
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public CreateDiagramContextMenuEntry(DockWorkspace dockWorkspace)
		{
			this.dockWorkspace = dockWorkspace;
		}

		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1
				&& context.SelectedTreeNodes[0] is AssemblyTreeNode tn
				&& tn.LoadedAssembly.IsLoadedAsValidAssembly;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			var assembly = (context.SelectedTreeNodes?.FirstOrDefault() as AssemblyTreeNode)?.LoadedAssembly;
			if (assembly == null)
				return;
			ExecuteAsync(assembly.FileName).HandleExceptions();
		}

		async Task ExecuteAsync(string assemblyFile)
		{
			var outputFolder = await FilePickers.PickFolderAsync("Select target folder");
			if (string.IsNullOrEmpty(outputFolder))
				return;

			// Run in a dedicated frozen tab so browsing the tree while the diagram generates can't cancel it.
			await dockWorkspace.RunInNewTabAsync(Resources.CreatingDiagram,
				token => Task.Run(() => RunGenerator(assemblyFile, outputFolder), token)).ConfigureAwait(true);
		}

		static AvaloniaEditTextOutput RunGenerator(string assemblyFile, string outputFolder)
		{
			var output = new AvaloniaEditTextOutput();
			var stopwatch = Stopwatch.StartNew();
			var command = new GenerateHtmlDiagrammer {
				Assembly = assemblyFile,
				OutputFolder = outputFolder,
			};
			command.Run();
			stopwatch.Stop();
			output.Title = "Create Diagram";
			output.Write(string.Format(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1")));
			output.WriteLine();
			output.WriteLine();
			output.Write("Learn more: https://github.com/icsharpcode/ILSpy/wiki/Diagramming#tips-for-using-the-html-diagrammer");
			output.WriteLine();
			output.WriteLine();
			var diagramHtml = Path.Combine(outputFolder, "index.html");
			output.AddRevealFileButton(diagramHtml);
			return output;
		}

	}
}
