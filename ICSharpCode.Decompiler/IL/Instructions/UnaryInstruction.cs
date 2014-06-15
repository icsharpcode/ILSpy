using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	abstract class UnaryInstruction(OpCode opCode) : ILInstruction(opCode)
	{
		public ILInstruction Operand = Pop;

		public override bool IsPeeking { get { return Operand.IsPeeking; } }
	}

	class LogicNotInstruction() : UnaryInstruction(OpCode.LogicNot)
	{
	}

	class ConvInstruction(
		public readonly StackType FromType, public readonly PrimitiveType ToType, public readonly OverflowMode ConvMode
	) : UnaryInstruction(OpCode.Conv)
	{
	}
}
