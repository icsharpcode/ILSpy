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
	/// Unconditional branch. <c>goto target;</c>
	/// </summary>
	/// <remarks>
	/// When jumping to the entrypoint of the current block container, the branch represents a <c>continue</c> statement.
	/// </remarks>
	partial class Branch : SimpleInstruction
	{
		readonly int targetILOffset;
		Block targetBlock;
		
		public Branch(int targetILOffset) : base(OpCode.Branch)
		{
			this.targetILOffset = targetILOffset;
		}
		
		public Branch(Block targetBlock) : base(OpCode.Branch)
		{
			if (targetBlock == null)
				throw new ArgumentNullException("targetBlock");
			this.targetBlock = targetBlock;
			this.targetILOffset = targetBlock.ILRange.Start;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
			}
		}
		
		public int TargetILOffset {
			get { return targetBlock != null ? targetBlock.ILRange.Start : targetILOffset; }
		}
		
		public Block TargetBlock {
			get { return targetBlock; }
			set {
				if (targetBlock != null && IsConnected)
					targetBlock.IncomingEdgeCount--;
				targetBlock = value;
				if (targetBlock != null && IsConnected)
					targetBlock.IncomingEdgeCount++;
			}
		}
		
		/// <summary>
		/// Gets the BlockContainer that contains the target block.
		/// </summary>
		public BlockContainer TargetContainer {
			get { return (BlockContainer)targetBlock?.Parent; }
		}
		
		protected override void Connected()
		{
			base.Connected();
			if (targetBlock != null)
				targetBlock.IncomingEdgeCount++;
		}
		
		protected override void Disconnected()
		{
			base.Disconnected();
			if (targetBlock != null)
				targetBlock.IncomingEdgeCount--;
		}
		
		public string TargetLabel {
			get { return targetBlock != null ? targetBlock.Label : CecilExtensions.OffsetToString(TargetILOffset); }
		}
		
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			if (phase > ILPhase.InILReader) {
				Debug.Assert(targetBlock.Parent is BlockContainer);
				Debug.Assert(this.IsDescendantOf(targetBlock.Parent));
				Debug.Assert(targetBlock.Parent.Children[targetBlock.ChildIndex] == targetBlock);
			}
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			output.WriteReference(TargetLabel, (object)targetBlock ?? TargetILOffset, isLocal: true);
		}
	}
}
