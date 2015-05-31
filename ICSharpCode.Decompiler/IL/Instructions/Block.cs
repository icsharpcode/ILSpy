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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

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
	///     var result = inst.Phase1().Phase2();
	///     if (result != void) evalStack.Push(result);
	///   }
	///   return FinalInstruction.Phase1().Phase2();
	/// </code>
	/// <para>
	/// Note: if execution reaches the end of the instruction list,
	/// the FinalInstruction (which is not part of the list) will be executed.
	/// The block returns returns the result value of the FinalInstruction.
	/// For blocks returning void, the FinalInstruction will usually be 'nop'.
	/// </para>
	/// </summary>
	/// <remarks>
	/// Fun fact: wrapping a pop instruction in a block
	/// (<c>new Block { FinalInstruction = popInst }</c>) turns it
	/// from a phase-1 pop instruction to a phase-2 pop instruction.
	/// However, this is just of theoretical interest; we currently don't plan to use inline blocks that
	/// pop elements that they didn't push themselves.
	/// </remarks>
	partial class Block : ILInstruction
	{
		public readonly InstructionCollection<ILInstruction> Instructions;
		ILInstruction finalInstruction;
		
		/// <summary>
		/// For blocks in a block container, this field holds
		/// the number of incoming control flow edges to this block.
		/// </summary>
		/// <remarks>
		/// This variable is automatically updated when adding/removing branch instructions from the ILAst,
		/// or when adding the block as an entry point to a BlockContainer.
		/// </remarks>
		public int IncomingEdgeCount { get; internal set; }

		public ILInstruction FinalInstruction {
			get {
				return finalInstruction;
			}
			set {
				ValidateChild(value);
				SetChildInstruction(ref finalInstruction, value, Instructions.Count);
			}
		}
		
		protected internal override void InstructionCollectionUpdateComplete()
		{
			base.InstructionCollectionUpdateComplete();
			if (finalInstruction.Parent == this)
				finalInstruction.ChildIndex = Instructions.Count;
		}
		
		public Block() : base(OpCode.Block)
		{
			this.Instructions = new InstructionCollection<ILInstruction>(this, 0);
			this.FinalInstruction = new Nop();
		}
		
		public override ILInstruction Clone()
		{
			Block clone = new Block();
			clone.ILRange = this.ILRange;
			clone.Instructions.AddRange(this.Instructions.Select(inst => inst.Clone()));
			clone.FinalInstruction = this.FinalInstruction.Clone();
			return clone;
		}
		
		internal override void CheckInvariant()
		{
			base.CheckInvariant();
			for (int i = 0; i < Instructions.Count - 1; i++) {
				// only the last instruction may have an unreachable endpoint
				Debug.Assert(!Instructions[i].HasFlag(InstructionFlags.EndPointUnreachable));
			}
		}
		
		public override StackType ResultType {
			get {
				return finalInstruction.ResultType;
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
			if (finalInstruction.OpCode != OpCode.Nop) {
				output.Write("final: ");
				finalInstruction.WriteTo(output);
				output.WriteLine();
			}
			output.Unindent();
			output.Write("}");
		}
		
		protected override int GetChildCount()
		{
			return Instructions.Count + 1;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			if (index == Instructions.Count)
				return finalInstruction;
			return Instructions[index];
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			if (index == Instructions.Count)
				FinalInstruction = value;
			else
				Instructions[index] = value;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.None;
			foreach (var inst in Instructions) {
				flags |= Phase1Boundary(inst.Flags);
				if (inst.ResultType != StackType.Void) {
					// implicit push
					flags |= InstructionFlags.MayWriteEvaluationStack;
				}
			}
			flags |= Phase1Boundary(FinalInstruction.Flags);
			return flags;
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
		
		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			for (int i = 0; i < Instructions.Count; i++) {
				var inst = Instructions[i].Inline(InstructionFlags.None, state);
				inst.TransformStackIntoVariables(state);
				if (inst.ResultType != StackType.Void) {
					var type = state.TypeSystem.Compilation.FindType(inst.ResultType.ToKnownTypeCode());
					ILVariable variable = new ILVariable(VariableKind.StackSlot, type, state.Variables.Count);
					state.Variables.Push(variable);
					inst = new Void(new StLoc(variable, inst));
				}
				Instructions[i] = inst;
				if (inst.HasFlag(InstructionFlags.EndPointUnreachable))
					return;
			}
			FinalInstruction = FinalInstruction.Inline(InstructionFlags.None, state);
			FinalInstruction.TransformStackIntoVariables(state);
		}
	}
}
