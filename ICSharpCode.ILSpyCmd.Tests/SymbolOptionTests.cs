// Copyright (c) 2026 Piero Viano
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
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpyX.Symbols;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	[TestFixture]
	public class SymbolOptionTests
	{
		const string SumMethod = "M:ICSharpCode.ILSpyCmd.Tests.SymbolOptionSample.Sum(System.Int32[])";

		static readonly string testAssemblyPath = typeof(SymbolOptionTests).Assembly.Location;

		/// <summary>Copies the test assembly to a directory of its own, without its PDB.</summary>
		static string CopyAssemblyWithoutPdb()
		{
			var dir = Path.Combine(Path.GetTempPath(), $"ILSpyCmdSymbols_{Guid.NewGuid():N}");
			Directory.CreateDirectory(dir);
			var target = Path.Combine(dir, Path.GetFileName(testAssemblyPath));
			File.Copy(testAssemblyPath, target);
			return target;
		}

		[Test]
		public async Task Without_symbols_locals_get_generated_names()
		{
			var result = await RunAsync(CopyAssemblyWithoutPdb(), "--disable-updatecheck", "-usepdb", "-m", SumMethod);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Not.Contain("runningTotalFromPdb"));
		}

		[Test]
		public async Task Symbol_path_supplies_the_pdb_that_is_not_next_to_the_assembly()
		{
			var assembly = CopyAssemblyWithoutPdb();
			var pdbDirectory = Path.GetDirectoryName(testAssemblyPath)!;

			var result = await RunAsync(assembly, "--disable-updatecheck", "--symbol-path", pdbDirectory, "-m", SumMethod);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("runningTotalFromPdb"));
		}

		[Test]
		public async Task Serve_symbols_serves_the_input_assembly_until_stopped()
		{
			using var stop = new CancellationTokenSource();
			ILSpyCmdProgram.ServeSymbolsStopToken = stop.Token;
			try
			{
				var run = RunAsync(testAssemblyPath, "--disable-updatecheck", "--serve-symbols", "0");
				string address = null;
				for (int i = 0; i < 200 && address == null && !run.IsCompleted; i++)
				{
					await Task.Delay(50);
					address = Regex.Match(CurrentOutput(), @"http://localhost:\d+/").Value is { Length: > 0 } a ? a : null;
				}
				Assert.That(address, Is.Not.Null, "the server prints its address once it listens");

				using var reader = new PEReader(File.OpenRead(testAssemblyPath));
				var key = SymbolKey.GetPEKey(reader, testAssemblyPath)!;
				using var client = new HttpClient { BaseAddress = new Uri(address!) };
				var bytes = await client.GetByteArrayAsync(key.Key);
				Assert.That(bytes, Is.EqualTo(File.ReadAllBytes(testAssemblyPath)));

				stop.Cancel();
				var result = await run.WaitAsync(TimeSpan.FromSeconds(30));
				Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			}
			finally
			{
				ILSpyCmdProgram.ServeSymbolsStopToken = CancellationToken.None;
			}
		}
	}

	public static class SymbolOptionSample
	{
		public static int Sum(int[] values)
		{
			int runningTotalFromPdb = 0;
			foreach (int value in values)
				runningTotalFromPdb += value * 2 + 1;
			return runningTotalFromPdb;
		}
	}
}
