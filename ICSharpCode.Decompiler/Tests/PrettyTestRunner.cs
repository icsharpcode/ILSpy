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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Tests.Helpers;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	public class PrettyTestRunner
	{
		const string TestCasePath = @"../../Tests/TestCases/Pretty";

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
					Tester.Disassemble(output.PathToAssembly, ilFile);
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
