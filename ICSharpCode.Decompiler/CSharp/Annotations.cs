/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 2014-08-24
 * Time: 18:42
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.Decompiler.IL;

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
	
	/// <summary>
	/// Currently unused; we'll probably use the LdToken ILInstruction as annotation instead when LdToken support gets reimplemented.
	/// </summary>
	public class LdTokenAnnotation {}
	
	static class AnnotationExtensions
	{
		public static ExpressionWithILInstruction WithILInstruction(this Expression expression, ILInstruction instruction)
		{
			expression.AddAnnotation(instruction);
			return new ExpressionWithILInstruction(expression);
		}
		
		public static ExpressionWithILInstruction WithILInstruction(this Expression expression, IEnumerable<ILInstruction> instructions)
		{
			foreach (var inst in instructions)
				expression.AddAnnotation(inst);
			return new ExpressionWithILInstruction(expression);
		}
		
		public static ExpressionWithILInstruction WithoutILInstruction(this Expression expression)
		{
			return new ExpressionWithILInstruction(expression);
		}
		
		public static ConvertedExpression WithILInstruction(this ExpressionWithResolveResult expression, ILInstruction instruction)
		{
			expression.Expression.AddAnnotation(instruction);
			return new ConvertedExpression(expression.Expression, expression.ResolveResult);
		}
		
		public static ConvertedExpression WithoutILInstruction(this ExpressionWithResolveResult expression)
		{
			return new ConvertedExpression(expression.Expression, expression.ResolveResult);
		}
		
		public static ExpressionWithResolveResult WithRR(this Expression expression, ResolveResult resolveResult)
		{
			expression.AddAnnotation(resolveResult);
			return new ExpressionWithResolveResult(expression, resolveResult);
		}
		
		public static ConvertedExpression WithRR(this ExpressionWithILInstruction expression, ResolveResult resolveResult)
		{
			expression.Expression.AddAnnotation(resolveResult);
			return new ConvertedExpression(expression, resolveResult);
		}
	}
}
