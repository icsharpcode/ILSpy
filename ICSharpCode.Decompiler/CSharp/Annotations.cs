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

using System.Collections.Generic;
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
	public class LdTokenAnnotation {}
	
	/// <summary>
	/// Used by <see cref="Transforms.DeclareVariables"/> and <see cref="Transforms.DelegateConstruction"/>.
	/// </summary>
	sealed class CapturedVariableAnnotation {}
	
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
		/// Retrieves the symbol associated with this AstNode, or null if no symbol is associated with the node.
		/// </summary>
		public static ISymbol GetSymbol(this AstNode node)
		{
			var rr = node.Annotation<ResolveResult>();
			return rr != null ? rr.GetSymbol() : null;
		}
		
		public static ResolveResult GetResolveResult(this AstNode node)
		{
			return node.Annotation<ResolveResult>() ?? ErrorResolveResult.UnknownError;
		}
		
		public static ILVariable GetILVariable(this IdentifierExpression expr)
		{
			var rr = expr.Annotation<ResolveResult>() as ILVariableResolveResult;
			if (rr != null)
				return rr.Variable;
			else
				return null;
		}
		
		public static ILVariable GetILVariable(this VariableInitializer vi)
		{
			var rr = vi.Annotation<ResolveResult>() as ILVariableResolveResult;
			if (rr != null)
				return rr.Variable;
			else
				return null;
		}

		public static ILVariable GetILVariable(this ForeachStatement loop)
		{
			var rr = loop.Annotation<ResolveResult>() as ILVariableResolveResult;
			if (rr != null)
				return rr.Variable;
			else
				return null;
		}

		public static VariableInitializer WithILVariable(this VariableInitializer vi, ILVariable v)
		{
			vi.AddAnnotation(new ILVariableResolveResult(v, v.Type));
			return vi;
		}

		public static ForeachStatement WithILVariable(this ForeachStatement loop, ILVariable v)
		{
			loop.AddAnnotation(new ILVariableResolveResult(v, v.Type));
			return loop;
		}
	}
	
	public class ILVariableResolveResult : ResolveResult
	{
		public readonly ILVariable Variable;
		
		public ILVariableResolveResult(ILVariable v, IType type) : base(type)
		{
			this.Variable = v;
		}
	}

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
}
