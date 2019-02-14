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
			var block = container.EntryPoint;
			if (!(block.Instructions.SecondToLastOrDefault() is IfInstruction ifInst && block.Instructions.LastOrDefault() is Leave leaveElse))
				return;
			if (!ifInst.FalseInst.MatchNop())
				return;
			if (!(Block.Unwrap(ifInst.TrueInst) is Leave leave))
				return;
			if (!(leave.IsLeavingFunction && leaveElse.IsLeavingFunction))
				return;
			if (leave.Value.MatchNop() || leaveElse.Value.MatchNop())
				return;
			// if (cond) {
			//   leave (value)
			// }
			// leave (elseValue)
			// =>
			// leave (if (cond) value else elseValue)
			IfInstruction value = new IfInstruction(ifInst.Condition, leave.Value, leaveElse.Value) { ILRange = ifInst.ILRange };
			Leave combinedLeave = new Leave(leave.TargetContainer, value) { ILRange = leaveElse.ILRange };
			combinedLeave.AddILRange(leave.ILRange);
			ifInst.ReplaceWith(combinedLeave);
			block.Instructions.RemoveAt(combinedLeave.ChildIndex + 1);
		}
	}
}
