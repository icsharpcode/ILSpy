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

#nullable enable

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public abstract class EntityDeclaration : AstNode
	{
		public abstract SymbolKind SymbolKind { get; }

		public virtual AstNodeCollection<AttributeSection> Attributes {
			get { return base.GetChildren(Slots.AttributeSection); }
		}

		public Modifiers Modifiers { get; set; }

		public bool HasModifier(Modifiers mod)
		{
			return (Modifiers & mod) == mod;
		}

		public virtual string Name {
			get {
				return GetChild(Slots.Identifier)?.Name ?? string.Empty;
			}
			set {
				SetChild(Slots.Identifier, Identifier.Create(value, TextLocation.Empty));
			}
		}

		public virtual Identifier NameToken {
			get { return GetChild(Slots.Identifier)!; }
			set { SetChild(Slots.Identifier, value); }
		}

		public virtual AstType ReturnType {
			get { return GetChild(Slots.Type)!; }
			set { SetChild(Slots.Type, value); }
		}

		protected bool MatchAttributesAndModifiers(EntityDeclaration o, PatternMatching.Match match)
		{
			return (this.Modifiers == Modifiers.Any || this.Modifiers == o.Modifiers) && this.Attributes.DoMatch(o.Attributes, match);
		}
	}
}
