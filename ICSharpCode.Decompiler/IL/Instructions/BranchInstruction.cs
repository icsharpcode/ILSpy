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
	class Branch(OpCode opCode, public int TargetILOffset) : ILInstruction(opCode)
	{
		public override bool IsPeeking { get { return false; } }

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode.ToString());
			output.Write(' ');
			output.WriteReference(CecilExtensions.OffsetToString(TargetILOffset), TargetILOffset, isLocal: true);
		}

		public override bool IsEndReachable
		{
			get
			{
				// end is reachable for conditional branches, but not unconditional ones
				return OpCode == OpCode.ConditionalBranch;
			}
		}

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
		}
	}

	/// <summary>
	/// Special instruction for unresolved branches.
	/// Created by ILReader phase, replaced with TODO when building basic blocks.
	/// </summary>
	class ConditionalBranch(public ILInstruction Condition, int targetILOffset) : Branch(OpCode.ConditionalBranch, targetILOffset)
	{
		public override bool IsPeeking { get { return Condition.IsPeeking; } }

		public override void WriteTo(ITextOutput output)
		{
			base.WriteTo(output);
			output.Write('(');
			Condition.WriteTo(output);
			output.Write(')');
		}

		public override void TransformChildren(Func<ILInstruction, ILInstruction> transformFunc)
		{
			Condition = transformFunc(Condition);
		}
	}

	class RetVoid() : SimpleInstruction(OpCode.Ret)
	{
		public override bool IsEndReachable { get { return false; } }
	}

	class Ret() : UnaryInstruction(OpCode.Ret)
	{
		public override bool IsEndReachable { get { return false; } }
	}
}
