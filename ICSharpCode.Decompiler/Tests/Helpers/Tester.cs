using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.NRefactory.CSharp;
using Microsoft.CSharp;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	public static class Tester
	{
		public static string CompileCSharp(string sourceFileName, bool optimize = false, bool useDebug = true)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
			CompilerParameters options = new CompilerParameters();
			options.GenerateExecutable = true;
			options.CompilerOptions = "/unsafe /o" + (optimize ? "+" : "-") + (useDebug ? " /debug" : "");
			options.ReferencedAssemblies.Add("System.Core.dll");
			CompilerResults results = provider.CompileAssemblyFromFile(options, sourceFileName);
			if (results.Errors.Count > 0) {
				StringBuilder b = new StringBuilder("Compiler error:");
				foreach (var error in results.Errors) {
					b.AppendLine(error.ToString());
				}
				throw new Exception(b.ToString());
			}
			return results.PathToAssembly;
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

			output = outputTask.Result;
			error = errorTask.Result;

			return process.ExitCode;
		}

		public static string DecompileCSharp(string assemblyFileName)
		{
			CSharpDecompiler decompiler = new CSharpDecompiler(AssemblyDefinition.ReadAssembly(assemblyFileName).MainModule);
			var syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();
			new Helpers.RemoveCompilerAttribute().Run(syntaxTree);
			new Helpers.RemoveEmptyNamespace().Run(syntaxTree);
			
			StringWriter output = new StringWriter();
			var visitor = new CSharpOutputVisitor(output, FormattingOptionsFactory.CreateSharpDevelop());
			syntaxTree.AcceptVisitor(visitor);

			string fileName = Path.GetTempFileName();
			File.WriteAllText(fileName, output.ToString());

			return fileName;
		}
	}
}
