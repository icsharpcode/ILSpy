// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.IO;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	[NonParallelizable]
	public sealed class AIPromptProviderTests
	{
		[Test]
		public void GetSystemPrompt_ReturnsNonEmptyString_ForKnownPromptIds()
		{
			var promptIds = new[] { "explanation", "rename", "chat", "security", "security_audit", "generate_docs", "search", "assembly_summary" };

			foreach (var promptId in promptIds)
			{
				var prompt = AIPromptProvider.Instance.GetSystemPrompt(promptId);
				Assert.That(prompt, Is.Not.Null.And.Not.Empty);
			}
		}

		[Test]
		public void GetSystemPrompt_ThrowsArgumentException_ForUnknownPromptId()
		{
			Assert.Throws<ArgumentException>(() => AIPromptProvider.Instance.GetSystemPrompt("nonexistent"));
		}

		[TestCase("")]
		[TestCase(" ")]
		[TestCase("\t")]
		public void GetSystemPrompt_ThrowsArgumentException_ForBlankPromptId(string promptId)
		{
			Assert.Throws<ArgumentException>(() => AIPromptProvider.Instance.GetSystemPrompt(promptId));
		}

		[Test]
		public void GetSystemPrompt_ReturnsSameInstance_OnRepeatedCalls()
		{
			var prompt1 = AIPromptProvider.Instance.GetSystemPrompt("explanation");
			var prompt2 = AIPromptProvider.Instance.GetSystemPrompt("explanation");

			Assert.That(ReferenceEquals(prompt1, prompt2), Is.True, "Caching should return same string instance.");
		}

		[Test]
		public void GetSystemPrompt_WithModelId_ReturnsPrompt()
		{
			var prompt = AIPromptProvider.Instance.GetSystemPrompt("explanation", "claude-opus-5");

			Assert.That(prompt, Is.Not.Null.And.Not.Empty);
		}

		[Test]
		public void GetSystemPrompt_SelectsModelSpecificVariation()
		{
			string promptsDirectory = GetPromptsDirectory();
			Directory.CreateDirectory(promptsDirectory);
			string variationPath = Path.Combine(promptsDirectory, "explanation.phase4-test.prompt");
			File.WriteAllText(variationPath, "---\napplies_to_models: [phase4-test-model]\n---\nPhase 4 model-specific explanation prompt.");
			try
			{
				Assert.That(AIPromptProvider.Instance.GetSystemPrompt("explanation", "phase4-test-model"), Is.EqualTo("Phase 4 model-specific explanation prompt."));
			}
			finally
			{
				File.Delete(variationPath);
			}
		}

		[Test]
		public void GetSystemPrompt_SelectsFirstLexicalMatchingVariation()
		{
			string promptsDirectory = GetPromptsDirectory();
			Directory.CreateDirectory(promptsDirectory);
			string firstPath = Path.Combine(promptsDirectory, "explanation.phase4-01.prompt");
			string secondPath = Path.Combine(promptsDirectory, "explanation.phase4-02.prompt");
			const string modelId = "phase4-order-model";
			File.WriteAllText(firstPath, $"---\napplies_to_models: [{modelId}]\n---\nFirst lexical variation.");
			File.WriteAllText(secondPath, $"---\napplies_to_models: [{modelId}]\n---\nSecond lexical variation.");
			try
			{
				Assert.That(AIPromptProvider.Instance.GetSystemPrompt("explanation", modelId), Is.EqualTo("First lexical variation."));
			}
			finally
			{
				File.Delete(firstPath);
				File.Delete(secondPath);
			}
		}

		[Test]
		public void GetSystemPrompt_MatchesModelIdsCaseSensitively()
		{
			string promptsDirectory = GetPromptsDirectory();
			Directory.CreateDirectory(promptsDirectory);
			string variationPath = Path.Combine(promptsDirectory, "explanation.phase4-case.prompt");
			const string configuredModelId = "Phase4-Case-Model";
			const string requestedModelId = "phase4-case-model";
			File.WriteAllText(variationPath, $"---\napplies_to_models: [{configuredModelId}]\n---\nCase-sensitive variation.");
			try
			{
				Assert.That(AIPromptProvider.Instance.GetSystemPrompt("explanation", requestedModelId), Is.EqualTo(AIPromptProvider.Instance.GetSystemPrompt("explanation")));
			}
			finally
			{
				File.Delete(variationPath);
			}
		}

		[Test]
		public void GetSystemPrompt_IgnoresMalformedVariationAndUsesBasePrompt()
		{
			string promptsDirectory = GetPromptsDirectory();
			Directory.CreateDirectory(promptsDirectory);
			string variationPath = Path.Combine(promptsDirectory, "explanation.phase4-malformed.prompt");
			File.WriteAllText(variationPath, "---\napplies_to_models: [phase4-malformed-model\n---\nMalformed variation.");
			try
			{
				Assert.That(AIPromptProvider.Instance.GetSystemPrompt("explanation", "phase4-malformed-model"), Is.EqualTo(AIPromptProvider.Instance.GetSystemPrompt("explanation")));
			}
			finally
			{
				File.Delete(variationPath);
			}
		}

		private static string GetPromptsDirectory()
		{
			return Path.Combine(Path.GetDirectoryName(typeof(AIPromptProvider).Assembly.Location)!, "AI", "prompts");
		}
	}
}
