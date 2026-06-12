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

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Engine-level tests for the C# bracket searcher. Each test feeds a small document
/// and asserts on the returned <see cref="BracketSearchResult"/>'s offsets — the
/// renderer side is the same code path as caret + IBackgroundRenderer rendering used
/// elsewhere in the editor, so the value of testing here is the language-specific
/// "skip brackets inside strings / comments" semantics.
/// </summary>
[TestFixture]
public class CSharpBracketSearcherTests
{
	// CSharpBracketSearcher is internal; the easiest way to get an instance without
	// making it public is through CSharpLanguage.BracketSearcher (its public-API path).
	static IBracketSearcher Searcher() => new ICSharpCode.ILSpy.Languages.CSharpLanguage().BracketSearcher;

	static TextDocument Doc(string text) => new(text);

	[Test]
	public void Caret_Right_Of_Opening_Paren_Matches_The_Closer()
	{
		// Cursor sits just after '(' at offset 1. SearchBracket scans forward to the
		// matching ')' at offset 4. Returned pair must include both single-char brackets.
		var doc = Doc("a(b+c)d");
		var result = Searcher().SearchBracket(doc, offset: 2);
		result.Should().NotBeNull();
		result!.OpeningBracketOffset.Should().Be(1);
		result.ClosingBracketOffset.Should().Be(5);
	}

	[Test]
	public void Caret_Right_Of_Closing_Brace_Matches_The_Opener()
	{
		// Cursor sits just after '}'. The searcher walks backward to the matching '{'.
		var doc = Doc("class C { int x; }");
		var result = Searcher().SearchBracket(doc, offset: doc.TextLength);
		result.Should().NotBeNull();
		result!.OpeningBracketOffset.Should().Be(8);
		result.ClosingBracketOffset.Should().Be(17);
	}

	[Test]
	public void Brackets_Inside_String_Literals_Are_Ignored()
	{
		// The ')' inside the string literal must NOT close the outer '(' at offset 9.
		// The slow searcher's string-aware parse should walk past it and match the
		// real ')' at the end.
		var doc = Doc("Console.Write(\"hello)world\")");
		var result = Searcher().SearchBracket(doc, offset: 14);
		result.Should().NotBeNull();
		result!.OpeningBracketOffset.Should().Be(13);
		result.ClosingBracketOffset.Should().Be(doc.TextLength - 1);
	}

	[Test]
	public void Brackets_Inside_Line_Comments_Are_Ignored()
	{
		// Closer inside `// no )` is not matched; the real closer at line end is.
		var doc = Doc("f(a) // no )\n");
		var result = Searcher().SearchBracket(doc, offset: 2);
		result.Should().NotBeNull();
		result!.OpeningBracketOffset.Should().Be(1);
		result.ClosingBracketOffset.Should().Be(3);
	}

	[Test]
	public void Returns_Null_When_Caret_Is_Not_On_A_Bracket()
	{
		var doc = Doc("plain text");
		Searcher().SearchBracket(doc, offset: 4).Should().BeNull();
	}

	[Test]
	public void Default_Searcher_Always_Returns_Null()
	{
		// Non-C# languages get this no-op default — confirms the placeholder behaves
		// as the IBracketSearcher contract requires.
		var doc = Doc("(((");
		DefaultBracketSearcher.DefaultInstance.SearchBracket(doc, 1).Should().BeNull();
	}
}
