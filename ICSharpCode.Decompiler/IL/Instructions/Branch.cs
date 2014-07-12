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
		public readonly int TargetILOffset;
		
		/// <summary>
		/// Pops the specified number of arguments from the evaluation stack during the branching operation.
		/// Note that the Branch instruction does not set InstructionFlags.MayPop -- the pop instead is considered
		/// to happen after the branch was taken.
		/// </summary>
		public int PopCount;
		
		public Branch(int targetILOffset) : base(OpCode.Branch)
		{
			this.TargetILOffset = targetILOffset;
		}
		
		public string TargetLabel {
			get { return CecilExtensions.OffsetToString(TargetILOffset); }
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(' ');
			output.WriteReference(TargetLabel, TargetILOffset, isLocal: true);
			if (PopCount != 0) {
				output.Write(" (pop ");
				output.Write(PopCount.ToString());
				output.Write(')');
			}
		}
	}
}
