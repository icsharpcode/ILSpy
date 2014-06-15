using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Represents a decoded IL instruction
	/// </summary>
	public abstract class ILInstruction(public readonly OpCode OpCode)
	{
		public static readonly ILInstruction Nop = new SimpleInstruction(OpCode.Nop);
		public static readonly ILInstruction Pop = new SimpleInstruction(OpCode.Pop);

		/// <summary>
		/// Gets the ILRange for this instruction alone, ignoring the operands.
		/// </summary>
		public Interval ILRange;

		/// <summary>
		/// Gets whether this instruction peeks at the top value of the stack.
		/// If this instruction also pops elements from the stack, this property refers to the top value
		/// left after the pop operations.
		/// </summary>
		public abstract bool IsPeeking { get; }

		public abstract void WriteTo(ITextOutput output);
	}
}
