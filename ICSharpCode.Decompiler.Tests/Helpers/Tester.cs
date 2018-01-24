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
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.TypeSystem;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CSharp;
using Mono.Cecil;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	[Flags]
	public enum CompilerOptions
	{
		None,
		Optimize = 0x1,
		UseDebug = 0x2,
		Force32Bit = 0x4,
		Library = 0x8,
		UseRoslyn = 0x10,
	}
	
	[Flags]
	public enum AssemblerOptions
	{
		None,
		UseDebug = 0x1,
		Force32Bit = 0x2,
		Library = 0x4,
		UseOwnDisassembler = 0x8,
	}

	public static class Tester
	{
		public static string AssembleIL(string sourceFileName, AssemblerOptions options = AssemblerOptions.UseDebug)
		{
			string ilasmPath = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Microsoft.NET\Framework\v4.0.30319\ilasm.exe");
			string outputFile = Path.Combine(Path.GetDirectoryName(sourceFileName), Path.GetFileNameWithoutExtension(sourceFileName));
			string otherOptions = " ";
			if (options.HasFlag(AssemblerOptions.Force32Bit)) {
				outputFile += ".32";
				otherOptions += "/32BitPreferred ";
			}
			if (options.HasFlag(AssemblerOptions.Library)) {
				outputFile += ".dll";
				otherOptions += "/dll ";
			} else {
				outputFile += ".exe";
				otherOptions += "/exe ";
			}
			
			
			if (options.HasFlag(AssemblerOptions.UseDebug)) {
				otherOptions += "/debug ";
			}
			
			ProcessStartInfo info = new ProcessStartInfo(ilasmPath);
			info.Arguments = $"/nologo {otherOptions}/output=\"{outputFile}\" \"{sourceFileName}\"";
			info.RedirectStandardError = true;
			info.RedirectStandardOutput = true;
			info.UseShellExecute = false;

			Process process = Process.Start(info);

			var outputTask = process.StandardOutput.ReadToEndAsync();
			var errorTask = process.StandardError.ReadToEndAsync();

			Task.WaitAll(outputTask, errorTask);
			process.WaitForExit();

			Console.WriteLine("output: " + outputTask.Result);
			Console.WriteLine("errors: " + errorTask.Result);
			Assert.AreEqual(0, process.ExitCode, "ilasm failed");

			return outputFile;
		}
		
		public static string Disassemble(string sourceFileName, string outputFile, AssemblerOptions asmOptions)
		{
			if (asmOptions.HasFlag(AssemblerOptions.UseOwnDisassembler)) {
				using (ModuleDefinition module = ModuleDefinition.ReadModule(sourceFileName))
				using (var writer = new StreamWriter(outputFile)) {
					module.Name = Path.GetFileNameWithoutExtension(outputFile);
					var output = new PlainTextOutput(writer);
					ReflectionDisassembler rd = new ReflectionDisassembler(output, CancellationToken.None);
					rd.DetectControlStructure = false;
					rd.WriteAssemblyReferences(module);
					if (module.Assembly != null)
						rd.WriteAssemblyHeader(module.Assembly);
					output.WriteLine();
					rd.WriteModuleHeader(module);
					output.WriteLine();
					rd.WriteModuleContents(module);
				}
				return outputFile;
			}

			string ildasmPath = SdkUtility.GetSdkPath("ildasm.exe");
			
			ProcessStartInfo info = new ProcessStartInfo(ildasmPath);
			info.Arguments = $"/out=\"{outputFile}\" \"{sourceFileName}\"";
			info.RedirectStandardError = true;
			info.RedirectStandardOutput = true;
			info.UseShellExecute = false;

			Process process = Process.Start(info);

			var outputTask = process.StandardOutput.ReadToEndAsync();
			var errorTask = process.StandardError.ReadToEndAsync();

			Task.WaitAll(outputTask, errorTask);
			process.WaitForExit();

			Console.WriteLine("output: " + outputTask.Result);
			Console.WriteLine("errors: " + errorTask.Result);

			return outputFile;
		}

		static readonly Lazy<IEnumerable<MetadataReference>> defaultReferences = new Lazy<IEnumerable<MetadataReference>>(delegate {
			string refAsmPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
				@"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5");
			return new[]
			{
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "mscorlib.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "System.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "System.Core.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "System.Xml.dll"))
			};
		});
		

		public static List<string> GetPreprocessorSymbols(CompilerOptions flags)
		{
			var preprocessorSymbols = new List<string>();
			if (flags.HasFlag(CompilerOptions.UseDebug)) {
				preprocessorSymbols.Add("DEBUG");
			}
			if (flags.HasFlag(CompilerOptions.Optimize)) {
				preprocessorSymbols.Add("OPT");
			}
			if (flags.HasFlag(CompilerOptions.UseRoslyn)) {
				preprocessorSymbols.Add("ROSLYN");
			} else {
				preprocessorSymbols.Add("LEGACY_CSC");
			}
			return preprocessorSymbols;
		}

		public static CompilerResults CompileCSharp(string sourceFileName, CompilerOptions flags = CompilerOptions.UseDebug, string outputFileName = null)
		{
			List<string> sourceFileNames = new List<string> { sourceFileName };
			foreach (Match match in Regex.Matches(File.ReadAllText(sourceFileName), @"#include ""([\w\d./]+)""")) {
				sourceFileNames.Add(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFileName), match.Groups[1].Value)));
			}

			var preprocessorSymbols = GetPreprocessorSymbols(flags);

			if (flags.HasFlag(CompilerOptions.UseRoslyn)) {
				var parseOptions = new CSharpParseOptions(preprocessorSymbols: preprocessorSymbols.ToArray(), languageVersion: LanguageVersion.Latest);
				var syntaxTrees = sourceFileNames.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, path: f));
				var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(sourceFileName),
					syntaxTrees, defaultReferences.Value,
					new CSharpCompilationOptions(
					flags.HasFlag(CompilerOptions.Library) ? OutputKind.DynamicallyLinkedLibrary : OutputKind.ConsoleApplication,
					platform: flags.HasFlag(CompilerOptions.Force32Bit) ? Platform.X86 : Platform.AnyCpu,
					optimizationLevel: flags.HasFlag(CompilerOptions.Optimize) ? OptimizationLevel.Release : OptimizationLevel.Debug,
					allowUnsafe: true,
					deterministic: true
				));
				CompilerResults results = new CompilerResults(new TempFileCollection());
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();
				var emitResult = compilation.Emit(results.PathToAssembly);
				if (!emitResult.Success) {
					StringBuilder b = new StringBuilder("Compiler error:");
					foreach (var diag in emitResult.Diagnostics) {
						b.AppendLine(diag.ToString());
					}
					throw new Exception(b.ToString());
				}
				return results;
			} else {
				var provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
				CompilerParameters options = new CompilerParameters();
				options.GenerateExecutable = !flags.HasFlag(CompilerOptions.Library);
				options.CompilerOptions = "/unsafe /o" + (flags.HasFlag(CompilerOptions.Optimize) ? "+" : "-");
				options.CompilerOptions += (flags.HasFlag(CompilerOptions.UseDebug) ? " /debug" : "");
				options.CompilerOptions += (flags.HasFlag(CompilerOptions.Force32Bit) ? " /platform:anycpu32bitpreferred" : "");
				if (preprocessorSymbols.Count > 0) {
					options.CompilerOptions += " /d:" + string.Join(";", preprocessorSymbols);
				}
				if (outputFileName != null) {
					options.OutputAssembly = outputFileName;
				}

				options.ReferencedAssemblies.Add("System.dll");
				options.ReferencedAssemblies.Add("System.Core.dll");
				options.ReferencedAssemblies.Add("System.Xml.dll");
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
		}

		internal static string GetSuffix(CompilerOptions cscOptions)
		{
			string suffix = "";
			if ((cscOptions & CompilerOptions.Optimize) != 0)
				suffix += ".opt";
			if ((cscOptions & CompilerOptions.Force32Bit) != 0)
				suffix += ".32";
			if ((cscOptions & CompilerOptions.UseDebug) != 0)
				suffix += ".dbg";
			if ((cscOptions & CompilerOptions.UseRoslyn) != 0)
				suffix += ".roslyn";
			return suffix;
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

		public static string DecompileCSharp(string assemblyFileName, DecompilerSettings settings = null)
		{
			using (var module = ModuleDefinition.ReadModule(assemblyFileName)) {
				var typeSystem = new DecompilerTypeSystem(module);
				CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, settings ?? new DecompilerSettings());
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
		}
		
		public static void RunAndCompareOutput(string testFileName, string outputFile, string decompiledOutputFile, string decompiledCodeFile = null)
		{
			string output1, output2, error1, error2;
			int result1 = Tester.Run(outputFile, out output1, out error1);
			int result2 = Tester.Run(decompiledOutputFile, out output2, out error2);
			
			Assert.AreEqual(0, result1, "Exit code != 0; did the test case crash?" + Environment.NewLine + error1);
			Assert.AreEqual(0, result2, "Exit code != 0; did the decompiled code crash?" + Environment.NewLine + error2);
			
			if (output1 != output2 || error1 != error2) {
				StringBuilder b = new StringBuilder();
				b.AppendLine($"Test {testFileName} failed: output does not match.");
				if (decompiledCodeFile != null) {
					b.AppendLine($"Decompiled code in {decompiledCodeFile}:line 1");
				}
				if (error1 != error2) {
					b.AppendLine("Got different error output.");
					b.AppendLine("Original error:");
					b.AppendLine(error1);
					b.AppendLine();
					b.AppendLine("Error after de+re-compiling:");
					b.AppendLine(error2);
					b.AppendLine();
				}
				if (output1 != output2) {
					string outputFileName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(testFileName));
					File.WriteAllText(outputFileName + ".original.out", output1);
					File.WriteAllText(outputFileName + ".decompiled.out", output2);
					int diffLine = 0;
					string lastHeader = null;
					Tuple<string, string> errorItem = null;
					foreach (var pair in output1.Replace("\r", "").Split('\n').Zip(output2.Replace("\r", "").Split('\n'), Tuple.Create)) {
						diffLine++;
						if (pair.Item1 != pair.Item2) {
							errorItem = pair;
							break;
						}
						if (pair.Item1.EndsWith(":", StringComparison.Ordinal)) {
							lastHeader = pair.Item1;
						}
					}
					b.AppendLine($"Output differs; first difference in line {diffLine}");
					if (lastHeader != null) {
						b.AppendLine(lastHeader);
					}
					b.AppendLine($"{outputFileName}.original.out:line {diffLine}");
					b.AppendLine(errorItem.Item1);
					b.AppendLine($"{outputFileName}.decompiled.out:line {diffLine}");
					b.AppendLine(errorItem.Item2);
				}
				Assert.Fail(b.ToString());
			}
		}
	}
}
