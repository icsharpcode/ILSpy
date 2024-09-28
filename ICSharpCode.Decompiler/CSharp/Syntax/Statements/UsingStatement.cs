﻿// 
// UsingStatement.cs
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
	/// [ await ] using (ResourceAcquisition) EmbeddedStatement
	/// </summary>
	public class UsingStatement : Statement
	{
		public static readonly TokenRole UsingKeywordRole = new TokenRole("using");
		public static readonly TokenRole AwaitRole = UnaryOperatorExpression.AwaitRole;
		public static readonly Role<AstNode> ResourceAcquisitionRole = new Role<AstNode>("ResourceAcquisition", AstNode.Null);

		public CSharpTokenNode UsingToken {
			get { return GetChildByRole(UsingKeywordRole); }
		}

		public CSharpTokenNode AwaitToken {
			get { return GetChildByRole(AwaitRole); }
		}

		public bool IsAsync {
			get { return !GetChildByRole(AwaitRole).IsNull; }
			set { SetChildByRole(AwaitRole, value ? new CSharpTokenNode(TextLocation.Empty, null) : null); }
		}

		public CSharpTokenNode LParToken {
			get { return GetChildByRole(Roles.LPar); }
		}

		public bool IsEnhanced { get; set; }

		/// <summary>
		/// Either a VariableDeclarationStatement, or an Expression.
		/// </summary>
		public AstNode ResourceAcquisition {
			get { return GetChildByRole(ResourceAcquisitionRole); }
			set { SetChildByRole(ResourceAcquisitionRole, value); }
		}

		public CSharpTokenNode RParToken {
			get { return GetChildByRole(Roles.RPar); }
		}

		public Statement EmbeddedStatement {
			get { return GetChildByRole(Roles.EmbeddedStatement); }
			set { SetChildByRole(Roles.EmbeddedStatement, value); }
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitUsingStatement(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitUsingStatement(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitUsingStatement(this, data);
		}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{
			UsingStatement? o = other as UsingStatement;
			return o != null && this.IsAsync == o.IsAsync && this.ResourceAcquisition.DoMatch(o.ResourceAcquisition, match) && this.EmbeddedStatement.DoMatch(o.EmbeddedStatement, match);
		}
	}
}
