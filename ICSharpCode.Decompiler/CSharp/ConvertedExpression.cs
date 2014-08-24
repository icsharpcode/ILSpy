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
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Output of C# ExpressionBuilder -- a pair of decompiled C# expression and its type
	/// </summary>
	struct ConvertedExpression
	{
		public readonly Expression Expression;
		public readonly IType Type;
		
		public ConvertedExpression(Expression expression, IType type)
		{
			this.Expression = expression;
			this.Type = type;
		}
		
		public Expression ConvertTo(IType targetType, ExpressionBuilder expressionBuilder)
		{
			if (targetType.IsKnownType(KnownTypeCode.Boolean))
				return ConvertToBoolean();
			if (Type.Equals(targetType))
				return Expression;
			if (Type.Kind == TypeKind.ByReference && targetType.Kind == TypeKind.Pointer && Expression is DirectionExpression) {
				// convert from reference to pointer
				Expression arg = ((DirectionExpression)Expression).Expression.Detach();
				var pointerExpr = new ConvertedExpression(
					new UnaryOperatorExpression(UnaryOperatorType.AddressOf, arg),
					new PointerType(((ByReferenceType)Type).ElementType));
				// perform remaining pointer cast, if necessary
				return pointerExpr.ConvertTo(targetType, expressionBuilder);
			}
			if (Expression is PrimitiveExpression) {
				object value = ((PrimitiveExpression)Expression).Value;
				var rr = expressionBuilder.resolver.ResolveCast(targetType, new ConstantResolveResult(Type, value));
				if (rr.IsCompileTimeConstant && !rr.IsError)
					return expressionBuilder.astBuilder.ConvertConstantValue(rr);
			}
			return new CastExpression(
				expressionBuilder.astBuilder.ConvertType(targetType),
				Expression);
		}
		
		public Expression ConvertToBoolean()
		{
			if (Type.IsKnownType(KnownTypeCode.Boolean) || Type.Kind == TypeKind.Unknown)
				return Expression;
			else if (Expression is PrimitiveExpression && Type.IsKnownType(KnownTypeCode.Int32))
				return new PrimitiveExpression((int)((PrimitiveExpression)Expression).Value != 0);
			else if (Type.Kind == TypeKind.Pointer)
				return new BinaryOperatorExpression(Expression, BinaryOperatorType.InEquality, new NullReferenceExpression());
			else
				return new BinaryOperatorExpression(Expression, BinaryOperatorType.InEquality, new PrimitiveExpression(0));
		}
	}
}
