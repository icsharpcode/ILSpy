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

/// <summary>Converts the absolute source paths recorded in a PDB into paths relative to the original project directory.</summary>
public static class ProjectPaths
{
	/// <summary>True for files the SDK generated under an <c>obj</c> directory.</summary>
	public static bool IsGenerated(string originalPath)
	{
		var segments = Split(originalPath);
		return segments.Take(segments.Length - 1).Any(s => s.Equals("obj", StringComparison.OrdinalIgnoreCase));
	}

	/// <summary>
	/// Maps each path to its location relative to the deepest directory containing all of them,
	/// which is the project directory when the paths are the project's own source files.
	/// </summary>
	public static IReadOnlyDictionary<string, string> ToProjectRelative(IEnumerable<string> originalPaths)
	{
		var paths = originalPaths.Distinct().Select(p => (Original: p, Segments: Split(p))).ToList();
		if (paths.Count == 0)
			return new Dictionary<string, string>();
		int common = paths[0].Segments.Length - 1;
		foreach (var (_, segments) in paths)
		{
			int i = 0;
			while (i < common && i < segments.Length - 1 && segments[i] == paths[0].Segments[i])
				i++;
			common = i;
		}
		return paths.ToDictionary(p => p.Original, p => string.Join(Path.DirectorySeparatorChar, p.Segments[common..]));
	}

	static string[] Split(string path) => path.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
}
