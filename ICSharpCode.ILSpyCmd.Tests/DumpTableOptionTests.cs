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
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	[TestFixture]
	public class DumpTableOptionTests
	{
		static readonly string testAssemblyPath = typeof(DumpTableOptionTests).Assembly.Location;

		[Test]
		public async Task TypeDefTableContainsSampleType()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "TypeDef");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("RID"));
			Assert.That(result.Output, Does.Contain("Token"));
			Assert.That(result.Output, Does.Contain(nameof(DumpTableSample)));
			// the FieldList/MethodList columns SRM hides must be part of the dump
			Assert.That(result.Output, Does.Contain("FieldList"));
			Assert.That(result.Output, Does.Contain("MethodList"));
		}

		[Test]
		public async Task PropertyTableContainsSampleProperty()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "Property");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(nameof(DumpTableSample.SampleProperty)));
		}

		[Test]
		public async Task MethodSemanticsTableShowsAccessors()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "MethodSemantics");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("Getter"));
		}

		[Test]
		public async Task NestedClassTableHasRows()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "NestedClass");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("NestedClass"));
			// at least one row beyond the header: RID 1 must be present
			Assert.That(result.Output, Does.Match(@"(?m)^\s*1\s"));
		}

		[Test]
		public async Task ClassLayoutTableHasRows()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "ClassLayout");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("PackingSize"));
			Assert.That(result.Output, Does.Contain("ClassSize"));
			Assert.That(result.Output, Does.Match(@"(?m)^\s*1\s"));
		}

		[Test]
		public async Task JsonOutputIsParseableAndContainsRows()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "Property", "--json");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			using var doc = JsonDocument.Parse(result.Output);
			var root = doc.RootElement;
			Assert.That(root.GetProperty("table").GetString(), Is.EqualTo("Property"));
			Assert.That(root.GetProperty("rowCount").GetInt32(), Is.GreaterThan(0));
			bool foundSample = false;
			foreach (var row in root.GetProperty("rows").EnumerateArray())
			{
				Assert.That(row.GetProperty("RID").ValueKind, Is.EqualTo(JsonValueKind.Number));
				if (row.GetProperty("Name").GetString() == nameof(DumpTableSample.SampleProperty))
					foundSample = true;
			}
			Assert.That(foundSample, Is.True, "expected a Property row named SampleProperty");
		}

		[Test]
		public async Task UnknownTableNameReportsUsageError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "Bogus");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("Bogus"));
			Assert.That(result.Error, Does.Contain("TypeDef"), "the error should list valid table names");
		}

		[Test]
		public async Task JsonWithoutDumpTableReportsUsageError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--json");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("--json"));
		}

		[Test]
		public async Task TableByDecimalNumber()
		{
			// 23 == 0x17 == Property
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "23");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(nameof(DumpTableSample.SampleProperty)));
		}

		[Test]
		public async Task TableByHexNumber()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "0x17");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(nameof(DumpTableSample.SampleProperty)));
		}

		[Test]
		public async Task OutOfRangeTableNumberReportsUsageError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "0x40");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("0x40"));
		}

		[Test]
		public async Task OutputDirWritesEveryAssemblyCompletely()
		{
			// Two input assemblies, so the per-file output writer is swapped between files;
			// a writer that is replaced without being flushed truncates the earlier file.
			string ilspyCmdAssemblyPath = typeof(ILSpyCmdProgram).Assembly.Location;
			string outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(outputDir);
			try
			{
				var result = await RunAsync(testAssemblyPath, ilspyCmdAssemblyPath,
					"--disable-updatecheck", "--dump-table", "TypeDef", "--json", "-o", outputDir);

				Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
				foreach (string assemblyPath in new[] { testAssemblyPath, ilspyCmdAssemblyPath })
				{
					string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(assemblyPath) + ".TypeDef.json");
					Assert.That(File.Exists(outputFile), Is.True, outputFile);
					// a truncated file is not parseable JSON
					using var doc = JsonDocument.Parse(File.ReadAllText(outputFile));
					Assert.That(doc.RootElement.GetProperty("rowCount").GetInt32(), Is.GreaterThan(0), outputFile);
				}
			}
			finally
			{
				Directory.Delete(outputDir, recursive: true);
			}
		}

		[Test]
		public async Task TableNameIsCaseInsensitive()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-table", "property");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(nameof(DumpTableSample.SampleProperty)));
		}
	}

	public class DumpTableSample
	{
		public string SampleProperty => "sample";

		public class NestedSample
		{
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
	public struct ExplicitLayoutSample
	{
		public int A;
	}
}
