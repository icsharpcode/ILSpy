// Copyright (c) 2025 Siegfried Pammer
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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Not present in this revision of the C# specification grammar: C# 14 extension blocks ('extension(receiver) { members }').
	/// <c>extension_declaration ::= attribute_section* 'extension' type_parameter* '(' parameter* ')' constraint* '{' member* '}'</c>
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class ExtensionDeclaration : EntityDeclaration
	{
		public readonly static TokenRole ExtensionKeywordRole = new TokenRole("extension");

		public override SymbolKind SymbolKind => SymbolKind.TypeDefinition;

		[Slot("AttributeRole")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Roles.TypeParameter")]
		public partial AstNodeCollection<TypeParameterDeclaration> TypeParameters { get; }

		[Slot("Roles.Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> ReceiverParameters { get; }

		[Slot("Roles.Constraint")]
		public partial AstNodeCollection<Constraint> Constraints { get; }

		[Slot("Roles.TypeMemberRole")]
		public partial AstNodeCollection<EntityDeclaration> Members { get; }

		public ExtensionDeclaration()
		{
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitExtensionDeclaration(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitExtensionDeclaration(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitExtensionDeclaration(this, data);
		}
	}
}
