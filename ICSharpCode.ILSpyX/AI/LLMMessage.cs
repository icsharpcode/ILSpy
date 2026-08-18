// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed record LLMMessage
	{
		public LLMMessage(string role, string content)
		{
			if (role is not ("user" or "assistant" or "system"))
				throw new ArgumentException("Role must be 'user', 'assistant', or 'system'.", nameof(role));

			Role = role;
			Content = content ?? throw new ArgumentNullException(nameof(content));
		}

		public string Role { get; }

		public string Content { get; }

		public void Deconstruct(out string role, out string content)
		{
			role = Role;
			content = Content;
		}
	}
}
