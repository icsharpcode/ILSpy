﻿// Copyright (c) 2018 Daniel Grunwald
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

using System.Collections.Immutable;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	[DecompilerAstNode(hasNullNode: false)]
	public partial class TupleAstType : AstType
	{
		public static readonly Role<TupleTypeElement> ElementRole = new Role<TupleTypeElement>("Element", TupleTypeElement.Null);

		public AstNodeCollection<TupleTypeElement> Elements {
			get { return GetChildrenByRole(ElementRole); }
		}

		public override ITypeReference ToTypeReference(NameLookupMode lookupMode, InterningProvider interningProvider = null)
		{
			return new TupleTypeReference(
				this.Elements.Select(e => e.Type.ToTypeReference(lookupMode, interningProvider)).ToImmutableArray(),
				this.Elements.Select(e => e.Name).ToImmutableArray()
			);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			return other is TupleAstType o && Elements.DoMatch(o.Elements, match);
		}
	}

	[DecompilerAstNode(hasNullNode: true)]
	public partial class TupleTypeElement : AstNode
	{
		public AstType Type {
			get { return GetChildByRole(Roles.Type); }
			set { SetChildByRole(Roles.Type, value); }
		}

		public string Name {
			get { return GetChildByRole(Roles.Identifier).Name; }
			set { SetChildByRole(Roles.Identifier, Identifier.Create(value)); }
		}

		public Identifier NameToken {
			get { return GetChildByRole(Roles.Identifier); }
			set { SetChildByRole(Roles.Identifier, value); }
		}

		public override NodeType NodeType => NodeType.Unknown;

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			return other is TupleTypeElement o
				&& Type.DoMatch(o.Type, match)
				&& MatchString(Name, o.Name);
		}
	}
}
