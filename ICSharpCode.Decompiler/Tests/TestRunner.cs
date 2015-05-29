using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Tests.Helpers;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	public class TestRunner
	{
		const string TestCasePath = @"../../Tests/TestCases";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(TestRunner).GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any())
				.Select(m => m.Name)
				.ToArray();
			foreach (var file in new DirectoryInfo(TestCasePath).EnumerateFiles()) {
				var testName = Path.GetFileNameWithoutExtension(file.Name);
				Assert.Contains(testName, testNames);
			}
		}

		[Test]
		public void HelloWorld()
		{
			TestCompileDecompileCompileOutputAll("HelloWorld.cs");
		}

		[Test]
		public void ControlFlow()
		{
			TestCompileDecompileCompileOutputAll("ControlFlow.cs");
		}

		[Test]
		public void CompoundAssignment()
		{
			TestCompileDecompileCompileOutputAll("CompoundAssignment.cs");
		}

		[Test]
		public void PropertiesAndEvents()
		{
			TestCompileDecompileCompileOutputAll("PropertiesAndEvents.cs");
		}

		void TestCompileDecompileCompileOutputAll(string testFileName)
		{
			TestCompileDecompileCompileOutput(testFileName, CompilerOptions.None);
			TestCompileDecompileCompileOutput(testFileName, CompilerOptions.UseDebug);
			TestCompileDecompileCompileOutput(testFileName, CompilerOptions.Optimize);
			TestCompileDecompileCompileOutput(testFileName, CompilerOptions.UseDebug | CompilerOptions.Optimize);
		}

		void TestCompileDecompileCompileOutput(string testFileName, CompilerOptions options = CompilerOptions.UseDebug)
		{
			CompilerResults outputFile = null, decompiledOutputFile = null;
			string output1, output2, error1, error2;

			try {
				outputFile = Tester.CompileCSharp(Path.Combine(TestCasePath, testFileName), options);
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile.PathToAssembly);
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, options);
				int result1 = Tester.Run(outputFile.PathToAssembly, out output1, out error1);
				int result2 = Tester.Run(decompiledOutputFile.PathToAssembly, out output2, out error2);

				Assert.AreEqual(result1, result2);
				Assert.AreEqual(output1, output2);
				Assert.AreEqual(error1, error2);
			} finally {
				if (outputFile != null)
					outputFile.TempFiles.Delete();
				if (decompiledOutputFile != null)
					decompiledOutputFile.TempFiles.Delete();
			}
		}
	}
}
