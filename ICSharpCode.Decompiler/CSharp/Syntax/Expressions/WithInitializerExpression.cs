// Copyright (c) 2010-2021 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Expression with Initializer
	/// </summary>
	public class WithInitializerExpression : Expression
	{
		public readonly static TokenRole WithKeywordRole = new TokenRole("with");
		public readonly static Role<ArrayInitializerExpression> InitializerRole = ArrayCreateExpression.InitializerRole;

		public Expression Expression {
			get { return GetChildByRole(Roles.Expression); }
			set { SetChildByRole(Roles.Expression, value); }
		}

		public CSharpTokenNode WithToken {
			get { return GetChildByRole(WithKeywordRole); }
		}

		public ArrayInitializerExpression Initializer {
			get { return GetChildByRole(InitializerRole); }
			set { SetChildByRole(InitializerRole, value); }
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitWithInitializerExpression(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitWithInitializerExpression(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitWithInitializerExpression(this, data);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			return other is WithInitializerExpression o
				&& this.Expression.DoMatch(o.Expression, match)
				&& this.Initializer.DoMatch(o.Initializer, match);
		}
	}
}
