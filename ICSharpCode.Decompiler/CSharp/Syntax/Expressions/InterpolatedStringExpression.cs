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
