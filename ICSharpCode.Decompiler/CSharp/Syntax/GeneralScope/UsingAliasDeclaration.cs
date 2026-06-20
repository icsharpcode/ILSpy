// 
// UsingAliasDeclaration.cs
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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>using_alias_directive ::= 'using' identifier '=' type ';'</c> (C# grammar §14.6.2)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class UsingAliasDeclaration : AstNode
	{
		public const string UsingKeyword = "using";

		[Slot("Alias")]
		public partial string Alias { get; set; }

		[Slot("Import")]
		public partial AstType Import { get; set; }

		public UsingAliasDeclaration(string alias, string nameSpace)
		{
			AddChild(Identifier.Create(alias), Slots.Alias);
			AddChild(new SimpleType(nameSpace), Slots.Import);
		}
	}
}
