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

using System.Threading.Tasks;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// A mistake on the command line is answered with a one-line message and EX_USAGE,
	/// never with an unhandled exception.
	/// </summary>
	[TestFixture]
	public class ILSpyCmdUsageErrorTests
	{
		static readonly string testAssemblyPath = typeof(ILSpyCmdUsageErrorTests).Assembly.Location;

		[Test]
		public async Task UnknownOptionIsReportedAsUsageError()
		{
			var result = await RunAsync(testAssemblyPath, "--no-such-option");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("Unrecognized option '--no-such-option'"));
			Assert.That(result.Error, Does.Not.Contain("Exception"));
		}

		[Test]
		public async Task InvalidLanguageVersionIsReportedAsUsageError()
		{
			var result = await RunAsync(testAssemblyPath, "-lv", "CSharp99", "-t", "Some.Type");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("Invalid value specified for languageversion"));
			Assert.That(result.Error, Does.Not.Contain("Exception"));
		}

		[Test]
		public async Task ProjectWithoutOutputDirectoryIsReportedAsUsageError()
		{
			var result = await RunAsync(testAssemblyPath, "-p");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("--project"));
			Assert.That(result.Error, Does.Contain("--outputdir"));
		}
	}
}
