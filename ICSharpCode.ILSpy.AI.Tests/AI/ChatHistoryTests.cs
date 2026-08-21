// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.IO;

using ICSharpCode.ILSpy.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.AI.Tests.AI
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

		[Test]
		public void GetOrCreate_DoesNotResumeReadOnlyTarget()
		{
			var history = new ChatHistory();
			var target = new AIConversationTarget("deleted", "Deleted", "openai", "https://api.openai.com", "gpt-4o");
			ChatConversation old = history.GetOrCreate(target);
			old.ReadOnly = true;
			ChatConversation replacement = history.GetOrCreate(target);
			Assert.That(replacement.Id, Is.Not.EqualTo(old.Id));
			Assert.That(old.ReadOnly, Is.True);
		}

		[Test]
		public void Schema2_RoundTripsConversationTargetAndReadOnlyState()
		{
			string path = Path.Combine(Path.GetTempPath(), "ilspy-chat-schema2-" + Guid.NewGuid() + ".json");
			try
			{
				var history = new ChatHistory { AssemblyPath = "sample.dll" };
				ChatConversation writable = history.StartNew(new AIConversationTarget("p1", "Work", "openai", "https://one.example", "model-a"));
				writable.Messages.Add(new ChatMessage { Role = "user", Content = "hello" });
				ChatConversation deleted = history.StartNew(new AIConversationTarget("deleted", "Old", "openai", "https://two.example", "model-b"));
				deleted.ReadOnly = true;
				history.Save(path);

				ChatHistory loaded = ChatHistory.Load(path);
				Assert.That(loaded.SchemaVersion, Is.EqualTo(2));
				Assert.That(loaded.Conversations, Has.Count.EqualTo(2));
				Assert.That(loaded.Conversations[0].Target, Is.EqualTo(writable.Target));
				Assert.That(loaded.Conversations[0].Messages[0].Content, Is.EqualTo("hello"));
				Assert.That(loaded.Conversations[1].ReadOnly, Is.True);
			}
			finally { if (File.Exists(path)) File.Delete(path); }
		}

		[Test]
		public void StartNew_AlwaysCreatesBoundaryEvenWhenTargetMatches()
		{
			var history = new ChatHistory();
			var target = new AIConversationTarget("p1", "Work", "openai", "https://api.example", "model");
			ChatConversation first = history.StartNew(target);
			ChatConversation second = history.StartNew(target with { ProfileName = "Renamed" });
			Assert.That(second.Id, Is.Not.EqualTo(first.Id));
			Assert.That(history.ActiveConversationId, Is.EqualTo(second.Id));
			Assert.That(history.Conversations, Has.Count.EqualTo(2));
		}

		[Test]
		public void UnknownTargetConversation_IsForcedReadOnlyOnLoad()
		{
			string path = Path.Combine(Path.GetTempPath(), "ilspy-chat-unknown-" + Guid.NewGuid() + ".json");
			try
			{
				File.WriteAllText(path, "{\"SchemaVersion\":2,\"Conversations\":[{\"Id\":\"c1\",\"Messages\":[],\"ReadOnly\":false}]}");
				ChatHistory loaded = ChatHistory.Load(path);
				Assert.That(loaded.Conversations, Has.Count.EqualTo(1));
				Assert.That(loaded.Conversations[0].ReadOnly, Is.True);
			}
			finally { if (File.Exists(path)) File.Delete(path); }
		}
	}
}
