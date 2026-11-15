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

using System.Text;

using Redecomp.Core.Reconstruction;

namespace Redecomp.Bench;

/// <summary>Scores of one reconstruction against the original sources of a sample.</summary>
public sealed record BenchResult(
	string Sample, BuildVariant Variant, SourceOrigin Origin, bool Rebuilds, bool SameBehavior,
	Ratio FilesAtOriginalPath, Ratio LocalNames, Ratio Comments);

public readonly record struct Ratio(int Found, int Total)
{
	public override string ToString() => Total == 0 ? "n/a" : $"{100.0 * Found / Total:0}% ({Found}/{Total})";
}

/// <summary>
/// Reconstructs every sample in every build variant and compares the result with the original sources.
/// Usage: Redecomp.Bench [--out results.md]
/// </summary>
public static class Program
{
	public static int Main(string[] args)
	{
		string? outFile = args.Length == 2 && args[0] == "--out" ? args[1] : null;
		var results = new List<BenchResult>();
		foreach (string sample in Directory.EnumerateDirectories(SampleBuilder.SamplesDirectory).Select(d => Path.GetFileName(d)).Order())
		{
			foreach (var variant in Enum.GetValues<BuildVariant>())
			{
				var result = Run(sample, variant);
				results.Add(result);
				Console.Error.WriteLine($"{sample} / {variant}: done");
			}
		}
		string table = Format(results);
		Console.WriteLine(table);
		if (outFile != null)
			File.WriteAllText(outFile, table);
		return 0;
	}

	public static BenchResult Run(string sample, BuildVariant variant)
	{
		string assembly = SampleBuilder.Build(sample, variant);
		string output = ProjectBuilder.NewTempDirectory();
		var reconstruction = ProjectReconstructor.Reconstruct(assembly, output);

		bool rebuilds = false, sameBehavior = false;
		try
		{
			string rebuilt = ProjectBuilder.BuildProject(output, sample);
			rebuilds = true;
			sameBehavior = ProjectBuilder.RunAssembly(assembly) == ProjectBuilder.RunAssembly(rebuilt);
		}
		catch (InvalidOperationException)
		{
			// A failed build or a crashing run leaves the corresponding score at false.
		}

		string sourceDir = SampleBuilder.SourceDirectory(sample);
		var originals = SourceFiles(sourceDir);
		var reconstructed = SourceFiles(output);
		int atPath = originals.Count(o => File.Exists(Path.Combine(output, o)));

		var originalLocals = originals.SelectMany(f => SourceMetrics.DeclaredLocals(File.ReadAllText(Path.Combine(sourceDir, f)))).ToHashSet();
		var outputLocals = reconstructed.SelectMany(f => SourceMetrics.DeclaredLocals(File.ReadAllText(Path.Combine(output, f)))).ToHashSet();
		var originalComments = originals.SelectMany(f => SourceMetrics.CommentLines(File.ReadAllText(Path.Combine(sourceDir, f)))).ToList();
		var outputComments = reconstructed.SelectMany(f => SourceMetrics.CommentLines(File.ReadAllText(Path.Combine(output, f)))).ToHashSet();

		return new BenchResult(sample, variant, reconstruction.Origin, rebuilds, sameBehavior,
			new Ratio(atPath, originals.Count),
			new Ratio(originalLocals.Count(outputLocals.Contains), originalLocals.Count),
			new Ratio(originalComments.Count(outputComments.Contains), originalComments.Count));
	}

	static List<string> SourceFiles(string root)
		=> Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
			.Select(f => Path.GetRelativePath(root, f))
			.Where(f => !f.Split(Path.DirectorySeparatorChar).Any(s => s is "bin" or "obj" or "bin-redecomp"))
			.ToList();

	public static string Format(IEnumerable<BenchResult> results)
	{
		var sb = new StringBuilder();
		sb.AppendLine("| Sample | Variant | Sources | Rebuilds | Same behavior | Files at original path | Local names | Comments |");
		sb.AppendLine("|---|---|---|---|---|---|---|---|");
		foreach (var r in results)
		{
			sb.AppendLine($"| {r.Sample} | {r.Variant} | {r.Origin} | {YesNo(r.Rebuilds)} | {YesNo(r.SameBehavior)} | {r.FilesAtOriginalPath} | {r.LocalNames} | {r.Comments} |");
		}
		return sb.ToString();

		static string YesNo(bool value) => value ? "yes" : "no";
	}
}
