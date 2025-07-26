﻿// 
// BlockStatement.cs
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

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// { Statements }
	/// </summary>
	[DecompilerAstNode(hasNullNode: true, hasPatternPlaceholder: true)]
	public partial class BlockStatement : Statement, IEnumerable<Statement>
	{
		public static readonly Role<Statement> StatementRole = new Role<Statement>("Statement", Statement.Null);

		public CSharpTokenNode LBraceToken {
			get { return GetChildByRole(Roles.LBrace); }
		}

		public AstNodeCollection<Statement> Statements {
			get { return GetChildrenByRole(StatementRole); }
		}

		public CSharpTokenNode RBraceToken {
			get { return GetChildByRole(Roles.RBrace); }
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			BlockStatement o = other as BlockStatement;
			return o != null && !o.IsNull && this.Statements.DoMatch(o.Statements, match);
		}

		public void Add(Statement statement)
		{
			AddChild(statement, StatementRole);
		}

		public void Add(Expression expression)
		{
			AddChild(new ExpressionStatement(expression), StatementRole);
		}

		IEnumerator<Statement> IEnumerable<Statement>.GetEnumerator()
		{
			return this.Statements.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Statements.GetEnumerator();
		}
	}
}
