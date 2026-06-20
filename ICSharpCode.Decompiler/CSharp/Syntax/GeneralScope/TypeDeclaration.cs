// 
// TypeDeclaration.cs
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
	public enum ClassType
	{
		Class,
		Struct,
		Interface,
		Enum,
		/// <summary>
		/// C# 9 'record class'
		/// </summary>
		RecordClass,
		/// <summary>
		/// C# 10 'record struct'
		/// </summary>
		RecordStruct,
	}

	/// <summary>
	/// <c>type_declaration ::= attribute_section* modifier* ( 'class' | 'struct' | 'interface' | 'enum' | 'record' 'class'? | 'record' 'struct' ) identifier type_parameter* ( '(' parameter* ')' )? ( ':' type+ )? constraint* '{' member* '}' ';'?</c> (C# grammar §15.2.1, §16.2.1, §19.2.1, §20.2)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class TypeDeclaration : EntityDeclaration
	{
		public override SymbolKind SymbolKind {
			get { return SymbolKind.TypeDefinition; }
		}

		public ClassType ClassType { get; set; }

		[Slot("AttributeSection")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Identifier")]
		public override partial Identifier NameToken { get; set; }

		[Slot("TypeParameter")]
		public partial AstNodeCollection<TypeParameterDeclaration> TypeParameters { get; }

		public bool HasPrimaryConstructor { get; set; }

		[Slot("Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> PrimaryConstructorParameters { get; }

		[Slot("BaseType")]
		public partial AstNodeCollection<AstType> BaseTypes { get; }

		[Slot("Constraint")]
		public partial AstNodeCollection<Constraint> Constraints { get; }

		[Slot("TypeMember")]
		public partial AstNodeCollection<EntityDeclaration> Members { get; }
	}
}
