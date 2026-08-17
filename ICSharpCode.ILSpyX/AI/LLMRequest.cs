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

namespace ICSharpCode.ILSpyX.AI
{
	public sealed record LLMRequest
	{
		public LLMRequest(string systemPrompt, IReadOnlyList<LLMMessage> messages, int maxTokens, double temperature = 0.7)
		{
			if (maxTokens <= 0)
				throw new ArgumentOutOfRangeException(nameof(maxTokens), "Maximum tokens must be greater than zero.");
			if (!double.IsFinite(temperature) || temperature < 0 || temperature > 2)
				throw new ArgumentOutOfRangeException(nameof(temperature), "Temperature must be between 0 and 2.");

			SystemPrompt = systemPrompt ?? throw new ArgumentNullException(nameof(systemPrompt));
			Messages = messages ?? throw new ArgumentNullException(nameof(messages));
			for (int i = 0; i < messages.Count; i++)
			{
				if (messages[i] is null)
					throw new ArgumentException("Messages cannot contain null entries.", nameof(messages));
			}
			MaxTokens = maxTokens;
			Temperature = temperature;
		}

		public string SystemPrompt { get; }

		public IReadOnlyList<LLMMessage> Messages { get; }

		public int MaxTokens { get; }

		public double Temperature { get; }

		public void Deconstruct(out string systemPrompt, out IReadOnlyList<LLMMessage> messages, out int maxTokens, out double temperature)
		{
			systemPrompt = SystemPrompt;
			messages = Messages;
			maxTokens = MaxTokens;
			temperature = Temperature;
		}
	}
}
