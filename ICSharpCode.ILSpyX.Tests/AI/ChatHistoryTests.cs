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
	}
}
