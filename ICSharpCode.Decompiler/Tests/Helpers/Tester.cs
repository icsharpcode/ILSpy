using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.NRefactory.CSharp;
using Microsoft.CSharp;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	[Flags]
	public enum CompilerOptions
	{
		None,
		Optimize,
		UseDebug
	}

	public static class Tester
	{
		public static CompilerResults CompileCSharp(string sourceFileName, CompilerOptions flags = CompilerOptions.UseDebug)
		{
			CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
			CompilerParameters options = new CompilerParameters();
			options.GenerateExecutable = true;
			options.CompilerOptions = "/unsafe /o" + (flags.HasFlag(CompilerOptions.Optimize) ? "+" : "-") + (flags.HasFlag(CompilerOptions.UseDebug) ? " /debug" : "");
			options.ReferencedAssemblies.Add("System.Core.dll");
			CompilerResults results = provider.CompileAssemblyFromFile(options, sourceFileName);
			if (results.Errors.Cast<CompilerError>().Any(e => !e.IsWarning)) {
				StringBuilder b = new StringBuilder("Compiler error:");
				foreach (var error in results.Errors) {
					b.AppendLine(error.ToString());
				}
				throw new Exception(b.ToString());
			}
			return results;
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

		public static string DecompileCSharp(string assemblyFileName)
		{
			var typeSystem = new DecompilerTypeSystem(ModuleDefinition.ReadModule(assemblyFileName));
			CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, new DecompilerSettings());
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
}
