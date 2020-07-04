// Copyright (c) 2020 Siegfried Pammer
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
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.DebugInfo;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public abstract class VariableDesignation : AstNode
	{
		public static Role<VariableDesignation> VariableDesignationRole = new Role<VariableDesignation>("VariableDesignation");
	}

	/// <summary>
	/// Identifier
	/// </summary>
	public class SingleVariableDesignation : VariableDesignation
	{
		public override NodeType NodeType => NodeType.Unknown;

		public string Identifier {
			get { return GetChildByRole(Roles.Identifier).Name; }
			set { SetChildByRole(Roles.Identifier, Syntax.Identifier.Create(value)); }
		}

		public Identifier IdentifierToken {
			get { return GetChildByRole(Roles.Identifier); }
			set { SetChildByRole(Roles.Identifier, value); }
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitSingleVariableDesignation(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitSingleVariableDesignation(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitSingleVariableDesignation(this, data);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			return other is SingleVariableDesignation o && MatchString(this.Identifier, o.Identifier);
		}
	}

	/// <summary>
	/// ( VariableDesignation (, VariableDesignation)* )
	/// </summary>
	public class ParenthesizedVariableDesignation : VariableDesignation
	{

		public CSharpTokenNode LParToken {
			get { return GetChildByRole(Roles.LPar); }
		}

		public AstNodeCollection<VariableDesignation> VariableDesignations {
			get { return GetChildrenByRole(VariableDesignation.VariableDesignationRole); }
		}

		public CSharpTokenNode RParToken {
			get { return GetChildByRole(Roles.RPar); }
		}

		public override NodeType NodeType => NodeType.Unknown;

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitParenthesizedVariableDesignation(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitParenthesizedVariableDesignation(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitParenthesizedVariableDesignation(this, data);
		}

		protected internal override bool DoMatch(AstNode other, Match match)
		{
			return other is ParenthesizedVariableDesignation o && VariableDesignations.DoMatch(o.VariableDesignations, match);
		}
	}
}
