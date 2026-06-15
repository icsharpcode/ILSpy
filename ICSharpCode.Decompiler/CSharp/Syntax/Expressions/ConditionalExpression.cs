// 
// ConditionalExpression.cs
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
	/// Operator precedence is not represented in the syntax tree; required parentheses are reconstructed by <see cref="ICSharpCode.Decompiler.CSharp.OutputVisitor.InsertParenthesesVisitor"/>.
	/// <c>conditional_expression ::= expression '?' expression ':' expression</c> (C# grammar §12.21)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class ConditionalExpression : Expression
	{
		public readonly static Role<Expression> ConditionRole = Roles.Condition;
		public readonly static TokenRole QuestionMarkRole = new TokenRole("?");
		public readonly static Role<Expression> TrueRole = new Role<Expression>("True", Expression.Null);
		public readonly static TokenRole ColonRole = Roles.Colon;
		public readonly static Role<Expression> FalseRole = new Role<Expression>("False", Expression.Null);

		[Slot("ConditionRole")]
		public partial Expression Condition { get; set; }

		[Slot("TrueRole")]
		public partial Expression TrueExpression { get; set; }

		[Slot("FalseRole")]
		public partial Expression FalseExpression { get; set; }

		public ConditionalExpression()
		{
		}

		public ConditionalExpression(Expression condition, Expression trueExpression, Expression falseExpression)
		{
			AddChild(condition, ConditionRole);
			AddChild(trueExpression, TrueRole);
			AddChild(falseExpression, FalseRole);
		}
	}
}
