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

using System.Diagnostics;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Unconditional branch. <c>goto target;</c>
	/// </summary>
	/// <remarks>
	/// When jumping to the entrypoint of the current block container, the branch represents a <c>continue</c> statement.
	/// 
	/// Phase-1 execution of a branch is a no-op.
	/// Phase-2 execution removes PopCount elements from the evaluation stack
	/// and jumps to the target block.
	/// </remarks>
	partial class Leave : ILInstruction, IBranchOrLeaveInstruction
	{
		BlockContainer? targetContainer;

		public Leave(BlockContainer? targetContainer, ILInstruction? value = null) : base(OpCode.Leave)
		{
			// Note: ILReader will create Leave instructions with targetContainer==null to represent 'endfinally',
			// the targetContainer will then be filled in by BlockBuilder
			this.targetContainer = targetContainer;
			this.Value = value ?? new Nop();
		}

		protected override InstructionFlags ComputeFlags()
		{
			return value.Flags | InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
		}

		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable;
			}
		}

		public BlockContainer TargetContainer {
			get { return targetContainer!; }
			set {
				if (targetContainer != null && IsConnected)
					targetContainer.LeaveCount--;
				targetContainer = value;
				if (targetContainer != null && IsConnected)
					targetContainer.LeaveCount++;
			}
		}

		protected override void Connected()
		{
			base.Connected();
			if (targetContainer != null)
				targetContainer.LeaveCount++;
		}

		protected override void Disconnected()
		{
			base.Disconnected();
			if (targetContainer != null)
				targetContainer.LeaveCount--;
		}

		public string TargetLabel {
			get { return targetContainer?.EntryPoint != null ? targetContainer.EntryPoint.Label : string.Empty; }
		}

		/// <summary>
		/// Gets whether the leave instruction is directly leaving the whole ILFunction.
		/// (TargetContainer == main container of the function).
		/// 
		/// This is only valid for functions returning void (representing value-less "return;"),
		/// and for iterators (representing "yield break;").
		/// 
		/// Note: returns false for leave instructions that indirectly leave the function
		/// (e.g. leaving a try block, and the try-finally construct is immediately followed
		/// by another leave instruction)
		/// </summary>
		public bool IsLeavingFunction {
			get { return targetContainer?.Parent is ILFunction; }
		}

		/// <summary>
		/// Gets whether this branch executes at least one finally block before jumping to the end of the target block container.
		/// </summary>
		public bool TriggersFinallyBlock {
			get {
				return Branch.GetExecutesFinallyBlock(this, TargetContainer);
			}
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(targetContainer!));
			Debug.Assert(phase <= ILPhase.InILReader || phase == ILPhase.InAsyncAwait || value.ResultType == targetContainer!.ResultType);
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (targetContainer != null)
			{
				output.Write(' ');
				output.WriteLocalReference(TargetLabel, targetContainer);
				output.Write(" (");
				value.WriteTo(output, options);
				output.Write(')');
			}
		}
	}
}
