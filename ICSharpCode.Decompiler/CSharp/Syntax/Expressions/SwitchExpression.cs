// Copyright (c) 2020 Daniel Grunwald
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


namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Expression switch { SwitchSections }
	/// </summary>
	public class SwitchExpression : Expression
	{
		public static readonly TokenRole SwitchKeywordRole = new TokenRole("switch");
		public static readonly Role<SwitchExpressionSection> SwitchSectionRole = new Role<SwitchExpressionSection>("SwitchSection");

		public Expression Expression {
			get { return GetChildByRole(Roles.Expression); }
			set { SetChildByRole(Roles.Expression, value); }
		}

		public CSharpTokenNode SwitchToken {
			get { return GetChildByRole(SwitchKeywordRole); }
		}

		public CSharpTokenNode LBraceToken {
			get { return GetChildByRole(Roles.LBrace); }
		}

		public AstNodeCollection<SwitchExpressionSection> SwitchSections {
			get { return GetChildrenByRole(SwitchSectionRole); }
		}

		public CSharpTokenNode RBraceToken {
			get { return GetChildByRole(Roles.RBrace); }
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitSwitchExpression(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitSwitchExpression(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitSwitchExpression(this, data);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			SwitchExpression o = other as SwitchExpression;
			return o != null && this.Expression.DoMatch(o.Expression, match) && this.SwitchSections.DoMatch(o.SwitchSections, match);
		}
	}

	/// <summary>
	/// Pattern => Expression
	/// </summary>
	public class SwitchExpressionSection : AstNode
	{
		public static readonly Role<Expression> PatternRole = new Role<Expression>("Pattern", Expression.Null);
		public static readonly Role<Expression> BodyRole = new Role<Expression>("Body", Expression.Null);

		public Expression Pattern {
			get { return GetChildByRole(PatternRole); }
			set { SetChildByRole(PatternRole, value); }
		}

		public CSharpTokenNode ArrowToken {
			get { return GetChildByRole(Roles.Arrow); }
		}

		public Expression Body {
			get { return GetChildByRole(BodyRole); }
			set { SetChildByRole(BodyRole, value); }
		}

		public override NodeType NodeType => NodeType.Unknown;

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitSwitchExpressionSection(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitSwitchExpressionSection(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitSwitchExpressionSection(this, data);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			SwitchExpressionSection o = other as SwitchExpressionSection;
			return o != null && this.Pattern.DoMatch(o.Pattern, match) && this.Body.DoMatch(o.Body, match);
		}
	}
}
