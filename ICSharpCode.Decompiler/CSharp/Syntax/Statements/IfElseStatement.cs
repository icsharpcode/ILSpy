// 
// IfElseStatement.cs
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
	/// <c>if_statement ::= 'if' '(' expression ')' statement ( 'else' statement )?</c> (C# grammar §13.8.2)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class IfElseStatement : Statement
	{
		public readonly static TokenRole IfKeywordRole = new TokenRole("if");
		public readonly static TokenRole ElseKeywordRole = new TokenRole("else");

		[Slot("ConditionRole")]
		public partial Expression Condition { get; set; }

		[Slot("TrueRole")]
		public partial Statement TrueStatement { get; set; }

		[Slot("FalseRole")]
		public partial Statement? FalseStatement { get; set; }

		public IfElseStatement()
		{
		}

		public IfElseStatement(Expression condition, Statement trueStatement, Statement? falseStatement = null)
		{
			this.Condition = condition;
			this.TrueStatement = trueStatement;
			this.FalseStatement = falseStatement;
		}
	}
}
