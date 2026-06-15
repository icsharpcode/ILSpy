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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Represents a type parameter.
	/// Mirroring the C# syntax, constraints are not part of the type parameter declaration;
	/// they belong to the parent type or method.
	/// <c>type_parameter ::= attribute_section* ( 'in' | 'out' )? identifier</c> (C# grammar §8.5, §15.2.3)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class TypeParameterDeclaration : AstNode
	{
		public static readonly Role<AttributeSection> AttributeRole = EntityDeclaration.AttributeRole;
		public static readonly TokenRole OutVarianceKeywordRole = new TokenRole("out");
		public static readonly TokenRole InVarianceKeywordRole = new TokenRole("in");

		[Slot("AttributeRole")]
		public partial AstNodeCollection<AttributeSection> Attributes { get; }

		VarianceModifier variance;

		public VarianceModifier Variance {
			get { return variance; }
			set { variance = value; }
		}

		public string Name {
			get {
				return GetChildByRole(Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, Identifier.Create(value));
			}
		}

		// DoMatch compares the name string; exclude the token slot to avoid matching it twice.
		[ExcludeFromMatch]
		[Slot("Roles.Identifier")]
		public partial Identifier NameToken { get; set; }

		public TypeParameterDeclaration()
		{
		}

		public TypeParameterDeclaration(string name)
		{
			Name = name;
		}
	}
}
