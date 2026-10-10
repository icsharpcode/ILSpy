// Copyright (c) 2026 Christoph Wille
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

using System.Collections.Concurrent;
using System.Threading;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// Pins the term-matching semantics of <see cref="AbstractSearchStrategy.IsMatch"/>:
/// plain containment, the +/-/=/~ operators, multi-term conjunction, and the
/// handling of degenerate terms.
/// </summary>
[TestFixture]
public class IsMatchTests
{
	sealed class ExposingSearchStrategy : AbstractSearchStrategy
	{
		public ExposingSearchStrategy(params string[] keywords)
			: base(new SearchRequest { Keywords = keywords }, new ConcurrentQueue<SearchResult>())
		{
		}

		public bool Match(string name) => IsMatch(name);

		public override void Search(MetadataFile module, CancellationToken cancellationToken)
		{
		}
	}

	static bool IsMatch(string name, params string[] keywords)
		=> new ExposingSearchStrategy(keywords).Match(name);

	[Test]
	public void Plain_Term_Matches_Substring_Ignoring_Case()
	{
		Assert.That(IsMatch("StringBuilder", "builder"), Is.True);
		Assert.That(IsMatch("StringBuilder", "STRING"), Is.True);
		Assert.That(IsMatch("StringBuilder", "Comparer"), Is.False);
	}

	[Test]
	public void Plus_Operator_Requires_The_Term_To_Be_Contained()
	{
		Assert.That(IsMatch("Enumerable", "+Enum"), Is.True);
		Assert.That(IsMatch("Enumerable", "+enumera"), Is.True);
		Assert.That(IsMatch("List", "+Enum"), Is.False);
	}

	[Test]
	public void Minus_Operator_Excludes_Names_Containing_The_Term()
	{
		Assert.That(IsMatch("StringBuilder", "-Builder"), Is.False);
		Assert.That(IsMatch("StringComparer", "-Builder"), Is.True);
	}

	[Test]
	public void Equals_Operator_Requires_Exact_Name_Match()
	{
		Assert.That(IsMatch("String", "=String"), Is.True);
		Assert.That(IsMatch("String", "=string"), Is.True);
		Assert.That(IsMatch("StringBuilder", "=String"), Is.False);
	}

	[Test]
	public void Equals_Operator_Compares_Against_The_Backtick_Suffixed_Name()
	{
		// The compare window is max(term length incl. '=', chars before '`'), so a
		// generic type only matches when the term spells out the arity suffix too.
		Assert.That(IsMatch("List`1", "=List`1"), Is.True);
		Assert.That(IsMatch("List`1", "=List"), Is.False);
		Assert.That(IsMatch("List`1", "=Dictionary"), Is.False);
	}

	[Test]
	public void Fuzzy_Operator_Matches_Noncontiguous_Character_Sequences()
	{
		Assert.That(IsMatch("StringBuilder", "~sb"), Is.True);
		Assert.That(IsMatch("StringBuilder", "~strbld"), Is.True);
		Assert.That(IsMatch("StringBuilder", "~xyz"), Is.False);
		// Characters must appear in order: 'b' never precedes 's'.
		Assert.That(IsMatch("StringBuilder", "~bs"), Is.False);
	}

	[Test]
	public void Fuzzy_Operator_Ignores_Case_On_Both_Sides()
	{
		Assert.That(IsMatch("StringBuilder", "~SB"), Is.True);
		Assert.That(IsMatch("stringbuilder", "~STRB"), Is.True);
	}

	[Test]
	public void Fuzzy_Term_Longer_Than_The_Name_Never_Matches()
	{
		Assert.That(IsMatch("Ab", "~abc"), Is.False);
	}

	[Test]
	public void Multiple_Terms_Are_A_Conjunction()
	{
		Assert.That(IsMatch("StringBuilder", "String", "Builder"), Is.True);
		Assert.That(IsMatch("StringBuilder", "String", "-Builder"), Is.False);
		Assert.That(IsMatch("StringComparer", "String", "-Builder"), Is.True);
	}

	[Test]
	public void Degenerate_Terms_Match_Everything()
	{
		// An empty term is skipped; a bare operator has no payload to test.
		Assert.That(IsMatch("Anything", ""), Is.True);
		Assert.That(IsMatch("Anything", "~"), Is.True);
		Assert.That(IsMatch("Anything", "-"), Is.True);
	}
}
