// 
// ForStatement.cs
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
	/// <c>for_statement ::= 'for' '(' statement* ';' expression? ';' statement* ')' statement</c> (C# grammar §13.9.4)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class ForStatement : Statement
	{
		public const string ForKeyword = "for";

		/// <summary>
		/// Gets the list of initializer statements.
		/// Note: this contains multiple statements for "for (a = 2, b = 1; a > b; a--)", but contains
		/// only a single statement for "for (int a = 2, b = 1; a > b; a--)" (a single VariableDeclarationStatement with two variables)
		/// </summary>
		[Slot("ForInitializer")]
		public partial AstNodeCollection<Statement> Initializers { get; }

		[Slot("Condition")]
		public partial Expression? Condition { get; set; }

		[Slot("Iterator")]
		public partial AstNodeCollection<Statement> Iterators { get; }

		[Slot("EmbeddedStatement")]
		public partial Statement EmbeddedStatement { get; set; }
	}
}
