// Copyright (c) 2026 Christoph Wille
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
using System.Threading.Tasks;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// Pins the command-line surface of ilspycmd: which option spellings, value delimiters and
	/// argument orders are accepted, how every option binds, and which mistakes exit with
	/// EX_USAGE. The command-line parsing library is an implementation detail behind this
	/// contract, so these tests assert exit codes and the offending token, never the wording
	/// of a library's messages. The other fixtures cover what the options do; this one covers
	/// that they are still reachable from the command line.
	/// </summary>
	[TestFixture]
	public class ArgumentParsingTests
	{
		static readonly string testAssemblyPath = typeof(ArgumentParsingTests).Assembly.Location;
		static readonly string pdbPath = Path.ChangeExtension(testAssemblyPath, ".pdb");
		const string sampleType = "ICSharpCode.ILSpyCmd.Tests.MemberOptionSample";
		const string sampleInterface = "ICSharpCode.ILSpyCmd.Tests.ArgumentParsingSampleInterface";
		const string resourceContainer = "ICSharpCode.ILSpyCmd.Tests.g.resources";

		string tempDirectory;

		[SetUp]
		public void SetUp()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), "ILSpyCmdArgs_" + Path.GetRandomFileName());
			Directory.CreateDirectory(tempDirectory);
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(tempDirectory))
				Directory.Delete(tempDirectory, recursive: true);
		}

		// Placeholders in the test case rows, filled in at run time: the assembly path and the
		// temp directory are only known then.
		const string Asm = "{asm}";
		const string Pdb = "{pdb}";
		const string AsmDir = "{asmdir}";
		const string Temp = "{temp}";

		string[] Expand(string[] args)
		{
			string[] expanded = new string[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				expanded[i] = args[i]
					.Replace(Asm, testAssemblyPath)
					.Replace(Pdb, pdbPath)
					.Replace(AsmDir, Path.GetDirectoryName(testAssemblyPath))
					.Replace(Temp, tempDirectory);
			}
			return expanded;
		}

		[TestCase(new[] { "-il", "-t", sampleType, Asm }, "MemberOptionSample", TestName = "OptionsBeforeTheAssembly")]
		[TestCase(new[] { Asm, "--type=" + sampleType, "-il" }, "MemberOptionSample", TestName = "EqualsSignDelimitsLongOptionValue")]
		[TestCase(new[] { Asm, "-t:" + sampleType, "-il" }, "MemberOptionSample", TestName = "ColonDelimitsShortOptionValue")]
		[TestCase(new[] { Asm, "-lv", "csharp8_0", "-t", sampleType }, "MemberOptionSample", TestName = "LanguageVersionIsCaseInsensitive")]
		[TestCase(new[] { Asm, "-lv", "CSharp8_0", "-t", sampleType }, "MemberOptionSample", TestName = "LanguageVersionCanonicalCase")]
		[TestCase(new[] { Asm, "-usepdb", "-t", sampleType }, "MemberOptionSample", TestName = "UsePdbWithoutValueUsesThePdbNextToTheAssembly")]
		[TestCase(new[] { Asm, "-usepdb:" + Pdb, "-t", sampleType }, "MemberOptionSample", TestName = "UsePdbWithColonValue")]
		[TestCase(new[] { Asm, "-ds", "RemoveDeadCode=true", "-ds", "RemoveDeadStores=true", "-t", sampleType }, "MemberOptionSample", TestName = "RepeatedDecompilerSettingOption")]
		[TestCase(new[] { Asm, "-r", AsmDir, "-r", Temp, "-t", sampleType }, "MemberOptionSample", TestName = "RepeatedReferencePathOption")]
		[TestCase(new[] { Asm, "--ignore-decompilation-errors", "--no-dead-code", "--no-dead-stores", "-t", sampleType }, "MemberOptionSample", TestName = "FlagsWithoutValuesCombine")]
		public async Task AcceptedSyntaxDecompilesTheSelectedType(string[] args, string expectedOutput)
		{
			var result = await RunAsync(Expand(args));

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(expectedOutput));
		}

		[Test]
		public async Task ListOptionTakesEntityLetters()
		{
			var result = await RunAsync(testAssemblyPath, "-l", "ci");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("Class " + sampleType));
			Assert.That(result.Output, Does.Contain("Interface " + sampleInterface));
		}

		[Test]
		public async Task ListOptionRepeatedWithEntityNames()
		{
			var result = await RunAsync(testAssemblyPath, "-l", "class", "-l", "interface");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("Class " + sampleType));
			Assert.That(result.Output, Does.Contain("Interface " + sampleInterface));
		}

		[Test]
		public async Task ListOptionSelectsOnlyTheNamedKind()
		{
			var result = await RunAsync(testAssemblyPath, "--list", "i");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("Interface " + sampleInterface));
			Assert.That(result.Output, Does.Not.Contain("Class "));
		}

		[Test]
		public async Task GeneratePdbWritesIntoTheOutputDirectory()
		{
			var result = await RunAsync(testAssemblyPath, "-genpdb", "-o", tempDirectory);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(File.Exists(Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(testAssemblyPath) + ".pdb")), Is.True);
		}

		[Test]
		public async Task ListResourcesPrintsContainerEntries()
		{
			var result = await RunAsync(testAssemblyPath, "--list-resources");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(resourceContainer + "/mainwindow.baml"));
		}

		[Test]
		public async Task ResourceOptionExtractsIntoTheOutputDirectory()
		{
			var result = await RunAsync(testAssemblyPath, "--resource", resourceContainer, "-o", tempDirectory);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(File.Exists(Path.Combine(tempDirectory, resourceContainer)), Is.True);
		}

		[Test]
		public async Task DiagrammerFilterOptionsBind()
		{
			string docsPath = Path.Combine(tempDirectory, "docs.xml");
			File.WriteAllText(docsPath, "<?xml version=\"1.0\"?><doc><assembly><name>ICSharpCode.ILSpyCmd.Tests</name></assembly><members></members></doc>");
			string outputDirectory = Path.Combine(tempDirectory, "diagrammer");

			var result = await RunAsync(testAssemblyPath,
				"--generate-diagrammer", "--generate-diagrammer-json-only",
				"--generate-diagrammer-include", @"ICSharpCode\.ILSpyCmd\.Tests\..+",
				"--generate-diagrammer-exclude", ".+MemberOptionSample",
				"--generate-diagrammer-report-excluded",
				"--generate-diagrammer-strip-namespaces", "System.Collections", "--generate-diagrammer-strip-namespaces", "System",
				"--generate-diagrammer-docs", docsPath,
				"-o", outputDirectory);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			string report = Path.Combine(outputDirectory, "excluded types.txt");
			Assert.That(File.Exists(report), Is.True, report);
			Assert.That(File.ReadAllText(report), Does.Contain("MemberOptionSample"));
		}

		[TestCase("-v")]
		[TestCase("--version")]
		public async Task VersionOptionPrintsBothVersions(string option)
		{
			var result = await RunAsync(option);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Match(@"^ilspycmd: \d+\.\d+"));
			Assert.That(result.Output, Does.Contain("ICSharpCode.Decompiler:"));
		}

		// The public option surface. A hardcoded list on purpose: the test must not learn the
		// surface from the parsing library's own attributes.
		static readonly string[] documentedOptions = {
			"--outputdir", "--project", "--type", "--member", "--ilcode", "--il-sequence-points",
			"--generate-pdb", "--use-varnames-from-pdb", "--list", "--list-resources", "--resource",
			"--decompile-baml", "--dump-table", "--dump-pdb", "--json", "--languageversion",
			"--ilspy-settingsfile", "--decompiler-setting", "--referencepath",
			"--ignore-decompilation-errors", "--no-dead-code", "--no-dead-stores", "--dump-package",
			"--bundle-entry", "--nested-directories", "--disable-updatecheck", "--generate-diagrammer",
			"--generate-diagrammer-include", "--generate-diagrammer-exclude",
			"--generate-diagrammer-report-excluded", "--generate-diagrammer-docs",
			"--generate-diagrammer-strip-namespaces",
#if DEBUG
			"--ilast", "--after-transform",
#endif
		};

		[TestCase("-h")]
		[TestCase("--help")]
		public async Task HelpListsEveryDocumentedOption(string option)
		{
			var result = await RunAsync(option);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			foreach (string name in documentedOptions)
				Assert.That(result.Output, Does.Contain(name));
			// a developer option, kept out of the help on purpose
			Assert.That(result.Output, Does.Not.Contain("--generate-diagrammer-json-only"));
		}

		[TestCase(new string[0], "Assembly file name", TestName = "NoArgumentsAtAll")]
		[TestCase(new[] { "does-not-exist.dll" }, "does-not-exist.dll", TestName = "MissingInputFile")]
		[TestCase(new[] { Asm, "-usepdb=missing.pdb", "-t", sampleType }, "missing.pdb", TestName = "MissingPdbFile")]
		[TestCase(new[] { Asm, "-r", Temp + "/no-such-reference-dir", "-t", sampleType }, "no-such-reference-dir", TestName = "MissingReferenceDirectory")]
		[TestCase(new[] { Asm, "--ilspy-settingsfile", "missing.xml", "-t", sampleType }, "missing.xml", TestName = "MissingSettingsFile")]
		[TestCase(new[] { Asm, "-i" }, "-i", TestName = "ShortAliasesAreNotBundled")]
		public async Task UsageMistakeExitsWithUsageErrorNamingTheToken(string[] args, string token)
		{
			var result = await RunAsync(Expand(args));

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE), result.Error);
			Assert.That(result.Error, Does.Contain(token));
			Assert.That(result.Error, Does.Not.Contain("Exception"));
		}
	}

	/// <summary>The one interface in this assembly, for the --list tests.</summary>
	public interface ArgumentParsingSampleInterface
	{
	}
}
