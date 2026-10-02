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
using System.IO;

#nullable enable

namespace ICSharpCode.ILSpyX.Symbols
{
	/// <summary>
	/// One location of a <see cref="SymbolPath"/>: an HTTP(S) symbol server, or a local/UNC
	/// directory laid out either flat (<c>dir/name.pdb</c>) or as a symbol store
	/// (<c>dir/name.pdb/id/name.pdb</c>).
	/// </summary>
	/// <param name="CacheDirectory">Where files downloaded from an HTTP server are stored; always
	/// set for HTTP locations, <c>null</c> for local ones.</param>
	public sealed record SymbolPathElement(string Location, bool IsHttp, string? CacheDirectory);

	/// <summary>
	/// A symbol search path in the debugger's <c>_NT_SYMBOL_PATH</c> syntax: <c>;</c>-separated
	/// elements, each a plain directory, <c>srv*[cache*]server</c>,
	/// <c>symsrv*symsrv.dll*[cache*]server</c>, or <c>cache*[dir]</c> (a cache for the HTTP
	/// servers that follow it).
	/// </summary>
	public sealed class SymbolPath
	{
		public const string MicrosoftSymbolServer = "https://msdl.microsoft.com/download/symbols";
		public const string NuGetSymbolServer = "https://symbols.nuget.org/download/symbols";
		public const string DefaultSymbolPath = "srv*" + MicrosoftSymbolServer + ";srv*" + NuGetSymbolServer;

		public static string DefaultCacheDirectory => Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData,
				Environment.SpecialFolderOption.DoNotVerify),
			"ILSpy", "SymbolCache");

		public IReadOnlyList<SymbolPathElement> Elements { get; }

		SymbolPath(IReadOnlyList<SymbolPathElement> elements)
		{
			Elements = elements;
		}

		/// <param name="defaultCacheDirectory">The cache for HTTP servers not preceded by an explicit
		/// cache; defaults to <see cref="DefaultCacheDirectory"/>.</param>
		public static SymbolPath Parse(string? text, string? defaultCacheDirectory = null)
		{
			string defaultCache = string.IsNullOrWhiteSpace(defaultCacheDirectory)
				? DefaultCacheDirectory : defaultCacheDirectory;
			var elements = new List<SymbolPathElement>();
			string? currentCache = null;
			foreach (var rawElement in (text ?? "").Split(';'))
			{
				string element = rawElement.Trim();
				if (element.Length == 0)
					continue;
				string[] parts = element.Split('*');
				string head = parts[0].Trim();
				if (parts.Length > 1 && head.Equals("cache", StringComparison.OrdinalIgnoreCase))
				{
					currentCache = NonEmpty(parts[1]) ?? defaultCache;
					continue;
				}
				int first;
				if (parts.Length > 1 && head.Equals("srv", StringComparison.OrdinalIgnoreCase))
					first = 1;
				else if (parts.Length > 2 && head.Equals("symsrv", StringComparison.OrdinalIgnoreCase))
					first = 2;
				else
				{
					elements.Add(Create(element, currentCache ?? defaultCache));
					continue;
				}
				// srv*a*b*server: the last part is the upstream server, the first of the others is
				// the downstream store it is cached in (an empty one means the default cache).
				string? server = NonEmpty(parts[parts.Length - 1]);
				if (server == null)
					continue;
				string? cache = parts.Length - first > 1 ? NonEmpty(parts[first]) ?? defaultCache : null;
				elements.Add(Create(server, cache ?? currentCache ?? defaultCache));
			}
			return new SymbolPath(elements);
		}

		static SymbolPathElement Create(string location, string? cache)
		{
			bool isHttp = location.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				|| location.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
			return new SymbolPathElement(location, isHttp, isHttp ? cache : null);
		}

		static string? NonEmpty(string s)
		{
			s = s.Trim();
			return s.Length == 0 ? null : s;
		}
	}
}
