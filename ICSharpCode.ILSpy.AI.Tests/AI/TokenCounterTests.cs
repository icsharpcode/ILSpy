// Copyright (c) 2026 Dr. Masroor Ehsan

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.AI.Tests.AI
{
	[TestFixture]
	public class TokenCounterTests
	{
		[Test]
		public void CountTokens_NullAndEmpty_ReturnZero()
		{
			TokenCounter.CountTokens(null).Should().Be(0);
			TokenCounter.CountTokens(string.Empty).Should().Be(0);
		}

		[Test]
		public void CountTokens_UsesDifferentHeuristicsForCodeAndProse()
		{
			var text = new string('a', 120);

			TokenCounter.CountTokens(text, isCode: true).Should().Be(41);
			TokenCounter.CountTokens(text, isCode: false).Should().Be(31);
		}

		[Test]
		public void TruncateToTokenBudget_ReturnsOriginalWhenItFits()
		{
			TokenCounter.TruncateToTokenBudget("short", 1000).Should().Be("short");
		}

		[Test]
		public void TruncateToTokenBudget_IncludesSuffixWithinBudget()
		{
			var text = "line one\nline two\nline three\nline four";

			var result = TokenCounter.TruncateToTokenBudget(text, 5);

			result.Should().EndWith("...");
			result.Should().Contain("line one");
			TokenCounter.CountTokens(result).Should().BeLessThanOrEqualTo(5);
		}

		[Test]
		public void TruncateToTokenBudget_ZeroBudgetReturnsEmpty()
		{
			TokenCounter.TruncateToTokenBudget("some text", 0).Should().BeEmpty();
			TokenCounter.TruncateToTokenBudget("some text", -1).Should().BeEmpty();
		}

		[Test]
		public void TruncateToTokenBudget_NullReturnsEmpty()
		{
			TokenCounter.TruncateToTokenBudget(null, 10).Should().BeEmpty();
		}

		[Test]
		public void TruncateToTokenBudget_DoesNotSplitUnicodeSurrogatePair()
		{
			var text = "😀😀😀😀😀";

			var result = TokenCounter.TruncateToTokenBudget(text, 2);

			result.Should().NotContain("\uFFFD");
			for (int i = 0; i < result.Length; i++)
			{
				char c = result[i];
				if (char.IsHighSurrogate(c))
					i.Should().BeLessThan(result.Length - 1);
				if (char.IsLowSurrogate(c))
					i.Should().BeGreaterThan(0);
			}
			TokenCounter.CountTokens(result).Should().BeLessThanOrEqualTo(2);
		}
	}
}
