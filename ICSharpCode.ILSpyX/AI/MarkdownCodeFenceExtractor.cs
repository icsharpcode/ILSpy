// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Linq;

using Markdig;
using Markdig.Syntax;

namespace ICSharpCode.ILSpyX.AI
{
	/// <summary>
	/// Extracts fenced code blocks from AI-generated markdown so they can be acted on without
	/// re-parsing the surrounding prose. Used by the "Open in Decompiler" and code-block copy
	/// actions in the AI panes.
	/// </summary>
	public static class MarkdownCodeFenceExtractor
	{
		/// <summary>
		/// A fenced code block lifted out of a markdown document: its language tag (if any),
		/// the code content without the fence markers, and the zero-based source lines it spans.
		/// </summary>
		public sealed class CodeFence
		{
			/// <summary>Language identifier (e.g. "csharp", "il", "xml"), or <see langword="null"/> when unspecified.</summary>
			public string? Language { get; init; }

			/// <summary>The code content, excluding the surrounding fence markers.</summary>
			public string Code { get; init; } = string.Empty;

			/// <summary>Zero-based line where the opening fence starts in the source markdown.</summary>
			public int StartLine { get; init; }

			/// <summary>Zero-based line just past the last code line (exclusive).</summary>
			public int EndLine { get; init; }

			/// <summary>Is this a C# code fence? Accepts the common tags models emit.</summary>
			public bool IsCSharp =>
				Language is not null &&
				(Language.Equals("csharp", StringComparison.OrdinalIgnoreCase)
					|| Language.Equals("cs", StringComparison.OrdinalIgnoreCase)
					|| Language.Equals("c#", StringComparison.OrdinalIgnoreCase));

			/// <summary>Is this an IL code fence?</summary>
			public bool IsIL =>
				Language is not null && Language.Equals("il", StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Extracts every fenced code block from <paramref name="markdown"/>, in document order.
		/// Returns an empty list for blank input and for markdown with no fenced blocks.
		/// </summary>
		public static IReadOnlyList<CodeFence> Extract(string? markdown)
		{
			if (string.IsNullOrEmpty(markdown))
				return System.Array.Empty<CodeFence>();

			var document = Markdown.Parse(markdown);
			var fences = new List<CodeFence>();

			foreach (var block in document.Descendants<FencedCodeBlock>())
			{
				fences.Add(new CodeFence {
					Language = string.IsNullOrWhiteSpace(block.Info) ? null : block.Info,
					Code = block.Lines.ToString(),
					StartLine = block.Line,
					EndLine = block.Line + block.Lines.Count + 1,
				});
			}

			return fences;
		}

		/// <summary>Extracts only the C#-tagged code fences from <paramref name="markdown"/>.</summary>
		public static IReadOnlyList<CodeFence> ExtractCSharpFences(string? markdown)
			=> Extract(markdown).Where(f => f.IsCSharp).ToList();

		/// <summary>Extracts only the IL-tagged code fences from <paramref name="markdown"/>.</summary>
		public static IReadOnlyList<CodeFence> ExtractILFences(string? markdown)
			=> Extract(markdown).Where(f => f.IsIL).ToList();

		/// <summary>
		/// Returns the code fence whose source span contains <paramref name="zeroBasedLine"/>, or
		/// <see langword="null"/> when the line falls outside every fence. Fences never overlap in
		/// valid markdown, so the first match is unambiguous.
		/// </summary>
		public static CodeFence? FindFenceAtLine(string? markdown, int zeroBasedLine)
			=> Extract(markdown).FirstOrDefault(f => zeroBasedLine >= f.StartLine && zeroBasedLine < f.EndLine);
	}
}
