using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

using NDesk.Options;

class Program
{
	static int Main(string[] args)
	{
		const LanguageVersion languageVersion = WholeProjectDecompiler.defaultLanguageVersion;
		var settings = new DecompilerSettings(languageVersion);

		bool show_help = false;
		List<string> names = new List<string>();

		var p = new OptionSet() {
			{ "decompile",  "Decompile assembly to source code", v => settings.ProduceSourceCode = v != null },
			{ "?|h|help",  "show this message and exit", v => show_help = v != null },
		};

		List<string> extra = null;
		try
		{
			extra = p.Parse(args);
		}
		catch (OptionException e)
		{
			Console.Write($"Error: {e.Message}");
			show_help = true;
		}
		show_help = extra == null || extra.Count == 0;
		if (show_help)
		{
			Console.WriteLine("Usage: disasm [options] <1.dll> <2.dll> ...");
			Console.WriteLine();
			Console.WriteLine("Where options is one of following: ");
			Console.WriteLine();
			p.WriteOptionDescriptions(Console.Out);
		}

		foreach (string file_ in extra)
		{
			if (!File.Exists(file_))
			{
				Console.WriteLine($"Error: File '{file_}' does not exists.");
				return 2;
			}

			string path = Path.GetFullPath(file_);
			string inputDir = Path.GetDirectoryName(path);
			string file = Path.GetFileName(file_);
			string relFile = Path.GetFileNameWithoutExtension(file_);

			string decompiledDir = Path.Combine(inputDir, relFile);
			if (settings.ProduceSourceCode) //If not producing code, specify directory anyway, just to see if something outputs into it.
			{
				if (!Directory.Exists(decompiledDir))
				{
					Directory.CreateDirectory(decompiledDir);
				}
			}

			if (settings.ProduceSourceCode)
			{
				Console.Write($"Decompiling {file}... ");
			}

			Stopwatch w = Stopwatch.StartNew();
			using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			PEFile module = new PEFile(path, fileStream, PEStreamOptions.PrefetchEntireImage);
			var resolver = new LocalAssemblyResolver(path, inputDir, module.Reader.DetectTargetFrameworkId());
			resolver.AddSearchDirectory(inputDir);
			resolver.RemoveSearchDirectory(".");

			// use a fixed GUID so that we can diff the output between different ILSpy runs without spurious changes
			var projectGuid = Guid.Parse("{127C83E4-4587-4CF9-ADCA-799875F3DFE6}");
			var decompiler = new WholeProjectDecompiler(settings, projectGuid, resolver, resolver, debugInfoProvider: null);

			decompiler.DecompileProject(module, decompiledDir);

			if (settings.ProduceSourceCode)
			{
				Console.WriteLine($"ok.");
				Console.WriteLine($"Used time: {w.Elapsed.TotalSeconds:f2} sec");
			}
		}
		return 0;
	}

	public static string ByteArrayToString(byte[] ba)
	{
		StringBuilder hex = new StringBuilder(ba.Length * 3);
		foreach (byte b in ba)
			hex.AppendFormat("{0:X2} ", b);
		return hex.ToString();
	}
}
