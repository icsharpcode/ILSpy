using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	partial class Tester
	{
		public static CompilerResults CompileVB(string sourceFileName, CompilerOptions flags = CompilerOptions.UseDebug, string outputFileName = null)
		{
			List<string> sourceFileNames = new List<string> { sourceFileName };
			foreach (Match match in Regex.Matches(File.ReadAllText(sourceFileName), @"#include ""([\w\d./]+)"""))
			{
				sourceFileNames.Add(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFileName), match.Groups[1].Value)));
			}

			var preprocessorSymbols = GetPreprocessorSymbols(flags).Select(symbol => new KeyValuePair<string, object>(symbol, 1)).ToList();

			if (flags.HasFlag(CompilerOptions.UseRoslyn))
			{
				var parseOptions = new VisualBasicParseOptions(preprocessorSymbols: preprocessorSymbols, languageVersion: LanguageVersion.Latest);
				var syntaxTrees = sourceFileNames.Select(f => SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f), parseOptions, path: f));
				var references = defaultReferences.Value;
				if (flags.HasFlag(CompilerOptions.ReferenceVisualBasic))
				{
					references = references.Concat(visualBasic.Value);
				}
				var compilation = VisualBasicCompilation.Create(Path.GetFileNameWithoutExtension(sourceFileName),
					syntaxTrees, references,
					new VisualBasicCompilationOptions(
					flags.HasFlag(CompilerOptions.Library) ? OutputKind.DynamicallyLinkedLibrary : OutputKind.ConsoleApplication,
					platform: flags.HasFlag(CompilerOptions.Force32Bit) ? Platform.X86 : Platform.AnyCpu,
					optimizationLevel: flags.HasFlag(CompilerOptions.Optimize) ? OptimizationLevel.Release : OptimizationLevel.Debug,
					deterministic: true
				));
				CompilerResults results = new CompilerResults(new TempFileCollection());
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();
				var emitResult = compilation.Emit(results.PathToAssembly);
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
				throw new NotSupportedException("Cannot use mcs for VB");
			}
			else
			{
				var provider = new VBCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
				CompilerParameters options = new CompilerParameters();
				options.GenerateExecutable = !flags.HasFlag(CompilerOptions.Library);
				options.CompilerOptions = "/optimize" + (flags.HasFlag(CompilerOptions.Optimize) ? "+" : "-");
				options.CompilerOptions += (flags.HasFlag(CompilerOptions.UseDebug) ? " /debug" : "");
				options.CompilerOptions += (flags.HasFlag(CompilerOptions.Force32Bit) ? " /platform:anycpu32bitpreferred" : "");
				options.CompilerOptions += " /optioninfer+ /optionexplicit+";
				if (preprocessorSymbols.Count > 0)
				{
					options.CompilerOptions += " /d:" + string.Join(",", preprocessorSymbols.Select(p => $"{p.Key}={p.Value}"));
				}
				if (outputFileName != null)
				{
					options.OutputAssembly = outputFileName;
				}

				options.ReferencedAssemblies.Add("System.dll");
				options.ReferencedAssemblies.Add("System.Core.dll");
				options.ReferencedAssemblies.Add("System.Xml.dll");
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
	}
}
