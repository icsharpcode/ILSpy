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
using System.Linq.Expressions;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	public enum CompoundAssignmentType : byte
	{
		EvaluatesToOldValue,
		EvaluatesToNewValue
	}

	public abstract partial class CompoundAssignmentInstruction : ILInstruction
	{
		public readonly CompoundAssignmentType CompoundAssignmentType;

		public CompoundAssignmentInstruction(OpCode opCode, CompoundAssignmentType compoundAssignmentType, ILInstruction target, ILInstruction value)
			: base(opCode)
		{
			this.CompoundAssignmentType = compoundAssignmentType;
			this.Target = target;
			this.Value = value;
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
	}

	public partial class NumericCompoundAssign : CompoundAssignmentInstruction, ILiftableInstruction
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

		public bool IsLifted { get; }

		public NumericCompoundAssign(BinaryNumericInstruction binary, ILInstruction target, ILInstruction value, IType type, CompoundAssignmentType compoundAssignmentType)
			: base(OpCode.NumericCompoundAssign, compoundAssignmentType, target, value)
		{
			Debug.Assert(IsBinaryCompatibleWithType(binary, type));
			this.CheckForOverflow = binary.CheckForOverflow;
			this.Sign = binary.Sign;
			this.LeftInputType = binary.LeftInputType;
			this.RightInputType = binary.RightInputType;
			this.UnderlyingResultType = binary.UnderlyingResultType;
			this.Operator = binary.Operator;
			this.IsLifted = binary.IsLifted;
			this.type = type;
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
			if (type.Kind == TypeKind.Unknown) {
				return false; // avoid introducing a potentially-incorrect compound assignment
			} else if (type.Kind == TypeKind.Enum) {
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
			} else if (type.Kind == TypeKind.Pointer) {
				switch (binary.Operator) {
					case BinaryNumericOperator.Add:
					case BinaryNumericOperator.Sub:
						// ensure that the byte offset is a multiple of the pointer size
						return PointerArithmeticOffset.Detect(
							binary.Right,
							(PointerType)type,
							checkForOverflow: binary.CheckForOverflow
						) != null;
					default:
						return false; // operator not supported on pointer types
				}
			}
			if (binary.Sign != Sign.None) {
				if (type.IsCSharpSmallIntegerType()) {
					// C# will use numeric promotion to int, binary op must be signed
					if (binary.Sign != Sign.Signed)
						return false;
				} else {
					// C# will use sign from type
					if (type.GetSign() != binary.Sign)
						return false;
				}
			}
			// Can't transform if the RHS value would be need to be truncated for the LHS type.
			if (Transforms.TransformAssignment.IsImplicitTruncation(binary.Right, type, binary.IsLifted))
				return false;
			return true;
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = Target.Flags | Value.Flags | InstructionFlags.SideEffect;
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
		
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			ILRange.WriteTo(output, options);
			output.Write(OpCode);
			output.Write("." + BinaryNumericInstruction.GetOperatorName(Operator));
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

	public partial class UserDefinedCompoundAssign : CompoundAssignmentInstruction
	{
		public readonly IMethod Method;
		public bool IsLifted => false; // TODO: implement lifted user-defined compound assignments

		public UserDefinedCompoundAssign(IMethod method, CompoundAssignmentType compoundAssignmentType, ILInstruction target, ILInstruction value)
			: base(OpCode.UserDefinedCompoundAssign, compoundAssignmentType, target, value)
		{
			this.Method = method;
			Debug.Assert(Method.IsOperator || IsStringConcat(method));
			Debug.Assert(compoundAssignmentType == CompoundAssignmentType.EvaluatesToNewValue || (Method.Name == "op_Increment" || Method.Name == "op_Decrement"));
			Debug.Assert(IsValidCompoundAssignmentTarget(Target));
		}

		public static bool IsStringConcat(IMethod method)
		{
			return method.Name == "Concat" && method.IsStatic && method.DeclaringType.IsKnownType(KnownTypeCode.String);
		}

		public override StackType ResultType => Method.ReturnType.GetStackType();

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			ILRange.WriteTo(output, options);
			output.Write(OpCode);
			
			if (CompoundAssignmentType == CompoundAssignmentType.EvaluatesToNewValue)
				output.Write(".new");
			else
				output.Write(".old");
			output.Write(' ');
			Method.WriteTo(output);
			output.Write('(');
			this.Target.WriteTo(output, options);
			output.Write(", ");
			this.Value.WriteTo(output, options);
			output.Write(')');
		}
	}

	public partial class DynamicCompoundAssign : CompoundAssignmentInstruction
	{
		public ExpressionType Operation { get; }
		public CSharpArgumentInfo TargetArgumentInfo { get; }
		public CSharpArgumentInfo ValueArgumentInfo { get; }
		public CSharpBinderFlags BinderFlags { get; }

		public DynamicCompoundAssign(ExpressionType op, CSharpBinderFlags binderFlags, ILInstruction target, CSharpArgumentInfo targetArgumentInfo, ILInstruction value, CSharpArgumentInfo valueArgumentInfo)
			: base(OpCode.DynamicCompoundAssign, CompoundAssignmentTypeFromOperation(op), target, value)
		{
			if (!IsExpressionTypeSupported(op))
				throw new ArgumentOutOfRangeException("op");
			this.BinderFlags = binderFlags;
			this.Operation = op;
			this.TargetArgumentInfo = targetArgumentInfo;
			this.ValueArgumentInfo = valueArgumentInfo;
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			ILRange.WriteTo(output, options);
			output.Write(OpCode);
			output.Write("." + Operation.ToString().ToLower());
			DynamicInstruction.WriteBinderFlags(BinderFlags, output, options);
			if (CompoundAssignmentType == CompoundAssignmentType.EvaluatesToNewValue)
				output.Write(".new");
			else
				output.Write(".old");
			output.Write(' ');
			DynamicInstruction.WriteArgumentList(output, options, (Target, TargetArgumentInfo), (Value, ValueArgumentInfo));
		}

		internal static bool IsExpressionTypeSupported(ExpressionType type)
		{
			return type == ExpressionType.AddAssign
				|| type == ExpressionType.AddAssignChecked
				|| type == ExpressionType.AndAssign
				|| type == ExpressionType.DivideAssign
				|| type == ExpressionType.ExclusiveOrAssign
				|| type == ExpressionType.LeftShiftAssign
				|| type == ExpressionType.ModuloAssign
				|| type == ExpressionType.MultiplyAssign
				|| type == ExpressionType.MultiplyAssignChecked
				|| type == ExpressionType.OrAssign
				|| type == ExpressionType.PostDecrementAssign
				|| type == ExpressionType.PostIncrementAssign
				|| type == ExpressionType.PreDecrementAssign
				|| type == ExpressionType.PreIncrementAssign
				|| type == ExpressionType.RightShiftAssign
				|| type == ExpressionType.SubtractAssign
				|| type == ExpressionType.SubtractAssignChecked;
		}

		static CompoundAssignmentType CompoundAssignmentTypeFromOperation(ExpressionType op)
		{
			switch (op) {
				case ExpressionType.PostIncrementAssign:
				case ExpressionType.PostDecrementAssign:
					return CompoundAssignmentType.EvaluatesToOldValue;
				default:
					return CompoundAssignmentType.EvaluatesToNewValue;
			}
		}
	}
}


