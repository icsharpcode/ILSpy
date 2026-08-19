// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class AIProfileTests
	{
		[Test]
		public void Create_GeneratesStableIdAndDefaults()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));

			profile.Id.Should().NotBeNullOrWhiteSpace();
			profile.ProviderType.Should().Be("openai");
			profile.BaseUrl.Should().Be("https://api.openai.com");
			profile.Models.Should().Equal("gpt-4o");
			profile.LastSelectedModel.Should().Be("gpt-4o");
			profile.HasStoredKey.Should().BeFalse();
		}

		[Test]
		public void Create_IdsAreUnique()
		{
			var first = AIProfile.Create(AIProviderCatalog.Get("openai"));
			var second = AIProfile.Create(AIProviderCatalog.Get("openai"));

			first.Id.Should().NotBe(second.Id);
		}

		[Test]
		public void Clone_PreservesIdAndContent()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("anthropic"));
			profile.Name = "Work";
			profile.Models.Add("claude-sonnet");
			profile.LastSelectedModel = "claude-sonnet";
			profile.HasStoredKey = true;

			var clone = profile.Clone();

			clone.Id.Should().Be(profile.Id);
			clone.Name.Should().Be("Work");
			clone.Models.Should().Equal(profile.Models);
			clone.LastSelectedModel.Should().Be("claude-sonnet");
			clone.HasStoredKey.Should().BeTrue();
		}

		[Test]
		public void Clone_IsolatesModelEdits()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			var clone = profile.Clone();

			clone.Models[0] = "changed";
			clone.Models.Add("second");

			profile.Models.Should().Equal("gpt-4o");
		}

		[Test]
		public void Duplicate_GetsNewIdAndClearsSecret()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.HasStoredKey = true;

			var duplicate = profile.Duplicate();

			duplicate.Id.Should().NotBe(profile.Id);
			duplicate.HasStoredKey.Should().BeFalse();
			duplicate.Models.Should().Equal(profile.Models);
			duplicate.BaseUrl.Should().Be(profile.BaseUrl);
		}

		[Test]
		public void Validate_AcceptsWellFormedProfile()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Name = "Default";

			profile.Validate().Should().BeEmpty();
		}

		[Test]
		public void Validate_RejectsBlankName()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Name = "  ";

			profile.Validate().Should().Contain(error => error.Contains("name", StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void Validate_RejectsUnsupportedProviderType()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.ProviderType = "made-up";

			profile.Validate().Should().Contain(error => error.Contains("provider", StringComparison.OrdinalIgnoreCase));
		}

		[TestCase("not-a-uri")]
		[TestCase("ftp://example.com")]
		[TestCase("api.openai.com")]
		public void Validate_RejectsNonAbsoluteOrNonHttpEndpoints(string endpoint)
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.BaseUrl = endpoint;

			profile.Validate().Should().Contain(error => error.Contains("endpoint", StringComparison.OrdinalIgnoreCase));
		}

		[TestCase("https://api.openai.com")]
		[TestCase("http://localhost:11434")]
		public void Validate_AcceptsAbsoluteHttpEndpoints(string endpoint)
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Name = "Default";
			profile.BaseUrl = endpoint;

			profile.Validate().Should().BeEmpty();
		}

		[Test]
		public void Validate_RequiresAtLeastOneModel()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Models.Clear();

			profile.Validate().Should().Contain(error => error.Contains("model", StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void Validate_RejectsDuplicateModelsCaseInsensitively()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Models.Add("GPT-4O");

			profile.Validate().Should().Contain(error => error.Contains("model", StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void Validate_RejectsBlankModel()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Models.Add(" ");

			profile.Validate().Should().Contain(error => error.Contains("model", StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void TrimNormalizesNameEndpointAndModels()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Name = "  Work  ";
			profile.BaseUrl = "  https://api.openai.com  ";
			profile.Models[0] = "  gpt-4o  ";

			profile.Normalize();

			profile.Name.Should().Be("Work");
			profile.BaseUrl.Should().Be("https://api.openai.com");
			profile.Models.Should().Equal("gpt-4o");
		}

		[Test]
		public void ResolveModel_RestoresRememberedModelWhenValid()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.Models.Add("gpt-4o-mini");
			profile.LastSelectedModel = "gpt-4o-mini";

			profile.ResolveModel().Should().Be("gpt-4o-mini");
		}

		[Test]
		public void ResolveModel_FallsBackToFirstModelWhenRememberedIsGone()
		{
			var profile = AIProfile.Create(AIProviderCatalog.Get("openai"));
			profile.LastSelectedModel = "no-longer-present";

			profile.ResolveModel().Should().Be(profile.Models[0]);
		}
	}
}
