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
				results.PathToAssembly = outputFileName;

				bool targetNet40 = (flags & CompilerOptions.TargetNet40) != 0;

				var (roslynVersion, languageVersion, targetFramework) = (flags & CompilerOptions.UseRoslynMask) switch {
					0 => ("legacy", "11", null),
					CompilerOptions.UseRoslyn1_3_2 => ("1.3.2", "14", null),
					CompilerOptions.UseRoslyn2_10_0 => ("2.10.0", "latest", targetNet40 ? null : ".NETCoreApp,Version=v2.2"),
					CompilerOptions.UseRoslyn3_11_0 => ("3.11.0", "latest", targetNet40 ? null : ".NETCoreApp,Version=v5.0"),
					_ => (roslynLatestVersion, flags.HasFlag(CompilerOptions.Preview) ? "preview" : "latest", targetNet40 ? null : CurrentNetCoreAppVersion)
				};

				var vbcPath = roslynToolset.GetVBCompiler(roslynVersion);

				IEnumerable<string> references;
				string libPath;
				if ((flags & CompilerOptions.UseRoslynMask) != 0 && targetFramework != null)
				{
					var coreRefAsmPath = RefAssembliesToolset.GetPath(targetFramework);
					// On Windows vbc.exe binds the core library through its implicit desktop SDK
					// path, so the plain reference list works. Without that implicit SDK, the
					// netcore-2.2 reference set needs the same reference list as the C# side
					// (System.Private.CoreLib plus the facade assemblies): its facades are split
					// across many files and vbc only binds special types like System.Void from an
					// assembly that defines them rather than following type forwards.
					IEnumerable<string> referenceNames = !OperatingSystem.IsWindows() && targetFramework == ".NETCoreApp,Version=v2.2"
						? core220DefaultReferences
						: coreDefaultReferences;
					if (!OperatingSystem.IsWindows() && targetFramework == ".NETCoreApp,Version=v2.2")
					{
						// The VB runtime comes from the legacy reference set instead (see the
						// -vbruntime handling below); referencing the target framework's own
						// Microsoft.VisualBasic as well would be a BC32210 identity conflict.
						referenceNames = referenceNames.Where(r => r != "Microsoft.VisualBasic.dll");
					}
					references = referenceNames.Select(r => "-r:\"" + r + "\"");
					libPath = coreRefAsmPath;
				}
				else
				{
					references = defaultReferences.Select(r => "-r:\"" + r + "\"");
					if (!OperatingSystem.IsWindows())
					{
						// The dotnet-hosted compiler has no implicit mscorlib reference
						// (see CompileCSharp); vbc resolves the bare name via -libpath.
						references = references.Prepend("-r:\"mscorlib.dll\"");
					}
					libPath = RefAssembliesToolset.GetPath("legacy");
				}
				if (flags.HasFlag(CompilerOptions.ReferenceVisualBasic))
				{
					// In the non-Windows netcore-2.2 configuration the VB runtime comes in via
					// -vbruntime (see below); also referencing the reference set's own
					// Microsoft.VisualBasic facade would be a BC32210 identity conflict.
					if (OperatingSystem.IsWindows() || targetFramework != ".NETCoreApp,Version=v2.2")
					{
						references = references.Concat(new[] { "-r:\"Microsoft.VisualBasic.dll\"" });
					}
				}
				string otherOptions = $"-nologo -noconfig " +
					"-optioninfer+ -optionexplicit+ " +
					$"-langversion:{languageVersion} " +
					$"/optimize{(flags.HasFlag(CompilerOptions.Optimize) ? "+ " : "- ")}";

				if (!OperatingSystem.IsWindows())
				{
					// The dotnet-hosted vbc has no implicit SDK path for resolving the standard
					// libraries and the VB runtime. The My templates are disabled because they
					// need ApplicationServices types missing from the .NET build of
					// Microsoft.VisualBasic (the .NET SDK sets _MYTYPE=Empty for the same
					// reason); the decompile comparison strips the My namespace anyway.
					otherOptions += $"-sdkpath:\"{libPath}\" ";
					otherOptions += "-define:_MYTYPE=\\\"Empty\\\" ";
					if ((flags & CompilerOptions.UseRoslynMask) != 0 && targetFramework != null)
					{
						// In the .NET reference packs Microsoft.VisualBasic.dll is a
						// type-forwarding facade, and vbc does not follow forwards when binding
						// its runtime helpers (e.g. ProjectData); point it at the implementation.
						// Before .NET Core 3.0 there is no Microsoft.VisualBasic.Core.dll and the
						// core build of the VB runtime is a trimmed-down subset (no UBound etc.);
						// use the legacy reference set's desktop implementation instead, which is
						// also what vbc.exe on Windows implicitly uses as its default VB runtime.
						string vbRuntime = Path.Combine(libPath, "Microsoft.VisualBasic.Core.dll");
						if (!File.Exists(vbRuntime))
						{
							vbRuntime = Path.Combine(RefAssembliesToolset.GetPath("legacy"), "Microsoft.VisualBasic.dll");
						}
						otherOptions += $"-vbruntime:\"{vbRuntime}\" ";
					}
				}

				// See UseCompilerServer for why /shared is not passed to every compiler.
				if (roslynVersion != "legacy" && UseCompilerServer(roslynVersion))
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

				var command = WrapCompiler(vbcPath, $"{otherOptions}-libpath:\"{libPath}\" {string.Join(" ", references)} -out:\"{Path.GetFullPath(results.PathToAssembly)}\" {string.Join(" ", sourceFileNames.Select(fn => '"' + Path.GetFullPath(fn) + '"'))}")
					.WithValidation(CommandResultValidation.None);
				//Console.WriteLine($"\"{command.TargetFilePath}\" {command.Arguments}");

				var result = await command.ExecuteBufferedAsync().ConfigureAwait(false);

				if (!string.IsNullOrWhiteSpace(result.StandardOutput))
				{
					Console.WriteLine("output:" + Environment.NewLine + result.StandardOutput);
				}
				if (!string.IsNullOrWhiteSpace(result.StandardError))
				{
					Console.WriteLine("errors:" + Environment.NewLine + result.StandardError);
				}
				Assert.That(result.ExitCode, Is.EqualTo(0), "vbc failed");

				return results;
			}
			else
			{
				throw new NotSupportedException("Cannot use mcs for VB");
			}
		}
	}
}
