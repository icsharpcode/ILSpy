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
	class StatementBuilder(ICompilation compilation)
	{
		readonly ExpressionBuilder exprBuilder = new ExpressionBuilder(compilation);

		public Statement Convert(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.BlockContainer:
					return ConvertBlockContainer((BlockContainer)inst);
				case OpCode.Ret:
					if (inst is ReturnVoidInstruction)
						return new ReturnStatement().WithAnnotation(inst);
					return new ReturnStatement(ConvertUnaryArg(inst)).WithAnnotation(inst);
				case OpCode.Throw:
					return new ThrowStatement(ConvertUnaryArg(inst)).WithAnnotation(inst);
				case OpCode.ConditionalBranch:
					return ConvertConditionalBranch((ConditionalBranch)inst);
				default:
					return new ExpressionStatement(exprBuilder.Convert(inst));
			}
		}

		private Statement ConvertConditionalBranch(ConditionalBranch inst)
		{
			var condition = exprBuilder.ConvertCondition(inst.Condition);
			return new IfElseStatement(condition, new GotoStatement(inst.TargetLabel));
		}

		private Expression ConvertUnaryArg(ILInstruction inst)
		{
			return exprBuilder.Convert(((UnaryInstruction)inst).Operand);
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
