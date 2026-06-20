// 
// SyntaxTree.cs
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

using System.Collections.Generic;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>compilation_unit ::= extern_alias_directive* using_directive* global_attributes? compilation_unit_body</c> (C# grammar §14.2)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class SyntaxTree : AstNode
	{
		[Slot("Member")]
		public partial AstNodeCollection<AstNode> Members { get; }

		/// <summary>
		/// Gets all defined types in this syntax tree.
		/// </summary>
		/// <returns>
		/// A list containing <see cref="TypeDeclaration"/> or <see cref="DelegateDeclaration"/> nodes.
		/// </returns>
		public IEnumerable<EntityDeclaration> GetTypes(bool includeInnerTypes = false)
		{
			Stack<AstNode> nodeStack = new Stack<AstNode>();
			nodeStack.Push(this);
			while (nodeStack.Count > 0)
			{
				var curNode = nodeStack.Pop();
				if (curNode is TypeDeclaration || curNode is DelegateDeclaration)
				{
					yield return (EntityDeclaration)curNode;
				}
				foreach (var child in curNode.Children)
				{
					if (!(child is Statement || child is Expression) &&
						(child.Slot?.Kind != Slots.TypeMember || ((child is TypeDeclaration || child is DelegateDeclaration) && includeInnerTypes)))
						nodeStack.Push(child);
				}
			}
		}
	}
}
