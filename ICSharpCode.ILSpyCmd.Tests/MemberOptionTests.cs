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
using System.Threading.Tasks;

using NUnit.Framework;

namespace ICSharpCode.ILSpyCmd.Tests
{
	[TestFixture]
	public class ILSpyCmdMemberOptionTests
	{
		static readonly string testAssemblyPath = typeof(ILSpyCmdMemberOptionTests).Assembly.Location;

		static async Task<(int ExitCode, string Output, string Error)> RunAsync(params string[] args)
		{
			var originalOut = Console.Out;
			var originalError = Console.Error;
			var stdout = new StringWriter();
			var stderr = new StringWriter();
			try
			{
				Console.SetOut(stdout);
				Console.SetError(stderr);
				int exitCode = await ILSpyCmdProgram.Main(args);
				return (exitCode, stdout.ToString(), stderr.ToString());
			}
			finally
			{
				Console.SetOut(originalOut);
				Console.SetError(originalError);
			}
		}

		[Test]
		public async Task MethodByDocumentationIdString()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Add(System.Int32,System.Int32)");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
			Assert.That(result.Output, Does.Not.Contain("Unrelated"));
		}

		[Test]
		public async Task PropertyByDocumentationIdString()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-m", "P:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Answer");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("Answer"));
			Assert.That(result.Output, Does.Not.Contain("Add(int a, int b)"));
		}

		[Test]
		public async Task MethodByMetadataToken()
		{
			int token = typeof(MemberOptionSample).GetMethod(nameof(MemberOptionSample.Add))!.MetadataToken;
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-m", $"0x{token:x8}");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
		}

		[Test]
		public async Task UnknownMemberReportsError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.DoesNotExist");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			Assert.That(result.Error, Does.Contain("DoesNotExist"));
		}

		[Test]
		public async Task MalformedTokenReportsTokenSpecificError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-m", "0xZZ000001");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			Assert.That(result.Error, Does.Contain("metadata token"));
			Assert.That(result.Error, Does.Not.Contain("documentation id"));
		}

		[Test]
		public async Task MemberOfAnotherModuleReportsDistinctError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-m", "M:System.Object.ToString");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			Assert.That(result.Error, Does.Contain("defined in"));
		}

		[Test]
		public async Task SurroundingWhitespaceIsIgnored()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-m", " M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Add(System.Int32,System.Int32) ");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
		}

		[Test]
		public async Task OutOfRangeTokenReportsError()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck", "-m", "0x06ffffff");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			Assert.That(result.Error, Does.Contain("0x06ffffff"));
		}

		[Test]
		public async Task TypeAndMemberOptionsAreMutuallyExclusive()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-t", "ICSharpCode.ILSpyCmd.Tests.MemberOptionSample",
				"-m", "P:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Answer");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
		}
	}

	public class MemberOptionSample
	{
		public int Add(int a, int b)
		{
			return a + b;
		}

		public string Answer => "42";

		public void Unrelated()
		{
		}
	}
}
