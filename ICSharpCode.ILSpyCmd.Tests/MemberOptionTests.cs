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

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	[TestFixture]
	public class ILSpyCmdMemberOptionTests
	{
		static readonly string testAssemblyPath = typeof(ILSpyCmdMemberOptionTests).Assembly.Location;

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

		/// <summary>
		/// The short form is what a user reaches for, because knowing the parameter list means
		/// knowing the overload count beforehand. Where it names one member, it just works.
		/// </summary>
		[Test]
		public async Task ShortFormWithoutSignatureDecompilesTheMember()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Add");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
			Assert.That(result.Output, Does.Not.Contain("names 2 members"));
		}

		/// <summary>
		/// The short form of an overloaded member names the whole group. Every member is shown -
		/// making the user re-run with a full signature would defeat the point of accepting the
		/// short form - with a comment saying the ID was ambiguous and what it matched.
		/// </summary>
		[Test]
		public async Task ShortFormOfOverloadedMemberDecompilesEveryMember()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Scale");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Scale(int value)"));
			Assert.That(result.Output, Does.Contain("string Scale(string value)"));
			Assert.That(result.Output, Does.Contain("names 2 members"));
			Assert.That(result.Output, Does.Contain("M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Scale(System.Int32)"));
			Assert.That(result.Output, Does.Contain("M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Scale(System.String)"));
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

		public int Scale(int value)
		{
			return value * 2;
		}

		public string Scale(string value)
		{
			return value + value;
		}
	}
}
