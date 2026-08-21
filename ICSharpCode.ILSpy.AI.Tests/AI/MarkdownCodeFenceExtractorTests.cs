// Copyright (c) 2026 Dr. Masroor Ehsan

using ICSharpCode.ILSpy.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.AI.Tests.AI
{
	[TestFixture]
	public class MarkdownCodeFenceExtractorTests
	{
		const string SampleMarkdown = "# Heading\n\nSome prose.\n\n```csharp\npublic class Greeter { }\n```\n\nMore prose.\n\n```il\n.method public void M() { }\n```\n\n```\nvar untagged = 1;\n```\n";

		[Test]
		public void Extract_FindsAllFences_InDocumentOrder()
		{
			var fences = MarkdownCodeFenceExtractor.Extract(SampleMarkdown);
			Assert.That(fences, Has.Count.EqualTo(3));
			Assert.That(fences[0].Language, Is.EqualTo("csharp"));
			Assert.That(fences[1].Language, Is.EqualTo("il"));
		}

		[Test]
		public void Extract_ExcludesFenceMarkersFromCode()
		{
			var fences = MarkdownCodeFenceExtractor.Extract(SampleMarkdown);
			Assert.That(fences[0].Code, Is.EqualTo("public class Greeter { }"));
			Assert.That(fences[0].Code, Does.Not.Contain("```"));
			Assert.That(fences[1].Code, Is.EqualTo(".method public void M() { }"));
		}

		[Test]
		public void Extract_UntaggedFenceHasNullLanguage()
		{
			var fences = MarkdownCodeFenceExtractor.Extract(SampleMarkdown);
			Assert.That(fences[2].Language, Is.Null);
		}

		[Test]
		public void Extract_ReportsLineSpansContainingContent()
		{
			var fences = MarkdownCodeFenceExtractor.Extract(SampleMarkdown);
			var first = fences[0];
			Assert.That(first.StartLine, Is.GreaterThanOrEqualTo(0));
			// The code content line sits between the opening fence and the (exclusive) end.
			Assert.That(first.StartLine + 1, Is.LessThan(first.EndLine));
			Assert.That(first.EndLine, Is.GreaterThan(first.StartLine));
		}

		[Test]
		public void IsCSharp_AcceptsCommonTags_CaseInsensitively()
		{
			var extractor = (string lang) => new MarkdownCodeFenceExtractor.CodeFence { Language = lang };
			Assert.That(extractor("csharp").IsCSharp, Is.True);
			Assert.That(extractor("CSharp").IsCSharp, Is.True);
			Assert.That(extractor("cs").IsCSharp, Is.True);
			Assert.That(extractor("c#").IsCSharp, Is.True);
			Assert.That(extractor("il").IsCSharp, Is.False);
			Assert.That(extractor("").IsCSharp, Is.False);
		}

		[Test]
		public void IsIL_MatchesIlTag()
		{
			Assert.That(new MarkdownCodeFenceExtractor.CodeFence { Language = "il" }.IsIL, Is.True);
			Assert.That(new MarkdownCodeFenceExtractor.CodeFence { Language = "IL" }.IsIL, Is.True);
			Assert.That(new MarkdownCodeFenceExtractor.CodeFence { Language = "csharp" }.IsIL, Is.False);
		}

		[Test]
		public void ExtractCSharpFences_OnlyReturnsCSharp()
		{
			var fences = MarkdownCodeFenceExtractor.ExtractCSharpFences(SampleMarkdown);
			Assert.That(fences, Has.Count.EqualTo(1));
			Assert.That(fences[0].Language, Is.EqualTo("csharp"));
		}

		[Test]
		public void ExtractILFences_OnlyReturnsIL()
		{
			var fences = MarkdownCodeFenceExtractor.ExtractILFences(SampleMarkdown);
			Assert.That(fences, Has.Count.EqualTo(1));
			Assert.That(fences[0].Language, Is.EqualTo("il"));
		}

		[Test]
		public void Extract_NullAndEmptyReturnsEmptyList()
		{
			Assert.That(MarkdownCodeFenceExtractor.Extract(null), Is.Empty);
			Assert.That(MarkdownCodeFenceExtractor.Extract(string.Empty), Is.Empty);
			Assert.That(MarkdownCodeFenceExtractor.Extract("Just prose, no fences."), Is.Empty);
		}

		[Test]
		public void FindFenceAtLine_ReturnsOwningFence()
		{
			var fences = MarkdownCodeFenceExtractor.Extract(SampleMarkdown);
			var first = fences[0];
			// A caret on a code content line inside the C# fence resolves to that fence.
			var found = MarkdownCodeFenceExtractor.FindFenceAtLine(SampleMarkdown, first.StartLine + 1);
			Assert.That(found, Is.Not.Null);
			Assert.That(found!.Language, Is.EqualTo("csharp"));
			// A line before the first fence resolves to nothing.
			Assert.That(MarkdownCodeFenceExtractor.FindFenceAtLine(SampleMarkdown, 0), Is.Null);
		}
	}
}
