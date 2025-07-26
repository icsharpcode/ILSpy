﻿using System.Collections.Generic;

using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[DecompilerAstNode(hasNullNode: false)]
	public partial class InterpolatedStringExpression : Expression
	{
		public static readonly TokenRole OpenQuote = new TokenRole("$\"");
		public static readonly TokenRole CloseQuote = new TokenRole("\"");

		public AstNodeCollection<InterpolatedStringContent> Content {
			get { return GetChildrenByRole(InterpolatedStringContent.Role); }
		}

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
	/// { Expression , Alignment : Suffix }
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class Interpolation : InterpolatedStringContent
	{
		public static readonly TokenRole LBrace = new TokenRole("{");
		public static readonly TokenRole RBrace = new TokenRole("}");

		public CSharpTokenNode LBraceToken {
			get { return GetChildByRole(LBrace); }
		}

		public Expression Expression {
			get { return GetChildByRole(Roles.Expression); }
			set { SetChildByRole(Roles.Expression, value); }
		}

		public int Alignment { get; }

		public string Suffix { get; }

		public CSharpTokenNode RBraceToken {
			get { return GetChildByRole(RBrace); }
		}

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
