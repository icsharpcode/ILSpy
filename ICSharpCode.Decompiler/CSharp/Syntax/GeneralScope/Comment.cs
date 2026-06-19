// 
// Comment.cs
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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public enum CommentType
	{
		/// <summary>
		/// "//" comment
		/// </summary>
		SingleLine,
		/// <summary>
		/// "/* */" comment
		/// </summary>
		MultiLine,
		/// <summary>
		/// "///" comment
		/// </summary>
		Documentation,
		/// <summary>
		/// Inactive code (code in non-taken "#if")
		/// </summary>
		InactiveCode,
		/// <summary>
		/// "/** */" comment
		/// </summary>
		MultiLineDocumentation
	}

	/// <summary>
	/// <c>comment ::= '//' input_character* | '/*' input_character* '*/'</c> (C# lexical grammar §6.3.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class Comment : Trivia
	{
		public CommentType CommentType { get; set; }

		public string Content { get; set; } = string.Empty;

		public Comment(string content, CommentType type = CommentType.SingleLine)
		{
			this.CommentType = type;
			this.Content = content;
		}

		public Comment(CommentType commentType, TextLocation startLocation, TextLocation endLocation) : base(startLocation, endLocation)
		{
			this.CommentType = commentType;
		}
	}
}
