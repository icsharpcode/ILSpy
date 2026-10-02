// Copyright (c) 2026 Piero Viano
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

#nullable enable

namespace ICSharpCode.ILSpyX.Symbols
{
	/// <summary>
	/// The Source Link document map of a PDB
	/// (https://github.com/dotnet/designs/blob/main/accepted/2020/diagnostics/source-link.md):
	/// maps build-time document paths to URLs. A key is either an exact path, or a path prefix
	/// ending in <c>*</c> whose value has one <c>*</c> that receives the rest of the path with
	/// <c>\</c> turned into <c>/</c>. Exact matches win; otherwise the longest prefix wins.
	/// </summary>
	public sealed class SourceLinkMap
	{
		readonly List<(string Path, string Uri, bool IsPrefix)> entries;

		SourceLinkMap(List<(string, string, bool)> entries)
		{
			this.entries = entries;
		}

		/// <returns>The map, or <c>null</c> if <paramref name="json"/> is not a valid Source Link document.</returns>
		public static SourceLinkMap? Parse(string json)
		{
			try
			{
				using var document = JsonDocument.Parse(json);
				if (document.RootElement.ValueKind != JsonValueKind.Object
					|| !document.RootElement.TryGetProperty("documents", out var documents)
					|| documents.ValueKind != JsonValueKind.Object)
					return null;
				var entries = new List<(string, string, bool)>();
				foreach (var property in documents.EnumerateObject())
				{
					if (property.Value.ValueKind != JsonValueKind.String)
						return null;
					string path = property.Name;
					string uri = property.Value.GetString()!;
					bool isPrefix = path.EndsWith("*", StringComparison.Ordinal);
					if (isPrefix)
					{
						if (uri.Count(c => c == '*') != 1)
							return null;
						path = path.Substring(0, path.Length - 1);
					}
					else if (uri.Contains('*'))
					{
						return null;
					}
					entries.Add((path, uri, isPrefix));
				}
				return new SourceLinkMap(entries);
			}
			catch (JsonException)
			{
				return null;
			}
		}

		/// <returns>The URL of <paramref name="documentPath"/>, or <c>null</c> if the map does not cover it.</returns>
		public string? GetUri(string documentPath)
		{
			foreach (var entry in entries)
			{
				if (!entry.IsPrefix && string.Equals(entry.Path, documentPath, StringComparison.OrdinalIgnoreCase))
					return entry.Uri;
			}
			(string Path, string Uri, bool IsPrefix)? best = null;
			foreach (var entry in entries)
			{
				if (entry.IsPrefix && documentPath.StartsWith(entry.Path, StringComparison.OrdinalIgnoreCase)
					&& (best == null || entry.Path.Length > best.Value.Path.Length))
					best = entry;
			}
			if (best == null)
				return null;
			string rest = documentPath.Substring(best.Value.Path.Length).Replace('\\', '/');
			return best.Value.Uri.Replace("*", string.Join("/", rest.Split('/').Select(Uri.EscapeDataString)));
		}
	}
}
