using Mono.Cecil;
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

		public sealed override bool IsPeeking { get { return Operand.IsPeeking; } }

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			Operand = transformFunc(Operand);
		}
	}

	class VoidInstruction() : UnaryInstruction(OpCode.Void)
	{
		public override bool NoResult { get { return true; } }
	}

	class LogicNotInstruction() : UnaryInstruction(OpCode.LogicNot)
	{
	}

	class UnaryNumericInstruction(OpCode opCode, StackType opType) : UnaryInstruction(opCode)
	{
		public readonly StackType OpType = opType;

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			output.Write(OpType);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}
	}

	class IsInst(public readonly TypeReference Type) : UnaryInstruction(OpCode.IsInst)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Type);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}
	}

	class ConvInstruction(
		public readonly StackType FromType, public readonly PrimitiveType ToType, public readonly OverflowMode ConvMode
	) : UnaryInstruction(OpCode.Conv)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.WriteSuffix(ConvMode);
			output.Write(' ');
			output.Write(FromType);
			output.Write("->");
			output.Write(ToType);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}
	}
}
 