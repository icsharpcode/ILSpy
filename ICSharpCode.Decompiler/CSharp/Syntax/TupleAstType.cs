// Copyright (c) 2018 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>tuple_type ::= '(' tuple_type_element (',' tuple_type_element)+ ')'</c> (C# grammar §8.3.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class TupleAstType : AstType
	{
		public static readonly Role<TupleTypeElement> ElementRole = new Role<TupleTypeElement>("Element", TupleTypeElement.Null);

		[Slot("ElementRole")]
		public partial AstNodeCollection<TupleTypeElement> Elements { get; }
	}

	/// <summary>
	/// <c>tuple_type_element ::= type identifier?</c> (C# grammar §8.3.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: true)]
	public partial class TupleTypeElement : AstNode
	{
		[Slot("Roles.Type")]
		public partial AstType Type { get; set; }

		public string Name {
			get { return GetChildByRole(Roles.Identifier).Name; }
			set { SetChildByRole(Roles.Identifier, Identifier.Create(value)); }
		}

		// DoMatch compares the name string; exclude the token slot to avoid matching it twice.
		[ExcludeFromMatch]
		[Slot("Roles.Identifier")]
		public partial Identifier NameToken { get; set; }

		public override NodeType NodeType => NodeType.Unknown;
	}
}
