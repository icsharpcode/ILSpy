// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;

namespace ICSharpCode.ILSpy.AI
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
			if (messages is null)
				throw new ArgumentNullException(nameof(messages));
			var messageSnapshot = new LLMMessage[messages.Count];
			for (int i = 0; i < messages.Count; i++)
			{
				messageSnapshot[i] = messages[i]
					?? throw new ArgumentException("Messages cannot contain null entries.", nameof(messages));
			}
			Messages = messageSnapshot;
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
