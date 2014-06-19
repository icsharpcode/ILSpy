using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class LoadStaticField(FieldReference field, OpCode opCode = OpCode.Ldsfld) : SimpleInstruction(opCode), ISupportsVolatilePrefix
	{
		public readonly FieldReference Field = field;

		public bool IsVolatile { get; set; }

		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Field);
		}

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.SideEffects; }
		}
	}

	class StoreStaticField(FieldReference field) : UnaryInstruction(OpCode.Stsfld), ISupportsVolatilePrefix
	{
		public readonly FieldReference Field = field;

		public bool IsVolatile { get; set; }

		public override bool NoResult { get { return true; } }

		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, Field);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.SideEffects | Operand.Flags; }
		}
	}

	class LoadInstanceField(FieldReference field, OpCode opCode = OpCode.Ldfld) : UnaryInstruction(opCode), ISupportsVolatilePrefix
	{
		public readonly FieldReference Field = field;

		public bool IsVolatile { get; set; }

		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			output.Write(OpCode);
			output.Write(' ');
			output.WriteReference(Field.Name, Field);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.SideEffects | InstructionFlags.MayThrow | Operand.Flags; }
		}
	}

	class StoreInstanceField(FieldReference field, OpCode opCode = OpCode.Stfld) : BinaryInstruction(opCode), ISupportsVolatilePrefix
	{
		public readonly FieldReference Field = field;

		public bool IsVolatile { get; set; }

		public override bool NoResult { get { return true; } }

		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			output.Write(OpCode);
			output.Write(' ');
			output.WriteReference(Field.Name, Field);
			output.Write('(');
			Left.WriteTo(output);
			output.Write(", ");
			Right.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.SideEffects | InstructionFlags.MayThrow | Left.Flags | Right.Flags; }
		}
	}
}
