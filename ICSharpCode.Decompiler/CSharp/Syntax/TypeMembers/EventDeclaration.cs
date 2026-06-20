// 
// EventDeclaration.cs
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

using System;
using System.ComponentModel;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>event_declaration ::= attribute_section* modifier* 'event' type variable_initializer* ';'</c> (C# grammar §15.8.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class EventDeclaration : EntityDeclaration
	{
		public const string EventKeyword = "event";

		public override SymbolKind SymbolKind {
			get { return SymbolKind.Event; }
		}

		[Slot("AttributeSection")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Type")]
		public override partial AstType ReturnType { get; set; }

		[Slot("Variable")]
		public partial AstNodeCollection<VariableInitializer> Variables { get; }

		// Hide .Name and .NameToken from users; the actual field names
		// are stored in the VariableInitializer.
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Name {
			get { return string.Empty; }
			set { throw new NotSupportedException(); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Identifier NameToken {
			get { return null!; }
			set { throw new NotSupportedException(); }
		}
	}

	/// <summary>
	/// <c>custom_event_declaration ::= attribute_section* modifier* 'event' type ( type '.' )? identifier '{' accessor accessor '}'</c> (C# grammar §15.8.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class CustomEventDeclaration : EntityDeclaration
	{
		public const string EventKeyword = "event";

		public override SymbolKind SymbolKind {
			get { return SymbolKind.Event; }
		}

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

		[Slot("AddAccessor")]
		public partial Accessor? AddAccessor { get; set; }

		[Slot("RemoveAccessor")]
		public partial Accessor? RemoveAccessor { get; set; }
	}
}
