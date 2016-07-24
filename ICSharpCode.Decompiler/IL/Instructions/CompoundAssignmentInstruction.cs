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
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;

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
		
		/// <summary>
		/// The operator used by this assignment operator instruction.
		/// </summary>
		public readonly BinaryNumericOperator Operator;
		
		public readonly CompoundAssignmentType CompoundAssignmentType;

		public CompoundAssignmentInstruction(BinaryNumericOperator op, ILInstruction target, ILInstruction value, IType type, bool checkForOverflow, Sign sign, CompoundAssignmentType compoundAssigmentType)
			: base(OpCode.CompoundAssignmentInstruction)
		{
			this.CheckForOverflow = checkForOverflow;
			this.Sign = sign;
			this.Operator = op;
			this.Target = target;
			this.type = type;
			this.Value = value;
			this.CompoundAssignmentType = compoundAssigmentType;
			Debug.Assert(compoundAssigmentType == CompoundAssignmentType.EvaluatesToNewValue || (op == BinaryNumericOperator.Add || op == BinaryNumericOperator.Sub));
			Debug.Assert(IsValidCompoundAssignmentTarget(Target));
		}
		
		internal static bool IsValidCompoundAssignmentTarget(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.LdLoc:
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

		public override void WriteTo(ITextOutput output)
		{
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
			Target.WriteTo(output);
			output.Write(", ");
			Value.WriteTo(output);
			output.Write(')');
		}
	}
}


