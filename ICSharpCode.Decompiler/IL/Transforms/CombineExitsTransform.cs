using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class CombineExitsTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!(function.Body is BlockContainer container && container.Blocks.Count == 1))
				return;
			var block = container.EntryPoint;
			if (block.Kind != BlockKind.ControlFlow)
				return;
			if (!(block.Instructions.SecondToLastOrDefault() is IfInstruction ifInst && block.Instructions.LastOrDefault() is Leave leave))
				return;
			if (!ifInst.FalseInst.MatchNop())
				return;
			if (!(Block.Unwrap(ifInst.TrueInst) is Leave leave2))
				return;
			if (!(leave.IsLeavingFunction && leave2.IsLeavingFunction))
				return;
			if (leave.Value.MatchNop() || leave2.Value.MatchNop())
				return;
			IfInstruction value = new IfInstruction(ifInst.Condition, leave.Value, leave2.Value);
			Leave combinedLeave = new Leave(leave.TargetContainer, value);
			ifInst.ReplaceWith(combinedLeave);
			block.Instructions.RemoveAt(combinedLeave.ChildIndex + 1);
		}
	}
}
