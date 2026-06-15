// 
// ParameterDeclarationExpression.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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
	/// <c>fixed_parameter ::= attribute_section* ( 'this' | 'scoped'? ( 'ref' 'readonly'? | 'out' | 'in' ) )? type identifier ( '=' expression )?</c> (C# grammar §15.6.2.1)
	/// <c>parameter_array ::= attribute_section* 'params' type identifier</c> (C# grammar §15.6.2.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false, hasPatternPlaceholder: true)]
	public partial class ParameterDeclaration : AstNode
	{
		public static readonly Role<AttributeSection> AttributeRole = EntityDeclaration.AttributeRole;
		public static readonly TokenRole ThisModifierRole = new TokenRole("this");
		public static readonly TokenRole ScopedRefRole = new TokenRole("scoped");
		public static readonly TokenRole RefModifierRole = new TokenRole("ref");
		public static readonly TokenRole ReadonlyModifierRole = ComposedType.ReadonlyRole;
		public static readonly TokenRole OutModifierRole = new TokenRole("out");
		public static readonly TokenRole InModifierRole = new TokenRole("in");
		public static readonly TokenRole ParamsModifierRole = new TokenRole("params");

		[Slot("AttributeRole")]
		public partial AstNodeCollection<AttributeSection> Attributes { get; }

		bool hasThisModifier;
		bool isParams;
		bool isScopedRef;

		public bool HasThisModifier {
			get { return hasThisModifier; }
			set {
				hasThisModifier = value;
			}
		}

		public bool IsParams {
			get { return isParams; }
			set {
				isParams = value;
			}
		}

		public bool IsScopedRef {
			get { return isScopedRef; }
			set {
				isScopedRef = value;
			}
		}

		ReferenceKind parameterModifier;

		public ReferenceKind ParameterModifier {
			get { return parameterModifier; }
			set {
				parameterModifier = value;
			}
		}

		[Slot("Roles.Type")]
		public partial AstType Type { get; set; }

		[NameSlot("Roles.Identifier")]
		public partial string Name { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression DefaultExpression { get; set; }

		public ParameterDeclaration()
		{
		}

		public new ParameterDeclaration Clone()
		{
			return (ParameterDeclaration)base.Clone();
		}
	}
}

