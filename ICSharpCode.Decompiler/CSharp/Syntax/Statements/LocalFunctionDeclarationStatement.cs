using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public class LocalFunctionDeclarationStatement : Statement
	{
		public AstNodeCollection<TypeParameterDeclaration> TypeParameters {
			get { return GetChildrenByRole(Roles.TypeParameter); }
		}

		public CSharpTokenNode LParToken {
			get { return GetChildByRole(Roles.LPar); }
		}

		public AstNodeCollection<ParameterDeclaration> Parameters {
			get { return GetChildrenByRole(Roles.Parameter); }
		}

		public CSharpTokenNode RParToken {
			get { return GetChildByRole(Roles.RPar); }
		}

		public AstNodeCollection<Constraint> Constraints {
			get { return GetChildrenByRole(Roles.Constraint); }
		}

		public BlockStatement Body {
			get { return GetChildByRole(Roles.Body); }
			set { SetChildByRole(Roles.Body, value); }
		}

		public Modifiers Modifiers {
			get { return EntityDeclaration.GetModifiers(this); }
			set { EntityDeclaration.SetModifiers(this, value); }
		}

		public bool HasModifier(Modifiers mod)
		{
			return (Modifiers & mod) == mod;
		}

		public IEnumerable<CSharpModifierToken> ModifierTokens {
			get { return GetChildrenByRole(EntityDeclaration.ModifierRole); }
		}

		public virtual string Name {
			get {
				return GetChildByRole(Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, Identifier.Create(value, TextLocation.Empty));
			}
		}

		public virtual Identifier NameToken {
			get { return GetChildByRole(Roles.Identifier); }
			set { SetChildByRole(Roles.Identifier, value); }
		}

		public virtual AstType ReturnType {
			get { return GetChildByRole(Roles.Type); }
			set { SetChildByRole(Roles.Type, value); }
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
			LocalFunctionDeclarationStatement o = other as LocalFunctionDeclarationStatement;
			return o != null && MatchString(this.Name, o.Name)
				&& (this.Modifiers == Modifiers.Any || this.Modifiers == o.Modifiers)
				&& this.ReturnType.DoMatch(o.ReturnType, match)
				&& this.TypeParameters.DoMatch(o.TypeParameters, match)
				&& this.Parameters.DoMatch(o.Parameters, match) && this.Constraints.DoMatch(o.Constraints, match)
				&& this.Body.DoMatch(o.Body, match);
		}
	}
}
