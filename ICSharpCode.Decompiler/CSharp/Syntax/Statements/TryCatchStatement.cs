﻿// 
// TryCatchStatement.cs
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
	/// try TryBlock CatchClauses finally FinallyBlock
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class TryCatchStatement : Statement
	{
		public static readonly TokenRole TryKeywordRole = new TokenRole("try");
		public static readonly Role<BlockStatement> TryBlockRole = new Role<BlockStatement>("TryBlock", BlockStatement.Null);
		public static readonly Role<CatchClause> CatchClauseRole = new Role<CatchClause>("CatchClause", CatchClause.Null);
		public static readonly TokenRole FinallyKeywordRole = new TokenRole("finally");
		public static readonly Role<BlockStatement> FinallyBlockRole = new Role<BlockStatement>("FinallyBlock", BlockStatement.Null);

		public CSharpTokenNode TryToken {
			get { return GetChildByRole(TryKeywordRole); }
		}

		public BlockStatement TryBlock {
			get { return GetChildByRole(TryBlockRole); }
			set { SetChildByRole(TryBlockRole, value); }
		}

		public AstNodeCollection<CatchClause> CatchClauses {
			get { return GetChildrenByRole(CatchClauseRole); }
		}

		public CSharpTokenNode FinallyToken {
			get { return GetChildByRole(FinallyKeywordRole); }
		}

		public BlockStatement FinallyBlock {
			get { return GetChildByRole(FinallyBlockRole); }
			set { SetChildByRole(FinallyBlockRole, value); }
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			TryCatchStatement o = other as TryCatchStatement;
			return o != null && this.TryBlock.DoMatch(o.TryBlock, match) && this.CatchClauses.DoMatch(o.CatchClauses, match) && this.FinallyBlock.DoMatch(o.FinallyBlock, match);
		}
	}

	/// <summary>
	/// catch (Type VariableName) { Body }
	/// </summary>
	[DecompilerAstNode(hasNullNode: true, hasPatternPlaceholder: true)]
	public partial class CatchClause : AstNode
	{
		public static readonly TokenRole CatchKeywordRole = new TokenRole("catch");
		public static readonly TokenRole WhenKeywordRole = new TokenRole("when");
		public static readonly Role<Expression> ConditionRole = Roles.Condition;
		public static readonly TokenRole CondLPar = new TokenRole("(");
		public static readonly TokenRole CondRPar = new TokenRole(")");

		public override NodeType NodeType {
			get {
				return NodeType.Unknown;
			}
		}

		public CSharpTokenNode CatchToken {
			get { return GetChildByRole(CatchKeywordRole); }
		}

		public CSharpTokenNode LParToken {
			get { return GetChildByRole(Roles.LPar); }
		}

		public AstType Type {
			get { return GetChildByRole(Roles.Type); }
			set { SetChildByRole(Roles.Type, value); }
		}

		public string VariableName {
			get { return GetChildByRole(Roles.Identifier).Name; }
			set {
				if (string.IsNullOrEmpty(value))
					SetChildByRole(Roles.Identifier, null);
				else
					SetChildByRole(Roles.Identifier, Identifier.Create(value));
			}
		}

		public Identifier VariableNameToken {
			get {
				return GetChildByRole(Roles.Identifier);
			}
			set {
				SetChildByRole(Roles.Identifier, value);
			}
		}

		public CSharpTokenNode RParToken {
			get { return GetChildByRole(Roles.RPar); }
		}

		public CSharpTokenNode WhenToken {
			get { return GetChildByRole(WhenKeywordRole); }
		}

		public CSharpTokenNode CondLParToken {
			get { return GetChildByRole(CondLPar); }
		}

		public Expression Condition {
			get { return GetChildByRole(ConditionRole); }
			set { SetChildByRole(ConditionRole, value); }
		}

		public CSharpTokenNode CondRParToken {
			get { return GetChildByRole(CondRPar); }
		}

		public BlockStatement Body {
			get { return GetChildByRole(Roles.Body); }
			set { SetChildByRole(Roles.Body, value); }
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			CatchClause o = other as CatchClause;
			return o != null && this.Type.DoMatch(o.Type, match) && MatchString(this.VariableName, o.VariableName) && this.Body.DoMatch(o.Body, match);
		}
	}
}
