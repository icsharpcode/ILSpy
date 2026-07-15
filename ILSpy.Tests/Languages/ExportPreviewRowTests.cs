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

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// The Export Project/Solution dialog's choices, extracted as pure statics so they are testable
/// without the window: the preview rows (per-assembly project name, target subdirectory, and the
/// invalid / duplicate-name / PDB-eligible badges) and the solution file name the user may type in
/// solution mode.
/// </summary>
[TestFixture]
public class ExportPreviewRowTests
{
	[AvaloniaTest]
	public async Task Solution_Mode_Uses_ShortName_Subdirectories_And_Flags_No_Duplicates()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName)
			.GroupBy(a => a.ShortName).Select(g => g.First())
			.Take(2).ToList();
		assemblies.Should().HaveCount(2);

		var rows = ExportProjectDialog.BuildPreviewRows(assemblies, solutionMode: true);

		rows.Should().HaveCount(2);
		rows.Should().OnlyContain(r => r.IsValidAssembly);
		rows.Should().OnlyContain(r => !r.HasDuplicateShortName);
		rows.Select(r => r.TargetSubdirectory).Should().BeEquivalentTo(assemblies.Select(a => a.ShortName));
		rows.Select(r => r.ProjectName).Should().BeEquivalentTo(assemblies.Select(a => a.ShortName + ".csproj"));
	}

	[AvaloniaTest]
	public async Task Project_Mode_Writes_Into_The_Chosen_Folder_Directly()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName);

		var rows = ExportProjectDialog.BuildPreviewRows(new[] { assembly }, solutionMode: false);

		rows.Single().TargetSubdirectory.Should().Be(".",
			"project mode writes the single project into the chosen folder, not a subdirectory");
	}

	[AvaloniaTest]
	public async Task Duplicate_Short_Names_Are_Flagged_On_Every_Affected_Row()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName);

		var rows = ExportProjectDialog.BuildPreviewRows(new[] { assembly, assembly }, solutionMode: true);

		rows.Should().HaveCount(2);
		rows.Should().OnlyContain(r => r.HasDuplicateShortName);
		rows.Should().OnlyContain(r => r.BadgeText.Contains("duplicate name"));
	}

	[AvaloniaTest]
	public async Task An_Assembly_That_Failed_To_Load_Is_Badged_As_Invalid()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var good = await vm.OpenFixtureAsync("FixtureA");
		var broken = await vm.OpenBrokenFixtureAsync();

		var rows = ExportProjectDialog.BuildPreviewRows([good, broken], solutionMode: true);

		rows.Should().HaveCount(2);
		rows.Single(r => r.ProjectName.StartsWith(broken.ShortName)).BadgeText.Should().Contain("not a valid assembly",
			"the dialog has to say which rows the export will skip");
		rows.Single(r => r.ProjectName.StartsWith(good.ShortName)).IsValidAssembly.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task The_Export_Flow_Settles_Loads_Before_The_Dialog_Reads_Them()
	{
		var (_, vm) = await TestHarness.BootAsync();
		// A LoadedAssembly loads lazily -- the tree builds its entries en masse and only the first await
		// starts the work -- so a selected-but-never-opened assembly reaches the export flow with its
		// load untouched. Its preview-row badges are read off the load result, so the flow has to settle
		// the load first; otherwise the dialog blocks the UI thread forcing one as it builds the rows.
		var assembly = new LoadedAssembly(vm.AssemblyTreeModel.AssemblyList!, FixtureAssembly.Emit("FixturePreview"));
		assembly.IsLoadedAsValidAssembly.Should().BeFalse("nothing has triggered the load yet");

		await ProjectExport.EnsureAssembliesLoadedAsync([assembly]);

		assembly.IsLoadedAsValidAssembly.Should().BeTrue(
			"the export flow settles every selected assembly's load up front");
		ExportProjectDialog.BuildPreviewRows([assembly], solutionMode: false)
			.Single().IsValidAssembly.Should().BeTrue("the dialog now badges it from a completed load");
	}

	[AvaloniaTest]
	public async Task Solution_Name_Field_Is_Offered_Only_In_Solution_Mode()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var a = await vm.OpenFixtureAsync("FixtureA");
		var b = await vm.OpenFixtureAsync("FixtureB");
		var settings = AppComposition.Current.GetExport<SettingsService>();

		var solutionDialog = new ExportProjectDialog(settings, [a, b], solutionMode: true);
		solutionDialog.Show();
		solutionDialog.SolutionNamePanel.IsVisible.Should().BeTrue(
			"the .sln is the user's to name when several assemblies are exported");
		solutionDialog.Capture("solution-mode");
		solutionDialog.Close();

		var projectDialog = new ExportProjectDialog(settings, [a], solutionMode: false);
		projectDialog.Show();
		projectDialog.SolutionNamePanel.IsVisible.Should().BeFalse(
			"a single project export writes no solution, so there is no name to ask for");
		projectDialog.Capture("project-mode");
		projectDialog.Close();
	}

	[AvaloniaTest]
	public async Task Export_Dialog_Fits_Its_Content_Without_Scrolling()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var a = await vm.OpenFixtureAsync("FixtureA");
		var b = await vm.OpenFixtureAsync("FixtureB");
		var settings = AppComposition.Current.GetExport<SettingsService>();

		// Solution mode is the tall case: it carries the solution-name field on top of everything
		// project mode shows.
		foreach (var (mode, assemblies) in new[] {
			("solution", new[] { a, b }),
			("project", new[] { a }),
		})
		{
			var dialog = new ExportProjectDialog(settings, assemblies, solutionMode: mode == "solution");
			dialog.Show();
			Dispatcher.UIThread.RunJobs(DispatcherPriority.Loaded);

			dialog.OptionsScroll.Extent.Height.Should().BeLessThanOrEqualTo(dialog.OptionsScroll.Viewport.Height + 0.5,
				$"every option must be reachable without scrolling in {mode} mode "
				+ $"(content {dialog.OptionsScroll.Extent.Height}, viewport {dialog.OptionsScroll.Viewport.Height})");

			dialog.Close();
		}
	}

	[TestCase("MySolution", "MySolution.sln", TestName = "A typed name gains the .sln extension")]
	[TestCase("MySolution.sln", "MySolution.sln", TestName = "An already-qualified name is not doubled up")]
	[TestCase("  Spaced  ", "Spaced.sln", TestName = "Surrounding whitespace is trimmed")]
	[TestCase("", null, TestName = "A blank name defers to the folder-derived default")]
	[TestCase("   ", null, TestName = "A whitespace-only name defers to the folder-derived default")]
	[TestCase(null, null, TestName = "An unset name defers to the folder-derived default")]
	// Stripping the extension the user typed leaves nothing behind, which CleanUpFileName would turn
	// into "-", exporting "-.sln" for what reads as a request for the default.
	[TestCase(".sln", null, TestName = "A bare extension defers to the folder-derived default")]
	[TestCase("  .SLN  ", null, TestName = "A padded bare extension defers to the folder-derived default")]
	public void Typed_Solution_Names_Are_Normalized(string? typed, string? expected)
	{
		ExportProjectDialog.NormalizeSolutionFileName(typed).Should().Be(expected);
	}

	[Test]
	public void A_Typed_Solution_Name_Cannot_Escape_The_Output_Folder()
	{
		var normalized = ExportProjectDialog.NormalizeSolutionFileName("../../etc/passwd");

		normalized.Should().NotBeNull();
		normalized.Should().EndWith(".sln");
		Path.GetFileName(normalized).Should().Be(normalized,
			"the name is combined with the chosen output directory, so it must stay a bare file name");
	}
}
