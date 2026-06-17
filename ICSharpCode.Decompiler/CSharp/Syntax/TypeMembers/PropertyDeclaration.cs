// 
// PropertyDeclaration.cs
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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <code>
	/// property_declaration ::=
	///       attribute_section* modifier* type ( type '.' )? identifier '{' accessor* '}' ( '=' expression ';' )?
	///     | attribute_section* modifier* type ( type '.' )? identifier '=&gt;' expression ';'
	/// </code>
	/// (C# grammar §15.7.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class PropertyDeclaration : EntityDeclaration
	{
		public static readonly TokenRole GetKeywordRole = new TokenRole("get");
		public static readonly TokenRole SetKeywordRole = new TokenRole("set");
		public static readonly TokenRole InitKeywordRole = new TokenRole("init");

		public override SymbolKind SymbolKind {
			get { return SymbolKind.Property; }
		}

		/// <summary>
		/// Gets/Sets the type reference of the interface that is explicitly implemented.
		/// Null node if this member is not an explicit interface implementation.
		/// </summary>
		[Slot("AttributeRole")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Roles.Type")]
		public override partial AstType ReturnType { get; set; }

		[Slot("Roles.Identifier")]
		public override partial Identifier NameToken { get; set; }

		[Slot("PrivateImplementationTypeRole")]
		public partial AstType? PrivateImplementationType { get; set; }

		[Slot("GetterRole")]
		public partial Accessor? Getter { get; set; }

		[Slot("SetterRole")]
		public partial Accessor? Setter { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression? Initializer { get; set; }

		[Slot("ExpressionBodyRole")]
		public partial Expression? ExpressionBody { get; set; }

		public bool IsAutomaticProperty {
			get {
				if (Getter is { Body: not null })
				{
					return false;
				}

				if (Setter is { Body: not null })
				{
					return false;
				}

				return true;
			}
		}
	}
}
