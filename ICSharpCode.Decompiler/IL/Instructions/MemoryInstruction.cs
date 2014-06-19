using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	interface ISupportsMemoryPrefix : ISupportsVolatilePrefix
	{
		/// <summary>
		/// Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.
		/// </summary>
		byte UnalignedPrefix { get; set; }
	}

	interface ISupportsVolatilePrefix
	{
		/// <summary>
		/// Gets/Sets whether the memory access is volatile.
		/// </summary>
		bool IsVolatile { get; set; }
	}

	class LoadIndirect(public readonly TypeReference TypeReference) : UnaryInstruction(OpCode.LdInd), ISupportsMemoryPrefix
	{
		public byte UnalignedPrefix { get; set; }
		public bool IsVolatile { get; set; }

		public override void WriteTo(ITextOutput output)
		{
			if (IsVolatile)
				output.Write("volatile.");
			if (UnalignedPrefix != 0)
				output.Write("unaligned " + UnalignedPrefix + ".");
			output.Write(OpCode);
			output.Write(' ');
			Disassembler.DisassemblerHelpers.WriteOperand(output, TypeReference);
			output.Write('(');
			Operand.WriteTo(output);
			output.Write(')');
		}
	}
}
