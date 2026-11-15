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

using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;

using Redecomp.Core.Recovery;

namespace Redecomp.Core.Reconstruction;

/// <summary>
/// Moves decompiled files, which the decompiler names after namespaces and types, to the paths the
/// source files had in the original project, merging types that were declared in the same file.
/// </summary>
public static class SourceRelocator
{
	public static void Apply(MetadataReader module, MetadataReader pdb, string outputDirectory, bool useNestedDirectoriesForNamespaces)
	{
		var documents = TypeDocumentMap.Build(module, pdb);
		var relative = ProjectPaths.ToProjectRelative(documents.Values.Where(d => !ProjectPaths.IsGenerated(d)));

		// Each top-level type: decompiled file, original file (empty when unknown), namespace.
		var moves = new List<(string From, string To, string Namespace)>();
		foreach (var handle in module.TypeDefinitions)
		{
			var type = module.GetTypeDefinition(handle);
			if (!type.GetDeclaringType().IsNil)
				continue;
			string ns = module.GetString(type.Namespace);
			string from = WholeProjectDecompiler.GetFileNameForType(ns, module.GetString(type.Name), ".cs", useNestedDirectoriesForNamespaces);
			if (!File.Exists(Path.Combine(outputDirectory, from)))
				continue;
			string? to = documents.TryGetValue(handle, out var doc) && relative.TryGetValue(doc, out var rel) ? rel : null;
			moves.Add((from, to ?? "", ns));
		}

		// Types the PDB cannot place (no method bodies, no type documents) follow the other types of their namespace.
		var namespaceDirectories = moves.Where(m => m.To != "")
			.GroupBy(m => m.Namespace)
			.ToDictionary(g => g.Key, g => g.GroupBy(m => Path.GetDirectoryName(m.To) ?? "").MaxBy(d => d.Count())!.Key);
		for (int i = 0; i < moves.Count; i++)
		{
			var (from, to, ns) = moves[i];
			if (to == "" && namespaceDirectories.TryGetValue(ns, out var dir))
				moves[i] = (from, Path.Combine(dir, Path.GetFileName(from)), ns);
		}
		moves.RemoveAll(m => m.To == "" || m.To == m.From);
		if (moves.Count == 0)
			return;

		// Read everything before writing so a move never overwrites a file that has not been moved yet.
		var contents = moves.Select(m => m.From).Distinct().ToDictionary(f => f, f => File.ReadAllText(Path.Combine(outputDirectory, f)));
		foreach (string from in contents.Keys)
			File.Delete(Path.Combine(outputDirectory, from));
		foreach (var group in moves.GroupBy(m => m.To))
		{
			var files = group.Select(m => m.From).Distinct().Select(f => contents[f]).ToList();
			foreach (var (path, text) in Merge(group.Key, files))
			{
				string target = Path.Combine(outputDirectory, path);
				Directory.CreateDirectory(Path.GetDirectoryName(target)!);
				File.WriteAllText(target, text);
			}
		}
		DeleteEmptyDirectories(outputDirectory);
	}

	/// <summary>
	/// Combines decompiled files into the single original file when they share the same file-scoped
	/// namespace; files that cannot be combined keep their own file next to it.
	/// </summary>
	static IEnumerable<(string Path, string Text)> Merge(string target, List<string> files)
	{
		var parsed = files.Select(Parse).ToList();
		var mergeable = parsed.Where(p => p.Namespace == parsed[0].Namespace).ToList();
		var usings = mergeable.SelectMany(p => p.Usings).Distinct().ToList();
		string body = string.Join("\n\n", mergeable.Select(p => p.Body.Trim('\n')));
		string header = usings.Count > 0 ? string.Join("\n", usings) + "\n\n" : "";
		string ns = parsed[0].Namespace != null ? parsed[0].Namespace + "\n\n" : "";
		yield return (target, header + ns + body + "\n");

		string directory = Path.GetDirectoryName(target) ?? "";
		foreach (var other in parsed.Except(mergeable))
			yield return (Path.Combine(directory, other.FallbackName), other.Text);
	}

	sealed record ParsedFile(string Text, List<string> Usings, string? Namespace, string Body, string FallbackName);

	static ParsedFile Parse(string text)
	{
		var lines = text.Replace("\r\n", "\n").Split('\n');
		var usings = new List<string>();
		string? ns = null;
		int i = 0;
		for (; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (line.StartsWith("using ") && line.EndsWith(';'))
				usings.Add(line);
			else if (ns == null && line.StartsWith("namespace ") && line.EndsWith(';'))
				ns = line;
			else if (line.Length > 0)
				break;
		}
		string body = string.Join("\n", lines[i..]);
		var declared = System.Text.RegularExpressions.Regex.Match(body, @"\b(?:class|record|struct|interface|enum|delegate\s+\S+)\s+(\w+)");
		string fallback = (declared.Success ? declared.Groups[1].Value : "Types") + ".cs";
		return new ParsedFile(text, usings, ns, body, fallback);
	}

	static void DeleteEmptyDirectories(string root)
	{
		foreach (string dir in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
		{
			if (!Directory.EnumerateFileSystemEntries(dir).Any())
				Directory.Delete(dir);
		}
	}
}
