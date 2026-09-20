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
using System.Diagnostics;
using System.IO;
using System.Text.Json;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Metadata;
using ICSharpCode.ILSpyX.PdbProvider;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Windows.Pdb;

/// <summary>
/// Covers PdbDumper against a Windows (native MSF) PDB. Mono.Cecil reads that format in managed
/// code anywhere, but its writer is COM-only, so no such PDB can be produced off Windows: the
/// fixture is compiled here by the in-box C# compiler, which still emits the native format.
/// The Portable PDB path is covered cross-platform by the ilspycmd tests.
/// </summary>
[TestFixture]
public class NativePdbDumperTests
{
	const string Source = """
		using System;

		public class Sample
		{
			public int Add(int a, int b)
			{
				int sum = a + b;
				return sum;
			}
		}
		""";

	static string directory = null!;
	static string assemblyPath = null!;
	static string pdbPath = null!;

	[OneTimeSetUp]
	public void CompileFixture()
	{
		string csc = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.Windows),
			@"Microsoft.NET\Framework64\v4.0.30319\csc.exe");
		if (!File.Exists(csc))
			Assert.Ignore($"The in-box C# compiler is not present at '{csc}'.");

		directory = Path.Combine(Path.GetTempPath(), "ilspy-nativepdb-" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);
		string sourcePath = Path.Combine(directory, "Sample.cs");
		File.WriteAllText(sourcePath, Source);
		assemblyPath = Path.Combine(directory, "Sample.dll");
		pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");

		// /debug:full is the native format; Roslyn would have to be told /debug:portable
		var psi = new ProcessStartInfo(csc,
			$"/nologo /target:library /debug:full /out:\"{assemblyPath}\" \"{sourcePath}\"") {
			RedirectStandardOutput = true,
			RedirectStandardError = true,
		};
		using var process = Process.Start(psi)!;
		string output = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
		process.WaitForExit();
		Assert.That(process.ExitCode, Is.Zero, $"csc failed: {output}");
		Assert.That(File.Exists(pdbPath), Is.True, "csc did not produce a PDB.");
		DebugInfoUtils.IsWindowsPdb(File.OpenRead(pdbPath)).Should()
			.BeTrue("the fixture only means anything if the PDB is the native format");
	}

	[OneTimeTearDown]
	public void DeleteFixture()
	{
		if (directory != null && Directory.Exists(directory))
			Directory.Delete(directory, recursive: true);
	}

	static string DumpToString(string? explicitPdb, bool asJson)
	{
		var writer = new StringWriter();
		PdbDumper.Dump(assemblyPath, explicitPdb, writer, asJson);
		return writer.ToString();
	}

	[Test]
	public void ReadsItWithTheNativeReader()
	{
		DumpToString(explicitPdb: null, asJson: false)
			.Should().Contain("Symbols:  NativePdbReader",
				"the point of this fixture is the native format; the Portable reader would mean "
				+ "the compiler emitted a Portable PDB and the suite covers nothing new");
	}

	[Test]
	public void ListsTheDocumentAndTheMethod()
	{
		string dump = DumpToString(explicitPdb: null, asJson: false);

		dump.Should().MatchRegex(@"Documents \(1\)", "the one source file is listed");
		dump.Should().Contain("Sample.cs");
		dump.Should().Contain("System.Int32 Sample::Add(System.Int32,System.Int32)",
			"methods are listed by their full signature");
	}

	[Test]
	public void ReadsAPdbNamedExplicitly()
	{
		DumpToString(explicitPdb: pdbPath, asJson: false)
			.Should().Be(DumpToString(explicitPdb: null, asJson: false),
				"naming the file next to the assembly is the same as finding it");
	}

	[Test]
	public void DecodesSequencePoints()
	{
		string dump = DumpToString(explicitPdb: null, asJson: false);

		// 'int sum = a + b;' is line 7 of Source, and a sequence point maps an IL offset to it.
		dump.Should().MatchRegex(@"IL_[0-9a-fA-F]{4}\s+\(7,\d+\)-\(7,\d+\)",
			"a sequence point carries the source span, not just the offset");
	}

	[Test]
	public void DecodesTheScopeTree()
	{
		string dump = DumpToString(explicitPdb: null, asJson: false);

		dump.Should().MatchRegex(@"Scope IL_[0-9a-fA-F]{4}\.\.",
			"the scope tree is decoded, not skipped");
		dump.Should().MatchRegex(@"\[\d+\] sum",
			"a native PDB carries the local's name and slot");
	}

	[Test]
	public void ProducesParseableJson()
	{
		string json = DumpToString(explicitPdb: null, asJson: true);

		Action parse = () => JsonDocument.Parse(json);
		parse.Should().NotThrow("--json output has to be machine-readable");
		json.Should().Contain("Sample.cs");
	}

	[Test]
	public void ReportsAnAssemblyWithoutSymbols()
	{
		string stripped = Path.Combine(directory, "NoSymbols.dll");
		File.Copy(assemblyPath, stripped);

		Action dump = () => PdbDumper.Dump(stripped, null, TextWriter.Null, asJson: false);

		dump.Should().Throw<PdbDumper.NoDebugSymbolsException>(
			"an assembly whose PDB is not beside it has nothing to dump");
	}
}
