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
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class SwitchOnStringTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			HashSet<BlockContainer> changedContainers = new HashSet<BlockContainer>();

			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					SwitchInstruction newSwitch;
					Block blockAfterSwitch = null;
					if (!MatchCascadingIfStatements(block.Instructions, i, out newSwitch, out blockAfterSwitch) &&
						!MatchLegacySwitchOnString(block.Instructions, i, out newSwitch, out blockAfterSwitch) &&
						!MatchRoslynSwitchOnString(block.Instructions, i, out newSwitch))
						continue;

					if (i + 1 < block.Instructions.Count && block.Instructions[i + 1] is Branch b && blockAfterSwitch != null) {
						block.Instructions[i + 1].ReplaceWith(new Branch(blockAfterSwitch));
					}

					block.Instructions[i].ReplaceWith(newSwitch);
					if (newSwitch.Value.MatchLdLoc(out var switchVar) && !block.Instructions[i - 1].MatchLdLoc(switchVar)) {
						block.Instructions.RemoveAt(i - 1);
						i--;
					}

					// This happens in some cases:
					// Use correct index after transformation.
					if (i >= block.Instructions.Count)
						i = block.Instructions.Count;
				}

				if (block.Parent is BlockContainer container)
					changedContainers.Add(container);
			}

			foreach (var container in changedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);
		}

		bool MatchCascadingIfStatements(InstructionCollection<ILInstruction> instructions, int i, out SwitchInstruction inst, out Block blockAfterSwitch)
		{
			inst = null;
			blockAfterSwitch = null;
			if (i < 1) return false;
			// match first block: checking switch-value for null or first value (Roslyn)
			if (!(instructions[i].MatchIfInstruction(out var condition, out var firstBlockJump) &&
				instructions[i - 1].MatchStLoc(out var switchValueVar, out var switchValue) && switchValueVar.Type.IsKnownType(KnownTypeCode.String)))
				return false;
			if (!firstBlockJump.MatchBranch(out var firstBlock))
				return false;
			bool isLegacy;
			Block defaultBlock;
			List<(string, Block)> values = new List<(string, Block)>();
			if (condition.MatchCompEquals(out var left, out var right) && right.MatchLdNull() && left.MatchLdLoc(switchValueVar)) {
				isLegacy = true;
				defaultBlock = firstBlock;
			} else if (MatchStringEqualityComparison(condition, switchValueVar, out string value)) {
				isLegacy = false;
				defaultBlock = null;
				values.Add((value, firstBlock));
			} else return false;
			if (!(instructions.ElementAtOrDefault(i + 1) is Branch nextCaseJump))
				return false;
			Block currentCaseBlock = nextCaseJump.TargetBlock;
			Block nextCaseBlock;
			while ((nextCaseBlock = MatchCaseBlock(currentCaseBlock, switchValueVar, out string value, out Block block)) != null) {
				values.Add((value, block));
				currentCaseBlock = nextCaseBlock;
			}
			if (!ExtractLastJumpFromBlock(currentCaseBlock, out var exitBlock))
				return false;
			if (values.Count == 0)
				return false;
			if (!values.All(b => ExtractLastJumpFromBlock(b.Item2, out var nextExit) && nextExit == exitBlock))
				return false;
			if (currentCaseBlock.IncomingEdgeCount == (isLegacy ? 2 : 1)) {
				var sections = new List<SwitchSection>(values.SelectWithIndex((index, b) => new SwitchSection { Labels = new LongSet(index), Body = new Branch(b.Item2) }));
				var stringToInt = new StringToInt(new LdLoc(switchValueVar), values.SelectArray(item => item.Item1));
				inst = new SwitchInstruction(stringToInt);
				inst.Sections.AddRange(sections);
				blockAfterSwitch = currentCaseBlock;
				return true;
			}
			return false;
		}

		bool ExtractLastJumpFromBlock(Block block, out Block exitBlock)
		{
			exitBlock = null;
			var lastInst = block.Instructions.LastOrDefault();
			if (lastInst == null || !lastInst.MatchBranch(out exitBlock))
				return false;
			return true;
		}

		Block MatchCaseBlock(Block currentBlock, ILVariable switchVariable, out string value, out Block caseBlock)
		{
			value = null;
			caseBlock = null;
			if (currentBlock.IncomingEdgeCount != 1 || currentBlock.Instructions.Count != 2)
				return null;
			if (!currentBlock.Instructions[0].MatchIfInstruction(out var condition, out var caseBlockBranch))
				return null;
			if (!caseBlockBranch.MatchBranch(out caseBlock))
				return null;
			if (!MatchStringEqualityComparison(condition, switchVariable, out value))
				return null;
			if (!currentBlock.Instructions[1].MatchBranch(out var nextBlock))
				return null;
			return nextBlock;
		}

		bool MatchLegacySwitchOnString(InstructionCollection<ILInstruction> instructions, int i, out SwitchInstruction inst, out Block blockAfterSwitch)
		{
			inst = null;
			blockAfterSwitch = null;
			if (i < 1) return false;
			// match first block: checking switch-value for null
			if (!(instructions[i].MatchIfInstruction(out var condition, out var exitBlockJump) &&
				instructions[i - 1].MatchStLoc(out var switchValueVar, out var switchValue) && switchValueVar.Type.IsKnownType(KnownTypeCode.String)))
				return false;
			if (!exitBlockJump.MatchBranch(out var exitBlock))
				return false;
			if (!(condition.MatchCompEquals(out var left, out var right) && right.MatchLdNull() && left.Match(switchValue).Success))
				return false;
			var nextBlockJump = instructions.ElementAtOrDefault(i + 1) as Branch;
			if (nextBlockJump == null || nextBlockJump.TargetBlock.IncomingEdgeCount != 1)
				return false;
			// match second block: checking compiler-generated Dictionary<string, int> for null
			var nextBlock = nextBlockJump.TargetBlock;
			if (nextBlock.Instructions.Count != 2 || !nextBlock.Instructions[0].MatchIfInstruction(out condition, out var tryGetValueBlockJump))
				return false;
			if (!tryGetValueBlockJump.MatchBranch(out var tryGetValueBlock))
				return false;
			if (!nextBlock.Instructions[1].MatchBranch(out var dictInitBlock) || dictInitBlock.IncomingEdgeCount != 1)
				return false;
			if (!(condition.MatchCompNotEquals(out left, out right) && right.MatchLdNull() &&
				MatchDictionaryFieldLoad(left, out var dictField, out var dictionaryType)))
				return false;
			// match third block: initialization of compiler-generated Dictionary<string, int>
			if (dictInitBlock.IncomingEdgeCount != 1 || dictInitBlock.Instructions.Count < 3)
				return false;
			if (!ExtractStringValuesFromDictionaryInitBlock(dictInitBlock, out var stringValues, tryGetValueBlock, dictionaryType, dictField))
				return false;
			// match fourth block: TryGetValue on compiler-generated Dictionary<string, int>
			if (tryGetValueBlock.IncomingEdgeCount != 2 || tryGetValueBlock.Instructions.Count != 2)
				return false;
			if (!tryGetValueBlock.Instructions[0].MatchIfInstruction(out condition, out var defaultBlockJump))
				return false;
			if (!defaultBlockJump.MatchBranch(out var defaultBlock))
				return false;
			if (!(condition.MatchLogicNot(out var arg) && arg is Call c && c.Method.Name == "TryGetValue" &&
				MatchDictionaryFieldLoad(c.Arguments[0], out var dictField2, out _) && dictField2.Equals(dictField)))
				return false;
			if (!c.Arguments[1].MatchLdLoc(switchValueVar) || !c.Arguments[2].MatchLdLoca(out var switchIndexVar))
				return false;
			if (!tryGetValueBlock.Instructions[1].MatchBranch(out var switchBlock))
				return false;
			// match fifth block: switch-instruction block
			if (switchBlock.IncomingEdgeCount != 1 || switchBlock.Instructions.Count != 2)
				return false;
			if (!(switchBlock.Instructions[0] is SwitchInstruction switchInst && switchInst.Value.MatchLdLoc(switchIndexVar)))
				return false;
			if (!switchBlock.Instructions[1].MatchBranch(defaultBlock))
				return false;
			// switch contains case null:
			var sections = new List<SwitchSection>(switchInst.Sections);
			if (exitBlock != defaultBlock) {
				stringValues.Add(null);
				sections.Add(new SwitchSection() { Labels = new Util.LongSet(stringValues.Count - 1), Body = new Branch(exitBlock) });
			}
			var stringToInt = new StringToInt(switchValue.Clone(), stringValues.ToArray());
			inst = new SwitchInstruction(stringToInt);
			inst.DefaultBody = switchInst.DefaultBody;
			inst.Sections.AddRange(sections);
			blockAfterSwitch = defaultBlock;
			return true;
		}

		bool MatchDictionaryFieldLoad(ILInstruction inst, out IField dictField, out IType dictionaryType)
		{
			dictField = null;
			dictionaryType = null;
			return inst.MatchLdObj(out var dictionaryFieldLoad, out dictionaryType) &&
				IsStringToIntDictionary(dictionaryType) &&
				dictionaryFieldLoad.MatchLdsFlda(out dictField) &&
				dictField.IsCompilerGeneratedOrIsInCompilerGeneratedClass();
		}

		bool ExtractStringValuesFromDictionaryInitBlock(Block block, out List<string> values, Block targetBlock, IType dictionaryType, IField dictionaryField)
		{
			values = null;
			if (!(block.Instructions[0].MatchStLoc(out var dictVar, out var newObjDict) &&
				newObjDict is NewObj newObj && newObj.Arguments.Count == 1 && newObj.Arguments[0].MatchLdcI4(out var valuesLength)))
				return false;
			if (block.Instructions.Count != valuesLength + 3)
				return false;
			values = new List<string>(valuesLength);
			for (int i = 0; i < valuesLength; i++) {
				if (!(block.Instructions[i + 1] is Call c && c.Method.Name == "Add" && c.Arguments.Count == 3 &&
					c.Arguments[0].MatchLdLoc(dictVar) && c.Arguments[1].MatchLdStr(out var value) && c.Arguments[2].MatchLdcI4(i)))
					return false;
				values.Add(value);
			}
			if (!(block.Instructions[valuesLength + 1].MatchStObj(out var loadField, out var dictVarLoad, out var dictType) &&
				dictType.Equals(dictionaryType) && loadField.MatchLdsFlda(out var dictField) && dictField.Equals(dictionaryField)) &&
				dictVarLoad.MatchLdLoc(dictVar))
				return false;
			return block.Instructions[valuesLength + 2].MatchBranch(targetBlock);
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

		bool MatchRoslynSwitchOnString(InstructionCollection<ILInstruction> instructions, int i, out SwitchInstruction inst)
		{
			inst = null;
			if (i < 1) return false;
			if (!(instructions[i] is SwitchInstruction switchInst && switchInst.Value.MatchLdLoc(out var targetVar) &&
				MatchComputeStringHashCall(instructions[i - 1], targetVar, out var switchValue)))
				return false;

			var stringValues = new List<(int, string, Block)>();
			int index = 0;
			foreach (var section in switchInst.Sections) {
				if (!section.Body.MatchBranch(out Block target))
					return false;
				if (target.IncomingEdgeCount != 1 || target.Instructions.Count == 0)
					return false;
				if (!target.Instructions[0].MatchIfInstruction(out var condition, out var bodyBranch))
					return false;
				if (!MatchStringEqualityComparison(condition, switchValue.Variable, out string stringValue))
					return false;
				if (!bodyBranch.MatchBranch(out Block body))
					return false;
				stringValues.Add((index++, stringValue, body));
			}

			var value = new StringToInt(switchValue.Clone(), stringValues.Select(item => item.Item2).ToArray());
			inst = new SwitchInstruction(value);
			inst.Sections.AddRange(stringValues.Select(section => new SwitchSection { Labels = new Util.LongSet(section.Item1), Body = new Branch(section.Item3) }));

			return true;
		}

		bool MatchComputeStringHashCall(ILInstruction inst, ILVariable targetVar, out LdLoc switchValue)
		{
			switchValue = null;
			if (!inst.MatchStLoc(targetVar, out var value))
				return false;
			if (!(value is Call c && c.Arguments.Count == 1 && c.Method.Name == "ComputeStringHash" && c.Method.IsCompilerGeneratedOrIsInCompilerGeneratedClass()))
				return false;
			if (!(c.Arguments[0] is LdLoc))
				return false;
			switchValue = (LdLoc)c.Arguments[0];
			return true;
		}

		bool MatchStringEqualityComparison(ILInstruction condition, ILVariable variable, out string stringValue)
		{
			stringValue = null;
			ILInstruction left, right;
			if (condition is Call c && c.Method.IsOperator && c.Method.Name == "op_Equality" && c.Arguments.Count == 2) {
				left = c.Arguments[0];
				right = c.Arguments[1];
				if (!right.MatchLdStr(out stringValue))
					return false;
			} else if (condition.MatchCompEquals(out left, out right) && right.MatchLdNull()) {
			} else return false;
			return left.MatchLdLoc(variable);
		}
	}
}
