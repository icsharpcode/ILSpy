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

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>switch_statement ::= 'switch' expression switch_section*</c> (C# grammar §13.8.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class SwitchStatement : Statement
	{
		public const string SwitchKeyword = "switch";

		[Slot("Expression")]
		public partial Expression Expression { get; set; }

		[Slot("SwitchSection")]
		public partial AstNodeCollection<SwitchSection> SwitchSections { get; }
	}

	/// <summary>
	/// <c>switch_section ::= switch_label+ statement*</c> (C# grammar §13.8.3)
	/// </summary>
	[DecompilerAstNode(hasPatternPlaceholder: true)]
	public partial class SwitchSection : AstNode
	{
		[Slot("CaseLabel")]
		public partial AstNodeCollection<CaseLabel> CaseLabels { get; }

		[Slot("EmbeddedStatement")]
		public partial AstNodeCollection<Statement> Statements { get; }
	}

	/// <summary>
	/// <c>switch_label ::= 'case' expression ':' | 'default' ':'</c> (C# grammar §13.8.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class CaseLabel : AstNode
	{
		public const string CaseKeyword = "case";
		public const string DefaultKeyword = "default";

		/// <summary>
		/// Gets or sets the expression. The expression can be null - if the expression is null, it's the default switch section.
		/// </summary>
		[Slot("Expression")]
		public partial Expression? Expression { get; set; }
	}
}
