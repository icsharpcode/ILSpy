// 
// SwitchStatement.cs
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
	/// <c>switch_statement : 'switch' selector_expression switch_block ;</c> (C# grammar §13.8.3)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class SwitchStatement : Statement
	{
		public static readonly TokenRole SwitchKeywordRole = new TokenRole("switch");
		public static readonly Role<SwitchSection> SwitchSectionRole = new Role<SwitchSection>("SwitchSection", null);

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		[Slot("SwitchSectionRole")]
		public partial AstNodeCollection<SwitchSection> SwitchSections { get; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			SwitchStatement o = other as SwitchStatement;
			return o != null && this.Expression.DoMatch(o.Expression, match) && this.SwitchSections.DoMatch(o.SwitchSections, match);
		}
	}

	/// <summary>
	/// <c>switch_section : switch_label+ statement_list ;</c> (C# grammar §13.8.3)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false, hasPatternPlaceholder: true)]
	public partial class SwitchSection : AstNode
	{
		public static readonly Role<CaseLabel> CaseLabelRole = new Role<CaseLabel>("CaseLabel", null);

		public override NodeType NodeType {
			get {
				return NodeType.Unknown;
			}
		}

		[Slot("CaseLabelRole")]
		public partial AstNodeCollection<CaseLabel> CaseLabels { get; }

		[Slot("Roles.EmbeddedStatement")]
		public partial AstNodeCollection<Statement> Statements { get; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			SwitchSection o = other as SwitchSection;
			return o != null && this.CaseLabels.DoMatch(o.CaseLabels, match) && this.Statements.DoMatch(o.Statements, match);
		}
	}

	/// <summary>
	/// <c>switch_label : 'case' pattern case_guard? ':' | 'default' ':' ;</c> (C# grammar §13.8.3)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class CaseLabel : AstNode
	{
		public static readonly TokenRole CaseKeywordRole = new TokenRole("case");
		public static readonly TokenRole DefaultKeywordRole = new TokenRole("default");

		public override NodeType NodeType {
			get {
				return NodeType.Unknown;
			}
		}

		/// <summary>
		/// Gets or sets the expression. The expression can be null - if the expression is null, it's the default switch section.
		/// </summary>
		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		public CaseLabel()
		{
		}

		public CaseLabel(Expression expression)
		{
			this.Expression = expression;
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			CaseLabel o = other as CaseLabel;
			return o != null && this.Expression.DoMatch(o.Expression, match);
		}
	}
}
