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
	[DecompilerAstNode]
	public sealed partial class TryCatchStatement : Statement
	{
		public const string TryKeyword = "try";
		public const string FinallyKeyword = "finally";

		[Slot("TryBlock")]
		public partial BlockStatement TryBlock { get; set; }

		[Slot("CatchClause")]
		public partial AstNodeCollection<CatchClause> CatchClauses { get; }

		[Slot("FinallyBlock")]
		public partial BlockStatement? FinallyBlock { get; set; }
	}

	/// <summary>
	/// <c>catch_clause ::= 'catch' ( '(' type identifier? ')' )? ( 'when' '(' expression ')' )? block</c> (C# grammar §13.11)
	/// </summary>
	[DecompilerAstNode(hasPatternPlaceholder: true)]
	public partial class CatchClause : AstNode
	{
		public const string CatchKeyword = "catch";
		public const string WhenKeyword = "when";
		public const string CondLPar = "(";
		public const string CondRPar = ")";

		[Slot("Type")]
		public partial AstType? Type { get; set; }

		[Slot("Identifier")]
		public partial string? VariableName { get; set; }

		[Slot("Condition")]
		public partial Expression? Condition { get; set; }

		[Slot("Body")]
		public partial BlockStatement Body { get; set; }
	}
}
