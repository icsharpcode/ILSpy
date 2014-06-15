using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Base class for unconditional and conditional branches.
	/// </summary>
	class BranchInstruction(OpCode opCode, public int TargetILOffset) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return false; } }

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode.ToString());
			output.Write(' ');
			output.WriteReference(CecilExtensions.OffsetToString(TargetILOffset), TargetILOffset, isLocal: true);
		}
	}

	/// <summary>
	/// Special instruction for unresolved branches.
	/// Created by ILReader phase, replaced with TODO when building basic blocks.
	/// </summary>
	class ConditionalBranchInstruction(public ILInstruction Condition, int targetILOffset) : BranchInstruction(OpCode.ConditionalBranch, targetILOffset)
	{
		public override bool IsPeeking { get { return Condition.IsPeeking; } }

		public override void WriteTo(ITextOutput output)
		{
			base.WriteTo(output);
			output.Write('(');
			Condition.WriteTo(output);
			output.Write(')');
		}
	}
}
