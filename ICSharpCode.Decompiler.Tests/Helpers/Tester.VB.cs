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

using CliWrap;
using CliWrap.Buffered;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	partial class Tester
	{
		public static async Task<CompilerResults> CompileVB(string sourceFileName, CompilerOptions flags = CompilerOptions.UseDebug, string outputFileName = null)
		{
			List<string> sourceFileNames = new List<string> { sourceFileName };
			foreach (Match match in Regex.Matches(File.ReadAllText(sourceFileName), @"#include ""([\w\d./]+)"""))
			{
				sourceFileNames.Add(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(sourceFileName), match.Groups[1].Value)));
			}

			var preprocessorSymbols = GetPreprocessorSymbols(flags).Select(symbol => new KeyValuePair<string, object>(symbol, 1)).ToList();

			if ((flags & CompilerOptions.UseMcsMask) == 0)
			{
				CompilerResults results = new CompilerResults();
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();

				var (roslynVersion, languageVersion) = (flags & CompilerOptions.UseRoslynMask) switch {
					0 => ("legacy", "11"),
					CompilerOptions.UseRoslyn1_3_2 => ("1.3.2", "14"),
					CompilerOptions.UseRoslyn2_10_0 => ("2.10.0", "latest"),
					CompilerOptions.UseRoslyn3_11_0 => ("3.11.0", "latest"),
					_ => (roslynLatestVersion, flags.HasFlag(CompilerOptions.Preview) ? "preview" : "latest")
				};

				var vbcPath = roslynToolset.GetVBCompiler(roslynVersion);

				IEnumerable<string> references;
				if ((flags & CompilerOptions.UseRoslynMask) != 0)
				{
					references = coreDefaultReferences.Select(r => "-r:\"" + r + "\"");
				}
				else
				{
					references = defaultReferences.Select(r => "-r:\"" + r + "\"");
				}
				if (flags.HasFlag(CompilerOptions.ReferenceVisualBasic))
				{
					references = references.Concat(new[] { "-r:\"Microsoft.VisualBasic.dll\"" });
				}
				string otherOptions = $"-noconfig " +
					"-optioninfer+ -optionexplicit+ " +
					$"-langversion:{languageVersion} " +
					$"/optimize{(flags.HasFlag(CompilerOptions.Optimize) ? "+ " : "- ")}";

				// note: the /shared switch is undocumented. It allows us to use the VBCSCompiler.exe compiler
				// server to speed up testing
				if (roslynVersion != "legacy")
				{
					otherOptions += "/shared ";
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
					otherOptions += " \"-d:" + string.Join(",", preprocessorSymbols.Select(kv => kv.Key + "=" + kv.Value)) + "\" ";
				}

				var command = Cli.Wrap(vbcPath)
					.WithArguments($"{otherOptions}{string.Join(" ", references)} -out:\"{Path.GetFullPath(results.PathToAssembly)}\" {string.Join(" ", sourceFileNames.Select(fn => '"' + Path.GetFullPath(fn) + '"'))}")
					.WithValidation(CommandResultValidation.None);
				Console.WriteLine($"\"{command.TargetFilePath}\" {command.Arguments}");

				var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

				Console.WriteLine("output: " + result.StandardOutput);
				Console.WriteLine("errors: " + result.StandardError);
				Assert.AreEqual(0, result.ExitCode, "vbc failed");

				return results;
			}
			else
			{
				throw new NotSupportedException("Cannot use mcs for VB");
			}
		}
	}
}
