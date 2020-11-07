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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using Microsoft.DiaSymReader.Tools;

namespace ICSharpCode.ILSpy
{
	[ExportMainMenuCommand(Menu = nameof(Resources._File), Header = nameof(Resources.DEBUGDumpPDBAsXML), MenuCategory = nameof(Resources.Open), MenuOrder = 2.6)]
	sealed class Pdb2XmlCommand : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			var selectedNodes = MainWindow.Instance.SelectedNodes;
			return selectedNodes?.Any() == true
				&& selectedNodes.All(n => n is AssemblyTreeNode asm && !asm.LoadedAssembly.HasLoadError);
		}

		public override void Execute(object parameter)
		{
			Execute(MainWindow.Instance.SelectedNodes.OfType<AssemblyTreeNode>());
		}

		internal static void Execute(IEnumerable<AssemblyTreeNode> nodes)
		{
			var highlighting = HighlightingManager.Instance.GetDefinitionByExtension(".xml");
			var options = PdbToXmlOptions.IncludeEmbeddedSources | PdbToXmlOptions.IncludeMethodSpans | PdbToXmlOptions.IncludeTokens;
			Docking.DockWorkspace.Instance.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
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
			}, ct)).Then(output => Docking.DockWorkspace.Instance.ShowNodes(output, null, highlighting)).HandleExceptions();
		}
	}

	[ExportContextMenuEntry(Header = nameof(Resources.DEBUGDumpPDBAsXML))]
	class Pdb2XmlCommandContextMenuEntry : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			Pdb2XmlCommand.Execute(context.SelectedTreeNodes.OfType<AssemblyTreeNode>());
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
