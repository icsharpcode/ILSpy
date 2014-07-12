// Copyright (c) 2014 Daniel Grunwald
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

using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.CSharp
{
	class StatementBuilder : ILVisitor<Statement>
	{
		readonly ExpressionBuilder exprBuilder;
		
		public StatementBuilder(ICompilation compilation)
		{
			this.exprBuilder = new ExpressionBuilder(compilation);
		}
		
		public Statement Convert(ILInstruction inst)
		{
			return inst.AcceptVisitor(this);
		}

		protected override Statement Default(ILInstruction inst)
		{
			return new ExpressionStatement(exprBuilder.Convert(inst));
		}

		protected internal override Statement VisitIfInstruction(IfInstruction inst)
		{
			var condition = exprBuilder.ConvertCondition(inst.Condition);
			var trueStatement = Convert(inst.TrueInst);
			var falseStatement = inst.FalseInst.OpCode == OpCode.Nop ? null : Convert(inst.FalseInst);
			return new IfElseStatement(condition, trueStatement, falseStatement);
		}

		protected internal override Statement VisitBranch(Branch inst)
		{
			return new GotoStatement(inst.TargetLabel);
		}
		
		protected internal override Statement VisitThrow(Throw inst)
		{
			return new ThrowStatement(exprBuilder.Convert(inst.Argument));
		}
		
		protected internal override Statement VisitReturn(Return inst)
		{
			if (inst.Argument == null)
				return new ReturnStatement();
			return new ReturnStatement(exprBuilder.Convert(inst.Argument));
		}

		protected internal override Statement VisitBlockContainer(BlockContainer inst)
		{
			return ConvertBlockContainer(inst);
		}

		public BlockStatement ConvertBlockContainer(BlockContainer container)
		{
			BlockStatement blockStatement = new BlockStatement();
			foreach (var block in container.Blocks) {
				blockStatement.Add(new LabelStatement { Label = block.Label });
				foreach (var inst in block.Instructions) {
					blockStatement.Add(Convert(inst));
				}
			}
			return blockStatement;
		}
	}
}
