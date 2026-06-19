// Copyright (c) 2023 Daniel Grunwald
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
	/// <code>
	/// recursive_pattern ::=
	///       type? '{' pattern* '}' variable_designation?
	///     | type? '(' pattern* ')' variable_designation?
	/// </code>
	/// (C# grammar §11.2.5, §11.2.6)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class RecursivePatternExpression : Expression
	{
		[Slot("Type")]
		public partial AstType? Type { get; set; }

		[Slot("SubPattern")]
		public partial AstNodeCollection<Expression> SubPatterns { get; }

		[Slot("VariableDesignation")]
		public partial VariableDesignation? Designation { get; set; }

		public bool IsPositional { get; set; }
	}
}
