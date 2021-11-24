using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	class NormalizeBlockStatements : DepthFirstAstVisitor, IAstTransform
	{
		TransformContext context;
		bool hasNamespace;
		NamespaceDeclaration singleNamespaceDeclaration;

		public override void VisitSyntaxTree(SyntaxTree syntaxTree)
		{
			singleNamespaceDeclaration = null;
			hasNamespace = false;
			base.VisitSyntaxTree(syntaxTree);
			if (context.Settings.FileScopedNamespaces && singleNamespaceDeclaration != null)
			{
				singleNamespaceDeclaration.IsFileScoped = true;
			}
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			singleNamespaceDeclaration = null;
			if (!hasNamespace)
			{
				hasNamespace = true;
				singleNamespaceDeclaration = namespaceDeclaration;
			}
			base.VisitNamespaceDeclaration(namespaceDeclaration);

		}

		public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			base.VisitIfElseStatement(ifElseStatement);
			DoTransform(ifElseStatement.TrueStatement, ifElseStatement);
			DoTransform(ifElseStatement.FalseStatement, ifElseStatement);
		}

		public override void VisitWhileStatement(WhileStatement whileStatement)
		{
			base.VisitWhileStatement(whileStatement);
			InsertBlock(whileStatement.EmbeddedStatement);
		}

		public override void VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		{
			base.VisitDoWhileStatement(doWhileStatement);
			InsertBlock(doWhileStatement.EmbeddedStatement);
		}

		public override void VisitForeachStatement(ForeachStatement foreachStatement)
		{
			base.VisitForeachStatement(foreachStatement);
			InsertBlock(foreachStatement.EmbeddedStatement);
		}

		public override void VisitForStatement(ForStatement forStatement)
		{
			base.VisitForStatement(forStatement);
			InsertBlock(forStatement.EmbeddedStatement);
		}

		public override void VisitFixedStatement(FixedStatement fixedStatement)
		{
			base.VisitFixedStatement(fixedStatement);
			InsertBlock(fixedStatement.EmbeddedStatement);
		}

		public override void VisitLockStatement(LockStatement lockStatement)
		{
			base.VisitLockStatement(lockStatement);
			InsertBlock(lockStatement.EmbeddedStatement);
		}

		public override void VisitUsingStatement(UsingStatement usingStatement)
		{
			base.VisitUsingStatement(usingStatement);
			DoTransform(usingStatement.EmbeddedStatement, usingStatement);
		}

		void DoTransform(Statement statement, Statement parent)
		{
			if (statement.IsNull)
				return;
			if (context.Settings.AlwaysUseBraces)
			{
				if (!IsElseIf(statement, parent))
				{
					InsertBlock(statement);
				}
			}
			else
			{
				if (statement is BlockStatement b && b.Statements.Count == 1 && IsAllowedAsEmbeddedStatement(b.Statements.First(), parent))
				{
					statement.ReplaceWith(b.Statements.First().Detach());
				}
				else if (!IsAllowedAsEmbeddedStatement(statement, parent))
				{
					InsertBlock(statement);
				}
			}
		}

		bool IsElseIf(Statement statement, Statement parent)
		{
			return parent is IfElseStatement && statement.Role == IfElseStatement.FalseRole;
		}

		static void InsertBlock(Statement statement)
		{
			if (statement.IsNull)
				return;
			if (!(statement is BlockStatement))
			{
				var b = new BlockStatement();
				statement.ReplaceWith(b);
				if (statement is EmptyStatement && !statement.Children.Any())
				{
					b.CopyAnnotationsFrom(statement);
				}
				else
				{
					b.Add(statement);
				}
			}
		}

		bool IsAllowedAsEmbeddedStatement(Statement statement, Statement parent)
		{
			switch (statement)
			{
				case IfElseStatement ies:
					return parent is IfElseStatement && ies.Role == IfElseStatement.FalseRole;
				case VariableDeclarationStatement vds:
				case WhileStatement ws:
				case DoWhileStatement dws:
				case SwitchStatement ss:
				case ForeachStatement fes:
				case ForStatement fs:
				case LockStatement ls:
				case FixedStatement fxs:
					return false;
				case UsingStatement us:
					return parent is UsingStatement;
				default:
					return !(parent?.Parent is IfElseStatement);
			}
		}

		void IAstTransform.Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			rootNode.AcceptVisitor(this);
		}

		public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			if (context.Settings.UseExpressionBodyForCalculatedGetterOnlyProperties)
			{
				SimplifyPropertyDeclaration(propertyDeclaration);
			}
			base.VisitPropertyDeclaration(propertyDeclaration);
		}

		public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
		{
			if (context.Settings.UseExpressionBodyForCalculatedGetterOnlyProperties)
			{
				SimplifyIndexerDeclaration(indexerDeclaration);
			}
			base.VisitIndexerDeclaration(indexerDeclaration);
		}

		static readonly PropertyDeclaration CalculatedGetterOnlyPropertyPattern = new PropertyDeclaration() {
			Attributes = { new Repeat(new AnyNode()) },
			Modifiers = Modifiers.Any,
			Name = Pattern.AnyString,
			PrivateImplementationType = new AnyNodeOrNull(),
			ReturnType = new AnyNode(),
			Getter = new Accessor() {
				Modifiers = Modifiers.Any,
				Body = new BlockStatement() { new ReturnStatement(new AnyNode("expression")) }
			}
		};

		static readonly IndexerDeclaration CalculatedGetterOnlyIndexerPattern = new IndexerDeclaration() {
			Attributes = { new Repeat(new AnyNode()) },
			Modifiers = Modifiers.Any,
			PrivateImplementationType = new AnyNodeOrNull(),
			Parameters = { new Repeat(new AnyNode()) },
			ReturnType = new AnyNode(),
			Getter = new Accessor() {
				Modifiers = Modifiers.Any,
				Body = new BlockStatement() { new ReturnStatement(new AnyNode("expression")) }
			}
		};

		/// <summary>
		/// Modifiers that are emitted on accessors, but can be moved to the property declaration.
		/// </summary>
		const Modifiers movableModifiers = Modifiers.Readonly;

		void SimplifyPropertyDeclaration(PropertyDeclaration propertyDeclaration)
		{
			var m = CalculatedGetterOnlyPropertyPattern.Match(propertyDeclaration);
			if (!m.Success)
				return;
			if ((propertyDeclaration.Getter.Modifiers & ~movableModifiers) != 0)
				return;
			propertyDeclaration.Modifiers |= propertyDeclaration.Getter.Modifiers;
			propertyDeclaration.ExpressionBody = m.Get<Expression>("expression").Single().Detach();
			propertyDeclaration.Getter.Remove();
		}

		void SimplifyIndexerDeclaration(IndexerDeclaration indexerDeclaration)
		{
			var m = CalculatedGetterOnlyIndexerPattern.Match(indexerDeclaration);
			if (!m.Success)
				return;
			if ((indexerDeclaration.Getter.Modifiers & ~movableModifiers) != 0)
				return;
			indexerDeclaration.Modifiers |= indexerDeclaration.Getter.Modifiers;
			indexerDeclaration.ExpressionBody = m.Get<Expression>("expression").Single().Detach();
			indexerDeclaration.Getter.Remove();
		}
	}
}
