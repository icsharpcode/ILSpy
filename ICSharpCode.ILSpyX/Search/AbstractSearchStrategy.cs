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
using System.Collections.Generic;
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

	public struct SearchRequest
	{
		public DecompilerSettings DecompilerSettings;
		public ITreeNodeFactory TreeNodeFactory;
		public ISearchResultFactory SearchResultFactory;
		public SearchMode Mode;
		public AssemblySearchKind AssemblySearchKind;
		public MemberSearchKind MemberSearchKind;
		public string[] Keywords;
		public Regex? RegEx;
		public bool FullNameSearch;
		public bool OmitGenerics;
		// When set, CheckVisibility bypasses the api-visibility filter so private /
		// compiler-generated entities (state-machines, display classes, anonymous
		// closures — anything with `<...>` segments in its metadata name) become
		// findable. Set by the query parser when the user types `<` or `>` in
		// their search term: those characters are characteristically present in
		// compiler-generated names and rare in everyday API names, so they're a
		// reliable signal that the user wants the visibility filter relaxed.
		public bool IncludePrivateApi;
		public string InNamespace;
		public string InAssembly;
	}

	public abstract class AbstractSearchStrategy
	{
		enum TermOperator
		{
			Contains,
			NotContains,
			Exact,
			Fuzzy
		}

		readonly struct PreparedTerm
		{
			// For Exact this is the unstripped term (including the '=' prefix), because the
			// comparison below works with an offset of 1 and the full term length; for Fuzzy
			// it is the stripped term lowered once with ToLowerInvariant; for the others it
			// is the term with any '+'/'-' prefix stripped.
			public readonly string Text;
			public readonly TermOperator Operator;

			public PreparedTerm(TermOperator op, string text)
			{
				this.Operator = op;
				this.Text = text;
			}
		}

		protected readonly string[] searchTerm;
		protected readonly Regex? regex;
		protected readonly bool fullNameSearch;
		protected readonly bool omitGenerics;
		protected readonly SearchRequest searchRequest;
		private readonly IProducerConsumerCollection<SearchResult> resultQueue;
		// The search terms are invariant for the lifetime of a strategy (each keystroke
		// creates a new request + strategy), so prefix stripping and lowercasing are done
		// once here instead of per candidate name in IsMatch.
		private readonly PreparedTerm[] preparedTerms;

		protected AbstractSearchStrategy(SearchRequest request, IProducerConsumerCollection<SearchResult> resultQueue)
		{
			this.resultQueue = resultQueue;
			this.searchTerm = request.Keywords;
			this.regex = request.RegEx;
			this.searchRequest = request;
			this.fullNameSearch = request.FullNameSearch;
			this.omitGenerics = request.OmitGenerics;
			this.preparedTerms = PrepareTerms(request.Keywords);
		}

		static PreparedTerm[] PrepareTerms(string[] keywords)
		{
			var result = new List<PreparedTerm>(keywords.Length);
			foreach (string term in keywords)
			{
				if (string.IsNullOrEmpty(term))
					continue;
				switch (term[0])
				{
					case '+': // must contain
						result.Add(new PreparedTerm(TermOperator.Contains, term.Substring(1)));
						break;
					case '-': // should not contain
						if (term.Length > 1)
							result.Add(new PreparedTerm(TermOperator.NotContains, term.Substring(1)));
						break;
					case '=': // exact match
						if (term.Length > 1)
							result.Add(new PreparedTerm(TermOperator.Exact, term));
						break;
					case '~':
						if (term.Length > 1)
							result.Add(new PreparedTerm(TermOperator.Fuzzy, term.Substring(1).ToLowerInvariant()));
						break;
					default:
						result.Add(new PreparedTerm(TermOperator.Contains, term));
						break;
				}
			}
			return result.ToArray();
		}

		public abstract void Search(MetadataFile module, CancellationToken cancellationToken);

		protected virtual bool IsMatch(string name)
		{
			if (regex != null)
			{
				return regex.IsMatch(name);
			}

			foreach (var term in preparedTerms)
			{
				// How to handle overlapping matches?
				switch (term.Operator)
				{
					case TermOperator.NotContains:
						if (name.IndexOf(term.Text, StringComparison.OrdinalIgnoreCase) >= 0)
							return false;
						break;
					case TermOperator.Exact:
					{
						var equalCompareLength = name.IndexOf('`');
						if (equalCompareLength == -1)
							equalCompareLength = name.Length;

						if (String.Compare(term.Text, 1, name, 0, Math.Max(term.Text.Length, equalCompareLength),
							StringComparison.OrdinalIgnoreCase) != 0)
							return false;
					}
					break;
					case TermOperator.Fuzzy:
						if (!IsNoncontiguousMatch(name, term.Text))
							return false;
						break;
					default:
						if (name.IndexOf(term.Text, StringComparison.OrdinalIgnoreCase) < 0)
							return false;
						break;
				}
			}
			return true;
		}

		static bool IsNoncontiguousMatch(ReadOnlySpan<char> text, ReadOnlySpan<char> loweredSearchTerm)
		{
			if (text.IsEmpty || loweredSearchTerm.IsEmpty)
			{
				return false;
			}
			var textLength = text.Length;
			if (loweredSearchTerm.Length > textLength)
			{
				return false;
			}
			var i = 0;
			for (int searchIndex = 0; searchIndex < loweredSearchTerm.Length;)
			{
				while (i != textLength)
				{
					if (char.ToLowerInvariant(text[i]) == loweredSearchTerm[searchIndex])
					{
						// Check if all characters in searchTerm have been matched
						if (loweredSearchTerm.Length == ++searchIndex)
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
