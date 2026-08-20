// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.IO;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
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
			string promptsDirectory = Path.Combine(Path.GetDirectoryName(typeof(AIPromptProvider).Assembly.Location)!, "AI", "prompts");
			Directory.CreateDirectory(promptsDirectory);
			string variationPath = Path.Combine(promptsDirectory, "explanation.phase3-test.prompt");
			File.WriteAllText(variationPath, "---\napplies_to_models: [phase3-test-model]\n---\nPhase 3 model-specific explanation prompt.");
			try
			{
				Assert.That(AIPromptProvider.Instance.GetSystemPrompt("explanation", "phase3-test-model"), Is.EqualTo("Phase 3 model-specific explanation prompt."));
			}
			finally
			{
				File.Delete(variationPath);
			}
		}
	}
}
