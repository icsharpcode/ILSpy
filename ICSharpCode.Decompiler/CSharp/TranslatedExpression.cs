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
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.Decompiler.IL;

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
		/// </remarks>
		public TranslatedExpression ConvertTo(IType targetType, ExpressionBuilder expressionBuilder, bool checkForOverflow = false)
		{
			var type = this.Type;
			if (type.Equals(targetType))
				return this;
			var compilation = expressionBuilder.compilation;
			if (type.IsKnownType(KnownTypeCode.Boolean) && targetType.GetStackType().IsIntegerType()) {
				// convert from boolean to integer (or enum)
				return new ConditionalExpression(
					this.Expression,
					LdcI4(compilation, 1).ConvertTo(targetType, expressionBuilder, checkForOverflow),
					LdcI4(compilation, 0).ConvertTo(targetType, expressionBuilder, checkForOverflow)
				).WithoutILInstruction().WithRR(new ResolveResult(targetType));
			}
			// Special-case IntPtr and UIntPtr: they behave slightly weird, e.g. converting to them always checks for overflow,
			// but converting from them never checks for overflow.
			if (checkForOverflow && type.IsKnownType(KnownTypeCode.IntPtr) && !targetType.IsKnownType(KnownTypeCode.Int64)) {
				// Convert through `long` instead.
				return this.ConvertTo(compilation.FindType(KnownTypeCode.Int64), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			} else if (checkForOverflow && type.IsKnownType(KnownTypeCode.UIntPtr) && !targetType.IsKnownType(KnownTypeCode.UInt64)) {
				// Convert through `ulong` instead.
				return this.ConvertTo(compilation.FindType(KnownTypeCode.UInt64), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
			}
			
			if (targetType.IsKnownType(KnownTypeCode.Boolean)) {
				// convert to boolean through byte, to simulate the truncation to 8 bits
				return this.ConvertTo(compilation.FindType(KnownTypeCode.Byte), expressionBuilder, checkForOverflow)
					.ConvertToBoolean(expressionBuilder);
			}
			if ((targetType.IsKnownType(KnownTypeCode.IntPtr) || targetType.IsKnownType(KnownTypeCode.UIntPtr))
			    && type.Kind != TypeKind.Pointer && !checkForOverflow)
			{
				// (u)long -> (U)IntPtr casts in C# can throw overflow exceptions in 32-bit mode, even in unchecked context.
				// To avoid those, convert via `void*`.
				return this.ConvertTo(new PointerType(compilation.FindType(KnownTypeCode.Void)), expressionBuilder, checkForOverflow)
					.ConvertTo(targetType, expressionBuilder, checkForOverflow);
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
				// Then dereference the pointer:
				var derefExpr = new UnaryOperatorExpression(UnaryOperatorType.Dereference, arg.Expression);
				var elementRR = new ResolveResult(elementType);
				derefExpr.AddAnnotation(elementRR);
				// And then take a reference:
				return new DirectionExpression(FieldDirection.Ref, derefExpr)
					.WithoutILInstruction()
					.WithRR(new ByReferenceResolveResult(elementRR, false));
			}
			var rr = expressionBuilder.resolver.WithCheckForOverflow(checkForOverflow).ResolveCast(targetType, ResolveResult);
			if (rr.IsCompileTimeConstant && !rr.IsError) {
				return expressionBuilder.ConvertConstantValue(rr)
					.WithILInstruction(this.ILInstructions);
			}
			if (targetType.Kind == TypeKind.Pointer && 0.Equals(ResolveResult.ConstantValue)) {
				return new NullReferenceExpression().CastTo(expressionBuilder.ConvertType(targetType))
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(targetType, null));
			}
			var castExpr = new CastExpression(expressionBuilder.ConvertType(targetType), Expression);
			castExpr.AddAnnotation(checkForOverflow ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
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
		/// </summary>
		public TranslatedExpression ConvertToBoolean(ExpressionBuilder expressionBuilder)
		{
			if (Type.IsKnownType(KnownTypeCode.Boolean) || Type.Kind == TypeKind.Unknown) {
				return this;
			}
			Debug.Assert(Type.GetStackType().IsIntegerType());
			IType boolType = expressionBuilder.compilation.FindType(KnownTypeCode.Boolean);
			if (ResolveResult.IsCompileTimeConstant && ResolveResult.ConstantValue is int) {
				bool val = (int)ResolveResult.ConstantValue != 0;
				return new PrimitiveExpression(val)
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(boolType, val));
			} else if (ResolveResult.IsCompileTimeConstant && ResolveResult.ConstantValue is byte) {
				bool val = (byte)ResolveResult.ConstantValue != 0;
				return new PrimitiveExpression(val)
					.WithILInstruction(this.ILInstructions)
					.WithRR(new ConstantResolveResult(boolType, val));
			} else if (Type.Kind == TypeKind.Pointer) {
				var nullRef = new NullReferenceExpression()
					.WithoutILInstruction()
					.WithRR(new ConstantResolveResult(SpecialType.NullType, null));
				return new BinaryOperatorExpression(Expression, BinaryOperatorType.InEquality, nullRef.Expression)
					.WithoutILInstruction()
					.WithRR(new OperatorResolveResult(boolType, System.Linq.Expressions.ExpressionType.NotEqual,
					                                  this.ResolveResult, nullRef.ResolveResult));
			} else {
				var zero = new PrimitiveExpression(0)
					.WithoutILInstruction()
					.WithRR(new ConstantResolveResult(expressionBuilder.compilation.FindType(KnownTypeCode.Int32), 0));
				return new BinaryOperatorExpression(Expression, BinaryOperatorType.InEquality, zero.Expression)
					.WithoutILInstruction()
					.WithRR(new OperatorResolveResult(boolType, System.Linq.Expressions.ExpressionType.NotEqual,
					                                  this.ResolveResult, zero.ResolveResult));
			}
		}
	}
}
