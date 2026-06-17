// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public abstract class EntityDeclaration : AstNode
	{
		// Modifiers are stored in the Modifiers scalar; this role only tags the keyword tokens the
		// output visitor emits (e.g. for the override-link reference in TextTokenWriter).
		// Identity marker for modifier keywords in the output (modifiers are scalars, not child nodes);
		// the token writer uses it to make the 'override' modifier a go-to-definition reference.
		public static readonly TokenRole ModifierRole = new TokenRole("modifier");

		public abstract SymbolKind SymbolKind { get; }

		public virtual AstNodeCollection<AttributeSection> Attributes {
			get { return base.GetChildrenByRole<AttributeSection>(SlotKind.Attribute); }
		}

		public Modifiers Modifiers { get; set; }

		public bool HasModifier(Modifiers mod)
		{
			return (Modifiers & mod) == mod;
		}

		public virtual string Name {
			get {
				return GetChildByRole<Identifier>(SlotKind.Identifier)?.Name ?? string.Empty;
			}
			set {
				SetChildByRole(SlotKind.Identifier, Identifier.Create(value, TextLocation.Empty));
			}
		}

		public virtual Identifier NameToken {
			get { return GetChildByRole<Identifier>(SlotKind.Identifier); }
			set { SetChildByRole(SlotKind.Identifier, value); }
		}

		public virtual AstType ReturnType {
			get { return GetChildByRole<AstType>(SlotKind.Type); }
			set { SetChildByRole(SlotKind.Type, value); }
		}

		protected bool MatchAttributesAndModifiers(EntityDeclaration o, PatternMatching.Match match)
		{
			return (this.Modifiers == Modifiers.Any || this.Modifiers == o.Modifiers) && this.Attributes.DoMatch(o.Attributes, match);
		}
	}
}
