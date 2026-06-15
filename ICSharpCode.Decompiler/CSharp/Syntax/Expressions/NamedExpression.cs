// 
// NamedExpression.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2011 Xamarin
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
	/// No standalone expression production: 'name = value' as used in object initializers (member_initializer), anonymous-object members (member_declarator), and named attribute arguments.
	/// <c>named_expression ::= identifier '=' expression</c>
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class NamedExpression : Expression
	{
		public NamedExpression()
		{
		}

		public NamedExpression(string name, Expression expression)
		{
			this.Name = name;
			this.Expression = expression;
		}

		public string Name {
			get {
				return GetChildByRole(Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, Identifier.Create(value));
			}
		}

		// DoMatch compares the Name string; exclude the token slot to avoid matching it twice.
		[ExcludeFromMatch]
		[Slot("Roles.Identifier")]
		public partial Identifier NameToken { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }
	}
}
