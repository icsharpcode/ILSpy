using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A simple instruction that does not pop any elements from the stack.
	/// </summary>
	class SimpleInstruction(OpCode opCode) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return false; } }

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
		}
	}

	class PeekInstruction(OpCode opCode = OpCode.Peek) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return true; } }

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
		}
	}

	class ConstantStringInstruction(public readonly string Value) : SimpleInstruction(OpCode.LdStr)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}

	class ConstantI4Instruction(public readonly int Value) : SimpleInstruction(OpCode.LdcI4)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}

	class ConstantI8Instruction(public readonly long Value) : SimpleInstruction(OpCode.LdcI8)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}

	class ConstantFloatInstruction(public readonly double Value) : SimpleInstruction(OpCode.LdcI8)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}
}
