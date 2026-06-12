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

using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// End-to-end smoke test of project export. Drives
/// <see cref="CSharpLanguage.DecompileAssembly"/> with
/// <see cref="DecompilationOptions.SaveAsProjectDirectory"/> set, asserts the resulting
/// directory has a <c>.csproj</c> file and at least one <c>.cs</c> file.
/// </summary>
[TestFixture]
public class ProjectExportTests
{
	[Test]
	public void Language_Base_Defaults_ProjectFileExtension_To_Null()
	{
		// The base Language.ProjectFileExtension is the gate for File → Save Code's
		// "save as project" filter. Default null = single-file-only for languages that
		// haven't opted in.
		var disassembler = new ILLanguage();
		((object?)disassembler.ProjectFileExtension).Should().BeNull(
			"only project-aware languages override; the IL disassembler is single-file only");
	}

	[Test]
	public void CSharpLanguage_Reports_CSProj_As_ProjectFileExtension()
	{
		var cs = new CSharpLanguage();
		cs.ProjectFileExtension.Should().Be(".csproj");
	}

	[AvaloniaTest]
	public async Task DecompileAssembly_With_SaveAsProjectDirectory_Writes_Csproj_And_Cs_Files()
	{
		// Drives the project-export path end-to-end against a tiny emitted fixture assembly, so the
		// smoke test isn't dominated by decompiling a multi-hundred-type framework assembly.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var loaded = await vm.OpenFixtureAsync();

		var tempDir = Path.Combine(Path.GetTempPath(), "ILSpyExport_" + System.Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		try
		{
			var options = new ICSharpCode.ILSpy.DecompilationOptions(new DecompilerSettings()) {
				FullDecompilation = true,
				SaveAsProjectDirectory = tempDir,
			};
			var language = AppComposition.Current.GetExport<LanguageService>()
				.Languages.OfType<CSharpLanguage>().First();
			var sw = new StringWriter();
			var output = new ICSharpCode.Decompiler.PlainTextOutput(sw);
			// (PlainTextOutput lives in the root ICSharpCode.Decompiler namespace — no Output suffix.)

			language.DecompileAssembly(loaded, output, options);

			Directory.EnumerateFiles(tempDir, "*.csproj").Should().HaveCount(1,
				"WholeProjectDecompiler must write exactly one .csproj");
			Directory.EnumerateFiles(tempDir, "*.cs", SearchOption.AllDirectories).Should().NotBeEmpty(
				"at least one .cs file (AssemblyInfo or a type) must be produced");
			sw.ToString().Should().Contain("Project written to",
				"the ITextOutput buffer must include a project-written breadcrumb so the editor tab shows feedback");
		}
		finally
		{
			try
			{ Directory.Delete(tempDir, recursive: true); }
			catch { /* best-effort */ }
		}
	}
}
