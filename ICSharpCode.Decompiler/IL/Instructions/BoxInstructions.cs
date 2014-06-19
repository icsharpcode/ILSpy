using ICSharpCode.Decompiler.Disassembler;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class UnboxAny(public readonly TypeReference TypeReference) : UnaryInstruction(OpCode.UnboxAny)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			TypeReference.WriteTo(output);
            output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}

		public override InstructionFlags Flags
		{
			get { return Operand.Flags | InstructionFlags.MayThrow | InstructionFlags.SideEffects; }
		}
	}
}
