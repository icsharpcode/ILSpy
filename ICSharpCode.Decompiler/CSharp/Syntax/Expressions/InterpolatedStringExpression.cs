using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public class InterpolatedStringExpression : Expression
	{
		public static readonly TokenRole OpenQuote = new TokenRole("$\"");
		public static readonly TokenRole CloseQuote = new TokenRole("\"");

		public AstNodeCollection<InterpolatedStringContent> Content => GetChildrenByRole(InterpolatedStringContent.Role);

		public InterpolatedStringExpression()
		{

		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitInterpolatedStringExpression(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitInterpolatedStringExpression(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitInterpolatedStringExpression(this, data);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			var o = other as InterpolatedStringExpression;
			return o != null && !o.IsNull && this.Content.DoMatch(o.Content, match);
		}
	}

	public abstract class InterpolatedStringContent : AstNode
	{
		#region Null
		public new static readonly InterpolatedStringContent Null = new NullInterpolatedStringContent();

		sealed class NullInterpolatedStringContent : InterpolatedStringContent
		{
			public override bool IsNull => true;

			public override void AcceptVisitor(IAstVisitor visitor)
			{
				visitor.VisitNullNode(this);
			}

			public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
			{
				return visitor.VisitNullNode(this);
			}

			public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
			{
				return visitor.VisitNullNode(this, data);
			}

			protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
			{
				return other == null || other.IsNull;
			}
		}
		#endregion

		public new static readonly Role<InterpolatedStringContent> Role = new Role<InterpolatedStringContent>("InterpolatedStringContent", Syntax.InterpolatedStringContent.Null);

		public override NodeType NodeType => NodeType.Unknown;
	}

	/// <summary>
	/// { Expression }
	/// </summary>
	public class Interpolation : InterpolatedStringContent
	{
		public static readonly TokenRole LBrace = new TokenRole("{");
		public static readonly TokenRole RBrace = new TokenRole("}");

		public CSharpTokenNode LBraceToken => GetChildByRole(LBrace);

		public Expression Expression {
			get => GetChildByRole(Roles.Expression);
			set => SetChildByRole(Roles.Expression, value);
		}

		public string Suffix { get; }

		public CSharpTokenNode RBraceToken => GetChildByRole(RBrace);

		public Interpolation()
		{

		}

		public Interpolation(Expression expression, string suffix = null)
		{
			Expression = expression;
			Suffix = suffix;
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitInterpolation(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitInterpolation(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitInterpolation(this, data);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			var o = other as Interpolation;
			return o != null && this.Expression.DoMatch(o.Expression, match);
		}
	}

	public class InterpolatedStringText : InterpolatedStringContent
	{
		public string Text { get; set; }

		public InterpolatedStringText()
		{

		}

		public InterpolatedStringText(string text)
		{
			Text = text;
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitInterpolatedStringText(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitInterpolatedStringText(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitInterpolatedStringText(this, data);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			var o = other as InterpolatedStringText;
			return o != null && o.Text == this.Text;
		}
	}
}
