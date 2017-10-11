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
using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Detects switch-on-nullable patterns employed by the C# compiler and transforms them to an ILAst-switch-instruction.
	/// </summary>
	class SwitchOnNullableTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			if (!context.Settings.LiftNullables)
				return;

			HashSet<BlockContainer> changedContainers = new HashSet<BlockContainer>();

			foreach (var block in function.Descendants.OfType<Block>()) {
				bool changed = false;
				for (int i = block.Instructions.Count - 1; i >= 0; i--) {
					SwitchInstruction newSwitch;
					if (MatchSwitchOnNullable(block.Instructions, i, out newSwitch)) {
						block.Instructions[i + 1].ReplaceWith(newSwitch);
						block.Instructions.RemoveRange(i - 2, 3);
						i -= 2;
						changed = true;
						continue;
					}
					if (MatchRoslynSwitchOnNullable(block.Instructions, i, out newSwitch)) {
						block.Instructions[i - 1].ReplaceWith(newSwitch);
						block.Instructions.RemoveRange(i, 2);
						i--;
						changed = true;
						continue;
					}
				}
				if (!changed) continue;
				SwitchDetection.SimplifySwitchInstruction(block);
				if (block.Parent is BlockContainer container)
					changedContainers.Add(container);
			}

			foreach (var container in changedContainers)
				container.SortBlocks(deleteUnreachableBlocks: true);
		}

		/// <summary>
		/// Matches legacy C# switch on nullable.
		/// </summary>
		bool MatchSwitchOnNullable(InstructionCollection<ILInstruction> instructions, int i, out SwitchInstruction newSwitch)
		{
			newSwitch = null;
			// match first block:
			// stloc tmp(ldloca switchValueVar)
			// stloc switchVariable(call GetValueOrDefault(ldloc tmp))
			// if (logic.not(call get_HasValue(ldloc tmp))) br nullCaseBlock
			// br switchBlock
			if (i < 2) return false;
			if (!instructions[i - 2].MatchStLoc(out var tmp, out var ldloca) ||
				!instructions[i - 1].MatchStLoc(out var switchVariable, out var getValueOrDefault) ||
				!instructions[i].MatchIfInstruction(out var condition, out var trueInst))
				return false;
			if (!instructions[i + 1].MatchBranch(out var switchBlock) || !trueInst.MatchBranch(out var nullCaseBlock))
				return false;
			if (!ldloca.MatchLdLoca(out var switchValueVar))
				return false;
			if (!condition.MatchLogicNot(out var getHasValue))
				return false;
			if (!(getValueOrDefault is Call getValueOrDefaultCall) || getValueOrDefaultCall.Method.FullName != "System.Nullable.GetValueOrDefault" ||
				getValueOrDefaultCall.Method.DeclaringType.TypeParameterCount != 1)
				return false;
			if (!(getHasValue is Call getHasValueCall) || !getHasValueCall.Method.IsAccessor || getHasValueCall.Method.FullName != "System.Nullable.get_HasValue" ||
				getHasValueCall.Method.DeclaringType.TypeParameterCount != 1)
				return false;
			if (getHasValueCall.Arguments.Count != 1 || getValueOrDefaultCall.Arguments.Count != 1)
				return false;
			if (!getHasValueCall.Arguments[0].MatchLdLoc(tmp) || !getValueOrDefaultCall.Arguments[0].MatchLdLoc(tmp))
				return false;
			// match second block: switchBlock
			// switch (ldloc swtichVariable) {
			// 	case [0..1): br caseBlock1
			//  ... more cases ...
			// 	case [long.MinValue..0),[1..5),[6..10),[11..long.MaxValue]: br defaultBlock
			// }
			if (switchBlock.Instructions.Count != 1 || switchBlock.IncomingEdgeCount != 1)
				return false;
			if (!(switchBlock.Instructions[0] is SwitchInstruction switchInst))
				return false;
			newSwitch = new SwitchInstruction(new LdLoc(switchValueVar));
			newSwitch.IsLifted = true;
			SwitchSection defaultSection = null;
			foreach (var section in switchInst.Sections) {
				if (defaultSection == null || section.Labels.Count() >= defaultSection.Labels.Count())
					defaultSection = section;
				newSwitch.Sections.Add(section);
			}
			if (defaultSection.Body.MatchBranch(out var defaultBlock) && defaultBlock == nullCaseBlock)
				defaultSection.HasNullLabel = true;
			else {
				newSwitch.Sections.Add(new SwitchSection { Body = new Branch(nullCaseBlock), HasNullLabel = true });
			}
			return true;
		}

		/// <summary>
		/// Matches Roslyn C# switch on nullable.
		/// </summary>
		bool MatchRoslynSwitchOnNullable(InstructionCollection<ILInstruction> instructions, int i, out SwitchInstruction newSwitch)
		{
			newSwitch = null;
			// match first block:
			// stloc tmp(ldloc switchValueVar)
			// if (logic.not(call get_HasValue(ldloca tmp))) br nullCaseBlock
			// br switchBlock
			if (i < 1) return false;
			if (!instructions[i - 1].MatchStLoc(out var tmp, out var switchValue) ||
				!instructions[i].MatchIfInstruction(out var condition, out var trueInst))
				return false;
			if (!instructions[i + 1].MatchBranch(out var switchBlock) || !trueInst.MatchBranch(out var nullCaseBlock))
				return false;
			if (!condition.MatchLogicNot(out var getHasValue) || !NullableLiftingTransform.MatchHasValueCall(getHasValue, out var target1) || target1 != tmp)
				return false;
			// match second block: switchBlock
			// stloc switchVar(call GetValueOrDefault(ldloca tmp))
			// switch (ldloc switchVar) {
			// 	case [0..1): br caseBlock1
			// ... more cases ...
			// 	case [long.MinValue..0),[1..5),[6..10),[11..long.MaxValue]: br defaultBlock
			// }
			if (switchBlock.Instructions.Count != 2 || switchBlock.IncomingEdgeCount != 1)
				return false;
			if (!switchBlock.Instructions[0].MatchStLoc(out var switchVar, out var getValueOrDefault))
				return false;
			if (!NullableLiftingTransform.MatchGetValueOrDefault(getValueOrDefault, out var target2) || target2 != tmp)
				return false;
			if (!(switchBlock.Instructions[1] is SwitchInstruction switchInst))
				return false;
			newSwitch = new SwitchInstruction(switchValue);
			newSwitch.IsLifted = true;
			SwitchSection defaultSection = null;
			foreach (var section in switchInst.Sections) {
				if (defaultSection == null || section.Labels.Count() >= defaultSection.Labels.Count())
					defaultSection = section;
				newSwitch.Sections.Add(section);
			}
			if (defaultSection.Body.MatchBranch(out var defaultBlock) && defaultBlock == nullCaseBlock)
				defaultSection.HasNullLabel = true;
			else {
				newSwitch.Sections.Add(new SwitchSection { Body = new Branch(nullCaseBlock), HasNullLabel = true });
			}
			return true;
		}

	}
}
