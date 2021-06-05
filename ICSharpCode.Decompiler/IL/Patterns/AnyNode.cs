#nullable enable
using System;

namespace ICSharpCode.Decompiler.IL.Patterns
{
	partial class PatternInstruction : ILInstruction
	{
		public override void AcceptVisitor(ILVisitor visitor)
		{
			throw new NotSupportedException();
		}

		public override T AcceptVisitor<C, T>(ILVisitor<C, T> visitor, C context)
		{
			throw new NotSupportedException();
		}

		public override T AcceptVisitor<T>(ILVisitor<T> visitor)
		{
			throw new NotSupportedException();
		}

		protected override InstructionFlags ComputeFlags()
		{
			throw new NotSupportedException();
		}

		public override InstructionFlags DirectFlags {
			get {
				throw new NotSupportedException();
			}
		}
	}

	partial class AnyNode : PatternInstruction
	{
		CaptureGroup? group;

		public AnyNode(CaptureGroup? group = null)
			: base(OpCode.AnyNode)
		{
			this.group = group;
		}

		protected internal override bool PerformMatch(ILInstruction? other, ref Match match)
		{
			if (other == null)
				return false;
			if (group != null)
				match.Add(group, other);
			return true;
		}
	}
}
