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

#if DEBUG && WINDOWS
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using Microsoft.DiaSymReader.Tools;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// DEBUG-only main-menu entry: Dump every selected assembly's PDB as XML in the active
	/// decompiler tab via <see cref="PdbToXmlConverter"/>. Windows-only because
	/// <c>Microsoft.DiaSymReader</c> uses native COM interop. WPF gates this command
	/// identically (<c>#if DEBUG &amp;&amp; WINDOWS</c>).
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDumpPDBAsXML), MenuCategory = "Debug", MenuOrder = 33)]
	[Shared]
	sealed class Pdb2XmlCommand : SimpleCommand
	{
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly DockWorkspace dockWorkspace;

		[ImportingConstructor]
		public Pdb2XmlCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.dockWorkspace = dockWorkspace;
		}

		public override bool CanExecute(object? parameter)
		{
			var selected = assemblyTreeModel.SelectedItems;
			return selected?.Count > 0
				&& selected.All(n => n is AssemblyTreeNode asm && !asm.LoadedAssembly.HasLoadError);
		}

		public override void Execute(object? parameter)
		{
			var nodes = assemblyTreeModel.SelectedItems.OfType<AssemblyTreeNode>().ToArray();
			if (nodes.Length == 0)
				return;
			ExecuteAsync(nodes).HandleExceptions();
		}

		async Task ExecuteAsync(AssemblyTreeNode[] nodes)
		{
			var options = PdbToXmlOptions.IncludeEmbeddedSources
				| PdbToXmlOptions.IncludeMethodSpans
				| PdbToXmlOptions.IncludeTokens;
			// Run in a dedicated frozen tab so navigation cannot cancel this long run.
			await dockWorkspace.RunInNewTabAsync("Dumping PDB as XML…", token => Task.Run(() => {
				var output = new AvaloniaEditTextOutput { Title = "PDB as XML", SyntaxExtensionOverride = ".xml" };
				var writer = new TextOutputWriter(output);
				foreach (var node in nodes)
				{
					var pdbFileName = Path.ChangeExtension(node.LoadedAssembly.FileName, ".pdb");
					if (!File.Exists(pdbFileName))
						continue;
					using var pdbStream = File.OpenRead(pdbFileName);
					using var peStream = File.OpenRead(node.LoadedAssembly.FileName);
					PdbToXmlConverter.ToXml(writer, pdbStream, peStream, options);
				}
				return output;
			}, token));
		}
	}

	/// <summary>
	/// Adapter that lets <see cref="PdbToXmlConverter"/>'s TextWriter API write into our
	/// <see cref="AvaloniaEditTextOutput"/> sink.
	/// </summary>
	internal sealed class TextOutputWriter(ICSharpCode.Decompiler.ITextOutput output) : System.IO.TextWriter
	{
		public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
		public override void Write(char value) => output.Write(value);
		public override void Write(string? value)
		{
			if (value != null)
				output.Write(value);
		}
		public override void WriteLine() => output.WriteLine();
	}
}
#endif
