using ICSharpCode.Decompiler.Disassembler;
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

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			Operand = Operand.Inline(flagsBefore, instructionStack, out finished);
			return this;
		}
	}

	class VoidInstruction() : UnaryInstruction(OpCode.Void)
	{
		public override bool NoResult { get { return true; } }

		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
		}
	}

	class LogicNotInstruction() : UnaryInstruction(OpCode.LogicNot)
	{
		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
		}
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

		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
		}
	}

	class IsInst(public readonly TypeReference Type) : UnaryInstruction(OpCode.IsInst)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			Type.WriteTo(output);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return Operand.Flags; }
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

		public override InstructionFlags Flags
		{
			get { return Operand.Flags | InstructionFlags.MayThrow; }
		}
	}
}
 