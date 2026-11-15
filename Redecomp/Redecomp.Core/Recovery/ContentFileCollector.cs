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

namespace Redecomp.Core.Recovery;

/// <summary>
/// Finds the files deployed next to an assembly that are project content (configuration, static
/// assets, data files) rather than build artifacts such as binaries, symbols and runtime manifests.
/// </summary>
public static class ContentFileCollector
{
	static readonly HashSet<string> BinaryExtensions = new(StringComparer.OrdinalIgnoreCase) {
		".dll", ".exe", ".pdb", ".so", ".dylib", ".a", ".lib",
	};

	/// <summary>Returns the content files as paths relative to the assembly directory.</summary>
	public static IReadOnlyList<string> Collect(string assemblyPath)
	{
		string directory = Path.GetDirectoryName(Path.GetFullPath(assemblyPath))!;
		string name = Path.GetFileNameWithoutExtension(assemblyPath);
		var artifacts = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
			name, // apphost on Unix
			name + ".deps.json",
			name + ".runtimeconfig.json",
			name + ".runtimeconfig.dev.json",
			"createdump",
		};
		return Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
			.Select(f => Path.GetRelativePath(directory, f))
			.Where(f => !BinaryExtensions.Contains(Path.GetExtension(f)))
			.Where(f => !(Path.GetDirectoryName(f) == "" && artifacts.Contains(f)))
			.Where(f => !IsDocumentationOfAnAssembly(directory, f))
			.Order(StringComparer.Ordinal)
			.ToList();
	}

	// XML documentation files sit next to the assembly they document and are read by the decompiler.
	static bool IsDocumentationOfAnAssembly(string directory, string relativePath)
		=> Path.GetExtension(relativePath).Equals(".xml", StringComparison.OrdinalIgnoreCase)
			&& File.Exists(Path.Combine(directory, Path.ChangeExtension(relativePath, ".dll")));
}
