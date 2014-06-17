using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class LoadStaticField(FieldReference field, OpCode opCode = OpCode.Ldsfld) : UnaryInstruction(opCode), ISupportsVolatilePrefix
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
	}

	class StoreInstanceField(FieldReference field, OpCode opCode = OpCode.Ldfld) : BinaryInstruction(opCode), ISupportsVolatilePrefix
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
	}
}
