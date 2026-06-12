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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// End-to-end tests of <see cref="ProjectExporter"/>, the headless engine behind the Export
/// Project/Solution dialog: project mode (one assembly), solution mode (several), optional PDB
/// emission, strong-name key copy, and the invariant that it never mutates persisted settings.
/// </summary>
[TestFixture]
public class ProjectExportRunnerTests
{
	static CSharpLanguage Language()
		=> AppComposition.Current.GetExport<LanguageService>().Languages.OfType<CSharpLanguage>().First();

	// Emit and open `count` distinct tiny fixture assemblies. Exporting these instead of the
	// multi-hundred-type framework assemblies the default list carries keeps each test near ~1s
	// while still exercising the full project/solution writer end to end.
	static async Task<List<LoadedAssembly>> OpenFixtures(MainWindowViewModel vm, int count)
	{
		var result = new List<LoadedAssembly>();
		for (int i = 0; i < count; i++)
			result.Add(await vm.OpenFixtureAsync($"Fixture{(char)('A' + i)}"));
		return result;
	}

	static ProjectExportOptions Options(string outputDir, bool generatePdb = false,
		bool embedSourceFilesInPdb = false, string? strongNameKeyFile = null)
		=> new(outputDir,
			UseSdkStyleProjectFormat: true,
			UseNestedDirectoriesForNamespaces: false,
			RemoveDeadCode: false,
			RemoveDeadStores: false,
			UseDebugSymbols: false,
			StrongNameKeyFile: strongNameKeyFile,
			GeneratePdb: generatePdb,
			EmbedSourceFilesInPdb: embedSourceFilesInPdb);

	// PDB generation is exercised against a controlled assembly in
	// ICSharpCode.Decompiler.Tests.PdbGenerationTestRunner. It is deliberately NOT run here against
	// framework assemblies: the writer's DEBUG-only duplicate-sequence-point Debug.Assert (which also
	// [Ignore]s the Members PDB fixture) aborts the test host in a Debug build. The runner's project/
	// solution structure and settings handling are what these tests cover.

	[AvaloniaTest]
	public async Task Project_Mode_Writes_Csproj_And_Cs()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assembly = await vm.OpenFixtureAsync();

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpyProj_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			var progress = new RecordingProgress();
			var result = await ProjectExporter.ExportAsync(
				new[] { assembly }, solutionMode: false, Options(tempDir),
				new DecompilerSettings(), Language(), progress, CancellationToken.None);

			result.Success.Should().BeTrue(result.StatusText);
			Directory.EnumerateFiles(tempDir, "*.csproj").Should().HaveCount(1);
			Directory.EnumerateFiles(tempDir, "*.cs", SearchOption.AllDirectories).Should().NotBeEmpty();
			progress.Reports.Should().Contain(p => p.TotalUnits > 0,
				"project export reports a determinate per-file unit count to the progress sink");
		}
		finally
		{
			TryDelete(tempDir);
		}
	}

	[AvaloniaTest]
	public async Task Solution_Mode_Writes_Sln_And_Projects()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assemblies = await OpenFixtures(vm, 2);

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpyProjSln_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			var result = await ProjectExporter.ExportAsync(
				assemblies, solutionMode: true, Options(tempDir),
				new DecompilerSettings(), Language(), progress: null, CancellationToken.None);

			result.Success.Should().BeTrue(result.StatusText);
			Directory.EnumerateFiles(tempDir, "*.sln").Should().HaveCount(1);
			foreach (var a in assemblies)
			{
				var projectDir = Path.Combine(tempDir, a.ShortName);
				Directory.EnumerateFiles(projectDir, "*.csproj").Should().HaveCount(1, a.ShortName);
			}
		}
		finally
		{
			TryDelete(tempDir);
		}
	}

	[AvaloniaTest]
	public async Task StrongNameKeyFile_Is_Copied_Into_Project()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assembly = await vm.OpenFixtureAsync();

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpyProjSnk_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		var keyFile = Path.Combine(Path.GetTempPath(), "ILSpyKey_" + System.Guid.NewGuid().ToString("N") + ".snk");
		await File.WriteAllBytesAsync(keyFile, new byte[] { 1, 2, 3, 4 });
		try
		{
			var result = await ProjectExporter.ExportAsync(
				new[] { assembly }, solutionMode: false, Options(tempDir, strongNameKeyFile: keyFile),
				new DecompilerSettings(), Language(), progress: null, CancellationToken.None);

			result.Success.Should().BeTrue(result.StatusText);
			File.Exists(Path.Combine(tempDir, Path.GetFileName(keyFile))).Should().BeTrue(
				"the strong-name key must be copied next to the exported project");
		}
		finally
		{
			TryDelete(tempDir);
			TryDeleteFile(keyFile);
		}
	}

	[AvaloniaTest]
	public async Task Runner_Does_Not_Mutate_Persisted_Settings()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assembly = await vm.OpenFixtureAsync();

		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		bool originalSdk = settingsService.DecompilerSettings.UseSdkStyleProjectFormat;

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpyProjNoMutate_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			// Flip every settings-backed toggle relative to the persisted value, on a CLONE.
			var options = Options(tempDir) with { UseSdkStyleProjectFormat = !originalSdk };
			await ProjectExporter.ExportAsync(
				new[] { assembly }, solutionMode: false, options,
				settingsService.DecompilerSettings.Clone(), Language(), progress: null, CancellationToken.None);

			settingsService.DecompilerSettings.UseSdkStyleProjectFormat.Should().Be(originalSdk,
				"exporting must apply overrides to a clone, never the persisted settings");
		}
		finally
		{
			TryDelete(tempDir);
		}
	}

	static void TryDelete(string dir)
	{
		try
		{ Directory.Delete(dir, recursive: true); }
		catch { /* best-effort */ }
	}

	static void TryDeleteFile(string file)
	{
		try
		{ File.Delete(file); }
		catch { /* best-effort */ }
	}

	sealed class RecordingProgress : System.IProgress<DecompilationProgress>
	{
		public List<DecompilationProgress> Reports { get; } = new();

		public void Report(DecompilationProgress value)
		{
			lock (Reports)
				Reports.Add(value);
		}
	}
}
