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
	/// <summary>
	/// Release build with a portable PDB whose SourceLink map points at the samples on GitHub,
	/// as packages built in CI usually ship.
	/// </summary>
	SourceLink,
}

/// <summary>Compiles the sample applications under <c>samples/</c> and caches the outputs per test run.</summary>
public static class SampleBuilder
{
	static readonly Dictionary<(string, BuildVariant), string> cache = new();
	static readonly string workRoot = Path.Combine(Path.GetTempPath(), "redecomp-tests", Guid.NewGuid().ToString("N"));

	public static string SamplesDirectory { get; } = FindSamplesDirectory();

	/// <summary>
	/// Raw-content URL of the samples directory at a commit where the samples are identical to the
	/// working tree; SourceLink builds point their documents there. The PDB checksums guarantee that
	/// a stale commit can only make the recovery fail, never return different sources.
	/// </summary>
	public const string SourceLinkBaseUrl = "https://raw.githubusercontent.com/franck00/ilspy/617bcbf74b60e795b714acf51bd0d4b16783e6d2/Redecomp/samples";

	public static string SourceDirectory(string sample) => Path.Combine(SamplesDirectory, sample);

	/// <summary>Samples made of a single project, i.e. with a project file at their root.</summary>
	public static IEnumerable<string> SingleProjectSamples()
		=> Directory.EnumerateDirectories(SamplesDirectory)
			.Where(d => Directory.EnumerateFiles(d, "*.csproj").Any())
			.Select(d => Path.GetFileName(d))
			.Order(StringComparer.Ordinal);

	/// <summary>Samples made of several projects, i.e. with a solution file at their root.</summary>
	public static IEnumerable<string> SolutionSamples()
		=> Directory.EnumerateDirectories(SamplesDirectory)
			.Where(d => Directory.EnumerateFiles(d, "*.slnx").Any())
			.Select(d => Path.GetFileName(d))
			.Order(StringComparer.Ordinal);

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
			Run("dotnet", $"build \"{copy}\" -c Release -o \"{output}\" {Properties(variant, copy, sample)}");
			string assembly = Path.Combine(output, sample + ".dll");
			cache[(sample, variant)] = assembly;
			return assembly;
		}
	}

	static readonly Dictionary<(string, BuildVariant), string> publishCache = new();
	static string? localFeed;

	/// <summary>
	/// Directory holding the NuGet packages built from <c>samples/_packages</c>; samples restore
	/// from it instead of nuget.org.
	/// </summary>
	public static string LocalFeed
	{
		get {
			lock (publishCache)
			{
				if (localFeed != null)
					return localFeed;
				string feed = Path.Combine(workRoot, "feed");
				foreach (string project in Directory.EnumerateFiles(Path.Combine(SamplesDirectory, "_packages"), "*.csproj", SearchOption.AllDirectories))
				{
					string copy = Path.Combine(workRoot, "packages", Path.GetFileNameWithoutExtension(project));
					CopyDirectory(Path.GetDirectoryName(project)!, copy);
					Run("dotnet", $"pack \"{copy}\" -c Release -o \"{feed}\"");
				}
				return localFeed = feed;
			}
		}
	}

	/// <summary>
	/// Publishes the executable project of a multi-project sample and returns the publish directory,
	/// as a deployed application would look.
	/// </summary>
	public static string Publish(string sample, BuildVariant variant)
	{
		string feed = LocalFeed;
		lock (publishCache)
		{
			if (publishCache.TryGetValue((sample, variant), out var cached))
				return cached;
			string copy = Path.Combine(workRoot, variant.ToString(), "publish-src", sample);
			CopyDirectory(SourceDirectory(sample), copy);
			string app = FindExecutableProject(copy);
			string output = Path.Combine(workRoot, variant.ToString(), "publish", sample);
			Run("dotnet", $"publish \"{app}\" -c Release -o \"{output}\" -p:RestoreSources=\"{feed}\" {Properties(variant, copy, sample)}");
			publishCache[(sample, variant)] = output;
			return output;
		}
	}

	/// <summary>The project of a solution directory whose output is an executable.</summary>
	public static string FindExecutableProject(string directory)
		=> Directory.EnumerateFiles(directory, "*.csproj", SearchOption.AllDirectories)
			.Single(p => File.ReadAllText(p).Contains("<OutputType>Exe</OutputType>", StringComparison.OrdinalIgnoreCase));

	static string Properties(BuildVariant variant, string copy, string sample) => variant switch {
		BuildVariant.NoPdb => "-p:DebugType=none",
		BuildVariant.PortablePdbWithDocs => "-p:DebugType=portable -p:GenerateDocumentationFile=true -p:NoWarn=CS1591",
		BuildVariant.EmbeddedSources => "-p:DebugType=embedded -p:EmbedAllSources=true",
		BuildVariant.SourceLink => $"-p:DebugType=portable -p:SourceLink=\"{WriteSourceLinkMap(copy, sample)}\"",
		_ => throw new ArgumentOutOfRangeException(nameof(variant)),
	};

	static string WriteSourceLinkMap(string copy, string sample)
	{
		string root = Path.GetFullPath(copy).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
		string json = System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, Dictionary<string, string>> {
			["documents"] = new() { [root + "*"] = $"{SourceLinkBaseUrl}/{sample}/*" },
		});
		string path = Path.Combine(copy, "sourcelink.json");
		File.WriteAllText(path, json);
		return path;
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
