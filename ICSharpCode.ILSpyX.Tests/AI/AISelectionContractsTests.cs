// Copyright (c) 2026 Dr. Masroor Ehsan

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class AISelectionContractsTests
	{
		[Test]
		public void ConfigurationState_Ready_AllowsRequests()
		{
			var state = AIConfigurationState.Ready();

			state.IsReady.Should().BeTrue();
			state.Reason.Should().Be(AIReadinessReason.Ready);
			state.Message.Should().BeEmpty();
		}

		[Test]
		public void ConfigurationState_NotReady_CarriesReasonAndMessage()
		{
			var state = AIConfigurationState.NotReady(AIReadinessReason.MissingApiKey, "Add an API key to the active profile.");

			state.IsReady.Should().BeFalse();
			state.Reason.Should().Be(AIReadinessReason.MissingApiKey);
			state.Message.Should().Be("Add an API key to the active profile.");
		}

		[Test]
		public void ConversationTarget_BoundToIdentityProviderEndpointAndModel()
		{
			var target = new AIConversationTarget("p1", "Work", "openai", "https://api.openai.com", "gpt-4o");

			target.BelongsTo("p1", "openai", "https://api.openai.com", "gpt-4o").Should().BeTrue();
			target.BelongsTo("p1", "openai", "https://api.openai.com", "gpt-4o-mini").Should().BeFalse();
			target.BelongsTo("p1", "openai", "https://other.example.com", "gpt-4o").Should().BeFalse();
			target.BelongsTo("p1", "anthropic", "https://api.openai.com", "gpt-4o").Should().BeFalse();
			target.BelongsTo("p2", "openai", "https://api.openai.com", "gpt-4o").Should().BeFalse();
		}

		[Test]
		public void ConversationTarget_ProfileRenameDoesNotBreakBoundary()
		{
			var target = new AIConversationTarget("p1", "Old Name", "openai", "https://api.openai.com", "gpt-4o");

			target.BelongsTo("p1", "openai", "https://api.openai.com", "gpt-4o").Should().BeTrue();
			target.ProfileName.Should().Be("Old Name");
		}

		[Test]
		public void Snapshot_CapturesResolvedTargetAndGlobalPreferences()
		{
			var snapshot = new AISelectionSnapshot {
				ProfileId = "p1",
				ProfileName = "Work",
				ProviderType = "openai",
				Endpoint = "https://api.openai.com",
				Model = "gpt-4o",
				ApiKey = "secret",
				CredentialId = "profile-abc",
				MaxContextTokens = 16000,
				StreamResponses = false,
				SendIL = true,
				SendCallGraph = true
			};

			snapshot.ProfileId.Should().Be("p1");
			snapshot.MaxContextTokens.Should().Be(16000);
			snapshot.SendIL.Should().BeTrue();
			snapshot.SendCallGraph.Should().BeTrue();
			snapshot.StreamResponses.Should().BeFalse();
		}
	}
}
