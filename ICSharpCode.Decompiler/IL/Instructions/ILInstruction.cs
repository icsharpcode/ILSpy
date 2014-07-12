// Copyright (c) 2014 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.CSharp;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Represents a decoded IL instruction
	/// </summary>
	public abstract class ILInstruction
	{
		public readonly OpCode OpCode;
		
		protected ILInstruction(OpCode opCode)
		{
			this.OpCode = opCode;
		}
		
		internal static void ValidateArgument(ILInstruction inst)
		{
			if (inst == null)
				throw new ArgumentNullException("inst");
			if (inst.ResultType == StackType.Void)
				throw new ArgumentException("Argument must not be of type void", "inst");
		}
		
		[Conditional("DEBUG")]
		internal void CheckInvariant()
		{
		}
		
		/// <summary>
		/// Gets the stack type of the value produced by this instruction.
		/// </summary>
		public abstract StackType ResultType { get; }
		
		InstructionFlags flags = (InstructionFlags)(-1);
		
		public InstructionFlags Flags {
			get {
				if (flags == (InstructionFlags)(-1)) {
					flags = ComputeFlags();
				}
				return flags;
			}
		}

		/// <summary>
		/// Returns whether the instruction has at least one of the specified flags.
		/// </summary>
		public bool HasFlag(InstructionFlags flags)
		{
			return (this.Flags & flags) != 0;
		}
		
		protected void SetChildInstruction(ref ILInstruction childPointer, ILInstruction newValue)
		{
			childPointer = newValue;
			flags = (InstructionFlags)(-1);
		}
		
		protected internal void AddChildInstruction(ILInstruction newChild)
		{
			flags = (InstructionFlags)(-1);
		}
		
		protected internal void RemoveChildInstruction(ILInstruction newChild)
		{
			flags = (InstructionFlags)(-1);
		}
		
		protected abstract InstructionFlags ComputeFlags();
		
		/// <summary>
		/// Gets the ILRange for this instruction alone, ignoring the operands.
		/// </summary>
		public Interval ILRange;

		/// <summary>
		/// Writes the ILAst to the text output.
		/// </summary>
		public abstract void WriteTo(ITextOutput output);

		public override string ToString()
		{
			var output = new PlainTextOutput();
			WriteTo(output);
			return output.ToString();
		}
		
		/// <summary>
		/// Calls the Visit*-method on the visitor corresponding to the concrete type of this instruction.
		/// </summary>
		public abstract T AcceptVisitor<T>(ILVisitor<T> visitor);
		
		/// <summary>
		/// Computes an aggregate value over the direct children of this instruction.
		/// </summary>
		/// <param name="initial">The initial value used to initialize the accumulator.</param>
		/// <param name="visitor">The visitor used to compute the value for the child instructions.</param>
		/// <param name="func">The function that combines the accumulator with the computed child value.</param>
		/// <returns>The final value in the accumulator.</returns>
		public abstract TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func);
		
		/// <summary>
		/// Transforms the children of this instruction by applying the specified visitor.
		/// </summary>
		public abstract void TransformChildren(ILVisitor<ILInstruction> visitor);
		
		/// <summary>
		/// Attempts inlining from the instruction stack into this instruction.
		/// </summary>
		/// <param name="flagsBefore">Combined instruction flags of the instructions
		/// that the instructions getting inlined would get moved over.</param>
		/// <param name="instructionStack">The instruction stack.</param>
		/// <param name="finished">Receives 'true' if all open 'pop' or 'peek' placeholders were inlined into; false otherwise.</param>
		internal abstract ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished);
	}
}
