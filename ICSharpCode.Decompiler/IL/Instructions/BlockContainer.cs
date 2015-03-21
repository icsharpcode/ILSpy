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

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A container of IL blocks.
	/// Each block is an extended basic block (branches may only jump to the beginning of blocks, not into the middle),
	/// and only branches within this container may reference the blocks in this container.
	/// That means that viewed from the outside, the block container has a single entry point (but possibly multiple exit points),
	/// and the same holds for every block within the container.
	/// 
	/// If a block within the container falls through to its end point, control flow is transferred to the end point
	/// of the whole block container. The return value of the block is ignored in this case, the container always
	/// returns void.
	/// </summary>
	partial class BlockContainer : ILInstruction
	{
		public readonly InstructionCollection<Block> Blocks;
		Block entryPoint;

		public Block EntryPoint {
			get { return entryPoint; }
			private set {
				if (entryPoint != null && IsConnected)
					entryPoint.IncomingEdgeCount--;
				entryPoint = value;
				if (entryPoint != null && IsConnected)
					entryPoint.IncomingEdgeCount++;
			}
		}

		public BlockContainer() : base(OpCode.BlockContainer)
		{
			this.Blocks = new InstructionCollection<Block>(this, 0);
		}

		protected internal override void InstructionCollectionUpdateComplete()
		{
			base.InstructionCollectionUpdateComplete();
			this.EntryPoint = this.Blocks.FirstOrDefault();
		}
		
		protected override void Connected()
		{
			base.Connected();
			if (entryPoint != null)
				entryPoint.IncomingEdgeCount++;
		}
		
		protected override void Disconnected()
		{
			base.Disconnected();
			if (entryPoint != null)
				entryPoint.IncomingEdgeCount--;
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.WriteLine("BlockContainer {");
			output.Indent();
			foreach (var inst in Blocks) {
				inst.WriteTo(output);
				output.WriteLine();
				output.WriteLine();
			}
			output.Unindent();
			output.Write("}");
		}
		
		protected override int GetChildCount()
		{
			return Blocks.Count;
		}
		
		protected override ILInstruction GetChild(int index)
		{
			return Blocks[index];
		}
		
		protected override void SetChild(int index, ILInstruction value)
		{
			if (Blocks[index] != value)
				throw new InvalidOperationException("Cannot replace blocks in BlockContainer");
		}

		internal override void CheckInvariant()
		{
			base.CheckInvariant();
			Debug.Assert(EntryPoint == Blocks[0]);
			Debug.Assert(!IsConnected || EntryPoint.IncomingEdgeCount >= 1);
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			InstructionFlags flagsInAnyBlock = InstructionFlags.None;
			InstructionFlags flagsInAllBlocks = ~InstructionFlags.None;
			foreach (var block in Blocks) {
				flagsInAnyBlock |= block.Flags;
				flagsInAllBlocks &= block.Flags;
			}
			// Return EndPointUnreachable only if no block has a reachable endpoint.
			// The other flags are combined from all blocks.
			return (flagsInAnyBlock & ~InstructionFlags.EndPointUnreachable)
				| (flagsInAllBlocks & InstructionFlags.EndPointUnreachable);
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, IInlineContext context)
		{
			// Blocks are phase-1 boundaries
			return this;
		}

		internal override void TransformStackIntoVariables(TransformStackIntoVariablesState state)
		{
			EntryPoint.TransformStackIntoVariables(state);
			ImmutableArray<ILVariable> variables;
			if (state.FinalVariables.TryGetValue(this, out variables))
				state.Variables = variables.ToStack();
			else
				state.Variables.Clear();
		}
	}
}


