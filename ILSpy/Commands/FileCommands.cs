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

using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using global::Avalonia.Controls;
using global::Avalonia.Controls.ApplicationLifetimes;
using global::Avalonia.Platform.Storage;

using ICSharpCode.ILSpy.Properties;

using ILSpy.AssemblyTree;
using ILSpy.Views;

namespace ILSpy.Commands
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._Open), MenuIcon = "Images/Open", MenuCategory = nameof(Resources.Open), MenuOrder = 0, InputGestureText = "Ctrl+O")]
	[Shared]
	[method: ImportingConstructor]
	sealed class OpenCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override async void Execute(object? parameter)
		{
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

			foreach (var f in files)
			{
				if (f.TryGetLocalPath() is { } path)
					assemblyTreeModel.OpenAssembly(path);
			}
		}
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.OpenFrom_GAC), MenuIcon = "Images/AssemblyListGAC", MenuCategory = nameof(Resources.Open), MenuOrder = 1)]
	[Shared]
	sealed class OpenFromGacCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources.OpenFrom_GAC);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.ManageAssembly_Lists), MenuIcon = "Images/AssemblyList", MenuCategory = nameof(Resources.Open), MenuOrder = 1.7)]
	[Shared]
	sealed class ManageAssemblyListsCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources.ManageAssembly_Lists);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._Reload), MenuIcon = "Images/Refresh", MenuCategory = nameof(Resources.Open), MenuOrder = 2, InputGestureText = "F5")]
	[Shared]
	sealed class RefreshCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources._Reload);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDecompile), MenuCategory = nameof(Resources.Open), MenuOrder = 2.5)]
	[Shared]
	sealed class DecompileAllCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources.DEBUGDecompile);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDecompile100x), MenuCategory = nameof(Resources.Open), MenuOrder = 2.6)]
	[Shared]
	sealed class Decompile100TimesCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources.DEBUGDecompile100x);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDisassemble), MenuCategory = nameof(Resources.Open), MenuOrder = 2.5)]
	[Shared]
	sealed class DisassembleAllCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources.DEBUGDisassemble);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.DEBUGDumpPDBAsXML), MenuCategory = nameof(Resources.Open), MenuOrder = 2.6)]
	[Shared]
	sealed class Pdb2XmlCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources.DEBUGDumpPDBAsXML);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._RemoveAssembliesWithLoadErrors), MenuCategory = nameof(Resources.Remove), MenuOrder = 2.6)]
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

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.ClearAssemblyList), MenuCategory = nameof(Resources.Remove), MenuOrder = 2.6)]
	[Shared]
	[method: ImportingConstructor]
	sealed class ClearAssemblyListCommand(AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter)
			=> (assemblyTreeModel.AssemblyList?.Count ?? 0) > 0;

		public override void Execute(object? parameter) => assemblyTreeModel.AssemblyList?.Clear();
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources._SaveCode), MenuIcon = "Images/Save", MenuCategory = nameof(Resources.Save), MenuOrder = 0, InputGestureText = "Ctrl+S")]
	[Shared]
	sealed class SaveCommand : SimpleCommand
	{
		public override void Execute(object? parameter) => NotImplementedDialog.Show(Resources._SaveCode);
	}

	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._File), Header = nameof(Resources.GeneratePortable), MenuCategory = nameof(Resources.Save))]
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
