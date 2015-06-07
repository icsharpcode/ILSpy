// Copyright (c) 2014 Daniel Grunwald
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
using System;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Escape invalid identifiers.
	/// </summary>
	/// <remarks>
	/// This transform is not enabled by default.
	/// </remarks>
	public 	class EscapeInvalidIdentifiers : IAstTransform
	{
		bool IsValid(char ch)
		{
			if (char.IsLetterOrDigit(ch))
				return true;
			if (ch == '_')
				return true;
			return false;
		}
		
		string ReplaceInvalid(string s)
		{
			return string.Concat(s.Select(ch => IsValid(ch) ? ch.ToString() : string.Format("_{0:X4}", (int)ch)));
		}
		
		public void Run(AstNode rootNode, TransformContext context)
		{
			foreach (var ident in rootNode.DescendantsAndSelf.OfType<Identifier>()) {
				ident.Name = ReplaceInvalid(ident.Name);
			}
		}
	}
}
