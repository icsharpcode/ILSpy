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
	partial class Leave : ILInstruction
	{
		BlockContainer targetContainer;
		
		public Leave(BlockContainer targetContainer) : base(OpCode.Leave)
		{
			// Note: ILReader will create Leave instructions with targetContainer==null to represent 'endfinally',
			// the targetContainer will then be filled in by BlockBuilder
			this.targetContainer = targetContainer;
			this.Value = new Nop();
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
			get { return targetContainer; }
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
		/// Gets whether the leave instruction is leaving the whole ILFunction.
		/// (TargetContainer == main container of the function).
		/// 
		/// This is only valid for functions returning void (representing value-less "return;"),
		/// and for iterators (representing "yield break;").
		/// </summary>
		public bool IsLeavingFunction {
			get { return targetContainer?.Parent is ILFunction; }
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(phase <= ILPhase.InILReader || this.IsDescendantOf(targetContainer));
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			if (targetContainer != null) {
				output.Write(' ');
				output.WriteReference(TargetLabel, targetContainer, isLocal: true);
				output.Write(" (");
				value.WriteTo(output);
				output.Write(')');
			}
		}
	}
}
