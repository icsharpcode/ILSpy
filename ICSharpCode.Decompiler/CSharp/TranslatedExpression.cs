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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Helper struct so that the compiler can ensure we don't forget both the ILInstruction annotation and the ResolveResult annotation.
	/// Use '.WithILInstruction(...)' or '.WithoutILInstruction()' to create an instance of this struct.
	/// </summary>
	struct ExpressionWithILInstruction
	{
		public readonly Expression Expression;

		public IEnumerable<ILInstruction> ILInstructions {
			get { return Expression.Annotations.OfType<ILInstruction>(); }
		}

		internal ExpressionWithILInstruction(Expression expression)
		{
			Debug.Assert(expression != null);
			this.Expression = expression;
		}

		public static implicit operator Expression(ExpressionWithILInstruction expression)
		{
			return expression.Expression;
		}
	}

	/// <summary>
	/// Helper struct so that the compiler can ensure we don't forget both the ILInstruction annotation and the ResolveResult annotation.
	/// Use '.WithRR(...)'.
	/// </summary>
	struct ExpressionWithResolveResult
	{
		public readonly Expression Expression;

		// Because ResolveResult is frequently accessed within the ExpressionBuilder, we put it directly
		// in this struct instead of accessing it through the list of annotations.
		public readonly ResolveResult ResolveResult;

		public IType Type {
			get { return ResolveResult.Type; }
		}

		internal ExpressionWithResolveResult(Expression expression)
		{
			Debug.Assert(expression != null);
			this.Expression = expression;
			this.ResolveResult = expression.Annotation<ResolveResult>() ?? ErrorResolveResult.UnknownError;
		}

		internal ExpressionWithResolveResult(Expression expression, ResolveResult resolveResult)
		{
			Debug.Assert(expression != null && resolveResult != null);
			Debug.Assert(expression.Annotation<ResolveResult>() == resolveResult);
			this.Expression = expression;
			this.ResolveResult = resolveResult;
		}

		public static implicit operator Expression(ExpressionWithResolveResult expression)
		{
			return expression.Expression;
		}
	}

	/// <summary>
	/// Output of C# ExpressionBuilder -- a decompiled C# expression that has both a resolve result and ILInstruction annotation.
	/// </summary>
	/// <remarks>
	/// The resolve result is also always available as annotation on the expression, but having
	/// TranslatedExpression as a separate type is still useful to ensure that no case in the expression builder
	/// forgets to add the annotation.
	/// </remarks>
	[DebuggerDisplay("{Expression} : {ResolveResult}")]
	struct TranslatedExpression
	{
		public readonly Expression Expression;

		// Because ResolveResult is frequently accessed within the ExpressionBuilder, we put it directly
		// in this struct instead of accessing it through the list of annotations.
		public readonly ResolveResult ResolveResult;

		public IEnumerable<ILInstruction> ILInstructions {
			get { return Expression.Annotations.OfType<ILInstruction>(); }
		}

		public IType Type {
			get { return ResolveResult.Type; }
		}

		internal TranslatedExpression(Expression expression)
		{
			Debug.Assert(expression != null);
			this.Expression = expression;
			this.ResolveResult = expression.Annotation<ResolveResult>() ?? ErrorResolveResult.UnknownError;
		}

		internal TranslatedExpression(Expression expression, ResolveResult resolveResult)
		{
			Debug.Assert(expression != null && resolveResult != null);
			Debug.Assert(expression.Annotation<ResolveResult>() == resolveResult);
			this.ResolveResult = resolveResult;
			this.Expression = expression;
		}

		public static implicit operator Expression(TranslatedExpression expression)
		{
			return expression.Expression;
		}

		public static implicit operator ExpressionWithResolveResult(TranslatedExpression expression)
		{
			return new ExpressionWithResolveResult(expression.Expression, expression.ResolveResult);
		}

		public static implicit operator ExpressionWithILInstruction(TranslatedExpression expression)
		{
			return new ExpressionWithILInstruction(expression.Expression);
		}

		/// <summary>
		/// Returns a new TranslatedExpression that represents the specified descendant expression.
		/// All ILInstruction annotations from the current expression are copied to the descendant expression.
		/// The descendant expression is detached from the AST.
		/// </summary>
		public TranslatedExpression UnwrapChild(Expression descendant)
		{
			if (descendant == Expression)
				return this;
			for (AstNode parent = descendant.Parent; parent != null; parent = parent.Parent)
			{
				foreach (var inst in parent.Annotations.OfType<ILInstruction>())
					descendant.AddAnnotation(inst);
				if (parent == Expression)
					return new TranslatedExpression(descendant.Detach());
			}
			throw new ArgumentException("descendant must be a descendant of the current node");
		}

		/// <summary>
		/// Adds casts (if necessary) to convert this expression to the specified target type.
		/// </summary>
		/// <remarks>
		/// If the target type is narrower than the source type, the value is truncated.
		/// If the target type is wider than the source type, the value is sign- or zero-extended based on the
		/// sign of the source type.
		/// This fits with the ExpressionBuilder's post-condition, so e.g. an assignment can simply
		/// call <c>Translate(stloc.Value).ConvertTo(stloc.Variable.Type)</c> and have the overall C# semantics match the IL semantics.
		/// 
		/// From the caller's perspective, IntPtr/UIntPtr behave like normal C# integers except that they have native int size.
		/// All the special cases necessary to make IntPtr/UIntPtr behave sanely are handled internally in ConvertTo().
		/// 
		/// Post-condition:
		///    The "expected evaluation result" is the value computed by <c>this.Expression</c>,
		///    converted to targetType via an IL conv instruction.
		/// 
		///    ConvertTo(targetType, allowImplicitConversion=false).Type must be equal to targetType (modulo identity conversions).
		///      The value computed by the converted expression must match the "expected evaluation result".
		/// 
		///    ConvertTo(targetType, allowImplicitConversion=true) must produce an expression that,
		///      when evaluated in a context where it will be implicitly converted to targetType,
		///      evaluates to the "expected evaluation result".
		/// </remarks>
		public TranslatedExpression ConvertTo(IType targetType, ExpressionBuilder expressionBuilder, bool checkForOverflow = false, bool allowImplicitConversion = false)
		{
			var type = this.Type;
			if (type.Equals(targetType))
			{
				// Make explicit conversion implicit, if possible
				if (allowImplicitConversion)
				{
					switch (ResolveResult)
					{
						case ConversionResolveResult conversion:
						{
							if (Expression is CastExpression cast && CastCanBeMadeImplicit(
									Resolver.CSharpConversions.Get(expressionBuilder.compilation),
									conversion.Conversion,
									conversion.Input.Type,
									type, targetType
								))
							{
								var result = this.UnwrapChild(cast.Expression);
								if (conversion.Conversion.IsUserDefined)
								{
									result.Expression.AddAnnotation(new ImplicitConversionAnnotation(conversion));
								}
								return result;
							}
							else if (Expression is ObjectCreateExpression oce && conversion.Conversion.IsMethodGroupConversion
								  && oce.Arguments.Count == 1 && expressionBuilder.settings.UseImplicitMethodGroupConversion)
							{
								return this.UnwrapChild(oce.Arguments.Single());
							}
							break;
						}
						case InvocationResolveResult invocation:
						{
							if (Expression is ObjectCreateExpression oce && oce.Arguments.Count == 1 && invocation.Type.IsKnownType(KnownTypeCode.NullableOfT))
							{
								return this.UnwrapChild(oce.Arguments.Single());
							}
							break;
						}
					}
				}
				return this;
			}
			if (targetType.Kind == TypeKind.Void || targetType.Kind == TypeKind.None)
			{
				return this; // don't attempt to insert cast to '?' or 'void' as these are not valid.
			}
			else if (targetType.Kind == TypeKind.Unknown)
			{
				// don't attempt cast to '?', or casts between an unknown type and a known type with same name
				if (targetType.Name == "?" || targetType.ReflectionName == type.ReflectionName)
				{
					return this;
				}
				// However we still want explicit casts to types that are merely unresolved
			}
			var convAnnotation = this.Expression.Annotation<ImplicitConversionAnnotation>();
			if (convAnnotation != null)
			{
				// If an implicit user-defined conversion was stripped from this expression;
				// it needs to be re-introduced before we can apply other casts to this expression.
				// This happens when the CallBuilder discovers that the conversion is necessary in
				// order to choose the correct overload.
				this.Expression.RemoveAnnotations<ImplicitConversionAnnotation>();
				return new CastExpression(expressionBuilder.ConvertType(convAnnotation.TargetType), Expression)
					.WithoutILInstruction()
					.WithRR(convAnnotation.ConversionResolveResult)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
			}
			if (Expression is ThrowExpression && allowImplicitConversion)
			{
				return this; // Throw expressions have no type and are implicitly convertible to any type
			}
			if (Expression is TupleExpression tupleExpr && targetType is TupleType targetTupleType
				&& tupleExpr.Elements.Count == targetTupleType.ElementTypes.Length)
			{
				// Conversion of a tuple literal: convert element-wise
				var newTupleExpr = new TupleExpression();
				var newElementRRs = new List<ResolveResult>();
				foreach (var (elementExpr, elementTargetType) in tupleExpr.Elements.Zip(targetTupleType.ElementTypes))
				{
					var newElementExpr = new TranslatedExpression(elementExpr.Detach())
						.ConvertTo(elementTargetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
					newTupleExpr.Elements.Add(newElementExpr.Expression);
					newElementRRs.Add(newElementExpr.ResolveResult);
				}
				return newTupleExpr.WithILInstruction(this.ILInstructions)
					.WithRR(new TupleResolveResult(
						expressionBuilder.compilation, newElementRRs.ToImmutableArray(),
						valueTupleAssembly: targetTupleType.GetDefinition()?.ParentModule
					));
			}
			var compilation = expressionBuilder.compilation;
			var conversions = Resolver.CSharpConversions.Get(compilation);
			if (ResolveResult is ConversionResolveResult conv && Expression is CastExpression cast2
				&& !conv.Conversion.IsUserDefined
				&& CastCanBeMadeImplicit(conversions, conv.Conversion, conv.Input.Type, type, targetType))
			{
				var unwrapped = this.UnwrapChild(cast2.Expression);
				if (allowImplicitConversion)
					return unwrapped;
				return unwrapped.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
			}
			if (Expression is UnaryOperatorExpression uoe && uoe.Operator == UnaryOperatorType.NullConditional && targetType.IsReferenceType == true)
			{
				// "(T)(x?).AccessChain" is invalid, but "((T)x)?.AccessChain" is valid and equivalent
				return new UnaryOperatorExpression(
					UnaryOperatorType.NullConditional,
					UnwrapChild(uoe.Expression).ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion)
				).WithRR(new ResolveResult(targetType)).WithoutILInstruction();
			}
			IType utype = NullableType.GetUnderlyingType(type);
			IType targetUType = NullableType.GetUnderlyingType(targetType);
			if (type.IsKnownType(KnownTypeCode.Boolean) && !targetUType.IsKnownType(KnownTypeCode.Boolean)
				&& targetUType.GetStackType().IsIntegerType())
			{
				// convert from boolean to integer (or enum)
				return new ConditionalExpression(
					this.Expression,
					LdcI4(compilation, 1).ConvertTo(targetType, expressionBuilder, checkForOverflow),
					LdcI4(compilation, 0).ConvertTo(targetType, expressionBuilder, checkForOverflow)
				).WithoutILInstruction().WithRR(new ResolveResult(targetType));
			}
			if (targetType.IsKnownType(KnownTypeCode.Boolean))
			{
				// convert to boolean through byte, to simulate the truncation to 8 bits
				return this.ConvertTo(compilation.FindType(KnownTypeCode.Byte), expressionBuilder, checkForOverflow)
					.ConvertToBoolean(expressionBuilder);
			}

			// Special-case IntPtr and UIntPtr: they behave extremely weird, see IntPtr.txt for details.
			if (type.IsKnownType(KnownTypeCode.IntPtr))
			{ // Conversion from IntPtr
			  // Direct cast only works correctly for IntPtr -> long.
			  // IntPtr -> int works correctly only in checked context.
			  // Everything else can be worked around by casting via long.
				if (!(targetType.IsKnownType(KnownTypeCode.Int64) || targetType.Kind == TypeKind.NInt
					|| (checkForOverflow && targetType.IsKnownType(KnownTypeCode.Int32))
					|| targetType.Kind.IsAnyPointer() || targetType.Kind == TypeKind.ByReference))
				{
					var convertVia = expressionBuilder.settings.NativeIntegers ? SpecialType.NInt : compilation.FindType(KnownTypeCode.Int64);
					return this.ConvertTo(convertVia, expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
				}
			}
			else if (type.IsKnownType(KnownTypeCode.UIntPtr))
			{ // Conversion from UIntPtr
			  // Direct cast only works correctly for UIntPtr -> ulong.
			  // UIntPtr -> uint works correctly only in checked context.
			  // Everything else can be worked around by casting via ulong.
				if (!(targetType.IsKnownType(KnownTypeCode.UInt64) || targetType.Kind == TypeKind.NUInt
					|| (checkForOverflow && targetType.IsKnownType(KnownTypeCode.UInt32))
					|| targetType.Kind.IsAnyPointer() || targetType.Kind == TypeKind.ByReference))
				{
					var convertVia = expressionBuilder.settings.NativeIntegers ? SpecialType.NUInt : compilation.FindType(KnownTypeCode.UInt64);
					return this.ConvertTo(convertVia, expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
				}
			}
			if (targetUType.IsKnownType(KnownTypeCode.IntPtr) && utype.GetStackType().IsIntegerType())
			{ // Conversion to IntPtr
				if (type.IsKnownType(KnownTypeCode.Int32) || type.Kind == TypeKind.NInt)
				{
					// normal casts work for int/nint (both in checked and unchecked context)
					// note that pointers only allow normal casts in unchecked contexts
				}
				else if (expressionBuilder.settings.NativeIntegers)
				{
					// if native integer types are available, prefer using those
					return this.ConvertTo(SpecialType.NInt, expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
				}
				else if (checkForOverflow)
				{
					// if overflow-checking is enabled, we can simply cast via long:
					// (and long itself works directly in checked context)
					if (!type.IsKnownType(KnownTypeCode.Int64))
					{
						return this.ConvertTo(compilation.FindType(KnownTypeCode.Int64), expressionBuilder, checkForOverflow)
							.ConvertTo(targetType, expressionBuilder, checkForOverflow);
					}
				}
				else if (!type.Kind.IsAnyPointer())
				{
					// If overflow-checking is disabled, the only way to truncate to native size
					// without throwing an exception in 32-bit mode is to use a pointer type.
					return this.ConvertTo(new PointerType(compilation.FindType(KnownTypeCode.Void)), expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow);
				}
			}
			else if (targetUType.IsKnownType(KnownTypeCode.UIntPtr) && utype.GetStackType().IsIntegerType())
			{ // Conversion to UIntPtr
				if (type.IsKnownType(KnownTypeCode.UInt32) || type.Kind.IsAnyPointer() || type.Kind == TypeKind.NUInt)
				{
					// normal casts work for uint/nuint and pointers (both in checked and unchecked context)
				}
				else if (expressionBuilder.settings.NativeIntegers)
				{
					// if native integer types are available, prefer using those
					return this.ConvertTo(SpecialType.NUInt, expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
				}
				else if (checkForOverflow)
				{
					// if overflow-checking is enabled, we can simply cast via ulong:
					// (and ulong itself works directly in checked context)
					if (!type.IsKnownType(KnownTypeCode.UInt64))
					{
						return this.ConvertTo(compilation.FindType(KnownTypeCode.UInt64), expressionBuilder, checkForOverflow)
							.ConvertTo(targetType, expressionBuilder, checkForOverflow);
					}
				}
				else
				{
					// If overflow-checking is disabled, the only way to truncate to native size
					// without throwing an exception in 32-bit mode is to use a pointer type.
					return this.ConvertTo(new PointerType(compilation.FindType(KnownTypeCode.Void)), expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow);
				}
			}

			if (targetType.Kind.IsAnyPointer() && type.Kind == TypeKind.Enum)
			{
				// enum to pointer: C# doesn't allow such casts
				// -> convert via underlying type
				return this.ConvertTo(type.GetEnumUnderlyingType(), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			}
			else if (targetUType.Kind == TypeKind.Enum && type.Kind.IsAnyPointer())
			{
				// pointer to enum: C# doesn't allow such casts
				// -> convert via underlying type
				return this.ConvertTo(targetUType.GetEnumUnderlyingType(), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			}
			if (targetType.Kind.IsAnyPointer() && type.IsKnownType(KnownTypeCode.Char)
			   || targetUType.IsKnownType(KnownTypeCode.Char) && type.Kind.IsAnyPointer())
			{
				// char <-> pointer: C# doesn't allow such casts
				// -> convert via ushort
				return this.ConvertTo(compilation.FindType(KnownTypeCode.UInt16), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			}
			if (targetType.Kind == TypeKind.Pointer && type.Kind == TypeKind.ByReference && Expression is DirectionExpression)
			{
				// convert from reference to pointer
				Expression arg = ((DirectionExpression)Expression).Expression.Detach();
				var pointerType = new PointerType(((ByReferenceType)type).ElementType);
				if (arg is UnaryOperatorExpression argUOE && argUOE.Operator == UnaryOperatorType.Dereference)
				{
					// &*ptr -> ptr
					return new TranslatedExpression(argUOE).UnwrapChild(argUOE.Expression)
						.ConvertTo(targetType, expressionBuilder);
				}
				var pointerExpr = new UnaryOperatorExpression(UnaryOperatorType.AddressOf, arg)
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ResolveResult(pointerType));
				// perform remaining pointer cast, if necessary
				return pointerExpr.ConvertTo(targetType, expressionBuilder);
			}
			if (targetType.Kind == TypeKind.ByReference)
			{
				if (NormalizeTypeVisitor.TypeErasure.EquivalentTypes(targetType, this.Type))
				{
					return this;
				}
				var elementType = ((ByReferenceType)targetType).ElementType;
				if (this.Expression is DirectionExpression thisDir && this.ILInstructions.Any(i => i.OpCode == OpCode.AddressOf)
					&& thisDir.Expression.GetResolveResult()?.Type.GetStackType() == elementType.GetStackType())
				{
					// When converting a reference to a temporary to a different type,
					// apply the cast to the temporary instead.
					var convertedTemp = this.UnwrapChild(thisDir.Expression).ConvertTo(elementType, expressionBuilder, checkForOverflow);
					return new DirectionExpression(FieldDirection.Ref, convertedTemp)
						.WithILInstruction(this.ILInstructions)
						.WithRR(new ByReferenceResolveResult(convertedTemp.ResolveResult, ReferenceKind.Ref));
				}
				if (this.Type.Kind == TypeKind.ByReference && !IsFixedVariable())
				{
					// Convert between managed reference types.
					// We can't do this by going through a pointer type because that would temporarily stop GC tracking.
					// Instead, emit `ref Unsafe.As<T>(ref expr)`
					return expressionBuilder.CallUnsafeIntrinsic("As", new[] { this.Expression },
						typeArguments: new IType[] { ((ByReferenceType)this.Type).ElementType, elementType },
						returnType: targetType);
				}
				// Convert from integer/pointer to reference.
				// First, convert to the corresponding pointer type:
				var arg = this.ConvertTo(new PointerType(elementType), expressionBuilder, checkForOverflow);
				Expression expr;
				ResolveResult elementRR;
				if (arg.Expression is UnaryOperatorExpression unary && unary.Operator == UnaryOperatorType.AddressOf)
				{
					// If we already have an address -> unwrap
					expr = arg.UnwrapChild(unary.Expression);
					elementRR = expr.GetResolveResult();
				}
				else
				{
					// Otherwise dereference the pointer:
					expr = new UnaryOperatorExpression(UnaryOperatorType.Dereference, arg.Expression);
					elementRR = new ResolveResult(elementType);
					expr.AddAnnotation(elementRR);
				}
				// And then take a reference:
				return new DirectionExpression(FieldDirection.Ref, expr)
					.WithoutILInstruction()
					.WithRR(new ByReferenceResolveResult(elementRR, ReferenceKind.Ref));
			}
			if (this.ResolveResult.IsCompileTimeConstant && this.ResolveResult.ConstantValue != null
				&& NullableType.IsNullable(targetType) && !utype.Equals(targetUType)
				&& targetUType.GetStackType().IsIntegerType())
			{
				// Casts like `(uint?)-1` are only valid in an explicitly unchecked context, but we
				// don't have logic to ensure such a context (usually we emit into an implicitly unchecked context).
				// This only applies with constants as input (int->uint? is fine in implicitly unchecked context).
				// We use an intermediate cast to the nullable's underlying type, which results
				// in a constant conversion, so the final output will be something like `(uint?)uint.MaxValue`
				return ConvertTo(targetUType, expressionBuilder, checkForOverflow, allowImplicitConversion: false)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
			}
			var rr = expressionBuilder.resolver.WithCheckForOverflow(checkForOverflow).ResolveCast(targetType, ResolveResult);
			if (rr.IsCompileTimeConstant && !rr.IsError)
			{
				var convertedResult = expressionBuilder.ConvertConstantValue(rr, allowImplicitConversion)
					.WithILInstruction(this.ILInstructions);
				if (convertedResult.Expression is PrimitiveExpression outputLiteral && this.Expression is PrimitiveExpression inputLiteral)
				{
					outputLiteral.Format = inputLiteral.Format;
				}
				return convertedResult;
			}
			else if (rr.IsError && targetType.IsReferenceType == true && type.IsReferenceType == true)
			{
				// Conversion between two reference types, but no direct cast allowed? cast via object
				// Just make sure we avoid infinite recursion even if the resolver falsely claims we can't cast directly:
				if (!(targetType.IsKnownType(KnownTypeCode.Object) || type.IsKnownType(KnownTypeCode.Object)))
				{
					return this.ConvertTo(compilation.FindType(KnownTypeCode.Object), expressionBuilder)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow, allowImplicitConversion);
				}
			}
			if (targetType.Kind.IsAnyPointer() && (0.Equals(ResolveResult.ConstantValue) || 0u.Equals(ResolveResult.ConstantValue)))
			{
				if (allowImplicitConversion)
				{
					return new NullReferenceExpression()
						.WithILInstruction(this.ILInstructions)
						.WithRR(new ConstantResolveResult(SpecialType.NullType, null));
				}
				return new CastExpression(expressionBuilder.ConvertType(targetType), new NullReferenceExpression())
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(SpecialType.NullType, null));
			}
			if (allowImplicitConversion)
			{
				if (conversions.ImplicitConversion(ResolveResult, targetType).IsValid)
				{
					return this;
				}
			}
			else
			{
				if (NormalizeTypeVisitor.IgnoreNullabilityAndTuples.EquivalentTypes(type, targetType))
				{
					// avoid an explicit cast when types differ only in nullability of reference types
					return this;
				}
			}
			var castExpr = new CastExpression(expressionBuilder.ConvertType(targetType), Expression);
			bool needsCheckAnnotation = targetUType.GetStackType().IsIntegerType();
			if (needsCheckAnnotation)
			{
				castExpr.AddAnnotation(checkForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			}
			return castExpr.WithoutILInstruction().WithRR(rr);
		}

		bool IsFixedVariable()
		{
			if (this.Expression is DirectionExpression dirExpr)
			{
				var inst = dirExpr.Expression.Annotation<ILInstruction>();
				return inst != null && PointerArithmeticOffset.IsFixedVariable(inst);
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets whether an implicit conversion from 'inputType' to 'newTargetType'
		/// would have the same semantics as the existing cast from 'inputType' to 'oldTargetType'.
		/// The existing cast is classified in 'conversion'.
		/// </summary>
		bool CastCanBeMadeImplicit(Resolver.CSharpConversions conversions, Conversion conversion, IType inputType, IType oldTargetType, IType newTargetType)
		{
			if (!conversion.IsImplicit)
			{
				// If the cast was required for the old conversion, avoid making it implicit.
				return false;
			}
			if (oldTargetType.Kind == TypeKind.NInt || oldTargetType.Kind == TypeKind.NUInt
				|| newTargetType.Kind == TypeKind.NInt || newTargetType.Kind == TypeKind.NUInt)
			{
				// nint has identity conversion with IntPtr, but the two have different implicit conversions
				return false;
			}
			if (conversion.IsBoxingConversion)
			{
				return conversions.IsBoxingConversionOrInvolvingTypeParameter(inputType, newTargetType);
			}
			if (conversion.IsInterpolatedStringConversion)
			{
				return newTargetType.IsKnownType(KnownTypeCode.FormattableString)
					|| newTargetType.IsKnownType(KnownTypeCode.IFormattable);
			}
			return conversions.IdentityConversion(oldTargetType, newTargetType);
		}

		TranslatedExpression LdcI4(ICompilation compilation, int val)
		{
			return new PrimitiveExpression(val)
				.WithoutILInstruction()
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), val));
		}

		/// <summary>
		/// In conditional contexts, remove the bool-cast emitted when converting
		/// an "implicit operator bool" invocation.
		/// </summary>
		public TranslatedExpression UnwrapImplicitBoolConversion(Func<IType, bool> typeFilter = null)
		{
			if (!this.Type.IsKnownType(KnownTypeCode.Boolean))
				return this;
			if (!(this.ResolveResult is ConversionResolveResult rr))
				return this;
			if (!(rr.Conversion.IsUserDefined && rr.Conversion.IsImplicit))
				return this;
			if (typeFilter != null && !typeFilter(rr.Input.Type))
				return this;
			if (this.Expression is CastExpression cast)
			{
				return this.UnwrapChild(cast.Expression);
			}
			return this;
		}

		/// <summary>
		/// Converts this expression to a boolean expression.
		/// 
		/// Expects that the input expression is an integer expression; produces an expression
		/// that returns <c>true</c> iff the integer value is not 0.
		/// 
		/// If negate is true, instead produces an expression that returns <c>true</c> iff the integer value is 0.
		/// </summary>
		public TranslatedExpression ConvertToBoolean(ExpressionBuilder expressionBuilder, bool negate = false)
		{
			if (Type.IsKnownType(KnownTypeCode.Boolean) || Type.Kind == TypeKind.Unknown)
			{
				if (negate)
				{
					return expressionBuilder.LogicNot(this).WithoutILInstruction();
				}
				else
				{
					return this;
				}
			}
			Debug.Assert(Type.GetStackType().IsIntegerType());
			IType boolType = expressionBuilder.compilation.FindType(KnownTypeCode.Boolean);
			if (ResolveResult.IsCompileTimeConstant && ResolveResult.ConstantValue is int)
			{
				bool val = (int)ResolveResult.ConstantValue != 0;
				val ^= negate;
				return new PrimitiveExpression(val)
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(boolType, val));
			}
			else if (ResolveResult.IsCompileTimeConstant && ResolveResult.ConstantValue is byte)
			{
				bool val = (byte)ResolveResult.ConstantValue != 0;
				val ^= negate;
				return new PrimitiveExpression(val)
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(boolType, val));
			}
			else if (Type.Kind == TypeKind.Pointer)
			{
				var nullRef = new NullReferenceExpression()
					.WithoutILInstruction()
					.WithRR(new ConstantResolveResult(SpecialType.NullType, null));
				var op = negate ? BinaryOperatorType.Equality : BinaryOperatorType.InEquality;
				return new BinaryOperatorExpression(Expression, op, nullRef.Expression)
					.WithoutILInstruction()
					.WithRR(new OperatorResolveResult(boolType, System.Linq.Expressions.ExpressionType.NotEqual,
													  this.ResolveResult, nullRef.ResolveResult));
			}
			else
			{
				var zero = new PrimitiveExpression(0)
					.WithoutILInstruction()
					.WithRR(new ConstantResolveResult(expressionBuilder.compilation.FindType(KnownTypeCode.Int32), 0));
				var op = negate ? BinaryOperatorType.Equality : BinaryOperatorType.InEquality;
				return new BinaryOperatorExpression(Expression, op, zero.Expression)
					.WithoutILInstruction()
					.WithRR(new OperatorResolveResult(boolType, System.Linq.Expressions.ExpressionType.NotEqual,
													  this.ResolveResult, zero.ResolveResult));
			}
		}
	}
}
