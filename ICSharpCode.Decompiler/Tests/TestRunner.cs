using System;
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
		const string TestCasePath = @"..\..\Tests\TestCases";

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

		void TestCompileDecompileCompileOutputAll(string testFileName)
		{
			TestCompileDecompileCompileOutput(testFileName, optimize: false, useDebug: true);
			TestCompileDecompileCompileOutput(testFileName, optimize: false, useDebug: false);
			TestCompileDecompileCompileOutput(testFileName, optimize: true, useDebug: true);
			TestCompileDecompileCompileOutput(testFileName, optimize: true, useDebug: false);
		}

		void TestCompileDecompileCompileOutput(string testFileName, bool optimize = false, bool useDebug = true)
		{
			string outputFile = null, decompiledOutputFile = null;
			string output1, output2, error1, error2;

			try {
				outputFile = Tester.CompileCSharp(Path.Combine(TestCasePath, "HelloWorld.cs"), optimize, useDebug);
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile);
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, optimize, useDebug);
				int result1 = Tester.Run(outputFile, out output1, out error1);
				int result2 = Tester.Run(decompiledOutputFile, out output2, out error2);

				Assert.AreEqual(result1, result2);
				Assert.AreEqual(output1, output2);
				Assert.AreEqual(error1, error2);
			} finally {
				if (outputFile != null)
					File.Delete(outputFile);
				if (decompiledOutputFile != null)
					File.Delete(decompiledOutputFile);
			}
		}
	}
}
