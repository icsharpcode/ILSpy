// Copyright (c) 2017 Siegfried Pammer
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class NullCoalescingTransform : IBlockTransform
	{
		BlockTransformContext context;

		void IBlockTransform.Run(Block block, BlockTransformContext context)
		{
			this.context = context;
			for (int i = block.Instructions.Count - 1; i >= 0; i--) {
				if (TransformNullCoalescing(block, i)) {
					block.Instructions.RemoveAt(i);
					continue;
				}
			}
		}

		/// <summary>
		/// stloc s(valueInst)
		/// if (comp(ldloc s == ldnull)) {
		///		stloc s(fallbackInst)
		/// }
		/// =>
		/// stloc s(if.notnull(valueInst, fallbackInst)
		/// </summary>
		bool TransformNullCoalescing(Block block, int i)
		{
			if (i == 0) return false;
			if (!(block.Instructions[i] is IfInstruction ifInstruction) || !(block.Instructions[i - 1] is StLoc stloc) || stloc.Variable.Kind != VariableKind.StackSlot)
				return false;
			if (!ifInstruction.Condition.MatchCompEquals(out var left, out var right) || !left.MatchLdLoc(stloc.Variable) || !right.MatchLdNull())
				return false;
			if (!ifInstruction.FalseInst.MatchNop() || !(ifInstruction.TrueInst is Block b) || b.Instructions.Count != 1 || !(b.Instructions[0] is StLoc fallbackStore) || fallbackStore.Variable != stloc.Variable)
				return false;
			context.Step("TransformNullCoalescing", stloc);
			stloc.Value.ReplaceWith(new NullCoalescingInstruction(stloc.Value.Clone(), fallbackStore.Value.Clone()));
			return true;
		}
	}
}
