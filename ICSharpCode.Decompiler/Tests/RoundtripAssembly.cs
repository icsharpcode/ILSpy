// Copyright (c) 2016 Daniel Grunwald
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.NRefactory.Utils;
using Mono.Cecil;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	public class RoundtripAssembly
	{
		static readonly string testDir = Path.GetFullPath("../../../ILSpy-tests");
		static readonly string msbuild = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "msbuild", "14.0", "bin", "msbuild.exe");
		static readonly string nunit = Path.Combine(testDir, "nunit", "nunit3-console.exe");
		
		[Test]
		public void Cecil_net45()
		{
			RunWithTest("Mono.Cecil-net45", "Mono.Cecil.dll", "Mono.Cecil.Tests.dll");
		}
		
		[Test]
		public void NewtonsoftJson_net40()
		{
			try {
				RunWithTest("Newtonsoft.Json-net40", "Newtonsoft.Json.dll", "Newtonsoft.Json.Tests.dll");
			} catch (CompilationFailedException ex) {
				Assert.Ignore(ex.Message);
			}
		}
		
		[Test]
		public void NRefactory_CSharp()
		{
			try {
				RunWithTest("NRefactory", "ICSharpCode.NRefactory.CSharp.dll", "ICSharpCode.NRefactory.Tests.dll");
			} catch (CompilationFailedException ex) {
				Assert.Ignore(ex.Message);
			}
		}
		
		[Test]
		public void Implicit_Conversions()
		{
			RunWithOutput("Random Tests\\TestCases", "ImplicitConversions.exe");
		}

		[Test]
		public void Implicit_Conversions_32()
		{
			RunWithOutput("Random Tests\\TestCases", "ImplicitConversions_32.exe");
		}

		[Test]
		public void Explicit_Conversions()
		{
			try {
				RunWithOutput("Random Tests\\TestCases", "ExplicitConversions.exe");
			} catch (AssertionException ex) {
				Assert.Ignore(ex.Message);
			}
		}

		[Test]
		public void Explicit_Conversions_32()
		{
			try {
				RunWithOutput("Random Tests\\TestCases", "ExplicitConversions_32.exe");
			} catch (AssertionException ex) {
				Assert.Ignore(ex.Message);
			}
		}

		[Test]
		public void Random_Tests_TestCases()
		{
			try {
				RunWithOutput("Random Tests\\TestCases", "TestCase-1.exe");
			} catch (AssertionException ex) {
				Assert.Ignore(ex.Message);
			}
		}

		void RunWithTest(string dir, string fileToRoundtrip, string fileToTest)
		{
			RunInternal(dir, fileToRoundtrip, () => RunTest(Path.Combine(testDir, dir) + "-output", fileToTest));
		}
		
		void RunWithOutput(string dir, string fileToRoundtrip)
		{
			string inputDir = Path.Combine(testDir, dir);
			string outputDir = inputDir + "-output";
			RunInternal(dir, fileToRoundtrip, () => Tester.RunAndCompareOutput(fileToRoundtrip, Path.Combine(inputDir, fileToRoundtrip), Path.Combine(outputDir, fileToRoundtrip)));
		}
		
		void RunInternal(string dir, string fileToRoundtrip, Action testAction)
		{
			if (!Directory.Exists(testDir)) {
				Assert.Ignore($"Assembly-roundtrip test ignored: test directory '{testDir}' needs to be checked out separately." + Environment.NewLine +
				              $"git clone https://github.com/icsharpcode/ILSpy-tests \"{testDir}\"");
			}
			string inputDir = Path.Combine(testDir, dir);
			//RunTest(inputDir, fileToTest);
			string decompiledDir = inputDir + "-decompiled";
			string outputDir = inputDir + "-output";
			ClearDirectory(decompiledDir);
			ClearDirectory(outputDir);
			string projectFile = null;
			foreach (string file in Directory.EnumerateFiles(inputDir, "*", SearchOption.AllDirectories)) {
				if (!file.StartsWith(inputDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) {
					Assert.Fail($"Unexpected file name: {file}");
				}
				string relFile = file.Substring(inputDir.Length + 1);
				Directory.CreateDirectory(Path.Combine(outputDir, Path.GetDirectoryName(relFile)));
				if (relFile.Equals(fileToRoundtrip, StringComparison.OrdinalIgnoreCase)) {
					Console.WriteLine($"Decompiling {fileToRoundtrip}...");
					Stopwatch w = Stopwatch.StartNew();
					DefaultAssemblyResolver resolver = new DefaultAssemblyResolver();
					resolver.AddSearchDirectory(inputDir);
					var module = ModuleDefinition.ReadModule(file, new ReaderParameters { AssemblyResolver = resolver });
					var decompiler = new TestProjectDecompiler(inputDir);
					// use a fixed GUID so that we can diff the output between different ILSpy runs without spurious changes
					decompiler.ProjectGuid = Guid.Parse("{127C83E4-4587-4CF9-ADCA-799875F3DFE6}");
					decompiler.DecompileProject(module, decompiledDir);
					Console.WriteLine($"Decompiled {fileToRoundtrip} in {w.Elapsed.TotalSeconds:f2}");
					projectFile = Path.Combine(decompiledDir, module.Assembly.Name.Name + ".csproj");
				} else {
					File.Copy(file, Path.Combine(outputDir, relFile));
				}
			}
			Assert.IsNotNull(projectFile, $"Could not find {fileToRoundtrip}");
			
			Compile(projectFile, outputDir);
			testAction();
		}

		static void ClearDirectory(string dir)
		{
			Directory.CreateDirectory(dir);
			foreach (string subdir in Directory.EnumerateDirectories(dir)) {
				Directory.Delete(subdir, true);
			}
			foreach (string file in Directory.EnumerateFiles(dir)) {
				File.Delete(file);
			}
		}
		
		static void Compile(string projectFile, string outputDir)
		{
			var info = new ProcessStartInfo(msbuild);
			info.Arguments = $"/nologo /v:minimal /p:OutputPath=\"{outputDir}\" \"{projectFile}\"";
			info.CreateNoWindow = true;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			Console.WriteLine($"\"{info.FileName}\" {info.Arguments}");
			using (var p = Process.Start(info)) {
				Regex errorRegex = new Regex(@"^[\w\d.\\-]+\(\d+,\d+\):");
				string suffix = $" [{projectFile}]";
				string line;
				while ((line = p.StandardOutput.ReadLine()) != null) {
					if (line.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) {
						line = line.Substring(0, line.Length - suffix.Length);
					}
					Match m = errorRegex.Match(line);
					if (m.Success) {
						// Make path absolute so that it gets hyperlinked
						line = Path.GetDirectoryName(projectFile) + Path.DirectorySeparatorChar + line;
					}
					Console.WriteLine(line);
				}
				p.WaitForExit();
				if (p.ExitCode != 0)
					throw new CompilationFailedException($"Compilation of {Path.GetFileName(projectFile)} failed");
			}
		}
		
		static void RunTest(string outputDir, string fileToTest)
		{
			var info = new ProcessStartInfo(nunit);
			info.WorkingDirectory = outputDir;
			info.Arguments = $"\"{fileToTest}\"";
			info.CreateNoWindow = true;
			info.UseShellExecute = false;
			info.RedirectStandardOutput = true;
			Console.WriteLine($"\"{info.FileName}\" {info.Arguments}");
			using (var p = Process.Start(info)) {
				string line;
				while ((line = p.StandardOutput.ReadLine()) != null) {
					Console.WriteLine(line);
				}
				p.WaitForExit();
				if (p.ExitCode != 0)
					throw new TestRunFailedException($"Test execution of {Path.GetFileName(fileToTest)} failed");
			}
		}

		class TestProjectDecompiler : WholeProjectDecompiler
		{
			readonly string[] localAssemblies;

			public TestProjectDecompiler(string baseDir)
			{
				localAssemblies = new DirectoryInfo(baseDir).EnumerateFiles("*.dll").Select(f => f.FullName).ToArray();
			}

			protected override bool IsGacAssembly(AssemblyNameReference r, AssemblyDefinition asm)
			{
				if (asm == null)
					return false;
				return !localAssemblies.Contains(asm.MainModule.FullyQualifiedName);
			}
		}

		class CompilationFailedException : Exception
		{
			public CompilationFailedException(string message) : base(message)
			{
			}
		}
		
		class TestRunFailedException : Exception
		{
			public TestRunFailedException(string message) : base(message)
			{
			}
		}
	}
}
