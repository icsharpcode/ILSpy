using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	[Flags]
	public enum VBCompilerOptions
	{
		None,
		Optimize = 0x1,
		UseDebug = 0x2,
		Force32Bit = 0x4,
		Library = 0x8,
		UseRoslyn = 0x10,
	}

	partial class Tester
	{
		internal static string GetSuffix(VBCompilerOptions vbcOptions)
		{
			string suffix = "";
			if ((vbcOptions & VBCompilerOptions.Optimize) != 0)
				suffix += ".opt";
			if ((vbcOptions & VBCompilerOptions.Force32Bit) != 0)
				suffix += ".32";
			if ((vbcOptions & VBCompilerOptions.UseDebug) != 0)
				suffix += ".dbg";
			if ((vbcOptions & VBCompilerOptions.UseRoslyn) != 0)
				suffix += ".roslyn";
			return suffix;
		}

		public static List<KeyValuePair<string, object>> GetPreprocessorSymbols(VBCompilerOptions flags)
		{
			var preprocessorSymbols = new List<KeyValuePair<string, object>>();
			if (flags.HasFlag(VBCompilerOptions.UseDebug)) {
				preprocessorSymbols.Add(new KeyValuePair<string, object>("DEBUG", 1));
			}
			if (flags.HasFlag(VBCompilerOptions.Optimize)) {
				preprocessorSymbols.Add(new KeyValuePair<string, object>("OPT", 1));
			}
			if (flags.HasFlag(VBCompilerOptions.UseRoslyn)) {
				preprocessorSymbols.Add(new KeyValuePair<string, object>("ROSLYN", 1));
				preprocessorSymbols.Add(new KeyValuePair<string, object>("VB11", 1));
				preprocessorSymbols.Add(new KeyValuePair<string, object>("VB14", 1));
				preprocessorSymbols.Add(new KeyValuePair<string, object>("VB15", 1));
			} else {
				preprocessorSymbols.Add(new KeyValuePair<string, object>("LEGACY_VBC", 1));
			}
			return preprocessorSymbols;
		}

		public static CompilerResults CompileVB(string sourceFileName, VBCompilerOptions flags = VBCompilerOptions.UseDebug, string outputFileName = null)
		{
			List<string> sourceFileNames = new List<string> { sourceFileName };
			foreach (Match match in Regex.Matches(File.ReadAllText(sourceFileName), @"#include ""([\w\d./]+)""")) {
				sourceFileNames.Add(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFileName), match.Groups[1].Value)));
			}

			var preprocessorSymbols = GetPreprocessorSymbols(flags);

			if (flags.HasFlag(VBCompilerOptions.UseRoslyn)) {
				var parseOptions = new VisualBasicParseOptions(preprocessorSymbols: preprocessorSymbols, languageVersion: LanguageVersion.Latest);
				var syntaxTrees = sourceFileNames.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, path: f));
				var compilation = VisualBasicCompilation.Create(Path.GetFileNameWithoutExtension(sourceFileName),
					syntaxTrees, defaultReferences.Value,
					new VisualBasicCompilationOptions(
					flags.HasFlag(VBCompilerOptions.Library) ? OutputKind.DynamicallyLinkedLibrary : OutputKind.ConsoleApplication,
					platform: flags.HasFlag(VBCompilerOptions.Force32Bit) ? Platform.X86 : Platform.AnyCpu,
					optimizationLevel: flags.HasFlag(VBCompilerOptions.Optimize) ? OptimizationLevel.Release : OptimizationLevel.Debug,
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
				var provider = new VBCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v10.0" } });
				CompilerParameters options = new CompilerParameters();
				options.GenerateExecutable = !flags.HasFlag(VBCompilerOptions.Library);
				options.CompilerOptions = "/o" + (flags.HasFlag(VBCompilerOptions.Optimize) ? "+" : "-");
				options.CompilerOptions += (flags.HasFlag(VBCompilerOptions.UseDebug) ? " /debug" : "");
				options.CompilerOptions += (flags.HasFlag(VBCompilerOptions.Force32Bit) ? " /platform:anycpu32bitpreferred" : "");
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

	}
}
