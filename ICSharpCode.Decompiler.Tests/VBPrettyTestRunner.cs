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
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Tests.Helpers;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class VBPrettyTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/VBPretty";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(VBPrettyTestRunner).GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
				.Select(m => m.Name)
				.ToArray();
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles())
			{
				if (file.Extension.Equals(".vb", StringComparison.OrdinalIgnoreCase))
				{
					var testName = file.Name.Split('.')[0];
					Assert.Contains(testName, testNames);
					Assert.IsTrue(File.Exists(Path.Combine(TestCasePath, testName + ".cs")));
				}
			}
		}

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
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
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
			CompilerOptions.UseRoslynLatest,
			CompilerOptions.Optimize | CompilerOptions.UseRoslynLatest,
		};

		[Test]
		public async Task Async([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await Run(options: options | CompilerOptions.Library);
		}

		[Test] // TODO: legacy VB compound assign
		public async Task VBCompoundAssign([ValueSource(nameof(roslynOnlyOptions))] CompilerOptions options)
		{
			await Run(options: options | CompilerOptions.Library);
		}

		[Test]
		public async Task Select([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await Run(options: options | CompilerOptions.Library);
		}

		[Test]
		public async Task Issue1906([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await Run(options: options | CompilerOptions.Library);
		}

		[Test]
		public async Task Issue2192([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await Run(options: options | CompilerOptions.Library);
		}

		[Test]
		public async Task VBPropertiesTest([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await Run(options: options | CompilerOptions.Library);
		}

		[Test]
		public async Task VBAutomaticEvents([ValueSource(nameof(defaultOptions))] CompilerOptions options)
		{
			await Run(options: options | CompilerOptions.Library);
		}

		async Task Run([CallerMemberName] string testName = null, CompilerOptions options = CompilerOptions.UseDebug, DecompilerSettings settings = null)
		{
			var vbFile = Path.Combine(TestCasePath, testName + ".vb");
			var csFile = Path.Combine(TestCasePath, testName + ".cs");
			var exeFile = Path.Combine(TestCasePath, testName) + Tester.GetSuffix(options) + ".exe";
			if (options.HasFlag(CompilerOptions.Library))
			{
				exeFile = Path.ChangeExtension(exeFile, ".dll");
			}

			var executable = await Tester.CompileVB(vbFile, options | CompilerOptions.ReferenceVisualBasic, exeFile).ConfigureAwait(false);
			var decompiled = await Tester.DecompileCSharp(executable.PathToAssembly, settings ?? new DecompilerSettings { FileScopedNamespaces = false }).ConfigureAwait(false);

			CodeAssert.FilesAreEqual(csFile, decompiled, Tester.GetPreprocessorSymbols(options).ToArray());
			Tester.RepeatOnIOError(() => File.Delete(decompiled));
		}
	}
}
