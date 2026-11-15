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

using System.Diagnostics;

namespace Redecomp.Bench;

/// <summary>How a sample is compiled; each variant ships a different amount of debug information.</summary>
public enum BuildVariant
{
	/// <summary>Release build without any PDB, as most production binaries are shipped.</summary>
	NoPdb,
	/// <summary>Release build with a portable PDB and an XML documentation file next to the assembly.</summary>
	PortablePdbWithDocs,
	/// <summary>Release build with the PDB embedded in the assembly and all sources embedded in the PDB.</summary>
	EmbeddedSources,
}

/// <summary>Compiles the sample applications under <c>samples/</c> and caches the outputs per test run.</summary>
public static class SampleBuilder
{
	static readonly Dictionary<(string, BuildVariant), string> cache = new();
	static readonly string workRoot = Path.Combine(Path.GetTempPath(), "redecomp-tests", Guid.NewGuid().ToString("N"));

	public static string SamplesDirectory { get; } = FindSamplesDirectory();

	public static string SourceDirectory(string sample) => Path.Combine(SamplesDirectory, sample);

	/// <summary>Builds the sample and returns the path of its main assembly.</summary>
	public static string Build(string sample, BuildVariant variant)
	{
		lock (cache)
		{
			if (cache.TryGetValue((sample, variant), out var cached))
				return cached;
			// Each variant builds from its own copy so intermediate outputs never leak between variants.
			string copy = Path.Combine(workRoot, variant.ToString(), sample);
			CopyDirectory(SourceDirectory(sample), copy);
			string output = Path.Combine(workRoot, variant.ToString(), "out");
			string properties = variant switch {
				BuildVariant.NoPdb => "-p:DebugType=none",
				BuildVariant.PortablePdbWithDocs => "-p:DebugType=portable -p:GenerateDocumentationFile=true -p:NoWarn=CS1591",
				BuildVariant.EmbeddedSources => "-p:DebugType=embedded -p:EmbedAllSources=true",
				_ => throw new ArgumentOutOfRangeException(nameof(variant)),
			};
			Run("dotnet", $"build \"{copy}\" -c Release -o \"{output}\" {properties}");
			string assembly = Path.Combine(output, sample + ".dll");
			cache[(sample, variant)] = assembly;
			return assembly;
		}
	}

	/// <summary>Runs a process to completion and returns its standard output; throws when it fails.</summary>
	public static string Run(string fileName, string arguments, string? workingDirectory = null)
	{
		var psi = new ProcessStartInfo(fileName, arguments) {
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
		};
		using var process = Process.Start(psi)!;
		string stdout = process.StandardOutput.ReadToEnd();
		string stderr = process.StandardError.ReadToEnd();
		process.WaitForExit();
		if (process.ExitCode != 0)
			throw new InvalidOperationException($"'{fileName} {arguments}' failed:\n{stdout}\n{stderr}");
		return stdout;
	}

	static void CopyDirectory(string source, string target)
	{
		foreach (string file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
		{
			string relative = Path.GetRelativePath(source, file);
			if (relative.StartsWith("bin" + Path.DirectorySeparatorChar) || relative.StartsWith("obj" + Path.DirectorySeparatorChar))
				continue;
			string destination = Path.Combine(target, relative);
			Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
			File.Copy(file, destination, overwrite: true);
		}
		// Keeps the copy isolated from any Directory.Build.props above the temp folder.
		File.WriteAllText(Path.Combine(target, "Directory.Build.props"), "<Project />");
		File.WriteAllText(Path.Combine(target, "Directory.Packages.props"), "<Project />");
	}

	static string FindSamplesDirectory()
	{
		for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
		{
			string candidate = Path.Combine(dir.FullName, "samples");
			if (Directory.Exists(candidate) && File.Exists(Path.Combine(dir.FullName, "Redecomp.slnx")))
				return candidate;
		}
		throw new DirectoryNotFoundException("Could not locate the Redecomp samples directory.");
	}
}
