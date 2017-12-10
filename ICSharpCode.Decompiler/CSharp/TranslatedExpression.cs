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
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

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
			for (AstNode parent = descendant.Parent; parent != null; parent = parent.Parent) {
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
		/// </remarks>
		public TranslatedExpression ConvertTo(IType targetType, ExpressionBuilder expressionBuilder, bool checkForOverflow = false, bool allowImplicitConversion = false)
		{
			var type = this.Type;
			if (type.Equals(targetType)) {
				// Make explicit conversion implicit, if possible
				if (allowImplicitConversion) {
					switch (ResolveResult) {
						case ConversionResolveResult conversion: {
							if (Expression is CastExpression cast
							&& (type.IsKnownType(KnownTypeCode.Object) && conversion.Conversion.IsBoxingConversion
								|| conversion.Conversion.IsAnonymousFunctionConversion
								|| (conversion.Conversion.IsImplicit && (conversion.Conversion.IsUserDefined || targetType.IsKnownType(KnownTypeCode.Decimal)))
							)) {
								return this.UnwrapChild(cast.Expression);
							} else if (Expression is ObjectCreateExpression oce && conversion.Conversion.IsMethodGroupConversion
									&& oce.Arguments.Count == 1 && expressionBuilder.settings.UseImplicitMethodGroupConversion) {
								return this.UnwrapChild(oce.Arguments.Single());
							}
							break;
						}
						case InvocationResolveResult invocation: {
							if (Expression is ObjectCreateExpression oce && oce.Arguments.Count == 1 && invocation.Type.IsKnownType(KnownTypeCode.NullableOfT)) {
								return this.UnwrapChild(oce.Arguments.Single());
							}
							break;
						}
					}
				}
				return this;
			}
			if (targetType.Kind == TypeKind.Unknown) {
				return this; // don't attempt to insert cast to '?'
			}
			var compilation = expressionBuilder.compilation;
			bool isLifted = type.IsKnownType(KnownTypeCode.NullableOfT) && targetType.IsKnownType(KnownTypeCode.NullableOfT);
			IType utype = isLifted ? NullableType.GetUnderlyingType(type) : type;
			IType targetUType = isLifted ? NullableType.GetUnderlyingType(targetType) : targetType;
			if (type.IsKnownType(KnownTypeCode.Boolean) && targetType.GetStackType().IsIntegerType()) {
				// convert from boolean to integer (or enum)
				return new ConditionalExpression(
					this.Expression,
					LdcI4(compilation, 1).ConvertTo(targetType, expressionBuilder, checkForOverflow),
					LdcI4(compilation, 0).ConvertTo(targetType, expressionBuilder, checkForOverflow)
				).WithoutILInstruction().WithRR(new ResolveResult(targetType));
			}
			if (targetType.IsKnownType(KnownTypeCode.Boolean)) {
				// convert to boolean through byte, to simulate the truncation to 8 bits
				return this.ConvertTo(compilation.FindType(KnownTypeCode.Byte), expressionBuilder, checkForOverflow)
					.ConvertToBoolean(expressionBuilder);
			}
			
			// Special-case IntPtr and UIntPtr: they behave extremely weird, see IntPtr.txt for details.
			if (type.IsKnownType(KnownTypeCode.IntPtr)) { // Conversion from IntPtr
				// Direct cast only works correctly for IntPtr -> long.
				// IntPtr -> int works correctly only in checked context.
				// Everything else can be worked around by casting via long.
				if (!(targetType.IsKnownType(KnownTypeCode.Int64) || checkForOverflow && targetType.IsKnownType(KnownTypeCode.Int32))) {
					return this.ConvertTo(compilation.FindType(KnownTypeCode.Int64), expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow);
				}
			} else if (type.IsKnownType(KnownTypeCode.UIntPtr)) { // Conversion from UIntPtr
				// Direct cast only works correctly for UIntPtr -> ulong.
				// UIntPtr -> uint works correctly only in checked context.
				// Everything else can be worked around by casting via ulong.
				if (!(targetType.IsKnownType(KnownTypeCode.UInt64) || checkForOverflow && targetType.IsKnownType(KnownTypeCode.UInt32))) {
					return this.ConvertTo(compilation.FindType(KnownTypeCode.UInt64), expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow);
				}
			}
			if (targetType.IsKnownType(KnownTypeCode.IntPtr)) { // Conversion to IntPtr
				if (type.IsKnownType(KnownTypeCode.Int32)) {
					// normal casts work for int (both in checked and unchecked context)
				} else if (checkForOverflow) {
					// if overflow-checking is enabled, we can simply cast via long:
					// (and long itself works directly in checked context)
					if (!type.IsKnownType(KnownTypeCode.Int64)) {
						return this.ConvertTo(compilation.FindType(KnownTypeCode.Int64), expressionBuilder, checkForOverflow)
							.ConvertTo(targetType, expressionBuilder, checkForOverflow);
					}
				} else {
					// If overflow-checking is disabled, the only way to truncate to native size
					// without throwing an exception in 32-bit mode is to use a pointer type.
					if (type.Kind != TypeKind.Pointer) {
						return this.ConvertTo(new PointerType(compilation.FindType(KnownTypeCode.Void)), expressionBuilder, checkForOverflow)
							.ConvertTo(targetType, expressionBuilder, checkForOverflow);
					}
				}
			} else if (targetType.IsKnownType(KnownTypeCode.UIntPtr)) { // Conversion to UIntPtr
				if (type.IsKnownType(KnownTypeCode.UInt32) || type.Kind == TypeKind.Pointer) {
					// normal casts work for uint and pointers (both in checked and unchecked context)
				} else if (checkForOverflow) {
					// if overflow-checking is enabled, we can simply cast via ulong:
					// (and ulong itself works directly in checked context)
					if (!type.IsKnownType(KnownTypeCode.UInt64)) {
						return this.ConvertTo(compilation.FindType(KnownTypeCode.UInt64), expressionBuilder, checkForOverflow)
							.ConvertTo(targetType, expressionBuilder, checkForOverflow);
					}
				} else {
					// If overflow-checking is disabled, the only way to truncate to native size
					// without throwing an exception in 32-bit mode is to use a pointer type.
					return this.ConvertTo(new PointerType(compilation.FindType(KnownTypeCode.Void)), expressionBuilder, checkForOverflow)
						.ConvertTo(targetType, expressionBuilder, checkForOverflow);
				}
			}

			if (targetType.Kind == TypeKind.Pointer && type.Kind == TypeKind.Enum) {
				// enum to pointer: C# doesn't allow such casts
				// -> convert via underlying type
				return this.ConvertTo(type.GetEnumUnderlyingType(), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			} else if (targetType.Kind == TypeKind.Enum && type.Kind == TypeKind.Pointer) {
				// pointer to enum: C# doesn't allow such casts
				// -> convert via underlying type
				return this.ConvertTo(targetType.GetEnumUnderlyingType(), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			}
			if (targetType.Kind == TypeKind.Pointer && type.IsKnownType(KnownTypeCode.Char)
			   || targetType.IsKnownType(KnownTypeCode.Char) && type.Kind == TypeKind.Pointer) {
				// char <-> pointer: C# doesn't allow such casts
				// -> convert via ushort
				return this.ConvertTo(compilation.FindType(KnownTypeCode.UInt16), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			}
			if (targetType.Kind == TypeKind.Pointer && type.Kind == TypeKind.ByReference && Expression is DirectionExpression) {
				// convert from reference to pointer
				Expression arg = ((DirectionExpression)Expression).Expression.Detach();
				var pointerType = new PointerType(((ByReferenceType)type).ElementType);
				if (arg is UnaryOperatorExpression argUOE && argUOE.Operator == UnaryOperatorType.Dereference) {
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
			if (targetType.Kind == TypeKind.ByReference) {
				// Convert from integer/pointer to reference.
				// First, convert to the corresponding pointer type:
				var elementType = ((ByReferenceType)targetType).ElementType;
				var arg = this.ConvertTo(new PointerType(elementType), expressionBuilder, checkForOverflow);
				Expression expr;
				ResolveResult elementRR;
				if (arg.Expression is UnaryOperatorExpression unary && unary.Operator == UnaryOperatorType.AddressOf) {
					// If we already have an address -> unwrap
					expr = arg.UnwrapChild(unary.Expression);
					elementRR = expr.GetResolveResult();
				} else {
					// Otherwise dereference the pointer:
					expr = new UnaryOperatorExpression(UnaryOperatorType.Dereference, arg.Expression);
					elementRR = new ResolveResult(elementType);
					expr.AddAnnotation(elementRR);
				}
				// And then take a reference:
				return new DirectionExpression(FieldDirection.Ref, expr)
					.WithoutILInstruction()
					.WithRR(new ByReferenceResolveResult(elementRR, false));
			}
			var rr = expressionBuilder.resolver.WithCheckForOverflow(checkForOverflow).ResolveCast(targetType, ResolveResult);
			if (rr.IsCompileTimeConstant && !rr.IsError) {
				return expressionBuilder.ConvertConstantValue(rr, allowImplicitConversion)
					.WithILInstruction(this.ILInstructions);
			}
			if (targetType.Kind == TypeKind.Pointer && (0.Equals(ResolveResult.ConstantValue) || 0u.Equals(ResolveResult.ConstantValue))) {
				if (allowImplicitConversion) {
					return new NullReferenceExpression()
						.WithILInstruction(this.ILInstructions)
						.WithRR(new ConstantResolveResult(targetType, null));
				}
				return new CastExpression(expressionBuilder.ConvertType(targetType), new NullReferenceExpression())
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(targetType, null));
			}
			var conversions = Resolver.CSharpConversions.Get(compilation);
			if (allowImplicitConversion && conversions.ImplicitConversion(type, targetType).IsValid) {
				return this;
			}
			var castExpr = new CastExpression(expressionBuilder.ConvertType(targetType), Expression);
			bool avoidCheckAnnotation = utype.IsKnownType(KnownTypeCode.Single) && targetUType.IsKnownType(KnownTypeCode.Double);
			if (!avoidCheckAnnotation) {
				castExpr.AddAnnotation(checkForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
			}
			return castExpr.WithoutILInstruction().WithRR(rr);
		}
		
		TranslatedExpression LdcI4(ICompilation compilation, int val)
		{
			return new PrimitiveExpression(val)
				.WithoutILInstruction()
				.WithRR(new ConstantResolveResult(compilation.FindType(KnownTypeCode.Int32), val));
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
			if (Type.IsKnownType(KnownTypeCode.Boolean) || Type.Kind == TypeKind.Unknown) {
				if (negate) {
					return expressionBuilder.LogicNot(this).WithoutILInstruction();
				} else {
					return this;
				}
			}
			Debug.Assert(Type.GetStackType().IsIntegerType());
			IType boolType = expressionBuilder.compilation.FindType(KnownTypeCode.Boolean);
			if (ResolveResult.IsCompileTimeConstant && ResolveResult.ConstantValue is int) {
				bool val = (int)ResolveResult.ConstantValue != 0;
				val ^= negate;
				return new PrimitiveExpression(val)
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(boolType, val));
			} else if (ResolveResult.IsCompileTimeConstant && ResolveResult.ConstantValue is byte) {
				bool val = (byte)ResolveResult.ConstantValue != 0;
				val ^= negate;
				return new PrimitiveExpression(val)
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(boolType, val));
			} else if (Type.Kind == TypeKind.Pointer) {
				var nullRef = new NullReferenceExpression()
					.WithoutILInstruction()
					.WithRR(new ConstantResolveResult(SpecialType.NullType, null));
				var op = negate ? BinaryOperatorType.Equality : BinaryOperatorType.InEquality;
				return new BinaryOperatorExpression(Expression, op, nullRef.Expression)
					.WithoutILInstruction()
					.WithRR(new OperatorResolveResult(boolType, System.Linq.Expressions.ExpressionType.NotEqual,
					                                  this.ResolveResult, nullRef.ResolveResult));
			} else {
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
