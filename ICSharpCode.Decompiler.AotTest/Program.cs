// Copyright (c) 2026 Christoph Wille
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
using System.IO;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.AotTest
{
	/// <summary>
	/// Decompiles one assembly to one file and then checks that a DecompilerException thrown
	/// from a nested helper still names that helper in its stack trace. The same binary is run
	/// as a Native AOT publish and as a managed build; the gate compares the two output files
	/// byte for byte and relies on the exit code for the stack-trace check.
	/// </summary>
	static class Program
	{
		static int Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.Error.WriteLine("usage: ICSharpCode.Decompiler.AotTest <input assembly> <output file>");
				return 2;
			}
			string input = Path.GetFullPath(args[0]);
			string output = Path.GetFullPath(args[1]);

			// Unresolved references become unknown types in the output instead of an exception,
			// so a reference that one host resolves and the other does not shows up as an output
			// difference, which is what the gate compares. (Every shipping consumer, ilspycmd
			// included, runs with this setting.)
			var settings = new DecompilerSettings { ThrowOnAssemblyResolveErrors = false };
			var decompiler = new CSharpDecompiler(input, settings);
			File.WriteAllText(output, decompiler.DecompileWholeModuleAsString());
			Console.WriteLine($"decompiled {input} -> {output}");

			// Which file each reference resolved to depends on the host (shared framework, NuGet
			// cache, application directory); the ledger goes to the console only, never into the
			// compared output, and explains a mismatch between the native and the managed run.
			foreach (var module in decompiler.TypeSystem.ReferencedModules)
			{
				Console.WriteLine($"  {module.FullAssemblyName} -> {module.MetadataFile?.FileName ?? "NOT FOUND"}");
			}

			string trace;
			try
			{
				AotGateFrames.ThrowFromHelper();
				return 3;
			}
			catch (Exception ex)
			{
				using var file = new PEFile(input);
				trace = new DecompilerException(file, "AOT gate", ex).ToString();
			}
			Console.WriteLine(trace);
			if (trace.Contains(nameof(AotGateFrames.ThrowFromHelper), StringComparison.Ordinal))
			{
				return 0;
			}
			Console.Error.WriteLine($"FAIL: the DecompilerException stack trace does not name {nameof(AotGateFrames.ThrowFromHelper)}");
			return 1;
		}
	}

	static class AotGateFrames
	{
		public static void ThrowFromHelper()
		{
			throw new InvalidOperationException("thrown for the stack trace check");
		}
	}
}
