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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

using CliWrap;
using CliWrap.Buffered;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.PdbProvider;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

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
		UseRoslyn1_3_2 = 0x10,
		UseMcs2_6_4 = 0x20,
		ReferenceVisualBasic = 0x40,
		TargetNet40 = 0x80,
		GeneratePdb = 0x100,
		Preview = 0x200,
		UseRoslyn2_10_0 = 0x400,
		UseRoslyn3_11_0 = 0x800,
		UseRoslynLatest = 0x1000,
		UseMcs5_23 = 0x2000,
		UseTestRunner = 0x4000,
		NullableEnable = 0x8000,
		UseMcsMask = UseMcs2_6_4 | UseMcs5_23,
		UseRoslynMask = UseRoslyn1_3_2 | UseRoslyn2_10_0 | UseRoslyn3_11_0 | UseRoslynLatest
	}

	[Flags]
	public enum AssemblerOptions
	{
		None,
		UseDebug = 0x1,
		Force32Bit = 0x2,
		Library = 0x4,
		/// Testing our own disassembler, or working around a bug in ildasm.
		UseOwnDisassembler = 0x8,
		/// Work around bug in .NET 5 ilasm (https://github.com/dotnet/runtime/issues/32400)
		UseLegacyAssembler = 0x10,
	}

	public static partial class Tester
	{
		public static readonly string TesterPath;
		public static readonly string TestCasePath;

		static readonly string testRunnerBasePath;
		static readonly string packagesPropsFile;
		static readonly string roslynLatestVersion;
		static readonly RoslynToolset roslynToolset;
		static readonly VsWhereToolset vswhereToolset;

		static Tester()
		{
			TesterPath = Path.GetDirectoryName(typeof(Tester).Assembly.Location);
			TestCasePath = Path.Combine(TesterPath, "../../../../TestCases");
#if DEBUG
			testRunnerBasePath = Path.Combine(TesterPath, "../../../../../ICSharpCode.Decompiler.TestRunner/bin/Debug/net6.0-windows");
#else
			testRunnerBasePath = Path.Combine(TesterPath, "../../../../../ICSharpCode.Decompiler.TestRunner/bin/Release/net6.0-windows");
#endif
			packagesPropsFile = Path.Combine(TesterPath, "../../../../../packages.props");
			roslynLatestVersion = XDocument.Load(packagesPropsFile).XPathSelectElement("//RoslynVersion").Value;
			roslynToolset = new RoslynToolset();
			vswhereToolset = new VsWhereToolset();
		}

		internal static async Task Initialize()
		{
			await roslynToolset.Fetch("1.3.2").ConfigureAwait(false);
			await roslynToolset.Fetch("2.10.0").ConfigureAwait(false);
			await roslynToolset.Fetch("3.11.0").ConfigureAwait(false);
			await roslynToolset.Fetch(roslynLatestVersion).ConfigureAwait(false);

			await vswhereToolset.Fetch().ConfigureAwait(false);

#if DEBUG
			await BuildTestRunner("win-x86", "Debug").ConfigureAwait(false);
			await BuildTestRunner("win-x64", "Debug").ConfigureAwait(false);
#else
			await BuildTestRunner("win-x86", "Release").ConfigureAwait(false);
			await BuildTestRunner("win-x64", "Release").ConfigureAwait(false);
#endif
		}

		static async Task BuildTestRunner(string runtime, string config)
		{
			await Cli.Wrap("dotnet.exe")
				.WithArguments(new[] { "build", Path.Combine(TesterPath, "../../../../../ICSharpCode.Decompiler.TestRunner/ICSharpCode.Decompiler.TestRunner.csproj"), "-r", runtime, "-c", config, "--self-contained" })
				.ExecuteAsync();
		}

		public static async Task<string> AssembleIL(string sourceFileName, AssemblerOptions options = AssemblerOptions.UseDebug)
		{
			string ilasmPath;
			if (options.HasFlag(AssemblerOptions.UseLegacyAssembler))
			{
				ilasmPath = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Microsoft.NET\Framework\v4.0.30319\ilasm.exe");
			}
			else
			{
				ilasmPath = Path.Combine(
					Path.GetDirectoryName(typeof(Tester).Assembly.Location),
					"ilasm.exe");
			}
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

			var command = Cli.Wrap(ilasmPath)
				.WithArguments($"/nologo {otherOptions}/output=\"{outputFile}\" \"{sourceFileName}\"")
				.WithValidation(CommandResultValidation.None);

			var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

			Console.WriteLine("output: " + result.StandardOutput);
			Console.WriteLine("errors: " + result.StandardError);
			Assert.AreEqual(0, result.ExitCode, "ilasm failed");

			return outputFile;
		}

		public static async Task<string> Disassemble(string sourceFileName, string outputFile, AssemblerOptions asmOptions)
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

			string ildasmPath = Path.Combine(
				Path.GetDirectoryName(typeof(Tester).Assembly.Location),
				"ildasm.exe");

			var command = Cli.Wrap(ildasmPath)
				.WithArguments($"/utf8 /out=\"{outputFile}\" \"{sourceFileName}\"")
				.WithValidation(CommandResultValidation.None);

			var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

			Console.WriteLine("output: " + result.StandardOutput);
			Console.WriteLine("errors: " + result.StandardError);
			Assert.AreEqual(0, result.ExitCode, "ildasm failed");

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

		static readonly string coreRefAsmPath = new DotNetCorePathFinder(TargetFrameworkIdentifier.NET,
			new Version(6, 0), "Microsoft.NETCore.App")
				.GetReferenceAssemblyPath(".NETCoreApp,Version=v6.0");

		public static readonly string RefAsmPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
			@"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2");

		static readonly string[] defaultReferences = new[] {
			"System.dll",
			"System.Core.dll",
			"System.Xml.dll",
			"Microsoft.CSharp.dll"
		};

		static readonly string[] coreDefaultReferences = new[]
			{
				"netstandard.dll",
				"mscorlib.dll",
				"System.dll",
				"System.Collections.dll",
				"System.Console.dll",
				"System.Core.dll",
				"System.Linq.dll",
				"System.Linq.Expressions.dll",
				"System.Linq.Queryable.dll",
				"System.IO.FileSystem.Watcher.dll",
				"System.Threading.dll",
				"System.Threading.Thread.dll",
				"System.Runtime.dll",
				"System.Runtime.InteropServices.dll",
				"System.Xml.dll",
				"System.Xml.ReaderWriter.dll",
				"System.ValueTuple.dll",
				"Microsoft.CSharp.dll",
				"Microsoft.VisualBasic.dll",
			};

		const string targetFrameworkAttributeSnippet = @"

[assembly: System.Runtime.Versioning.TargetFramework("".NETCoreApp,Version=v6.0"", FrameworkDisplayName = """")]

";

		static readonly Lazy<string> targetFrameworkAttributeSnippetFile = new Lazy<string>(GetTargetFrameworkAttributeSnippetFile);

		static string GetTargetFrameworkAttributeSnippetFile()
		{
			var tempFile = Path.GetTempFileName();
			File.WriteAllText(tempFile, targetFrameworkAttributeSnippet);
			return tempFile;
		}

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
			if (flags.HasFlag(CompilerOptions.TargetNet40))
			{
				preprocessorSymbols.Add("NET40");
			}
			if ((flags & CompilerOptions.UseRoslynMask) != 0)
			{
				if (!flags.HasFlag(CompilerOptions.TargetNet40))
				{
					preprocessorSymbols.Add("NETCORE");
					preprocessorSymbols.Add("NET60");
				}
				preprocessorSymbols.Add("ROSLYN");
				preprocessorSymbols.Add("CS60");
				preprocessorSymbols.Add("VB11");
				preprocessorSymbols.Add("VB14");
				if (flags.HasFlag(CompilerOptions.UseRoslyn2_10_0)
					|| flags.HasFlag(CompilerOptions.UseRoslyn3_11_0)
					|| flags.HasFlag(CompilerOptions.UseRoslynLatest))
				{
					preprocessorSymbols.Add("ROSLYN2");
					preprocessorSymbols.Add("CS70");
					preprocessorSymbols.Add("CS71");
					preprocessorSymbols.Add("CS72");
					preprocessorSymbols.Add("CS73");
					preprocessorSymbols.Add("VB15");
				}
				if (flags.HasFlag(CompilerOptions.UseRoslyn3_11_0)
					|| flags.HasFlag(CompilerOptions.UseRoslynLatest))
				{
					preprocessorSymbols.Add("ROSLYN3");
					preprocessorSymbols.Add("CS80");
					preprocessorSymbols.Add("CS90");
					preprocessorSymbols.Add("VB16");
				}
				if (flags.HasFlag(CompilerOptions.UseRoslynLatest))
				{
					preprocessorSymbols.Add("ROSLYN4");
					preprocessorSymbols.Add("CS100");
					if (flags.HasFlag(CompilerOptions.Preview))
					{
						preprocessorSymbols.Add("CS110");
					}
				}
			}
			else if ((flags & CompilerOptions.UseMcsMask) != 0)
			{
				preprocessorSymbols.Add("MCS");
				if (flags.HasFlag(CompilerOptions.UseMcs2_6_4))
				{
					preprocessorSymbols.Add("MCS2");
				}
				if (flags.HasFlag(CompilerOptions.UseMcs5_23))
				{
					preprocessorSymbols.Add("MCS5");
				}
			}
			else
			{
				preprocessorSymbols.Add("LEGACY_CSC");
				preprocessorSymbols.Add("LEGACY_VBC");
			}
			return preprocessorSymbols;
		}

		public static async Task<CompilerResults> CompileCSharp(string sourceFileName, CompilerOptions flags = CompilerOptions.UseDebug, string outputFileName = null)
		{
			List<string> sourceFileNames = new List<string> { sourceFileName };
			foreach (Match match in Regex.Matches(File.ReadAllText(sourceFileName), @"#include ""([\w\d./]+)"""))
			{
				sourceFileNames.Add(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFileName), match.Groups[1].Value)));
			}
			bool targetNet40 = (flags & CompilerOptions.TargetNet40) != 0;
			bool useRoslyn = (flags & CompilerOptions.UseRoslynMask) != 0;
			if (useRoslyn && !targetNet40)
			{
				sourceFileNames.Add(targetFrameworkAttributeSnippetFile.Value);
			}

			var preprocessorSymbols = GetPreprocessorSymbols(flags);

			if ((flags & CompilerOptions.UseMcsMask) == 0)
			{
				CompilerResults results = new CompilerResults();
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();

				var (roslynVersion, languageVersion) = (flags & CompilerOptions.UseRoslynMask) switch {
					0 => ("legacy", "5"),
					CompilerOptions.UseRoslyn1_3_2 => ("1.3.2", "6"),
					CompilerOptions.UseRoslyn2_10_0 => ("2.10.0", "latest"),
					CompilerOptions.UseRoslyn3_11_0 => ("3.11.0", "latest"),
					_ => (roslynLatestVersion, flags.HasFlag(CompilerOptions.Preview) ? "preview" : "latest")
				};

				var cscPath = roslynToolset.GetCSharpCompiler(roslynVersion);

				string libPath;
				IEnumerable<string> references;
				if (useRoslyn && !targetNet40)
				{
					libPath = "\"" + coreRefAsmPath + "\"";
					references = coreDefaultReferences.Select(r => "-r:\"" + Path.Combine(coreRefAsmPath, r) + "\"");
				}
				else
				{
					libPath = "\"" + RefAsmPath + "\",\"" + Path.Combine(RefAsmPath, "Facades") + "\"";
					references = defaultReferences.Select(r => "-r:\"" + Path.Combine(RefAsmPath, r) + "\"");
				}
				if (flags.HasFlag(CompilerOptions.ReferenceVisualBasic))
				{
					references = references.Concat(new[] { "-r:\"Microsoft.VisualBasic.dll\"" });
				}
				string otherOptions = $"-noconfig " +
					$"-langversion:{languageVersion} " +
					$"-unsafe -o{(flags.HasFlag(CompilerOptions.Optimize) ? "+ " : "- ")}";

				// note: the /shared switch is undocumented. It allows us to use the VBCSCompiler.exe compiler
				// server to speed up testing
				if (roslynVersion != "legacy")
				{
					otherOptions += "/shared ";
					if (!targetNet40 && Version.Parse(roslynVersion).Major > 2)
					{
						if (flags.HasFlag(CompilerOptions.NullableEnable))
							otherOptions += "/nullable+ ";
						else
							otherOptions += "/nullable- ";
					}
				}

				if (flags.HasFlag(CompilerOptions.Library))
				{
					otherOptions += "-t:library ";
				}
				else
				{
					otherOptions += "-t:exe ";
				}

				if (flags.HasFlag(CompilerOptions.GeneratePdb))
				{
					otherOptions += "-debug:full ";
				}
				else
				{
					otherOptions += "-debug- ";
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

				var command = Cli.Wrap(cscPath)
					.WithArguments($"{otherOptions} -lib:{libPath} {string.Join(" ", references)} -out:\"{Path.GetFullPath(results.PathToAssembly)}\" {string.Join(" ", sourceFileNames.Select(fn => '"' + Path.GetFullPath(fn) + '"'))}")
					.WithValidation(CommandResultValidation.None);
				Console.WriteLine($"\"{command.TargetFilePath}\" {command.Arguments}");

				var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

				Console.WriteLine("output: " + result.StandardOutput);
				Console.WriteLine("errors: " + result.StandardError);
				Assert.AreEqual(0, result.ExitCode, "csc failed");

				return results;
			}
			else
			{
				CompilerResults results = new CompilerResults();
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();
				string testBasePath = RoundtripAssembly.TestDir;
				if (!Directory.Exists(testBasePath))
				{
					Assert.Ignore($"Compilation with mcs ignored: test directory '{testBasePath}' needs to be checked out separately." + Environment.NewLine +
			  $"git clone https://github.com/icsharpcode/ILSpy-tests \"{testBasePath}\"");
				}
				string mcsPath = (flags & CompilerOptions.UseMcsMask) switch {
					CompilerOptions.UseMcs5_23 => Path.Combine(testBasePath, @"mcs\5.23\bin\mcs.bat"),
					_ => Path.Combine(testBasePath, @"mcs\2.6.4\bin\gmcs.bat")
				};
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

				var command = Cli.Wrap(mcsPath)
					.WithArguments($"{otherOptions}-out:\"{Path.GetFullPath(results.PathToAssembly)}\" {string.Join(" ", sourceFileNames.Select(fn => '"' + Path.GetFullPath(fn) + '"'))}")
					.WithValidation(CommandResultValidation.None);
				Console.WriteLine($"\"{command.TargetFilePath}\" {command.Arguments}");

				var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

				Console.WriteLine("output: " + result.StandardOutput);
				Console.WriteLine("errors: " + result.StandardError);
				Assert.AreEqual(0, result.ExitCode, "mcs failed");

				return results;
			}
		}

		internal static DecompilerSettings GetSettings(CompilerOptions cscOptions)
		{
			if ((cscOptions & CompilerOptions.UseRoslynMask) != 0)
			{
				var langVersion = (cscOptions & CompilerOptions.UseRoslynMask) switch {
					CompilerOptions.UseRoslyn1_3_2 => CSharp.LanguageVersion.CSharp6,
					CompilerOptions.UseRoslyn2_10_0 => CSharp.LanguageVersion.CSharp7_3,
					CompilerOptions.UseRoslyn3_11_0 => CSharp.LanguageVersion.CSharp9_0,
					_ => cscOptions.HasFlag(CompilerOptions.Preview) ? CSharp.LanguageVersion.Latest : CSharp.LanguageVersion.CSharp10_0,
				};
				DecompilerSettings settings = new(langVersion) {
					// Never use file-scoped namespaces
					FileScopedNamespaces = false
				};
				return settings;
			}
			else
			{
				var settings = new DecompilerSettings(CSharp.LanguageVersion.CSharp5);
				if ((cscOptions & CompilerOptions.UseMcsMask) != 0)
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
				syntaxTrees, coreDefaultReferences.Select(r => MetadataReference.CreateFromFile(Path.Combine(coreRefAsmPath, r))),
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
			if ((cscOptions & CompilerOptions.TargetNet40) != 0)
				suffix += ".net40";
			if ((cscOptions & CompilerOptions.UseRoslyn1_3_2) != 0)
				suffix += ".roslyn1";
			if ((cscOptions & CompilerOptions.UseRoslyn2_10_0) != 0)
				suffix += ".roslyn2";
			if ((cscOptions & CompilerOptions.UseRoslyn3_11_0) != 0)
				suffix += ".roslyn3";
			if ((cscOptions & CompilerOptions.UseRoslynLatest) != 0)
				suffix += ".roslyn";
			if ((cscOptions & CompilerOptions.UseMcs2_6_4) != 0)
				suffix += ".mcs2";
			if ((cscOptions & CompilerOptions.UseMcs5_23) != 0)
				suffix += ".mcs5";
			return suffix;
		}

		public static async Task<(int ExitCode, string Output, string Error)> Run(string assemblyFileName)
		{
			var command = Cli.Wrap(assemblyFileName)
				.WithValidation(CommandResultValidation.None);

			var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

			return (result.ExitCode, result.StandardOutput, result.StandardError);
		}

		public static async Task<(int ExitCode, string Output, string Error)> RunWithTestRunner(string assemblyFileName, bool force32Bit)
		{
			string testRunner = Path.Combine(testRunnerBasePath, force32Bit ? "win-x86" : "win-x64", "ICSharpCode.Decompiler.TestRunner.exe");
			var command = Cli.Wrap(testRunner)
				.WithArguments(assemblyFileName)
				.WithValidation(CommandResultValidation.None);

			var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

			return (result.ExitCode, result.StandardOutput, result.StandardError);
		}

		public static Task<string> DecompileCSharp(string assemblyFileName, DecompilerSettings settings = null)
		{
			if (settings == null)
				settings = new DecompilerSettings();
			using (var file = new FileStream(assemblyFileName, FileMode.Open, FileAccess.Read))
			{
				var module = new PEFile(assemblyFileName, file, PEStreamOptions.PrefetchEntireImage);
				string targetFramework = module.Metadata.DetectTargetFrameworkId();
				var resolver = new UniversalAssemblyResolver(assemblyFileName, false,
					targetFramework, null, PEStreamOptions.PrefetchMetadata);
				resolver.AddSearchDirectory(targetFramework.Contains(".NETFramework") ? RefAsmPath : coreRefAsmPath);
				var typeSystem = new DecompilerTypeSystem(module, resolver, settings);
				CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, settings);
				decompiler.AstTransforms.Insert(0, new RemoveEmbeddedAttributes());
				decompiler.AstTransforms.Insert(0, new RemoveCompilerAttribute());
				decompiler.AstTransforms.Insert(0, new RemoveNamespaceMy());
				decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
				var pdbFileName = Path.ChangeExtension(assemblyFileName, ".pdb");
				if (File.Exists(pdbFileName))
					decompiler.DebugInfoProvider = DebugInfoUtils.FromFile(module, pdbFileName);
				var syntaxTree = decompiler.DecompileWholeModuleAsSingleFile(sortTypes: true);

				StringWriter output = new StringWriter();
				CSharpFormattingOptions formattingPolicy = CreateFormattingPolicyForTests();
				var visitor = new CSharpOutputVisitor(output, formattingPolicy);
				syntaxTree.AcceptVisitor(visitor);

				string fileName = Path.GetTempFileName();
				File.WriteAllText(fileName, output.ToString());

				return Task.FromResult(fileName);
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

		public static async Task RunAndCompareOutput(string testFileName, string outputFile, string decompiledOutputFile, string decompiledCodeFile = null, bool useTestRunner = false, bool force32Bit = false)
		{
			string output1, output2, error1, error2;
			int result1, result2;

			if (useTestRunner)
			{
				(result1, output1, error1) = await RunWithTestRunner(outputFile, force32Bit).ConfigureAwait(false);
				(result2, output2, error2) = await RunWithTestRunner(decompiledOutputFile, force32Bit).ConfigureAwait(false);
			}
			else
			{
				(result1, output1, error1) = await Run(outputFile).ConfigureAwait(false);
				(result2, output2, error2) = await Run(decompiledOutputFile).ConfigureAwait(false);
			}

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

		public static async Task SignAssembly(string assemblyPath, string keyFilePath)
		{
			string snPath = SdkUtility.GetSdkPath("sn.exe");

			var command = Cli.Wrap(snPath)
				.WithArguments($"-R \"{assemblyPath}\" \"{keyFilePath}\"")
				.WithValidation(CommandResultValidation.None);

			var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);
			Assert.AreEqual(0, result.ExitCode, "sn failed");

			Console.WriteLine("output: " + result.StandardOutput);
			Console.WriteLine("errors: " + result.StandardError);
		}

		public static async Task<string> FindMSBuild()
		{
			string path = vswhereToolset.GetVsWhere();

			var result = await Cli.Wrap(path)
				.WithArguments(@"-latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe")
				.WithValidation(CommandResultValidation.None)
				.ExecuteBufferedAsync().ConfigureAwait(false);
			if (result.ExitCode != 0)
				throw new InvalidOperationException("Could not find MSBuild");
			return result.StandardOutput.TrimEnd();
		}
	}

	public class CompilerResults
	{
		public string PathToAssembly { get; set; }

		public void DeleteTempFiles()
		{

		}
	}
}
