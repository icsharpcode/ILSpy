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

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.TypeSystem.ReflectionHelper;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class PatternMatchingTransform : IILTransform
	{

		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.PatternMatching)
				return;
			foreach (var container in function.Descendants.OfType<BlockContainer>())
			{
				ControlFlowGraph? cfg = null;
				foreach (var block in container.Blocks)
				{
					if (PatternMatchValueTypes(block, container, context, ref cfg))
					{
						continue;
					}
					if (PatternMatchRefTypes(block, container, context, ref cfg))
					{
						continue;
					}
				}
			}
		}

		/// Block {
		///		...
		///		stloc V(isinst T(testedOperand))
		///		if (comp.o(ldloc V == ldnull)) br falseBlock
		///		br trueBlock
		/// }
		/// 
		/// All other uses of V are in blocks dominated by trueBlock.
		/// =>
		/// Block {
		///		...
		///		if (match.type[T].notnull(V = testedOperand)) br trueBlock
		///		br falseBlock
		/// }
		///
		/// - or -
		/// 
		/// Block {
		/// 	stloc s(isinst T(testedOperand))
		/// 	stloc v(ldloc s)
		/// 	if (logic.not(comp.o(ldloc s != ldnull))) br falseBlock
		/// 	br trueBlock
		/// }
		/// =>
		/// Block {
		///		...
		///		if (match.type[T].notnull(V = testedOperand)) br trueBlock
		///		br falseBlock
		/// }
		/// 
		/// All other uses of V are in blocks dominated by trueBlock.
		private bool PatternMatchRefTypes(Block block, BlockContainer container, ILTransformContext context, ref ControlFlowGraph? cfg)
		{
			if (!block.MatchIfAtEndOfBlock(out var condition, out var trueInst, out var falseInst))
				return false;
			int pos = block.Instructions.Count - 3;
			if (condition.MatchLdLoc(out var conditionVar))
			{
				// stloc conditionVar(comp.o(ldloc s == ldnull))
				// if (logic.not(ldloc conditionVar)) br trueBlock
				if (pos < 0)
					return false;
				if (!(conditionVar.IsSingleDefinition && conditionVar.LoadCount == 1
					&& conditionVar.Kind == VariableKind.StackSlot))
				{
					return false;
				}
				if (!block.Instructions[pos].MatchStLoc(conditionVar, out condition))
					return false;
				pos--;
			}
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
			if (!arg.MatchLdLoc(out var s))
				return false;
			if (!s.IsSingleDefinition)
				return false;
			if (s.Kind is not (VariableKind.Local or VariableKind.StackSlot))
				return false;
			if (pos < 0)
				return false;
			// stloc V(isinst T(testedOperand))
			ILInstruction storeToV = block.Instructions[pos];
			if (!storeToV.MatchStLoc(out var v, out var value))
				return false;
			if (value.MatchLdLoc(s))
			{
				// stloc v(ldloc s)
				pos--;
				if (!block.Instructions[pos].MatchStLoc(s, out value))
					return false;
				if (!v.IsSingleDefinition)
					return false;
				if (v.Kind is not (VariableKind.Local or VariableKind.StackSlot))
					return false;
				if (s.LoadCount != 2)
					return false;
			}
			else
			{
				if (v != s)
					return false;
			}
			IType? unboxType;
			if (value is UnboxAny unboxAny)
			{
				// stloc S(unbox.any T(isinst T(testedOperand)))
				unboxType = unboxAny.Type;
				value = unboxAny.Argument;
			}
			else
			{
				unboxType = null;
			}
			if (value is not IsInst { Argument: var testedOperand, Type: var type })
				return false;
			if (type.IsReferenceType != true)
				return false;
			if (!(unboxType == null || type.Equals(unboxType)))
				return false;

			if (!v.Type.Equals(type))
				return false;
			if (!CheckAllUsesDominatedBy(v, container, trueInst, storeToV, context, ref cfg))
				return false;
			context.Step($"Type pattern matching {v.Name}", block);
			//	if (match.type[T].notnull(V = testedOperand)) br trueBlock

			var ifInst = (IfInstruction)block.Instructions.SecondToLastOrDefault()!;

			ifInst.Condition = new MatchInstruction(v, testedOperand) {
				CheckNotNull = true,
				CheckType = true
			}.WithILRange(ifInst.Condition);
			ifInst.TrueInst = trueInst;
			block.Instructions[block.Instructions.Count - 1] = falseInst;
			block.Instructions.RemoveRange(pos, ifInst.ChildIndex - pos);
			v.Kind = VariableKind.PatternLocal;
			return true;
		}

		private bool CheckAllUsesDominatedBy(ILVariable v, BlockContainer container, ILInstruction trueInst,
			ILInstruction storeToV, ILTransformContext context, ref ControlFlowGraph? cfg)
		{
			var targetBlock = trueInst as Block;
			if (targetBlock == null && !trueInst.MatchBranch(out targetBlock))
			{
				return false;
			}

			if (targetBlock.Parent != container)
				return false;
			cfg ??= new ControlFlowGraph(container, context.CancellationToken);
			var targetBlockNode = cfg.GetNode(targetBlock);
			Debug.Assert(v.StoreInstructions.Count == 1);
			var uses = v.LoadInstructions.Concat<ILInstruction>(v.AddressInstructions)
				.Concat(v.StoreInstructions.Cast<ILInstruction>());
			foreach (var use in uses)
			{
				if (use == storeToV)
					continue;
				Block? found = null;
				for (ILInstruction? current = use; current != null; current = current.Parent)
				{
					if (current.Parent == container)
					{
						found = (Block)current;
						break;
					}
				}
				if (found == null)
					return false;
				var node = cfg.GetNode(found);
				if (!targetBlockNode.Dominates(node))
					return false;
			}
			return true;
		}

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
		private bool PatternMatchValueTypes(Block block, BlockContainer container, ILTransformContext context, ref ControlFlowGraph? cfg)
		{
			if (!MatchIsInstBlock(block, out var type, out var testedOperand,
				out var unboxBlock, out var falseBlock))
			{
				return false;
			}
			StLoc? tempStore = block.Instructions.ElementAtOrDefault(block.Instructions.Count - 3) as StLoc;
			if (tempStore == null || !tempStore.Value.MatchLdLoc(testedOperand.Variable))
			{
				tempStore = null;
			}
			if (!MatchUnboxBlock(unboxBlock, type, out var unboxOperand, out var v, out var storeToV))
			{
				return false;
			}
			if (unboxOperand == testedOperand.Variable)
			{
				// do nothing
			}
			else if (unboxOperand == tempStore?.Variable)
			{
				if (!(tempStore.Variable.IsSingleDefinition && tempStore.Variable.LoadCount == 1))
					return false;
			}
			else
			{
				return false;
			}
			if (!CheckAllUsesDominatedBy(v, container, unboxBlock, storeToV, context, ref cfg))
				return false;
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
			return true;
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
			[NotNullWhen(true)] out ILVariable? v, [NotNullWhen(true)] out ILInstruction? storeToV)
		{
			v = null;
			storeToV = null;
			testedOperand = null;
			if (unboxBlock.IncomingEdgeCount != 1)
				return false;
			storeToV = unboxBlock.Instructions[0];
			if (!storeToV.MatchStLoc(out v, out var value))
				return false;
			if (!(value.MatchUnboxAny(out var arg, out var t) && t.Equals(type) && arg.MatchLdLoc(out testedOperand)))
				return false;

			return true;
		}
	}
}
