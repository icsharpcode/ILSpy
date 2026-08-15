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

// The --ilast options exist in debug builds only, so this fixture compiles away with them.
#if DEBUG

using System.Threading.Tasks;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	[TestFixture]
	public class ILAstOptionTests
	{
		static readonly string testAssemblyPath = typeof(ILAstOptionTests).Assembly.Location;

		const string sumLoopId = "M:ICSharpCode.ILSpyCmd.Tests.ILAstSample.SumLoop(System.Int32)";

		static Task<(int ExitCode, string Output, string Error)> RunILAstAsync(params string[] args)
		{
			string[] common = { testAssemblyPath, "--disable-updatecheck", "-m", sumLoopId };
			string[] all = new string[common.Length + args.Length];
			common.CopyTo(all, 0);
			args.CopyTo(all, common.Length);
			return RunAsync(all);
		}

		[Test]
		public async Task ILAstOfSelectedMethodIsWritten()
		{
			var result = await RunILAstAsync("--ilast");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(nameof(ILAstSample.SumLoop)));
			Assert.That(result.Output, Does.Contain("ILFunction"));
		}

		[Test]
		public async Task StoppingAfterATransformYieldsDifferentILAst()
		{
			var full = await RunILAstAsync("--ilast");
			// ILInlining is the third transform of the pipeline; stopping there leaves the
			// method as unstructured blocks, while the full run has loops and expressions.
			var partial = await RunILAstAsync("--after-transform", "ILInlining");

			Assert.That(full.ExitCode, Is.EqualTo(0), full.Error);
			Assert.That(partial.ExitCode, Is.EqualTo(0), partial.Error);
			Assert.That(partial.Output, Does.Contain(nameof(ILAstSample.SumLoop)));
			Assert.That(partial.Output, Is.Not.EqualTo(full.Output));
		}

		[Test]
		public async Task TransformCanBeSelectedByIndex()
		{
			var partial = await RunILAstAsync("--after-transform", "1");
			var full = await RunILAstAsync("--ilast");

			Assert.That(partial.ExitCode, Is.EqualTo(0), partial.Error);
			Assert.That(partial.Output, Is.Not.EqualTo(full.Output));
		}

		[Test]
		public async Task UnknownTransformNameListsThePipeline()
		{
			var result = await RunILAstAsync("--after-transform", "NoSuchTransform");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			// the error must be actionable on its own: it lists the pipeline in run order
			Assert.That(result.Error, Does.Contain("ControlFlowSimplification"));
			Assert.That(result.Error, Does.Contain("AssignVariableNames"));
		}

		[Test]
		public async Task AmbiguousTransformNameReportsItsOccurrences()
		{
			// SplitVariables runs three times; the name alone cannot identify a stop point
			var result = await RunILAstAsync("--after-transform", "SplitVariables");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("SplitVariables"));
			Assert.That(result.Error, Does.Contain("2"));
		}

		[Test]
		public async Task WholeTypeCanBeDumped()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-t", "ICSharpCode.ILSpyCmd.Tests.ILAstSample", "--ilast");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(nameof(ILAstSample.SumLoop)));
			Assert.That(result.Output, Does.Contain(nameof(ILAstSample.Identity)));
		}
	}

	public static class ILAstSample
	{
		public static int SumLoop(int n)
		{
			int sum = 0;
			for (int i = 0; i < n; i++)
			{
				sum += i;
			}
			return sum;
		}

		public static string Identity(string value) => value;
	}
}

#endif
