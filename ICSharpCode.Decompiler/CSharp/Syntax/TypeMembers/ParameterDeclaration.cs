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

		public override NodeType NodeType => NodeType.Unknown;

		public AstNodeCollection<AttributeSection> Attributes {
			get { return GetChildrenByRole(AttributeRole); }
		}

		bool hasThisModifier;
		bool isParams;
		bool isScopedRef;

		public CSharpTokenNode ThisKeyword {
			get {
				if (hasThisModifier)
				{
					return GetChildByRole(ThisModifierRole);
				}
				return CSharpTokenNode.Null;
			}
		}

		public bool HasThisModifier {
			get { return hasThisModifier; }
			set {
				ThrowIfFrozen();
				hasThisModifier = value;
			}
		}

		public bool IsParams {
			get { return isParams; }
			set {
				ThrowIfFrozen();
				isParams = value;
			}
		}

		public bool IsScopedRef {
			get { return isScopedRef; }
			set {
				ThrowIfFrozen();
				isScopedRef = value;
			}
		}

		ReferenceKind parameterModifier;

		public ReferenceKind ParameterModifier {
			get { return parameterModifier; }
			set {
				ThrowIfFrozen();
				parameterModifier = value;
			}
		}

		public AstType Type {
			get { return GetChildByRole(Roles.Type); }
			set { SetChildByRole(Roles.Type, value); }
		}

		public string Name {
			get {
				return GetChildByRole(Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, Identifier.Create(value));
			}
		}

		public Identifier NameToken {
			get {
				return GetChildByRole(Roles.Identifier);
			}
			set {
				SetChildByRole(Roles.Identifier, value);
			}
		}

		public CSharpTokenNode AssignToken {
			get { return GetChildByRole(Roles.Assign); }
		}

		public Expression DefaultExpression {
			get { return GetChildByRole(Roles.Expression); }
			set { SetChildByRole(Roles.Expression, value); }
		}

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{
			var o = other as ParameterDeclaration;
			return o != null && this.Attributes.DoMatch(o.Attributes, match) && this.ParameterModifier == o.ParameterModifier
				&& this.Type.DoMatch(o.Type, match) && MatchString(this.Name, o.Name)
				&& this.DefaultExpression.DoMatch(o.DefaultExpression, match);
		}

		public ParameterDeclaration()
		{
		}

		public new ParameterDeclaration Clone()
		{
			return (ParameterDeclaration)base.Clone();
		}
	}
}

