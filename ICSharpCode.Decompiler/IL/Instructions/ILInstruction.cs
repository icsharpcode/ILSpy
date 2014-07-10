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
	public abstract class ILInstruction(OpCode opCode)
	{
		public static readonly ILInstruction Pop = new Pop();

		public readonly OpCode OpCode = opCode;

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

		/// <summary>
		/// Gets whether the instruction produces no result.
		/// Instructions without result may not be used as arguments to other instructions;
		/// and do not result in a stack push when used as a top-level instruction within a block.
		/// </summary>
		public virtual bool NoResult
		{
			get { return false; }
		}

		/// <summary>
		/// Gets whether the end point of this instruction is reachable from the start point.
		/// Returns false if the instruction performs an unconditional branch, or always throws an exception.
		/// </summary>
		public virtual bool IsEndReachable
		{
			get { return true; }
		}

		public abstract InstructionFlags Flags { get; }

		public virtual void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
		}

		/// <summary>
		/// Attempts inlining from the instruction stack into this instruction.
		/// </summary>
		/// <param name="flagsBefore">Combined instruction flags of the instructions
		/// that the instructions getting inlined would get moved over.</param>
		/// <param name="instructionStack">The instruction stack.</param>
		/// <param name="finished">Receives 'true' if all open 'pop' or 'peek' placeholders were inlined into; false otherwise.</param>
		internal abstract ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished);

		public abstract void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc);
	}
}
