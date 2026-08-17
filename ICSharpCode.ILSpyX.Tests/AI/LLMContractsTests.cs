// Copyright (c) 2026 Masroor
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
