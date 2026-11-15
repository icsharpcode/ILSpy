// Copyright (c) 2026 Franck
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

using Redecomp.Core.Reconstruction;

namespace Redecomp.Cli;

public static class Program
{
	const string Usage = "usage: redecomp <assembly.dll | publish-directory> -o <output-directory>";

	public static int Main(string[] args)
	{
		string? assembly = null;
		string? output = null;
		for (int i = 0; i < args.Length; i++)
		{
			if ((args[i] == "-o" || args[i] == "--output") && i + 1 < args.Length)
				output = args[++i];
			else if (assembly == null && !args[i].StartsWith('-'))
				assembly = args[i];
			else
				return Fail($"unexpected argument '{args[i]}'");
		}
		if (assembly == null || output == null)
			return Fail("missing assembly or output directory");
		if (Directory.Exists(assembly))
		{
			var solution = SolutionReconstructor.Reconstruct(assembly, output);
			Console.WriteLine($"Solution:    {solution.SolutionFile}");
			foreach (var project in solution.Projects)
				Print(project.Result);
			return 0;
		}
		if (!File.Exists(assembly))
			return Fail($"assembly or publish directory not found: {assembly}");

		Print(ProjectReconstructor.Reconstruct(assembly, output));
		return 0;
	}

	static void Print(ReconstructionResult result)
	{
		Console.WriteLine($"Project:     {result.ProjectFile}");
		Console.WriteLine($"  Sources:     {result.Origin switch {
			SourceOrigin.EmbeddedSources => "original files embedded in the PDB",
			_ => "decompiled from IL",
		}}");
		Console.WriteLine($"  Debug info:  {result.DebugInfo ?? "none"}");
	}

	static int Fail(string message)
	{
		Console.Error.WriteLine($"redecomp: {message}");
		Console.Error.WriteLine(Usage);
		return 2;
	}
}
