// Copyright (c) 2024 Christoph Wille for the SharpDevelop Team
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
using System.Windows;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.MermaidDiagrammer;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy.TextView
{
	[ExportContextMenuEntry(Header = nameof(Resources._CreateDiagram), Category = nameof(Resources.Save), Icon = "Images/Save")]
	[Shared]
	sealed class CreateDiagramContextMenuEntry(DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			var assembly = (context.SelectedTreeNodes?.FirstOrDefault() as AssemblyTreeNode)?.LoadedAssembly;
			if (assembly == null)
				return;

			var selectedPath = SelectDestinationFolder();
			if (string.IsNullOrEmpty(selectedPath))
				return;

			dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new() {
					EnableHyperlinks = true
				};
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					var command = new GenerateHtmlDiagrammer {
						Assembly = assembly.FileName,
						OutputFolder = selectedPath
					};

					command.Run();
				}
				catch (OperationCanceledException)
				{
					output.WriteLine();
					output.WriteLine(Resources.GenerationWasCancelled);
					throw;
				}
				stopwatch.Stop();
				output.WriteLine(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1"));
				output.WriteLine();
				output.WriteLine("Learn more: " + "https://github.com/icsharpcode/ILSpy/wiki/Diagramming#tips-for-using-the-html-diagrammer");
				output.WriteLine();

				var diagramHtml = Path.Combine(selectedPath, "index.html");
				output.AddButton(null, Resources.OpenExplorer, delegate { Process.Start("explorer", "/select,\"" + diagramHtml + "\""); });
				output.WriteLine();
				return output;
			}, ct), Properties.Resources.CreatingDiagram).Then(dockWorkspace.ShowText).HandleExceptions();

			return;
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			return context.SelectedTreeNodes?.Length == 1
				&& context.SelectedTreeNodes?.FirstOrDefault() is AssemblyTreeNode tn
				&& tn.LoadedAssembly.IsLoadedAsValidAssembly;
		}

		static string SelectDestinationFolder()
		{
			OpenFolderDialog dialog = new();
			dialog.Multiselect = false;
			dialog.Title = "Select target folder";

			if (dialog.ShowDialog() != true)
			{
				return null;
			}

			string selectedPath = Path.GetDirectoryName(dialog.FolderName);
			bool directoryNotEmpty;
			try
			{
				directoryNotEmpty = Directory.EnumerateFileSystemEntries(selectedPath).Any();
			}
			catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is System.Security.SecurityException)
			{
				MessageBox.Show(
					"The directory cannot be accessed. Please ensure it exists and you have sufficient rights to access it.",
					"Target directory not accessible",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return null;
			}

			return dialog.FolderName;
		}
	}
}
