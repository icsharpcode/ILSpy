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
		/// 	if (comp.o(isinst T(ldloc testedOperand) == ldnull)) br falseBlock
		/// 	br unboxBlock
		/// }
		/// 
		/// Block unboxBlock (incoming: 1) {
		/// 	stloc V(unbox.any T(ldloc testedOperand))
		/// 	if (nextCondition) br trueBlock
		/// 	br falseBlock
		/// }
		/// =>
		/// Block {
		///		...
		///		if (logic.and(match.type[T].notnull(V = testedOperand), nextCondition)) br trueBlock
		///		br falseBlock
		///	}
		/// 
		/// -or-
		/// Block {
		///		...
		/// 	if (comp.o(isinst T(ldloc testedOperand) == ldnull)) br falseBlock
		/// 	br unboxBlock
		/// }
		/// 
		/// Block unboxBlock (incoming: 1) {
		/// 	stloc V(unbox.any T(ldloc testedOperand))
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
					if (!MatchUnboxBlock(unboxBlock, type, testedOperand.Variable, falseBlock,
						out var v, out var nextCondition, out var trueBlock, out var inverseNextCondition))
					{
						continue;
					}
					context.Step($"PatternMatching with {v.Name}", block);
					if (inverseNextCondition)
					{
						nextCondition = Comp.LogicNot(nextCondition);
					}
					var ifInst = (IfInstruction)block.Instructions.SecondToLastOrDefault()!;
					ILInstruction logicAnd = IfInstruction.LogicAnd(new MatchInstruction(v, testedOperand) {
						CheckNotNull = true,
						CheckType = true
					}, nextCondition);
					ifInst.Condition = logicAnd;
					((Branch)ifInst.TrueInst).TargetBlock = trueBlock;
					((Branch)block.Instructions.Last()).TargetBlock = falseBlock;
					unboxBlock.Instructions.Clear();
					v.Kind = VariableKind.PatternLocal;
				}
				container.Blocks.RemoveAll(b => b.Instructions.Count == 0);
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
		/// 	if (nextCondition) br trueBlock
		/// 	br falseBlock
		/// }
		private bool MatchUnboxBlock(Block unboxBlock, IType type, ILVariable testedOperand, Block falseBlock,
			[NotNullWhen(true)] out ILVariable? v,
			[NotNullWhen(true)] out ILInstruction? nextCondition,
			[NotNullWhen(true)] out Block? trueBlock,
			out bool inverseCondition)
		{
			v = null;
			nextCondition = null;
			trueBlock = null;
			inverseCondition = false;
			if (unboxBlock.IncomingEdgeCount != 1 || unboxBlock.Instructions.Count != 3)
				return false;

			if (!unboxBlock.Instructions[0].MatchStLoc(out v, out var value))
				return false;
			if (!(value.MatchUnboxAny(out var arg, out var t) && t.Equals(type) && arg.MatchLdLoc(testedOperand)))
				return false;

			if (!unboxBlock.MatchIfAtEndOfBlock(out nextCondition, out var trueInst, out var falseInst))
				return false;

			if (trueInst.MatchBranch(out trueBlock) && falseInst.MatchBranch(falseBlock))
			{
				return true;
			}
			else if (trueInst.MatchBranch(falseBlock) && falseInst.MatchBranch(out trueBlock))
			{
				inverseCondition = true;
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
