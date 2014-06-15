using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A simple instruction that does not pop any elements from the stack.
	/// </summary>
	class SimpleInstruction(OpCode opCode) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return false; } }
	}

	class PeekInstruction(OpCode opCode = OpCode.Peek) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return true; } }
	}
}
