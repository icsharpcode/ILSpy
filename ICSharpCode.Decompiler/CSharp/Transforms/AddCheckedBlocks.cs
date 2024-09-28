﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Add checked/unchecked blocks.
	/// </summary>
	public class AddCheckedBlocks : IAstTransform
	{
		#region Annotation
		sealed class CheckedUncheckedAnnotation
		{
			/// <summary>
			/// true=checked, false=unchecked
			/// </summary>
			public bool IsChecked;

			/// <summary>
			/// Require an explicit unchecked block (can't rely on the project-level unchecked context)
			/// </summary>
			public bool IsExplicit;
		}

		public static readonly object CheckedAnnotation = new CheckedUncheckedAnnotation { IsChecked = true };
		public static readonly object UncheckedAnnotation = new CheckedUncheckedAnnotation { IsChecked = false };
		public static readonly object ExplicitUncheckedAnnotation = new CheckedUncheckedAnnotation { IsChecked = false, IsExplicit = true };
		#endregion

		/* 
			We treat placing checked/unchecked blocks as an optimization problem, with the following goals:
				1. Use minimum number of checked blocks+expressions
				2. Prefer checked expressions over checked blocks
				3. Make the scope of checked expressions as small as possible
				4. Open checked blocks as late as possible, and close checked blocks as late as possible
				(where goal 1 has the highest priority)
				
				Goal 4a (open checked blocks as late as possible) is necessary so that we don't move variable declarations
				  into checked blocks, as the variable might still be used after the checked block.
				  (this could cause DeclareVariables to omit the variable declaration, producing incorrect code)
				Goal 4b (close checked blocks as late as possible) makes the code look nicer in this case:
					checked {
						int c = a + b;
						int r = a + c;
						return r;
					}
				If the checked block was closed as early as possible, the variable r would have to be declared outside
				  (this would work, but look badly)
		 */

		#region struct Cost
		struct Cost
		{
			// highest possible cost so that the Blocks+Expressions addition doesn't overflow
			public static readonly Cost Infinite = new Cost(0x3fffffff, 0x3fffffff);

			public readonly int Blocks;
			public readonly int Expressions;

			public Cost(int blocks, int expressions)
			{
				this.Blocks = blocks;
				this.Expressions = expressions;
			}

			public static bool operator <(Cost a, Cost b)
			{
				return a.Blocks + a.Expressions < b.Blocks + b.Expressions
					|| a.Blocks + a.Expressions == b.Blocks + b.Expressions && a.Blocks < b.Blocks;
			}

			public static bool operator >(Cost a, Cost b)
			{
				return a.Blocks + a.Expressions > b.Blocks + b.Expressions
					|| a.Blocks + a.Expressions == b.Blocks + b.Expressions && a.Blocks > b.Blocks;
			}

			public static bool operator <=(Cost a, Cost b)
			{
				return a.Blocks + a.Expressions < b.Blocks + b.Expressions
					|| a.Blocks + a.Expressions == b.Blocks + b.Expressions && a.Blocks <= b.Blocks;
			}

			public static bool operator >=(Cost a, Cost b)
			{
				return a.Blocks + a.Expressions > b.Blocks + b.Expressions
					|| a.Blocks + a.Expressions == b.Blocks + b.Expressions && a.Blocks >= b.Blocks;
			}

			public static Cost operator +(Cost a, Cost b)
			{
				return new Cost(a.Blocks + b.Blocks, a.Expressions + b.Expressions);
			}

			public override string ToString()
			{
				return string.Format("[{0} + {1}]", Blocks, Expressions);
			}

			/// <summary>
			/// Gets the new cost if an expression with this cost is wrapped in a checked/unchecked expression.
			/// </summary>
			internal Cost WrapInCheckedExpr()
			{
				if (Expressions == 0)
				{
					return new Cost(Blocks, 1);
				}
				else
				{
					// hack: penalize multiple layers of nested expressions
					// This doesn't really fit into the original idea of minimizing the number of block+expressions;
					// but tends to produce better-looking results due to less nesting.
					return new Cost(Blocks, Expressions + 2);
				}
			}
		}
		#endregion

		#region class InsertedNode
		/// <summary>
		/// Holds the blocks and expressions that should be inserted
		/// </summary>
		abstract class InsertedNode
		{
			public static InsertedNode operator +(InsertedNode a, InsertedNode b)
			{
				if (a == null)
					return b;
				if (b == null)
					return a;
				return new InsertedNodeList(a, b);
			}

			public abstract void Insert();
		}

		class InsertedNodeList : InsertedNode
		{
			readonly InsertedNode child1, child2;

			public InsertedNodeList(AddCheckedBlocks.InsertedNode child1, AddCheckedBlocks.InsertedNode child2)
			{
				this.child1 = child1;
				this.child2 = child2;
			}

			public override void Insert()
			{
				child1.Insert();
				child2.Insert();
			}
		}

		class InsertedExpression : InsertedNode
		{
			readonly Expression expression;
			readonly bool isChecked;

			public InsertedExpression(Expression expression, bool isChecked)
			{
				this.expression = expression;
				this.isChecked = isChecked;
			}

			public override void Insert()
			{
				if (isChecked)
					expression.ReplaceWith(e => new CheckedExpression { Expression = e });
				else
					expression.ReplaceWith(e => new UncheckedExpression { Expression = e });
			}
		}

		class InsertedBlock : InsertedNode
		{
			readonly Statement firstStatement; // inclusive
			readonly Statement lastStatement; // exclusive
			readonly bool isChecked;

			public InsertedBlock(Statement firstStatement, Statement lastStatement, bool isChecked)
			{
				this.firstStatement = firstStatement;
				this.lastStatement = lastStatement;
				this.isChecked = isChecked;
			}

			public override void Insert()
			{
				BlockStatement newBlock = new BlockStatement();
				// Move all statements except for the first
				Statement next;
				for (Statement stmt = firstStatement.GetNextStatement(); stmt != lastStatement; stmt = next)
				{
					next = stmt.GetNextStatement();
					newBlock.Add(stmt.Detach());
				}
				// Replace the first statement with the new (un)checked block
				if (isChecked)
					firstStatement.ReplaceWith(new CheckedStatement { Body = newBlock });
				else
					firstStatement.ReplaceWith(new UncheckedStatement { Body = newBlock });
				// now also move the first node into the new block
				newBlock.Statements.InsertAfter(null, firstStatement);
			}
		}
		#endregion

		#region class Result
		/// <summary>
		/// Holds the result of an insertion operation.
		/// </summary>
		class Result
		{
			public Cost CostInCheckedContext;
			public InsertedNode NodesToInsertInCheckedContext;
			public Cost CostInUncheckedContext;
			public InsertedNode NodesToInsertInUncheckedContext;
		}
		#endregion

		public void Run(AstNode node, TransformContext context)
		{
			BlockStatement? block = node as BlockStatement;
			if (block == null)
			{
				for (AstNode? child = node.FirstChild; child != null; child = child.NextSibling)
				{
					Run(child, context);
				}
			}
			else
			{
				Result r = GetResultFromBlock(block);
				if (r.NodesToInsertInUncheckedContext != null)
					r.NodesToInsertInUncheckedContext.Insert();
			}
		}

		Result GetResultFromBlock(BlockStatement block)
		{
			// For a block, we are tracking 4 possibilities:
			// a) context is checked, no unchecked block open
			Cost costCheckedContext = new Cost(0, 0);
			InsertedNode? nodesCheckedContext = null;
			// b) context is checked, an unchecked block is open
			Cost costCheckedContextUncheckedBlockOpen = Cost.Infinite;
			InsertedNode? nodesCheckedContextUncheckedBlockOpen = null;
			Statement? uncheckedBlockStart = null;
			// c) context is unchecked, no checked block open
			Cost costUncheckedContext = new Cost(0, 0);
			InsertedNode? nodesUncheckedContext = null;
			// d) context is unchecked, a checked block is open
			Cost costUncheckedContextCheckedBlockOpen = Cost.Infinite;
			InsertedNode? nodesUncheckedContextCheckedBlockOpen = null;
			Statement? checkedBlockStart = null;

			Statement? statement = block.Statements.FirstOrDefault();
			while (true)
			{
				// Blocks can be closed 'for free'. We use '<=' so that blocks are closed as late as possible (goal 4b)
				if (costCheckedContextUncheckedBlockOpen <= costCheckedContext)
				{
					costCheckedContext = costCheckedContextUncheckedBlockOpen;
					nodesCheckedContext = nodesCheckedContextUncheckedBlockOpen + new InsertedBlock(uncheckedBlockStart, statement, false);
				}
				if (costUncheckedContextCheckedBlockOpen <= costUncheckedContext)
				{
					costUncheckedContext = costUncheckedContextCheckedBlockOpen;
					nodesUncheckedContext = nodesUncheckedContextCheckedBlockOpen + new InsertedBlock(checkedBlockStart, statement, true);
				}
				if (statement == null)
					break;
				// Now try opening blocks. We use '<=' so that blocks are opened as late as possible. (goal 4a)
				if (costCheckedContext + new Cost(1, 0) <= costCheckedContextUncheckedBlockOpen)
				{
					costCheckedContextUncheckedBlockOpen = costCheckedContext + new Cost(1, 0);
					nodesCheckedContextUncheckedBlockOpen = nodesCheckedContext;
					uncheckedBlockStart = statement;
				}
				if (costUncheckedContext + new Cost(1, 0) <= costUncheckedContextCheckedBlockOpen)
				{
					costUncheckedContextCheckedBlockOpen = costUncheckedContext + new Cost(1, 0);
					nodesUncheckedContextCheckedBlockOpen = nodesUncheckedContext;
					checkedBlockStart = statement;
				}
				// Now handle the statement
				Result stmtResult = GetResult(statement);

				costCheckedContext += stmtResult.CostInCheckedContext;
				nodesCheckedContext += stmtResult.NodesToInsertInCheckedContext;
				costCheckedContextUncheckedBlockOpen += stmtResult.CostInUncheckedContext;
				nodesCheckedContextUncheckedBlockOpen += stmtResult.NodesToInsertInUncheckedContext;
				costUncheckedContext += stmtResult.CostInUncheckedContext;
				nodesUncheckedContext += stmtResult.NodesToInsertInUncheckedContext;
				costUncheckedContextCheckedBlockOpen += stmtResult.CostInCheckedContext;
				nodesUncheckedContextCheckedBlockOpen += stmtResult.NodesToInsertInCheckedContext;

				if (statement is LabelStatement || statement is LocalFunctionDeclarationStatement)
				{
					// We can't move labels into blocks because that might cause goto-statements
					// to be unable to jump to the labels.
					// Also, we can't move local functions into blocks, because that might cause
					// them to become out of scope from the call-sites.
					costCheckedContextUncheckedBlockOpen = Cost.Infinite;
					costUncheckedContextCheckedBlockOpen = Cost.Infinite;
				}

				statement = statement.GetNextStatement();
			}

			return new Result {
				CostInCheckedContext = costCheckedContext, NodesToInsertInCheckedContext = nodesCheckedContext,
				CostInUncheckedContext = costUncheckedContext, NodesToInsertInUncheckedContext = nodesUncheckedContext
			};
		}

		Result GetResult(AstNode node)
		{
			if (node is BlockStatement)
				return GetResultFromBlock((BlockStatement)node);
			Result result = new Result();
			for (AstNode? child = node.FirstChild; child != null; child = child.NextSibling)
			{
				Result childResult = GetResult(child);
				result.CostInCheckedContext += childResult.CostInCheckedContext;
				result.NodesToInsertInCheckedContext += childResult.NodesToInsertInCheckedContext;
				result.CostInUncheckedContext += childResult.CostInUncheckedContext;
				result.NodesToInsertInUncheckedContext += childResult.NodesToInsertInUncheckedContext;
			}
			Expression? expr = node as Expression;
			if (expr != null)
			{
				CheckedUncheckedAnnotation annotation = expr.Annotation<CheckedUncheckedAnnotation>();
				if (annotation != null)
				{
					if (annotation.IsExplicit)
					{
						// We don't yet support distinguishing CostInUncheckedContext vs. CostInExplicitUncheckedContext,
						// so we always force an unchecked() expression here.
						if (annotation.IsChecked)
							throw new NotImplementedException("explicit checked"); // should not be needed
						result.CostInCheckedContext = result.CostInUncheckedContext.WrapInCheckedExpr();
						result.CostInUncheckedContext = result.CostInUncheckedContext.WrapInCheckedExpr();
						result.NodesToInsertInUncheckedContext += new InsertedExpression(expr, annotation.IsChecked);
						result.NodesToInsertInCheckedContext = result.NodesToInsertInUncheckedContext;
					}
					else
					{
						// If the annotation requires this node to be in a specific context, add a huge cost to the other context
						// That huge cost gives us the option to ignore a required checked/unchecked expression when there wouldn't be any
						// solution otherwise. (e.g. "for (checked(M().x += 1); true; unchecked(M().x += 2)) {}")
						if (annotation.IsChecked)
							result.CostInUncheckedContext += new Cost(10000, 0);
						else
							result.CostInCheckedContext += new Cost(10000, 0);
					}
				}
				// Embed this node in an checked/unchecked expression:
				if (expr.Parent is ExpressionStatement)
				{
					// We cannot use checked/unchecked for top-level-expressions.
				}
				else if (expr.Role.IsValid(Expression.Null))
				{
					// We use '<' so that expressions are introduced on the deepest level possible (goal 3)
					var costIfWrapWithChecked = result.CostInCheckedContext.WrapInCheckedExpr();
					var costIfWrapWithUnchecked = result.CostInUncheckedContext.WrapInCheckedExpr();
					if (costIfWrapWithChecked < result.CostInUncheckedContext)
					{
						result.CostInUncheckedContext = costIfWrapWithChecked;
						result.NodesToInsertInUncheckedContext = result.NodesToInsertInCheckedContext + new InsertedExpression(expr, true);
					}
					else if (costIfWrapWithUnchecked < result.CostInCheckedContext)
					{
						result.CostInCheckedContext = costIfWrapWithUnchecked;
						result.NodesToInsertInCheckedContext = result.NodesToInsertInUncheckedContext + new InsertedExpression(expr, false);
					}
				}
			}
			return result;
		}
	}
}
