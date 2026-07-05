// Copyright (c) 2017 Siegfried Pammer
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

#nullable enable

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>interpolated_string_expression ::= interpolated_string_content*</c> (C# grammar §12.8.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class InterpolatedStringExpression : Expression
	{
		public const string OpenQuote = "$\"";
		public const string CloseQuote = "\"";

		[Slot("Content")]
		public partial AstNodeCollection<InterpolatedStringContent> Content { get; }

		public InterpolatedStringExpression(IList<InterpolatedStringContent> content)
		{
			Content.AddRange(content);
		}
	}

	/// <summary>
	/// <code>
	/// interpolated_string_content ::=
	///       interpolation
	///     | interpolated_string_text
	/// </code>
	/// (C# grammar §12.8.3)
	/// </summary>
	[DecompilerAstNode]
	public abstract partial class InterpolatedStringContent : AstNode
	{
	}

	/// <summary>
	/// <c>interpolation ::= '{' expression ( ',' alignment )? ( ':' format )? '}'</c> (C# grammar §12.8.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class Interpolation : InterpolatedStringContent
	{
		[Slot("Expression")]
		public partial Expression Expression { get; set; }

		public int Alignment { get; }

		public string? Suffix { get; }
		public Interpolation(Expression expression, int alignment = 0, string? suffix = null)
		{
			Expression = expression;
			Alignment = alignment;
			Suffix = suffix;
		}
	}

	/// <summary>
	/// <c>interpolated_string_text ::= text_character+</c> (C# lexical grammar §12.8.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class InterpolatedStringText : InterpolatedStringContent
	{
		public string Text { get; set; } = string.Empty;

		public InterpolatedStringText()
		{
		}

		public InterpolatedStringText(string text)
		{
			Text = text;
		}
	}
}
