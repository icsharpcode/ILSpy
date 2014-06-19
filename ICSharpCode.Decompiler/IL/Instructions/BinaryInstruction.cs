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

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			Left = transformFunc(Left);
			Right = transformFunc(Right);
		}
	}

	class BinaryNumericInstruction(OpCode opCode, StackType opType, OverflowMode overflowMode)
		: BinaryInstruction(opCode)
	{
		public readonly StackType OpType = opType;
		public readonly OverflowMode OverflowMode = overflowMode;

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.WriteSuffix(OverflowMode);
			output.Write(' ');
			output.Write(OpType);
			output.Write('(');
			Left.WriteTo(output);
			output.Write(", ");
			Right.WriteTo(output);
			output.Write(')');
		}
	}

	class BinaryComparisonInstruction(OpCode opCode, StackType opType)
		: BinaryInstruction(opCode)
	{
		public readonly StackType OpType = opType;

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('.');
			output.Write(OpType);
			output.Write('(');
			Left.WriteTo(output);
			output.Write(", ");
			Right.WriteTo(output);
			output.Write(')');
		}
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
