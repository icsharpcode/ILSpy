// 
// VariableDeclarationStatement.cs
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
	/// <c>local_variable_declaration ::= type variable_initializer+</c> (C# grammar §13.6.2.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class VariableDeclarationStatement : Statement
	{
		public VariableDeclarationStatement(AstType type, string name, Expression? initializer = null)
		{
			this.Type = type;
			this.Variables.Add(new VariableInitializer(name, initializer));
		}

		public Modifiers Modifiers { get; set; }

		[Slot("Type")]
		public partial AstType Type { get; set; }

		[Slot("Variable")]
		public partial AstNodeCollection<VariableInitializer> Variables { get; }

		public VariableInitializer? GetVariable(string name)
		{
			return Variables.FirstOrNull(vi => vi.Name == name);
		}
	}
}
