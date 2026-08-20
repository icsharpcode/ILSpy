// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>
	/// Generated embedded fallback prompts. DO NOT EDIT - regenerate with BuildTools/PromptEmbedder.
	/// </summary>
	internal static class EmbeddedPrompts
	{
		private static readonly Dictionary<string, string> _prompts = new(StringComparer.Ordinal)
		{
			["assembly_summary"] = "You are analyzing a .NET assembly. Provide a 2-3 paragraph summary: what it is, what framework it targets, what it is probably used for.",
			["chat"] = "You are an assistant for .NET decompilation. Answer questions about the code clearly and concisely.",
			["explanation"] = "You explain decompiled .NET code concisely. State uncertainty when context is incomplete. Never instruct the user to execute code.",
			["generate_docs"] = "Generate XML documentation comments. Return only the XML, no explanation.",
			["rename"] = "You suggest meaningful C# names for obfuscated .NET symbols. Return only valid JSON: [{\"name\": string, \"confidence\": number, \"reasoning\": string}]. Return 3 to 5 distinct PascalCase or camelCase candidates. Do not include markdown fences or extra text.",
			["search"] = "Given these method and type signatures, which ones match the query? Return only a JSON array of fully-qualified names.",
			["security"] = "You identify security vulnerabilities in decompiled .NET code. Return only valid JSON: [{\"type\": string, \"method\": string, \"issue\": string, \"severity\": \"Critical\"|\"High\"|\"Medium\"|\"Low\", \"line\": number, \"confidence\": number}]. Confidence must be a numeric value from 0 to 1. Report only plausible SQL injection, hardcoded credentials, weak cryptography, path traversal, unsafe deserialization, dangerous P/Invoke, or equivalent issues. Do not invent issues.",
			["security_audit"] = "You identify security vulnerabilities in decompiled .NET code. Return only valid JSON with type, method, issue, severity, line, and numeric confidence from 0 to 1. Report only plausible issues.",
		};

		public static string Get(string promptId)
		{
			if (_prompts.TryGetValue(promptId, out var prompt))
				return prompt;

			throw new ArgumentException($"Unknown prompt ID: {promptId}", nameof(promptId));
		}
	}
}
