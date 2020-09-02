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
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp;
using Microsoft.DiaSymReader.Tools;

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
		UseMcs = 0x20,
		ReferenceVisualBasic = 0x40,
		ReferenceCore = 0x80,
		GeneratePdb = 0x100,
		Preview = 0x200
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

	public static partial class Tester
	{
		public static readonly string TestCasePath = Path.Combine(
	Path.GetDirectoryName(typeof(Tester).Assembly.Location),
	"../../../TestCases");

		public static string AssembleIL(string sourceFileName, AssemblerOptions options = AssemblerOptions.UseDebug)
		{
			string ilasmPath = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Microsoft.NET\Framework\v4.0.30319\ilasm.exe");
			string outputFile = Path.Combine(Path.GetDirectoryName(sourceFileName), Path.GetFileNameWithoutExtension(sourceFileName));
			string otherOptions = " ";
			if (options.HasFlag(AssemblerOptions.Force32Bit))
			{
				outputFile += ".32";
				otherOptions += "/32BitPreferred ";
			}
			if (options.HasFlag(AssemblerOptions.Library))
			{
				outputFile += ".dll";
				otherOptions += "/dll ";
			}
			else
			{
				outputFile += ".exe";
				otherOptions += "/exe ";
			}


			if (options.HasFlag(AssemblerOptions.UseDebug))
			{
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
			if (asmOptions.HasFlag(AssemblerOptions.UseOwnDisassembler))
			{
				using (var peFileStream = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read))
				using (var peFile = new PEFile(sourceFileName, peFileStream))
				using (var writer = new StringWriter())
				{
					var metadata = peFile.Metadata;
					var output = new PlainTextOutput(writer);
					ReflectionDisassembler rd = new ReflectionDisassembler(output, CancellationToken.None);
					rd.AssemblyResolver = new UniversalAssemblyResolver(sourceFileName, true, null);
					rd.DetectControlStructure = false;
					rd.WriteAssemblyReferences(metadata);
					if (metadata.IsAssembly)
						rd.WriteAssemblyHeader(peFile);
					output.WriteLine();
					rd.WriteModuleHeader(peFile, skipMVID: true);
					output.WriteLine();
					rd.WriteModuleContents(peFile);

					File.WriteAllText(outputFile, ReplacePrivImplDetails(writer.ToString()));
				}
				return outputFile;
			}

			string ildasmPath = SdkUtility.GetSdkPath("ildasm.exe");

			ProcessStartInfo info = new ProcessStartInfo(ildasmPath);
			info.Arguments = $"/nobar /utf8 /out=\"{outputFile}\" \"{sourceFileName}\"";
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

			// Unlike the .imagebase directive (which is a fixed value when compiling with /deterministic),
			// the image base comment still varies... ildasm putting a random number here?
			string il = File.ReadAllText(outputFile);
			il = Regex.Replace(il, @"^// Image base: 0x[0-9A-F]+\r?\n", "", RegexOptions.Multiline);
			// and while we're at it, also remove the MVID
			il = Regex.Replace(il, @"^// MVID: \{[0-9A-F-]+\}\r?\n", "", RegexOptions.Multiline);
			// and the ildasm version info (varies from system to system)
			il = Regex.Replace(il, @"^// +Microsoft .* Disassembler\. +Version.*\r?\n", "", RegexOptions.Multiline);
			// copyright header "All rights reserved" is dependent on system language
			il = Regex.Replace(il, @"^// +Copyright .* Microsoft.*\r?\n", "", RegexOptions.Multiline);
			// filename may contain full path
			il = Regex.Replace(il, @"^// WARNING: Created Win32 resource file.*\r?\n", "", RegexOptions.Multiline);
			il = ReplacePrivImplDetails(il);
			File.WriteAllText(outputFile, il);

			return outputFile;
		}

		private static string ReplacePrivImplDetails(string il)
		{
			return Regex.Replace(il, @"'<PrivateImplementationDetails>\{[0-9A-F-]+\}'", "'<PrivateImplementationDetails>'");
		}

		static readonly string coreRefAsmPath = new DotNetCorePathFinder(TargetFrameworkIdentifier.NETCoreApp, new Version(3, 1)).GetReferenceAssemblyPath(".NETCoreApp, Version = v3.1");

		static readonly string refAsmPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
			@"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2");
		static readonly string thisAsmPath = Path.GetDirectoryName(typeof(Tester).Assembly.Location);

		static readonly Lazy<IEnumerable<MetadataReference>> defaultReferences = new Lazy<IEnumerable<MetadataReference>>(delegate {
			return new[]
			{
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "Facades\\netstandard.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "mscorlib.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "System.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "System.Core.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, @"Facades\System.Runtime.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "System.Xml.dll")),
					MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "Microsoft.CSharp.dll")),
					MetadataReference.CreateFromFile(typeof(ValueTuple).Assembly.Location),
					MetadataReference.CreateFromFile(typeof(ValueTask).Assembly.Location),
					MetadataReference.CreateFromFile(typeof(Span<>).Assembly.Location),
			};
		});

		static readonly Lazy<IEnumerable<MetadataReference>> coreDefaultReferences = new Lazy<IEnumerable<MetadataReference>>(GetDefaultReferences);

		const string targetFrameworkAttributeSnippet = @"

[assembly: System.Runtime.Versioning.TargetFramework("".NETCoreApp, Version = v3.1"", FrameworkDisplayName = """")]

";

		static IEnumerable<MetadataReference> GetDefaultReferences()
		{
			foreach (var reference in Directory.EnumerateFiles(coreRefAsmPath, "*.dll"))
			{
				yield return MetadataReference.CreateFromFile(reference);
			}
		}

		static readonly Lazy<IEnumerable<MetadataReference>> visualBasic = new Lazy<IEnumerable<MetadataReference>>(delegate {
			return new[] {
				MetadataReference.CreateFromFile(Path.Combine(refAsmPath, "Microsoft.VisualBasic.dll"))
			};
		});

		public static List<string> GetPreprocessorSymbols(CompilerOptions flags)
		{
			var preprocessorSymbols = new List<string>();
			if (flags.HasFlag(CompilerOptions.UseDebug))
			{
				preprocessorSymbols.Add("DEBUG");
			}
			if (flags.HasFlag(CompilerOptions.Optimize))
			{
				preprocessorSymbols.Add("OPT");
			}
			if (flags.HasFlag(CompilerOptions.ReferenceCore))
			{
				preprocessorSymbols.Add("NETCORE");
			}
			if (flags.HasFlag(CompilerOptions.UseRoslyn))
			{
				preprocessorSymbols.Add("ROSLYN");
				preprocessorSymbols.Add("CS60");
				preprocessorSymbols.Add("CS70");
				preprocessorSymbols.Add("CS71");
				preprocessorSymbols.Add("CS72");
				preprocessorSymbols.Add("CS73");
				preprocessorSymbols.Add("CS80");
				preprocessorSymbols.Add("VB11");
				preprocessorSymbols.Add("VB14");
				preprocessorSymbols.Add("VB15");

				if (flags.HasFlag(CompilerOptions.Preview))
				{
					preprocessorSymbols.Add("CS90");
				}
			}
			else if (flags.HasFlag(CompilerOptions.UseMcs))
			{
				preprocessorSymbols.Add("MCS");
			}
			else
			{
				preprocessorSymbols.Add("LEGACY_CSC");
				preprocessorSymbols.Add("LEGACY_VBC");
			}
			return preprocessorSymbols;
		}

		public static CompilerResults CompileCSharp(string sourceFileName, CompilerOptions flags = CompilerOptions.UseDebug, string outputFileName = null)
		{
			List<string> sourceFileNames = new List<string> { sourceFileName };
			foreach (Match match in Regex.Matches(File.ReadAllText(sourceFileName), @"#include ""([\w\d./]+)"""))
			{
				sourceFileNames.Add(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFileName), match.Groups[1].Value)));
			}

			var preprocessorSymbols = GetPreprocessorSymbols(flags);

			if (flags.HasFlag(CompilerOptions.UseRoslyn))
			{
				var parseOptions = new CSharpParseOptions(
					preprocessorSymbols: preprocessorSymbols.ToArray(),
					languageVersion: flags.HasFlag(CompilerOptions.Preview) ? Microsoft.CodeAnalysis.CSharp.LanguageVersion.Preview : Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8
				);
				var syntaxTrees = sourceFileNames.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, path: f, encoding: Encoding.UTF8));
				if (flags.HasFlag(CompilerOptions.ReferenceCore))
				{
					syntaxTrees = syntaxTrees.Concat(new[] { SyntaxFactory.ParseSyntaxTree(targetFrameworkAttributeSnippet) });
				}
				IEnumerable<MetadataReference> references;
				if (flags.HasFlag(CompilerOptions.ReferenceCore))
				{
					references = coreDefaultReferences.Value;
				}
				else
				{
					references = defaultReferences.Value;
				}
				if (flags.HasFlag(CompilerOptions.ReferenceVisualBasic))
				{
					references = references.Concat(visualBasic.Value);
				}
				var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(sourceFileName),
					syntaxTrees, references,
					new CSharpCompilationOptions(
						flags.HasFlag(CompilerOptions.Library) ? OutputKind.DynamicallyLinkedLibrary : OutputKind.ConsoleApplication,
						platform: flags.HasFlag(CompilerOptions.Force32Bit) ? Platform.X86 : Platform.AnyCpu,
						optimizationLevel: flags.HasFlag(CompilerOptions.Optimize) ? OptimizationLevel.Release : OptimizationLevel.Debug,
						allowUnsafe: true,
						deterministic: true
					));
				CompilerResults results = new CompilerResults(new TempFileCollection());
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();
				string pdbName = null;
				if (flags.HasFlag(CompilerOptions.GeneratePdb))
					pdbName = Path.ChangeExtension(outputFileName, ".pdb");
				var emitResult = compilation.Emit(results.PathToAssembly, pdbName);
				if (!emitResult.Success)
				{
					StringBuilder b = new StringBuilder("Compiler error:");
					foreach (var diag in emitResult.Diagnostics)
					{
						b.AppendLine(diag.ToString());
					}
					throw new Exception(b.ToString());
				}
				return results;
			}
			else if (flags.HasFlag(CompilerOptions.UseMcs))
			{
				CompilerResults results = new CompilerResults(new TempFileCollection());
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();
				string testBasePath = RoundtripAssembly.TestDir;
				if (!Directory.Exists(testBasePath))
				{
					Assert.Ignore($"Compilation with mcs ignored: test directory '{testBasePath}' needs to be checked out separately." + Environment.NewLine +
			  $"git clone https://github.com/icsharpcode/ILSpy-tests \"{testBasePath}\"");
				}
				string mcsPath = Path.Combine(testBasePath, @"mcs\2.6.4\bin\gmcs.bat");
				string otherOptions = " -unsafe -o" + (flags.HasFlag(CompilerOptions.Optimize) ? "+ " : "- ");

				if (flags.HasFlag(CompilerOptions.Library))
				{
					otherOptions += "-t:library ";
				}
				else
				{
					otherOptions += "-t:exe ";
				}

				if (flags.HasFlag(CompilerOptions.UseDebug))
				{
					otherOptions += "-g ";
				}

				if (flags.HasFlag(CompilerOptions.Force32Bit))
				{
					otherOptions += "-platform:x86 ";
				}
				else
				{
					otherOptions += "-platform:anycpu ";
				}
				if (preprocessorSymbols.Count > 0)
				{
					otherOptions += " \"-d:" + string.Join(";", preprocessorSymbols) + "\" ";
				}

				ProcessStartInfo info = new ProcessStartInfo(mcsPath);
				info.Arguments = $"{otherOptions}-out:\"{Path.GetFullPath(results.PathToAssembly)}\" {string.Join(" ", sourceFileNames.Select(fn => '"' + Path.GetFullPath(fn) + '"'))}";
				info.RedirectStandardError = true;
				info.RedirectStandardOutput = true;
				info.UseShellExecute = false;

				Console.WriteLine($"\"{info.FileName}\" {info.Arguments}");

				Process process = Process.Start(info);

				var outputTask = process.StandardOutput.ReadToEndAsync();
				var errorTask = process.StandardError.ReadToEndAsync();

				Task.WaitAll(outputTask, errorTask);
				process.WaitForExit();

				Console.WriteLine("output: " + outputTask.Result);
				Console.WriteLine("errors: " + errorTask.Result);
				Assert.AreEqual(0, process.ExitCode, "mcs failed");
				return results;
			}
			else
			{
				var provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
				CompilerParameters options = new CompilerParameters();
				options.GenerateExecutable = !flags.HasFlag(CompilerOptions.Library);
				options.CompilerOptions = "/unsafe /o" + (flags.HasFlag(CompilerOptions.Optimize) ? "+" : "-");
				string debugOption = " /debug";
				if (flags.HasFlag(CompilerOptions.GeneratePdb))
				{
					debugOption += ":full";
					options.IncludeDebugInformation = true;
				}
				options.CompilerOptions += (flags.HasFlag(CompilerOptions.UseDebug) ? debugOption : "");
				options.CompilerOptions += (flags.HasFlag(CompilerOptions.Force32Bit) ? " /platform:anycpu32bitpreferred" : "");
				if (preprocessorSymbols.Count > 0)
				{
					options.CompilerOptions += " /d:" + string.Join(";", preprocessorSymbols);
				}
				if (outputFileName != null)
				{
					options.OutputAssembly = outputFileName;
				}
				options.ReferencedAssemblies.Add("System.dll");
				options.ReferencedAssemblies.Add("System.Core.dll");
				options.ReferencedAssemblies.Add("System.Xml.dll");
				options.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
				if (flags.HasFlag(CompilerOptions.ReferenceVisualBasic))
				{
					options.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
				}
				CompilerResults results = provider.CompileAssemblyFromFile(options, sourceFileNames.ToArray());
				if (results.Errors.Cast<CompilerError>().Any(e => !e.IsWarning))
				{
					StringBuilder b = new StringBuilder("Compiler error:");
					foreach (var error in results.Errors)
					{
						b.AppendLine(error.ToString());
					}
					throw new Exception(b.ToString());
				}
				return results;
			}
		}

		internal static DecompilerSettings GetSettings(CompilerOptions cscOptions)
		{
			if (cscOptions.HasFlag(CompilerOptions.UseRoslyn))
			{
				if (cscOptions.HasFlag(CompilerOptions.Preview))
				{
					return new DecompilerSettings(CSharp.LanguageVersion.Latest);
				}
				else
				{
					return new DecompilerSettings(CSharp.LanguageVersion.CSharp8_0);
				}
			}
			else
			{
				var settings= new DecompilerSettings(CSharp.LanguageVersion.CSharp5);
				if (cscOptions.HasFlag(CompilerOptions.UseMcs))
				{
					// we don't recompile with mcs but with roslyn, so we can use ref locals
					settings.UseRefLocalsForAccurateOrderOfEvaluation = true;
				}
				return settings;
			}
		}

		public static void CompileCSharpWithPdb(string assemblyName, Dictionary<string, string> sourceFiles)
		{
			var parseOptions = new CSharpParseOptions(languageVersion: Microsoft.CodeAnalysis.CSharp.LanguageVersion.Latest);

			List<EmbeddedText> embeddedTexts = new List<EmbeddedText>();
			List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();

			foreach (KeyValuePair<string, string> file in sourceFiles)
			{
				var sourceText = SourceText.From(file.Value, new UTF8Encoding(false), SourceHashAlgorithm.Sha256);
				syntaxTrees.Add(SyntaxFactory.ParseSyntaxTree(sourceText, parseOptions, file.Key));
				embeddedTexts.Add(EmbeddedText.FromSource(file.Key, sourceText));
			}

			var compilation = CSharpCompilation.Create(Path.GetFileNameWithoutExtension(assemblyName),
				syntaxTrees, defaultReferences.Value,
				new CSharpCompilationOptions(
					OutputKind.DynamicallyLinkedLibrary,
					platform: Platform.AnyCpu,
					optimizationLevel: OptimizationLevel.Release,
					allowUnsafe: true,
					deterministic: true
				));
			using (FileStream peStream = File.Open(assemblyName + ".dll", FileMode.OpenOrCreate, FileAccess.ReadWrite))
			using (FileStream pdbStream = File.Open(assemblyName + ".pdb", FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				var emitResult = compilation.Emit(peStream, pdbStream, options: new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb, pdbFilePath: assemblyName + ".pdb"), embeddedTexts: embeddedTexts);
				if (!emitResult.Success)
				{
					StringBuilder b = new StringBuilder("Compiler error:");
					foreach (var diag in emitResult.Diagnostics)
					{
						b.AppendLine(diag.ToString());
					}
					throw new Exception(b.ToString());
				}
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
			if ((cscOptions & CompilerOptions.UseMcs) != 0)
				suffix += ".mcs";
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
			if (settings == null)
				settings = new DecompilerSettings();
			using (var file = new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read))
			{
				var module = new PEFile(assemblyFileName, file, PEStreamOptions.PrefetchEntireImage);
				var resolver = new UniversalAssemblyResolver(assemblyFileName, false,
					module.Reader.DetectTargetFrameworkId(), PEStreamOptions.PrefetchMetadata);
				resolver.AddSearchDirectory(Path.GetDirectoryName(typeof(Span<>).Assembly.Location));
				var typeSystem = new DecompilerTypeSystem(module, resolver, settings);
				CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, settings);
				decompiler.AstTransforms.Insert(0, new RemoveEmbeddedAttributes());
				decompiler.AstTransforms.Insert(0, new RemoveCompilerAttribute());
				decompiler.AstTransforms.Insert(0, new RemoveNamespaceMy());
				decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
				var pdbFileName = Path.ChangeExtension(assemblyFileName, ".pdb");
				if (File.Exists(pdbFileName))
					decompiler.DebugInfoProvider = PdbProvider.DebugInfoUtils.FromFile(module, pdbFileName);
				var syntaxTree = decompiler.DecompileWholeModuleAsSingleFile(sortTypes: true);

				StringWriter output = new StringWriter();
				CSharpFormattingOptions formattingPolicy = CreateFormattingPolicyForTests();
				var visitor = new CSharpOutputVisitor(output, formattingPolicy);
				syntaxTree.AcceptVisitor(visitor);

				string fileName = Path.GetTempFileName();
				File.WriteAllText(fileName, output.ToString());

				return fileName;
			}
		}

		private static CSharpFormattingOptions CreateFormattingPolicyForTests()
		{
			var formattingPolicy = FormattingOptionsFactory.CreateSharpDevelop();
			formattingPolicy.StatementBraceStyle = BraceStyle.NextLine;
			formattingPolicy.CatchNewLinePlacement = NewLinePlacement.NewLine;
			formattingPolicy.ElseNewLinePlacement = NewLinePlacement.NewLine;
			formattingPolicy.FinallyNewLinePlacement = NewLinePlacement.NewLine;
			formattingPolicy.SpaceBeforeAnonymousMethodParentheses = true;
			return formattingPolicy;
		}

		public static void RunAndCompareOutput(string testFileName, string outputFile, string decompiledOutputFile, string decompiledCodeFile = null)
		{
			string output1, output2, error1, error2;
			int result1 = Tester.Run(outputFile, out output1, out error1);
			int result2 = Tester.Run(decompiledOutputFile, out output2, out error2);

			Assert.AreEqual(0, result1, "Exit code != 0; did the test case crash?" + Environment.NewLine + error1);
			Assert.AreEqual(0, result2, "Exit code != 0; did the decompiled code crash?" + Environment.NewLine + error2);

			if (output1 != output2 || error1 != error2)
			{
				StringBuilder b = new StringBuilder();
				b.AppendLine($"Test {testFileName} failed: output does not match.");
				if (decompiledCodeFile != null)
				{
					b.AppendLine($"Decompiled code in {decompiledCodeFile}:line 1");
				}
				if (error1 != error2)
				{
					b.AppendLine("Got different error output.");
					b.AppendLine("Original error:");
					b.AppendLine(error1);
					b.AppendLine();
					b.AppendLine("Error after de+re-compiling:");
					b.AppendLine(error2);
					b.AppendLine();
				}
				if (output1 != output2)
				{
					string outputFileName = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(testFileName));
					File.WriteAllText(outputFileName + ".original.out", output1);
					File.WriteAllText(outputFileName + ".decompiled.out", output2);
					int diffLine = 0;
					string lastHeader = null;
					Tuple<string, string> errorItem = null;
					foreach (var pair in output1.Replace("\r", "").Split('\n').Zip(output2.Replace("\r", "").Split('\n'), Tuple.Create))
					{
						diffLine++;
						if (pair.Item1 != pair.Item2)
						{
							errorItem = pair;
							break;
						}
						if (pair.Item1.EndsWith(":", StringComparison.Ordinal))
						{
							lastHeader = pair.Item1;
						}
					}
					b.AppendLine($"Output differs; first difference in line {diffLine}");
					if (lastHeader != null)
					{
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

		internal static void RepeatOnIOError(Action action, int numTries = 5)
		{
			for (int i = 0; i < numTries - 1; i++)
			{
				try
				{
					action();
					return;
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
					// potential virus scanner problem
				}
				Thread.Sleep(10);
			}
			// If the last try still fails, don't catch the exception
			action();
		}

		public static void SignAssembly(string assemblyPath, string keyFilePath)
		{
			string snPath = SdkUtility.GetSdkPath("sn.exe");

			ProcessStartInfo info = new ProcessStartInfo(snPath);
			info.Arguments = $"-R \"{assemblyPath}\" \"{keyFilePath}\"";
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
		}
	}
}
