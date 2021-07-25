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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class PatternMatchingTransform : IILTransform
	{
		/// Block {
		///		...
		///		[stloc temp(ldloc testedOperand)]
		/// 	if (comp.o(isinst T(ldloc testedOperand) == ldnull)) br falseBlock
		/// 	br unboxBlock
		/// }
		/// 
		/// Block unboxBlock (incoming: 1) {
		/// 	stloc V(unbox.any T(ldloc temp))
		/// 	...
		/// }
		/// =>
		/// Block {
		///		...
		///		if (match.type[T].notnull(V = testedOperand)) br unboxBlock
		///		br falseBlock
		///	}
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.PatternMatching)
				return;
			foreach (var container in function.Descendants.OfType<BlockContainer>())
			{
				foreach (var block in container.Blocks)
				{
					if (!MatchIsInstBlock(block, out var type, out var testedOperand,
						out var unboxBlock, out var falseBlock))
					{
						continue;
					}
					StLoc? tempStore = block.Instructions.ElementAtOrDefault(block.Instructions.Count - 3) as StLoc;
					if (tempStore == null || !tempStore.Value.MatchLdLoc(testedOperand.Variable))
					{
						tempStore = null;
					}
					if (!MatchUnboxBlock(unboxBlock, type, out var unboxOperand, out var v))
					{
						continue;
					}
					if (unboxOperand == testedOperand.Variable)
					{
						// do nothing
					}
					else if (unboxOperand == tempStore?.Variable)
					{
						if (!(tempStore.Variable.IsSingleDefinition && tempStore.Variable.LoadCount == 1))
							continue;
					}
					else
					{
						continue;
					}
					context.Step($"PatternMatching with {v.Name}", block);
					var ifInst = (IfInstruction)block.Instructions.SecondToLastOrDefault()!;
					ifInst.Condition = new MatchInstruction(v, testedOperand) {
						CheckNotNull = true,
						CheckType = true
					};
					((Branch)ifInst.TrueInst).TargetBlock = unboxBlock;
					((Branch)block.Instructions.Last()).TargetBlock = falseBlock;
					unboxBlock.Instructions.RemoveAt(0);
					if (unboxOperand == tempStore?.Variable)
					{
						block.Instructions.Remove(tempStore);
					}
					// HACK: condition detection uses StartILOffset of blocks to decide which branch of if-else
					// should become the then-branch. Change the unboxBlock StartILOffset from an offset inside
					// the pattern matching machinery to an offset belonging to an instruction in the then-block.
					unboxBlock.SetILRange(unboxBlock.Instructions[0]);
					v.Kind = VariableKind.PatternLocal;
				}
			}
		}

		///	...
		/// if (comp.o(isinst T(ldloc testedOperand) == ldnull)) br falseBlock
		/// br unboxBlock
		private bool MatchIsInstBlock(Block block,
			[NotNullWhen(true)] out IType? type,
			[NotNullWhen(true)] out LdLoc? testedOperand,
			[NotNullWhen(true)] out Block? unboxBlock,
			[NotNullWhen(true)] out Block? falseBlock)
		{
			type = null;
			testedOperand = null;
			unboxBlock = null;
			falseBlock = null;
			if (!block.MatchIfAtEndOfBlock(out var condition, out var trueInst, out var falseInst))
				return false;
			if (condition.MatchCompEqualsNull(out var arg))
			{
				ExtensionMethods.Swap(ref trueInst, ref falseInst);
			}
			else if (condition.MatchCompNotEqualsNull(out arg))
			{
				// do nothing
			}
			else
			{
				return false;
			}
			if (!arg.MatchIsInst(out arg, out type))
				return false;
			testedOperand = arg as LdLoc;
			if (testedOperand == null)
				return false;
			return trueInst.MatchBranch(out unboxBlock) && falseInst.MatchBranch(out falseBlock)
				&& unboxBlock.Parent == block.Parent && falseBlock.Parent == block.Parent;
		}

		/// Block unboxBlock (incoming: 1) {
		/// 	stloc V(unbox.any T(ldloc testedOperand))
		/// 	...
		/// }
		private bool MatchUnboxBlock(Block unboxBlock, IType type, [NotNullWhen(true)] out ILVariable? testedOperand,
			[NotNullWhen(true)] out ILVariable? v)
		{
			v = null;
			testedOperand = null;
			if (unboxBlock.IncomingEdgeCount != 1)
				return false;

			if (!unboxBlock.Instructions[0].MatchStLoc(out v, out var value))
				return false;
			if (!(value.MatchUnboxAny(out var arg, out var t) && t.Equals(type) && arg.MatchLdLoc(out testedOperand)))
				return false;

			return true;
		}
	}
}
