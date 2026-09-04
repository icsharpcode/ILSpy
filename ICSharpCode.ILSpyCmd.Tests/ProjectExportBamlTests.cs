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

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// This assembly carries "mainwindow.baml" as an embedded resource, so exporting it as a
	/// project exercises what happens to a BAML stream on the way into the project.
	/// </summary>
	[TestFixture]
	public class ProjectExportBamlTests
	{
		static readonly string testAssemblyPath = typeof(ProjectExportBamlTests).Assembly.Location;

		string outputDirectory;

		[SetUp]
		public void SetUp()
		{
			outputDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(outputDirectory);
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(outputDirectory))
				Directory.Delete(outputDirectory, recursive: true);
		}

		string ProjectFileContent()
		{
			string projectFile = Directory.EnumerateFiles(outputDirectory, "*.csproj").Single();
			return File.ReadAllText(projectFile);
		}

		[Test]
		public async Task BamlBecomesXamlWithoutAskingForIt()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-p", "-o", outputDirectory);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			string xamlFile = Path.Combine(outputDirectory, "mainwindow.xaml");
			Assert.That(File.Exists(xamlFile), Is.True, "the BAML resource is exported as XAML");
			Assert.That(File.ReadAllText(xamlFile), Does.Contain("Hello from BAML"));
		}

		[Test]
		public async Task TheProjectNamesTheXamlAsAPage()
		{
			await RunAsync(testAssemblyPath, "--disable-updatecheck", "-p", "-o", outputDirectory);

			string project = ProjectFileContent();
			Assert.Multiple(() => {
				Assert.That(project, Does.Contain("<Page Include=\"mainwindow.xaml\""));
				Assert.That(project, Does.Not.Contain("mainwindow.baml"), "the raw stream is not carried along as well");
			});
		}

		[Test]
		public async Task TheOldOptInFlagStillWorks()
		{
			// It is documented and scripted against; asking for what is now the default has to
			// keep meaning the same thing.
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-p", "-o", outputDirectory, "--decompile-baml");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(File.Exists(Path.Combine(outputDirectory, "mainwindow.xaml")), Is.True);
		}
	}
}
