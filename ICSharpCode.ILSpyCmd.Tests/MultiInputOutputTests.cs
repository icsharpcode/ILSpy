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

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	[TestFixture]
	public class MultiInputOutputTests
	{
		string tempDirectory;
		string firstAssemblyPath;
		string secondAssemblyPath;

		[SetUp]
		public void SetUp()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), "ILSpyCmdMultiInput_" + Path.GetRandomFileName());
			Directory.CreateDirectory(tempDirectory);
			firstAssemblyPath = Path.Combine(tempDirectory, "First.dll");
			secondAssemblyPath = Path.Combine(tempDirectory, "Second.dll");
			CreateAssembly(firstAssemblyPath, "First");
			CreateAssembly(secondAssemblyPath, "Second");
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(tempDirectory))
				Directory.Delete(tempDirectory, recursive: true);
		}

		static void CreateAssembly(string assemblyPath, string assemblyName)
		{
			var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
				.Split(Path.PathSeparator)
				.Select(path => MetadataReference.CreateFromFile(path));
			var resources = new[] {
				new ResourceDescription(assemblyName + ".txt",
					() => new MemoryStream(Encoding.UTF8.GetBytes(assemblyName)), isPublic: true)
			};
			var compilation = CSharpCompilation.Create(assemblyName,
				[CSharpSyntaxTree.ParseText($"public class {assemblyName}Type {{ public string Name => \"{assemblyName}\"; }}")],
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			using var output = File.Create(assemblyPath);
			var result = compilation.Emit(output, manifestResources: resources);
			Assert.That(result.Success, Is.True, () => string.Join(Environment.NewLine, result.Diagnostics));
		}

		[TestCase(new string[] { }, ".decompiled.cs", TestName = "CSharpOutput")]
		[TestCase(new string[] { "-il" }, ".il", TestName = "ILOutput")]
		[TestCase(new string[] { "-l", "c" }, ".list.txt", TestName = "TypeListOutput")]
		[TestCase(new string[] { "--list-resources" }, ".resources.txt", TestName = "ResourceListOutput")]
		public async Task MultiInputOutputMatchesSeparateRuns(string[] mode, string extension)
		{
			string firstName = Path.GetFileNameWithoutExtension(firstAssemblyPath);
			string secondName = Path.GetFileNameWithoutExtension(secondAssemblyPath);
			var firstControl = await RunAsync([.. mode, firstAssemblyPath]);
			var secondControl = await RunAsync([.. mode, secondAssemblyPath]);
			string outputDirectory = Path.Combine(tempDirectory, "output");

			var combined = await RunAsync([.. mode, "-o", outputDirectory, firstAssemblyPath, secondAssemblyPath]);

			Assert.That(combined.ExitCode, Is.EqualTo(0), combined.Error);
			Assert.That(File.ReadAllText(Path.Combine(outputDirectory, firstName + extension)), Is.EqualTo(firstControl.Output));
			Assert.That(File.ReadAllText(Path.Combine(outputDirectory, secondName + extension)), Is.EqualTo(secondControl.Output));
		}

		[Test]
		public async Task SolutionExportRejectsProjectDirectoriesThatDifferOnlyByCase()
		{
			string firstInputDirectory = Path.Combine(tempDirectory, "first");
			string secondInputDirectory = Path.Combine(tempDirectory, "second");
			Directory.CreateDirectory(firstInputDirectory);
			Directory.CreateDirectory(secondInputDirectory);
			string firstAssembly = Path.Combine(firstInputDirectory, "Library.dll");
			string secondAssembly = Path.Combine(secondInputDirectory, "library.dll");
			File.Copy(firstAssemblyPath, firstAssembly);
			File.Copy(firstAssemblyPath, secondAssembly);
			string outputDirectory = Path.Combine(tempDirectory, "solution");

			var result = await RunAsync("-p", "-o", outputDirectory, firstAssembly, secondAssembly);

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			Assert.That(result.Error, Does.Contain("Library").And.Contain("library"));
			Assert.That(Directory.GetFiles(outputDirectory, "*", SearchOption.AllDirectories), Is.Empty);
		}

		[Test]
		public async Task TypeListingEscapesControlCharactersOnlyWhenWritingToATerminal()
		{
			const string originalTypeName = "FirstType";
			const string controlCharacterTypeName = "First\r\u001b[K";
			byte[] assembly = File.ReadAllBytes(firstAssemblyPath);
			byte[] originalNameBytes = Encoding.UTF8.GetBytes(originalTypeName);
			byte[] controlCharacterNameBytes = Encoding.UTF8.GetBytes(controlCharacterTypeName);
			int nameOffset = assembly.AsSpan().IndexOf(originalNameBytes);
			Assert.That(nameOffset, Is.GreaterThanOrEqualTo(0), "expected type name in metadata");
			Buffer.BlockCopy(controlCharacterNameBytes, 0, assembly, nameOffset, controlCharacterNameBytes.Length);
			File.WriteAllBytes(firstAssemblyPath, assembly);

			var result = await RunAsync(firstAssemblyPath, "--list", "c");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			string displayedName = ConsoleEscaping.EscapeControlCharsIfNeeded(controlCharacterTypeName, !Console.IsOutputRedirected);
			Assert.That(result.Output, Does.Contain("Class " + displayedName));

			var suggestion = await RunAsync(firstAssemblyPath, "-t", "Fist");

			Assert.That(suggestion.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			string displayedSuggestion = ConsoleEscaping.EscapeControlCharsIfNeeded(controlCharacterTypeName, !Console.IsErrorRedirected);
			Assert.That(suggestion.Error, Does.Contain(displayedSuggestion));
		}
	}
}
