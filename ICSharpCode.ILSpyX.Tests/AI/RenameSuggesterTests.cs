// Copyright (c) 2026 Masroor

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class RenameSuggesterTests
	{
		[TestCase(null, false)]
		[TestCase("", false)]
		[TestCase("a", true)]
		[TestCase("b1", true)]
		[TestCase("method_47", true)]
		[TestCase("class", false)]
		[TestCase("ProcessPayment", false)]
		public void IsLikelyObfuscated_RecognizesShortAndGeneratedNames(string? name, bool expected)
		{
			RenameSuggester.IsLikelyObfuscated(name).Should().Be(expected);
		}

		[Test]
		public void ParseSuggestions_StripsFencesAndDeduplicatesNames()
		{
			string response = "```json\n[\n  {\"name\":\"ProcessPayment\",\"confidence\":0.91,\"reasoning\":\"clear\"},\n  {\"name\":\"ProcessPayment\",\"confidence\":0.5,\"reasoning\":\"duplicate\"},\n  {\"name\":\"class\",\"confidence\":0.1,\"reasoning\":\"keyword\"}\n]\n```";

			var suggestions = RenameSuggester.ParseSuggestions(response);

			suggestions.Should().HaveCount(1);
			suggestions[0].Name.Should().Be("ProcessPayment");
			suggestions[0].Confidence.Should().Be(0.91);
		}

		[Test]
		public void ParseSuggestions_RejectsInvalidJson()
		{
			var action = () => RenameSuggester.ParseSuggestions("not json");

			action.Should().Throw<RenameSuggestionParseException>();
		}
	}
}
