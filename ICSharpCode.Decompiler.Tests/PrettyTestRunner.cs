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
	public class PrettyTestRunner
	{
		const string TestCasePath = DecompilerTestBase.TestCasePath + "/Pretty";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(PrettyTestRunner).GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
				.Select(m => m.Name)
				.ToArray();
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles()) {
				if (file.Extension.Equals(".il", StringComparison.OrdinalIgnoreCase)
					|| file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase)) {
					var testName = file.Name.Split('.')[0];
					Assert.Contains(testName, testNames);
				}
			}
		}

		static readonly CompilerOptions[] noRoslynOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize
		};

		static readonly CompilerOptions[] roslynOnlyOptions =
		{
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn
		};

		static readonly CompilerOptions[] defaultOptions =
		{
			CompilerOptions.None,
			CompilerOptions.Optimize,
			CompilerOptions.UseRoslyn,
			CompilerOptions.Optimize | CompilerOptions.UseRoslyn
		};

		[Test]
		public void HelloWorld()
		{
			Run();
			Run(asmOptions: AssemblerOptions.UseDebug);
		}
		
		[Test]
		public void InlineAssignmentTest([ValueSource("noRoslynOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void CompoundAssignmentTest([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void ShortCircuit([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void ExceptionHandling([Values(CompilerOptions.None, CompilerOptions.Optimize)] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void AnonymousTypes([Values(CompilerOptions.None, CompilerOptions.Optimize)] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void Async([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test, Ignore("Not implemented")]
		public void LiftedOperators([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test, Ignore("Bad variable names; some if-else misdetected")]
		public void Loops([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void PropertiesAndEvents([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void AutoProperties([ValueSource("roslynOnlyOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void QueryExpressions([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void TypeAnalysisTests([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void CheckedUnchecked([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			if (cscOptions.HasFlag(CompilerOptions.UseRoslyn) && cscOptions.HasFlag(CompilerOptions.Optimize)) {
				Assert.Ignore("Roslyn opt replaces locals with stack slots, resulting in S_* variable names.");
			}
			Run(cscOptions: cscOptions);
		}

		[Test]
		public void PInvoke([ValueSource("defaultOptions")] CompilerOptions cscOptions)
		{
			// This tests needs our own disassembler; ildasm has a bug with marshalinfo.
			Run(cscOptions: cscOptions, asmOptions: AssemblerOptions.UseOwnDisassembler);
		}

		void Run([CallerMemberName] string testName = null, AssemblerOptions asmOptions = AssemblerOptions.None, CompilerOptions cscOptions = CompilerOptions.None)
		{
			var ilFile = Path.Combine(TestCasePath, testName);
			if ((cscOptions & CompilerOptions.Optimize) != 0)
				ilFile += ".opt";
			if ((cscOptions & CompilerOptions.Force32Bit) != 0)
				ilFile += ".32";
			if ((cscOptions & CompilerOptions.UseDebug) != 0)
				ilFile += ".dbg";
			if ((cscOptions & CompilerOptions.UseRoslyn) != 0)
				ilFile += ".roslyn";
			ilFile += ".il";
			var csFile = Path.Combine(TestCasePath, testName + ".cs");

			if (!File.Exists(ilFile)) {
				// re-create .il file if necessary
				CompilerResults output = null;
				try {
					output = Tester.CompileCSharp(csFile, cscOptions | CompilerOptions.Library);
					Tester.Disassemble(output.PathToAssembly, ilFile, asmOptions);
				} finally {
					if (output != null)
						output.TempFiles.Delete();
				}
			}

			var executable = Tester.AssembleIL(ilFile, asmOptions | AssemblerOptions.Library);
			var decompiled = Tester.DecompileCSharp(executable);
			
			CodeAssert.FilesAreEqual(csFile, decompiled);
		}
	}
}
