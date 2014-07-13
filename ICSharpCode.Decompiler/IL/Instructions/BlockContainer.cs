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
	/// A container of IL blocks.
	/// Each block is an extended basic block (branches may only jump to the beginning of blocks, not into the middle),
	/// and only branches within this container may reference the blocks in this container.
	/// That means that viewed from the outside, the block container has a single entry point (but possibly multiple exit points),
	/// and the same holds for every block within the container.
	/// </summary>
	partial class BlockContainer : ILInstruction
	{
		public readonly InstructionCollection<Block> Blocks;

		public Block EntryPoint {
			get {
				return Blocks[0];
			}
		}

		public BlockContainer() : base(OpCode.BlockContainer)
		{
			this.Blocks = new InstructionCollection<Block>(this);
		}

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
		
		public override IEnumerable<ILInstruction> Children {
			get { return Blocks; }
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


