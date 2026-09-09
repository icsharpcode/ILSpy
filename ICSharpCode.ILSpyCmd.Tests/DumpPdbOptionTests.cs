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
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	[TestFixture]
	public class DumpPdbOptionTests
	{
		static readonly string testAssemblyPath = typeof(DumpPdbOptionTests).Assembly.Location;

		[Test]
		public async Task DumpPdbListsDocumentsAndSequencePoints()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-pdb");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("Documents"));
			Assert.That(result.Output, Does.Contain("DumpPdbOptionTests.cs"));
			// the method this assertion lives in must show up with its sequence points
			Assert.That(result.Output, Does.Contain(nameof(DumpPdbListsDocumentsAndSequencePoints)));
			Assert.That(result.Output, Does.Contain("IL_"));
		}

		[Test]
		public async Task DumpPdbShowsLocalVariableNames()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-pdb");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			// a local of this test class, only knowable from the PDB
			Assert.That(result.Output, Does.Contain("scopeProbe"));
		}

		[Test]
		public async Task DumpPdbJsonIsParseableAndHasMethods()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-pdb", "--json");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			using var doc = JsonDocument.Parse(result.Output);
			var root = doc.RootElement;
			Assert.That(root.GetProperty("documents").GetArrayLength(), Is.GreaterThan(0));
			var methods = root.GetProperty("methods");
			Assert.That(methods.GetArrayLength(), Is.GreaterThan(0));
			var method = methods.EnumerateArray()
				.First(m => m.GetProperty("name").GetString() == nameof(ScopeProbe));
			Assert.That(method.GetProperty("sequencePoints").GetArrayLength(), Is.GreaterThan(0));
		}

		[Test]
		public async Task DumpPdbReportsMissingSymbols()
		{
			// a reference assembly ships without any PDB next to it
			string noPdbAssembly = typeof(object).Assembly.Location;

			var result = await RunAsync(noPdbAssembly, "--disable-updatecheck", "--dump-pdb");

			Assert.That(result.ExitCode, Is.Not.EqualTo(0));
			Assert.That(result.Error, Does.Contain("No debug symbols"));
		}

		[Test]
		public async Task DumpPdbAcceptsExplicitPdbPath()
		{
			string pdbPath = Path.ChangeExtension(testAssemblyPath, ".pdb");
			Assert.That(File.Exists(pdbPath), Is.True, pdbPath);

			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--dump-pdb", "-usepdb:" + pdbPath);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("DumpPdbOptionTests.cs"));
		}

		/// <summary>
		/// Gives <see cref="DumpPdbShowsLocalVariableNames"/> a named local to find in the dump.
		/// </summary>
		public static int ScopeProbe()
		{
			int scopeProbe = 42;
			return scopeProbe;
		}
	}
}
