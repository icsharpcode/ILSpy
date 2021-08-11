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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using NUnit.Framework;

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

			if (!flags.HasFlag(CompilerOptions.UseMcs))
			{
				CompilerResults results = new CompilerResults(new TempFileCollection());
				results.PathToAssembly = outputFileName ?? Path.GetTempFileName();

				var (roslynVersion, languageVersion) = (flags & CompilerOptions.UseRoslynMask) switch {
					0 => ("legacy", "11"),
					CompilerOptions.UseRoslyn1_3_2 => ("1.3.2", "14"),
					CompilerOptions.UseRoslyn2_10_0 => ("2.10.0", "latest"),
					_ => (RoslynLatestVersion, flags.HasFlag(CompilerOptions.Preview) ? "preview" : "latest")
				};

				var vbcPath = roslynToolset.GetVBCompiler(roslynVersion);

				IEnumerable<string> references;
				if ((flags & CompilerOptions.UseRoslynMask) != 0)
				{
					if (flags.HasFlag(CompilerOptions.ReferenceCore))
					{
						references = coreDefaultReferences.Value.Select(r => "-r:\"" + r + "\"");
					}
					else
					{
						references = roslynDefaultReferences.Value.Select(r => "-r:\"" + r + "\"");
					}
				}
				else
				{
					references = defaultReferences.Value.Select(r => "-r:\"" + r + "\"");
				}
				if (flags.HasFlag(CompilerOptions.ReferenceVisualBasic))
				{
					if ((flags & CompilerOptions.UseRoslynMask) != 0)
					{
						references = references.Concat(visualBasic.Value.Select(r => "-r:\"" + r + "\""));
					}
					else
					{
						references = references.Concat(new[] { "-r:\"Microsoft.VisualBasic.dll\"" });
					}
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

				ProcessStartInfo info = new ProcessStartInfo(vbcPath);
				info.Arguments = $"{otherOptions}{string.Join(" ", references)} -out:\"{Path.GetFullPath(results.PathToAssembly)}\" {string.Join(" ", sourceFileNames.Select(fn => '"' + Path.GetFullPath(fn) + '"'))}";
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
				Assert.AreEqual(0, process.ExitCode, "vbc failed");
				return results;
			}
			else
			{
				throw new NotSupportedException("Cannot use mcs for VB");
			}
		}
	}
}
