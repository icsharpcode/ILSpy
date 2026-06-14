// 
// ForeachStatement.cs
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
	/// foreach (Type VariableName in InExpression) EmbeddedStatement
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class ForeachStatement : Statement
	{
		public static readonly TokenRole AwaitRole = UnaryOperatorExpression.AwaitRole;
		public static readonly TokenRole ForeachKeywordRole = new TokenRole("foreach");
		public static readonly TokenRole InKeywordRole = new TokenRole("in");

		public bool IsAsync { get; set; }

		[Slot("Roles.Type")]
		public partial AstType VariableType { get; set; }

		[Slot("Roles.VariableDesignationRole")]
		public partial VariableDesignation VariableDesignation { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression InExpression { get; set; }

		[Slot("Roles.EmbeddedStatement")]
		public partial Statement EmbeddedStatement { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			ForeachStatement o = other as ForeachStatement;
			return o != null && this.VariableType.DoMatch(o.VariableType, match) && this.VariableDesignation.DoMatch(o.VariableDesignation, match)
				&& this.InExpression.DoMatch(o.InExpression, match) && this.EmbeddedStatement.DoMatch(o.EmbeddedStatement, match);
		}
	}
}
