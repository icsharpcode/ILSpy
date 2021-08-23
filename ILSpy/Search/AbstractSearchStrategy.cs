using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Search
{
	abstract class AbstractSearchStrategy
	{
		protected readonly string[] searchTerm;
		protected readonly Regex regex;
		protected readonly bool fullNameSearch;
		protected readonly bool omitGenerics;
		private readonly IProducerConsumerCollection<SearchResult> resultQueue;

		protected AbstractSearchStrategy(IProducerConsumerCollection<SearchResult> resultQueue, params string[] terms)
		{
			this.resultQueue = resultQueue;

			if (terms.Length == 1 && terms[0].Length > 2)
			{
				string search = terms[0];
				if (TryParseRegex(search, out regex))
				{
					fullNameSearch = search.Contains("\\.");
					omitGenerics = !search.Contains("<");
				}
				else
				{
					fullNameSearch = search.Contains(".");
					omitGenerics = !search.Contains("<");
				}
			}
			searchTerm = terms;
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

		bool TryParseRegex(string input, out Regex pattern)
		{
			pattern = null;
			if (!input.StartsWith("/", StringComparison.Ordinal))
			{
				return false;
			}
			input = input.Substring(1);
			if (input.EndsWith("/", StringComparison.Ordinal))
			{
				input = input.Remove(input.Length - 1);
			}
			if (string.IsNullOrWhiteSpace(input))
			{
				return false;
			}
			pattern = SafeNewRegex(input);
			return pattern != null;

			static Regex SafeNewRegex(string unsafePattern)
			{
				try
				{
					return new Regex(unsafePattern, RegexOptions.Compiled);
				}
				catch (ArgumentException)
				{
					return null;
				}
			}
		}
	}
}
