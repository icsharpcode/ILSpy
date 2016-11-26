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
	/// <summary>If statement / conditional expression. <c>if (condition) trueExpr else falseExpr</c></summary>
	/// <remarks>
	/// The condition must return StackType.I4, use comparison instructions like Ceq to check if other types are non-zero.
	///
	/// If the condition evaluates to non-zero, the TrueInst is executed.
	/// If the condition evaluates to zero, the FalseInst is executed.
	/// The return value of the IfInstruction is the return value of the TrueInst or FalseInst.
	/// 
	/// IfInstruction is also used to represent logical operators:
	///   "a || b" ==> if (a) (ldc.i4 1) else (b)
	///   "a && b" ==> if (a) (b) else (ldc.i4 0)
	///   "a ? b : c" ==> if (a) (b) else (c)
	/// </remarks>
	partial class IfInstruction : ILInstruction
	{
		public IfInstruction(ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst = null) : base(OpCode.IfInstruction)
		{
			this.Condition = condition;
			this.TrueInst = trueInst;
			this.FalseInst = falseInst ?? new Nop();
		}

		public static IfInstruction LogicAnd(ILInstruction lhs, ILInstruction rhs)
		{
			return new IfInstruction(lhs, rhs, new LdcI4(0));
		}

		public static IfInstruction LogicOr(ILInstruction lhs, ILInstruction rhs)
		{
			return new IfInstruction(lhs, new LdcI4(1), rhs);
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
		
		public override InstructionFlags DirectFlags {
			get {
				return InstructionFlags.ControlFlow;
			}
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			return InstructionFlags.ControlFlow | condition.Flags | SemanticHelper.CombineBranches(trueInst.Flags, falseInst.Flags);
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
