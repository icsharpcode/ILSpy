// 
// Tokens.cs
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

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public static class Tokens
	{
		// some pre defined constants for most used punctuation
		public const string LPar = "(";
		public const string RPar = ")";
		public const string LBracket = "[";
		public const string RBracket = "]";
		public const string LBrace = "{";
		public const string RBrace = "}";
		public const string LChevron = "<";
		public const string RChevron = ">";
		public const string Comma = ",";
		public const string Dot = ".";
		public const string Semicolon = ";";
		public const string Assign = "=";
		public const string Colon = ":";
		public const string DoubleColon = "::";
		public const string Arrow = "=>";

		public const string WhereKeyword = "where";
		public const string DelegateKeyword = "delegate";
		public const string ExternKeyword = "extern";
		public const string AliasKeyword = "alias";
		public const string NamespaceKeyword = "namespace";

		public const string EnumKeyword = "enum";
		public const string InterfaceKeyword = "interface";
		public const string StructKeyword = "struct";
		public const string ClassKeyword = "class";
		public const string RecordKeyword = "record";
		public const string RecordStructKeyword = "record";
	}
}
