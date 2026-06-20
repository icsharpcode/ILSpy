// 
// MethodDeclaration.cs
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
	/// <c>method_declaration ::= attribute_section* modifier* type ( type '.' )? identifier type_parameter* '(' parameter* ')' constraint* ( block | ';' )</c> (C# grammar §15.6.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class MethodDeclaration : EntityDeclaration
	{
		public override SymbolKind SymbolKind {
			get { return SymbolKind.Method; }
		}

		// Slots in document order; inherited contract slots (Attributes, ReturnType, NameToken) are
		// re-declared override per the flatten model (Part I.3).
		[Slot("AttributeSection")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Type")]
		public override partial AstType ReturnType { get; set; }

		/// <summary>
		/// Gets/Sets the type reference of the interface that is explicitly implemented.
		/// Null if this member is not an explicit interface implementation.
		/// </summary>
		[Slot("PrivateImplementationType")]
		public partial AstType? PrivateImplementationType { get; set; }

		[Slot("Identifier")]
		public override partial Identifier NameToken { get; set; }

		[Slot("TypeParameter")]
		public partial AstNodeCollection<TypeParameterDeclaration> TypeParameters { get; }

		[Slot("Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> Parameters { get; }

		[Slot("Constraint")]
		public partial AstNodeCollection<Constraint> Constraints { get; }

		[Slot("Body")]
		public partial BlockStatement? Body { get; set; }

		public bool IsExtensionMethod {
			get {
				ParameterDeclaration? pd = GetChild(Slots.Parameter);
				return pd != null && pd.HasThisModifier;
			}
		}
	}
}
