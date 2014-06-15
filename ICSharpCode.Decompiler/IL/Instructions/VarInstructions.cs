using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL.Instructions
{
	class LoadVarInstruction(public readonly ILVariable Variable) : ILInstruction(OpCode.LoadVar)
	{
		public override bool IsPeeking { get { return false; } }
	}
}
