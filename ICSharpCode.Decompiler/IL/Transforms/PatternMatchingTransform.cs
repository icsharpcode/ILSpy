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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

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
				foreach (var block in container.Blocks.Reverse())
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
				container.Blocks.RemoveAll(b => b.Instructions.Count == 0);
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
			if (condition.MatchCompEqualsNull(out var loadInNullCheck))
			{
				ExtensionMethods.Swap(ref trueInst, ref falseInst);
			}
			else if (condition.MatchCompNotEqualsNull(out loadInNullCheck))
			{
				// do nothing
			}
			else
			{
				return false;
			}
			if (!loadInNullCheck.MatchLdLoc(out var s))
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
				if (pos < 0 || !block.Instructions[pos].MatchStLoc(s, out value))
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
			if (!CheckAllUsesDominatedBy(v, container, trueInst, storeToV, loadInNullCheck, context, ref cfg))
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

			DetectPropertySubPatterns((MatchInstruction)ifInst.Condition, trueInst, falseInst, container, context, ref cfg);

			return true;
		}

		private static ILInstruction DetectPropertySubPatterns(MatchInstruction parentPattern, ILInstruction trueInst,
			ILInstruction parentFalseInst, BlockContainer container, ILTransformContext context, ref ControlFlowGraph? cfg)
		{
			if (!context.Settings.RecursivePatternMatching)
			{
				return trueInst;
			}
			while (true)
			{
				Block? trueBlock = trueInst as Block;
				if (!(trueBlock != null || trueInst.MatchBranch(out trueBlock)))
				{
					break;
				}
				if (!(trueBlock.IncomingEdgeCount == 1 && trueBlock.Parent == container))
				{
					break;
				}
				var nextTrueInst = DetectPropertySubPattern(parentPattern, trueBlock, parentFalseInst, context, ref cfg);
				if (nextTrueInst != null)
				{
					trueInst = nextTrueInst;
				}
				else
				{
					break;
				}
			}
			return trueInst;
		}

		private static ILInstruction? DetectPropertySubPattern(MatchInstruction parentPattern, Block block,
			ILInstruction parentFalseInst, ILTransformContext context, ref ControlFlowGraph? cfg)
		{
			// if (match.notnull.type[System.String] (V_0 = callvirt get_C(ldloc V_2))) br IL_0022
			// br IL_0037
			if (MatchBlockContainingOneCondition(block, out var condition, out var trueInst, out var falseInst))
			{
				bool negate = false;
				if (!DetectExitPoints.CompatibleExitInstruction(parentFalseInst, falseInst))
				{
					if (!DetectExitPoints.CompatibleExitInstruction(parentFalseInst, trueInst))
					{
						return null;
					}
					ExtensionMethods.Swap(ref trueInst, ref falseInst);
					negate = true;
				}
				if (MatchInstruction.IsPatternMatch(condition, out var operand, context.Settings))
				{
					if (!PropertyOrFieldAccess(operand, out var target, out _))
					{
						return null;
					}
					if (!target.MatchLdLocRef(parentPattern.Variable))
					{
						return null;
					}
					if (negate && !context.Settings.PatternCombinators)
					{
						return null;
					}
					context.Step("Move property sub pattern", condition);
					if (negate)
					{
						condition = Comp.LogicNot(condition);
					}
					parentPattern.SubPatterns.Add(condition);
				}
				else if (PropertyOrFieldAccess(condition, out var target, out _))
				{
					if (!target.MatchLdLocRef(parentPattern.Variable))
					{
						return null;
					}
					if (!negate && !context.Settings.PatternCombinators)
					{
						return null;
					}
					context.Step("Sub pattern: implicit != 0", condition);
					parentPattern.SubPatterns.Add(new Comp(negate ? ComparisonKind.Equality : ComparisonKind.Inequality,
						Sign.None, condition, new LdcI4(0)));
				}
				else
				{
					return null;
				}
				block.Instructions.Clear();
				block.Instructions.Add(trueInst);
				return trueInst;
			}
			else if (block.Instructions[0].MatchStLoc(out var targetVariable, out var operand))
			{
				if (!PropertyOrFieldAccess(operand, out var target, out var member))
				{
					return null;
				}
				if (!target.MatchLdLocRef(parentPattern.Variable))
				{
					return null;
				}
				if (!targetVariable.Type.Equals(member.ReturnType))
				{
					return null;
				}
				if (!CheckAllUsesDominatedBy(targetVariable, (BlockContainer)block.Parent!, block, block.Instructions[0], null, context, ref cfg))
				{
					return null;
				}
				context.Step("Property var pattern", block);
				var varPattern = new MatchInstruction(targetVariable, operand)
					.WithILRange(block.Instructions[0]);
				parentPattern.SubPatterns.Add(varPattern);
				block.Instructions.RemoveAt(0);
				targetVariable.Kind = VariableKind.PatternLocal;

				if (targetVariable.Type.IsKnownType(KnownTypeCode.NullableOfT))
				{
					return MatchNullableHasValueCheckPattern(block, varPattern, parentFalseInst, context, ref cfg)
						?? block;
				}

				var instructionAfterNullCheck = MatchNullCheckPattern(block, varPattern, parentFalseInst, context);
				if (instructionAfterNullCheck != null)
				{
					return DetectPropertySubPatterns(varPattern, instructionAfterNullCheck, parentFalseInst, (BlockContainer)block.Parent!, context, ref cfg);
				}
				else if (targetVariable.Type.IsReferenceType == false)
				{
					return DetectPropertySubPatterns(varPattern, block, parentFalseInst, (BlockContainer)block.Parent!, context, ref cfg);
				}
				else
				{
					return block;
				}
			}
			else
			{
				return null;
			}
		}

		private static ILInstruction? MatchNullCheckPattern(Block block, MatchInstruction varPattern,
			ILInstruction parentFalseInst, ILTransformContext context)
		{
			if (!MatchBlockContainingOneCondition(block, out var condition, out var trueInst, out var falseInst))
			{
				return null;
			}
			if (condition.MatchCompEqualsNull(out var arg) && arg.MatchLdLoc(varPattern.Variable))
			{
				ExtensionMethods.Swap(ref trueInst, ref falseInst);
			}
			else if (condition.MatchCompNotEqualsNull(out arg) && arg.MatchLdLoc(varPattern.Variable))
			{
			}
			else
			{
				return null;
			}
			if (!DetectExitPoints.CompatibleExitInstruction(falseInst, parentFalseInst))
			{
				return null;
			}
			context.Step("Null check pattern", block);
			varPattern.CheckNotNull = true;
			block.Instructions.Clear();
			block.Instructions.Add(trueInst);
			return trueInst;
		}

		private static ILInstruction? MatchNullableHasValueCheckPattern(Block block, MatchInstruction varPattern,
			ILInstruction parentFalseInst, ILTransformContext context, ref ControlFlowGraph? cfg)
		{
			if (!(varPattern.Variable.StoreCount == 1 && varPattern.Variable.LoadCount == 0))
			{
				return null;
			}
			if (!MatchBlockContainingOneCondition(block, out var condition, out var trueInst, out var falseInst))
			{
				return null;
			}
			if (!NullableLiftingTransform.MatchHasValueCall(condition, varPattern.Variable))
			{
				return null;
			}
			if (!DetectExitPoints.CompatibleExitInstruction(falseInst, parentFalseInst))
			{
				if (DetectExitPoints.CompatibleExitInstruction(trueInst, parentFalseInst))
				{
					if (!(varPattern.Variable.AddressCount == 1))
					{
						return null;
					}

					context.Step("Nullable.HasValue check -> null pattern", block);
					varPattern.ReplaceWith(new Comp(ComparisonKind.Equality, ComparisonLiftingKind.CSharp, StackType.O, Sign.None, varPattern.TestedOperand, new LdNull()));
					block.Instructions.Clear();
					block.Instructions.Add(falseInst);
					return falseInst;
				}
				return null;
			}
			if (varPattern.Variable.AddressCount == 1 && context.Settings.PatternCombinators)
			{
				context.Step("Nullable.HasValue check -> not null pattern", block);
				varPattern.ReplaceWith(new Comp(ComparisonKind.Inequality, ComparisonLiftingKind.CSharp, StackType.O, Sign.None, varPattern.TestedOperand, new LdNull()));
				block.Instructions.Clear();
				block.Instructions.Add(trueInst);
				return trueInst;
			}
			else if (varPattern.Variable.AddressCount != 2)
			{
				return null;
			}
			if (!(trueInst.MatchBranch(out var trueBlock) && trueBlock.Parent == block.Parent && trueBlock.IncomingEdgeCount == 1))
			{
				return null;
			}
			if (trueBlock.Instructions[0].MatchStLoc(out var newTargetVariable, out var getValueOrDefaultCall)
				&& NullableLiftingTransform.MatchGetValueOrDefault(getValueOrDefaultCall, varPattern.Variable))
			{
				context.Step("Nullable.HasValue check + Nullable.GetValueOrDefault pattern", block);
				varPattern.CheckNotNull = true;
				varPattern.Variable = newTargetVariable;
				newTargetVariable.Kind = VariableKind.PatternLocal;
				block.Instructions.Clear();
				block.Instructions.Add(trueInst);
				trueBlock.Instructions.RemoveAt(0);
				return DetectPropertySubPatterns(varPattern, trueBlock, parentFalseInst, (BlockContainer)block.Parent!, context, ref cfg);
			}
			else if (MatchBlockContainingOneCondition(trueBlock, out condition, out trueInst, out falseInst))
			{
				if (!(condition is Comp comp
					&& MatchInstruction.IsConstant(comp.Right)
					&& NullableLiftingTransform.MatchGetValueOrDefault(comp.Left, varPattern.Variable)))
				{
					return null;
				}
				if (!(context.Settings.RelationalPatterns || comp.Kind is ComparisonKind.Equality or ComparisonKind.Inequality))
				{
					return null;
				}
				bool negated = false;
				if (!DetectExitPoints.CompatibleExitInstruction(falseInst, parentFalseInst))
				{
					if (!DetectExitPoints.CompatibleExitInstruction(trueInst, parentFalseInst))
					{
						return null;
					}
					ExtensionMethods.Swap(ref trueInst, ref falseInst);
					negated = true;
				}
				if (comp.Kind == (negated ? ComparisonKind.Equality : ComparisonKind.Inequality))
				{
					return null;
				}
				if (negated && !context.Settings.PatternCombinators)
				{
					return null;
				}
				context.Step("Nullable.HasValue check + Nullable.GetValueOrDefault pattern", block);
				// varPattern: match (v = testedOperand)
				// comp: comp.i4(call GetValueOrDefault(ldloca v) != ldc.i4 42)
				// =>
				// comp.i4.lifted(testedOperand != ldc.i4 42)
				block.Instructions.Clear();
				block.Instructions.Add(trueInst);
				trueBlock.Instructions.Clear();
				comp.Left = varPattern.TestedOperand;
				comp.LiftingKind = ComparisonLiftingKind.CSharp;
				if (negated)
				{
					comp = Comp.LogicNot(comp);
				}
				varPattern.ReplaceWith(comp);
				return trueInst;
			}
			else
			{
				return null;
			}
		}

		private static bool PropertyOrFieldAccess(ILInstruction operand, [NotNullWhen(true)] out ILInstruction? target, [NotNullWhen(true)] out IMember? member)
		{
			if (operand is CallInstruction {
				Method: {
					SymbolKind: SymbolKind.Accessor,
					AccessorKind: MethodSemanticsAttributes.Getter,
					AccessorOwner: { } _member
				},
				Arguments: [var _target]
			})
			{
				target = _target;
				member = _member;
				return true;
			}
			else if (operand.MatchLdFld(out target, out var field))
			{
				member = field;
				return true;
			}
			else
			{
				member = null;
				return false;
			}
		}

		private static bool MatchBlockContainingOneCondition(Block block, [NotNullWhen(true)] out ILInstruction? condition, [NotNullWhen(true)] out ILInstruction? trueInst, [NotNullWhen(true)] out ILInstruction? falseInst)
		{
			switch (block.Instructions.Count)
			{
				case 2:
					return block.MatchIfAtEndOfBlock(out condition, out trueInst, out falseInst);
				case 3:
					condition = null;
					if (!block.MatchIfAtEndOfBlock(out var loadTemp, out trueInst, out falseInst))
						return false;
					if (!(loadTemp.MatchLdLoc(out var tempVar) && tempVar.IsSingleDefinition && tempVar.LoadCount == 1))
						return false;
					if (!block.Instructions[0].MatchStLoc(tempVar, out condition))
						return false;
					while (condition.MatchLogicNot(out var arg))
					{
						condition = arg;
						ExtensionMethods.Swap(ref trueInst, ref falseInst);
					}
					return true;
				default:
					condition = null;
					trueInst = null;
					falseInst = null;
					return false;
			}
		}

		private static bool CheckAllUsesDominatedBy(ILVariable v, BlockContainer container, ILInstruction trueInst,
			ILInstruction storeToV, ILInstruction? loadInNullCheck, ILTransformContext context, ref ControlFlowGraph? cfg)
		{
			var targetBlock = trueInst as Block;
			if (targetBlock == null && !trueInst.MatchBranch(out targetBlock))
			{
				return false;
			}

			if (targetBlock.Parent != container)
				return false;
			if (targetBlock.IncomingEdgeCount != 1)
				return false;
			cfg ??= new ControlFlowGraph(container, context.CancellationToken);
			var targetBlockNode = cfg.GetNode(targetBlock);
			var uses = v.LoadInstructions.Concat<ILInstruction>(v.AddressInstructions)
				.Concat(v.StoreInstructions.Cast<ILInstruction>());
			foreach (var use in uses)
			{
				if (use == storeToV || use == loadInNullCheck)
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
			if (!MatchIsInstBlock(block, out var type, out var testedOperand, out var testedVariable,
				out var boxType1, out var unboxBlock, out var falseInst))
			{
				return false;
			}
			StLoc? tempStore = block.Instructions.ElementAtOrDefault(block.Instructions.Count - 3) as StLoc;
			if (tempStore == null || !tempStore.Value.MatchLdLoc(testedVariable))
			{
				tempStore = null;
			}
			if (!MatchUnboxBlock(unboxBlock, type, out var unboxOperand, out var boxType2, out var storeToV))
			{
				return false;
			}
			if (!object.Equals(boxType1, boxType2))
			{
				return false;
			}
			if (unboxOperand == testedVariable)
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
			if (!CheckAllUsesDominatedBy(storeToV.Variable, container, unboxBlock, storeToV, null, context, ref cfg))
				return false;
			context.Step($"PatternMatching with {storeToV.Variable.Name}", block);
			var ifInst = (IfInstruction)block.Instructions.SecondToLastOrDefault()!;
			ifInst.Condition = new MatchInstruction(storeToV.Variable, testedOperand) {
				CheckNotNull = true,
				CheckType = true
			};
			ifInst.TrueInst = new Branch(unboxBlock);
			block.Instructions[^1] = falseInst;
			unboxBlock.Instructions.RemoveAt(0);
			if (unboxOperand == tempStore?.Variable)
			{
				block.Instructions.Remove(tempStore);
			}
			// HACK: condition detection uses StartILOffset of blocks to decide which branch of if-else
			// should become the then-branch. Change the unboxBlock StartILOffset from an offset inside
			// the pattern matching machinery to an offset belonging to an instruction in the then-block.
			unboxBlock.SetILRange(unboxBlock.Instructions[0]);
			storeToV.Variable.Kind = VariableKind.PatternLocal;
			DetectPropertySubPatterns((MatchInstruction)ifInst.Condition, unboxBlock, falseInst, container, context, ref cfg);
			return true;
		}

		///	...
		/// if (comp.o(isinst T(ldloc testedOperand) == ldnull)) br falseBlock
		/// br unboxBlock
		/// - or -
		/// ...
		/// if (comp.o(isinst T(box ``0(ldloc testedOperand)) == ldnull)) br falseBlock
		/// br unboxBlock
		private bool MatchIsInstBlock(Block block,
			[NotNullWhen(true)] out IType? type,
			[NotNullWhen(true)] out ILInstruction? testedOperand,
			[NotNullWhen(true)] out ILVariable? testedVariable,
			out IType? boxType,
			[NotNullWhen(true)] out Block? unboxBlock,
			[NotNullWhen(true)] out ILInstruction? falseInst)
		{
			type = null;
			testedOperand = null;
			testedVariable = null;
			boxType = null;
			unboxBlock = null;
			if (!block.MatchIfAtEndOfBlock(out var condition, out var trueInst, out falseInst))
			{
				return false;
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
			if (!arg.MatchIsInst(out testedOperand, out type))
			{
				return false;
			}
			if (!(testedOperand.MatchBox(out var boxArg, out boxType) && boxType.Kind == TypeKind.TypeParameter))
			{
				boxArg = testedOperand;
			}
			if (!boxArg.MatchLdLoc(out testedVariable))
			{
				return false;
			}
			return trueInst.MatchBranch(out unboxBlock) && unboxBlock.Parent == block.Parent;
		}

		/// Block unboxBlock (incoming: 1) {
		/// 	stloc V(unbox.any T(ldloc testedOperand))
		/// 	...
		/// 	- or -
		/// 	stloc V(unbox.any T(isinst T(box ``0(ldloc testedOperand))))
		/// 	...
		/// }
		private bool MatchUnboxBlock(Block unboxBlock, IType type, [NotNullWhen(true)] out ILVariable? testedVariable,
			out IType? boxType, [NotNullWhen(true)] out StLoc? storeToV)
		{
			boxType = null;
			storeToV = null;
			testedVariable = null;
			if (unboxBlock.IncomingEdgeCount != 1)
				return false;
			storeToV = unboxBlock.Instructions[0] as StLoc;
			if (storeToV == null)
				return false;
			var value = storeToV.Value;
			if (!(value.MatchUnboxAny(out var arg, out var t) && t.Equals(type)))
				return false;
			if (arg.MatchIsInst(out var isinstArg, out var isinstType) && isinstType.Equals(type))
			{
				arg = isinstArg;
			}
			if (arg.MatchBox(out var boxArg, out boxType) && boxType.Kind == TypeKind.TypeParameter)
			{
				arg = boxArg;
			}
			if (!arg.MatchLdLoc(out testedVariable))
			{
				return false;
			}
			if (boxType != null && !boxType.Equals(testedVariable.Type))
			{
				return false;
			}
			return true;
		}
	}
}
