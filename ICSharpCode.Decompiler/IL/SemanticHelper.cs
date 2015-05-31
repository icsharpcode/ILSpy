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

using System;

namespace ICSharpCode.Decompiler.IL
{
	static class SemanticHelper
	{
		// TODO: consider moving IfInstruction.CombineFlags and Block.Phase1Boundary here
		
		/// <summary>
		/// Gets whether instruction is pure:
		/// * must not have side effects
		/// * must not throw exceptions
		/// * must not branch
		/// </summary>
		internal static bool IsPure(InstructionFlags inst)
		{
			const InstructionFlags pureFlags = InstructionFlags.MayReadLocals;
			return (inst & ~pureFlags) == 0;
		}
		
		/// <summary>
		/// Gets whether the instruction sequence 'inst1; inst2;' may be ordered to 'inst2; inst1;'
		/// </summary>
		internal static bool MayReorder(InstructionFlags inst1, InstructionFlags inst2)
		{
			// If both instructions perform an impure action, we cannot reorder them
			if (!IsPure(inst1) && !IsPure(inst2))
				return false;
			// We cannot reorder if inst2 might write what inst1 looks at
			if (ConflictingPair(inst1, inst2, InstructionFlags.MayReadLocals, InstructionFlags.MayWriteLocals | InstructionFlags.SideEffect))
				return false;
			return true;
		}

		private static bool ConflictingPair(InstructionFlags inst1, InstructionFlags inst2, InstructionFlags readFlag, InstructionFlags writeFlag)
		{
			// if one instruction has the read flag and the other the write flag, that's a conflict
			return (inst1 & readFlag) != 0 && (inst2 & writeFlag) != 0
				|| (inst2 & readFlag) != 0 && (inst1 & writeFlag) != 0;
		}
	}
}