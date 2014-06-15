using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	class LoadVarInstruction(public readonly ILVariable Variable, OpCode opCode = OpCode.LoadVar) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return false; } }

		public override void WriteTo(ITextOutput output)
		{
			if (OpCode != OpCode.LoadVar) {
				output.Write(OpCode);
				output.Write(' ');
			}
			output.WriteReference(Variable.ToString(), Variable, isLocal: true);
		}
	}

	class StoreVarInstruction(public readonly ILVariable Variable) : UnaryInstruction(OpCode.StoreVar)
	{
		public override bool IsPeeking { get { return false; } }

		public override void WriteTo(ITextOutput output)
		{
			output.WriteReference(Variable.ToString(), Variable, isLocal: true);
			output.Write(" = ");
			Operand.WriteTo(output);
		}
	}
}
