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

#nullable enable

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>constructor_declaration ::= attribute_section* modifier* identifier '(' parameter* ')' constructor_initializer? ( block | ';' )</c> (C# grammar §15.11.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class ConstructorDeclaration : EntityDeclaration
	{
		public override SymbolKind SymbolKind {
			get { return SymbolKind.Constructor; }
		}

		[Slot("AttributeSection")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		// The name is just the declaring type name; exclude it from matching (and the inherited Name match).
		[ExcludeFromMatch]
		[Slot("Identifier")]
		public override partial Identifier NameToken { get; set; }

		[Slot("Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> Parameters { get; }

		[Slot("ConstructorInitializer")]
		public partial ConstructorInitializer? Initializer { get; set; }

		[Slot("Body")]
		public partial BlockStatement? Body { get; set; }
	}

	public enum ConstructorInitializerType
	{
		Any,
		Base,
		This
	}

	/// <summary>
	/// <c>constructor_initializer ::= ':' ( 'base' | 'this' ) '(' expression* ')'</c> (C# grammar §15.11.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class ConstructorInitializer : AstNode
	{
		public const string BaseKeyword = "base";
		public const string ThisKeyword = "this";

		public ConstructorInitializerType ConstructorInitializerType { get; set; }

		[Slot("Argument")]
		public partial AstNodeCollection<Expression> Arguments { get; }
	}
}
