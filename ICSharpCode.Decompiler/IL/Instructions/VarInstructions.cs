using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class LdLoc(public readonly ILVariable Variable) : SimpleInstruction(OpCode.LdLoc)
	{
		public override void WriteTo(ITextOutput output)
		{
			Variable.WriteTo(output);
		}

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.MayReadLocals; }
		}
	}

	class LdLoca(public readonly ILVariable Variable) : SimpleInstruction(OpCode.LdLoca)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write("ldloca ");
			Variable.WriteTo(output);
		}

		public override InstructionFlags Flags
		{
			get {
				// the address of a local can be considered to be a constant
				return InstructionFlags.None;
			}
		}
	}

	class StLoc(public readonly ILVariable Variable) : UnaryInstruction(OpCode.StLoc)
	{
		public override void WriteTo(ITextOutput output)
		{
			Variable.WriteTo(output);
			output.Write(" = ");
			Operand.WriteTo(output);
		}

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.MayWriteLocals | Operand.Flags; }
		}
	}
}
