// Copyright (c) 2017 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// out type expression
	/// </summary>
	public class OutVarDeclarationExpression : Expression
	{
		public readonly static TokenRole OutKeywordRole = DirectionExpression.OutKeywordRole;

		public CSharpTokenNode OutKeywordToken {
			get { return GetChildByRole(OutKeywordRole); }
		}

		public AstType Type {
			get { return GetChildByRole(Roles.Type); }
			set { SetChildByRole(Roles.Type, value); }
		}

		public VariableInitializer Variable {
			get { return GetChildByRole(Roles.Variable); }
			set { SetChildByRole(Roles.Variable, value); }
		}

		public OutVarDeclarationExpression()
		{
		}

		public OutVarDeclarationExpression(AstType type, string name)
		{
			this.Type = type;
			this.Variable = new VariableInitializer(name);
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitOutVarDeclarationExpression(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitOutVarDeclarationExpression(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitOutVarDeclarationExpression(this, data);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			var o = other as OutVarDeclarationExpression;
			return o != null && this.Type.DoMatch(o.Type, match) && this.Variable.DoMatch(o.Variable, match);
		}
	}
}
