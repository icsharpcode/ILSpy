﻿// Copyright (c) 2017 Siegfried Pammer
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

using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	using HashtableInitializer = Dictionary<IField, (List<(string, int)> Labels, IfInstruction JumpToNext, Block ContainingBlock, Block Previous, Block Next, bool Transformed)>;

	/// <summary>
	/// Detects switch-on-string patterns employed by the C# compiler and transforms them to an ILAst-switch-instruction.
	/// </summary>
	public class SwitchOnStringTransform : IILTransform
	{
		ILTransformContext context;
		private readonly SwitchAnalysis analysis = new SwitchAnalysis();

		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.SwitchStatementOnString)
				return;

			this.context = context;
			BlockContainer body = (BlockContainer)function.Body;
			var hashtableInitializers = ScanHashtableInitializerBlocks(body.EntryPoint);

			HashSet<BlockContainer> changedContainers = new HashSet<BlockContainer>();

			foreach (var block in function.Descendants.OfType<Block>())
			{
				bool changed = false;
				if (block.IncomingEdgeCount == 0)
					continue;
				for (int i = block.Instructions.Count - 1; i >= 0; i--)
				{
					if (SimplifyCascadingIfStatements(block.Instructions, ref i))
					{
						changed = true;
						continue;
					}
					if (SimplifyCSharp1CascadingIfStatements(block.Instructions, ref i))
					{
						changed = true;
						continue;
					}
					if (MatchLegacySwitchOnStringWithHashtable(block, hashtableInitializers, ref i))
					{
						changed = true;
						continue;
					}
					if (MatchLegacySwitchOnStringWithDict(block.Instructions, ref i))
					{
						changed = true;
						continue;
					}
					if (MatchRoslynSwitchOnString(block.Instructions, ref i))
					{
						changed = true;
						continue;
					}
					if (MatchRoslynSwitchOnStringUsingLengthAndChar(block, i))
					{
						changed = true;
						continue;
					}
				}
				if (!changed)
					continue;
				SwitchDetection.SimplifySwitchInstruction(block, context);
				if (block.Parent is BlockContainer container)
					changedContainers.Add(container);
			}

			var omittedBlocks = new Dictionary<Block, Block>();

			// Remove all transformed hashtable initializers from the entrypoint.
			foreach (var item in hashtableInitializers)
			{
				var (labels, jumpToNext, containingBlock, previous, next, transformed) = item.Value;
				if (!transformed)
					continue;
				if (!omittedBlocks.TryGetValue(previous, out var actual))
					actual = previous;
				context.Step("Remove hashtable initializer", actual);
				if (jumpToNext != null)
				{
					actual.Instructions.SecondToLastOrDefault().ReplaceWith(jumpToNext);
				}
				actual.Instructions.LastOrDefault().ReplaceWith(new Branch(next));
				omittedBlocks.Add(containingBlock, previous);
				changedContainers.Add(body);
			}

			// If all initializer where removed, remove the initial null check as well.
			if (hashtableInitializers.Count > 0 && omittedBlocks.Count == hashtableInitializers.Count && body.EntryPoint.Instructions.Count == 2)
			{
				if (body.EntryPoint.Instructions[0] is IfInstruction ifInst
					&& ifInst.TrueInst.MatchBranch(out var beginOfMethod) && body.EntryPoint.Instructions[1].MatchBranch(beginOfMethod))
				{
					context.Step("Remove initial null check", body);
					body.EntryPoint.Instructions.RemoveAt(0);
				}
			}

			foreach (var container in changedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);
		}

		HashtableInitializer ScanHashtableInitializerBlocks(Block entryPoint)
		{
			var hashtables = new HashtableInitializer();
			if (entryPoint.Instructions.Count != 2)
				return hashtables;
			// match first block: checking compiler-generated Hashtable for null
			// if (comp(volatile.ldobj System.Collections.Hashtable(ldsflda $$method0x600003f-1) != ldnull)) br switchHeadBlock
			// br tableInitBlock
			if (!(entryPoint.Instructions[0].MatchIfInstruction(out var condition, out var branchToSwitchHead)))
				return hashtables;
			if (!entryPoint.Instructions[1].MatchBranch(out var tableInitBlock))
				return hashtables;
			if (!(condition.MatchCompNotEquals(out var left, out var right) && right.MatchLdNull() &&
				MatchDictionaryFieldLoad(left, IsNonGenericHashtable, out var dictField, out var dictionaryType)))
				return hashtables;
			if (!branchToSwitchHead.MatchBranch(out var switchHead))
				return hashtables;
			// match second block: initialization of compiler-generated Hashtable
			// stloc table(newobj Hashtable..ctor(ldc.i4 capacity, ldc.f loadFactor))
			// call Add(ldloc table, ldstr value, box System.Int32(ldc.i4 index))
			// ... more calls to Add ...
			// volatile.stobj System.Collections.Hashtable(ldsflda $$method0x600003f - 1, ldloc table)
			// br switchHeadBlock
			if (tableInitBlock.IncomingEdgeCount != 1 || tableInitBlock.Instructions.Count < 3)
				return hashtables;
			Block previousBlock = entryPoint;
			while (tableInitBlock != null)
			{
				if (!ExtractStringValuesFromInitBlock(tableInitBlock, out var stringValues, out var blockAfterThisInitBlock, dictionaryType, dictField, true))
					break;
				var nextHashtableInitHead = tableInitBlock.Instructions.SecondToLastOrDefault() as IfInstruction;
				hashtables.Add(dictField, (stringValues, nextHashtableInitHead, tableInitBlock, previousBlock, blockAfterThisInitBlock, false));
				previousBlock = tableInitBlock;
				// if there is another IfInstruction before the end of the block, it might be a jump to the next hashtable init block.
				// if (comp(volatile.ldobj System.Collections.Hashtable(ldsflda $$method0x600003f-2) != ldnull)) br switchHeadBlock
				if (nextHashtableInitHead != null)
				{
					if (!(nextHashtableInitHead.Condition.MatchCompNotEquals(out left, out right) && right.MatchLdNull() &&
						MatchDictionaryFieldLoad(left, IsNonGenericHashtable, out var nextDictField, out _)))
						break;
					if (!nextHashtableInitHead.TrueInst.MatchBranch(switchHead))
						break;
					tableInitBlock = blockAfterThisInitBlock;
					dictField = nextDictField;
				}
				else
				{
					break;
				}
			}
			return hashtables;
		}

		bool SimplifyCascadingIfStatements(InstructionCollection<ILInstruction> instructions, ref int i)
		{
			// match first block: checking switch-value for null or first value (Roslyn)
			// if (call op_Equality(ldloc switchValueVar, ldstr value)) br firstBlock
			// -or-
			// if (comp(ldloc switchValueVar == ldnull)) br defaultBlock
			if (!instructions[i].MatchIfInstruction(out var condition, out var firstBlockOrDefaultJump))
				return false;
			var nextCaseJump = instructions[i + 1];
			while (condition.MatchLogicNot(out var arg))
			{
				condition = arg;
				ExtensionMethods.Swap(ref firstBlockOrDefaultJump, ref nextCaseJump);
			}
			// match call to operator ==(string, string)
			if (!MatchStringEqualityComparison(condition, out var switchValueVar, out string firstBlockValue, out bool isVBCompareString))
				return false;
			if (isVBCompareString)
			{
				ExtensionMethods.Swap(ref firstBlockOrDefaultJump, ref nextCaseJump);
			}

			if (firstBlockOrDefaultJump.MatchBranch(out var firstBlock))
			{
				// success
			}
			else if (firstBlockOrDefaultJump.MatchLeave(out _))
			{
				firstBlock = null;
				// success
			}
			else
			{
				return false;
			}

			var values = new List<(string, ILInstruction)>();
			var uniqueValues = new HashSet<string>();
			int numberOfUniqueMatchesWithCurrentVariable = 0;
			HashSet<Block> caseBlocks = new HashSet<Block>();
			caseBlocks.Add((Block)instructions[i].Parent);

			bool AddSwitchSection(string value, ILInstruction inst)
			{
				if (!uniqueValues.Add(value))
					return false;
				numberOfUniqueMatchesWithCurrentVariable++;
				values.Add((value, inst));
				return true;
			}

			ILInstruction switchValue = null;
			if (isVBCompareString && string.IsNullOrEmpty(firstBlockValue))
			{
				if (!AddSwitchSection(null, firstBlock ?? firstBlockOrDefaultJump))
					return false;
				if (!AddSwitchSection(string.Empty, firstBlock ?? firstBlockOrDefaultJump))
					return false;
			}
			else
			{
				if (!AddSwitchSection(firstBlockValue, firstBlock ?? firstBlockOrDefaultJump))
					return false;
			}

			bool removeExtraLoad = false;
			bool keepAssignmentBefore = false;
			if (i >= 1 && instructions[i - 1].MatchStLoc(switchValueVar, out switchValue))
			{
				// stloc switchValueVar(switchValue)
				// if (call op_Equality(ldloc switchValueVar, ldstr value)) br firstBlock

				// Newer versions of Roslyn use extra variables:
				if (i >= 2 && switchValue.MatchLdLoc(out var otherSwitchValueVar) && otherSwitchValueVar.IsSingleDefinition && otherSwitchValueVar.LoadCount == 1
					&& instructions[i - 2].MatchStLoc(otherSwitchValueVar, out var newSwitchValue))
				{
					switchValue = newSwitchValue;
					removeExtraLoad = true;
				}
			}
			else if (i >= 1 && instructions[i - 1] is StLoc stloc)
			{
				if (stloc.Value.MatchLdLoc(switchValueVar))
				{
					// in case of optimized legacy code there are two stlocs:
					// stloc otherSwitchValueVar(ldloc switchValue)
					// stloc switchValueVar(ldloc otherSwitchValueVar)
					// if (call op_Equality(ldloc otherSwitchValueVar, ldstr value)) br firstBlock
					var otherSwitchValueVar = switchValueVar;
					switchValueVar = stloc.Variable;
					numberOfUniqueMatchesWithCurrentVariable = 0;
					if (i >= 2 && instructions[i - 2].MatchStLoc(otherSwitchValueVar, out switchValue)
						&& otherSwitchValueVar.IsSingleDefinition && otherSwitchValueVar.LoadCount == 2)
					{
						removeExtraLoad = true;
					}
					else
					{
						switchValue = new LdLoc(otherSwitchValueVar);
					}
				}
				else
				{
					// Variable before the start of the switch is not related to the switch.
					keepAssignmentBefore = true;
					switchValue = new LdLoc(switchValueVar);
				}
			}
			else
			{
				// Instruction before the start of the switch is not related to the switch.
				keepAssignmentBefore = true;
				switchValue = new LdLoc(switchValueVar);
			}
			if (!switchValueVar.Type.IsKnownType(KnownTypeCode.String))
			{
				if (!context.Settings.SwitchOnReadOnlySpanChar)
					return false;
				if (!switchValueVar.Type.IsKnownType(KnownTypeCode.ReadOnlySpanOfT)
					&& !switchValueVar.Type.IsKnownType(KnownTypeCode.SpanOfT))
				{
					return false;
				}
			}
			// if instruction must be followed by a branch to the next case
			if (!nextCaseJump.MatchBranch(out Block currentCaseBlock))
				return false;
			// extract all cases and add them to the values list.
			ILInstruction nextCaseBlock;
			do
			{
				nextCaseBlock = MatchCaseBlock(currentCaseBlock, switchValueVar, out string value, out bool emptyStringEqualsNull, out ILInstruction block);
				if (nextCaseBlock == null)
					break;
				if (emptyStringEqualsNull && string.IsNullOrEmpty(value))
				{
					if (!AddSwitchSection(null, block))
						return false;
					if (!AddSwitchSection(string.Empty, block))
						return false;
				}
				else
				{
					if (!AddSwitchSection(value, block))
						return false;
				}
				caseBlocks.Add(currentCaseBlock);
				currentCaseBlock = nextCaseBlock as Block;
			} while (currentCaseBlock != null);

			// We didn't find enough cases, exit
			if (values.Count < 3)
				return false;
			context.Step(nameof(SimplifyCascadingIfStatements), instructions[i]);
			// if the switchValueVar is used in other places as well, do not eliminate the store.
			if (switchValueVar.LoadCount > numberOfUniqueMatchesWithCurrentVariable || !ValidateUsesOfSwitchValueVariable(switchValueVar, caseBlocks))
			{
				keepAssignmentBefore = true;
				removeExtraLoad = false; // prevent loads from being deleted after detecting that
										 // we have to keep the assignment before the switch statement
				switchValue = new LdLoc(switchValueVar);
			}
			int offset = firstBlock == null ? 1 : 0;
			var sections = new List<SwitchSection>(values.Skip(offset).SelectWithIndex((index, s) => new SwitchSection { Labels = new LongSet(index), Body = s.Item2 is Block b ? new Branch(b) : s.Item2.Clone() }));
			sections.Add(new SwitchSection { Labels = new LongSet(new LongInterval(0, sections.Count)).Invert(), Body = currentCaseBlock != null ? (ILInstruction)new Branch(currentCaseBlock) : new Leave((BlockContainer)nextCaseBlock) });
			var stringToInt = new StringToInt(switchValue, values.Skip(offset).Select(item => item.Item1).ToArray(), switchValueVar.Type);
			var inst = new SwitchInstruction(stringToInt);
			inst.Sections.AddRange(sections);
			if (removeExtraLoad)
			{
				inst.AddILRange(instructions[i - 2]);
				instructions[i - 2].ReplaceWith(inst);
				instructions.RemoveRange(i - 1, 3);
				i -= 2;
			}
			else
			{
				if (keepAssignmentBefore)
				{
					inst.AddILRange(instructions[i]);
					instructions[i].ReplaceWith(inst);
					instructions.RemoveAt(i + 1);
				}
				else
				{
					inst.AddILRange(instructions[i - 1]);
					instructions[i - 1].ReplaceWith(inst);
					instructions.RemoveRange(i, 2);
					i--;
				}
			}
			return true;
		}

		private bool ValidateUsesOfSwitchValueVariable(ILVariable switchValueVar, HashSet<Block> caseBlocks)
		{
			foreach (var use in switchValueVar.LoadInstructions)
			{
				bool isValid = false;
				foreach (var caseBlock in caseBlocks)
				{
					if (use.IsDescendantOf(caseBlock))
						isValid = true;
				}
				if (!isValid)
					return false;
			}

			return true;
		}

		bool SimplifyCSharp1CascadingIfStatements(InstructionCollection<ILInstruction> instructions, ref int i)
		{
			if (i < 1)
				return false;
			// match first block:
			// stloc switchValueVar(ldloc temp)
			// if (comp(ldloc temp == ldnull)) br defaultBlock
			// br isInternedBlock
			if (!(instructions[i].MatchIfInstruction(out var condition, out var defaultBlockJump)))
				return false;
			if (!instructions[i + 1].MatchBranch(out var isInternedBlock))
				return false;
			if (!defaultBlockJump.MatchBranch(out var defaultOrNullBlock))
				return false;
			if (!(condition.MatchCompEqualsNull(out var tempLoad) && tempLoad.MatchLdLoc(out var temp)))
				return false;
			if (!(temp.Kind == VariableKind.StackSlot && temp.LoadCount == 2))
				return false;
			if (!(instructions[i - 1].MatchStLoc(out var switchValueVar, out var switchValue) && switchValue.MatchLdLoc(temp)))
				return false;
			// match isInternedBlock:
			// stloc switchValueVarCopy(call IsInterned(ldloc switchValueVar))
			// if (comp(ldloc switchValueVarCopy == ldstr "case1")) br caseBlock1
			// br caseHeader2
			if (isInternedBlock.IncomingEdgeCount != 1 || isInternedBlock.Instructions.Count != 3)
				return false;
			if (!(isInternedBlock.Instructions[0].MatchStLoc(out var switchValueVarCopy, out var arg) && IsIsInternedCall(arg as Call, out arg) && arg.MatchLdLoc(switchValueVar)))
				return false;
			switchValueVar = switchValueVarCopy;
			int conditionOffset = 1;
			Block currentCaseBlock = isInternedBlock;
			var values = new List<(string, ILInstruction)>();

			if (!switchValueVarCopy.IsSingleDefinition)
				return false;

			// each case starts with:
			// if (comp(ldloc switchValueVar == ldstr "case label")) br caseBlock
			// br currentCaseBlock

			while (currentCaseBlock.Instructions[conditionOffset].MatchIfInstruction(out condition, out var caseBlockJump))
			{
				if (currentCaseBlock.Instructions.Count != conditionOffset + 2)
					break;
				if (!condition.MatchCompEquals(out var left, out var right))
					break;
				if (!left.MatchLdLoc(switchValueVar))
					break;
				if (!right.MatchLdStr(out string value))
					break;
				if (!(caseBlockJump.MatchBranch(out var caseBlock) || caseBlockJump.MatchLeave((BlockContainer)currentCaseBlock.Parent)))
					break;
				if (!currentCaseBlock.Instructions[conditionOffset + 1].MatchBranch(out currentCaseBlock))
					break;
				conditionOffset = 0;
				values.Add((value, caseBlockJump.Clone()));
			}

			if (values.Count != switchValueVarCopy.LoadCount)
				return false;
			context.Step(nameof(SimplifyCSharp1CascadingIfStatements), instructions[i]);

			// switch contains case null: 
			if (currentCaseBlock != defaultOrNullBlock)
			{
				values.Add((null, new Branch(defaultOrNullBlock)));
			}

			var sections = new List<SwitchSection>(values.SelectWithIndex((index, b) => new SwitchSection { Labels = new LongSet(index), Body = b.Item2 }));
			sections.Add(new SwitchSection { Labels = new LongSet(new LongInterval(0, sections.Count)).Invert(), Body = new Branch(currentCaseBlock) });
			var stringToInt = new StringToInt(switchValue, values.SelectArray(item => item.Item1), context.TypeSystem.FindType(KnownTypeCode.String));
			var inst = new SwitchInstruction(stringToInt);
			inst.Sections.AddRange(sections);

			inst.AddILRange(instructions[i - 1]);
			instructions[i].ReplaceWith(inst);
			instructions.RemoveAt(i + 1);
			instructions.RemoveAt(i - 1);

			return true;
		}

		bool IsIsInternedCall(Call call, out ILInstruction argument)
		{
			if (call != null
				&& call.Method.DeclaringType.IsKnownType(KnownTypeCode.String)
				&& call.Method.IsStatic
				&& call.Method.Name == "IsInterned"
				&& call.Arguments.Count == 1)
			{
				argument = call.Arguments[0];
				return true;
			}
			argument = null;
			return false;
		}

		/// <summary>
		/// Each case consists of two blocks:
		/// 1. block:
		/// if (call op_Equality(ldloc switchVariable, ldstr value)) br caseBlock
		/// br nextBlock
		/// -or-
		/// if (comp(ldloc switchValueVar == ldnull)) br nextBlock
		/// br caseBlock
		/// 2. block is caseBlock
		/// This method matches the above pattern or its inverted form:
		/// the call to ==(string, string) is wrapped in logic.not and the branch targets are reversed.
		/// Returns the next block that follows in the block-chain.
		/// The <paramref name="switchVariable"/> is updated if the value gets copied to a different variable.
		/// See comments below for more info.
		/// </summary>
		ILInstruction MatchCaseBlock(Block currentBlock, ILVariable switchVariable, out string value, out bool emptyStringEqualsNull, out ILInstruction caseBlockOrLeave)
		{
			value = null;
			caseBlockOrLeave = null;
			emptyStringEqualsNull = false;

			if (currentBlock.IncomingEdgeCount != 1 || currentBlock.Instructions.Count != 2)
				return null;
			if (!currentBlock.MatchIfAtEndOfBlock(out var condition, out var caseBlockBranch, out var nextBlockBranch))
				return null;
			if (!MatchStringEqualityComparison(condition, switchVariable, out value, out bool isVBCompareString))
			{
				return null;
			}
			if (isVBCompareString)
			{
				ExtensionMethods.Swap(ref caseBlockBranch, ref nextBlockBranch);
				emptyStringEqualsNull = true;
			}
			if (caseBlockBranch.MatchBranch(out var caseBlock))
			{
				caseBlockOrLeave = caseBlock;
			}
			else if (caseBlockBranch.MatchLeave(out _))
			{
				caseBlockOrLeave = caseBlockBranch;
			}
			else
			{
				return null;
			}
			if (nextBlockBranch.MatchBranch(out Block nextBlock))
			{
				// success
				return nextBlock;
			}
			else if (nextBlockBranch.MatchLeave(out BlockContainer blockContainer))
			{
				// success
				return blockContainer;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Matches the C# 2.0 switch-on-string pattern, which uses Dictionary&lt;string, int&gt;.
		/// </summary>
		bool MatchLegacySwitchOnStringWithDict(InstructionCollection<ILInstruction> instructions, ref int i)
		{
			// match first block: checking switch-value for null:
			// (In some cases, i.e., if switchValueVar is a parameter, the initial store is optional.)
			// stloc switchValueVar(switchValue)
			// if (comp(ldloc switchValueVar == ldnull)) br nullCase
			// br nextBlock
			if (!instructions[i].MatchIfInstruction(out var condition, out var exitBlockJump))
				return false;
			if (!(condition.MatchCompEquals(out var left, out var right) && right.MatchLdNull()))
				return false;
			// Extract switchValueVar
			if (!left.MatchLdLoc(out var switchValueVar) || !switchValueVar.IsSingleDefinition)
				return false;
			// If the switchValueVar is a stack slot and there is an assignment involving it right before the
			// switch-value null-check, we use the previously assigned variable as switchValueVar.
			ILInstruction switchValue;
			if (switchValueVar.Kind == VariableKind.StackSlot
				&& instructions.ElementAtOrDefault(i - 1) is StLoc extraStore
				&& extraStore.Value.MatchLdLoc(switchValueVar))
			{
				switchValueVar = extraStore.Variable;
				switchValue = extraStore.Value;
			}
			else
			{
				switchValue = null;
			}
			if (!switchValueVar.Type.IsKnownType(KnownTypeCode.String))
				return false;
			// either br nullCase or leave container
			BlockContainer leaveContainer = null;
			if (!exitBlockJump.MatchBranch(out var nullValueCaseBlock) && !exitBlockJump.MatchLeave(out leaveContainer))
				return false;
			var nextBlockJump = instructions.ElementAtOrDefault(i + 1) as Branch;
			if (nextBlockJump == null || nextBlockJump.TargetBlock.IncomingEdgeCount != 1)
				return false;
			// match second block: checking compiler-generated Dictionary<string, int> for null
			// if (comp(volatile.ldobj System.Collections.Generic.Dictionary`2[[System.String],[System.Int32]](ldsflda $$method0x600000c-1) != ldnull)) br caseNullBlock
			// br dictInitBlock
			var nextBlock = nextBlockJump.TargetBlock;
			if (nextBlock.Instructions.Count != 2 || !nextBlock.Instructions[0].MatchIfInstruction(out condition, out var tryGetValueBlockJump))
				return false;
			if (!tryGetValueBlockJump.MatchBranch(out var tryGetValueBlock))
				return false;
			if (!nextBlock.Instructions[1].MatchBranch(out var dictInitBlock) || dictInitBlock.IncomingEdgeCount != 1)
				return false;
			if (!(condition.MatchCompNotEquals(out left, out right) && right.MatchLdNull() &&
				MatchDictionaryFieldLoad(left, IsStringToIntDictionary, out var dictField, out var dictionaryType)))
				return false;
			// match third block: initialization of compiler-generated Dictionary<string, int>
			// stloc dict(newobj Dictionary..ctor(ldc.i4 valuesLength))
			// call Add(ldloc dict, ldstr value, ldc.i4 index)
			// ... more calls to Add ...
			// volatile.stobj System.Collections.Generic.Dictionary`2[[System.String],[System.Int32]](ldsflda $$method0x600003f-1, ldloc dict)
			// br switchHeadBlock
			if (dictInitBlock.IncomingEdgeCount != 1 || dictInitBlock.Instructions.Count < 3)
				return false;
			if (!ExtractStringValuesFromInitBlock(dictInitBlock, out var stringValues, out var blockAfterInit, dictionaryType, dictField, false))
				return false;
			if (tryGetValueBlock != blockAfterInit)
				return false;
			// match fourth block: TryGetValue on compiler-generated Dictionary<string, int>
			// if (logic.not(call TryGetValue(volatile.ldobj System.Collections.Generic.Dictionary`2[[System.String],[System.Int32]](ldsflda $$method0x600000c-1), ldloc switchValueVar, ldloca switchIndexVar))) br defaultBlock
			// br switchBlock
			if (tryGetValueBlock.IncomingEdgeCount != 2 || tryGetValueBlock.Instructions.Count != 2)
				return false;
			if (!tryGetValueBlock.Instructions[0].MatchIfInstruction(out condition, out var defaultBlockJump))
				return false;
			if (!defaultBlockJump.MatchBranch(out var defaultBlock) && !((leaveContainer != null && defaultBlockJump.MatchLeave(leaveContainer)) || defaultBlockJump.MatchLeave(out _)))
				return false;
			if (!(condition.MatchLogicNot(out var arg) && arg is CallInstruction c && c.Method.Name == "TryGetValue" &&
				MatchDictionaryFieldLoad(c.Arguments[0], IsStringToIntDictionary, out var dictField2, out _) && dictField2.Equals(dictField)))
				return false;
			if (!c.Arguments[1].MatchLdLoc(switchValueVar) || !c.Arguments[2].MatchLdLoca(out var switchIndexVar))
				return false;
			if (!tryGetValueBlock.Instructions[1].MatchBranch(out var switchBlock))
				return false;
			// match fifth block: switch-instruction block
			// switch (ldloc switchVariable) {
			// 	case [0..1): br caseBlock1
			//  ... more cases ...
			// 	case [long.MinValue..0),[13..long.MaxValue]: br defaultBlock
			// }
			// mcs has a bug: when there is only one case it still generates the full-blown Dictionary<string, int> pattern,
			// but uses only a simple if statement instead of the switch instruction.
			if (switchBlock.IncomingEdgeCount != 1 || switchBlock.Instructions.Count == 0)
				return false;
			var sections = new List<SwitchSection>();
			switch (switchBlock.Instructions[0])
			{
				case SwitchInstruction switchInst:
					if (switchBlock.Instructions.Count != 1)
						return false;
					if (!switchInst.Value.MatchLdLoc(switchIndexVar))
						return false;
					sections.AddRange(switchInst.Sections);
					break;
				case IfInstruction ifInst:
					if (switchBlock.Instructions.Count != 2)
						return false;
					if (!ifInst.Condition.MatchCompEquals(out left, out right))
						return false;
					if (!left.MatchLdLoc(switchIndexVar))
						return false;
					if (!right.MatchLdcI4(0))
						return false;
					sections.Add(new SwitchSection() { Body = ifInst.TrueInst, Labels = new LongSet(0) }.WithILRange(ifInst));
					sections.Add(new SwitchSection() { Body = switchBlock.Instructions[1], Labels = new LongSet(0).Invert() }.WithILRange(switchBlock.Instructions[1]));
					break;
			}
			// mcs: map sections without a value to the default section, if possible
			if (!FixCasesWithoutValue(sections, stringValues))
				return false;
			// switch contains case null:
			if (nullValueCaseBlock != defaultBlock)
			{
				if (!AddNullSection(sections, stringValues, nullValueCaseBlock))
				{
					return false;
				}
			}
			context.Step(nameof(MatchLegacySwitchOnStringWithDict), instructions[i]);
			bool keepAssignmentBefore = false;
			if (switchValueVar.LoadCount > 2 || switchValue == null)
			{
				switchValue = new LdLoc(switchValueVar);
				keepAssignmentBefore = true;
			}
			var stringToInt = new StringToInt(switchValue, stringValues, switchValueVar.Type);
			var inst = new SwitchInstruction(stringToInt);
			inst.Sections.AddRange(sections);
			instructions[i + 1].ReplaceWith(inst);
			if (keepAssignmentBefore)
			{
				// delete if (comp(ldloc switchValueVar == ldnull))
				inst.AddILRange(instructions[i]);
				instructions.RemoveAt(i);
				i--;
			}
			else
			{
				// delete both the if and the assignment before
				inst.AddILRange(instructions[i - 1]);
				instructions.RemoveRange(i - 1, 2);
				i -= 2;
			}
			return true;
		}

		bool FixCasesWithoutValue(List<SwitchSection> sections, List<(string, int)> stringValues)
		{
			bool HasLabel(SwitchSection section)
			{
				return section.Labels.Values.Any(i => stringValues.Any(value => i == value.Item2));
			}

			// Pick the section with the most labels as default section.
			// And collect all sections that have no value mapped to them.
			SwitchSection defaultSection = sections.First();
			List<SwitchSection> sectionsWithoutLabels = new List<SwitchSection>();
			foreach (var section in sections)
			{
				if (section == defaultSection)
					continue;
				if (section.Labels.Count() > defaultSection.Labels.Count())
				{
					if (!HasLabel(defaultSection))
						sectionsWithoutLabels.Add(defaultSection);
					defaultSection = section;
					continue;
				}
				if (!HasLabel(section))
					sectionsWithoutLabels.Add(section);
			}

			foreach (var section in sectionsWithoutLabels)
			{
				defaultSection.Labels = defaultSection.Labels.UnionWith(section.Labels);
				if (section.HasNullLabel)
					defaultSection.HasNullLabel = true;
				sections.Remove(section);
			}

			return true;
		}

		bool AddNullSection(List<SwitchSection> sections, List<(string Value, int Index)> stringValues, Block nullValueCaseBlock)
		{
			var label = new LongSet(stringValues.Max(item => item.Index) + 1);
			var possibleConflicts = sections.Where(sec => sec.Labels.Overlaps(label)).ToArray();
			if (possibleConflicts.Length > 1)
				return false;
			else if (possibleConflicts.Length == 1)
			{
				if (possibleConflicts[0].Labels.Count() == 1)
					return false; // cannot remove only label
				possibleConflicts[0].Labels = possibleConflicts[0].Labels.ExceptWith(label);
			}
			stringValues.Add((null, (int)label.Values.First()));
			sections.Add(new SwitchSection() { Labels = label, Body = new Branch(nullValueCaseBlock) });
			return true;
		}

		/// <summary>
		/// Matches 'volatile.ldobj dictionaryType(ldsflda dictField)'
		/// </summary>
		bool MatchDictionaryFieldLoad(ILInstruction inst, Func<IType, bool> typeMatcher, out IField dictField, out IType dictionaryType)
		{
			dictField = null;
			dictionaryType = null;
			return inst.MatchLdObj(out var dictionaryFieldLoad, out dictionaryType) &&
				typeMatcher(dictionaryType) &&
				dictionaryFieldLoad.MatchLdsFlda(out dictField) &&
				(dictField.IsCompilerGeneratedOrIsInCompilerGeneratedClass() || dictField.Name.StartsWith("$$method", StringComparison.Ordinal));
		}

		/// <summary>
		/// Matches and extracts values from Add-call sequences.
		/// </summary>
		bool ExtractStringValuesFromInitBlock(Block block, out List<(string, int)> values, out Block blockAfterInit, IType dictionaryType, IField dictionaryField, bool isHashtablePattern)
		{
			values = null;
			blockAfterInit = null;
			// stloc dictVar(newobj Dictionary..ctor(ldc.i4 valuesLength))
			// -or-
			// stloc dictVar(newobj Hashtable..ctor(ldc.i4 capacity, ldc.f4 loadFactor))
			if (!(block.Instructions[0].MatchStLoc(out var dictVar, out var newObjDict) && newObjDict is NewObj newObj))
				return false;
			if (!newObj.Method.DeclaringType.Equals(dictionaryType))
				return false;
			int valuesLength = 0;
			if (newObj.Arguments.Count == 2)
			{
				if (!newObj.Arguments[0].MatchLdcI4(out valuesLength))
					return false;
				if (!newObj.Arguments[1].MatchLdcF4(0.5f))
					return false;
			}
			else if (newObj.Arguments.Count == 1)
			{
				if (!newObj.Arguments[0].MatchLdcI4(out valuesLength))
					return false;
			}
			values = new List<(string, int)>(valuesLength);
			int i = 0;
			while (MatchAddCall(dictionaryType, block.Instructions[i + 1], dictVar, out var index, out var value))
			{
				values.Add((value, index));
				i++;
			}
			// final store to compiler-generated variable:
			// volatile.stobj dictionaryType(ldsflda dictionaryField, ldloc dictVar)
			if (!(block.Instructions[i + 1].MatchStObj(out var loadField, out var dictVarLoad, out var dictType) &&
				dictType.Equals(dictionaryType) && loadField.MatchLdsFlda(out var dictField) && dictField.Equals(dictionaryField) &&
				dictVarLoad.MatchLdLoc(dictVar)))
				return false;
			if (isHashtablePattern && block.Instructions[i + 2] is IfInstruction)
			{
				return block.Instructions[i + 3].MatchBranch(out blockAfterInit);
			}
			return block.Instructions[i + 2].MatchBranch(out blockAfterInit);
		}

		/// <summary>
		/// call Add(ldloc dictVar, ldstr value, ldc.i4 index)
		/// -or-
		/// call Add(ldloc dictVar, ldstr value, box System.Int32(ldc.i4 index))
		/// </summary>
		bool MatchAddCall(IType dictionaryType, ILInstruction inst, ILVariable dictVar, out int index, out string value)
		{
			value = null;
			index = -1;
			if (!(inst is CallInstruction c && c.Method.Name == "Add" && c.Arguments.Count == 3))
				return false;
			if (!c.Arguments[0].MatchLdLoc(dictVar))
				return false;
			if (!c.Arguments[1].MatchLdStr(out value))
			{
				if (c.Arguments[1].MatchLdsFld(out var field) && field.DeclaringType.IsKnownType(KnownTypeCode.String) && field.Name == "Empty")
				{
					value = "";
				}
				else
				{
					return false;
				}
			}
			if (!(c.Method.DeclaringType.Equals(dictionaryType) && !c.Method.IsStatic))
				return false;
			return (c.Arguments[2].MatchLdcI4(out index) || (c.Arguments[2].MatchBox(out var arg, out _) && arg.MatchLdcI4(out index)));
		}

		bool IsStringToIntDictionary(IType dictionaryType)
		{
			if (dictionaryType.FullName != "System.Collections.Generic.Dictionary")
				return false;
			if (dictionaryType.TypeArguments.Count != 2)
				return false;
			return dictionaryType.TypeArguments[0].IsKnownType(KnownTypeCode.String) &&
				dictionaryType.TypeArguments[1].IsKnownType(KnownTypeCode.Int32);
		}

		bool IsNonGenericHashtable(IType dictionaryType)
		{
			if (dictionaryType.FullName != "System.Collections.Hashtable")
				return false;
			if (dictionaryType.TypeArguments.Count != 0)
				return false;
			return true;
		}

		bool MatchLegacySwitchOnStringWithHashtable(Block block, HashtableInitializer hashtableInitializers, ref int i)
		{
			// match first block: checking switch-value for null
			// stloc tmp(ldloc switch-value)
			// stloc switchVariable(ldloc tmp)
			// if (comp(ldloc tmp == ldnull)) br nullCaseBlock
			// br getItemBlock
			if (block.Instructions.Count != i + 4)
				return false;
			if (!block.Instructions[i].MatchStLoc(out var tmp, out var switchValue))
				return false;
			if (!block.Instructions[i + 1].MatchStLoc(out var switchVariable, out var tmpLoad) || !tmpLoad.MatchLdLoc(tmp))
				return false;
			if (!block.Instructions[i + 2].MatchIfInstruction(out var condition, out var nullCaseBlockBranch))
				return false;
			if (!block.Instructions[i + 3].MatchBranch(out var getItemBlock) || !(nullCaseBlockBranch.MatchBranch(out var nullCaseBlock) || nullCaseBlockBranch is Leave))
				return false;
			if (!(condition.MatchCompEquals(out var left, out var right) && right.MatchLdNull() && left.MatchLdLoc(tmp)))
				return false;
			// match second block: get_Item on compiler-generated Hashtable
			// stloc tmp2(call get_Item(volatile.ldobj System.Collections.Hashtable(ldsflda $$method0x600003f - 1), ldloc switchVariable))
			// stloc switchVariable(ldloc tmp2)
			// if (comp(ldloc tmp2 == ldnull)) br defaultCaseBlock
			// br switchBlock
			if (getItemBlock.IncomingEdgeCount != 1 || getItemBlock.Instructions.Count != 4)
				return false;
			if (!(getItemBlock.Instructions[0].MatchStLoc(out var tmp2, out var getItem) && getItem is Call getItemCall && getItemCall.Method.Name == "get_Item"))
				return false;
			if (!getItemBlock.Instructions[1].MatchStLoc(out var switchVariable2, out var tmp2Load) || !tmp2Load.MatchLdLoc(tmp2))
				return false;
			if (!ILVariableEqualityComparer.Instance.Equals(switchVariable, switchVariable2))
				return false;
			if (!getItemBlock.Instructions[2].MatchIfInstruction(out condition, out var defaultBlockBranch))
				return false;
			if (!getItemBlock.Instructions[3].MatchBranch(out var switchBlock) || !(defaultBlockBranch.MatchBranch(out var defaultBlock) || defaultBlockBranch is Leave))
				return false;
			if (!(condition.MatchCompEquals(out left, out right) && right.MatchLdNull() && left.MatchLdLoc(tmp2)))
				return false;
			if (!(getItemCall.Arguments.Count == 2 && MatchDictionaryFieldLoad(getItemCall.Arguments[0], IsNonGenericHashtable, out var dictField, out _) && getItemCall.Arguments[1].MatchLdLoc(switchVariable)))
				return false;
			// Check if there is a hashtable init block at the beginning of the method
			if (!hashtableInitializers.TryGetValue(dictField, out var info))
				return false;
			var stringValues = info.Labels;
			// match third block: switch-instruction block
			// switch (ldobj System.Int32(unbox System.Int32(ldloc switchVariable))) {
			// 	case [0..1): br caseBlock1
			//  ... more cases ...
			// 	case [long.MinValue..0),[13..long.MaxValue]: br defaultBlock
			// }
			if (switchBlock.IncomingEdgeCount != 1 || switchBlock.Instructions.Count != 1)
				return false;
			if (!(switchBlock.Instructions[0] is SwitchInstruction switchInst && switchInst.Value.MatchLdObj(out var target, out var ldobjType) &&
				target.MatchUnbox(out var arg, out var unboxType) && arg.MatchLdLoc(switchVariable2) && ldobjType.IsKnownType(KnownTypeCode.Int32) && unboxType.Equals(ldobjType)))
				return false;
			var sections = new List<SwitchSection>(switchInst.Sections);
			// switch contains case null: 
			if (!(nullCaseBlockBranch is Leave) && nullCaseBlock != defaultBlock)
			{
				if (!AddNullSection(sections, stringValues, nullCaseBlock))
				{
					return false;
				}
			}
			context.Step(nameof(MatchLegacySwitchOnStringWithHashtable), block.Instructions[i]);
			var stringToInt = new StringToInt(switchValue, stringValues, context.TypeSystem.FindType(KnownTypeCode.String));
			var inst = new SwitchInstruction(stringToInt);
			inst.Sections.AddRange(sections);
			inst.AddILRange(block.Instructions[i]);
			block.Instructions[i].ReplaceWith(inst);
			block.Instructions.RemoveRange(i + 1, 3);
			info.Transformed = true;
			hashtableInitializers[dictField] = info;
			return true;
		}

		bool FindHashtableInitBlock(Block entryPoint, out List<(string, int)> stringValues, out IField dictField, out Block blockAfterThisInitBlock, out ILInstruction thisSwitchInitJumpInst, out ILInstruction nextSwitchInitJumpInst)
		{
			stringValues = null;
			dictField = null;
			blockAfterThisInitBlock = null;
			nextSwitchInitJumpInst = null;
			thisSwitchInitJumpInst = null;
			if (entryPoint.Instructions.Count != 2)
				return false;
			// match first block: checking compiler-generated Hashtable for null
			// if (comp(volatile.ldobj System.Collections.Hashtable(ldsflda $$method0x600003f-1) != ldnull)) br switchHeadBlock
			// br tableInitBlock
			if (!(entryPoint.Instructions[0].MatchIfInstruction(out var condition, out var branchToSwitchHead)))
				return false;
			if (!entryPoint.Instructions[1].MatchBranch(out var tableInitBlock))
				return false;
			if (!(condition.MatchCompNotEquals(out var left, out var right) && right.MatchLdNull() &&
				MatchDictionaryFieldLoad(left, IsNonGenericHashtable, out dictField, out var dictionaryType)))
				return false;
			if (!branchToSwitchHead.MatchBranch(out var switchHead))
				return false;
			thisSwitchInitJumpInst = entryPoint.Instructions[0];
			// match second block: initialization of compiler-generated Hashtable
			// stloc table(newobj Hashtable..ctor(ldc.i4 capacity, ldc.f loadFactor))
			// call Add(ldloc table, ldstr value, box System.Int32(ldc.i4 index))
			// ... more calls to Add ...
			// volatile.stobj System.Collections.Hashtable(ldsflda $$method0x600003f - 1, ldloc table)
			// br switchHeadBlock
			if (tableInitBlock.IncomingEdgeCount != 1 || tableInitBlock.Instructions.Count < 3)
				return false;
			if (!ExtractStringValuesFromInitBlock(tableInitBlock, out stringValues, out blockAfterThisInitBlock, dictionaryType, dictField, true))
				return false;
			// if there is another IfInstruction before the end of the block, it might be a jump to the next hashtable init block.
			// if (comp(volatile.ldobj System.Collections.Hashtable(ldsflda $$method0x600003f-2) != ldnull)) br switchHeadBlock
			if (tableInitBlock.Instructions.SecondToLastOrDefault() is IfInstruction nextHashtableInitHead)
			{
				if (!(nextHashtableInitHead.Condition.MatchCompNotEquals(out left, out right) && right.MatchLdNull() &&
					MatchDictionaryFieldLoad(left, IsNonGenericHashtable, out var nextSwitchInitField, out _)))
					return false;
				if (!nextHashtableInitHead.TrueInst.MatchBranch(switchHead))
					return false;
				nextSwitchInitJumpInst = nextHashtableInitHead;
			}
			return true;
		}

		bool MatchRoslynSwitchOnString(InstructionCollection<ILInstruction> instructions, ref int i)
		{
			if (i >= instructions.Count - 1)
				return false;
			// stloc switchValueVar(switchValue)
			// if (comp(ldloc switchValueVar == ldnull)) br nullCase
			// br nextBlock
			InstructionCollection<ILInstruction> switchBlockInstructions = instructions;
			int switchBlockInstructionsOffset = i;
			Block nullValueCaseBlock = null;
			ILInstruction instForNullCheck = null;
			if (instructions[i].MatchIfInstruction(out var condition, out var exitBlockJump)
				&& condition.MatchCompEqualsNull(out instForNullCheck))
			{
				var nextBlockJump = instructions[i + 1] as Branch;
				if (nextBlockJump == null || nextBlockJump.TargetBlock.IncomingEdgeCount != 1)
					return false;
				if (!exitBlockJump.MatchBranch(out nullValueCaseBlock))
					return false;
				switchBlockInstructions = nextBlockJump.TargetBlock.Instructions;
				switchBlockInstructionsOffset = 0;
			}
			// stloc switchValueVar(call ComputeStringHash(switchValueLoad))
			// switch (ldloc switchValueVar) {
			// 	case [211455823..211455824): br caseBlock1
			//  ... more cases ...
			// 	case [long.MinValue..-365098645),...,[1697255802..long.MaxValue]: br defaultBlock
			// }
			if (!(switchBlockInstructionsOffset + 1 < switchBlockInstructions.Count
				&& switchBlockInstructions[switchBlockInstructionsOffset + 1] is SwitchInstruction switchInst
				&& switchInst.Value.MatchLdLoc(out var switchValueVar)
				&& MatchComputeStringOrReadOnlySpanHashCall(switchBlockInstructions[switchBlockInstructionsOffset],
											  switchValueVar, out LdLoc switchValueLoad)))
			{
				return false;
			}

			if (instForNullCheck != null && !instForNullCheck.MatchLdLoc(switchValueLoad.Variable))
			{
				return false;
			}

			var stringValues = new List<(string Value, ILInstruction TargetBlockOrLeave)>();
			SwitchSection defaultSection = switchInst.GetDefaultSection();
			if (!(defaultSection.Body.MatchBranch(out Block exitOrDefaultBlock) || defaultSection.Body.MatchLeave(out _)))
				return false;
			foreach (var section in switchInst.Sections)
			{
				if (section == defaultSection)
					continue;
				// extract target block
				if (!section.Body.MatchBranch(out Block target))
					return false;
				string stringValue;
				bool emptyStringEqualsNull;
				if (MatchRoslynEmptyStringCaseBlockHead(target, switchValueLoad.Variable, out ILInstruction targetOrLeave, out Block currentExitBlock))
				{
					stringValue = "";
					emptyStringEqualsNull = false;
				}
				else if (!MatchRoslynCaseBlockHead(target, switchValueLoad.Variable, out targetOrLeave, out currentExitBlock, out stringValue, out emptyStringEqualsNull))
				{
					return false;
				}

				if (currentExitBlock != exitOrDefaultBlock)
					return false;
				if (emptyStringEqualsNull && string.IsNullOrEmpty(stringValue))
				{
					stringValues.Add((null, targetOrLeave));
					stringValues.Add((string.Empty, targetOrLeave));
				}
				else
				{
					stringValues.Add((stringValue, targetOrLeave));
				}
			}

			if (nullValueCaseBlock != null && exitOrDefaultBlock != nullValueCaseBlock)
			{
				stringValues.Add((null, nullValueCaseBlock));
			}
			// In newer Roslyn versions (>=3.7) the null check appears in the default case, not prior to the switch.
			ILInstruction exitOrDefault = exitOrDefaultBlock;
			if (!stringValues.Any(pair => pair.Value == null) && IsNullCheckInDefaultBlock(ref exitOrDefault, switchValueLoad.Variable, out nullValueCaseBlock))
			{
				stringValues.Add((null, nullValueCaseBlock));
			}
			exitOrDefaultBlock = (Block)exitOrDefault;

			context.Step(nameof(MatchRoslynSwitchOnString), switchValueLoad);
			if (exitOrDefaultBlock != null)
			{
				// change TargetBlock in case it was modified by IsNullCheckInDefaultBlock()
				((Branch)defaultSection.Body).TargetBlock = exitOrDefaultBlock;
			}
			ILInstruction switchValueInst = switchValueLoad;
			if (instructions == switchBlockInstructions)
			{
				// stloc switchValueLoadVariable(switchValue)
				// stloc switchValueVar(call ComputeStringHash(ldloc switchValueLoadVariable))
				// switch (ldloc switchValueVar) {
				bool keepAssignmentBefore;
				// if the switchValueLoad.Variable is only used in the compiler generated case equality checks, we can remove it.
				if (i >= 1 && instructions[i - 1].MatchStLoc(switchValueLoad.Variable, out var switchValueTmp) &&
					switchValueLoad.Variable.IsSingleDefinition && switchValueLoad.Variable.LoadCount == switchInst.Sections.Count)
				{
					switchValueInst = switchValueTmp;
					keepAssignmentBefore = false;
				}
				else
				{
					keepAssignmentBefore = true;
				}
				// replace stloc switchValueVar(call ComputeStringHash(...)) with new switch instruction
				var newSwitch = ReplaceWithSwitchInstruction(i);
				// remove old switch instruction
				newSwitch.AddILRange(instructions[i + 1]);
				instructions.RemoveAt(i + 1);
				// remove extra assignment
				if (!keepAssignmentBefore)
				{
					newSwitch.AddILRange(instructions[i - 1]);
					instructions.RemoveRange(i - 1, 1);
					i -= 1;
				}
			}
			else
			{
				bool keepAssignmentBefore;
				// if the switchValueLoad.Variable is only used in the compiler generated case equality checks, we can remove it.
				if (i >= 2 && instructions[i - 2].MatchStLoc(out var temporary, out var temporaryValue) && instructions[i - 1].MatchStLoc(switchValueLoad.Variable, out var tempLoad) && tempLoad.MatchLdLoc(temporary))
				{
					switchValueInst = temporaryValue;
					keepAssignmentBefore = false;
				}
				else
				{
					keepAssignmentBefore = true;
				}
				// replace null check with new switch instruction
				var newSwitch = ReplaceWithSwitchInstruction(i);
				newSwitch.AddILRange(switchInst);
				// remove jump instruction to switch block
				newSwitch.AddILRange(instructions[i + 1]);
				instructions.RemoveAt(i + 1);
				// remove extra assignment
				if (!keepAssignmentBefore)
				{
					newSwitch.AddILRange(instructions[i - 2]);
					instructions.RemoveRange(i - 2, 2);
					i -= 2;
				}
			}

			return true;

			SwitchInstruction ReplaceWithSwitchInstruction(int offset)
			{
				var defaultLabel = new LongSet(new LongInterval(0, stringValues.Count)).Invert();
				var values = new string[stringValues.Count];
				var sections = new SwitchSection[stringValues.Count];
				foreach (var (idx, (value, bodyInstruction)) in stringValues.WithIndex())
				{
					values[idx] = value;
					var body = bodyInstruction is Block b ? new Branch(b) : bodyInstruction;
					sections[idx] = new SwitchSection { Labels = new LongSet(idx), Body = body };
				}
				var newSwitch = new SwitchInstruction(new StringToInt(switchValueInst, values, switchValueLoad.Variable.Type));
				newSwitch.Sections.AddRange(sections);
				newSwitch.Sections.Add(new SwitchSection { Labels = defaultLabel, Body = defaultSection.Body });
				instructions[offset].ReplaceWith(newSwitch);
				return newSwitch;
			}
		}

		private bool MatchRoslynSwitchOnStringUsingLengthAndChar(Block block, int i)
		{
			var instructions = block.Instructions;
			// implements https://github.com/dotnet/roslyn/pull/66081
			// if (comp(ldloc switchValueVar == ldnull)) br nullCase
			// br nextBlock
			Block switchOnLengthBlock;
			int switchOnLengthBlockStartOffset;
			Block nullCase = null;
			if (instructions[i].MatchIfInstruction(out var condition, out var exitBlockJump)
				&& condition.MatchCompEqualsNull(out var ldloc)
				&& ldloc is LdLoc { Variable: var switchValueVar })
			{
				if (!instructions[i + 1].MatchBranch(out var nextBlock))
					return false;
				if (!exitBlockJump.MatchBranch(out nullCase) && !exitBlockJump.MatchLeave(out _))
					return false;
				// if (comp(ldloc switchValueVar == ldnull)) br ...
				// br switchOnLengthBlock
				if (nextBlock.IncomingEdgeCount == 1
					&& nextBlock.Instructions[0].MatchIfInstruction(out condition, out _)
					&& condition.MatchCompEqualsNull(out ldloc)
					&& ldloc.MatchLdLoc(switchValueVar))
				{
					if (!nextBlock.Instructions[1].MatchBranch(out switchOnLengthBlock))
						return false;
				}
				else
				{
					switchOnLengthBlock = nextBlock;
				}
				if (switchOnLengthBlock.IncomingEdgeCount != 1)
					return false;
				switchOnLengthBlockStartOffset = 0;
			}
			else
			{
				switchOnLengthBlock = block;
				switchValueVar = null; // will be extracted in MatchSwitchOnLengthBlock
				switchOnLengthBlockStartOffset = i;
			}
			ILInstruction defaultCase = null;
			if (!MatchSwitchOnLengthBlock(ref switchValueVar, switchOnLengthBlock, switchOnLengthBlockStartOffset, out var blocksByLength))
				return false;
			List<(string, ILInstruction)> stringValues = new();
			foreach (var b in blocksByLength)
			{
				if (b.Length.Count() != 1)
				{
					if (b.TargetBlock != nullCase)
						return false;
				}
				else
				{
					int length = (int)b.Length.Intervals[0].Start;
					switch (b.TargetBlock)
					{
						case Leave leave:
							break;
						case Block targetBlock:
							if (MatchSwitchOnCharBlock(targetBlock, length, switchValueVar, out var mapping))
							{
								foreach (var item in mapping)
								{
									if (!stringValues.Any(x => x.Item1 == item.StringValue))
									{
										stringValues.Add(item);
									}
									else
									{
										return false;
									}
								}
							}
							else if (MatchRoslynCaseBlockHead(targetBlock, switchValueVar, out var bodyOrLeave, out var exit, out string stringValue, out _))
							{
								if (exit != defaultCase)
									return false;
								if (!stringValues.Any(x => x.Item1 == stringValue))
								{
									stringValues.Add((stringValue, bodyOrLeave));
								}
								else
								{
									return false;
								}
							}
							else if (length == 0)
							{
								stringValues.Add(("", b.TargetBlock));
							}
							else
							{
								return false;
							}
							break;
						default:
							return false;
					}

				}
			}

			if (!stringValues.Any(pair => pair.Item1 == null))
			{
				if (IsNullCheckInDefaultBlock(ref defaultCase, switchValueVar, out var nullBlock))
				{
					stringValues.Add((null, nullBlock));
				}
				else if (nullCase != null && nullCase != defaultCase)
				{
					stringValues.Add((null, nullCase));
				}
			}

			context.Step(nameof(MatchRoslynSwitchOnStringUsingLengthAndChar), instructions[i]);
			var defaultLabel = new LongSet(new LongInterval(0, stringValues.Count)).Invert();
			var values = new string[stringValues.Count];
			var sections = new SwitchSection[stringValues.Count];
			foreach (var (idx, (value, bodyInstruction)) in stringValues.WithIndex())
			{
				values[idx] = value;
				var body = bodyInstruction is Block b ? new Branch(b) : bodyInstruction;
				sections[idx] = new SwitchSection { Labels = new LongSet(idx), Body = body };
			}
			var newSwitch = new SwitchInstruction(new StringToInt(new LdLoc(switchValueVar), values, switchValueVar.Type));
			newSwitch.Sections.AddRange(sections);
			newSwitch.Sections.Add(new SwitchSection { Labels = defaultLabel, Body = defaultCase is Block b2 ? new Branch(b2) : defaultCase });
			newSwitch.AddILRange(instructions[i]);
			if (nullCase != null)
			{
				newSwitch.AddILRange(instructions[i + 1]);
			}
			instructions[i] = newSwitch;
			instructions.RemoveRange(i + 1, instructions.Count - (i + 1));
			return true;

			bool MatchGetChars(ILInstruction instruction, ILVariable switchValueVar, out int index)
			{
				index = -1;
				if (context.Settings.SwitchOnReadOnlySpanChar && instruction.MatchLdObj(out var target, out var type) && type.IsKnownType(KnownTypeCode.UInt16))
				{
					return target is Call call
						&& (call.Method.FullNameIs("System.ReadOnlySpan", "get_Item")
						|| call.Method.FullNameIs("System.Span", "get_Item"))
						&& call.Arguments.Count == 2
						&& call.Arguments[0].MatchLdLoca(switchValueVar)
						&& call.Arguments[1].MatchLdcI4(out index);
				}
				else
				{
					return instruction is Call call
						&& call.Method.FullNameIs("System.String", "get_Chars")
						&& call.Arguments.Count == 2
						&& call.Arguments[0].MatchLdLoc(switchValueVar)
						&& call.Arguments[1].MatchLdcI4(out index);
				}
			}

			bool MatchSwitchOnCharBlock(Block block, int length, ILVariable switchValueVar, out List<(string StringValue, ILInstruction BodyOrLeave)> results)
			{
				results = null;
				if (block.IncomingEdgeCount != 1)
					return false;
				SwitchInstruction @switch;
				List<KeyValuePair<LongSet, ILInstruction>> sections;
				int index;
				switch (block.Instructions.Count)
				{
					case 1:
						@switch = block.Instructions[0] as SwitchInstruction;
						if (@switch == null)
							return false;
						if (!MatchGetChars(@switch.Value, switchValueVar, out index))
							return false;
						sections = @switch.Sections.SelectList(s => new KeyValuePair<LongSet, ILInstruction>(s.Labels, s.Body));
						break;
					case 2:
						if (!block.Instructions[0].MatchStLoc(out var charTempVar, out var getCharsCall))
							return false;
						if (!MatchGetChars(getCharsCall, switchValueVar, out index))
							return false;
						if (index < 0)
							return false;
						@switch = block.Instructions[1] as SwitchInstruction;
						if (@switch == null)
							return false;
						if (!@switch.Value.MatchLdLoc(charTempVar))
							return false;
						sections = @switch.Sections.SelectList(s => new KeyValuePair<LongSet, ILInstruction>(s.Labels, s.Body));
						break;
					default:
						if (!analysis.AnalyzeBlock(block))
						{
							return false;
						}
						if (!block.Instructions[0].MatchStLoc(out charTempVar, out getCharsCall))
							return false;
						if (!MatchGetChars(getCharsCall, switchValueVar, out index))
							return false;
						if (index < 0)
							return false;
						if (analysis.SwitchVariable != charTempVar)
							return false;
						sections = analysis.Sections;
						break;
				}
				if (index >= length)
					return false;
				bool hasDefaultSection = false;
				foreach (var (labels, body) in sections)
				{
					if (labels.Count() == 1)
					{
						char ch = unchecked((char)labels.Values.Single());
						if (!body.MatchBranch(out var targetBlock))
							return false;
						if (length == 1)
						{
							results ??= new();
							results.Add((ch.ToString(), targetBlock));
						}
						else
						{
							while (MatchRoslynCaseBlockHead(targetBlock, switchValueVar, out var bodyOrLeave, out var exit, out var stringValue, out _))
							{
								if (stringValue.Length != length || stringValue[index] != ch)
									return false;
								results ??= new();
								results.Add((stringValue, bodyOrLeave));
								if (exit == nullCase)
									break;
								targetBlock = exit;
							}
						}
					}
					else if (!hasDefaultSection)
					{
						hasDefaultSection = true;
					}
					else
					{
						return false;
					}
				}
				return results?.Count > 0;
			}

			bool MatchSwitchOnLengthBlock(ref ILVariable switchValueVar, Block switchOnLengthBlock, int startOffset, out List<(LongSet Length, ILInstruction TargetBlock)> blocks)
			{
				blocks = null;
				SwitchInstruction @switch;
				ILInstruction getLengthCall;
				ILVariable lengthVar;
				switch (switchOnLengthBlock.Instructions.Count - startOffset)
				{
					case 1:
						@switch = switchOnLengthBlock.Instructions[startOffset] as SwitchInstruction;
						if (@switch == null)
							return false;
						getLengthCall = @switch.Value;
						break;
					case 2:
						if (!switchOnLengthBlock.Instructions[startOffset].MatchStLoc(out lengthVar, out getLengthCall))
							return false;
						@switch = switchOnLengthBlock.Instructions[startOffset + 1] as SwitchInstruction;
						if (@switch == null)
							return false;
						if (!@switch.Value.MatchLdLoc(lengthVar))
							return false;
						break;
					case 3:
						@switch = null;
						if (!switchOnLengthBlock.Instructions[startOffset].MatchStLoc(out lengthVar, out getLengthCall))
							return false;
						if (!switchOnLengthBlock.Instructions[startOffset + 1].MatchIfInstruction(out var cond, out var gotoLength))
							return false;
						if (!gotoLength.MatchBranch(out var target))
							return false;
						if (!switchOnLengthBlock.Instructions[startOffset + 2].MatchBranch(out var gotoElse))
							return false;
						if (!cond.MatchCompEquals(out var lhs, out var rhs))
						{
							if (!cond.MatchCompNotEquals(out lhs, out rhs))
								return false;
							var t = target;
							target = gotoElse;
							gotoElse = t;
						}
						defaultCase = gotoElse;
						if (!lhs.MatchLdLoc(lengthVar) || !rhs.MatchLdcI4(out int length))
							return false;

						blocks = new() {
							(new LongSet(length), target),
							(new LongSet(length).Invert(), defaultCase)
						};
						break;
					default:
						return false;
				}
				if (getLengthCall is not Call call
					|| call.Arguments.Count != 1
					|| call.Method.Name != "get_Length")
				{
					return false;
				}
				var declaringTypeCode = call.Method.DeclaringTypeDefinition?.KnownTypeCode;
				switch (declaringTypeCode)
				{
					case KnownTypeCode.String:
						if (!call.Arguments[0].MatchLdLoc(switchValueVar))
							return false;
						break;
					case KnownTypeCode.ReadOnlySpanOfT:
					case KnownTypeCode.SpanOfT:
						if (!context.Settings.SwitchOnReadOnlySpanChar)
							return false;
						if (!call.Arguments[0].MatchLdLoca(out switchValueVar))
							return false;
						break;
					default:
						return false;
				}
				if (@switch == null)
					return true;
				blocks = new(@switch.Sections.Count);
				foreach (var section in @switch.Sections)
				{
					if (section.HasNullLabel)
						return false;
					if (!section.Body.MatchBranch(out var target) && !section.Body.MatchLeave(out _))
						return false;
					ILInstruction targetInst = target ?? section.Body;
					if (section.Labels.Count() != 1)
					{
						defaultCase ??= targetInst;
						if (defaultCase != targetInst)
							return false;
					}
					else
					{
						blocks.Add((section.Labels, targetInst));
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Matches:
		/// Block oldDefaultBlock (incoming: 1) {
		///     if (comp.o(ldloc switchVar == ldnull)) br nullValueCaseBlock
		///	    br newDefaultBlock
		/// }
		/// </summary>
		private bool IsNullCheckInDefaultBlock(ref ILInstruction exitOrDefault, ILVariable switchVar, out Block nullValueCaseBlock)
		{
			nullValueCaseBlock = null;
			if (exitOrDefault is not Block exitOrDefaultBlock)
				return false;
			if (!exitOrDefaultBlock.Instructions[0].MatchIfInstruction(out var condition, out var thenBranch))
				return false;
			if (!(condition.MatchCompEqualsNull(out var arg) && arg.MatchLdLoc(switchVar)))
				return false;
			if (!thenBranch.MatchBranch(out nullValueCaseBlock))
				return false;
			if (nullValueCaseBlock.Parent != exitOrDefaultBlock.Parent)
				return false;
			if (!exitOrDefaultBlock.Instructions[1].MatchBranch(out var elseBlock))
				return false;
			if (elseBlock.Parent != exitOrDefaultBlock.Parent)
				return false;
			exitOrDefault = elseBlock;
			return true;
		}

		/// <summary>
		/// Matches (and the negated version):
		/// if (call op_Equality(ldloc switchValueVar, stringValue)) br body
		/// br exit
		/// </summary>
		bool MatchRoslynCaseBlockHead(Block target, ILVariable switchValueVar, out ILInstruction bodyOrLeave, out Block defaultOrExitBlock, out string stringValue, out bool emptyStringEqualsNull)
		{
			bodyOrLeave = null;
			defaultOrExitBlock = null;
			stringValue = null;
			emptyStringEqualsNull = false;
			if (target.Instructions.Count != 2)
				return false;
			if (!target.Instructions[0].MatchIfInstruction(out var condition, out var bodyBranch))
			{
				// Special case: sometimes we don't have an if, because bodyBranch==exitBranch
				// and the C# compiler optimized out the if.
				// Example:
				//  Block IL_0063 (incoming: 1) {
				//   call op_Equality(ldloc V_4, ldstr "rowno")
				//   leave IL_0000(nop)
				// }
				condition = target.Instructions[0];
				bodyBranch = target.Instructions[1];
			}
			ILInstruction exitBranch = target.Instructions[1];
			// Handle negated conditions first:
			while (condition.MatchLogicNot(out var expr))
			{
				ExtensionMethods.Swap(ref exitBranch, ref bodyBranch);
				condition = expr;
			}
			if (!MatchStringEqualityComparison(condition, switchValueVar, out stringValue, out bool isVBCompareString))
				return false;
			if (isVBCompareString)
			{
				ExtensionMethods.Swap(ref exitBranch, ref bodyBranch);
				emptyStringEqualsNull = true;
			}
			if (!(exitBranch.MatchBranch(out defaultOrExitBlock) || exitBranch.MatchLeave(out _)))
				return false;
			if (bodyBranch.MatchLeave(out _))
			{
				bodyOrLeave = bodyBranch;
				return true;
			}
			if (bodyBranch.MatchBranch(out var bodyBlock))
			{
				bodyOrLeave = bodyBlock;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Block target(incoming: 1) {
		/// 	if (comp.o(ldloc switchValueVar == ldnull)) br exit
		/// 	br lengthCheckBlock
		/// }
		/// 
		/// Block lengthCheckBlock(incoming: 1) {
		/// 	if (logic.not(call get_Length(ldloc switchValueVar))) br body
		/// 	br exit
		/// }
		/// </summary>
		bool MatchRoslynEmptyStringCaseBlockHead(Block target, ILVariable switchValueVar, out ILInstruction bodyOrLeave, out Block defaultOrExitBlock)
		{
			bodyOrLeave = null;
			defaultOrExitBlock = null;
			if (target.Instructions.Count != 2 || target.IncomingEdgeCount != 1)
				return false;
			if (!target.Instructions[0].MatchIfInstruction(out var nullComparisonCondition, out var exitBranch))
				return false;
			if (!nullComparisonCondition.MatchCompEqualsNull(out var arg) || !arg.MatchLdLoc(switchValueVar))
				return false;
			if (!target.Instructions[1].MatchBranch(out Block lengthCheckBlock))
				return false;
			if (lengthCheckBlock.Instructions.Count != 2 || lengthCheckBlock.IncomingEdgeCount != 1)
				return false;
			if (!lengthCheckBlock.Instructions[0].MatchIfInstruction(out var lengthCheckCondition, out var exitBranch2))
				return false;
			ILInstruction bodyBranch;
			if (lengthCheckCondition.MatchLogicNot(out arg))
			{
				bodyBranch = exitBranch2;
				exitBranch2 = lengthCheckBlock.Instructions[1];
				lengthCheckCondition = arg;
			}
			else
			{
				bodyBranch = lengthCheckBlock.Instructions[1];
			}
			if (!(exitBranch2.MatchBranch(out defaultOrExitBlock) || exitBranch2.MatchLeave(out _)))
				return false;
			if (!MatchStringLengthCall(lengthCheckCondition, switchValueVar))
				return false;
			if (!exitBranch.Match(exitBranch2).Success)
				return false;
			if (bodyBranch.MatchLeave(out _))
			{
				bodyOrLeave = bodyBranch;
				return true;
			}
			if (bodyBranch.MatchBranch(out var bodyBlock))
			{
				bodyOrLeave = bodyBlock;
				return true;
			}
			return false;
		}

		/// <summary>
		/// call get_Length(ldloc switchValueVar)
		/// </summary>
		bool MatchStringLengthCall(ILInstruction inst, ILVariable switchValueVar)
		{
			return inst is Call call
				&& call.Method.DeclaringType.IsKnownType(KnownTypeCode.String)
				&& call.Method.IsAccessor
				&& call.Method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter
				&& call.Method.AccessorOwner.Name == "Length"
				&& call.Arguments.Count == 1
				&& call.Arguments[0].MatchLdLoc(switchValueVar);
		}

		/// <summary>
		/// Matches
		/// 'stloc(targetVar, call ComputeStringHash(ldloc switchValue))'
		/// - or -
		/// 'stloc(targetVar, call ComputeSpanHash(ldloc switchValue))'
		/// - or -
		/// 'stloc(targetVar, call ComputeReadOnlySpanHash(ldloc switchValue))'
		/// </summary>
		internal static bool MatchComputeStringOrReadOnlySpanHashCall(ILInstruction inst, ILVariable targetVar, out LdLoc switchValue)
		{
			switchValue = null;
			if (!inst.MatchStLoc(targetVar, out var value))
				return false;
			if (value is Call c && c.Arguments.Count == 1
				&& c.Method.Name is "ComputeStringHash" or "ComputeSpanHash" or "ComputeReadOnlySpanHash"
				&& c.Method.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
			{
				if (c.Arguments[0] is not LdLoc ldloc)
					return false;
				switchValue = ldloc;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Matches 'call string.op_Equality(ldloc(variable), ldstr stringValue)'
		///      or 'comp(ldloc(variable) == ldnull)'
		///      or 'call SequenceEqual(ldloc variable, call AsSpan(ldstr stringValue))'
		/// </summary>
		bool MatchStringEqualityComparison(ILInstruction condition, ILVariable variable, out string stringValue, out bool isVBCompareString)
		{
			return MatchStringEqualityComparison(condition, out var v, out stringValue, out isVBCompareString) && v == variable;
		}

		/// <summary>
		/// Matches 'call string.op_Equality(ldloc(variable), ldstr stringValue)'
		///      or 'comp(ldloc(variable) == ldnull)'
		///      or 'call SequenceEqual(ldloc variable, call AsSpan(ldstr stringValue))'
		/// </summary>
		bool MatchStringEqualityComparison(ILInstruction condition, out ILVariable variable, out string stringValue, out bool isVBCompareString)
		{
			stringValue = null;
			variable = null;
			isVBCompareString = false;
			while (condition is Comp comp && comp.Kind == ComparisonKind.Inequality && comp.Right.MatchLdcI4(0))
			{
				// if (x != 0) == if (x)
				condition = comp.Left;
			}
			if (condition is Call c)
			{
				ILInstruction left, right;
				if (c.Method.IsOperator && c.Method.Name == "op_Equality"
					&& c.Method.DeclaringType.IsKnownType(KnownTypeCode.String) && c.Arguments.Count == 2)
				{
					left = c.Arguments[0];
					right = c.Arguments[1];
				}
				else if (c.Method.IsStatic && c.Method.Name == "CompareString"
					&& c.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators"
					&& c.Arguments.Count == 3)
				{
					left = c.Arguments[0];
					right = c.Arguments[1];
					// VB CompareString(): return 0 on equality -> condition is effectively negated.
					// Also, the empty string is considered equal to null.
					isVBCompareString = true;
					if (!c.Arguments[2].MatchLdcI4(0))
					{
						// Option Compare Text: case insensitive comparison is not supported in C#
						return false;
					}
				}
				else if (c.Method.IsStatic && c.Method.Name == "SequenceEqual"
					&& c.Method.DeclaringType.FullName == "System.MemoryExtensions"
					&& c.Arguments.Count == 2)
				{
					left = c.Arguments[0];
					if (c.Arguments[1] is Call {
						Method.IsStatic: true,
						Method.Name: "AsSpan",
						Method.DeclaringType.FullName: "System.MemoryExtensions",
						Arguments: [var ldStr]
					} asSpanCall)
					{
						right = ldStr;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
				return left.MatchLdLoc(out variable) && right.MatchLdStr(out stringValue);
			}
			else if (condition.MatchCompEqualsNull(out var arg))
			{
				stringValue = null;
				return arg.MatchLdLoc(out variable);
			}
			else
			{
				return false;
			}
		}
	}
}
