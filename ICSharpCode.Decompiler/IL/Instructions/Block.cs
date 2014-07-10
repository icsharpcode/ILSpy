using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A block of IL instructions.
	/// </summary>
	partial class Block : ILInstruction
	{
		public readonly List<ILInstruction> Instructions = new List<ILInstruction>();

		/*
		public override bool IsPeeking { get { return Instructions.Count > 0 && Instructions[0].IsPeeking; } }

		public override bool NoResult { get { return true; } }

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			for (int i = 0; i < Instructions.Count; i++) {
				Instructions[i] = transformFunc(Instructions[i]);
			}
		}
		*/

		/// <summary>
		/// Gets the name of this block.
		/// </summary>
		public string Label
		{
			get { return Disassembler.DisassemblerHelpers.OffsetToString(this.ILRange.Start); }
		}

		public override void WriteTo(ITextOutput output)
		{
			output.WriteDefinition("Block " + Label, this);
			output.WriteLine(" {");
			output.Indent();
			foreach (var inst in Instructions) {
				inst.WriteTo(output);
				output.WriteLine();
			}
			output.Unindent();
			output.WriteLine("}");
		}

		/*
		public override InstructionFlags Flags
		{
			get {
				InstructionFlags flags = InstructionFlags.None;
				for (int i = 0; i < Instructions.Count; i++) {
					flags |= Instructions[i].Flags;
				}
				return flags;
			}
		}

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			if (Instructions.Count > 0)
				Instructions[0] = Instructions[0].Inline(flagsBefore, instructionStack, out finished);
			else
				finished = true;
			return this;
		}*/
	}

	/// <summary>
	/// A container of IL blocks.
	/// Each block is an extended basic block (branches may only jump to the beginning of blocks, not into the middle),
	/// and only branches within this container may reference the blocks in this container.
	/// That means that viewed from the outside, the block container has a single entry point (but possibly multiple exit points),
	/// and the same holds for every block within the container.
	/// </summary>
	partial class BlockContainer : ILInstruction
	{
		public List<Block> Blocks = new List<Block>();
		public Block EntryPoint { get { return Blocks[0]; } }

		/*
		public override bool IsPeeking { get { return EntryPoint.IsPeeking; } }

		public override bool NoResult { get { return true; } }

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			for (int i = 0; i < Blocks.Count; i++) {
				if (transformFunc(Blocks[i]) != Blocks[i])
					throw new InvalidOperationException("Cannot replace blocks");
            }
		}
		*/

		public override void WriteTo(ITextOutput output)
		{
			output.WriteLine("BlockContainer {");
			output.Indent();
			foreach (var inst in Blocks) {
				inst.WriteTo(output);
				output.WriteLine();
			}
			output.Unindent();
			output.WriteLine("}");
		}

		/*
		public override InstructionFlags Flags
		{
			get
			{
				InstructionFlags flags = InstructionFlags.None;
				for (int i = 0; i < Blocks.Count; i++) {
					flags |= Blocks[i].Flags;
				}
				return flags;
			}
		}

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
		}*/
	}
}
