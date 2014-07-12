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

namespace ICSharpCode.Decompiler.IL
{
	partial class IfInstruction : ILInstruction
	{
		ILInstruction condition;
		ILInstruction trueInst;
		ILInstruction falseInst;
		
		public IfInstruction(ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst = null) : base(OpCode.IfInstruction)
		{
			this.Condition = condition;
			this.TrueInst = trueInst;
			this.FalseInst = falseInst ?? new Nop();
		}
		
		public ILInstruction Condition {
			get { return condition; }
			set {
				ValidateArgument(value);
				SetChildInstruction(ref condition, value);
			}
		}
		
		public ILInstruction TrueInst {
			get { return trueInst; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				SetChildInstruction(ref trueInst, value);
			}
		}
		
		public ILInstruction FalseInst {
			get { return falseInst; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				SetChildInstruction(ref falseInst, value);
			}
		}
		
		public override StackType ResultType {
			get {
				return trueInst.ResultType;
			}
		}
		
		public override TAccumulate AggregateChildren<TSource, TAccumulate>(TAccumulate initial, ILVisitor<TSource> visitor, Func<TAccumulate, TSource, TAccumulate> func)
		{
			TAccumulate value = initial;
			value = func(value, condition.AcceptVisitor(visitor));
			value = func(value, trueInst.AcceptVisitor(visitor));
			value = func(value, falseInst.AcceptVisitor(visitor));
			return value;
		}
		
		public override void TransformChildren(ILVisitor<ILInstruction> visitor)
		{
			this.Condition = condition.AcceptVisitor(visitor);
			this.TrueInst = trueInst.AcceptVisitor(visitor);
			this.FalseInst = falseInst.AcceptVisitor(visitor);
		}
		
		internal override ILInstruction Inline(InstructionFlags flagsBefore, Stack<ILInstruction> instructionStack, out bool finished)
		{
			this.Condition = condition.Inline(flagsBefore, instructionStack, out finished);
			// don't continue inlining if this instruction still contains peek/pop instructions
			if (HasFlag(InstructionFlags.MayPeek | InstructionFlags.MayPop))
				finished = false;
			return this;
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			// the endpoint of the 'if' is only unreachable if both branches have an unreachable endpoint
			const InstructionFlags combineWithAnd = InstructionFlags.EndPointUnreachable;
			InstructionFlags trueFlags = trueInst.Flags;
			InstructionFlags falseFlags = falseInst.Flags;
			return condition.Flags | (trueFlags & falseFlags) | ((trueFlags | falseFlags) & ~combineWithAnd);
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write(" (");
			condition.WriteTo(output);
			output.Write(") ");
			trueInst.WriteTo(output);
			if (falseInst.OpCode != OpCode.Nop) {
				output.Write(" else ");
				falseInst.WriteTo(output);
			}
		}
	}
}
