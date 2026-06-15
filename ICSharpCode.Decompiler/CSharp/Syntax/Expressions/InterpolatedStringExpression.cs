using System.Collections.Generic;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>interpolated_string_expression ::= interpolated_string_content*</c> (C# grammar §12.8.3)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class InterpolatedStringExpression : Expression
	{
		public static readonly TokenRole OpenQuote = new TokenRole("$\"");
		public static readonly TokenRole CloseQuote = new TokenRole("\"");

		[Slot("InterpolatedStringContent.Role")]
		public partial AstNodeCollection<InterpolatedStringContent> Content { get; }

		public InterpolatedStringExpression()
		{

		}

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
	[DecompilerAstNode(hasNullNode: true)]
	public abstract partial class InterpolatedStringContent : AstNode
	{
		public new static readonly Role<InterpolatedStringContent> Role = new Role<InterpolatedStringContent>("InterpolatedStringContent", Syntax.InterpolatedStringContent.Null);

		public override NodeType NodeType => NodeType.Unknown;
	}

	/// <summary>
	/// <c>interpolation ::= '{' expression ( ',' alignment )? ( ':' format )? '}'</c> (C# grammar §12.8.3)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class Interpolation : InterpolatedStringContent
	{
		public static readonly TokenRole LBrace = new TokenRole("{");
		public static readonly TokenRole RBrace = new TokenRole("}");

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		public int Alignment { get; }

		public string Suffix { get; }

		public Interpolation()
		{

		}

		public Interpolation(Expression expression, int alignment = 0, string suffix = null)
		{
			Expression = expression;
			Alignment = alignment;
			Suffix = suffix;
		}
	}

	/// <summary>
	/// <c>interpolated_string_text ::= text_character+</c> (C# lexical grammar §12.8.3)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class InterpolatedStringText : InterpolatedStringContent
	{
		public string Text { get; set; }

		public InterpolatedStringText()
		{

		}

		public InterpolatedStringText(string text)
		{
			Text = text;
		}
	}
}
