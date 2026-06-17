// 
// Roles.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public static class Roles
	{

		// some pre defined constants for common roles


		// some pre defined constants for most used punctuation
		public static readonly TokenRole LPar = new TokenRole("(");
		public static readonly TokenRole RPar = new TokenRole(")");
		public static readonly TokenRole LBracket = new TokenRole("[");
		public static readonly TokenRole RBracket = new TokenRole("]");
		public static readonly TokenRole LBrace = new TokenRole("{");
		public static readonly TokenRole RBrace = new TokenRole("}");
		public static readonly TokenRole LChevron = new TokenRole("<");
		public static readonly TokenRole RChevron = new TokenRole(">");
		public static readonly TokenRole Comma = new TokenRole(",");
		public static readonly TokenRole Dot = new TokenRole(".");
		public static readonly TokenRole Semicolon = new TokenRole(";");
		public static readonly TokenRole Assign = new TokenRole("=");
		public static readonly TokenRole Colon = new TokenRole(":");
		public static readonly TokenRole DoubleColon = new TokenRole("::");
		public static readonly TokenRole Arrow = new TokenRole("=>");



		public readonly static TokenRole WhereKeyword = new TokenRole("where");
		public readonly static TokenRole DelegateKeyword = new TokenRole("delegate");
		public static readonly TokenRole ExternKeyword = new TokenRole("extern");
		public static readonly TokenRole AliasKeyword = new TokenRole("alias");
		public static readonly TokenRole NamespaceKeyword = new TokenRole("namespace");

		public static readonly TokenRole EnumKeyword = new TokenRole("enum");
		public static readonly TokenRole InterfaceKeyword = new TokenRole("interface");
		public static readonly TokenRole StructKeyword = new TokenRole("struct");
		public static readonly TokenRole ClassKeyword = new TokenRole("class");
		public static readonly TokenRole RecordKeyword = new TokenRole("record");
		public static readonly TokenRole RecordStructKeyword = new TokenRole("record");

	}
}
