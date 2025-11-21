// Copyright (c) 2018 Siegfried Pammer
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
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;

using Microsoft.Win32;

namespace ICSharpCode.ILSpy
{
	[ExportContextMenuEntry(Header = nameof(Resources.GeneratePortable))]
	[Shared]
	class GeneratePdbContextMenuEntry(LanguageService languageService, DockWorkspace dockWorkspace) : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			var selectedNodes = context.SelectedTreeNodes?.OfType<AssemblyTreeNode>().ToArray();
			if (selectedNodes == null || selectedNodes.Length == 0)
				return;

			if (selectedNodes.Length == 1)
			{
				var assembly = selectedNodes.First().LoadedAssembly;
				if (assembly == null)
					return;
				GeneratePdbForAssembly(assembly, languageService, dockWorkspace);
			}
			else
			{
				GeneratePdbForAssemblies(selectedNodes.Select(n => n.LoadedAssembly), languageService, dockWorkspace);
			}
		}

		public bool IsEnabled(TextViewContext context) => true;

		public bool IsVisible(TextViewContext context)
		{
			var selectedNodes = context.SelectedTreeNodes;
			return selectedNodes?.Any() == true
				&& selectedNodes.All(n => n is AssemblyTreeNode asm && asm.LoadedAssembly.IsLoadedAsValidAssembly);
		}

		internal static void GeneratePdbForAssembly(LoadedAssembly assembly, LanguageService languageService, DockWorkspace dockWorkspace)
		{
			var file = assembly.GetMetadataFileOrNull() as PEFile;
			if (!PortablePdbWriter.HasCodeViewDebugDirectoryEntry(file))
			{
				MessageBox.Show(string.Format(Resources.CannotCreatePDBFile, Path.GetFileName(assembly.FileName)));
				return;
			}
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.FileName = WholeProjectDecompiler.CleanUpFileName(assembly.ShortName, ".pdb");
			dlg.Filter = Resources.PortablePDBPdbAllFiles;
			dlg.InitialDirectory = Path.GetDirectoryName(assembly.FileName);
			if (dlg.ShowDialog() != true)
				return;
			DecompilationOptions options = dockWorkspace.ActiveTabPage.CreateDecompilationOptions();
			string fileName = dlg.FileName;
			dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				Stopwatch stopwatch = Stopwatch.StartNew();
				options.CancellationToken = ct;
				using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					try
					{
						var decompiler = new CSharpDecompiler(file, assembly.GetAssemblyResolver(options.DecompilerSettings.AutoLoadAssemblyReferences), options.DecompilerSettings);
						decompiler.CancellationToken = ct;
						PortablePdbWriter.WritePdb(file, decompiler, options.DecompilerSettings, stream, progress: options.Progress, currentProgressTitle: Resources.GeneratingPortablePDB);
					}
					catch (OperationCanceledException)
					{
						output.WriteLine();
						output.WriteLine(Resources.GenerationWasCancelled);
						throw;
					}
				}
				stopwatch.Stop();
				output.WriteLine(Resources.GenerationCompleteInSeconds, stopwatch.Elapsed.TotalSeconds.ToString("F1"));
				output.WriteLine();
				output.AddButton(null, Resources.OpenExplorer, delegate { Process.Start("explorer", "/select,\"" + fileName + "\""); });
				output.WriteLine();
				return output;
			}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
		}

		internal static void GeneratePdbForAssemblies(System.Collections.Generic.IEnumerable<LoadedAssembly> assemblies, LanguageService languageService, DockWorkspace dockWorkspace)
		{
			var assemblyArray = assemblies?.Where(a => a != null).ToArray();
			if (assemblyArray == null || assemblyArray.Length == 0)
				return;

			// Ensure at least one assembly supports PDB generation
			if (!assemblyArray.Any(a => PortablePdbWriter.HasCodeViewDebugDirectoryEntry(a.GetMetadataFileOrNull() as PEFile)))
			{
				MessageBox.Show(Resources.CannotCreatePDBFile);
				return;
			}

			// Ask for target folder
			using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
			{
				dlg.Description = Resources.SelectPDBOutputFolder;
				dlg.RootFolder = Environment.SpecialFolder.MyComputer;
				dlg.ShowNewFolderButton = true;
				// Show dialog on UI thread
				System.Windows.Forms.DialogResult result = dlg.ShowDialog();
				if (result != System.Windows.Forms.DialogResult.OK || string.IsNullOrWhiteSpace(dlg.SelectedPath))
					return;

				string targetFolder = dlg.SelectedPath;
				DecompilationOptions options = dockWorkspace.ActiveTabPage.CreateDecompilationOptions();

				dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
					AvalonEditTextOutput output = new AvalonEditTextOutput();
					Stopwatch totalWatch = Stopwatch.StartNew();
					options.CancellationToken = ct;

					int total = assemblyArray.Length;
					int processed = 0;
					foreach (var assembly in assemblyArray)
					{
						if (ct.IsCancellationRequested)
						{
							output.WriteLine();
							output.WriteLine(Resources.GenerationWasCancelled);
							throw new OperationCanceledException(ct);
						}

						var file = assembly.GetMetadataFileOrNull() as PEFile;
						if (file == null || !PortablePdbWriter.HasCodeViewDebugDirectoryEntry(file))
						{
							output.WriteLine(string.Format(Resources.CannotCreatePDBFile, Path.GetFileName(assembly.FileName)));
							processed++;
							if (options.Progress != null)
							{
								options.Progress.Report(new DecompilationProgress {
									Title = Resources.GeneratingPortablePDB,
									TotalUnits = total,
									UnitsCompleted = processed
								});
							}
							continue;
						}

						string fileName = Path.Combine(targetFolder, WholeProjectDecompiler.CleanUpFileName(assembly.ShortName, ".pdb"));

						try
						{
							using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
							{
								var decompiler = new CSharpDecompiler(file, assembly.GetAssemblyResolver(options.DecompilerSettings.AutoLoadAssemblyReferences), options.DecompilerSettings);
								decompiler.CancellationToken = ct;
								PortablePdbWriter.WritePdb(file, decompiler, options.DecompilerSettings, stream, progress: options.Progress, currentProgressTitle: Resources.GeneratingPortablePDB);
							}
							output.WriteLine(string.Format(Resources.GeneratedPDBFile, fileName));
						}
						catch (OperationCanceledException)
						{
							output.WriteLine();
							output.WriteLine(Resources.GenerationWasCancelled);
							throw;
						}
						catch (Exception ex)
						{
							output.WriteLine(string.Format(Resources.GenerationFailedForAssembly, assembly.FileName, ex.Message));
						}
						processed++;
						if (options.Progress != null)
						{
							options.Progress.Report(new DecompilationProgress {
								Title = Resources.GeneratingPortablePDB,
								TotalUnits = total,
								UnitsCompleted = processed
							});
						}
					}

					totalWatch.Stop();
					output.WriteLine();
					output.WriteLine(Resources.GenerationCompleteInSeconds, totalWatch.Elapsed.TotalSeconds.ToString("F1"));
					output.WriteLine();
					output.AddButton(null, Resources.OpenExplorer, delegate { Process.Start("explorer", "/select,\"" + targetFolder + "\""); });
					output.WriteLine();
					return output;
				}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
			}
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.GeneratePortable), MenuCategory = nameof(Resources.Save))]
	[Shared]
	class GeneratePdbMainMenuEntry(AssemblyTreeModel assemblyTreeModel, LanguageService languageService, DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override bool CanExecute(object parameter)
		{
			return assemblyTreeModel.SelectedNodes?.Any() == true
				&& assemblyTreeModel.SelectedNodes?.All(n => n is AssemblyTreeNode tn && !tn.LoadedAssembly.HasLoadError) == true;
		}

		public override void Execute(object parameter)
		{
			var selectedNodes = assemblyTreeModel.SelectedNodes?.OfType<AssemblyTreeNode>().ToArray();
			if (selectedNodes == null || selectedNodes.Length == 0)
				return;

			if (selectedNodes.Length == 1)
			{
				var assembly = selectedNodes.First().LoadedAssembly;
				if (assembly == null)
					return;
				GeneratePdbContextMenuEntry.GeneratePdbForAssembly(assembly, languageService, dockWorkspace);
			}
			else
			{
				GeneratePdbContextMenuEntry.GeneratePdbForAssemblies(selectedNodes.Select(n => n.LoadedAssembly), languageService, dockWorkspace);
			}
		}
	}
}
