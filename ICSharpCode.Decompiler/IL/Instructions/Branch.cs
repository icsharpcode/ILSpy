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
	partial class Branch : SimpleInstruction
	{
		readonly int targetILOffset;
		Block targetBlock;
		
		/// <summary>
		/// Pops the specified number of arguments from the evaluation stack during the branching operation.
		/// Note that the Branch instruction does not set InstructionFlags.MayPop -- the pop instead is considered
		/// to happen after the branch was taken.
		/// </summary>
		public int PopCount;
		
		public Branch(int targetILOffset) : base(OpCode.Branch)
		{
			this.targetILOffset = targetILOffset;
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
		
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			output.WriteReference(TargetLabel, (object)targetBlock ?? TargetILOffset, isLocal: true);
			if (PopCount != 0) {
				output.Write(" (pop ");
				output.Write(PopCount.ToString());
				output.Write(')');
			}
		}
	}
}
