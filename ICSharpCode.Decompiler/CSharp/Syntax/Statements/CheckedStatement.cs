﻿// 
// CheckedStatement.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// checked BodyBlock
	/// </summary>
	[DecompilerAstNode(false)]
	public partial class CheckedStatement : Statement
	{
		public static readonly TokenRole CheckedKeywordRole = new TokenRole("checked");

		public CSharpTokenNode CheckedToken {
			get { return GetChildByRole(CheckedKeywordRole); }
		}

		public BlockStatement Body {
			get { return GetChildByRole(Roles.Body); }
			set { SetChildByRole(Roles.Body, value); }
		}

		public CheckedStatement()
		{
		}

		public CheckedStatement(BlockStatement body)
		{
			AddChild(body, Roles.Body);
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitCheckedStatement(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitCheckedStatement(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitCheckedStatement(this, data);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			CheckedStatement o = other as CheckedStatement;
			return o != null && this.Body.DoMatch(o.Body, match);
		}
	}
}
