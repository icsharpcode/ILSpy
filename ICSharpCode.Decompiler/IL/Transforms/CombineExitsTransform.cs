// Copyright (c) 2019 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class CombineExitsTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!(function.Body is BlockContainer container && container.Blocks.Count == 1))
				return;
			var combinedExit = CombineExits(container.EntryPoint);
			if (combinedExit == null)
				return;
			ExpressionTransforms.RunOnSingleStatement(combinedExit, context);
		}

		static Leave CombineExits(Block block)
		{
			if (!(block.Instructions.SecondToLastOrDefault() is IfInstruction ifInst && block.Instructions.LastOrDefault() is Leave leaveElse))
				return null;
			if (!ifInst.FalseInst.MatchNop())
				return null;
			// try to unwrap true branch to single instruction:
			var trueInstruction = Block.Unwrap(ifInst.TrueInst);
			// if the true branch is a block with multiple instructions:
			// try to apply the combine exits transform to the nested block
			// and then continue on that transformed block.
			// Example:
			// if (cond) {
			//   if (cond2) {
			//     leave (value)
			//   }
			//   leave (value2)
			// }
			// leave (value3)
			// =>
			// leave (if (cond) value else if (cond2) value2 else value3)
			if (trueInstruction is Block nestedBlock && nestedBlock.Instructions.Count == 2)
				trueInstruction = CombineExits(nestedBlock);
			if (!(trueInstruction is Leave leave))
				return null;
			if (!(leave.IsLeavingFunction && leaveElse.IsLeavingFunction))
				return null;
			if (leave.Value.MatchNop() || leaveElse.Value.MatchNop())
				return null;
			// if (cond) {
			//   leave (value)
			// }
			// leave (elseValue)
			// =>
			// leave (if (cond) value else elseValue)
			IfInstruction value = new IfInstruction(ifInst.Condition, leave.Value, leaveElse.Value);
			value.AddILRange(ifInst);
			Leave combinedLeave = new Leave(leave.TargetContainer, value);
			combinedLeave.AddILRange(leaveElse);
			combinedLeave.AddILRange(leave);
			ifInst.ReplaceWith(combinedLeave);
			block.Instructions.RemoveAt(combinedLeave.ChildIndex + 1);
			return combinedLeave;
		}
	}
}
