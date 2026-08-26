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
	public async Task NavigateTo_Accepts_A_Member_Id_Without_Its_Signature()
	{
		// A cref may name a member without a parameter list, which is what a user reaches for:
		// spelling out the signature means knowing the overload count beforehand. Where the
		// short form names exactly one member, it selects that member.

		// Arrange - boot, and open an assembly with a member that has no overloads.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		string path = typeof(CommandLineArgumentsTests).Assembly.Location;
		var args = CommandLineArguments.Create(new[] {
			path, "--navigateto", "M:ICSharpCode.ILSpy.Tests.NavigateToSample.OnlyOne" });

		// Act.
		await vm.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);

		// Assert - selection landed on the member itself.
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().NotBeNull();
		vm.AssemblyTreeModel.SelectedItem!.GetType().Should().Be(typeof(MethodTreeNode));
		((MethodTreeNode)vm.AssemblyTreeModel.SelectedItem!).MethodDefinition.Name.Should().Be("OnlyOne");
	}

	[AvaloniaTest]
	public async Task NavigateTo_Short_Form_Of_An_Overloaded_Member_Selects_Every_Overload()
	{
		// The short form of an overloaded member names the whole group, and no single overload
		// is a better answer than its siblings. Selecting all of them shows every one without
		// leaving the member level: falling back to the declaring type would bury the group in
		// a large type's decompilation, and picking one would hide that there was a choice.

		// Arrange - boot, and open an assembly with an overloaded member.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		string path = typeof(CommandLineArgumentsTests).Assembly.Location;
		var args = CommandLineArguments.Create(new[] {
			path, "--navigateto", "M:ICSharpCode.ILSpy.Tests.NavigateToSample.Overloaded" });

		// Act.
		await vm.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);

		// Assert - every overload is selected, and nothing else.
		vm.AssemblyTreeModel.SelectedItems.Should().HaveCount(2);
		vm.AssemblyTreeModel.SelectedItems.Should().AllSatisfy(node =>
			((MethodTreeNode)node).MethodDefinition.Name.Should().Be("Overloaded"));
	}

	[AvaloniaTest]
	public async Task NavigateTo_Falls_Back_To_The_Loaded_Assembly_When_The_Id_Does_Not_Resolve()
	{
		// An ID that names nothing must not leave the tree on an empty selection with no
		// indication of what happened: the assembly the user asked to open is still the best
		// answer, and it is what opening it without --navigateto would have selected.

		// Arrange - boot, then ask to open an assembly the default list does not contain.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);
		vm.AssemblyTreeModel.SelectedItems.Clear();

		string path = typeof(CommandLineArgumentsTests).Assembly.Location;
		var args = CommandLineArguments.Create(new[] { path, "--navigateto", "M:No.Such.Type.NoSuchMember" });

		// Act.
		await vm.AssemblyTreeModel.HandleCommandLineArgumentsAsync(args);

		// Assert - the requested assembly is selected.
		((object?)vm.AssemblyTreeModel.SelectedItem).Should().NotBeNull(
			"an unresolvable target must fall back to the assembly that was opened");
		vm.AssemblyTreeModel.SelectedItem!.GetType().Should().Be(typeof(AssemblyTreeNode));
		vm.AssemblyTreeModel.SelectedItem!.ToString().Should().Be(path);
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

/// <summary>
/// Fixture for --navigateto: one member with no overloads, and one with several, so the short
/// form of a member ID can be exercised in both shapes.
/// </summary>
public class NavigateToSample
{
	public void OnlyOne(int a, int b) { }

	public void Overloaded(int a) { }

	public void Overloaded(string a) { }
}
