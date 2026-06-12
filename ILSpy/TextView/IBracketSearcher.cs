// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using AvaloniaEdit.Document;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>Language-specific bracket pair search. Returns the matched pair around
	/// the caret offset (or <see langword="null"/> when no bracket is at the caret).</summary>
	public interface IBracketSearcher
	{
		/// <summary>Searches for a matching bracket from <paramref name="offset"/> outward.
		/// Returns positions and lengths of both brackets, or <c>null</c> for "no match".</summary>
		BracketSearchResult? SearchBracket(IDocument document, int offset);
	}

	/// <summary>Pair of matched brackets located by <see cref="IBracketSearcher"/>.</summary>
	public sealed class BracketSearchResult
	{
		public int OpeningBracketOffset { get; }
		public int OpeningBracketLength { get; }
		public int ClosingBracketOffset { get; }
		public int ClosingBracketLength { get; }

		public BracketSearchResult(int openingBracketOffset, int openingBracketLength,
			int closingBracketOffset, int closingBracketLength)
		{
			this.OpeningBracketOffset = openingBracketOffset;
			this.OpeningBracketLength = openingBracketLength;
			this.ClosingBracketOffset = closingBracketOffset;
			this.ClosingBracketLength = closingBracketLength;
		}
	}

	/// <summary>No-op searcher used by languages that don't have a richer implementation
	/// (every <see cref="Language"/> defaults to this; only C# overrides today).</summary>
	public sealed class DefaultBracketSearcher : IBracketSearcher
	{
		public static readonly DefaultBracketSearcher DefaultInstance = new();
		public BracketSearchResult? SearchBracket(IDocument document, int offset) => null;
	}
}
