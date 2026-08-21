// Copyright (c) 2026 Dr. Masroor Ehsan

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// Provides approximate token counting for LLM context budget management.
	/// </summary>
	public static class TokenCounter
	{
		const string TruncationSuffix = "...";

		/// <summary>
		/// Estimates the token count using a three-characters-per-token ratio for code and
		/// four characters per token for prose, plus one token per line.
		/// </summary>
		public static int CountTokens(string? text, bool isCode = true)
		{
			if (string.IsNullOrEmpty(text))
				return 0;

			int lineCount = 1;
			foreach (char c in text)
			{
				if (c == '\n')
					lineCount++;
			}

			return text.Length / (isCode ? 3 : 4) + lineCount;
		}

		/// <summary>
		/// Truncates text to the largest Unicode-safe prefix that fits the token budget.
		/// When possible, the prefix ends at a line boundary. The truncation suffix is
		/// included in the budget.
		/// </summary>
		public static string TruncateToTokenBudget(string? text, int maxTokens, bool isCode = true)
		{
			if (string.IsNullOrEmpty(text) || maxTokens <= 0)
				return string.Empty;
			if (CountTokens(text, isCode) <= maxTokens)
				return text;
			if (CountTokens(TruncationSuffix, isCode) > maxTokens)
				return string.Empty;

			int low = 0;
			int high = text.Length;
			int bestLength = 0;

			while (low <= high)
			{
				int midpoint = low + (high - low) / 2;
				int length = GetUnicodeSafePrefixLength(text, midpoint);
				string candidate = text[..length] + TruncationSuffix;

				if (CountTokens(candidate, isCode) <= maxTokens)
				{
					bestLength = length;
					low = midpoint + 1;
				}
				else
				{
					high = midpoint - 1;
				}
			}

			int lastNewline = bestLength > 0 ? text.LastIndexOf('\n', bestLength - 1) : -1;
			if (lastNewline > 0)
				bestLength = lastNewline;
			if (bestLength > 0 && text[bestLength - 1] == '\r')
				bestLength--;

			return text[..bestLength] + TruncationSuffix;
		}

		static int GetUnicodeSafePrefixLength(string text, int length)
		{
			if (length > 0 && length < text.Length
				&& char.IsHighSurrogate(text[length - 1])
				&& char.IsLowSurrogate(text[length]))
			{
				return length - 1;
			}
			return length;
		}
	}
}
