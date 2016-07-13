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
	public enum CompoundAssignmentType : byte
	{
		None,
		EvaluatesToOldValue,
		EvaluatesToNewValue
	}
	
	public enum BinaryNumericOperator : byte
	{
		Add,
		Sub,
		Mul,
		Div,
		Rem,
		BitAnd,
		BitOr,
		BitXor,
		ShiftLeft,
		ShiftRight
	}
	
	public partial class BinaryNumericInstruction : BinaryInstruction
	{
		/// <summary>
		/// Gets whether the instruction checks for overflow.
		/// </summary>
		public readonly bool CheckForOverflow;
		
		/// <summary>
		/// For integer operations that depend on the sign, specifies whether the operation
		/// is signed or unsigned.
		/// For instructions that produce the same result for either sign, returns Sign.None.
		/// </summary>
		public readonly Sign Sign;
		
		/// <summary>
		/// The operator used by this binary operator instruction.
		/// </summary>
		public readonly BinaryNumericOperator Operator;
		
		readonly StackType resultType;

		public BinaryNumericInstruction(BinaryNumericOperator op, ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign)
			: base(OpCode.BinaryNumericInstruction, left, right)
		{
			this.CheckForOverflow = checkForOverflow;
			this.Sign = sign;
			this.Operator = op;
			this.resultType = ComputeResultType(op, left.ResultType, right.ResultType);
			Debug.Assert(resultType != StackType.Unknown);
			//Debug.Assert(CompoundAssignmentType == CompoundAssignmentType.None || IsValidCompoundAssignmentTarget(Left));
		}
		
		internal static bool IsValidCompoundAssignmentTarget(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.LdLoc:
				case OpCode.LdObj:
					return true;
				case OpCode.Call:
				case OpCode.CallVirt:
					return true; // TODO: check if corresponding setter exists
				default:
					return false;
			}
		}
		
		internal static StackType ComputeResultType(BinaryNumericOperator op, StackType left, StackType right)
		{
			// Based on Table 2: Binary Numeric Operations
			// also works for Table 5: Integer Operations
			// and for Table 7: Overflow Arithmetic Operations
			if (left == right || op == BinaryNumericOperator.ShiftLeft || op == BinaryNumericOperator.ShiftRight) {
				// Shift op codes use Table 6
				return left;
			}
			if (left == StackType.Ref || right == StackType.Ref) {
				if (left == StackType.Ref && right == StackType.Ref) {
					// sub(&, &) = I
					Debug.Assert(op == BinaryNumericOperator.Sub);
					return StackType.I;
				} else {
					// add/sub with I or I4 and &
					Debug.Assert(op == BinaryNumericOperator.Add || op == BinaryNumericOperator.Sub);
					return StackType.Ref;
				}
			}
			return StackType.Unknown;
		}
		
		public sealed override StackType ResultType {
			get {
				return resultType;
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = base.ComputeFlags();
			if (CheckForOverflow || (Operator == BinaryNumericOperator.Div || Operator == BinaryNumericOperator.Rem))
				flags |= InstructionFlags.MayThrow;
			return flags;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				if (Operator == BinaryNumericOperator.Div || Operator == BinaryNumericOperator.Rem)
					return base.DirectFlags | InstructionFlags.MayThrow;
				return base.DirectFlags;
			}
		}

		string GetOperatorName(BinaryNumericOperator @operator)
		{
			switch (@operator) {
				case BinaryNumericOperator.Add:
					return "add";
				case BinaryNumericOperator.Sub:
					return "sub";
				case BinaryNumericOperator.Mul:
					return "mul";
				case BinaryNumericOperator.Div:
					return "div";
				case BinaryNumericOperator.Rem:
					return "rem";
				case BinaryNumericOperator.BitAnd:
					return "bit.and";
				case BinaryNumericOperator.BitOr:
					return "bit.or";
				case BinaryNumericOperator.BitXor:
					return "bit.xor";
				case BinaryNumericOperator.ShiftLeft:
					return "bit.shl";
				case BinaryNumericOperator.ShiftRight:
					return "bit.shr";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			output.Write("." + GetOperatorName(Operator));
			if (CheckForOverflow)
				output.Write(".ovf");
			if (Sign == Sign.Unsigned)
				output.Write(".unsigned");
			else if (Sign == Sign.Signed)
				output.Write(".signed");
			output.Write('(');
			Left.WriteTo(output);
			output.Write(", ");
			Right.WriteTo(output);
			output.Write(')');
		}
	}
}


