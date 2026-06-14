// 
// ConstructorDeclaration.cs
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
	/// <c>constructor_declaration : attributes? constructor_modifier* constructor_declarator constructor_body ;</c> (C# grammar §15.11.1)
	/// <c>static_constructor_declaration : attributes? static_constructor_modifiers identifier '(' ')' static_constructor_body ;</c> (C# grammar §15.12)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class ConstructorDeclaration : EntityDeclaration
	{
		public static readonly Role<ConstructorInitializer> InitializerRole = new Role<ConstructorInitializer>("Initializer", ConstructorInitializer.Null);

		public override SymbolKind SymbolKind {
			get { return SymbolKind.Constructor; }
		}

		[Slot("AttributeRole")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Roles.Identifier")]
		public override partial Identifier NameToken { get; set; }

		[Slot("Roles.Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> Parameters { get; }

		[Slot("InitializerRole")]
		public partial ConstructorInitializer Initializer { get; set; }

		[Slot("Roles.Body")]
		public partial BlockStatement Body { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			ConstructorDeclaration o = other as ConstructorDeclaration;
			return o != null && this.MatchAttributesAndModifiers(o, match) && this.Parameters.DoMatch(o.Parameters, match)
				&& this.Initializer.DoMatch(o.Initializer, match) && this.Body.DoMatch(o.Body, match);
		}
	}

	public enum ConstructorInitializerType
	{
		Any,
		Base,
		This
	}

	/// <summary>
	/// <c>constructor_initializer : ':' 'base' '(' argument_list? ')' | ':' 'this' '(' argument_list? ')' ;</c> (C# grammar §15.11.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: true)]
	public partial class ConstructorInitializer : AstNode
	{
		public static readonly TokenRole BaseKeywordRole = new TokenRole("base");
		public static readonly TokenRole ThisKeywordRole = new TokenRole("this");

		public override NodeType NodeType {
			get {
				return NodeType.Unknown;
			}
		}

		public ConstructorInitializerType ConstructorInitializerType {
			get;
			set;
		}

		[Slot("Roles.Argument")]
		public partial AstNodeCollection<Expression> Arguments { get; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			ConstructorInitializer o = other as ConstructorInitializer;
			return o != null && !o.IsNull
				&& (this.ConstructorInitializerType == ConstructorInitializerType.Any || this.ConstructorInitializerType == o.ConstructorInitializerType)
				&& this.Arguments.DoMatch(o.Arguments, match);
		}
	}
}
