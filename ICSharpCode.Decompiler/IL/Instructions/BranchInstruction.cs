using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Base class for unconditional and conditional branches.
	/// </summary>
	public abstract class BranchInstruction : ILInstruction
	{
		public readonly int TargetILOffset;
		
		protected BranchInstruction(OpCode opCode, int targetILOffset) : base(opCode)
		{
			this.TargetILOffset = targetILOffset;
		}
		
		public string TargetLabel {
			get { return CecilExtensions.OffsetToString(TargetILOffset); }
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode.ToString());
			output.Write(' ');
			output.WriteReference(TargetLabel, TargetILOffset, isLocal: true);
		}
	}

	partial class Branch
	{
		public override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			return initial;
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			finished = true;
			return this;
		}
	}
	
	partial class ConditionalBranch
	{
		ILInstruction condition;
		
		public ConditionalBranch(ILInstruction condition, int targetILOffset) : base(OpCode.ConditionalBranch, targetILOffset)
		{
			this.Condition = condition;
		}
		
		public ILInstruction Condition {
			get {
				return condition;
			}
			set {
				ValidateArgument(value);
				SetChildInstruction(ref condition, value);
			}
		}
		
		public override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			return func(initial, condition.AcceptVisitor(visitor));
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Condition = condition.AcceptVisitor(visitor);
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			this.Condition = condition.Inline(flagsBefore, instructionStack, out finished);
			return this;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return condition.Flags | InstructionFlags.MayBranch;
		}
	}
	
	/*
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

		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			Condition = Condition.Inline(flagsBefore, instructionStack, out finished);
			return this;
		}

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.MayJump | Condition.Flags; }
		}
	}

	class ReturnVoidInstruction() : SimpleInstruction(OpCode.Ret)
	{
		public override bool IsEndReachable { get { return false; } }

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.MayJump; }
		}
	}

	class ReturnInstruction() : UnaryInstruction(OpCode.Ret)
	{
		public override bool IsEndReachable { get { return false; } }

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.MayJump | Operand.Flags; }
		}
	}

	class ThrowInstruction() : UnaryInstruction(OpCode.Throw)
	{
		public override bool IsEndReachable { get { return false; } }

		public override InstructionFlags Flags
		{
			get { return InstructionFlags.MayJump | Operand.Flags; }
		}
	}*/
}
