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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[DecompilerAstNode(hasNullNode: false)]
	public partial class PropertyDeclaration : EntityDeclaration
	{
		public static readonly TokenRole GetKeywordRole = new TokenRole("get");
		public static readonly TokenRole SetKeywordRole = new TokenRole("set");
		public static readonly TokenRole InitKeywordRole = new TokenRole("init");
		public static readonly Role<Accessor> GetterRole = new Role<Accessor>("Getter", Accessor.Null);
		public static readonly Role<Accessor> SetterRole = new Role<Accessor>("Setter", Accessor.Null);
		public static readonly Role<Expression> ExpressionBodyRole = new Role<Expression>("ExpressionBody", Expression.Null);

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
		public partial AstType PrivateImplementationType { get; set; }

		public CSharpTokenNode LBraceToken {
			get { return GetChildByRole(Roles.LBrace); }
		}

		[Slot("GetterRole")]
		public partial Accessor Getter { get; set; }

		[Slot("SetterRole")]
		public partial Accessor Setter { get; set; }

		public CSharpTokenNode RBraceToken {
			get { return GetChildByRole(Roles.RBrace); }
		}

		public CSharpTokenNode AssignToken {
			get { return GetChildByRole(Roles.Assign); }
		}

		[Slot("Roles.Expression")]
		public partial Expression Initializer { get; set; }

		[Slot("ExpressionBodyRole")]
		public partial Expression ExpressionBody { get; set; }

		public bool IsAutomaticProperty {
			get {
				if (!Getter.IsNull && !Getter.Body.IsNull)
				{
					return false;
				}

				if (!Setter.IsNull && !Setter.Body.IsNull)
				{
					return false;
				}

				return true;
			}
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			PropertyDeclaration o = other as PropertyDeclaration;
			return o != null && MatchString(this.Name, o.Name)
				&& this.MatchAttributesAndModifiers(o, match) && this.ReturnType.DoMatch(o.ReturnType, match)
				&& this.PrivateImplementationType.DoMatch(o.PrivateImplementationType, match)
				&& this.Getter.DoMatch(o.Getter, match) && this.Setter.DoMatch(o.Setter, match)
				&& this.Initializer.DoMatch(o.Initializer, match)
				&& this.ExpressionBody.DoMatch(o.ExpressionBody, match);
		}
	}
}
