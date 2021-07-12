// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Text.RegularExpressions;

namespace ICSharpCode.ILSpy.Search
{
	struct SearchRequest
	{
		public SearchMode Mode;
		public AssemblySearchKind AssemblySearchKind;
		public MemberSearchKind MemberSearchKind;
		public string[] Keywords;
		public Regex RegEx;
		public bool FullNameSearch;
		public bool OmitGenerics;
		public string InNamespace;
		public string InAssembly;

		public static SearchRequest FromInput(string input, SearchMode searchMode)
		{
			string[] parts = NativeMethods.CommandLineToArgumentArray(input);

			SearchRequest request = new SearchRequest();
			List<string> keywords = new List<string>();
			Regex regex = null;
			request.Mode = searchMode;

			foreach (string part in parts)
			{
				// Parse: [prefix:|@]["]searchTerm["]
				// Find quotes used for escaping
				int prefixLength = part.IndexOfAny(new[] { '"', '/' });
				if (prefixLength < 0)
				{
					// no quotes
					prefixLength = part.Length;
				}

				// Find end of prefix
				if (part.StartsWith("@", StringComparison.Ordinal))
				{
					prefixLength = 1;
				}
				else
				{
					prefixLength = part.IndexOf(':', 0, prefixLength);
				}
				string prefix;
				if (prefixLength <= 0)
				{
					prefix = null;
					prefixLength = -1;
				}
				else
				{
					prefix = part.Substring(0, prefixLength);
				}

				if (part.Length == prefixLength + 1)
				{
					continue;
				}

				// unescape quotes
				string searchTerm = string.Join(" ", NativeMethods.CommandLineToArgumentArray(part.Substring(prefixLength + 1).Trim()));

				if (prefix == null)
				{
					if (regex == null && searchTerm.StartsWith("/", StringComparison.Ordinal) && searchTerm.Length > 1)
					{
						int searchTermLength = searchTerm.Length - 1;
						if (searchTerm.EndsWith("/", StringComparison.Ordinal))
						{
							searchTermLength--;
						}

						request.FullNameSearch |= searchTerm.Contains("\\.");
						request.OmitGenerics |= !searchTerm.Contains("<");

						regex = CreateRegex(searchTerm.Substring(1, searchTermLength));
					}
					else
					{
						request.FullNameSearch |= searchTerm.Contains(".");
						request.OmitGenerics |= !searchTerm.Contains("<");
						keywords.Add(searchTerm);
					}
				}
				else
				{
					switch (prefix.ToUpperInvariant())
					{
						case "@":
							request.Mode = SearchMode.Token;
							break;
						case "INNAMESPACE":
							request.InNamespace ??= searchTerm;
							break;
						case "INASSEMBLY":
							request.InAssembly ??= searchTerm;
							break;
						case "A":
							request.AssemblySearchKind = AssemblySearchKind.NameOrFileName;
							request.Mode = SearchMode.Assembly;
							break;
						case "AF":
							request.AssemblySearchKind = AssemblySearchKind.FilePath;
							request.Mode = SearchMode.Assembly;
							break;
						case "AN":
							request.AssemblySearchKind = AssemblySearchKind.FullName;
							request.Mode = SearchMode.Assembly;
							break;
						case "N":
							request.Mode = SearchMode.Namespace;
							break;
						case "TM":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.All;
							break;
						case "T":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Type;
							break;
						case "M":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Member;
							break;
						case "MD":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Method;
							break;
						case "F":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Field;
							break;
						case "P":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Property;
							break;
						case "E":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Event;
							break;
						case "C":
							request.Mode = SearchMode.Literal;
							break;
						case "R":
							request.Mode = SearchMode.Resource;
							break;
					}
				}
			}

			Regex CreateRegex(string s)
			{
				try
				{
					return new Regex(s, RegexOptions.Compiled);
				}
				catch (ArgumentException)
				{
					return null;
				}
			}

			request.Keywords = keywords.ToArray();
			request.RegEx = regex;

			return request;
		}

	}
}
