// Copyright (c) 2020 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Operator precedence is not represented in the syntax tree; required parentheses are reconstructed by <see cref="ICSharpCode.Decompiler.CSharp.OutputVisitor.InsertParenthesesVisitor"/>.
	/// <c>switch_expression ::= expression 'switch' '{' switch_expression_arm* '}'</c> (C# grammar §12.12)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class SwitchExpression : Expression
	{
		public const string SwitchKeyword = "switch";

		[Slot("Expression")]
		public partial Expression Expression { get; set; }

		[Slot("SwitchExpressionSection")]
		public partial AstNodeCollection<SwitchExpressionSection> SwitchSections { get; }
	}

	/// <summary>
	/// <c>switch_expression_arm ::= pattern '=&gt;' expression</c> (C# grammar §12.12)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class SwitchExpressionSection : AstNode
	{
		[Slot("Pattern")]
		public partial Expression Pattern { get; set; }

		[Slot("SwitchExpressionBody")]
		public partial Expression Body { get; set; }
	}
}
