#nullable enable
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
using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Unconditional branch. <c>goto target;</c>
	/// </summary>
	/// <remarks>
	/// When jumping to the entrypoint of the current block container, the branch represents a <c>continue</c> statement.
	/// </remarks>
	partial class Branch : SimpleInstruction, IBranchOrLeaveInstruction
	{
		readonly int targetILOffset;
		Block? targetBlock;

		public Branch(int targetILOffset) : base(OpCode.Branch)
		{
			this.targetILOffset = targetILOffset;
		}

		public Branch(Block targetBlock) : base(OpCode.Branch)
		{
			this.targetBlock = targetBlock ?? throw new ArgumentNullException(nameof(targetBlock));
			this.targetILOffset = targetBlock.StartILOffset;
		}

		public int TargetILOffset {
			get { return targetBlock != null ? targetBlock.StartILOffset : targetILOffset; }
		}

		public Block TargetBlock {
			get {
				// HACK: We treat TargetBlock as non-nullable publicly, because it's only null inside
				// the ILReader, and becomes non-null once the BlockBuilder has run.
				return targetBlock!;
			}
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
			get { return (BlockContainer)targetBlock?.Parent!; }
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
			get { return targetBlock != null ? targetBlock.Label : string.Format("IL_{0:x4}", TargetILOffset); }
		}

		/// <summary>
		/// Gets whether this branch executes at least one finally block before jumping to the target block.
		/// </summary>
		public bool TriggersFinallyBlock {
			get {
				return GetExecutesFinallyBlock(this, TargetContainer);
			}
		}

		internal static bool GetExecutesFinallyBlock(ILInstruction? inst, BlockContainer? container)
		{
			for (; inst != container && inst != null; inst = inst.Parent)
			{
				if (inst.Parent is TryFinally && inst.SlotInfo == TryFinally.TryBlockSlot)
					return true;
			}
			return false;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			if (phase > ILPhase.InILReader)
			{
				Debug.Assert(targetBlock?.Parent is BlockContainer);
				Debug.Assert(this.IsDescendantOf(targetBlock!.Parent!));
				Debug.Assert(targetBlock!.Parent!.Children[targetBlock.ChildIndex] == targetBlock);
			}
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write(' ');
			output.WriteLocalReference(TargetLabel, (object?)targetBlock ?? TargetILOffset);
		}
	}

	interface IBranchOrLeaveInstruction
	{
		BlockContainer TargetContainer { get; }
	}
}
