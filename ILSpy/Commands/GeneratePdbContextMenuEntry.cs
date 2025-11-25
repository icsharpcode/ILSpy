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
using System.Collections.Generic;
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
						PortablePdbWriter.WritePdb(file, decompiler, options.DecompilerSettings, stream, progress: options.Progress, currentProgressTitle: string.Format(Resources.GeneratingPortablePDB, Path.GetFileName(assembly.FileName)));
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
				output.AddButton(null, Resources.OpenExplorer, delegate { ShellHelper.OpenFolderAndSelectItem(fileName); });
				output.WriteLine();
				return output;
			}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
		}

		internal static void GeneratePdbForAssemblies(IEnumerable<LoadedAssembly> assemblies, LanguageService languageService, DockWorkspace dockWorkspace)
		{
			var assemblyArray = assemblies?.Where(a => a != null).ToArray() ?? [];
			if (assemblyArray == null || assemblyArray.Length == 0)
				return;

			// Ensure at least one assembly supports PDB generation
			var supported = new Dictionary<LoadedAssembly, PEFile>();
			var unsupported = new List<LoadedAssembly>();
			foreach (var a in assemblyArray)
			{
				try
				{
					var file = a.GetMetadataFileOrNull() as PEFile;
					if (PortablePdbWriter.HasCodeViewDebugDirectoryEntry(file))
						supported.Add(a, file);
					else
						unsupported.Add(a);
				}
				catch
				{
					unsupported.Add(a);
				}
			}
			if (supported.Count == 0)
			{
				// none can be generated
				string msg = string.Format(Resources.CannotCreatePDBFile, ":" + Environment.NewLine +
					string.Join(Environment.NewLine, unsupported.Select(u => Path.GetFileName(u.FileName)))
					+ Environment.NewLine);
				MessageBox.Show(msg);
				return;
			}

			// Ask for target folder
			var dlg = new OpenFolderDialog();
			dlg.Title = Resources.SelectPDBOutputFolder;
			if (dlg.ShowDialog() != true || string.IsNullOrWhiteSpace(dlg.FolderName))
				return;

			string targetFolder = dlg.FolderName;
			DecompilationOptions options = dockWorkspace.ActiveTabPage.CreateDecompilationOptions();

			dockWorkspace.RunWithCancellation(ct => Task<AvalonEditTextOutput>.Factory.StartNew(() => {
				AvalonEditTextOutput output = new AvalonEditTextOutput();
				Stopwatch totalWatch = Stopwatch.StartNew();
				options.CancellationToken = ct;

				int total = assemblyArray.Length;
				int processed = 0;
				foreach (var assembly in assemblyArray)
				{
					// only process supported assemblies
					if (!supported.TryGetValue(assembly, out var file))
					{
						output.WriteLine(string.Format(Resources.CannotCreatePDBFile, Path.GetFileName(assembly.FileName)));
						processed++;
						if (options.Progress != null)
						{
							options.Progress.Report(new DecompilationProgress {
								Title = string.Format(Resources.GeneratingPortablePDB, Path.GetFileName(assembly.FileName)),
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
							PortablePdbWriter.WritePdb(file, decompiler, options.DecompilerSettings, stream, progress: options.Progress, currentProgressTitle: string.Format(Resources.GeneratingPortablePDB, Path.GetFileName(assembly.FileName)));
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
							Title = string.Format(Resources.GeneratingPortablePDB, Path.GetFileName(assembly.FileName)),
							TotalUnits = total,
							UnitsCompleted = processed
						});
					}
				}

				totalWatch.Stop();
				output.WriteLine();
				output.WriteLine(Resources.GenerationCompleteInSeconds, totalWatch.Elapsed.TotalSeconds.ToString("F1"));
				output.WriteLine();
				// Select all generated pdb files in explorer
				var generatedFiles = assemblyArray
					.Select(a => Path.Combine(targetFolder, WholeProjectDecompiler.CleanUpFileName(a.ShortName, ".pdb")))
					.Where(File.Exists)
					.ToList();
				if (generatedFiles.Any())
				{
					output.AddButton(null, Resources.OpenExplorer, delegate { ShellHelper.OpenFolderAndSelectItems(generatedFiles); });
				}
				else
				{
					output.AddButton(null, Resources.OpenExplorer, delegate { ShellHelper.OpenFolder(targetFolder); });
				}
				output.WriteLine();
				return output;
			}, ct)).Then(dockWorkspace.ShowText).HandleExceptions();
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
