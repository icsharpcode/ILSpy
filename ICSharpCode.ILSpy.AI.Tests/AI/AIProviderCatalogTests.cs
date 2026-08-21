// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.AI.Tests.AI
{
	[TestFixture]
	public class AIProviderCatalogTests
	{
		[Test]
		public void All_KnownProviderIdsArePresent()
		{
			AIProviderCatalog.All.Select(descriptor => descriptor.Id)
				.Should().BeEquivalentTo("openai", "anthropic", "ollama", "custom");
		}

		[TestCase("openai", true)]
		[TestCase("OpenAI", true)]
		[TestCase("ANTHROPIC", true)]
		[TestCase("ollama", true)]
		[TestCase("custom", true)]
		[TestCase("unknown", false)]
		[TestCase("", false)]
		[TestCase(null, false)]
		public void TryGet_ResolvesSupportedIdsCaseInsensitively(string? id, bool expected)
		{
			AIProviderCatalog.TryGet(id, out var descriptor).Should().Be(expected);
			if (expected)
				descriptor.Id.Should().Be(id!.Trim().ToLowerInvariant());
		}

		[Test]
		public void KeyRequirements_MatchCapabilityMatrix()
		{
			AIProviderCatalog.Get("openai").KeyRequirement.Should().Be(AIProviderKeyRequirement.Required);
			AIProviderCatalog.Get("anthropic").KeyRequirement.Should().Be(AIProviderKeyRequirement.Required);
			AIProviderCatalog.Get("ollama").KeyRequirement.Should().Be(AIProviderKeyRequirement.None);
			AIProviderCatalog.Get("custom").KeyRequirement.Should().Be(AIProviderKeyRequirement.Optional);
		}

		[Test]
		public void Implementations_MatchCapabilityMatrix()
		{
			AIProviderCatalog.Get("openai").Implementation.Should().Be(AIProviderImplementation.OpenAICompatible);
			AIProviderCatalog.Get("ollama").Implementation.Should().Be(AIProviderImplementation.OpenAICompatible);
			AIProviderCatalog.Get("custom").Implementation.Should().Be(AIProviderImplementation.OpenAICompatible);
			AIProviderCatalog.Get("anthropic").Implementation.Should().Be(AIProviderImplementation.Anthropic);
		}

		[Test]
		public void Defaults_ProvideAbsoluteEndpointsAndModels()
		{
			foreach (var descriptor in AIProviderCatalog.All)
			{
				descriptor.DefaultBaseUrl.Should().StartWith(descriptor.Id == "ollama" ? "http://" : "https://");
				descriptor.DefaultModel.Should().NotBeNullOrWhiteSpace();
				descriptor.Label.Should().NotBeNullOrWhiteSpace();
			}
		}
	}
}
