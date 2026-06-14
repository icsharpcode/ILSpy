// 
// DestructorDeclaration.cs
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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>finalizer_declaration : attributes? '~' identifier '(' ')' finalizer_body | attributes? 'extern' unsafe_modifier? '~' identifier '(' ')' finalizer_body | attributes? unsafe_modifier 'extern'? '~' identifier '(' ')' finalizer_body ;</c> (C# grammar §15.13)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class DestructorDeclaration : EntityDeclaration
	{
		public static readonly TokenRole TildeRole = new TokenRole("~");

		public override SymbolKind SymbolKind {
			get { return SymbolKind.Destructor; }
		}

		[Slot("AttributeRole")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Roles.Identifier")]
		public override partial Identifier NameToken { get; set; }

		[Slot("Roles.Body")]
		public partial BlockStatement Body { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			DestructorDeclaration o = other as DestructorDeclaration;
			return o != null && this.MatchAttributesAndModifiers(o, match) && this.Body.DoMatch(o.Body, match);
		}
	}
}
