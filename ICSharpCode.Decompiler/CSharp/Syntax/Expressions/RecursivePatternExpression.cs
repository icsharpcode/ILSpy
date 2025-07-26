﻿// Copyright (c) 2023 Daniel Grunwald
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
	[DecompilerAstNode(hasNullNode: false)]
	public partial class RecursivePatternExpression : Expression
	{
		public static readonly Role<Expression> SubPatternRole = new Role<Expression>("SubPattern", Syntax.Expression.Null);

		public AstType Type {
			get { return GetChildByRole(Roles.Type); }
			set { SetChildByRole(Roles.Type, value); }
		}

		public AstNodeCollection<Expression> SubPatterns {
			get { return GetChildrenByRole(SubPatternRole); }
		}

		public VariableDesignation Designation {
			get { return GetChildByRole(Roles.VariableDesignationRole); }
			set { SetChildByRole(Roles.VariableDesignationRole, value); }
		}

		public bool IsPositional { get; set; }

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			return other is RecursivePatternExpression o
				&& Type.DoMatch(o.Type, match)
				&& IsPositional == o.IsPositional
				&& SubPatterns.DoMatch(o.SubPatterns, match)
				&& Designation.DoMatch(o.Designation, match);
		}
	}
}
