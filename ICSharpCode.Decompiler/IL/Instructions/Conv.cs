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

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Semantic meaning of a <c>Conv</c> instruction.
	/// </summary>
	public enum ConversionKind : byte
	{
		/// <summary>
		/// Invalid conversion.
		/// </summary>
		Invalid,
		/// <summary>
		/// Conversion between two types of same size.
		/// Can be used to change the sign of integer types, which may involve overflow-checking.
		/// </summary>
		Nop,
		/// <summary>
		/// Integer-to-float conversion.
		/// Uses <c>InputSign</c> to decide whether the integer should be treated as signed or unsigned.
		/// </summary>
		IntToFloat,
		/// <summary>
		/// Float-to-integer conversion.
		/// (truncates toward zero)
		/// </summary>
		FloatToInt,
		/// <summary>
		/// Converts from the current precision available on the evaluation stack to the precision specified by
		/// the <c>TargetType</c>.
		/// Uses "round-to-nearest" mode is the precision is reduced.
		/// </summary>
		FloatPrecisionChange,
		/// <summary>
		/// Conversion of integer type to larger signed integer type.
		/// </summary>
		SignExtend,
		/// <summary>
		/// Conversion of integer type to larger unsigned integer type.
		/// </summary>
		ZeroExtend,
		/// <summary>
		/// Conversion to smaller integer type.
		/// 
		/// May involve overflow checking.
		/// </summary>
		/// <remarks>
		/// If the target type is smaller than the minimum stack width of 4 bytes,
		/// then the result of the conversion is zero extended (if the target type is unsigned)
		/// or sign-extended (if the target type is signed).
		/// </remarks>
		Truncate,
		/// <summary>
		/// Used to convert managed references/objects to unmanaged pointers.
		/// </summary>
		StopGCTracking
	}
	
	partial class Conv : UnaryInstruction
	{
		/// <summary>
		/// Gets the conversion kind.
		/// </summary>
		public readonly ConversionKind Kind;
		
		/// <summary>
		/// The target type of the conversion.
		/// </summary>
		public readonly PrimitiveType TargetType;
		
		/// <summary>
		/// Gets whether the conversion performs overflow-checking.
		/// </summary>
		public readonly bool CheckForOverflow;
		
		/// <summary>
		/// Gets the sign of the input type.
		/// 
		/// For conversions to integer types, the input Sign is set iff overflow-checking is enabled.
		/// For conversions to floating-point types, the input sign is always set.
		/// </summary>
		/// <remarks>
		/// The input sign does not have any effect on whether the conversion zero-extends or sign-extends;
		/// that is purely determined by the <c>TargetType</c>.
		/// </remarks>
		public readonly Sign Sign;
		
		public Conv(ILInstruction argument, PrimitiveType targetType, bool checkForOverflow, Sign sign) : base(OpCode.Conv, argument)
		{
			this.TargetType = targetType;
			this.CheckForOverflow = checkForOverflow;
			this.Sign = sign;
			this.Kind = GetConversionKind(targetType, argument.ResultType);
		}

		/// <summary>
		/// Implements Ecma-335 Table 8: Conversion Operators.
		/// </summary>
		static ConversionKind GetConversionKind(PrimitiveType targetType, StackType inputType)
		{
			switch (targetType) {
				case PrimitiveType.I1:
				case PrimitiveType.I2:
				case PrimitiveType.U1:
				case PrimitiveType.U2:
					switch (inputType) {
						case StackType.I4:
						case StackType.I8:
						case StackType.I:
							return ConversionKind.Truncate;
						case StackType.F:
							return ConversionKind.FloatToInt;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.I4:
				case PrimitiveType.U4:
					switch (inputType) {
						case StackType.I4:
							return ConversionKind.Nop;
						case StackType.I:
						case StackType.I8:
							return ConversionKind.Truncate;
						case StackType.F:
							return ConversionKind.FloatToInt;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.I8:
				case PrimitiveType.U8:
					switch (inputType) {
						case StackType.I4:
						case StackType.I:
							return targetType == PrimitiveType.I8 ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
						case StackType.I8:
							return ConversionKind.Nop;
						case StackType.F:
							return ConversionKind.FloatToInt;
						case StackType.Ref:
						case StackType.O:
							return ConversionKind.StopGCTracking;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.I:
				case PrimitiveType.U:
					switch (inputType) {
						case StackType.I4:
							return targetType == PrimitiveType.I ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
						case StackType.I:
							return ConversionKind.Nop;
						case StackType.I8:
							return ConversionKind.Truncate;
						case StackType.F:
							return ConversionKind.FloatToInt;
						case StackType.Ref:
						case StackType.O:
							return ConversionKind.StopGCTracking;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.R4:
				case PrimitiveType.R8:
					switch (inputType) {
						case StackType.I4:
						case StackType.I:
						case StackType.I8:
							return ConversionKind.IntToFloat;
						case StackType.F:
							return ConversionKind.FloatPrecisionChange;
						default:
							return ConversionKind.Invalid;
					}
				default:
					return ConversionKind.Invalid;
			}
		}
		
		public override StackType ResultType {
			get { return TargetType.GetStackType(); }
		}
		
		public override void WriteTo(ITextOutput output)
		{
			output.Write(OpCode);
			if (CheckForOverflow)
				output.Write(".ovf");
			if (Sign == Sign.Unsigned)
				output.Write(".unsigned");
			else if (Sign == Sign.Signed)
				output.Write(".signed");
			output.Write(' ');
			output.Write(Argument.ResultType);
			output.Write("->");
			output.Write(TargetType);
			output.Write(' ');
			switch (Kind) {
				case ConversionKind.SignExtend:
					output.Write("<sign extend>");
					break;
				case ConversionKind.ZeroExtend:
					output.Write("<zero extend>");
					break;
			}
			output.Write('(');
			Argument.WriteTo(output);
			output.Write(')');
		}
		
		protected override InstructionFlags ComputeFlags()
		{
			var flags = base.ComputeFlags();
			if (CheckForOverflow)
				flags |= InstructionFlags.MayThrow;
			return flags;
		}
	}
}
