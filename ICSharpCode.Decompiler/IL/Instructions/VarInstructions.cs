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
			output.WriteReference(Variable.ToString(), Variable, isLocal: true);
		}
	}

	class LdLoca(public readonly ILVariable Variable) : SimpleInstruction(OpCode.LdLoca)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.Write("ref ");
			output.WriteReference(Variable.ToString(), Variable, isLocal: true);
		}
	}

	class StLoc(public readonly ILVariable Variable) : UnaryInstruction(OpCode.StLoc)
	{
		public override void WriteTo(ITextOutput output)
		{
			output.WriteReference(Variable.ToString(), Variable, isLocal: true);
			output.Write(" = ");
			Operand.WriteTo(output);
		}
	}
}
