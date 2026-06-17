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
using System.IO;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class CommandLineArgumentsTests
{
	[AvaloniaTest]
	public async Task Language_Arg_Selects_The_Named_Language()
	{
		// `-l|--language <name>` switches the active output language at startup. Verifies the
		// command-line consumer maps the name through LanguageService and updates
		// CurrentLanguage so all subsequent decompilations use it.

		// Arrange — boot, capture the default language so the assertion is meaningful.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		languageService.CurrentLanguage.Name.Should().NotBe("IL", "baseline must differ from the value we'll assert");

		var args = CommandLineArguments.Create(new[] { "--language", "IL" });

		// Act — apply the parsed arguments through the same path App.OnOpened uses.
		await vm.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);

		// Assert — language is now IL.
		languageService.CurrentLanguage.Name.Should().Be("IL");
	}

	[AvaloniaTest]
	public async Task NavigateTo_Type_Arg_Selects_The_Matching_Type_Node()
	{
		// `-n|--navigateto T:<TypeId>` navigates to a type-tree-node at startup. The arg's
		// content is an XML-doc-style ID string ("T:System.Linq.Enumerable") which the
		// consumer resolves through the loaded assemblies and selects the matching tree node
		// (which then triggers a decompile).

		// Arrange — boot.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var args = CommandLineArguments.Create(new[] { "--navigateto", "T:System.Linq.Enumerable" });

		// Act — apply the args.
		await vm.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);

		// Assert — selected item is the System.Linq.Enumerable TypeTreeNode (ToString returns
		// the ReflectionName; that's a stable identifier independent of the active language).
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().NotBeNull();
		vm.AssemblyTreeModel.SelectedItem!.GetType().Should().Be(typeof(TypeTreeNode));
		vm.AssemblyTreeModel.SelectedItem!.ToString().Should().Be("System.Linq.Enumerable");
	}

	[AvaloniaTest]
	public async Task NavigateTo_None_Arg_Leaves_Selection_Empty()
	{
		// `-n none` is a sentinel that tells the consumer to clear (or leave empty) the
		// initial selection — used by the WPF VS add-in which sends the real navigation
		// target later via IPC. Verifies SelectedItem stays null after applying.

		// Arrange — boot, ensure no node is selected initially.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		// clear selection
		vm.AssemblyTreeModel.SelectedItems.Clear();

		var args = CommandLineArguments.Create(new[] { "--navigateto", "none" });

		// Act — apply the args.
		await vm.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);

		// Assert — SelectedItem is still null.
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().BeNull();
	}

	[AvaloniaTest]
	public async Task NavigateTo_Skips_A_Missing_Session_Assembly_Instead_Of_Crashing()
	{
		// A restored session can still list an assembly whose file has since been deleted or
		// moved. Navigating on launch eagerly loads every relevant assembly's metadata before
		// resolving the target; a gone file must be skipped, not abort startup with an
		// unhandled exception. Regression test for a DirectoryNotFoundException thrown out of
		// that pre-load loop.

		// Arrange — boot, then inject a session assembly whose file does not exist.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var missingPath = Path.Combine(
			Path.GetTempPath(), "ILSpyMissing_" + Guid.NewGuid().ToString("N"), "gone.dll");
		vm.AssemblyTreeModel.AssemblyList!.OpenAssembly(missingPath);

		var args = CommandLineArguments.Create(new[] { "--navigateto", "T:System.Linq.Enumerable" });

		// Act — applying the args must complete without throwing despite the missing entry.
		await vm.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);

		// Assert — the missing assembly was skipped and the present target still resolved.
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().NotBeNull();
		vm.AssemblyTreeModel.SelectedItem!.ToString().Should().Be("System.Linq.Enumerable");
	}
}
