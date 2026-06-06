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

using AwesomeAssertions;

using ILSpy.AppEnv;
using ILSpy.Languages;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// End-to-end smoke test of the multi-assembly solution (.sln) export path that backs the
/// "Save Code" entry when several assemblies are selected. Drives
/// <see cref="global::ILSpy.SolutionWriter.CreateSolutionAsync"/> directly (no file picker)
/// and asserts a solution file plus one project per assembly is produced.
/// </summary>
[TestFixture]
public class SolutionExportTests
{
	[AvaloniaTest]
	public async Task CreateSolution_Writes_Sln_And_Per_Assembly_Projects()
	{
		var (_, vm) = await TestHarness.BootAsync(3);

		// CoreLib is too big for a smoke test; the Uri and System.Linq reference assemblies
		// the headless fixture seeds are both manageable full-project decompiles.
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName)
			.GroupBy(a => a.ShortName).Select(g => g.First())
			.Take(2).ToList();
		assemblies.Should().HaveCountGreaterThanOrEqualTo(2, "a solution needs at least two distinct assemblies");

		foreach (var a in assemblies)
			await a.GetLoadResultAsync();

		var language = AppComposition.Current.GetExport<LanguageService>()
			.Languages.OfType<CSharpLanguage>().First();

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpySln_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		var slnPath = Path.Combine(tempDir, "Solution.sln");
		try
		{
			var result = await global::ILSpy.SolutionWriter.CreateSolutionAsync(slnPath, language, assemblies);

			result.Success.Should().BeTrue(
				"the solution export should succeed for valid assemblies. Status:\n" + result.StatusText);
			File.Exists(slnPath).Should().BeTrue("the .sln file must be written to the chosen path");

			foreach (var a in assemblies)
			{
				var projectDir = Path.Combine(tempDir, a.ShortName);
				Directory.EnumerateFiles(projectDir, "*.csproj").Should().HaveCount(1,
					$"each assembly gets its own subdirectory with one project file ({a.ShortName})");
				Directory.EnumerateFiles(projectDir, "*.cs", SearchOption.AllDirectories).Should().NotBeEmpty(
					$"the decompiled project for {a.ShortName} must contain at least one source file");
			}

			result.StatusText.Should().Contain("Created the Visual Studio Solution file",
				"a successful run reports the solution-written breadcrumb");
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* best-effort cleanup */ }
		}
	}

	[AvaloniaTest]
	public async Task CreateSolution_Aborts_On_Duplicate_Assembly_Names()
	{
		var (_, vm) = await TestHarness.BootAsync(3);

		var assembly = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName);
		await assembly.GetLoadResultAsync();

		var language = AppComposition.Current.GetExport<LanguageService>()
			.Languages.OfType<CSharpLanguage>().First();

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpySlnDup_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		var slnPath = Path.Combine(tempDir, "Solution.sln");
		try
		{
			// The same assembly twice collides on ShortName: the writer must refuse rather than
			// clobber one project directory with another.
			var result = await global::ILSpy.SolutionWriter.CreateSolutionAsync(
				slnPath, language, new[] { assembly, assembly });

			result.Success.Should().BeFalse("duplicate assembly names cannot produce a valid solution");
			result.StatusText.Should().Contain("Duplicate assembly names");
			File.Exists(slnPath).Should().BeFalse("no solution file is written when the export aborts");
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* best-effort cleanup */ }
		}
	}
}
