// 
// UsingDeclaration.cs
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

using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <code>
	/// using_directive ::=
	///       'using' type ';'
	///     | 'using' 'static' type ';'
	/// </code>
	/// (C# grammar §14.6.3, §14.6.4)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class UsingDeclaration : AstNode
	{
		public const string UsingKeyword = "using";

		[Slot("Import")]
		public partial AstType Import { get; set; }

		// Computed from Import (which is matched); exclude to avoid a redundant compare.
		[ExcludeFromMatch]
		public string Namespace {
			get { return ConstructNamespace(Import); }
		}

		internal static string ConstructNamespace(AstType type)
		{
			var stack = new Stack<string>();
			while (type is MemberType)
			{
				var mt = (MemberType)type;
				stack.Push(mt.MemberName);
				type = mt.Target;
				if (mt.IsDoubleColon)
				{
					stack.Push("::");
				}
				else
				{
					stack.Push(".");
				}
			}
			if (type is SimpleType simpleType)
				stack.Push(simpleType.Identifier ?? string.Empty);

			var result = new StringBuilder();
			while (stack.Count > 0)
				result.Append(stack.Pop());
			return result.ToString();
		}

		public UsingDeclaration(string nameSpace)
		{
			AddChild(AstType.Create(nameSpace), Slots.Import);
		}
	}
}
