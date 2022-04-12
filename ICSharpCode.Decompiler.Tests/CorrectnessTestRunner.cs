// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Tests.Helpers;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class CorrectnessTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/Correctness";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(CorrectnessTestRunner).GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
				.Select(m => m.Name)
				.ToArray();
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles())
			{
				if (file.Extension == ".txt" || file.Extension == ".exe" || file.Extension == ".config")
					continue;
				var testName = Path.GetFileNameWithoutExtension(file.Name);
				Assert.Contains(testName, testNames);
			}
		}

		static readonly CompilerOptions[] noMonoOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] net40OnlyOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40
		};

		static readonly CompilerOptions[] defaultOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
			CompilerOptions.UseMcs2_6_4,
			CompilerOptions.Optimize | CompilerOptions.UseMcs2_6_4,
			CompilerOptions.UseMcs5_23,
			CompilerOptions.Optimize | CompilerOptions.UseMcs5_23
		};

		static readonly CompilerOptions[] roslynOnlyOptions =
		{
			CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn1_3_2,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslyn2OrNewerOptions =
		{
			CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0 | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn2_10_0,
			CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn3_11_0,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		static readonly CompilerOptions[] roslynLatestOnlyOptions =
		{
			CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest | CompilerOptions.TargetNet40,
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		[Test]
		public async Task Comparisons([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task Conversions([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task FloatingPointArithmetic([ValueSource(nameof(noMonoOptions))] CompilerOptions options, [Values(32, 64)] int bits)
		{
			// The behavior of the #1794 incorrect `(float)(double)val` cast only causes test failures
			// for some runtime+compiler combinations.
			if (bits == 32)
				options |= CompilerOptions.Force32Bit;
			// Mono is excluded because we never use it for the second pass, so the test ends up failing
			// due to some Mono vs. Roslyn compiler differences.
			await RunCS(options: options);
		}

		[Test]
		public async Task HelloWorld([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task ControlFlow([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task CompoundAssignment([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task PropertiesAndEvents([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task Switch([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task Using([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task Loops([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task NullableTests([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task Generics([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task ValueTypeCall([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task InitializerTests([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task DecimalFields([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task UndocumentedExpressions([ValueSource(nameof(noMonoOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task Uninit([ValueSource(nameof(noMonoOptions))] CompilerOptions options)
		{
			await RunVB(options: options);
		}

		[Test]
		public async Task MemberLookup([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task OverloadResolution([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task ExpressionTrees([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task NullPropagation([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task DeconstructionTests([ValueSource(nameof(roslyn2OrNewerOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task BitNot([Values(false, true)] bool force32Bit)
		{
			CompilerOptions compiler = CompilerOptions.UseDebug;
			AssemblerOptions asm = AssemblerOptions.None;
			if (force32Bit)
			{
				compiler |= CompilerOptions.Force32Bit;
				asm |= AssemblerOptions.Force32Bit;
			}
			await RunIL("BitNot.il", compiler, asm);
		}

		[Test]
		public async Task Jmp()
		{
			await RunIL("Jmp.il");
		}

		[Test]
		public async Task StackTests()
		{
			// IL contains .corflags = 32BITREQUIRED
			await RunIL("StackTests.il", asmOptions: AssemblerOptions.Force32Bit);
		}

		[Test]
		public async Task StackTypes([Values(false, true)] bool force32Bit)
		{
			CompilerOptions compiler = CompilerOptions.UseRoslynLatest | CompilerOptions.UseDebug;
			AssemblerOptions asm = AssemblerOptions.None;
			if (force32Bit)
			{
				compiler |= CompilerOptions.Force32Bit;
				asm |= AssemblerOptions.Force32Bit;
			}
			await RunIL("StackTypes.il", compiler, asm);
		}

		[Test]
		public async Task UnsafeCode([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			if (options.HasFlag(CompilerOptions.UseMcs2_6_4))
			{
				Assert.Ignore("Decompiler bug with mono!");
			}
			await RunCS(options: options);
		}

		[Test]
		public async Task ConditionalAttr([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task TrickyTypes([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task Capturing([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task YieldReturn([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			if ((options & CompilerOptions.UseMcsMask) != 0)
			{
				Assert.Ignore("Decompiler bug with mono!");
			}
			await RunCS(options: options);
		}

		[Test]
		public async Task Async([ValueSource(nameof(noMonoOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task LINQRaytracer([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task StringConcat([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task DynamicTests([ValueSource(nameof(noMonoOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		[Test]
		public async Task MiniJSON([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			if (options.HasFlag(CompilerOptions.UseMcs2_6_4))
			{
				Assert.Ignore("Decompiler bug with mono!");
			}
			await RunCS(options: options);
		}

		[Test]
		public async Task ComInterop([ValueSource(nameof(net40OnlyOptions))] CompilerOptions options)
		{
			await RunCS(options: options);
		}

		async Task RunCS([CallerMemberName] string testName = null, CompilerOptions options = CompilerOptions.UseDebug)
		{
			if ((options & CompilerOptions.UseRoslynMask) != 0 && (options & CompilerOptions.TargetNet40) == 0)
				options |= CompilerOptions.UseTestRunner;
			string testFileName = testName + ".cs";
			string testOutputFileName = testName + Tester.GetSuffix(options) + ".exe";
			CompilerResults outputFile = null, decompiledOutputFile = null;

			try
			{
				outputFile = await Tester.CompileCSharp(Path.Combine(TestCasePath, testFileName), options,
					outputFileName: Path.Combine(TestCasePath, testOutputFileName)).ConfigureAwait(false);
				string decompiledCodeFile = await Tester.DecompileCSharp(outputFile.PathToAssembly, Tester.GetSettings(options)).ConfigureAwait(false);
				if ((options & CompilerOptions.UseMcsMask) != 0)
				{
					// For second pass, use roslyn instead of mcs.
					// mcs has some compiler bugs that cause it to not accept ILSpy-generated code,
					// for example when there's unreachable code due to other compiler bugs in the first mcs run.
					options &= ~CompilerOptions.UseMcsMask;
					options |= CompilerOptions.UseRoslynLatest;
					// Also, add an .exe.config so that we consistently use the .NET 4.x runtime.
					File.WriteAllText(outputFile.PathToAssembly + ".config", @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
	<startup>
		<supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.0,Profile=Client"" />
	</startup>
</configuration>");
					options |= CompilerOptions.TargetNet40;
				}
				decompiledOutputFile = await Tester.CompileCSharp(decompiledCodeFile, options).ConfigureAwait(false);

				await Tester.RunAndCompareOutput(testFileName, outputFile.PathToAssembly, decompiledOutputFile.PathToAssembly, decompiledCodeFile, (options & CompilerOptions.UseTestRunner) != 0, (options & CompilerOptions.Force32Bit) != 0);

				Tester.RepeatOnIOError(() => File.Delete(decompiledCodeFile));
				Tester.RepeatOnIOError(() => File.Delete(decompiledOutputFile.PathToAssembly));
			}
			finally
			{
				if (outputFile != null)
					outputFile.DeleteTempFiles();
				if (decompiledOutputFile != null)
					decompiledOutputFile.DeleteTempFiles();
			}
		}

		async Task RunVB([CallerMemberName] string testName = null, CompilerOptions options = CompilerOptions.UseDebug)
		{
			options |= CompilerOptions.ReferenceVisualBasic;
			if ((options & CompilerOptions.UseRoslynMask) != 0)
				options |= CompilerOptions.UseTestRunner;
			string testFileName = testName + ".vb";
			string testOutputFileName = testName + Tester.GetSuffix(options) + ".exe";
			CompilerResults outputFile = null, decompiledOutputFile = null;

			try
			{
				outputFile = await Tester.CompileVB(Path.Combine(TestCasePath, testFileName), options,
					outputFileName: Path.Combine(TestCasePath, testOutputFileName)).ConfigureAwait(false);
				string decompiledCodeFile = await Tester.DecompileCSharp(outputFile.PathToAssembly, Tester.GetSettings(options)).ConfigureAwait(false);
				decompiledOutputFile = await Tester.CompileCSharp(decompiledCodeFile, options).ConfigureAwait(false);

				await Tester.RunAndCompareOutput(testFileName, outputFile.PathToAssembly, decompiledOutputFile.PathToAssembly, decompiledCodeFile, (options & CompilerOptions.UseTestRunner) != 0, (options & CompilerOptions.Force32Bit) != 0);

				Tester.RepeatOnIOError(() => File.Delete(decompiledCodeFile));
				Tester.RepeatOnIOError(() => File.Delete(decompiledOutputFile.PathToAssembly));
			}
			finally
			{
				if (outputFile != null)
					outputFile.DeleteTempFiles();
				if (decompiledOutputFile != null)
					decompiledOutputFile.DeleteTempFiles();
			}
		}

		async Task RunIL(string testFileName, CompilerOptions options = CompilerOptions.UseDebug, AssemblerOptions asmOptions = AssemblerOptions.None)
		{
			string outputFile = null;
			CompilerResults decompiledOutputFile = null;

			try
			{
				options |= CompilerOptions.UseTestRunner;
				outputFile = await Tester.AssembleIL(Path.Combine(TestCasePath, testFileName), asmOptions).ConfigureAwait(false);
				string decompiledCodeFile = await Tester.DecompileCSharp(outputFile, Tester.GetSettings(options)).ConfigureAwait(false);
				decompiledOutputFile = await Tester.CompileCSharp(decompiledCodeFile, options).ConfigureAwait(false);

				await Tester.RunAndCompareOutput(testFileName, outputFile, decompiledOutputFile.PathToAssembly, decompiledCodeFile, (options & CompilerOptions.UseTestRunner) != 0, (options & CompilerOptions.Force32Bit) != 0).ConfigureAwait(false);

				Tester.RepeatOnIOError(() => File.Delete(decompiledCodeFile));
				Tester.RepeatOnIOError(() => File.Delete(decompiledOutputFile.PathToAssembly));
			}
			finally
			{
				if (decompiledOutputFile != null)
					decompiledOutputFile.DeleteTempFiles();
			}
		}
	}
}
