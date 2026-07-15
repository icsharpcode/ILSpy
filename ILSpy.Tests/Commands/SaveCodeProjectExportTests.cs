// Copyright (c) 2026 Siegfried Pammer
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

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Commands;

/// <summary>
/// Every File -> Save Code path for an assembly runs behind a cancellable progress overlay, never a
/// bare Task.Run, and every one that decompiles a whole assembly shares the Export Project command's
/// frozen determinate-progress tab: <see cref="ProjectExport.ExportSingleAssemblyAsync"/> for a
/// .csproj, <see cref="ProjectExport.ExportSolutionAsync"/> for several assemblies as a .sln. The
/// single-file save goes through <see cref="SaveCodeHelper.SaveNodeToFileWithProgressAsync"/> (the
/// same overlay normal decompilation uses). These assert each path writes its output and surfaces a
/// report/breadcrumb in a tab.
/// </summary>
[TestFixture]
public class SaveCodeProjectExportTests
{
	[AvaloniaTest]
	public async Task Save_As_Project_Runs_Through_The_Export_Project_Tab()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assembly = await vm.OpenFixtureAsync();
		var dock = vm.DockWorkspace;

		var language = AppComposition.Current.GetExport<LanguageService>()
			.Languages.OfType<CSharpLanguage>().First();
		var settings = AppComposition.Current.GetExport<SettingsService>().CreateEffectiveDecompilerSettings();

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpySaveProj_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			await ProjectExport.ExportSingleAssemblyAsync(
				assembly, tempDir, settings, language, dock);

			Directory.EnumerateFiles(tempDir, "*.csproj").Should().HaveCount(1,
				"Save Code -> .csproj must run the whole-project decompiler, exactly like Export Project");
			Directory.EnumerateFiles(tempDir, "*.cs", SearchOption.AllDirectories).Should().NotBeEmpty();

			var exportTab = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
				.Select(t => t.Content).OfType<DecompilerTabPageModel>()
				.FirstOrDefault(d => d.Text != null && d.Text.Contains("Project written to"));
			exportTab.Should().NotBeNull("the export runs in its own progress tab and shows the project-written report there");
			exportTab!.Title.Should().Be(assembly.Text,
				"a single-assembly export tab is titled after that assembly's tree-node label -- the same however it was started (Save Code or Export Project)");
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* best-effort */ }
		}
	}

	[AvaloniaTest]
	public async Task Save_Several_Assemblies_Exports_A_Solution_Through_The_Export_Project_Tab()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assemblies = new[] {
			await vm.OpenFixtureAsync("FixtureA"),
			await vm.OpenFixtureAsync("FixtureB"),
		};
		var dock = vm.DockWorkspace;

		var language = AppComposition.Current.GetExport<LanguageService>()
			.Languages.OfType<CSharpLanguage>().First();
		var settings = AppComposition.Current.GetExport<SettingsService>().CreateEffectiveDecompilerSettings();

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpySaveSln_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		// A name the output folder does not imply: Save Code lets the user pick the .sln file itself,
		// unlike the Export Project dialog, which derives the name from the chosen folder.
		var solutionPath = Path.Combine(tempDir, "Picked.sln");
		try
		{
			await ProjectExport.ExportSolutionAsync(assemblies, solutionPath, settings, language, dock);

			File.Exists(solutionPath).Should().BeTrue(
				"the solution must land on the exact path picked in Save Code, not one derived from the folder");
			foreach (var a in assemblies)
			{
				Directory.EnumerateFiles(Path.Combine(tempDir, a.ShortName), "*.csproj").Should().HaveCount(1,
					$"each exported assembly gets its own project ({a.ShortName})");
			}

			var exportTab = dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
				.Select(t => t.Content).OfType<DecompilerTabPageModel>()
				.FirstOrDefault(d => d.Title == string.Join(", ", assemblies.Select(a => a.Text)));
			exportTab.Should().NotBeNull(
				"a solution export tab is titled after the assemblies being exported -- the same however it was started (Save Code or Export Project)");
			exportTab!.Text.Should().Contain("Created the Visual Studio Solution file",
				"the export runs in its own progress tab and reports there");
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* best-effort */ }
		}
	}

	[AvaloniaTest]
	public async Task A_Failed_Load_In_The_Selection_Does_Not_Disqualify_The_Solution_Export()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var good = await vm.OpenFixtureAsync("FixtureA");
		var broken = await vm.OpenBrokenFixtureAsync();

		// The selection Ctrl+S sees: two assembly nodes, one of which failed to load. Rejecting it
		// here is what used to make Save Code fall through and quietly save the focused assembly
		// alone; the exporter skips the unloadable one and reports it instead.
		var nodes = new SharpTreeNode[] { new AssemblyTreeNode(good), new AssemblyTreeNode(broken) };

		ProjectExport.TryGetSolutionAssemblies(nodes, out var assemblies).Should().BeTrue(
			"an assembly that failed to load must not disqualify the whole selection");
		assemblies.Should().BeEquivalentTo([good, broken],
			"the exporter decides what to skip, so it has to see the failed load to report it");
	}

	[AvaloniaTest]
	public async Task A_Selection_Where_Nothing_Loaded_Has_Nothing_To_Export()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var broken = await vm.OpenBrokenFixtureAsync("BrokenA");
		var alsoBroken = await vm.OpenBrokenFixtureAsync("BrokenB");

		var nodes = new SharpTreeNode[] { new AssemblyTreeNode(broken), new AssemblyTreeNode(alsoBroken) };

		ProjectExport.TryGetSolutionAssemblies(nodes, out _).Should().BeFalse(
			"a solution of nothing but failed loads would be empty; Save Code leaves the selection to the single-node path");
	}

	[AvaloniaTest]
	public async Task Save_As_Single_File_Runs_Through_The_Cancellable_Progress_Tab()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assembly = await vm.OpenFixtureAsync();
		var dock = vm.DockWorkspace;

		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(assembly.ShortName);
		Assert.That(assemblyNode, Is.Not.Null);
		// Select the assembly so there is an active decompiler tab for RunWithCancellation to run on.
		vm.AssemblyTreeModel.SelectNode(assemblyNode);
		await dock.WaitForDecompiledTextAsync();

		var language = AppComposition.Current.GetExport<LanguageService>()
			.Languages.OfType<CSharpLanguage>().First();

		var path = Path.Combine(Path.GetTempPath(), "ILSpySaveSingle_" + System.Guid.NewGuid().ToString("N") + ".cs");
		try
		{
			await SaveCodeHelper.SaveNodeToFileWithProgressAsync(assemblyNode!, language, path, dock);

			File.Exists(path).Should().BeTrue("the single-file save must write the assembly to disk");
			(await File.ReadAllTextAsync(path)).Should().Contain(FixtureAssembly.TypeName,
				"the decompiled assembly's type must be present");

			dock.Documents!.VisibleDockables!.OfType<ContentTabPage>()
				.Any(t => t.Content is DecompilerTabPageModel d && d.Text != null && d.Text.Contains("Decompilation complete"))
				.Should().BeTrue("the single-file save runs behind the cancellable overlay and shows a completion breadcrumb");
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}
