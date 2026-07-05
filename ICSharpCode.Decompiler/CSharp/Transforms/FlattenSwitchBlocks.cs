// Copyright (c) 2011 Artur Zgodziński
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

#nullable enable

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

				context.Step("Flatten switch section block", blockStatement);
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
