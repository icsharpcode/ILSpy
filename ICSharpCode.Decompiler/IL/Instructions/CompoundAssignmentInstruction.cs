// Copyright (c) 2016 Siegfried Pammer
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
	public enum CompoundAssignmentType : byte
	{
		EvaluatesToOldValue,
		EvaluatesToNewValue
	}

	public partial class CompoundAssignmentInstruction : ILInstruction
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
		public StackType UnderlyingResultType { get; }

		/// <summary>
		/// The operator used by this assignment operator instruction.
		/// </summary>
		public readonly BinaryNumericOperator Operator;
		
		public readonly CompoundAssignmentType CompoundAssignmentType;

		public bool IsLifted { get; }

		public CompoundAssignmentInstruction(BinaryNumericInstruction binary, ILInstruction target, ILInstruction value, IType type, CompoundAssignmentType compoundAssignmentType)
			: base(OpCode.CompoundAssignmentInstruction)
		{
			Debug.Assert(IsBinaryCompatibleWithType(binary, type));
			this.CheckForOverflow = binary.CheckForOverflow;
			this.Sign = binary.Sign;
			this.LeftInputType = binary.LeftInputType;
			this.RightInputType = binary.RightInputType;
			this.UnderlyingResultType = binary.UnderlyingResultType;
			this.Operator = binary.Operator;
			this.CompoundAssignmentType = compoundAssignmentType;
			this.IsLifted = binary.IsLifted;
			this.Target = target;
			this.type = type;
			this.Value = value;
			this.ILRange = binary.ILRange;
			Debug.Assert(compoundAssignmentType == CompoundAssignmentType.EvaluatesToNewValue || (Operator == BinaryNumericOperator.Add || Operator == BinaryNumericOperator.Sub));
			Debug.Assert(IsValidCompoundAssignmentTarget(Target));
		}
		
		/// <summary>
		/// Gets whether the specific binary instruction is compatible with a compound operation on the specified type.
		/// </summary>
		internal static bool IsBinaryCompatibleWithType(BinaryNumericInstruction binary, IType type)
		{
			if (binary.IsLifted) {
				if (!NullableType.IsNullable(type))
					return false;
				type = NullableType.GetUnderlyingType(type);
			}
			if (type.Kind == TypeKind.Enum) {
				switch (binary.Operator) {
					case BinaryNumericOperator.Add:
					case BinaryNumericOperator.Sub:
					case BinaryNumericOperator.BitAnd:
					case BinaryNumericOperator.BitOr:
					case BinaryNumericOperator.BitXor:
						break; // OK
					default:
						return false; // operator not supported on enum types
				}
			}
			if (binary.Sign != Sign.None) {
				if (type.GetSign() != binary.Sign)
					return false;
			}
			return true;
		}

		internal static bool IsValidCompoundAssignmentTarget(ILInstruction inst)
		{
			switch (inst.OpCode) {
				// case OpCode.LdLoc: -- not valid -- does not mark the variable as written to
				case OpCode.LdObj:
					return true;
				case OpCode.Call:
				case OpCode.CallVirt:
					var owner = ((CallInstruction)inst).Method.AccessorOwner as IProperty;
					return owner != null && owner.CanSet;
				default:
					return false;
			}
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = target.Flags | value.Flags | InstructionFlags.SideEffect;
			if (CheckForOverflow || (Operator == BinaryNumericOperator.Div || Operator == BinaryNumericOperator.Rem))
				flags |= InstructionFlags.MayThrow;
			return flags;
		}
		
		public override InstructionFlags DirectFlags {
			get {
				var flags = InstructionFlags.SideEffect;
				if (CheckForOverflow || (Operator == BinaryNumericOperator.Div || Operator == BinaryNumericOperator.Rem))
					flags |= InstructionFlags.MayThrow;
				return flags;
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

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			ILRange.WriteTo(output, options);
			output.Write(OpCode);
			output.Write("." + GetOperatorName(Operator));
			if (CompoundAssignmentType == CompoundAssignmentType.EvaluatesToNewValue)
				output.Write(".new");
			else
				output.Write(".old");
			if (CheckForOverflow)
				output.Write(".ovf");
			if (Sign == Sign.Unsigned)
				output.Write(".unsigned");
			else if (Sign == Sign.Signed)
				output.Write(".signed");
			output.Write('(');
			Target.WriteTo(output, options);
			output.Write(", ");
			Value.WriteTo(output, options);
			output.Write(')');
		}
	}
}


