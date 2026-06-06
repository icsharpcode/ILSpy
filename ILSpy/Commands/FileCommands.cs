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

using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using global::Avalonia.Controls;
using global::Avalonia.Controls.ApplicationLifetimes;
using global::Avalonia.Platform.Storage;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;

using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.Docking;
using ILSpy.Languages;
using ILSpy.TreeNodes;
using ILSpy.Views;

namespace ILSpy.Commands
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._Open), MenuIcon = "Images/Open", MenuCategory = nameof(Resources.Open), MenuOrder = 0, InputGestureText = "Ctrl+O")]
	[ExportToolbarCommand(ToolTip = nameof(Resources.Open), ToolbarIcon = "Images/Open", ToolbarCategory = nameof(Resources.Open), ToolbarOrder = 0)]
	[Shared]
	[method: ImportingConstructor]
	sealed class OpenCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override async void Execute(object? parameter)
		{
			// Tests / scripted callers can pass paths directly and bypass the file picker.
			if (parameter is string singlePath)
			{
				assemblyTreeModel.OpenFiles(new[] { singlePath });
				return;
			}
			if (parameter is IEnumerable<string> paths)
			{
				assemblyTreeModel.OpenFiles(paths.ToArray());
				return;
			}

			var owner = (global::Avalonia.Application.Current?.ApplicationLifetime
				as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			if (owner == null)
				return;

			var files = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
				Title = Resources.Open,
				AllowMultiple = true,
				FileTypeFilter = new[] {
					new FilePickerFileType(".NET assemblies") {
						Patterns = new[] { "*.dll", "*.exe", "*.winmd", "*.wasm" },
					},
					new FilePickerFileType("NuGet packages") { Patterns = new[] { "*.nupkg" } },
					new FilePickerFileType("Portable PDB") { Patterns = new[] { "*.pdb" } },
					FilePickerFileTypes.All,
				},
			});

			var picked = files
				.Select(f => f.TryGetLocalPath())
				.Where(p => !string.IsNullOrEmpty(p))
				.ToArray()!;
			if (picked.Length > 0)
				assemblyTreeModel.OpenFiles(picked!);
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.OpenFrom_GAC), MenuIcon = "Images/AssemblyListGAC", MenuCategory = nameof(Resources.Open), MenuOrder = 1)]
	[Shared]
	sealed class OpenFromGacCommand : SimpleCommand
	{
		readonly AssemblyTreeModel assemblyTreeModel;

		[ImportingConstructor]
		public OpenFromGacCommand(AssemblyTreeModel assemblyTreeModel)
		{
			this.assemblyTreeModel = assemblyTreeModel;
		}

		// The Windows GAC doesn't exist on Linux/macOS. WPF gates this command on
		// `AppEnvironment.IsWindows`; Avalonia uses the BCL runtime check that already
		// returns the right answer per OS.
		public override bool CanExecute(object? parameter) => System.OperatingSystem.IsWindows();

		public override void Execute(object? parameter)
		{
			if (!CanExecute(parameter))
				return;
			var owner = (global::Avalonia.Application.Current?.ApplicationLifetime
				as global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			if (owner == null)
				return;
			_ = ShowAsync(owner);
		}

		async System.Threading.Tasks.Task ShowAsync(global::Avalonia.Controls.Window owner)
		{
			var dlg = new Views.OpenFromGacDialog();
			var fileNames = await dlg.ShowDialog<string[]>(owner);
			if (fileNames == null || fileNames.Length == 0)
				return;
			foreach (var name in fileNames)
				assemblyTreeModel.AssemblyList?.OpenAssembly(name);
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.ManageAssembly_Lists), MenuIcon = "Images/AssemblyList", MenuCategory = "AssemblyList", MenuOrder = 10)]
	[Shared]
	sealed class ManageAssemblyListsCommand : SimpleCommand
	{
		readonly SettingsService settingsService;

		[ImportingConstructor]
		public ManageAssemblyListsCommand(SettingsService settingsService)
		{
			this.settingsService = settingsService;
		}

		public override void Execute(object? parameter)
		{
			var owner = (global::Avalonia.Application.Current?.ApplicationLifetime
				as global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
			if (owner == null)
				return;
			var dlg = new Views.ManageAssemblyListsDialog(settingsService);
			_ = dlg.ShowDialog(owner);
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._Reload), MenuIcon = "Images/Refresh", MenuCategory = nameof(Resources.Open), MenuOrder = 2, InputGestureText = "F5")]
	[ExportToolbarCommand(ToolTip = nameof(Resources.RefreshCommand_ReloadAssemblies), ToolbarIcon = "Images/Refresh", ToolbarCategory = nameof(Resources.Open), ToolbarOrder = 2)]
	[Shared]
	[method: ImportingConstructor]
	sealed class RefreshCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override void Execute(object? parameter) => assemblyTreeModel.Refresh();
	}

	// DEBUG-only DecompileAllCommand + Decompile100TimesCommand live in DecompileAllCommand.cs;
	// the stubs that lived here were replaced with the real implementations in `<commit>`.

	// DEBUG-only DisassembleAllCommand also moved to DecompileAllCommand.cs to keep all
	// three parallel-decompile/disassemble stress-test commands in one place.

	// DEBUG-only Pdb2XmlCommand moved to its own file with `#if DEBUG && WINDOWS` gating.
	// On non-Windows or non-Debug builds the entry simply isn't compiled.

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._RemoveAssembliesWithLoadErrors), MenuCategory = "AssemblyList", MenuOrder = 11)]
	[Shared]
	[method: ImportingConstructor]
	sealed class RemoveAssembliesWithLoadErrors(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter)
			=> assemblyTreeModel.AssemblyList?.GetAssemblies().Any(l => l.HasLoadError) ?? false;

		public override void Execute(object? parameter)
		{
			var list = assemblyTreeModel.AssemblyList;
			if (list == null)
				return;
			foreach (var assembly in list.GetAssemblies())
			{
				if (assembly.HasLoadError)
					list.Unload(assembly);
			}
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.ClearAssemblyList), MenuCategory = "AssemblyList", MenuOrder = 12)]
	[Shared]
	[method: ImportingConstructor]
	sealed class ClearAssemblyListCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter)
			=> (assemblyTreeModel.AssemblyList?.Count ?? 0) > 0;

		public override void Execute(object? parameter) => assemblyTreeModel.AssemblyList?.Clear();
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._SaveCode), MenuIcon = "Images/Save", MenuCategory = nameof(Resources.Save), MenuOrder = 20, InputGestureText = "Ctrl+S")]
	[Shared]
	[method: ImportingConstructor]
	internal sealed class SaveCommand(
		AssemblyTreeModel assemblyTreeModel,
		LanguageService languageService,
		DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override bool CanExecute(object? parameter)
			=> assemblyTreeModel.SelectedItem is ILSpyTreeNode;

		public override async void Execute(object? parameter)
		{
			// Several selected assemblies export a Visual Studio solution (one project each),
			// matching the Save Code context-menu entry.
			if (SolutionExport.TryGetAssemblies(assemblyTreeModel.SelectedItems, out var assemblies))
			{
				await SolutionExport.PromptAndExportAsync(assemblies, languageService.CurrentLanguage, dockWorkspace);
				return;
			}

			if (assemblyTreeModel.SelectedItem is not ILSpyTreeNode node)
				return;
			// Resource nodes (and any future override) handle their own save format and dialog;
			// only fall through to the generic "save code" path when the node says it didn't
			// claim the request.
			if (node.Save())
				return;

			var language = languageService.CurrentLanguage;
			var defaultName = "output" + language.FileExtension;
			var path = await FilePickers.SaveAsync(
				$"{language.Name} (*{language.FileExtension})|*{language.FileExtension}|All files|*.*",
				defaultName).ConfigureAwait(false);
			if (path == null)
				return;
			await SaveCodeAsync(path).ConfigureAwait(false);
		}

		/// <summary>
		/// Public for tests + scripted callers: re-decompiles the currently selected node with
		/// <see cref="DecompilationOptions.FullDecompilation"/> on and writes the output to
		/// <paramref name="path"/> as plain text. Drives the taskbar progress while running so
		/// long saves give visual feedback.
		/// </summary>
		public async Task SaveCodeAsync(string path)
		{
			if (assemblyTreeModel.SelectedItem is not ILSpyTreeNode node)
				return;

			var taskbar = TryGetExport<TaskbarProgressService>();
			var language = languageService.CurrentLanguage;
			var options = new DecompilationOptions {
				FullDecompilation = true,
				EscapeInvalidIdentifiers = true,
			};
			taskbar?.SetState(TaskbarProgressState.Indeterminate);
			try
			{
				await Task.Run(() => {
					using var writer = new StreamWriter(path);
					var output = new ICSharpCode.Decompiler.PlainTextOutput(writer);
					try
					{
						node.Decompile(language, output, options);
					}
					catch (System.OperationCanceledException)
					{
						writer.WriteLine();
						writer.WriteLine("// Decompilation was cancelled.");
					}
				}).ConfigureAwait(false);
			}
			finally
			{
				taskbar?.SetState(TaskbarProgressState.None);
			}
		}

		static T? TryGetExport<T>() where T : class
		{
			try
			{ return AppComposition.Current.GetExport<T>(); }
			catch { return null; }
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.ExportProjectSolution), MenuIcon = "Images/Save", MenuCategory = nameof(Resources.Save), MenuOrder = 21)]
	[Shared]
	[method: ImportingConstructor]
	internal sealed class ExportProjectSolutionCommand(
		AssemblyTreeModel assemblyTreeModel,
		LanguageService languageService,
		DockWorkspace dockWorkspace,
		SettingsService settingsService) : SimpleCommand
	{
		public override bool CanExecute(object? parameter)
			=> ProjectExport.TryGetExportableAssemblies(assemblyTreeModel.SelectedItems, out _, out _);

		public override void Execute(object? parameter)
		{
			if (!ProjectExport.TryGetExportableAssemblies(assemblyTreeModel.SelectedItems, out var assemblies, out var solutionMode))
				return;
			_ = ProjectExport.PromptAndExportAsync(assemblies, solutionMode,
				languageService.CurrentLanguage, dockWorkspace, settingsService);
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.GeneratePortable), MenuCategory = nameof(Resources.Save), MenuOrder = 22)]
	[Shared]
	sealed class GeneratePdbCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources.GeneratePortable);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.E_xit), MenuOrder = 99999, MenuCategory = nameof(Resources.Exit))]
	[Shared]
	sealed class ExitCommand : SimpleCommand
	{
		public override void Execute(object? parameter)
		{
			(global::Avalonia.Application.Current?.ApplicationLifetime
				as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
		}
	}
}
