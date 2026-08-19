// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.IO;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class ChatHistoryTests
	{
		[Test]
		public void SaveLoad_RoundTripsMessages()
		{
			string path = Path.Combine(Path.GetTempPath(), "ilspy-chat-" + Guid.NewGuid() + ".json");
			try
			{
				var history = new ChatHistory { AssemblyPath = "sample.dll" };
				history.Messages.Add(new ChatMessage { Role = "user", Content = "What does this do?" });
				history.Messages.Add(new ChatMessage { Role = "assistant", Content = "It loads metadata." });
				history.Save(path);
				var loaded = ChatHistory.Load(path);
				Assert.That(loaded.AssemblyPath, Is.EqualTo("sample.dll"));
				Assert.That(loaded.Messages, Has.Count.EqualTo(2));
				Assert.That(loaded.Messages[1].Content, Is.EqualTo("It loads metadata."));
			}
			finally { if (File.Exists(path)) File.Delete(path); }
		}

		[Test]
		public void ToMarkdown_UsesConversationHeadings()
		{
			var history = new ChatHistory();
			history.Messages.Add(new ChatMessage { Role = "user", Content = "Hi" });
			history.Messages.Add(new ChatMessage { Role = "assistant", Content = "Hello" });
			string markdown = history.ToMarkdown("Session");
			Assert.That(markdown, Does.Contain("# Session"));
			Assert.That(markdown, Does.Contain("## User"));
			Assert.That(markdown, Does.Contain("## Assistant"));
		}

		[Test]
		public void TargetIdentity_IgnoresProfileName()
		{
			var first = new AIConversationTarget("p1", "Before", "openai", "https://api.openai.com", "gpt-4o");
			var second = new AIConversationTarget("p1", "After", "openai", "https://api.openai.com", "gpt-4o");
			Assert.That(first.BelongsTo(second.ProfileId, second.ProviderType, second.Endpoint, second.Model), Is.True);
		}

		[Test]
		public void LegacyHistory_IsReadOnlyAndTargetless()
		{
			string path = Path.Combine(Path.GetTempPath(), "ilspy-chat-legacy-" + Guid.NewGuid() + ".json");
			try
			{
				File.WriteAllText(path, "{\"AssemblyPath\":\"a.dll\",\"Messages\":[{\"Role\":\"user\",\"Content\":\"hello\"}]}");
				ChatHistory loaded = ChatHistory.Load(path);
				Assert.That(loaded.SchemaVersion, Is.EqualTo(2));
				Assert.That(loaded.ActiveConversation.ReadOnly, Is.True);
				Assert.That(loaded.ActiveConversation.Target, Is.Null);
				Assert.That(loaded.Messages, Has.Count.EqualTo(1));
			}
			finally { if (File.Exists(path)) File.Delete(path); }
		}

		[Test]
		public void GetOrCreate_ReusesTargetAndSeparatesIdentityChanges()
		{
			var history = new ChatHistory();
			var target = new AIConversationTarget("p1", "Default", "openai", "https://api.openai.com", "gpt-4o");
			ChatConversation first = history.GetOrCreate(target);
			first.Messages.Add(new ChatMessage { Content = "one" });
			ChatConversation same = history.GetOrCreate(new AIConversationTarget("p1", "Renamed", "openai", "https://api.openai.com", "gpt-4o"));
			ChatConversation different = history.GetOrCreate(new AIConversationTarget("p1", "Renamed", "openai", "https://other", "gpt-4o"));
			Assert.That(same.Id, Is.EqualTo(first.Id));
			Assert.That(different.Id, Is.Not.EqualTo(first.Id));
			Assert.That(history.Conversations, Has.Count.EqualTo(2));
		}
	}
}
