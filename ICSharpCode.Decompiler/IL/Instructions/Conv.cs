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

using ICSharpCode.Decompiler.TypeSystem;

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
		/// Truncates toward zero; may perform overflow-checking.
		/// </summary>
		FloatToInt,
		/// <summary>
		/// Converts from the current precision available on the evaluation stack to the precision specified by
		/// the <c>TargetType</c>.
		/// Uses "round-to-nearest" mode if the precision is reduced.
		/// </summary>
		FloatPrecisionChange,
		/// <summary>
		/// Conversion of integer type to larger signed integer type.
		/// May involve overflow checking (when converting from U4 to I on 32-bit).
		/// </summary>
		SignExtend,
		/// <summary>
		/// Conversion of integer type to larger unsigned integer type.
		/// May involve overflow checking (when converting from a signed type).
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
		StopGCTracking,
		/// <summary>
		/// Used to convert unmanaged pointers to managed references.
		/// </summary>
		StartGCTracking,
		/// <summary>
		/// Converts from an object reference (O) to an interior pointer (Ref) pointing to the start of the object.
		/// </summary>
		/// <remarks>
		/// C++/CLI emits "ldarg.1; stloc.0" where arg1 is a string and loc0 is "ref byte" (e.g. as part of the PtrToStringChars codegen);
		/// we represent this type conversion explicitly in the ILAst.
		/// </remarks>
		ObjectInterior
	}

	partial class Conv : UnaryInstruction, ILiftableInstruction
	{
		/// <summary>
		/// Gets the conversion kind.
		/// </summary>
		public readonly ConversionKind Kind;

		/// <summary>
		/// Gets whether the conversion performs overflow-checking.
		/// </summary>
		public readonly bool CheckForOverflow;

		/// <summary>
		/// Gets whether this conversion is a lifted nullable conversion.
		/// </summary>
		/// <remarks>
		/// A lifted conversion expects its argument to be a value of type Nullable{T}, where
		/// T.GetStackType() == conv.InputType.
		/// If the value is non-null:
		///  * it is sign/zero-extended to InputType (based on T's sign)
		///  * the underlying conversion is performed
		///  * the result is wrapped in a Nullable{TargetType}.
		/// If the value is null, the conversion evaluates to default(TargetType?).
		/// (this result type is underspecified, since there may be multiple C# types for the TargetType)
		/// </remarks>
		public bool IsLifted { get; }

		/// <summary>
		/// Gets the stack type of the input type.
		/// </summary>
		/// <remarks>
		/// For non-lifted conversions, this is equal to <c>Argument.ResultType</c>.
		/// For lifted conversions, corresponds to the underlying type of the argument.
		/// </remarks>
		public readonly StackType InputType;

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
		public readonly Sign InputSign;

		/// <summary>
		/// The target type of the conversion.
		/// </summary>
		/// <remarks>
		/// For lifted conversions, corresponds to the underlying target type.
		/// 
		/// Target type == PrimitiveType.None can happen for implicit conversions to O in invalid IL.
		/// </remarks>
		public readonly PrimitiveType TargetType;

		public Conv(ILInstruction argument, PrimitiveType targetType, bool checkForOverflow, Sign inputSign)
			: this(argument, argument.ResultType, inputSign, targetType, checkForOverflow)
		{
		}

		public Conv(ILInstruction argument, StackType inputType, Sign inputSign, PrimitiveType targetType, bool checkForOverflow, bool isLifted = false)
			: base(OpCode.Conv, argument)
		{
			bool needsSign = checkForOverflow || (!inputType.IsFloatType() && targetType.IsFloatType());
			Debug.Assert(!(needsSign && inputSign == Sign.None));
			this.InputSign = needsSign ? inputSign : Sign.None;
			this.InputType = inputType;
			this.TargetType = targetType;
			this.CheckForOverflow = checkForOverflow;
			this.Kind = GetConversionKind(targetType, this.InputType, this.InputSign);
			// Debug.Assert(Kind != ConversionKind.Invalid); // invalid conversion can happen with invalid IL/missing references
			this.IsLifted = isLifted;
		}

		internal override void CheckInvariant(ILPhase phase)
		{
			base.CheckInvariant(phase);
			// Debug.Assert(Kind != ConversionKind.Invalid); // invalid conversion can happen with invalid IL/missing references
			Debug.Assert(Argument.ResultType == (IsLifted ? StackType.O : InputType));
			Debug.Assert(!(IsLifted && Kind == ConversionKind.StopGCTracking));
		}

		/// <summary>
		/// Implements Ecma-335 Table 8: Conversion Operators.
		/// </summary>
		static ConversionKind GetConversionKind(PrimitiveType targetType, StackType inputType, Sign inputSign)
		{
			switch (targetType)
			{
				case PrimitiveType.I1:
				case PrimitiveType.I2:
				case PrimitiveType.U1:
				case PrimitiveType.U2:
					switch (inputType)
					{
						case StackType.I4:
						case StackType.I8:
						case StackType.I:
							return ConversionKind.Truncate;
						case StackType.F4:
						case StackType.F8:
							return ConversionKind.FloatToInt;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.I4:
				case PrimitiveType.U4:
					switch (inputType)
					{
						case StackType.I4:
							return ConversionKind.Nop;
						case StackType.I:
						case StackType.I8:
							return ConversionKind.Truncate;
						case StackType.F4:
						case StackType.F8:
							return ConversionKind.FloatToInt;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.I8:
				case PrimitiveType.U8:
					switch (inputType)
					{
						case StackType.I4:
						case StackType.I:
							if (inputSign == Sign.None)
								return targetType == PrimitiveType.I8 ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
							else
								return inputSign == Sign.Signed ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
						case StackType.I8:
							return ConversionKind.Nop;
						case StackType.F4:
						case StackType.F8:
							return ConversionKind.FloatToInt;
						case StackType.Ref:
						case StackType.O:
							return ConversionKind.StopGCTracking;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.I:
				case PrimitiveType.U:
					switch (inputType)
					{
						case StackType.I4:
							if (inputSign == Sign.None)
								return targetType == PrimitiveType.I ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
							else
								return inputSign == Sign.Signed ? ConversionKind.SignExtend : ConversionKind.ZeroExtend;
						case StackType.I:
							return ConversionKind.Nop;
						case StackType.I8:
							return ConversionKind.Truncate;
						case StackType.F4:
						case StackType.F8:
							return ConversionKind.FloatToInt;
						case StackType.Ref:
						case StackType.O:
							return ConversionKind.StopGCTracking;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.R4:
					switch (inputType)
					{
						case StackType.I4:
						case StackType.I:
						case StackType.I8:
							return ConversionKind.IntToFloat;
						case StackType.F4:
							return ConversionKind.Nop;
						case StackType.F8:
							return ConversionKind.FloatPrecisionChange;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.R:
				case PrimitiveType.R8:
					switch (inputType)
					{
						case StackType.I4:
						case StackType.I:
						case StackType.I8:
							return ConversionKind.IntToFloat;
						case StackType.F4:
							return ConversionKind.FloatPrecisionChange;
						case StackType.F8:
							return ConversionKind.Nop;
						default:
							return ConversionKind.Invalid;
					}
				case PrimitiveType.Ref:
					// There's no "conv.ref" in IL, but IL allows these conversions implicitly,
					// whereas we represent them explicitly in the ILAst.
					switch (inputType)
					{
						case StackType.I4:
						case StackType.I:
						case StackType.I8:
							return ConversionKind.StartGCTracking;
						case StackType.O:
							return ConversionKind.ObjectInterior;
						default:
							return ConversionKind.Invalid;
					}
				default:
					return ConversionKind.Invalid;
			}
		}

		public override StackType ResultType {
			get => IsLifted ? StackType.O : TargetType.GetStackType();
		}

		public StackType UnderlyingResultType {
			get => TargetType.GetStackType();
		}

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write(OpCode);
			if (CheckForOverflow)
			{
				output.Write(".ovf");
			}
			if (InputSign == Sign.Unsigned)
			{
				output.Write(".unsigned");
			}
			else if (InputSign == Sign.Signed)
			{
				output.Write(".signed");
			}
			if (IsLifted)
			{
				output.Write(".lifted");
			}
			output.Write(' ');
			output.Write(InputType);
			output.Write("->");
			output.Write(TargetType);
			output.Write(' ');
			switch (Kind)
			{
				case ConversionKind.SignExtend:
					output.Write("<sign extend>");
					break;
				case ConversionKind.ZeroExtend:
					output.Write("<zero extend>");
					break;
				case ConversionKind.Invalid:
					output.Write("<invalid>");
					break;
			}
			output.Write('(');
			Argument.WriteTo(output, options);
			output.Write(')');
		}

		protected override InstructionFlags ComputeFlags()
		{
			var flags = base.ComputeFlags();
			if (CheckForOverflow)
				flags |= InstructionFlags.MayThrow;
			return flags;
		}

		public override ILInstruction UnwrapConv(ConversionKind kind)
		{
			if (this.Kind == kind && !IsLifted)
				return Argument.UnwrapConv(kind);
			else
				return this;
		}
	}
}
