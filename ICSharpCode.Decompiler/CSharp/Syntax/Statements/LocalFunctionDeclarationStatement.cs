// Copyright (c) 2019 Siegfried Pammer
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

using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public class LocalFunctionDeclarationStatement : Statement
	{
		public static readonly Role<MethodDeclaration> MethodDeclarationRole = new Role<MethodDeclaration>("Method", null);

		public MethodDeclaration Declaration {
			get { return GetChildByRole(MethodDeclarationRole); }
			set { SetChildByRole(MethodDeclarationRole, value); }
		}

		public LocalFunctionDeclarationStatement(MethodDeclaration methodDeclaration)
		{
			AddChild(methodDeclaration, MethodDeclarationRole);
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitLocalFunctionDeclarationStatement(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitLocalFunctionDeclarationStatement(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitLocalFunctionDeclarationStatement(this, data);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			return other is LocalFunctionDeclarationStatement o && Declaration.DoMatch(o.Declaration, match);
		}
	}
}
