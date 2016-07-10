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
				if (file.Extension == ".txt" || file.Extension == ".exe")
					continue;
				var testName = Path.GetFileNameWithoutExtension(file.Name);
				Assert.Contains(testName, testNames);
			}
		}
		
		[Test]
		public void HelloWorld()
		{
			Run("HelloWorld");
			Run("HelloWorld", AssemblerOptions.UseDebug);
		}
		
		void Run(string testName, AssemblerOptions asmOptions = AssemblerOptions.None)
		{
			var ilFile = Path.Combine(TestCasePath, testName + ".il");
			var csFile = Path.Combine(TestCasePath, testName + ".cs");
			EnsureSourceFilesExist(Path.Combine(TestCasePath, testName));
			
			var executable = Tester.AssembleIL(ilFile, asmOptions);
			var decompiled = Tester.DecompileCSharp(executable);
			
			CodeAssert.FilesAreEqual(csFile, decompiled);
		}
		
		void EnsureSourceFilesExist(string fileName)
		{
			if (!File.Exists(fileName + ".il")) {
				CompilerResults output = null;
				try {
					output = Tester.CompileCSharp(fileName + ".cs", CompilerOptions.None);
					Tester.Disassemble(output.PathToAssembly, fileName + ".il");
				} finally {
					if (output != null)
						output.TempFiles.Delete();
				}
			}
		}
	}
}
