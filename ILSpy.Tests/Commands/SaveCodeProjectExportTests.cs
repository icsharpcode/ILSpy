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

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Commands;

/// <summary>
/// Every File -> Save Code path for an assembly runs behind a cancellable progress overlay, never a
/// bare Task.Run: the .csproj export goes through <see cref="ProjectExport.ExportSingleAssemblyAsync"/>
/// (the Export Project command's frozen determinate-progress tab), and the single-file save goes through
/// <see cref="SaveCodeHelper.SaveNodeToFileWithProgressAsync"/> (the same overlay normal decompilation
/// uses). These assert both paths write their output and surface a report/breadcrumb in a tab.
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
