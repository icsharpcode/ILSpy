using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// new Type[Dimensions]
	/// </summary>
	public class ArrayCreateExpression : Expression
	{
		public readonly static Role<ArraySpecifier> ArraySpecifierRole = new Role<ArraySpecifier>("ArraySpecifierRole", ArraySpecifier.Null);
		public readonly static Role<ArraySpecifier> AdditionalArraySpecifierRole = new Role<ArraySpecifier>("AdditionalArraySpecifier");
		public readonly static Role<ArrayInitializerExpression> InitializerRole = new Role<ArrayInitializerExpression>("Initializer", ArrayInitializerExpression.Null);
		
		public AstType Type {
			get { return GetChildByRole (Roles.Type); }
			set { SetChildByRole (Roles.Type, value); }
		}
		
		public ArraySpecifier ArraySpecifier
		{
			get { return GetChildByRole(ArraySpecifierRole); }
			set { SetChildByRole(ArraySpecifierRole, value); }
		}
		
		/// <summary>
		/// Gets additional array ranks (those without size info).
		/// Empty for "new int[5,1]"; will contain a single element for "new int[5][]".
		/// </summary>
		public AstNodeCollection<ArraySpecifier> AdditionalArraySpecifiers {
			get { return GetChildrenByRole(AdditionalArraySpecifierRole); }
		}
		
		public ArrayInitializerExpression Initializer {
			get { return GetChildByRole (InitializerRole); }
			set { SetChildByRole (InitializerRole, value); }
		}
		
		public override S AcceptVisitor<T, S> (IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitArrayCreateExpression (this, data);
		}
		
		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			ArrayCreateExpression o = other as ArrayCreateExpression;
			return o != null && this.Type.DoMatch(o.Type, match) && this.ArraySpecifier.DoMatch(o.ArraySpecifier, match) && this.AdditionalArraySpecifiers.DoMatch(o.AdditionalArraySpecifiers, match) && this.Initializer.DoMatch(o.Initializer, match);
		}
	}
}
