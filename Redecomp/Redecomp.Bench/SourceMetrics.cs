// Copyright (c) 2026 Franck
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

using System.Text;
using System.Text.RegularExpressions;

namespace Redecomp.Bench;

/// <summary>
/// Text-level measures comparing reconstructed C# sources with the originals. They use lexical
/// heuristics rather than a parser so the benchmark needs no package beyond the decompiler.
/// </summary>
public static partial class SourceMetrics
{
	static readonly HashSet<string> Keywords = [
		"abstract", "as", "base", "break", "case", "catch", "checked", "class", "const", "continue",
		"default", "delegate", "do", "else", "enum", "event", "explicit", "extern", "false", "finally",
		"fixed", "for", "foreach", "goto", "if", "implicit", "in", "interface", "internal", "is", "lock",
		"namespace", "new", "null", "operator", "out", "override", "params", "private", "protected",
		"public", "readonly", "ref", "return", "sealed", "sizeof", "stackalloc", "static", "struct",
		"switch", "this", "throw", "true", "try", "typeof", "unchecked", "unsafe", "using", "virtual",
		"volatile", "while", "await", "yield", "record", "required", "async", "partial", "when",
	];

	/// <summary>Names of the local variables declared in the code (including foreach, catch and out variables).</summary>
	public static ISet<string> DeclaredLocals(string code)
	{
		string stripped = Lex(code).Code;
		var locals = new HashSet<string>();
		foreach (Match m in StatementDeclaration().Matches(stripped))
			AddIfLocal(m.Groups["type"].Value, m.Groups["name"].Value);
		foreach (Match m in InlineDeclaration().Matches(stripped))
			AddIfLocal(m.Groups["type"].Value, m.Groups["name"].Value);
		return locals;

		void AddIfLocal(string type, string name)
		{
			string head = type.Split('<', '.', '?', '[')[0];
			if (!Keywords.Contains(head) && !Keywords.Contains(name))
				locals.Add(name);
		}
	}

	/// <summary>
	/// Number of declared locals whose name says nothing about their role: the fallback names the
	/// decompiler derives from a type or a usage slot (<c>num</c>, <c>flag</c>, <c>result</c>, ...).
	/// </summary>
	public static int GenericLocalCount(string code)
		=> DeclaredLocals(code).Count(name => GenericName().IsMatch(name));

	[GeneratedRegex(@"^(num|flag|text|obj|value|array|result|val|item|list|s|b|c)\d*$")]
	private static partial Regex GenericName();

	/// <summary>Text of every <c>//</c> and <c>///</c> comment, trimmed, one entry per line.</summary>
	public static IReadOnlyList<string> CommentLines(string code) => Lex(code).Comments;

	const string TypePattern = @"(?<type>[A-Za-z_][\w.]*(?:<[^;{}()]*?>)?\??(?:\[\])*)";

	// A declaration at the start of a statement, or at the start of a for/foreach/using header.
	[GeneratedRegex(@"(?:^|[;{}]|\b(?:for|foreach|using|fixed)\s*\()\s*(?:await\s+)?(?:using\s+)?(?:const\s+)?(?:scoped\s+)?(?:ref\s+(?:readonly\s+)?)?"
		+ TypePattern + @"\s+(?<name>[A-Za-z_]\w*)\s*(?:=|;|,|\bin\b)", RegexOptions.Multiline)]
	private static partial Regex StatementDeclaration();

	// Declarations inside expressions: out variables, type patterns and catch clauses.
	[GeneratedRegex(@"(?:\bout\s+|\bis\s+|\bcatch\s*\(\s*)" + TypePattern + @"\s+(?<name>[A-Za-z_]\w*)\b")]
	private static partial Regex InlineDeclaration();

	/// <summary>Splits code into the code with comments and literal contents blanked out, and the comment texts.</summary>
	static (string Code, List<string> Comments) Lex(string code)
	{
		var output = new StringBuilder(code.Length);
		var comments = new List<string>();
		int i = 0;
		while (i < code.Length)
		{
			char c = code[i];
			char next = i + 1 < code.Length ? code[i + 1] : '\0';
			if (c == '/' && next == '/')
			{
				int end = code.IndexOf('\n', i);
				if (end < 0)
					end = code.Length;
				string text = code[i..end].TrimStart('/').Trim();
				if (text.Length > 0)
					comments.Add(text);
				output.Append(' ', end - i);
				i = end;
			}
			else if (c == '/' && next == '*')
			{
				int end = code.IndexOf("*/", i + 2, StringComparison.Ordinal);
				end = end < 0 ? code.Length : end + 2;
				foreach (string line in code[(i + 2)..Math.Max(i + 2, end - 2)].Split('\n'))
				{
					string text = line.Trim().TrimStart('*').Trim();
					if (text.Length > 0)
						comments.Add(text);
				}
				output.Append(BlankKeepingNewlines(code[i..end]));
				i = end;
			}
			else if (c == '"' || c == '\'' || ((c == '@' || c == '$') && (next == '"' || next == '@' || next == '$')))
			{
				int end = EndOfLiteral(code, i);
				output.Append(BlankKeepingNewlines(code[i..end]));
				i = end;
			}
			else
			{
				output.Append(c);
				i++;
			}
		}
		return (output.ToString(), comments);
	}

	static int EndOfLiteral(string code, int start)
	{
		int i = start;
		bool verbatim = false;
		while (code[i] == '@' || code[i] == '$')
		{
			verbatim |= code[i] == '@';
			i++;
		}
		char quote = code[i];
		int quotes = 0;
		while (i + quotes < code.Length && code[i + quotes] == '"' && quote == '"')
			quotes++;
		if (quotes >= 3)
		{
			string delimiter = new('"', quotes);
			int close = code.IndexOf(delimiter, i + quotes, StringComparison.Ordinal);
			return close < 0 ? code.Length : close + quotes;
		}
		i++;
		while (i < code.Length)
		{
			if (code[i] == quote)
			{
				if (verbatim && i + 1 < code.Length && code[i + 1] == quote)
				{
					i += 2;
					continue;
				}
				return i + 1;
			}
			i += !verbatim && code[i] == '\\' ? 2 : 1;
		}
		return code.Length;
	}

	static string BlankKeepingNewlines(string text)
		=> new(text.Select(ch => ch == '\n' ? '\n' : ' ').ToArray());
}
