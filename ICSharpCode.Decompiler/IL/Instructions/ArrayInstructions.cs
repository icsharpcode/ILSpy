using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class LdLen() : UnaryInstruction(OpCode.LdLen)
	{
		public override InstructionFlags Flags
		{
			get { return InstructionFlags.MayThrow | Operand.Flags; }
		}
	}
}
