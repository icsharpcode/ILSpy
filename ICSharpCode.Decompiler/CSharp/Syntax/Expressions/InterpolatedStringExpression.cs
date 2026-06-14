using System.Collections.Generic;

using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>interpolated_string_expression : interpolated_regular_string_expression | interpolated_verbatim_string_expression | interpolated_raw_string_expression ;</c> (C# grammar §12.8.3)
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

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			InterpolatedStringExpression o = other as InterpolatedStringExpression;
			return o != null && !o.IsNull && this.Content.DoMatch(o.Content, match);
		}
	}

	[DecompilerAstNode(hasNullNode: true)]
	public abstract partial class InterpolatedStringContent : AstNode
	{
		public new static readonly Role<InterpolatedStringContent> Role = new Role<InterpolatedStringContent>("InterpolatedStringContent", Syntax.InterpolatedStringContent.Null);

		public override NodeType NodeType => NodeType.Unknown;
	}

	/// <summary>
	/// <c>regular_interpolation : expression (',' interpolation_minimum_width)? Regular_Interpolation_Format? ;</c> (C# grammar §12.8.3)
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

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			Interpolation o = other as Interpolation;
			return o != null && this.Expression.DoMatch(o.Expression, match);
		}
	}

	/// <summary>
	/// <c>Interpolated_Regular_String_Mid : Interpolated_Regular_String_Element+ ;</c> (C# lexical grammar §12.8.3)
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

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			InterpolatedStringText o = other as InterpolatedStringText;
			return o != null && o.Text == this.Text;
		}
	}
}
