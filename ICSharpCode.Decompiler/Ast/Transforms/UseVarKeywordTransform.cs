using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.Decompiler.Ast.Transforms
{
	class UseVarKeywordTransform : DepthFirstAstVisitor, IAstTransform
	{
		private readonly DecompilerSettings settings;

		public UseVarKeywordTransform(DecompilerSettings settings)
		{
			this.settings = settings;
		}

		public override void VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			var type = variableDeclarationStatement.Type;
			if (variableDeclarationStatement.Variables.Count != 1 || type.IsVar()) return;
			var variable = variableDeclarationStatement.Variables.Single();
			if (variable.Initializer.IsNull) return; // has to have an initializer
			if (variable.Initializer is IdentifierExpression && ((IdentifierExpression)variable.Initializer).Identifier == "null" ||
				variable.Initializer is NullReferenceExpression ||
				variable.Initializer is AnonymousMethodExpression ||
				variable.Initializer is LambdaExpression) return; // and can't be null or anonymous method

			var useVar = false;
			switch (settings.UseVar) {
				case VarKeywordUsage.WhenTypeIsEvident:
					useVar = IsTypeEvident(type, variable.Initializer);
					break;
				case VarKeywordUsage.WhenTypeIsEvidentOrLong:
					useVar = IsLongName(type) || IsTypeEvident(type, variable.Initializer);
					break;
				case VarKeywordUsage.Always:
					useVar = true;
					break;
				default:
					break;
			}

			if (useVar) {
				variableDeclarationStatement.Type = new SimpleType("var").CopyAnnotationsFrom(type);
			}

			base.VisitVariableDeclarationStatement(variableDeclarationStatement);
		}

		bool IsLongName(AstType type) => type.StartLocation.Column + 15 < type.EndLocation.Column;

		bool IsTypeEvident(AstType type, Expression initializer)
		{
			if (initializer is IdentifierExpression) return true; // you know types of your locals
			if (initializer is InvocationExpression) { // SOMETYPE variable = SOMETYPE.method123(ahoj, ahoj);
				var target = ((InvocationExpression)initializer).Target;
				do {
					if (target is TypeReferenceExpression && ((INode)type).Match(((TypeReferenceExpression)target).Type).Success) {
						return true;
					} else if (target is MemberReferenceExpression && ((MemberReferenceExpression)target).TypeArguments.Any(a => ((INode)type).Match(a).Success)) {
						return true;
					}

					if (target is MemberReferenceExpression) {
						target = ((MemberReferenceExpression)target).Target;
					} else target = null;
				} while (target != null);
			}
			if (initializer is CastExpression) {
				if (((INode)type).Match(((CastExpression)initializer).Type).Success) return true;
			}
			if (initializer is AsExpression) {
				if (((INode)type).Match(((AsExpression)initializer).Type).Success) return true;
			}
			return false;
		}


		public void Run(AstNode compilationUnit)
		{
			if (settings.UseVar == VarKeywordUsage.Never) return;
			compilationUnit.AcceptVisitor(this);
		}
	}
}
