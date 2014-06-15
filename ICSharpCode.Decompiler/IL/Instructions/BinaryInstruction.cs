using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	abstract class BinaryInstruction(OpCode opCode) : ILInstruction(opCode)
	{
		public ILInstruction Left = Pop;
		public ILInstruction Right = Pop;

		public override bool IsPeeking { get { return Left.IsPeeking; } }
	}

	class BinaryNumericInstruction(OpCode opCode, StackType opType, OverflowMode overflowMode)
		: BinaryInstruction(opCode)
	{
		public readonly StackType OpType = opType;
		public readonly OverflowMode OverflowMode = overflowMode;
	}

	class BinaryComparisonInstruction(OpCode opCode, StackType opType)
		: BinaryInstruction(opCode)
	{
		public readonly StackType OpType = opType;
	}

	public enum OverflowMode : byte
	{
		/// <summary>Don't check for overflow, treat integers as signed.</summary>
		None = 0,
		/// <summary>Check for overflow, treat integers as signed.</summary>
		Ovf = 1,
		/// <summary>Don't check for overflow, treat integers as unsigned.</summary>
		Un = 2,
		/// <summary>Check for overflow, treat integers as unsigned.</summary>
		Ovf_Un = 3
	}
}
