// 
// LambdaExpression.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
	/// <c>lambda_expression : attributes? anonymous_function_modifier? (return_type | ref_kind ref_return_type)? anonymous_function_signature '=&gt;' anonymous_function_body ;</c> (C# grammar §12.22.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class LambdaExpression : Expression
	{
		public static readonly Role<AttributeSection> AttributeRole = new Role<AttributeSection>("Attribute", null);
		public readonly static TokenRole AsyncModifierRole = new TokenRole("async");
		public static readonly Role<AstNode> BodyRole = new Role<AstNode>("Body", AstNode.Null);

		bool isAsync;

		[Slot("AttributeRole")]
		public partial AstNodeCollection<AttributeSection> Attributes { get; }

		public bool IsAsync {
			get { return isAsync; }
			set { isAsync = value; }
		}

		[Slot("Roles.Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> Parameters { get; }

		[Slot("BodyRole")]
		public partial AstNode Body { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			LambdaExpression o = other as LambdaExpression;
			return o != null && this.IsAsync == o.IsAsync && this.Parameters.DoMatch(o.Parameters, match) && this.Body.DoMatch(o.Body, match);
		}
	}
}
