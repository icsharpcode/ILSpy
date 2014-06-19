using System;

namespace ICSharpCode.Decompiler.IL
{
	internal class SemanticHelper
	{
		/// <summary>
		/// Gets whether the instruction sequence 'inst1; inst2;' may be ordered to 'inst2; inst1;'
		/// </summary>
		internal static bool MayReorder(InstructionFlags inst1, InstructionFlags inst2)
		{
			// If both instructions perform a non-read-only action, we cannot reorder them
			if ((inst1 & inst2 & ~(InstructionFlags.MayPeek | InstructionFlags.MayReadLocals)) != 0)
				return false;
			// We cannot reorder if inst2 might pop what inst1 peeks at
			if (ConflictingPair(inst1, inst2, InstructionFlags.MayPeek, InstructionFlags.MayPop))
				return false;
			if (ConflictingPair(inst1, inst2, InstructionFlags.MayReadLocals, InstructionFlags.MayWriteLocals))
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