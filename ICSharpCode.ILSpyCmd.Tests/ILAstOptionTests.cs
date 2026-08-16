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

using System.IO;
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

		// the loop of SumLoop is only recognised by HighLevelLoopTransform, at the very end of the
		// pipeline: its presence tells a full run from a truncated one, while the header line
		// (which names the requested transform count) would differ either way
		const string structuredLoop = "BlockContainer (for)";

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
			Assert.That(full.Output, Does.Contain(structuredLoop));
			Assert.That(partial.Output, Does.Not.Contain(structuredLoop));
		}

		[Test]
		public async Task TransformCanBeSelectedByIndex()
		{
			var partial = await RunILAstAsync("--after-transform", "1");

			Assert.That(partial.ExitCode, Is.EqualTo(0), partial.Error);
			Assert.That(partial.Output, Does.Contain(nameof(ILAstSample.SumLoop)));
			Assert.That(partial.Output, Does.Not.Contain(structuredLoop));
			// AssignVariableNames is the last transform, so the locals still carry their IL names
			Assert.That(partial.Output, Does.Contain("local V_0"));
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
			// SplitVariables runs more than once; the name alone cannot identify a stop point
			var result = await RunILAstAsync("--after-transform", "SplitVariables");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			// the message has to be the ambiguity one, not the unknown-name listing, which
			// mentions every transform of the pipeline as well
			Assert.That(result.Error, Does.Contain("'SplitVariables' runs"));
			Assert.That(result.Error, Does.Contain("times, at index"));
			Assert.That(result.Error, Does.Not.Contain("Unknown transform"));
		}

		[Test]
		public async Task NestedTransformNamesTheEntryThatRunsIt()
		{
			// LoopDetection runs inside a BlockILTransform, so it has no stop point of its own;
			// the pipeline listing has to show where it runs instead of hiding it
			var result = await RunILAstAsync("--after-transform", "LoopDetection");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("runs inside the transform at index"));
			Assert.That(result.Error, Does.Contain("BlockILTransform (LoopDetection"));
		}

		[Test]
		public async Task DebugSymbolsProvideLocalVariableNames()
		{
			var withPdb = await RunILAstAsync("--ilast", "-usepdb");
			var withoutPdb = await RunILAstAsync("--ilast");

			Assert.That(withPdb.ExitCode, Is.EqualTo(0), withPdb.Error);
			// the PDB's name for the accumulator, instead of the generated 'num'
			Assert.That(withPdb.Output, Does.Contain("local sum"));
			Assert.That(withoutPdb.Output, Does.Not.Contain("local sum"));
		}

		[Test]
		public async Task WholeTypeCanBeDumped()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-t", "ICSharpCode.ILSpyCmd.Tests.ILAstSample", "--ilast");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(nameof(ILAstSample.SumLoop)));
			Assert.That(result.Output, Does.Contain(nameof(ILAstSample.Identity)));
			// accessors are methods with bodies too, and the type system's Methods hides them
			Assert.That(result.Output, Does.Contain("get_" + nameof(ILAstSample.Counter)));
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
				// one transform only: this dumps every method of both assemblies, and the
				// truncation it guards against does not depend on the pipeline length
				var result = await RunAsync(testAssemblyPath, ilspyCmdAssemblyPath,
					"--disable-updatecheck", "--after-transform", "1", "-o", outputDir);

				Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
				foreach (string assemblyPath in new[] { testAssemblyPath, ilspyCmdAssemblyPath })
				{
					string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(assemblyPath) + ".ilast");
					Assert.That(File.Exists(outputFile), Is.True, outputFile);
					// every function ends with its closing brace; a truncated file breaks off
					// wherever the writer's buffer happened to end
					Assert.That(File.ReadAllText(outputFile).TrimEnd(), Does.EndWith("}"), outputFile);
				}
			}
			finally
			{
				Directory.Delete(outputDir, recursive: true);
			}
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

		public static int Counter { get; set; }
	}
}

#endif
