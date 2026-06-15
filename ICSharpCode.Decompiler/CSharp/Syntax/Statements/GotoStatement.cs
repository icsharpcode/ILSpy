// 
// GotoStatement.cs
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
	/// <c>goto_statement ::= 'goto' identifier ';'</c> (C# grammar §13.10.4)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class GotoStatement : Statement
	{
		public static readonly TokenRole GotoKeywordRole = new TokenRole("goto");

		public GotoStatement()
		{
		}

		public GotoStatement(string label)
		{
			this.Label = label;
		}

		public string Label {
			get {
				return NameToken.Name;
			}
			set {
				if (string.IsNullOrEmpty(value))
					NameToken = null;
				else
					NameToken = Identifier.Create(value);
			}
		}

		[Slot("Roles.Identifier")]
		public partial Identifier NameToken { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			GotoStatement o = other as GotoStatement;
			return o != null && MatchString(this.Label, o.Label);
		}
	}

	/// <summary>
	/// <c>goto_statement ::= 'goto' 'case' expression ';'</c> (C# grammar §13.10.4)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class GotoCaseStatement : Statement
	{
		public static readonly TokenRole GotoKeywordRole = new TokenRole("goto");
		public static readonly TokenRole CaseKeywordRole = new TokenRole("case");

		/// <summary>
		/// Used for "goto case LabelExpression;"
		/// </summary>
		[Slot("Roles.Expression")]
		public partial Expression LabelExpression { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			GotoCaseStatement o = other as GotoCaseStatement;
			return o != null && this.LabelExpression.DoMatch(o.LabelExpression, match);
		}
	}

	/// <summary>
	/// <c>goto_statement ::= 'goto' 'default' ';'</c> (C# grammar §13.10.4)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class GotoDefaultStatement : Statement
	{
		public static readonly TokenRole GotoKeywordRole = new TokenRole("goto");
		public static readonly TokenRole DefaultKeywordRole = new TokenRole("default");

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			GotoDefaultStatement o = other as GotoDefaultStatement;
			return o != null;
		}
	}
}
