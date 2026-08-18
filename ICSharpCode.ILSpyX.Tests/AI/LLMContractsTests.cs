// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class LLMContractsTests
	{
		[TestCase("user")]
		[TestCase("assistant")]
		[TestCase("system")]
		public void LLMMessage_AcceptsSupportedRole(string role)
		{
			var message = new LLMMessage(role, "content");

			Assert.That(message.Role, Is.EqualTo(role));
		}

		[TestCase(null)]
		[TestCase("")]
		[TestCase(" tool ")]
		public void LLMMessage_RejectsUnsupportedRole(string role)
		{
			Assert.That(() => new LLMMessage(role, "content"), Throws.ArgumentException);
		}

		[Test]
		public void LLMMessage_RejectsNullContent()
		{
			Assert.That(() => new LLMMessage("user", null!), Throws.ArgumentNullException);
		}

		[Test]
		public void LLMRequest_AcceptsValidValues()
		{
			var request = new LLMRequest(
				"system",
				new[] { new LLMMessage("user", "hello") },
				128,
				1.5);

			Assert.That(request.MaxTokens, Is.EqualTo(128));
			Assert.That(request.Temperature, Is.EqualTo(1.5));
		}

		[Test]
		public void LLMRequest_SnapshotsMessages()
		{
			var messages = new List<LLMMessage> { new("user", "first") };
			var request = new LLMRequest("system", messages, 128);

			messages[0] = new LLMMessage("user", "changed");
			messages.Add(new LLMMessage("assistant", "added"));

			Assert.That(request.Messages, Has.Count.EqualTo(1));
			Assert.That(request.Messages[0].Content, Is.EqualTo("first"));
		}

		[Test]
		public void LLMRequest_RejectsNullSystemPrompt()
		{
			Assert.That(() => new LLMRequest(null!, Array.Empty<LLMMessage>(), 1), Throws.ArgumentNullException);
		}

		[Test]
		public void LLMRequest_RejectsNullMessages()
		{
			Assert.That(() => new LLMRequest("system", null!, 1), Throws.ArgumentNullException);
		}

		[Test]
		public void LLMRequest_RejectsNullMessageEntry()
		{
			Assert.That(() => new LLMRequest("system", new LLMMessage[] { null! }, 1), Throws.ArgumentException);
		}

		[TestCase(0)]
		[TestCase(-1)]
		public void LLMRequest_RejectsNonPositiveMaxTokens(int maxTokens)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new LLMRequest("system", Array.Empty<LLMMessage>(), maxTokens));
		}

		[TestCase(-0.1)]
		[TestCase(2.1)]
		[TestCase(double.NaN)]
		[TestCase(double.PositiveInfinity)]
		[TestCase(double.NegativeInfinity)]
		public void LLMRequest_RejectsInvalidTemperature(double temperature)
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new LLMRequest("system", Array.Empty<LLMMessage>(), 1, temperature));
		}
	}
}
