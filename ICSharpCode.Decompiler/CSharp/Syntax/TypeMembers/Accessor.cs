// 
// PropertyDeclaration.cs
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
	public enum AccessorKind
	{
		Any,
		Getter,
		Setter,
		Init,
		Adder,
		Remover
	}

	/// <summary>
	/// get/set/init/add/remove
	/// </summary>
	[DecompilerAstNode(hasNullNode: true)]
	public partial class Accessor : EntityDeclaration
	{
		public override NodeType NodeType {
			get { return NodeType.Unknown; }
		}

		public override SymbolKind SymbolKind {
			get { return SymbolKind.Method; }
		}

		public AccessorKind Kind { get; set; }

		/// <summary>
		/// Gets the 'get'/'set'/'init'/'add'/'remove' keyword
		/// </summary>
		public CSharpTokenNode Keyword {
			get {
				var role = GetAccessorKeywordRole(Kind);
				return role == null ? CSharpTokenNode.Null : GetChildByRole(role);
			}
		}

		public BlockStatement Body {
			get { return GetChildByRole(Roles.Body); }
			set { SetChildByRole(Roles.Body, value); }
		}

		public static TokenRole? GetAccessorKeywordRole(AccessorKind kind)
		{
			return kind switch {
				AccessorKind.Getter => PropertyDeclaration.GetKeywordRole,
				AccessorKind.Setter => PropertyDeclaration.SetKeywordRole,
				AccessorKind.Init => PropertyDeclaration.InitKeywordRole,
				AccessorKind.Adder => CustomEventDeclaration.AddKeywordRole,
				AccessorKind.Remover => CustomEventDeclaration.RemoveKeywordRole,
				_ => null,
			};
		}
	}
}
