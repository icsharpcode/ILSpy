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

using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Translates from ILAst to C# expressions.
	/// </summary>
	class ExpressionBuilder : ILVisitor<ExpressionBuilder.ConvertedExpression>
	{
		private readonly ICompilation compilation;
		
		public ExpressionBuilder(ICompilation compilation)
		{
			this.compilation = compilation;
		}
		
		internal struct ConvertedExpression 
		{
			public readonly Expression Expression;
			public readonly IType Type;
			
			public ConvertedExpression(Expression expression, IType type)
			{
				this.Expression = expression;
				this.Type = type;
			}
		}

		public Expression Convert(ILInstruction inst)
		{
			var expr = inst.AcceptVisitor(this).Expression;
			expr.AddAnnotation(inst);
			return expr;
		}
		
		ConvertedExpression ConvertArgument(ILInstruction inst)
		{
			var cexpr = inst.AcceptVisitor(this);
			cexpr.Expression.AddAnnotation(inst);
			return cexpr;
		}

		protected internal override ConvertedExpression VisitLdcI4(LdcI4 inst)
		{
			return new ConvertedExpression(
						new PrimitiveExpression(inst.Value),
						compilation.FindType(KnownTypeCode.Int32));
		}

		protected internal override ConvertedExpression VisitLogicNot(LogicNot inst)
		{
			return new ConvertedExpression(
						new UnaryOperatorExpression(UnaryOperatorType.Not, ConvertCondition(inst.Argument)),
						compilation.FindType(KnownTypeCode.Boolean));
		}

		protected override ConvertedExpression Default(ILInstruction inst)
		{
			return ErrorExpression("OpCode not supported: " + inst.OpCode);
		}

		static ConvertedExpression ErrorExpression(string message)
		{
			var e = new ErrorExpression();
			e.AddChild(new Comment(message, CommentType.MultiLine), Roles.Comment);
			return new ConvertedExpression(e, SpecialType.UnknownType);
		}

		public Expression ConvertCondition(ILInstruction condition)
		{
			var expr = ConvertArgument(condition);
			if (expr.Type.IsKnownType(KnownTypeCode.Boolean) || expr.Type.Kind == TypeKind.Unknown)
				return expr.Expression;
			else if (expr.Type.Kind == TypeKind.Pointer)
				return new BinaryOperatorExpression(expr.Expression, BinaryOperatorType.InEquality, new NullReferenceExpression());
			else
				return new BinaryOperatorExpression(expr.Expression, BinaryOperatorType.InEquality, new PrimitiveExpression(0));
        }
	}
}
