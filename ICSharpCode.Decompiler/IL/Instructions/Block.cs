// Copyright (c) 2014-2016 Daniel Grunwald
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

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A block consists of a list of IL instructions.
	/// 
	/// <para>
	/// Note: if execution reaches the end of the instruction list,
	/// the FinalInstruction (which is not part of the list) will be executed.
	/// The block returns returns the result value of the FinalInstruction.
	/// For blocks returning void, the FinalInstruction will usually be 'nop'.
	/// </para>
	/// 
	/// There are three different uses for blocks:
	/// 1) Blocks in block containers. Used as targets for Branch instructions.
	/// 2) Blocks to group a bunch of statements, e.g. the TrueInst of an IfInstruction.
	/// 3) Inline blocks that evaluate to a value, e.g. for array initializers.
	/// 
	/// TODO: consider splitting inline blocks (with FinalInstruction) from those
	/// used in containers for control flow purposes -- these are very different things
	/// which should not share a class.
	/// </summary>
	partial class Block : ILInstruction
	{
		public static readonly SlotInfo InstructionSlot = new SlotInfo("Instruction", isCollection: true);
		public static readonly SlotInfo FinalInstructionSlot = new SlotInfo("FinalInstruction");
		
		public readonly BlockType Type;
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

		/// <summary>
		/// A 'final instruction' that gets executed after the <c>Instructions</c> collection.
		/// Provides the return value for the block.
		/// </summary>
		/// <remarks>
		/// Blocks in containers must have 'Nop' as a final instruction.
		/// </remarks>
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
		
		public Block(BlockType type = BlockType.ControlFlow) : base(OpCode.Block)
		{
			this.Type = type;
			this.Instructions = new InstructionCollection<ILInstruction>(this, 0);
			this.FinalInstruction = new Nop();
		}
		
		public override ILInstruction Clone()
		{
			Block clone = new Block(Type);
			clone.ILRange = this.ILRange;
			clone.Instructions.AddRange(this.Instructions.Select(inst => inst.Clone()));
			clone.FinalInstruction = this.FinalInstruction.Clone();
			return clone;
		}
		
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
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

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			output.Write("Block ");
			output.WriteDefinition(Label, this);
			if (Parent is BlockContainer)
				output.Write(" (incoming: {0})", IncomingEdgeCount);
			output.Write(' ');
			output.MarkFoldStart("{...}");
			output.WriteLine(" {");
			output.Indent();
			foreach (var inst in Instructions) {
				inst.WriteTo(output, options);
				output.WriteLine();
			}
			if (finalInstruction.OpCode != OpCode.Nop) {
				output.Write("final: ");
				finalInstruction.WriteTo(output, options);
				output.WriteLine();
			}
			output.Unindent();
			output.Write("}");
			output.MarkFoldEnd();
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
		
		protected override SlotInfo GetChildSlot(int index)
		{
			if (index == Instructions.Count)
				return FinalInstructionSlot;
			else
				return InstructionSlot;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.None;
			foreach (var inst in Instructions) {
				flags |= inst.Flags;
			}
			flags |= FinalInstruction.Flags;
			return flags;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.None;
			}
		}

		/// <summary>
		/// Deletes this block from its parent container.
		/// This may cause the indices of other blocks in that container to change.
		/// 
		/// It is an error to call this method on blocks that are not directly within a container.
		/// It is also an error to call this method on the entry-point block.
		/// </summary>
		public void Remove()
		{
			Debug.Assert(ChildIndex > 0);
			var container = (BlockContainer)Parent;
			Debug.Assert(container.Blocks[ChildIndex] == this);
			container.Blocks.SwapRemoveAt(ChildIndex);
		}

		/// <summary>
		/// Apply a list of transforms to this function.
		/// </summary>
		public void RunTransforms(IEnumerable<IBlockTransform> transforms, BlockTransformContext context)
		{
			this.CheckInvariant(ILPhase.Normal);
			foreach (var transform in transforms) {
				context.CancellationToken.ThrowIfCancellationRequested();
				context.StepStartGroup(transform.GetType().Name);
				transform.Run(this, context);
				this.CheckInvariant(ILPhase.Normal);
				context.StepEndGroup();
			}
		}

		/// <summary>
		/// Gets the predecessor of the given instruction.
		/// Returns null if inst.Parent is not a block.
		/// </summary>
		public static ILInstruction GetPredecessor(ILInstruction inst)
		{
			if (inst.Parent is Block block && inst.ChildIndex > 0) {
				return block.Instructions[inst.ChildIndex - 1];
			} else {
				return null;
			}
		}

		/// <summary>
		/// If inst is a block consisting of a single instruction, returns that instruction.
		/// Otherwise, returns the input instruction.
		/// </summary>
		public static ILInstruction Unwrap(ILInstruction inst)
		{
			if (inst is Block block) {
				if (block.Instructions.Count == 1 && block.finalInstruction.MatchNop())
					return block.Instructions[0];
			}
			return inst;
		}
	}
	
	public enum BlockType {
		ControlFlow,
		ArrayInitializer,
		CollectionInitializer,
		ObjectInitializer,
		PostfixOperator
	}
}
