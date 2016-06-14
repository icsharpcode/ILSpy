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

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
	/// <remarks>
	/// The condition must return StackType.I4, use comparison instructions like Ceq to check if other types are non-zero.
	/// Phase-1 execution of an IfInstruction consists of phase-1 execution of the condition.
	/// Phase-2 execution of an IfInstruction will phase-2-execute the condition.
	/// If the condition evaluates to a non-zero, the TrueInst is executed (both phase-1 and phase-2).
	/// If the condition evaluates to zero, the FalseInst is executed (both phase-1 and phase-2).
	/// The return value of the IfInstruction is the return value of the TrueInst or FalseInst.
	/// </remarks>
	partial class IfInstruction : ILInstruction
	{
		public IfInstruction(ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst = null) : base(OpCode.IfInstruction)
		{
			this.Condition = condition;
			this.TrueInst = trueInst;
			this.FalseInst = falseInst ?? new Nop();
		}
		
		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			Debug.Assert(condition.ResultType == StackType.I4);
		}
		
		public override StackType ResultType {
			get {
				return CommonResultType(trueInst.ResultType, falseInst.ResultType);
			}
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return condition.Flags | CombineFlags(trueInst.Flags, falseInst.Flags);
		}
		
		internal static InstructionFlags CombineFlags(InstructionFlags trueFlags, InstructionFlags falseFlags)
		{
			// the endpoint of the 'if' is only unreachable if both branches have an unreachable endpoint
			const InstructionFlags combineWithAnd = InstructionFlags.EndPointUnreachable;
			return (trueFlags & falseFlags) | ((trueFlags | falseFlags) & ~combineWithAnd);
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
