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

using Redecomp.Core.Recovery;

namespace Redecomp.Core.Reconstruction;

/// <summary>
/// Maps embedded source files to paths relative to the original project directory, and separates
/// the files the developer wrote from the ones the SDK generated under <c>obj/</c>.
/// </summary>
public sealed class EmbeddedSourceLayout
{
	public IReadOnlyList<(string RelativePath, byte[] Content)> UserFiles { get; }

	/// <summary>True when the SDK generated a global usings file, i.e. the project had ImplicitUsings enabled.</summary>
	public bool UsedImplicitUsings { get; }

	EmbeddedSourceLayout(IReadOnlyList<(string, byte[])> userFiles, bool usedImplicitUsings)
	{
		UserFiles = userFiles;
		UsedImplicitUsings = usedImplicitUsings;
	}

	public static EmbeddedSourceLayout Create(IReadOnlyList<EmbeddedSourceFile> files)
	{
		var user = new List<(string[] Segments, byte[] Content)>();
		bool implicitUsings = false;
		foreach (var file in files)
		{
			string[] segments = file.OriginalPath.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
			if (IsGenerated(segments))
			{
				if (segments[^1].EndsWith(".GlobalUsings.g.cs", StringComparison.OrdinalIgnoreCase))
					implicitUsings = true;
				continue;
			}
			user.Add((segments, file.Content));
		}
		if (user.Count == 0)
			return new EmbeddedSourceLayout([], implicitUsings);

		// The project directory is the deepest directory containing every user file.
		int common = user[0].Segments.Length - 1;
		foreach (var (segments, _) in user)
		{
			int i = 0;
			while (i < common && i < segments.Length - 1 && segments[i] == user[0].Segments[i])
				i++;
			common = i;
		}
		var mapped = user
			.Select(u => (string.Join(Path.DirectorySeparatorChar, u.Segments[common..]), u.Content))
			.ToList();
		return new EmbeddedSourceLayout(mapped, implicitUsings);
	}

	static bool IsGenerated(string[] segments)
		=> segments.Take(segments.Length - 1).Any(s => s.Equals("obj", StringComparison.OrdinalIgnoreCase));
}
