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
using System.Diagnostics;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	public enum BinaryNumericOperator : byte
	{
		None,
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

	public partial class BinaryNumericInstruction : BinaryInstruction, ILiftableInstruction
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

		public readonly StackType LeftInputType;
		public readonly StackType RightInputType;

		/// <summary>
		/// The operator used by this binary operator instruction.
		/// </summary>
		public readonly BinaryNumericOperator Operator;

		/// <summary>
		/// Gets whether this is a lifted nullable operation.
		/// </summary>
		/// <remarks>
		/// A lifted binary operation allows its arguments to be a value of type Nullable{T}, where
		/// T.GetStackType() == [Left|Right]InputType.
		/// If both input values are non-null:
		///  * they are sign/zero-extended to the corresponding InputType (based on T's sign)
		///  * the underlying numeric operator is applied
		///  * the result is wrapped in a Nullable{UnderlyingResultType}.
		/// If either input is null, the instruction evaluates to default(UnderlyingResultType?).
		/// (this result type is underspecified, since there may be multiple C# types for the stack type)
		/// </remarks>
		public bool IsLifted { get; }

		readonly StackType resultType;

		public BinaryNumericInstruction(BinaryNumericOperator op, ILInstruction left, ILInstruction right, bool checkForOverflow, Sign sign)
			: this(op, left, right, left.ResultType, right.ResultType, checkForOverflow, sign)
		{
		}

		public BinaryNumericInstruction(BinaryNumericOperator op, ILInstruction left, ILInstruction right, StackType leftInputType, StackType rightInputType, bool checkForOverflow, Sign sign, bool isLifted = false)
			: base(OpCode.BinaryNumericInstruction, left, right)
		{
			this.CheckForOverflow = checkForOverflow;
			this.Sign = sign;
			this.Operator = op;
			this.LeftInputType = leftInputType;
			this.RightInputType = rightInputType;
			this.IsLifted = isLifted;
			this.resultType = ComputeResultType(op, LeftInputType, RightInputType);
		}

		internal static StackType ComputeResultType(BinaryNumericOperator op, StackType left, StackType right)
		{
			// Based on Table 2: Binary Numeric Operations
			// also works for Table 5: Integer Operations
			// and for Table 7: Overflow Arithmetic Operations
			if (left == right || op == BinaryNumericOperator.ShiftLeft || op == BinaryNumericOperator.ShiftRight)
			{
				// Shift op codes use Table 6
				return left;
			}
			if (left == StackType.Ref || right == StackType.Ref)
			{
				if (left == StackType.Ref && right == StackType.Ref)
				{
					// sub(&, &) = I
					Debug.Assert(op == BinaryNumericOperator.Sub);
					return StackType.I;
				}
				else
				{
					// add/sub with I or I4 and &
					Debug.Assert(op == BinaryNumericOperator.Add || op == BinaryNumericOperator.Sub);
					return StackType.Ref;
				}
			}
			return StackType.Unknown;
		}

		public StackType UnderlyingResultType { get => resultType; }

		public sealed override StackType ResultType {
			get => IsLifted ? StackType.O : resultType;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			if (!IsLifted)
			{
				Debug.Assert(LeftInputType == Left.ResultType);
				Debug.Assert(RightInputType == Right.ResultType);
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
				if (CheckForOverflow || (Operator == BinaryNumericOperator.Div || Operator == BinaryNumericOperator.Rem))
					return base.DirectFlags | InstructionFlags.MayThrow;
				return base.DirectFlags;
			}
		}

		internal static string GetOperatorName(BinaryNumericOperator @operator)
		{
			switch (@operator)
			{
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

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			output.Write("." + GetOperatorName(Operator));
			if (CheckForOverflow)
			{
				output.Write(".ovf");
			}
			if (Sign == Sign.Unsigned)
			{
				output.Write(".unsigned");
			}
			else if (Sign == Sign.Signed)
			{
				output.Write(".signed");
			}
			output.Write('.');
			output.Write(resultType.ToString().ToLowerInvariant());
			if (IsLifted)
			{
				output.Write(".lifted");
			}
			output.Write('(');
			Left.WriteTo(output, options);
			output.Write(", ");
			Right.WriteTo(output, options);
			output.Write(')');
		}
	}
}


