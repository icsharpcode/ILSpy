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
				if (file.Extension == ".txt")
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
		public void ILTest()
		{
			TestAssembleDecompileCompileOutput("ILTest.il");
		}

		[Test, Ignore("Fixed statements and pointer arithmetic are broken")]
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
		
		void TestAssembleDecompileCompileOutput(string testFileName, CompilerOptions options = CompilerOptions.UseDebug)
		{
			string outputFile = null;
			CompilerResults decompiledOutputFile = null;

			try {
				outputFile = Tester.AssembleIL(Path.Combine(TestCasePath, testFileName));
				string decompiledCodeFile = Tester.DecompileCSharp(outputFile);
				decompiledOutputFile = Tester.CompileCSharp(decompiledCodeFile, options);
				
				Tester.RunAndCompareOutput(testFileName, outputFile, decompiledOutputFile.PathToAssembly, decompiledCodeFile);
				
				File.Delete(decompiledCodeFile);
				File.Delete(outputFile);
				File.Delete(decompiledOutputFile.PathToAssembly);
			} finally {
				if (decompiledOutputFile != null)
					decompiledOutputFile.TempFiles.Delete();
			}
		}
	}
}
