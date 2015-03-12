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

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A block consists of a list of IL instructions.
	/// <para>
	/// Phase-1 execution of a block is a no-op: any peek/pop instructions within the block are ignored at this stage.
	/// </para>
	/// <para>
	/// Phase-2 execution will execute the instructions in order, pseudo-code:
	/// </para>
	/// <code>
	///   foreach (var inst in Instructions) {
	///     inst.Phase1();
	///     var result = inst.Phase2();
	///     if (result != void) evalStack.Push(result);
	///   }
	/// </code>
	/// <para>
	/// If the execution reaches the end of the block, the block returns the result value of the last instruction.
	/// </para>
	/// TODO: actually I think it's a good idea to implement a small ILAst interpreter
	///    public virtual ILInstruction Phase1(InterpreterState state);
	///    public virtual InterpreterResult ILInstruction Phase2(InterpreterState state);
	/// It's probably the easiest solution for specifying clear semantics, and
	/// we might be able to use the interpreter logic for symbolic execution.
	/// In fact, I think we'll need at least Phase1() in order to implement TransformStackIntoVariablesState()
	/// without being incorrect about the pop-order in nested blocks.
	/// </summary>
	/// <remarks>
	/// Fun fact: the empty block acts like a phase-2 pop instruction,
	/// which is a slightly different behavior than the normal phase-1 <see cref="Pop"/> instruction!
	/// However, this is just of theoretical interest; we currently don't plan to use inline blocks that
	/// pop elements that they didn't push themselves.
	/// </remarks>
	partial class Block : ILInstruction, IEnumerable<ILInstruction>
	{
		public readonly InstructionCollection<ILInstruction> Instructions;
		
		public int IncomingEdgeCount;
		
		public Block() : base(OpCode.Block)
		{
			this.Instructions = new InstructionCollection<ILInstruction>(this);
		}
		
		public override StackType ResultType {
			get {
				if (Instructions.Count == 0)
					return StackType.Void;
				else
					return Instructions.Last().ResultType;
			}
		}
		
		/// <summary>
		/// Gets the name of this block.
		/// </summary>
		public string Label
		{
			get { return Disassembler.DisassemblerHelpers.OffsetToString(this.ILRange.Start); }
		}

		public override void WriteTo(ITextOutput output)
		{
			output.Write("Block ");
			output.WriteDefinition(Label, this);
			if (Parent is BlockContainer)
				output.Write(" (incoming: {0})", IncomingEdgeCount);
			output.WriteLine(" {");
			output.Indent();
			foreach (var inst in Instructions) {
				inst.WriteTo(output);
				output.WriteLine();
			}
			output.Unindent();
			output.Write("}");
		}
		
		public override IEnumerable<ILInstruction> Children {
			get { return Instructions; }
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			for (int i = 0; i < Instructions.Count; i++) {
				Instructions[i] = Instructions[i].AcceptVisitor(visitor);
			}
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.None;
			foreach (var inst in Instructions) {
				flags |= inst.Flags;
				if (inst.ResultType != StackType.Void) {
					// implicit push
					flags |= InstructionFlags.MayWriteEvaluationStack;
				}
			}
			// implicit pop at end of block
			if ((flags & InstructionFlags.EndPointUnreachable) == 0)
				flags |= InstructionFlags.MayWriteEvaluationStack;
			return Phase1Boundary(flags);
		}
		
		/// <summary>
		/// Adjust flags for a phase-1 boundary:
		/// The MayPop and MayPeek flags are removed and converted into
		/// MayReadEvaluationStack and/or MayWriteEvaluationStack flags.
		/// </summary>
		internal static InstructionFlags Phase1Boundary(InstructionFlags flags)
		{
			// Convert phase-1 flags to phase-2 flags
			if ((flags & InstructionFlags.MayPop) != 0)
				flags |= InstructionFlags.MayWriteEvaluationStack;
			if ((flags & (InstructionFlags.MayPeek | InstructionFlags.MayPop)) != 0)
				flags |= InstructionFlags.MayReadEvaluationStack;
			// an inline block has no phase-1 effects
			flags &= ~(InstructionFlags.MayPeek | InstructionFlags.MayPop);
			return flags;
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			// an inline block has no phase-1 effects, so we're immediately done with inlining
			return this;
		}
		
		// Add() and GetEnumerator() for collection initializer support:
		public void Add(ILInstruction instruction)
		{
			this.Instructions.Add(instruction);
		}
		
		IEnumerator<ILInstruction> IEnumerable<ILInstruction>.GetEnumerator()
		{
			return this.Instructions.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Instructions.GetEnumerator();
		}
	}
}
