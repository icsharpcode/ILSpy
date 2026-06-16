// 
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

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>try_statement ::= 'try' block catch_clause* ( 'finally' block )?</c> (C# grammar §13.11)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class TryCatchStatement : Statement
	{
		public static readonly TokenRole TryKeywordRole = new TokenRole("try");
		public static readonly Role<BlockStatement> TryBlockRole = new Role<BlockStatement>("TryBlock", null);
		public static readonly Role<CatchClause> CatchClauseRole = new Role<CatchClause>("CatchClause", null);
		public static readonly TokenRole FinallyKeywordRole = new TokenRole("finally");
		public static readonly Role<BlockStatement> FinallyBlockRole = new Role<BlockStatement>("FinallyBlock", null);

		[Slot("TryBlockRole")]
		public partial BlockStatement TryBlock { get; set; }

		[Slot("CatchClauseRole")]
		public partial AstNodeCollection<CatchClause> CatchClauses { get; }

		[Slot("FinallyBlockRole")]
		public partial BlockStatement? FinallyBlock { get; set; }
	}

	/// <summary>
	/// <c>catch_clause ::= 'catch' ( '(' type identifier? ')' )? ( 'when' '(' expression ')' )? block</c> (C# grammar §13.11)
	/// </summary>
	[DecompilerAstNode(hasNullNode: true, hasPatternPlaceholder: true)]
	public partial class CatchClause : AstNode
	{
		public static readonly TokenRole CatchKeywordRole = new TokenRole("catch");
		public static readonly TokenRole WhenKeywordRole = new TokenRole("when");
		public static readonly Role<Expression> ConditionRole = Roles.Condition;
		public static readonly TokenRole CondLPar = new TokenRole("(");
		public static readonly TokenRole CondRPar = new TokenRole(")");

		[Slot("Roles.Type")]
		public partial AstType? Type { get; set; }

		[NameSlot("Roles.Identifier", nullOnEmpty: true)]
		public partial string VariableName { get; set; }

		[Slot("ConditionRole")]
		public partial Expression? Condition { get; set; }

		[Slot("Roles.Body")]
		public partial BlockStatement Body { get; set; }
	}
}
