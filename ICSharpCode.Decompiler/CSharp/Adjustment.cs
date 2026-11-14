// Copyright (c) 2026 Daniel Grunwald
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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp;

enum AdjustmentKind : byte
{
	/// <summary>
	/// No adjustment.
	/// The adjusted type is equal to the unadjusted type.
	/// </summary>
	None,
	/// <summary>
	/// Indicates that the unadjusted expression it does not have any semantics on its own,
	/// it *must* be placed in a target-typed context.
	/// </summary>
	TargetType,
	/// <summary>
	/// Conversion of integer type to larger integer type with sign-extension.
	/// Can also be used to add nullability.
	/// Examples:
	///  * C# `int` to `long`
	///  * C# `int?` to `long?`
	///  * C# `sbyte` to `int`
	///  * `bool` to `long`
	///  * `SomeEnum` to `int*`
	///  * `uint` to `ulong` but with sign-extension instead of the natural zero-extension.
	/// Not all of these are possible as direct casts in C#, but casting both
	/// both the source and target types to primitive C# integer types
	/// (with a signed type for the source type) causes an unchecked cast
	/// between those primitive C# integer types to have the same semantics
	/// as this adjustment.
	/// </summary>
	SignExtend,
	/// <summary>
	/// Conversion of integer type to larger integer type with zero-extension.
	/// Can also be used to add nullability.
	/// Examples:
	///  * C# `uint` to `ulong`
	///  * C# `uint` to `ulong?`
	///  * C# `uint?` to `ulong?`
	///  * C# `byte` to `int`
	///  * `bool` to `long`
	///  * `char` to `long`
	///  * `int` to `long` but with zero-extension instead of the natural sign-extension.
	///  * `SomeEnum` to `int*`
	/// Not all of these are possible as direct casts in C#, but casting both
	/// both the source and target types to primitive C# integer types
	/// (with an unsigned type for the source type) causes an unchecked cast
	/// between those primitive C# integer types to have the same semantics
	/// as this adjustment.
	/// </summary>
	ZeroExtend,
	/// <summary>
	/// Conversion of integer type to smaller integer type.
	/// Examples:
	///  * C# `int` to `short`
	///  * C# `int?` to `short?`
	///  * C# `long` to `nint`
	///  * C# `long` to `bool`
	/// </summary>
	IntegerTruncation,
	/// <summary>
	/// Value-preserving integer type conversion.
	/// Can also be used to add nullability.
	/// Examples:
	///  * C# `int` to `uint`
	///  * C# `int` to `int?`
	///  * C# `int?` to `uint?`
	///  * C# `short` to `char`
	/// </summary>
	IntegerCast,
	/// <summary>
	/// Represents discarding the expression's value (like a `(void)` cast in C).
	/// </summary>
	Voided,
	/// <summary>
	/// Represents the conversion of a pointer (any IL integer type) to a managed reference.
	/// (same as ConversionKind.StartGCTracking)
	/// I4 will be sign/zero extended (depending on source type); I8 will be truncated.
	/// Examples:
	///  * `int*` to `ref int`
	///  * `nint` to `ref SomeStruct`
	///  * `int` to `ref SomeStruct` (int->nint->ref SomeStruct)
	///  * `long` to `ref SomeStruct` (long->nint->ref SomeStruct)
	/// </summary>
	PointerToManagedRef,
}

/// <summary>
/// Represents an adjustment applied to a C# expression.
/// See <see cref="TranslatedExpression.ToExpression"/> for how adjustments are applied to expressions.
/// </summary>
[DebuggerDisplay("{Kind} to {TargetType}")]
readonly struct Adjustment
{
	public readonly AdjustmentKind Kind;

	/// <summary>
	/// The target stack type after the adjustment.
	/// Don't rely on this too much -- the adjustment's target type might be a small integer type!
	/// </summary>
	public readonly StackType TargetStackType;

	/// <summary>
	/// An example target type of the adjustment.
	/// Not the only possible target type -- other types might also be valid
	/// (an adjustment specifies semantic intent rather than a single concrete conversion).
	/// </summary>
	public readonly IType TargetType;

	//private readonly object details;

	public Adjustment(AdjustmentKind kind, IType targetType)
	{
		this.Kind = kind;
		this.TargetType = targetType;
		this.TargetStackType = targetType.GetStackType();
	}

	/// <summary>
	/// Determines whether two types can be considered equivalent for the purposes of acting as the adjustment target type.
	/// </summary>
	internal static bool CompatibleTypesForAdjustmentSemantics(IType type1, IType type2)
	{
		StackType stackType1 = type1.GetStackType();
		StackType stackType2 = type2.GetStackType();
		if (stackType1 == StackType.VT && stackType2 == StackType.VT)
		{
			if (NullableType.IsNullable(type1) && NullableType.IsNullable(type2))
			{
				var utype1 = NullableType.GetUnderlyingType(type1);
				var utype2 = NullableType.GetUnderlyingType(type2);
				// nullable integer types are flexible like non-nullable integer types
				if (utype1.GetStackType().IsIntegerType() && utype2.GetStackType().IsIntegerType())
				{
					return utype1.GetSize() == utype2.GetSize();
				}
			}
			return NormalizeTypeVisitor.TypeErasure.EquivalentTypes(type1, type2);
		}
		if (stackType1 == StackType.I4 && stackType2 == StackType.I4)
		{
			return type1.GetSize() == type2.GetSize();
		}
		return stackType1 == stackType2;
	}

	/// <summary>
	/// Determines whether this applying `prev`+`this` is equivalent to
	/// directly applying `this` to the source type.
	/// 
	/// If no source type is provided, returns true only if this equivalence
	/// holds for all possible source types.
	/// </summary>
	public bool Subsumes(Adjustment prev)
	{
		if (prev.Kind == AdjustmentKind.None)
			return true;
		return this.Kind switch {
			AdjustmentKind.ZeroExtend => prev.Kind is AdjustmentKind.ZeroExtend or AdjustmentKind.IntegerCast,
			AdjustmentKind.SignExtend => prev.Kind is AdjustmentKind.SignExtend or AdjustmentKind.IntegerCast,
			AdjustmentKind.IntegerTruncation => prev.Kind is AdjustmentKind.IntegerTruncation or AdjustmentKind.IntegerCast,
			AdjustmentKind.IntegerCast => prev.Kind is AdjustmentKind.IntegerCast,
			AdjustmentKind.PointerToManagedRef => prev.Kind is AdjustmentKind.IntegerCast || (prev.Kind is AdjustmentKind.IntegerTruncation && prev.TargetStackType == StackType.I),
			_ => false,
		};
	}

	/// <summary>
	/// Checks that this is a valid adjustment for the given source type.
	/// </summary>
	[Conditional("DEBUG")]
	internal void CheckInvariant(IType sourceType)
	{
		switch (Kind)
		{
			case AdjustmentKind.None:
				Debug.Assert(sourceType.Equals(this.TargetType));
				break;
			case AdjustmentKind.TargetType:
				Debug.Assert(sourceType.Kind is TypeKind.None or TypeKind.Null);
				break;
			case AdjustmentKind.SignExtend:
			case AdjustmentKind.ZeroExtend:
			case AdjustmentKind.IntegerCast:
			case AdjustmentKind.IntegerTruncation:
				var sourceUType = NullableType.GetUnderlyingType(sourceType);
				var targetUType = NullableType.GetUnderlyingType(this.TargetType);
				Debug.Assert(sourceUType.GetStackType().IsIntegerType());
				Debug.Assert(targetUType.GetStackType().IsIntegerType());
				if (Kind == AdjustmentKind.IntegerTruncation)
				{
					Debug.Assert(sourceUType.GetSize() > targetUType.GetSize());
				}
				else if (Kind == AdjustmentKind.IntegerCast)
				{
					Debug.Assert(sourceUType.GetSize() == targetUType.GetSize());
				}
				else // zero/sign extend 
				{
					Debug.Assert(sourceUType.GetSize() < targetUType.GetSize());
				}
				break;
			case AdjustmentKind.Voided:
				Debug.Assert(this.TargetType.IsKnownType(KnownTypeCode.Void));
				break;
			case AdjustmentKind.PointerToManagedRef:
				Debug.Assert(sourceType.GetStackType().IsIntegerType());
				Debug.Assert(this.TargetType is ByReferenceType);
				break;
		}
	}
}

class AdjustmentBuilder(ExpressionBuilder exprBuilder)
{
	internal readonly ExpressionBuilder exprBuilder = exprBuilder;
	internal readonly ICompilation compilation = exprBuilder.compilation;

	public readonly Adjustment Voided = new(AdjustmentKind.Voided, exprBuilder.compilation.FindType(KnownTypeCode.Void));

	/// <summary>
	/// Turns the adjustment into an explicit cast to `targetType`.
	/// 
	/// Precondition: `targetType` must match this adjustment's target stack type.
	/// Postcondition: The resulting expression will have type `targetType`.
	/// </summary>
	public ExpressionWithResolveResult ApplyAdjustment(ExpressionWithResolveResult expr, Adjustment adjustment)
	{
		adjustment.CheckInvariant(expr.ResolveResult.Type);
		switch (adjustment.Kind)
		{
			case AdjustmentKind.None:
				return expr;
			case AdjustmentKind.TargetType:
				return new CastExpression(exprBuilder.ConvertType(adjustment.TargetType), expr)
					.WithRR(new ResolveResult(adjustment.TargetType));
			case AdjustmentKind.SignExtend:
			case AdjustmentKind.ZeroExtend:
			case AdjustmentKind.IntegerTruncation:
			case AdjustmentKind.IntegerCast:
				return ConvertInteger(expr, adjustment.TargetType, adjustment.Kind);
			case AdjustmentKind.Voided:
				// Cannot adjust to `void` in C#. But really, no one should be calling
				// this method with the translation of a type-void ILInstruction.
				Debug.Assert(expr.Type.GetStackType() == StackType.Void);
				return expr;
			case AdjustmentKind.PointerToManagedRef:
				// Use: `ref *(T*)expr`
				var brt = (ByReferenceType)adjustment.TargetType.SkipModifiers();
				expr = ConvertInteger(expr, new PointerType(brt.ElementType));
				var derefRR = new ResolveResult(brt.ElementType);
				Expression deref;
				if (expr.ResolveResult is MemberResolveResult { Member: IField field }
					&& CSharpDecompiler.IsFixedField(field, out _, out _))
				{
					// For fixed fields, it's better to use `field[0]` instead of `*field`.
					deref = new IndexerExpression(expr, [new PrimitiveExpression(0)]);
				}
				else
				{
					deref = new UnaryOperatorExpression(UnaryOperatorType.Dereference, expr);
				}
				return new DirectionExpression(FieldDirection.Ref, deref.WithRR(derefRR))
					.WithRR(new ByReferenceResolveResult(derefRR, ReferenceKind.Ref));
			default:
				throw new NotSupportedException($"Unsupported adjustment kind: {adjustment.Kind}");
		}
	}

	/// <summary>
	/// Determines the appropriate integer adjustment kind when converting from `sourceType` to a target
	/// integer type of the specified size.
	/// Zero/sign-extension are differentiated based on the source type's sign.
	/// </summary>
	public static AdjustmentKind DetermineIntegerAdjustment(IType sourceType, int targetSize)
	{
		Debug.Assert(sourceType.GetStackType().IsIntegerType());
		Debug.Assert(targetSize > 0);
		int sourceSize = sourceType.GetSize();
		if (targetSize > sourceSize)
		{
			return sourceType.GetSign() == Sign.Signed ? AdjustmentKind.SignExtend : AdjustmentKind.ZeroExtend;
		}
		else if (targetSize < sourceSize)
		{
			return AdjustmentKind.IntegerTruncation;
		}
		else
		{
			return AdjustmentKind.IntegerCast;
		}
	}

	/// <summary>
	/// Converts `expr` to `targetType`.
	/// Semantically, this is one of:
	/// - Sign extension
	/// - Zero extension
	/// - No-op conversion
	/// - Truncation
	/// `kind` can be used to explicitly pick one of these options; but it it's not provided, the method will
	/// choose the appropriate conversion based on the types involved
	/// (sign/zero extension will be differentiated based on the source type when `kind` is not given).
	/// 
	/// Additionally, this can have the semantics of converting a non-nullable integer to a nullable integer;
	/// or converting between two nullable integer types.
	/// 
	/// Precondition: `expr.Type` and `targetType` must be both IL integer types.
	/// If the input type is nullable, then so must be the target type.
	/// Postcondition: The resulting expression will have type `targetType`.
	/// </summary>
	public ExpressionWithResolveResult ConvertInteger(ExpressionWithResolveResult expr, IType targetType,
		AdjustmentKind? kind = null, bool checkOverflow = false)
	{
		Debug.Assert(kind is null or AdjustmentKind.SignExtend or AdjustmentKind.ZeroExtend
								  or AdjustmentKind.IntegerTruncation or AdjustmentKind.IntegerCast);
		var sourceType = expr.Type;
		if (targetType.Equals(sourceType))
		{
			return expr;
		}
		bool isLifted = NullableType.IsNullable(sourceType);
		var sourceUType = NullableType.GetUnderlyingType(sourceType);
		var targetUType = NullableType.GetUnderlyingType(targetType);
		Debug.Assert(sourceUType.GetStackType().IsIntegerType());
		Debug.Assert(targetUType.GetStackType().IsIntegerType());
		if (sourceUType.IsKnownType(KnownTypeCode.Boolean) && !targetUType.IsKnownType(KnownTypeCode.Boolean))
		{
			// C# does not allow casting bool to integer types directly.
			if (isLifted)
			{
				// I'm not aware of any case where this case can be reached, but to be safe we handle it.
				// We need switch-expressions to avoid repeatedly evaluating `expr`.
				// `expr switch { null => (int?)null, true => 1, false => 0 }`
				return new SwitchExpression(
					expr,
					new SwitchExpressionSection(
						new NullReferenceExpression(),
						new CastExpression(exprBuilder.ConvertType(targetType), new NullReferenceExpression())
					),
					new SwitchExpressionSection(
						new PrimitiveExpression(true),
						ConvertInteger(LdcI4(1), targetType, kind)
					),
					new SwitchExpressionSection(
						new PrimitiveExpression(false),
						ConvertInteger(LdcI4(0), targetType, kind)
					)
				).WithRR(new ResolveResult(targetType));
			}
			else
			{
				// b ? 1 : 0
				return new ConditionalExpression(
					expr,
					ConvertInteger(LdcI4(1), targetType, kind),
					ConvertInteger(LdcI4(0), targetType, kind)
				).WithRR(new ResolveResult(targetType));
			}
		}
		if (targetUType.IsKnownType(KnownTypeCode.Boolean))
		{
			if (sourceUType.GetSize() > 1 || (checkOverflow && sourceUType.GetSign() == Sign.Signed))
			{
				// Truncation to `bool` must go through `byte` first.
				// Same if sign change requires overflow checking.
				var step = ConvertInteger(expr, GetIntegerType(1, Sign.Unsigned, isLifted), checkOverflow: checkOverflow);
				return ConvertInteger(step, targetType);
			}
			else
			{
				// Source is already 1 byte, check for `!= 0`
				if (isLifted)
				{
					// I'm not aware of any case where this case can be reached, but to be safe we handle it.
					// We need switch-expressions to avoid repeatedly evaluating `expr`.
					// expr switch { null => (bool?)null, 0 => false, _ => true }
					return new SwitchExpression(
						expr,
						new SwitchExpressionSection(
							new NullReferenceExpression(),
							new CastExpression(exprBuilder.ConvertType(targetType), new NullReferenceExpression())
						),
						new SwitchExpressionSection(
							ConvertInteger(LdcI4(0), sourceUType),
							new PrimitiveExpression(false)
						),
						new SwitchExpressionSection(
							new IdentifierExpression("_"),
							new PrimitiveExpression(true)
						)
					).WithRR(new ResolveResult(targetType));
				}
				else
				{
					// expr != 0
					return new BinaryOperatorExpression(
						expr,
						BinaryOperatorType.InEquality,
						LdcI4(0)
					).WithRR(new ResolveResult(targetType));
				}
			}
		}
		if (kind is AdjustmentKind.SignExtend or AdjustmentKind.ZeroExtend)
		{
			// In C#, sign/zero extension are differentiated based on the sign of the input type,
			// so we may need an extra cast to the correct sign before the actual extension conversion.
			Sign sign = kind == AdjustmentKind.SignExtend ? Sign.Signed : Sign.Unsigned;
			if (sourceUType.GetSign() != sign)
			{
				var convertVia = GetIntegerType(sourceUType.GetSize(), sign, isLifted);
				var step = ConvertInteger(expr, convertVia, checkOverflow: checkOverflow);
				return ConvertInteger(step, targetType, checkOverflow: checkOverflow);
			}
		}
		// Special-case IntPtr and UIntPtr: they behave extremely weird, see IntPtr.txt for details.
		if (sourceType.IsKnownType(KnownTypeCode.IntPtr))
		{
			// Conversion from IntPtr
			// Direct cast only works correctly for IntPtr -> long.
			// IntPtr -> int works correctly only in checked context.
			// IntPtr -> void* works but never checks for overflow.
			// Everything else can be worked around by casting via nint/long.
			if (!(targetType.IsKnownType(KnownTypeCode.Int64) || targetType.Kind == TypeKind.NInt
				|| (checkOverflow && targetType.IsKnownType(KnownTypeCode.Int32))
				|| (!checkOverflow && targetType.Kind.IsAnyPointer())))
			{
				var convertVia = exprBuilder.settings.NativeIntegers ? SpecialType.NInt : compilation.FindType(KnownTypeCode.Int64);
				var step = ConvertInteger(expr, convertVia, checkOverflow: checkOverflow);
				return ConvertInteger(step, targetType, checkOverflow: checkOverflow);
			}
		}
		else if (sourceType.IsKnownType(KnownTypeCode.UIntPtr))
		{
			// Conversion from UIntPtr
			// Direct cast only works correctly for UIntPtr -> ulong.
			// UIntPtr -> uint works correctly only in checked context.
			// UIntPtr -> void* works and overflow cannot happen as the sign matches.
			// Everything else can be worked around by casting via ulong.
			if (!(targetType.IsKnownType(KnownTypeCode.UInt64) || targetType.Kind == TypeKind.NUInt
				|| (checkOverflow && targetType.IsKnownType(KnownTypeCode.UInt32))
				|| targetType.Kind.IsAnyPointer()))
			{
				var convertVia = exprBuilder.settings.NativeIntegers ? SpecialType.NUInt : compilation.FindType(KnownTypeCode.UInt64);
				var step = ConvertInteger(expr, convertVia, checkOverflow: checkOverflow);
				return ConvertInteger(step, targetType, checkOverflow: checkOverflow);
			}
		}
		if (targetUType.IsKnownType(KnownTypeCode.IntPtr) || targetUType.IsKnownType(KnownTypeCode.UIntPtr))
		{
			// Conversion to (U)IntPtr
			IType? convertVia;
			if (!CouldOverflow(sourceUType, targetUType))
			{
				// Any cast that never overflows can be done directly in both contexts.
				convertVia = null;
			}
			else if (checkOverflow)
			{
				// long -> IntPtr: always performs overflow check, so we can use it here
				// ulong -> UIntPtr: always performs overflow check, so we can use it here
				var workingType = targetUType.IsKnownType(KnownTypeCode.IntPtr) ? KnownTypeCode.Int64 : KnownTypeCode.UInt64;
				if (sourceUType.IsKnownType(workingType))
				{
					convertVia = null;
				}
				else
				{
					// anything else: convert via nint/long or nuint/ulong
					convertVia = compilation.FindType(workingType);
				}
			}
			else
			{
				// The only way to truncate to native size without performing an overflow check
				// in 32-bit mode is to use a pointer sourceType.
				if (sourceType.Kind.IsAnyPointer())
				{
					convertVia = null;
				}
				else
				{
					convertVia = new PointerType(compilation.FindType(KnownTypeCode.Void));
				}
			}
			if (convertVia != null)
			{
				// If nint is available, prefer using that.
				if (exprBuilder.settings.NativeIntegers)
				{
					convertVia = targetUType.IsKnownType(KnownTypeCode.IntPtr) ? SpecialType.NInt : SpecialType.NUInt;
				}
				var step = ConvertInteger(expr, convertVia, checkOverflow: checkOverflow);
				return ConvertInteger(step, targetType, checkOverflow: checkOverflow);
			}
		}
		// Now we have two types that are integer/char/enum/pointer.
		if (targetType.Kind.IsAnyPointer() && sourceType.Kind == TypeKind.Enum)
		{
			// enum to pointer: C# doesn't allow such casts
			// -> convert via enum's underlying type
			var step = ConvertInteger(expr, sourceType.GetEnumUnderlyingType());
			return ConvertInteger(step, targetType, kind, checkOverflow);
		}
		else if (targetUType.Kind == TypeKind.Enum && sourceType.Kind.IsAnyPointer())
		{
			// pointer to enum: C# doesn't allow such casts
			// -> convert via enum's underlying type
			var step = ConvertInteger(expr, targetUType.GetEnumUnderlyingType(), kind, checkOverflow);
			return ConvertInteger(step, targetType);
		}
		if (targetType.Kind.IsAnyPointer() && sourceType.IsKnownType(KnownTypeCode.Char)
		   || targetUType.IsKnownType(KnownTypeCode.Char) && sourceType.Kind.IsAnyPointer())
		{
			// char <-> pointer: C# doesn't allow such casts
			// -> convert via ushort
			var step = ConvertInteger(expr, compilation.FindType(KnownTypeCode.UInt16), kind, checkOverflow);
			return ConvertInteger(step, targetType, kind, checkOverflow);
		}
		var castExpr = new CastExpression(exprBuilder.ConvertType(targetType), expr);
		if (expr.ResolveResult.IsCompileTimeConstant)
		{
			// We can check concretely if there's an overflow:
			var castRR = exprBuilder.resolver.WithCheckForOverflow(true).ResolveCast(targetType, expr.ResolveResult);
			if (castRR.IsCompileTimeConstant && !castRR.IsError)
			{
				// There was no overflow, so we could safely use the cast expression as-is.
			}
			else if (checkOverflow)
			{
				// Definite overflow at compile-time :/
				castExpr.AddAnnotation(AddCheckedBlocks.CheckedAnnotation);
			}
			else
			{
				// We need to propagate the constant value:
				castRR = exprBuilder.resolver.WithCheckForOverflow(false).ResolveCast(targetType, expr.ResolveResult);
				castExpr.AddAnnotation(AddCheckedBlocks.ExplicitUncheckedAnnotation);
			}
			if (castRR.IsCompileTimeConstant && !castRR.IsError)
			{
				// We can re-emit the constant, which gives cleaner code
				// (e.g. `unchecked((uint)(-1)))` turns into `uint.MaxValue`)
				return exprBuilder.ConvertConstantValue(castRR).WithILInstruction(expr.Expression.Annotations.OfType<ILInstruction>());
			}
			return castExpr.WithRR(castRR);
		}
		else
		{
			if (CouldOverflow(sourceUType, targetUType))
			{
				// Might overflow at run-time, so ensure it appears in the correct checked/unchecked context.
				castExpr.AddAnnotation(checkOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			}
			return castExpr.WithRR(new ResolveResult(targetType));
		}
	}

	// Whether a C# cast between two integer types could overflow.
	static bool CouldOverflow(IType sourceUType, IType targetUType)
	{
		int sourceSize = sourceUType.GetSize();
		int targetSize = targetUType.GetSize();
		if (sourceSize > targetSize)
			return true; // truncation: overflow definitely possible
		if (sourceUType.GetSign() == targetUType.GetSign())
		{
			Debug.Assert(sourceSize <= targetSize);
			return false; // same sign, larger or equal size: no overflow possible
		}
		else
		{
			if (sourceUType.GetSign() == Sign.Signed)
			{
				return true; // signed -> unsigned overflows for negative values
			}
			// unsigned -> signed can overflow only if the types have the same size.
			return targetSize == sourceSize
				|| (targetSize == TypeUtils.NativeIntSize && sourceSize >= 4)
				|| (sourceSize == TypeUtils.NativeIntSize && targetSize >= 4);
		}
	}

	public IType GetIntegerType(int size, Sign sign, bool isLifted = false, IType? typeHint = null)
	{
		IType type;
		if (typeHint != null)
		{
			typeHint = NullableType.GetUnderlyingType(typeHint);
		}
		if (typeHint != null && typeHint.GetSize() == size && (sign == Sign.None || typeHint.GetSign() == sign))
		{
			type = typeHint;
		}
		else if (size == TypeUtils.NativeIntSize && exprBuilder.settings.NativeIntegers)
		{
			type = sign == Sign.Unsigned ? SpecialType.NUInt : SpecialType.NInt;
		}
		else
		{
			type = compilation.FindType(TypeUtils.GetIntegerTypeCode(size, sign));
		}
		if (isLifted)
		{
			type = NullableType.Create(compilation, type);
		}
		return type;
	}

	ExpressionWithResolveResult LdcI4(int value)
	{
		return new PrimitiveExpression(value)
			.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), value));
	}
}
