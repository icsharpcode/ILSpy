using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Syntax;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	class FlattenSwitchBlocks : IAstTransform
	{
		public void Run(AstNode rootNode, TransformContext context)
		{
			foreach (var switchSection in rootNode.Descendants.OfType<SwitchSection>())
			{
				if (switchSection.Statements.Count != 1)
					continue;

				var blockStatement = switchSection.Statements.First() as BlockStatement;
				if (blockStatement == null || blockStatement.Statements.Any(ContainsLocalDeclaration))
					continue;

				blockStatement.Remove();
				blockStatement.Statements.MoveTo(switchSection.Statements);
			}

			bool ContainsLocalDeclaration(AstNode node)
			{
				if (node is VariableDeclarationStatement || node is LocalFunctionDeclarationStatement || node is OutVarDeclarationExpression)
					return true;
				if (node is BlockStatement)
					return false;
				foreach (var child in node.Children)
				{
					if (ContainsLocalDeclaration(child))
						return true;
				}
				return false;
			}
		}
	}
}
