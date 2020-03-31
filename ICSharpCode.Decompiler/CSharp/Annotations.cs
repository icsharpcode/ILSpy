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
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp
{
	// Annotations:
	//  * AstNodes should be annotated with the corresponding ILInstruction
	//  * AstNodes referring to other entities should be annotated with the IEntity
	//  * Expression type information is currently only available in the ExpressionBuilder, but we might change 'WithTypeInfo()'
	//    to use an annotation in the future
	//  * IntroduceUnsafeModifier.PointerArithmeticAnnotation is placed on arithmetic operators that operate on pointers.
	//    TODO: actually, we could use the type info instead?
	//  * AddCheckedBlocks.CheckedAnnotation / AddCheckedBlocks.UnCheckedAnnotation is used on checked/unchecked integer arithmetic
	//    TODO: here the info is also redundant, we could peek at the BinaryNumericInstruction instead
	//          but on the other hand, some unchecked casts are not backed by any BinaryNumericInstruction

	/// <summary>
	/// Currently unused; we'll probably use the LdToken ILInstruction as annotation instead when LdToken support gets reimplemented.
	/// </summary>
	public class LdTokenAnnotation { }

	public static class AnnotationExtensions
	{
		internal static ExpressionWithILInstruction WithILInstruction(this Expression expression, ILInstruction instruction)
		{
			expression.AddAnnotation(instruction);
			return new ExpressionWithILInstruction(expression);
		}

		internal static ExpressionWithILInstruction WithILInstruction(this Expression expression, IEnumerable<ILInstruction> instructions)
		{
			foreach (var inst in instructions)
				expression.AddAnnotation(inst);
			return new ExpressionWithILInstruction(expression);
		}

		internal static ExpressionWithILInstruction WithoutILInstruction(this Expression expression)
		{
			return new ExpressionWithILInstruction(expression);
		}

		internal static TranslatedStatement WithILInstruction(this Statement statement, ILInstruction instruction)
		{
			statement.AddAnnotation(instruction);
			return new TranslatedStatement(statement);
		}

		internal static TranslatedStatement WithILInstruction(this Statement statement, IEnumerable<ILInstruction> instructions)
		{
			foreach (var inst in instructions)
				statement.AddAnnotation(inst);
			return new TranslatedStatement(statement);
		}

		internal static TranslatedStatement WithoutILInstruction(this Statement statement)
		{
			return new TranslatedStatement(statement);
		}

		internal static TranslatedExpression WithILInstruction(this ExpressionWithResolveResult expression, ILInstruction instruction)
		{
			expression.Expression.AddAnnotation(instruction);
			return new TranslatedExpression(expression.Expression, expression.ResolveResult);
		}

		internal static TranslatedExpression WithILInstruction(this ExpressionWithResolveResult expression, IEnumerable<ILInstruction> instructions)
		{
			foreach (var inst in instructions)
				expression.Expression.AddAnnotation(inst);
			return new TranslatedExpression(expression.Expression, expression.ResolveResult);
		}

		internal static TranslatedExpression WithILInstruction(this TranslatedExpression expression, ILInstruction instruction)
		{
			expression.Expression.AddAnnotation(instruction);
			return expression;
		}

		internal static TranslatedExpression WithoutILInstruction(this ExpressionWithResolveResult expression)
		{
			return new TranslatedExpression(expression.Expression, expression.ResolveResult);
		}

		internal static ExpressionWithResolveResult WithRR(this Expression expression, ResolveResult resolveResult)
		{
			expression.AddAnnotation(resolveResult);
			return new ExpressionWithResolveResult(expression, resolveResult);
		}

		internal static TranslatedExpression WithRR(this ExpressionWithILInstruction expression, ResolveResult resolveResult)
		{
			expression.Expression.AddAnnotation(resolveResult);
			return new TranslatedExpression(expression, resolveResult);
		}

		/// <summary>
		/// Retrieves the <see cref="ISymbol"/> associated with this AstNode, or null if no symbol is associated with the node.
		/// </summary>
		public static ISymbol GetSymbol(this AstNode node)
		{
			var rr = node.Annotation<ResolveResult>();
			if (rr is MethodGroupResolveResult) {
				// delegate construction?
				var newObj = node.Annotation<NewObj>();
				if (newObj != null) {
					var funcptr = newObj.Arguments.ElementAtOrDefault(1);
					if (funcptr is LdFtn ldftn) {
						return ldftn.Method;
					} else if (funcptr is LdVirtFtn ldVirtFtn) {
						return ldVirtFtn.Method;
					}
				}
				var ldVirtDelegate = node.Annotation<LdVirtDelegate>();
				if (ldVirtDelegate != null) {
					return ldVirtDelegate.Method;
				}
			}
			return rr?.GetSymbol();
		}

		/// <summary>
		/// Retrieves the <see cref="ResolveResult"/> associated with this <see cref="AstNode"/>, or <see cref="ErrorResolveResult.UnknownError"/> if no resolve result is associated with the node.
		/// </summary>
		public static ResolveResult GetResolveResult(this AstNode node)
		{
			return node.Annotation<ResolveResult>() ?? ErrorResolveResult.UnknownError;
		}

		/// <summary>
		/// Retrieves the <see cref="ILVariable"/> associated with this <see cref="IdentifierExpression"/>, or <c>null</c> if no variable is associated with this identifier.
		/// </summary>
		public static ILVariable GetILVariable(this IdentifierExpression expr)
		{
			var rr = expr.Annotation<ResolveResult>() as ILVariableResolveResult;
			if (rr != null)
				return rr.Variable;
			else
				return null;
		}

		/// <summary>
		/// Retrieves the <see cref="ILVariable"/> associated with this <see cref="VariableInitializer"/>, or <c>null</c> if no variable is associated with this initializer.
		/// </summary>
		public static ILVariable GetILVariable(this VariableInitializer vi)
		{
			var rr = vi.Annotation<ResolveResult>() as ILVariableResolveResult;
			if (rr != null)
				return rr.Variable;
			else
				return null;
		}

		/// <summary>
		/// Retrieves the <see cref="ILVariable"/> associated with this <see cref="ForeachStatement"/>, or <c>null</c> if no variable is associated with this foreach statement.
		/// </summary>
		public static ILVariable GetILVariable(this ForeachStatement loop)
		{
			var rr = loop.Annotation<ResolveResult>() as ILVariableResolveResult;
			if (rr != null)
				return rr.Variable;
			else
				return null;
		}

		/// <summary>
		/// Adds an <see cref="ILVariable"/> to this initializer.
		/// </summary>
		public static VariableInitializer WithILVariable(this VariableInitializer vi, ILVariable v)
		{
			vi.AddAnnotation(new ILVariableResolveResult(v, v.Type));
			return vi;
		}

		/// <summary>
		/// Adds an <see cref="ILVariable"/> to this foreach statement.
		/// </summary>
		public static ForeachStatement WithILVariable(this ForeachStatement loop, ILVariable v)
		{
			loop.AddAnnotation(new ILVariableResolveResult(v, v.Type));
			return loop;
		}

		/// <summary>
		/// Copies all annotations from <paramref name="other"/> to <paramref name="node"/>.
		/// </summary>
		public static T CopyAnnotationsFrom<T>(this T node, AstNode other) where T : AstNode
		{
			foreach (object annotation in other.Annotations) {
				node.AddAnnotation(annotation);
			}
			return node;
		}

		/// <summary>
		/// Copies all <see cref="ILInstruction"/> annotations from <paramref name="other"/> to <paramref name="node"/>.
		/// </summary>
		public static T CopyInstructionsFrom<T>(this T node, AstNode other) where T : AstNode
		{
			foreach (object annotation in other.Annotations.OfType<ILInstruction>()) {
				node.AddAnnotation(annotation);
			}
			return node;
		}
	}

	/// <summary>
	/// Represents a reference to a local variable.
	/// </summary>
	public class ILVariableResolveResult : ResolveResult
	{
		public readonly ILVariable Variable;

		public ILVariableResolveResult(ILVariable v) : base(v.Type)
		{
			this.Variable = v;
		}

		public ILVariableResolveResult(ILVariable v, IType type) : base(type)
		{
			this.Variable = v ?? throw new ArgumentNullException(nameof(v));
		}
	}

	/// <summary>
	/// Annotates a <see cref="ForeachStatement"/> with the instructions for the GetEnumerator, MoveNext and get_Current calls.
	/// </summary>
	public class ForeachAnnotation
	{
		public readonly ILInstruction GetEnumeratorCall;
		public readonly ILInstruction MoveNextCall;
		public readonly ILInstruction GetCurrentCall;

		public ForeachAnnotation(ILInstruction getEnumeratorCall, ILInstruction moveNextCall, ILInstruction getCurrentCall)
		{
			GetEnumeratorCall = getEnumeratorCall;
			MoveNextCall = moveNextCall;
			GetCurrentCall = getCurrentCall;
		}
	}

	/// <summary>
	/// Annotates the top-level block statement of a function
	/// with the implicitly executed return/yield break.
	/// </summary>
	public class ImplicitReturnAnnotation
	{
		public readonly Leave Leave;

		public ImplicitReturnAnnotation(Leave leave)
		{
			this.Leave = leave;
		}
	}

	/// <summary>
	/// Annotates an expression when an implicit user-defined conversion was omitted.
	/// </summary>
	public class ImplicitConversionAnnotation
	{
		public readonly ConversionResolveResult ConversionResolveResult;
		public IType TargetType => ConversionResolveResult.Type;

		public ImplicitConversionAnnotation(ConversionResolveResult conversionResolveResult)
		{
			this.ConversionResolveResult = conversionResolveResult;
		}
	}

	/// <summary>
	/// Annotates a QueryGroupClause with the ILFunctions of each (implicit lambda) expression.
	/// </summary>
	public class QueryGroupClauseAnnotation
	{
		public readonly ILFunction KeyLambda;
		public readonly ILFunction ProjectionLambda;

		public QueryGroupClauseAnnotation(ILFunction key, ILFunction projection)
		{
			this.KeyLambda = key;
			this.ProjectionLambda = projection;
		}
	}

	/// <summary>
	/// Annotates a QueryJoinClause with the ILFunctions of each (implicit lambda) expression.
	/// </summary>
	public class QueryJoinClauseAnnotation
	{
		public readonly ILFunction OnLambda;
		public readonly ILFunction EqualsLambda;

		public QueryJoinClauseAnnotation(ILFunction on, ILFunction equals)
		{
			this.OnLambda = on;
			this.EqualsLambda = equals;
		}
	}
}
