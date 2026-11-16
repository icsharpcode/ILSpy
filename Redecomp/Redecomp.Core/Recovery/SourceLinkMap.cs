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

using System.Text.Json;

namespace Redecomp.Core.Recovery;

/// <summary>
/// The SourceLink map of a PDB: which URL serves each source file, from the JSON the compiler
/// stores in the PDB (<c>{"documents": {"/src/*": "https://host/repo/commit/*"}}</c>).
/// </summary>
public sealed class SourceLinkMap
{
	readonly List<(string Pattern, string Url)> entries;

	SourceLinkMap(List<(string, string)> entries) => this.entries = entries;

	public static SourceLinkMap Parse(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var entries = doc.RootElement.GetProperty("documents").EnumerateObject()
			.Select(p => (p.Name, p.Value.GetString() ?? ""))
			.ToList();
		return new SourceLinkMap(entries);
	}

	/// <summary>URL serving the document at <paramref name="documentPath"/>, or null when the map does not cover it.</summary>
	/// <remarks>
	/// Follows the SourceLink specification: an entry ending in <c>*</c> matches every path starting
	/// with the text before it, the rest of the path (with <c>/</c> separators) replaces the <c>*</c>
	/// of the URL, and the longest matching entry wins.
	/// </remarks>
	public Uri? Map(string documentPath)
	{
		string? best = null;
		int bestLength = -1;
		foreach (var (pattern, url) in entries)
		{
			if (pattern.EndsWith('*'))
			{
				string prefix = pattern[..^1];
				if (prefix.Length > bestLength && documentPath.StartsWith(prefix, StringComparison.Ordinal))
				{
					string rest = documentPath[prefix.Length..].Replace('\\', '/');
					best = url.Replace("*", rest, StringComparison.Ordinal);
					bestLength = prefix.Length;
				}
			}
			else if (pattern == documentPath && pattern.Length > bestLength)
			{
				best = url;
				bestLength = pattern.Length;
			}
		}
		return best != null && Uri.TryCreate(best, UriKind.Absolute, out var uri) ? uri : null;
	}
}
