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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class SwitchOnStringTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			HashSet<BlockContainer> changedContainers = new HashSet<BlockContainer>();

			foreach (var block in function.Descendants.OfType<Block>()) {
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					if (!MatchRoslynSwitchOnString(block.Instructions, i, out var newSwitch))
						continue;

					block.Instructions[i].ReplaceWith(newSwitch);
					block.Instructions.RemoveAt(i - 1);

					i--;

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
