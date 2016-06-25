// Copyright (c) 2015 Daniel Grunwald
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.NRefactory.CSharp;
using Microsoft.CSharp;
using Mono.Cecil;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	[Flags]
	public enum CompilerOptions
	{
		None,
		Optimize,
		UseDebug
	}

	public static class Tester
	{
		public static CompilerResults CompileCSharp(string sourceFileName, CompilerOptions flags = CompilerOptions.UseDebug)
		{
			List<string> sourceFileNames = new List<string> { sourceFileName };
			foreach (Match match in Regex.Matches(File.ReadAllText(sourceFileName), @"#include ""([\w\d./]+)""")) {
				sourceFileNames.Add(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFileName), match.Groups[1].Value)));
			}
			
			CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
			CompilerParameters options = new CompilerParameters();
			options.GenerateExecutable = true;
			options.CompilerOptions = "/unsafe /o" + (flags.HasFlag(CompilerOptions.Optimize) ? "+" : "-") + (flags.HasFlag(CompilerOptions.UseDebug) ? " /debug" : "");
			options.ReferencedAssemblies.Add("System.Core.dll");
			CompilerResults results = provider.CompileAssemblyFromFile(options, sourceFileNames.ToArray());
			if (results.Errors.Cast<CompilerError>().Any(e => !e.IsWarning)) {
				StringBuilder b = new StringBuilder("Compiler error:");
				foreach (var error in results.Errors) {
					b.AppendLine(error.ToString());
				}
				throw new Exception(b.ToString());
			}
			return results;
		}

		public static int Run(string assemblyFileName, out string output, out string error)
		{
			ProcessStartInfo info = new ProcessStartInfo(assemblyFileName);
			info.RedirectStandardError = true;
			info.RedirectStandardOutput = true;
			info.UseShellExecute = false;

			Process process = Process.Start(info);

			var outputTask = process.StandardOutput.ReadToEndAsync();
			var errorTask = process.StandardError.ReadToEndAsync();

			Task.WaitAll(outputTask, errorTask);
			process.WaitForExit();

			output = outputTask.Result;
			error = errorTask.Result;

			return process.ExitCode;
		}

		public static string DecompileCSharp(string assemblyFileName)
		{
			var typeSystem = new DecompilerTypeSystem(ModuleDefinition.ReadModule(assemblyFileName));
			CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, new DecompilerSettings());
			decompiler.AstTransforms.Insert(0, new RemoveCompilerAttribute());
			decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
			var syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();
			
			StringWriter output = new StringWriter();
			var visitor = new CSharpOutputVisitor(output, FormattingOptionsFactory.CreateSharpDevelop());
			syntaxTree.AcceptVisitor(visitor);

			string fileName = Path.GetTempFileName();
			File.WriteAllText(fileName, output.ToString());

			return fileName;
		}
		
		public static void RunAndCompareOutput(string testFileName, string outputFile, string decompiledOutputFile, string decompiledCodeFile = null)
		{
			string output1, output2, error1, error2;
			int result1 = Tester.Run(outputFile, out output1, out error1);
			int result2 = Tester.Run(decompiledOutputFile, out output2, out error2);
			
			if (result1 != result2 || output1 != output2 || error1 != error2) {
				Console.WriteLine("Test {0} failed.", testFileName);
				if (decompiledCodeFile != null)
					Console.WriteLine("Decompiled code in {0}:line 1", decompiledCodeFile);
				if (error1 == "" && error2 != "") {
					Console.WriteLine(error2);
				} else {
					string outputFileName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(testFileName));
					File.WriteAllText(outputFileName + ".original.out", output1);
					File.WriteAllText(outputFileName + ".decompiled.out", output2);
					int diffLine = 0;
					foreach (var pair in output1.Split('\n').Zip(output2.Split('\n'), Tuple.Create)) {
						diffLine++;
						if (pair.Item1 != pair.Item2) {
							break;
						}
					}
					Console.WriteLine("Output: {0}.original.out:line {1}", outputFileName, diffLine);
					Console.WriteLine("Output: {0}.decompiled.out:line {1}", outputFileName, diffLine);
				}
			}
			Assert.AreEqual(0, result1, "Exit code != 0; did the test case crash?" + Environment.NewLine + error1);
			Assert.AreEqual(0, result2, "Exit code != 0; did the decompiled code crash?" + Environment.NewLine + error2);
			Assert.AreEqual(error1, error2);
			Assert.AreEqual(output1, output2);
		}
	}
}
