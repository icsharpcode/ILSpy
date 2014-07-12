using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A container of IL blocks.
	/// Each block is an extended basic block (branches may only jump to the beginning of blocks, not into the middle),
	/// and only branches within this container may reference the blocks in this container.
	/// That means that viewed from the outside, the block container has a single entry point (but possibly multiple exit points),
	/// and the same holds for every block within the container.
	/// </summary>
	partial class BlockContainer : ILInstruction
	{
		public readonly IList<Block> Blocks;

		public Block EntryPoint {
			get {
				return Blocks[0];
			}
		}

		public BlockContainer() : base(OpCode.BlockContainer)
		{
			this.Blocks = new InstructionCollection<Block>(this);
		}

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
		*/public override void WriteTo(ITextOutput output)
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

		public override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate value = initial;
			foreach (var block in Blocks) {
				value = func(value, block.AcceptVisitor(visitor));
			}
			return value;
		}

		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			foreach (var block in Blocks) {
				// Recurse into the blocks, but don't allow replacing the block
				if (block.AcceptVisitor(visitor) != block)
					throw new InvalidOperationException("Cannot replace blocks in BlockContainer");
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = InstructionFlags.None;
			foreach (var block in Blocks) {
				flags |= block.Flags;
			}
			return flags;
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = false;
			return this;
		}
	}
}


