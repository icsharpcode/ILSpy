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
			Disassembler.DisassemblerHelpers.WriteOperand(output, TypeReference);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}
	}
}
