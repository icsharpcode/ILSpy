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
	/// No-op conversion of the expression to the adjusted target type.
	/// (i.e. nothing happens in IL, but the C# type might change)
	/// No-op adjustments can be safely ignored.
	/// </summary>
	None,
	/// <summary>
	/// Conversion of integer type to larger integer type with sign-extension.
	/// </summary>
	SignExtend,
	/// <summary>
	/// Conversion of integer type to larger integer type with zero-extension.
	/// </summary>
	ZeroExtend,
	/// <summary>
	/// Conversion of integer type to smaller integer type.
	/// </summary>
	IntegerTruncation,
	/// <summary>
	/// Special case for nullable integer type conversions of the same size
	/// (where only the nullable wrapper type changes, the underlying integer is a no-op conversion,
	///  e.g. `int?` to `uint?`)
	/// </summary>
	NullableIntegerNop,
	/// <summary>
	/// Represents discarding the expression's value (like a `(void)` cast in C).
	/// </summary>
	Voided,
	/// <summary>
	/// Represents the conversion of a pointer (any IL integer type) to a managed reference.
	/// (same as ConversionKind.StartGCTracking)
	/// I4 will be sign/zero extended (depending on source type); I8 will be truncated.
	/// </summary>
	PointerToManagedRef,
}

/// <summary>
/// Represents an adjustment applied to a C# expression.
/// See <see cref="TranslatedExpression.Adjusted"/> for how adjustments are applied to expressions.
/// </summary>
[DebuggerDisplay("{Kind} to {TargetType}")]
readonly struct Adjustment
{
	public readonly AdjustmentKind Kind;

	/// <summary>
	/// The target stack type of the adjustment.
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
	/// Creates a new adjustment with the same kind and same semantics,
	/// but a different target type.
	/// </summary>
	public Adjustment ChangeType(IType newTargetType)
	{
		Debug.Assert(newTargetType.GetStackType() == this.TargetStackType);
		return new Adjustment(this.Kind, newTargetType);
	}

	/// <summary>
	/// Determines whether this applying `prev`+`this` is equivalent to
	/// directly applying `this` to the source type.
	/// </summary>
	public bool Subsumes(Adjustment prev)
	{
		if (prev.Kind == AdjustmentKind.None)
			return true;
		return this.Kind switch {
			AdjustmentKind.ZeroExtend => prev.Kind is AdjustmentKind.ZeroExtend or AdjustmentKind.NullableIntegerNop,
			AdjustmentKind.SignExtend => prev.Kind is AdjustmentKind.SignExtend or AdjustmentKind.NullableIntegerNop,
			AdjustmentKind.IntegerTruncation => prev.Kind is AdjustmentKind.IntegerTruncation or AdjustmentKind.NullableIntegerNop,
			AdjustmentKind.PointerToManagedRef => prev.Kind is AdjustmentKind.IntegerTruncation && prev.TargetStackType == StackType.I,
			_ => false,
		};
	}
}

class AdjustmentBuilder(ExpressionBuilder exprBuilder)
{
	readonly ExpressionBuilder exprBuilder = exprBuilder;
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
		switch (adjustment.Kind)
		{
			case AdjustmentKind.None:
				return ConvertNop(expr, adjustment.TargetType);
			case AdjustmentKind.SignExtend:
			case AdjustmentKind.ZeroExtend:
			case AdjustmentKind.IntegerTruncation:
			case AdjustmentKind.NullableIntegerNop:
				return ConvertInteger(expr, adjustment.TargetType, adjustment.Kind);
			case AdjustmentKind.Voided:
				// Cannot adjust to `void` in C#. But really, no one should be calling
				// this method with the translation of a type-void ILInstruction.
				Debug.Assert(expr.Type.GetStackType() == StackType.Void);
				return expr;
			case AdjustmentKind.PointerToManagedRef:
				// Use: `ref (T*)expr`
				var brt = (ByReferenceType)adjustment.TargetType;
				expr = ConvertInteger(expr, new PointerType(brt.ElementType));
				var derefRR = new ResolveResult(brt.ElementType);
				var deref = new UnaryOperatorExpression(UnaryOperatorType.Dereference, expr)
					.WithRR(derefRR);
				return new DirectionExpression(FieldDirection.Ref, deref)
					.WithRR(new ByReferenceResolveResult(derefRR, ReferenceKind.Ref));
			default:
				throw new NotSupportedException($"Unsupported adjustment kind: {adjustment.Kind}");
		}
	}

	/// <summary>
	/// Converts `expr` to `targetType` without impacting the semantics of the expression.
	/// 
	/// Precondition: `targetType` must match the expression's existing stack type.
	/// Postcondition: The resulting expression will be equivalent to `targetType` (modulo type erasure).
	/// </summary>
	public ExpressionWithResolveResult ConvertNop(ExpressionWithResolveResult expr, IType targetType)
	{
		StackType stackType = targetType.GetStackType();
		Debug.Assert(stackType == expr.Type.GetStackType());
		if (stackType.IsIntegerType())
		{
			return ConvertInteger(expr, targetType);
		}
		if (NormalizeTypeVisitor.TypeErasure.EquivalentTypes(expr.Type, targetType))
		{
			return expr;
		}
		if (stackType == StackType.Ref)
		{
			// Convert between managed reference types.
			// We can't do this by going through a pointer type because that would temporarily stop GC tracking.
			// Instead, emit `ref Unsafe.As<T>(expr)`
			return exprBuilder.CallUnsafeIntrinsic("As", [expr],
						typeArguments: [
							((ByReferenceType)expr.Type).ElementType,
							((ByReferenceType)targetType).ElementType
						],
						returnType: targetType);
		}
		throw new NotSupportedException($"Conversion from {expr.Type} to {targetType} is not supported.");
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
	public ExpressionWithResolveResult ConvertInteger(ExpressionWithResolveResult expr, IType targetType, AdjustmentKind? kind = null)
	{
		Debug.Assert(kind is null or AdjustmentKind.SignExtend or AdjustmentKind.ZeroExtend or AdjustmentKind.IntegerTruncation);
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
				// b != null ? (b.Value ? 1 : 0) : (int?)null
				var firstCondition = new BinaryOperatorExpression(
					expr,
					BinaryOperatorType.InEquality,
					new NullReferenceExpression()
				);
				var thenPart = new ConditionalExpression(
					new MemberReferenceExpression(expr, "Value"),
					ConvertInteger(LdcI4(1), targetUType, kind),
					ConvertInteger(LdcI4(0), targetUType, kind)
				);
				return new ConditionalExpression(
					firstCondition,
					thenPart,
					new CastExpression(exprBuilder.ConvertType(targetType), new NullReferenceExpression())
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
			// Truncation to `bool` must go through `byte` first.
			var step = ConvertInteger(expr, compilation.FindType(KnownTypeCode.Byte));
			return ConvertInteger(step, targetType);
		}
		if (kind is AdjustmentKind.SignExtend or AdjustmentKind.ZeroExtend)
		{
			// In C#, sign/zero extension are differentiated based on the sign of the input type,
			// so we may need an extra cast to the correct sign before the actual extension conversion.
			Sign sign = kind == AdjustmentKind.SignExtend ? Sign.Signed : Sign.Unsigned;
			if (sourceUType.GetSign() != sign)
			{
				var stepType = compilation.FindType(sourceUType.GetStackType(), sign);
				if (isLifted)
				{
					stepType = NullableType.Create(compilation, stepType);
				}
				var step = ConvertInteger(expr, stepType);
				return ConvertInteger(step, targetType);
			}
		}
		// Now we have two types that are integer/char/enum/pointer.
		if (targetType.Kind.IsAnyPointer() && sourceType.Kind == TypeKind.Enum)
		{
			// enum to pointer: C# doesn't allow such casts
			// -> convert via enum's underlying type
			var step = ConvertInteger(expr, sourceType.GetEnumUnderlyingType(), kind);
			return ConvertInteger(step, targetType);
		}
		else if (targetUType.Kind == TypeKind.Enum && sourceType.Kind.IsAnyPointer())
		{
			// pointer to enum: C# doesn't allow such casts
			// -> convert via enum's underlying type
			var step = ConvertInteger(expr, targetUType.GetEnumUnderlyingType(), kind);
			return ConvertInteger(step, targetType);
		}
		if (targetType.Kind.IsAnyPointer() && sourceType.IsKnownType(KnownTypeCode.Char)
		   || targetUType.IsKnownType(KnownTypeCode.Char) && sourceType.Kind.IsAnyPointer())
		{
			// char <-> pointer: C# doesn't allow such casts
			// -> convert via ushort
			var step = ConvertInteger(expr, compilation.FindType(KnownTypeCode.UInt16));
			return ConvertInteger(step, targetType);
		}
		var castExpr = new CastExpression(exprBuilder.ConvertType(targetType), expr);
		bool couldOverflow = targetUType.GetSize() < sourceType.GetSize()
			|| (targetUType.GetSize() == sourceUType.GetSize() && targetUType.GetSign() != sourceUType.GetSign());
		if (couldOverflow)
		{
			if (expr.ResolveResult.IsCompileTimeConstant)
			{
				castExpr.AddAnnotation(AddCheckedBlocks.ExplicitUncheckedAnnotation);
			}
			else
			{
				castExpr.AddAnnotation(AddCheckedBlocks.UncheckedAnnotation);
			}
		}
		return castExpr.WithRR(new ConversionResolveResult(targetType, expr.ResolveResult, Conversion.ExplicitNumericConversion));
	}

	ExpressionWithResolveResult BoolToInt(ExpressionWithResolveResult expr, IType type)
	{
		Debug.Assert(expr.Type.IsKnownType(KnownTypeCode.Boolean));
		return new ConditionalExpression(
			expr,
			LdcI4(1),
			LdcI4(0)
		).WithRR(new ResolveResult(type));
	}

	ExpressionWithResolveResult LdcI4(int value)
	{
		return new PrimitiveExpression(value)
			.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), value));
	}
}
