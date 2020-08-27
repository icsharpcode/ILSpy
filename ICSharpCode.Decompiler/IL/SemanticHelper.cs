// Copyright (c) 2014 Daniel Grunwald
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

using System.Linq;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	static class SemanticHelper
	{
		internal static InstructionFlags CombineBranches(InstructionFlags trueFlags, InstructionFlags falseFlags)
		{
			// the endpoint of the 'if' is only unreachable if both branches have an unreachable endpoint
			const InstructionFlags combineWithAnd = InstructionFlags.EndPointUnreachable;
			return (trueFlags & falseFlags) | ((trueFlags | falseFlags) & ~combineWithAnd);
		}

		/// <summary>
		/// Gets whether instruction is pure:
		/// * must not have side effects
		/// * must not throw exceptions
		/// * must not branch
		/// </summary>
		internal static bool IsPure(InstructionFlags inst)
		{
			// ControlFlow is fine: internal control flow is pure as long as it's not an infinite loop,
			// and infinite loops are impossible without MayBranch.
			const InstructionFlags pureFlags = InstructionFlags.MayReadLocals | InstructionFlags.ControlFlow;
			return (inst & ~pureFlags) == 0;
		}

		/// <summary>
		/// Gets whether the instruction sequence 'inst1; inst2;' may be ordered to 'inst2; inst1;'
		/// </summary>
		internal static bool MayReorder(ILInstruction inst1, ILInstruction inst2)
		{
			// If both instructions perform an impure action, we cannot reorder them
			if (!IsPure(inst1.Flags) && !IsPure(inst2.Flags))
				return false;
			// We cannot reorder if inst2 might write what inst1 looks at
			if (Inst2MightWriteToVariableReadByInst1(inst1, inst2))
				return false;
			// and the same in reverse:
			if (Inst2MightWriteToVariableReadByInst1(inst2, inst1))
				return false;
			return true;
		}

		static bool Inst2MightWriteToVariableReadByInst1(ILInstruction inst1, ILInstruction inst2)
		{
			if (!inst1.HasFlag(InstructionFlags.MayReadLocals))
			{
				// quick exit if inst1 doesn't read any variables
				return false;
			}
			var variables = inst1.Descendants.OfType<LdLoc>().Select(load => load.Variable).ToHashSet();
			if (inst2.HasFlag(InstructionFlags.SideEffect) && variables.Any(v => v.AddressCount > 0))
			{
				// If inst2 might have indirect writes, we cannot reorder with any loads of variables that have their address taken.
				return true;
			}
			foreach (var inst in inst2.Descendants)
			{
				if (inst.HasDirectFlag(InstructionFlags.MayWriteLocals))
				{
					ILVariable v = ((IInstructionWithVariableOperand)inst).Variable;
					if (variables.Contains(v))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}