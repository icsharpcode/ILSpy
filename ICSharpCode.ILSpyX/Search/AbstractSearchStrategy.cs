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
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Abstractions;

namespace ICSharpCode.ILSpyX.Search
{
	public enum SearchMode
	{
		TypeAndMember,
		Type,
		Member,
		Method,
		Field,
		Property,
		Event,
		Literal,
		Token,
		Resource,
		Assembly,
		Namespace
	}

	struct SearchRequest
	{
		public DecompilerSettings DecompilerSettings;
		public ITreeNodeFactory TreeNodeFactory;
		public ISearchResultFactory SearchResultFactory;
		public SearchMode Mode;
		public AssemblySearchKind AssemblySearchKind;
		public MemberSearchKind MemberSearchKind;
		public string[] Keywords;
		public Regex RegEx;
		public bool FullNameSearch;
		public bool OmitGenerics;
		public string InNamespace;
		public string InAssembly;
	}

	abstract class AbstractSearchStrategy
	{
		protected readonly string[] searchTerm;
		protected readonly Regex regex;
		protected readonly bool fullNameSearch;
		protected readonly bool omitGenerics;
		protected readonly SearchRequest searchRequest;
		private readonly IProducerConsumerCollection<SearchResult> resultQueue;

		protected AbstractSearchStrategy(SearchRequest request, IProducerConsumerCollection<SearchResult> resultQueue)
		{
			this.resultQueue = resultQueue;
			this.searchTerm = request.Keywords;
			this.regex = request.RegEx;
			this.searchRequest = request;
			this.fullNameSearch = request.FullNameSearch;
			this.omitGenerics = request.OmitGenerics;
		}

		public abstract void Search(PEFile module, CancellationToken cancellationToken);

		protected virtual bool IsMatch(string name)
		{
			if (regex != null)
			{
				return regex.IsMatch(name);
			}

			for (int i = 0; i < searchTerm.Length; ++i)
			{
				// How to handle overlapping matches?
				var term = searchTerm[i];
				if (string.IsNullOrEmpty(term))
					continue;
				string text = name;
				switch (term[0])
				{
					case '+': // must contain
						term = term.Substring(1);
						goto default;
					case '-': // should not contain
						if (term.Length > 1 && text.IndexOf(term.Substring(1), StringComparison.OrdinalIgnoreCase) >= 0)
							return false;
						break;
					case '=': // exact match
					{
						var equalCompareLength = text.IndexOf('`');
						if (equalCompareLength == -1)
							equalCompareLength = text.Length;

						if (term.Length > 1 && String.Compare(term, 1, text, 0, Math.Max(term.Length, equalCompareLength),
							StringComparison.OrdinalIgnoreCase) != 0)
							return false;
					}
					break;
					case '~':
						if (term.Length > 1 && !IsNoncontiguousMatch(text.ToLower(), term.Substring(1).ToLower()))
							return false;
						break;
					default:
						if (text.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0)
							return false;
						break;
				}
			}
			return true;
		}

		bool IsNoncontiguousMatch(string text, string searchTerm)
		{
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm))
			{
				return false;
			}
			var textLength = text.Length;
			if (searchTerm.Length > textLength)
			{
				return false;
			}
			var i = 0;
			for (int searchIndex = 0; searchIndex < searchTerm.Length;)
			{
				while (i != textLength)
				{
					if (text[i] == searchTerm[searchIndex])
					{
						// Check if all characters in searchTerm have been matched
						if (searchTerm.Length == ++searchIndex)
							return true;
						i++;
						break;
					}
					i++;
				}
				if (i == textLength)
					return false;
			}
			return false;
		}

		protected void OnFoundResult(SearchResult result)
		{
			resultQueue.TryAdd(result);
		}
	}
}
