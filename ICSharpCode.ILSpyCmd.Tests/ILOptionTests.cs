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

using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// -il and --il-sequence-points apply the same --type / --member selection as the C# output.
	/// </summary>
	[TestFixture]
	public class ILOptionTests
	{
		static readonly string testAssemblyPath = typeof(ILOptionTests).Assembly.Location;

		const string sampleType = "ICSharpCode.ILSpyCmd.Tests.MemberOptionSample";

		// a type of this assembly that is outside the selection in every test below
		const string otherType = nameof(CliTestRunner);

		[Test]
		public async Task TypeSelectionDisassemblesOnlyThatType()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-il", "-t", sampleType);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(".class"));
			Assert.That(result.Output, Does.Contain("MemberOptionSample"));
			Assert.That(result.Output, Does.Contain("int32 Add ("));
			Assert.That(result.Output, Does.Not.Contain(otherType));
		}

		[Test]
		public async Task MethodSelectionDisassemblesOnlyThatMethod()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-il",
				"-m", $"M:{sampleType}.Add(System.Int32,System.Int32)");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(".method"));
			Assert.That(result.Output, Does.Contain("int32 Add ("));
			Assert.That(result.Output, Does.Not.Contain("Unrelated"));
			Assert.That(result.Output, Does.Not.Contain(otherType));
		}

		[Test]
		public async Task PropertySelectionIncludesItsAccessors()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-il",
				"-m", $"P:{sampleType}.Answer");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(".property"));
			Assert.That(result.Output, Does.Contain("get_Answer"));
			Assert.That(result.Output, Does.Not.Contain("Add ("));
		}

		[Test]
		public async Task ShortFormOfOverloadedMemberDisassemblesEveryMember()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-il",
				"-m", $"M:{sampleType}.Scale");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("instance int32 Scale ("));
			Assert.That(result.Output, Does.Contain("instance string Scale ("));
			Assert.That(result.Output, Does.Contain("names 2 members"));
		}

		[Test]
		public async Task SequencePointsHonorTheSelection()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "--il-sequence-points", "-t", sampleType);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("MemberOptionSample"));
			Assert.That(result.Output, Does.Not.Contain(otherType));
		}

		[Test]
		public async Task TypeAndMemberOptionsAreMutuallyExclusive()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-il",
				"-t", sampleType, "-m", $"P:{sampleType}.Answer");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
		}

		[Test]
		public async Task UnknownTypeReportsError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-il", "-t", "Does.Not.Exist");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			Assert.That(result.Error, Does.Contain("Does.Not.Exist"));
		}

		[Test]
		public async Task OutputFileIsNamedAfterTheSelectedType()
		{
			string outputDirectory = Path.Combine(Path.GetTempPath(), "ilspycmd-il-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(outputDirectory);
			try
			{
				var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-il", "-t", sampleType, "-o", outputDirectory);

				Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
				string ilFile = Path.Combine(outputDirectory, sampleType + ".il");
				Assert.That(File.Exists(ilFile), Is.True, "expected " + ilFile);
				Assert.That(File.ReadAllText(ilFile), Does.Not.Contain(otherType));
			}
			finally
			{
				Directory.Delete(outputDirectory, recursive: true);
			}
		}
	}
}
