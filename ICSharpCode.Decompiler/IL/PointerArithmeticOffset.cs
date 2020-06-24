using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Analyses the RHS of a 'ptr + int' or 'ptr - int' operation.
	/// </summary>
	struct PointerArithmeticOffset
	{
		/// <summary>
		/// Given an instruction that computes a pointer arithmetic offset in bytes,
		/// returns an instruction that computes the same offset in number of elements.
		/// 
		/// Returns null if no such instruction can be found.
		/// </summary>
		/// <param name="byteOffsetInst">Input instruction.</param>
		/// <param name="pointerElementType">The target type of the pointer type.</param>
		/// <param name="checkForOverflow">Whether the pointer arithmetic operation checks for overflow.</param>
		/// <param name="unwrapZeroExtension">Whether to allow zero extensions in the mul argument.</param>
		public static ILInstruction Detect(ILInstruction byteOffsetInst, IType pointerElementType,
			bool checkForOverflow,
			bool unwrapZeroExtension = false)
		{
			if (byteOffsetInst is Conv conv && conv.InputType == StackType.I8 && conv.ResultType == StackType.I) {
				byteOffsetInst = conv.Argument;
			}
			int? elementSize = ComputeSizeOf(pointerElementType);
			if (elementSize == 1) {
				return byteOffsetInst;
			} else if (byteOffsetInst is BinaryNumericInstruction mul && mul.Operator == BinaryNumericOperator.Mul) {
				if (mul.IsLifted)
					return null;
				if (mul.CheckForOverflow != checkForOverflow)
					return null;
				if (elementSize > 0 && mul.Right.MatchLdcI(elementSize.Value)
					|| mul.Right.UnwrapConv(ConversionKind.SignExtend) is SizeOf sizeOf && sizeOf.Type.Equals(pointerElementType)) {
					var countOffsetInst = mul.Left;
					if (unwrapZeroExtension) {
						countOffsetInst = countOffsetInst.UnwrapConv(ConversionKind.ZeroExtend);
					}
					return countOffsetInst;
				}
			} else if (byteOffsetInst.UnwrapConv(ConversionKind.SignExtend) is SizeOf sizeOf && sizeOf.Type.Equals(pointerElementType)) {
				return new LdcI4(1).WithILRange(byteOffsetInst);
			} else if (byteOffsetInst.MatchLdcI(out long val)) {
				// If the offset is a constant, it's possible that the compiler
				// constant-folded the multiplication.
				if (elementSize > 0 && (val % elementSize == 0) && val > 0) {
					val /= elementSize.Value;
					if (val <= int.MaxValue) {
						return new LdcI4((int)val).WithILRange(byteOffsetInst);
					}
				}
			}
			return null;
		}

		public static int? ComputeSizeOf(IType type)
		{
			switch (type.GetEnumUnderlyingType().GetDefinition()?.KnownTypeCode) {
				case KnownTypeCode.Boolean:
				case KnownTypeCode.SByte:
				case KnownTypeCode.Byte:
					return 1;
				case KnownTypeCode.Char:
				case KnownTypeCode.Int16:
				case KnownTypeCode.UInt16:
					return 2;
				case KnownTypeCode.Int32:
				case KnownTypeCode.UInt32:
				case KnownTypeCode.Single:
					return 4;
				case KnownTypeCode.Int64:
				case KnownTypeCode.UInt64:
				case KnownTypeCode.Double:
					return 8;
				case KnownTypeCode.Decimal:
					return 16;
			}
			return null;
		}


		/// <summary>
		/// Returns true if <c>inst</c> computes the address of a fixed variable; false if it computes the address of a moveable variable.
		/// (see "Fixed and moveable variables" in the C# specification)
		/// </summary>
		internal static bool IsFixedVariable(ILInstruction inst)
		{
			switch (inst) {
				case LdLoca ldloca:
					return ldloca.Variable.CaptureScope == null; // locals are fixed if uncaptured
				case LdFlda ldflda:
					return IsFixedVariable(ldflda.Target);
				default:
					return inst.ResultType == StackType.I;
			}
		}
	}
}
