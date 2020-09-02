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
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn,
		};

		static readonly CompilerOptions[] defaultOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn,
			CompilerOptions.UseMcs,
			CompilerOptions.Optimize | CompilerOptions.UseMcs
		};

		static readonly CompilerOptions[] roslynOnlyOptions =
		{
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn
		};

		[Test]
		public void Comparisons([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void Conversions([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void FloatingPointArithmetic([ValueSource("noMonoOptions")] CompilerOptions options, [Values(32, 64)] int bits)
		{
			// The behavior of the #1794 incorrect `(float)(double)val` cast only causes test failures
			// for some runtime+compiler combinations.
			if (bits == 32)
				options |= CompilerOptions.Force32Bit;
			// Mono is excluded because we never use it for the second pass, so the test ends up failing
			// due to some Mono vs. Roslyn compiler differences.
			RunCS(options: options);
		}

		[Test]
		public void HelloWorld([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void ControlFlow([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void CompoundAssignment([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void PropertiesAndEvents([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void Switch([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void Using([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void Loops([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void NullableTests([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void Generics([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void ValueTypeCall([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void InitializerTests([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void DecimalFields([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void UndocumentedExpressions([ValueSource("noMonoOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void Uninit([ValueSource("noMonoOptions")] CompilerOptions options)
		{
			RunVB(options: options);
		}

		[Test]
		public void MemberLookup([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void OverloadResolution([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void ExpressionTrees([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void NullPropagation([ValueSource("roslynOnlyOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void DeconstructionTests([ValueSource("roslynOnlyOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void BitNot([Values(false, true)] bool force32Bit)
		{
			CompilerOptions compiler = CompilerOptions.UseDebug;
			AssemblerOptions asm = AssemblerOptions.None;
			if (force32Bit)
			{
				compiler |= CompilerOptions.Force32Bit;
				asm |= AssemblerOptions.Force32Bit;
			}
			RunIL("BitNot.il", compiler, asm);
		}

		[Test]
		public void Jmp()
		{
			RunIL("Jmp.il");
		}

		[Test]
		public void StackTests()
		{
			RunIL("StackTests.il");
		}

		[Test]
		public void StackTypes([Values(false, true)] bool force32Bit)
		{
			CompilerOptions compiler = CompilerOptions.UseRoslyn | CompilerOptions.UseDebug;
			AssemblerOptions asm = AssemblerOptions.None;
			if (force32Bit)
			{
				compiler |= CompilerOptions.Force32Bit;
				asm |= AssemblerOptions.Force32Bit;
			}
			RunIL("StackTypes.il", compiler, asm);
		}

		[Test]
		public void UnsafeCode([ValueSource("defaultOptions")] CompilerOptions options)
		{
			if (options.HasFlag(CompilerOptions.UseMcs))
			{
				Assert.Ignore("Decompiler bug with mono!");
			}
			RunCS(options: options);
		}

		[Test]
		public void ConditionalAttr([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void TrickyTypes([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void Capturing([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void YieldReturn([ValueSource("defaultOptions")] CompilerOptions options)
		{
			if (options.HasFlag(CompilerOptions.UseMcs))
			{
				Assert.Ignore("Decompiler bug with mono!");
			}
			RunCS(options: options);
		}

		[Test]
		public void Async([ValueSource("noMonoOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void LINQRaytracer([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void StringConcat([ValueSource("defaultOptions")] CompilerOptions options)
		{
			RunCS(options: options);
		}

		[Test]
		public void MiniJSON([ValueSource("defaultOptions")] CompilerOptions options)
		{
			if (options.HasFlag(CompilerOptions.UseMcs))
			{
				Assert.Ignore("Decompiler bug with mono!");
			}
			RunCS(options: options);
		}

		void RunCS([CallerMemberName] string testName = null, CompilerOptions options = CompilerOptions.UseDebug)
		{
			string testFileName = testName + ".cs";
			string testOutputFileName = testName + Tester.GetSuffix(options) + ".exe";
			CompilerResults outputFile = null, decompiledOutputFile = null;

			try
			{
				outputFile = Tester.CompileCSharp(Path.Combine(TestCasePath, testFileName), options,
					outputFileName: Path.Combine(TestCasePath, testOutputFileName));
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile.PathToAssembly, Tester.GetSettings(options));
				if (options.HasFlag(CompilerOptions.UseMcs))
				{
					// For second pass, use roslyn instead of mcs.
					// mcs has some compiler bugs that cause it to not accept ILSpy-generated code,
					// for example when there's unreachable code due to other compiler bugs in the first mcs run.
					options &= ~CompilerOptions.UseMcs;
					options |= CompilerOptions.UseRoslyn;
					// Also, add an .exe.config so that we consistently use the .NET 4.x runtime.
					File.WriteAllText(outputFile.PathToAssembly + ".config", @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
	<startup>
		<supportedRuntime version=""v4.0"" sku="".NETFramework,Version=v4.0,Profile=Client"" />
	</startup>
</configuration>");
				}
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, options);

				Tester.RunAndCompareOutput(testFileName, outputFile.PathToAssembly, decompiledOutputFile.PathToAssembly, decompiledCodeFile);

				Tester.RepeatOnIOError(() => File.Delete(decompiledCodeFile));
				Tester.RepeatOnIOError(() => File.Delete(decompiledOutputFile.PathToAssembly));
			}
			finally
			{
				if (outputFile != null)
					outputFile.TempFiles.Delete();
				if (decompiledOutputFile != null)
					decompiledOutputFile.TempFiles.Delete();
			}
		}

		void RunVB([CallerMemberName] string testName = null, CompilerOptions options = CompilerOptions.UseDebug)
		{
			options |= CompilerOptions.ReferenceVisualBasic;
			string testFileName = testName + ".vb";
			string testOutputFileName = testName + Tester.GetSuffix(options) + ".exe";
			CompilerResults outputFile = null, decompiledOutputFile = null;

			try
			{
				outputFile = Tester.CompileVB(Path.Combine(TestCasePath, testFileName), options,
					outputFileName: Path.Combine(TestCasePath, testOutputFileName));
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile.PathToAssembly, Tester.GetSettings(options));
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, options);

				Tester.RunAndCompareOutput(testFileName, outputFile.PathToAssembly, decompiledOutputFile.PathToAssembly, decompiledCodeFile);

				Tester.RepeatOnIOError(() => File.Delete(decompiledCodeFile));
				Tester.RepeatOnIOError(() => File.Delete(decompiledOutputFile.PathToAssembly));
			}
			finally
			{
				if (outputFile != null)
					outputFile.TempFiles.Delete();
				if (decompiledOutputFile != null)
					decompiledOutputFile.TempFiles.Delete();
			}
		}

		void RunIL(string testFileName, CompilerOptions options = CompilerOptions.UseDebug, AssemblerOptions asmOptions = AssemblerOptions.None)
		{
			string outputFile = null;
			CompilerResults decompiledOutputFile = null;

			try
			{
				outputFile = Tester.AssembleIL(Path.Combine(TestCasePath, testFileName), asmOptions);
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile, Tester.GetSettings(options));
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, options);

				Tester.RunAndCompareOutput(testFileName, outputFile, decompiledOutputFile.PathToAssembly, decompiledCodeFile);

				Tester.RepeatOnIOError(() => File.Delete(decompiledCodeFile));
				Tester.RepeatOnIOError(() => File.Delete(decompiledOutputFile.PathToAssembly));
			}
			finally
			{
				if (decompiledOutputFile != null)
					decompiledOutputFile.TempFiles.Delete();
			}
		}
	}
}
