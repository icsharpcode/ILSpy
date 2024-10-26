// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

#if DEBUG

using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using Microsoft.DiaSymReader.Tools;

namespace ICSharpCode.ILSpy
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDumpPDBAsXML), MenuCategory = nameof(Resources.Open), MenuOrder = 2.6)]
	[Shared]
	sealed class Pdb2XmlCommand(AssemblyTreeModel assemblyTreeModel, DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			var selectedNodes = assemblyTreeModel.SelectedNodes;
			return selectedNodes?.Any() == true
				&& selectedNodes.All(n => n is AssemblyTreeNode asm && !asm.LoadedAssembly.HasLoadError);
		}

		public override void Execute(object parameter)
		{
			Execute(assemblyTreeModel.SelectedNodes.OfType<AssemblyTreeNode>(), dockWorkspace);
		}

		internal static void Execute(IEnumerable<AssemblyTreeNode> nodes, DockWorkspace dockWorkspace)
		{
			var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
			var options = PdbToXmlOptions.IncludeEmbeddedSources | PdbToXmlOptions.IncludeMethodSpans | PdbToXmlOptions.IncludeTokens;
			dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				var writer = new TextOutputWriter(output);
				foreach (var node in nodes)
				{
					string pdbFileName = Path.ChangeExtension(node.LoadedAssembly.FileName, ".pdb");
					if (!File.Exists(pdbFileName))
						continue;
					using (var pdbStream = File.OpenRead(pdbFileName))
					using (var peStream = File.OpenRead(node.LoadedAssembly.FileName))
						PdbToXmlConverter.ToXml(writer, pdbStream, peStream, options);
				}
				return output;
			}, ct)).Then(output => dockWorkspace.ShowNodes(output, null, highlighting)).HandleExceptions();
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources.DEBUGDumpPDBAsXML))]
	[Shared]
	class Pdb2XmlCommandContextMenuEntry(DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			Pdb2XmlCommand.Execute(context.SelectedTreeNodes.OfType<AssemblyTreeNode>(), dockWorkspace);
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			var selectedNodes = context.SelectedTreeNodes;
			return selectedNodes?.Any() == true
				&& selectedNodes.All(n => n is AssemblyTreeNode asm && asm.LoadedAssembly.IsLoadedAsValidAssembly);
		}
	}

}

#endif
