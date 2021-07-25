// Copyright (c) 2021 Daniel Grunwald, Siegfried Pammer
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

using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Block IL_0018 (incoming: *) {
	///     stloc s(ldc.i4 1)
	///     br IL_0019
	/// }
	/// 
	/// Block IL_0019 (incoming: > 1) {
	///     if (logic.not(ldloc s)) br IL_0027
	///     br IL_001d
	/// }
	/// 
	/// replace br IL_0019 with br IL_0027
	/// </summary>
	class RemoveInfeasiblePathTransform : IILTransform
	{
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			foreach (var container in function.Descendants.OfType<BlockContainer>())
			{
				bool changed = false;
				foreach (var block in container.Blocks)
				{
					changed |= DoTransform(block, context);
				}

				if (changed)
				{
					container.SortBlocks(deleteUnreachableBlocks: true);
				}
			}
		}


		private bool DoTransform(Block block, ILTransformContext context)
		{
			if (!MatchBlock1(block, out var s, out int value, out var br))
				return false;
			if (!MatchBlock2(br.TargetBlock, s, value, out var exitInst))
				return false;
			context.Step("RemoveInfeasiblePath", br);
			br.ReplaceWith(exitInst.Clone());
			s.RemoveIfRedundant = true;
			return true;
		}

		// Block IL_0018 (incoming: *) {
		//  stloc s(ldc.i4 1)
		//  br IL_0019
		// }
		private bool MatchBlock1(Block block, [NotNullWhen(true)] out ILVariable? variable, out int constantValue, [NotNullWhen(true)] out Branch? branch)
		{
			variable = null;
			constantValue = 0;
			branch = null;
			if (block.Instructions.Count != 2)
				return false;
			if (block.Instructions[0] is not StLoc
				{
					Variable: { Kind: VariableKind.StackSlot } s,
					Value: LdcI4 { Value: 0 or 1 } valueInst
				})
			{
				return false;
			}
			if (block.Instructions[1] is not Branch br)
				return false;
			variable = s;
			constantValue = valueInst.Value;
			branch = br;
			return true;
		}

		// Block IL_0019 (incoming: > 1) {
		//     if (logic.not(ldloc s)) br IL_0027
		//     br IL_001d
		// }
		bool MatchBlock2(Block block, ILVariable s, int constantValue, [NotNullWhen(true)] out ILInstruction? exitInst)
		{
			exitInst = null;
			if (block.Instructions.Count != 2)
				return false;
			if (block.IncomingEdgeCount <= 1)
				return false;
			if (!block.MatchIfAtEndOfBlock(out var load, out var trueInst, out var falseInst))
				return false;
			if (!load.MatchLdLoc(s))
				return false;
			exitInst = constantValue != 0 ? trueInst : falseInst;
			return exitInst is Branch or Leave { Value: Nop };
		}
	}
}
