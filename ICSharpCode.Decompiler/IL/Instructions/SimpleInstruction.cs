using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A simple instruction that does not have any arguments.
	/// </summary>
	abstract class SimpleInstruction(OpCode opCode) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return false; } }

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
		}
	}

	class Nop() : SimpleInstruction(OpCode.Nop)
	{
		public override bool NoResult { get { return true; } }
	}

	class Pop() : SimpleInstruction(OpCode.Pop)
	{
	}

	class Peek() : SimpleInstruction(OpCode.Peek)
	{
		public override bool IsPeeking { get { return true; } }
	}

	class Ckfinite() : SimpleInstruction(OpCode.Ckfinite)
	{
		public override bool IsPeeking { get { return true; } }
		public override bool NoResult { get { return true; } }
	}

	class Arglist() : SimpleInstruction(OpCode.Arglist)
	{
	}

	class DebugBreak() : SimpleInstruction(OpCode.DebugBreak)
	{
		public override bool NoResult { get { return true; } }
	}

	class ConstantString(public readonly string Value) : SimpleInstruction(OpCode.LdStr)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}

	class ConstantI4(public readonly int Value) : SimpleInstruction(OpCode.LdcI4)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}

	class ConstantI8(public readonly long Value) : SimpleInstruction(OpCode.LdcI8)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}

	class ConstantFloat(public readonly double Value) : SimpleInstruction(OpCode.LdcI8)
	{
		public override void WriteTo(ITextOutput output)
		{
			Disassembler.DisassemblerHelpers.WriteOperand(output, Value);
		}
	}

	class ConstantNull() : SimpleInstruction(OpCode.LdNull)
	{
	}
}
