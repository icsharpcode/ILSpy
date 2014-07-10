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
	class StatementBuilder(ICompilation compilation) : ILVisitor<Statement>
	{
		readonly ExpressionBuilder exprBuilder = new ExpressionBuilder(compilation);

		public Statement Convert(ILInstruction inst)
		{
			return inst.AcceptVisitor(this);
		}

		protected override Statement Default(ILInstruction inst)
		{
			return new ExpressionStatement(exprBuilder.Convert(inst));
		}

		protected internal override Statement VisitConditionalBranch(ConditionalBranch inst)
		{
			var condition = exprBuilder.ConvertCondition(inst.Condition);
			return new IfElseStatement(condition, new GotoStatement(inst.TargetLabel));
		}

		protected internal override Statement VisitBranch(Branch inst)
		{
			return new GotoStatement(inst.TargetLabel);
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
