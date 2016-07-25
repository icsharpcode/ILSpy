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
using ICSharpCode.Decompiler.Tests.Helpers;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	public class CorrectnessTestRunner
	{
		const string TestCasePath = @"../../Tests/TestCases/Correctness";

		[Test]
		public void AllFilesHaveTests()
		{
			var testNames = typeof(CorrectnessTestRunner).GetMethods()
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
		public void Comparisons()
		{
			TestCompileDecompileCompileOutputAll("Comparisons.cs");
		}

		[Test]
		public void Conversions()
		{
			TestCompileDecompileCompileOutputAll("Conversions.cs");
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

		[Test]
		public void Switch()
		{
			TestCompileDecompileCompileOutputAll("Switch.cs");
		}

		[Test]
		public void Generics()
		{
			TestCompileDecompileCompileOutputAll("Generics.cs");
		}

		[Test]
		public void ValueTypeCall()
		{
			TestCompileDecompileCompileOutputAll("ValueTypeCall.cs");
		}
		
		[Test]
		public void InitializerTests()
		{
			TestCompileDecompileCompileOutputAll("InitializerTests.cs");
		}
		
		[Test]
		public void DecimalFields()
		{
			TestCompileDecompileCompileOutputAll("DecimalFields.cs");
		}
		
		[Test]
		public void UndocumentedExpressions()
		{
			TestCompileDecompileCompileOutputAll("UndocumentedExpressions.cs");
		}
		
		[Test]
		public void MemberLookup()
		{
			TestCompileDecompileCompileOutputAll("MemberLookup.cs");
		}

		[Test]
		public void BitNot()
		{
			TestAssembleDecompileCompileOutput("BitNot.il");
			TestAssembleDecompileCompileOutput("BitNot.il", CompilerOptions.UseDebug | CompilerOptions.Force32Bit, AssemblerOptions.Force32Bit);
		}

		[Test, Ignore("Pointer reference expression is not supported")]
		public void UnsafeCode()
		{
			TestCompileDecompileCompileOutputAll("UnsafeCode.cs");
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

			try {
				outputFile = Tester.CompileCSharp(Path.Combine(TestCasePath, testFileName), options);
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile.PathToAssembly);
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, options);
				
				Tester.RunAndCompareOutput(testFileName, outputFile.PathToAssembly, decompiledOutputFile.PathToAssembly, decompiledCodeFile);
				
				File.Delete(decompiledCodeFile);
				File.Delete(outputFile.PathToAssembly);
				File.Delete(decompiledOutputFile.PathToAssembly);
			} finally {
				if (outputFile != null)
					outputFile.TempFiles.Delete();
				if (decompiledOutputFile != null)
					decompiledOutputFile.TempFiles.Delete();
			}
		}
		
		void TestAssembleDecompileCompileOutput(string testFileName, CompilerOptions options = CompilerOptions.UseDebug, AssemblerOptions asmOptions = AssemblerOptions.None)
		{
			string outputFile = null;
			CompilerResults decompiledOutputFile = null;

			try {
				outputFile = Tester.AssembleIL(Path.Combine(TestCasePath, testFileName), asmOptions);
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile);
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, options);
				
				Tester.RunAndCompareOutput(testFileName, outputFile, decompiledOutputFile.PathToAssembly, decompiledCodeFile);
				
				File.Delete(decompiledCodeFile);
				File.Delete(decompiledOutputFile.PathToAssembly);
			} finally {
				if (decompiledOutputFile != null)
					decompiledOutputFile.TempFiles.Delete();
			}
		}
	}
}
