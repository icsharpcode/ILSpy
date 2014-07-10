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
	class ExpressionBuilder(ICompilation compilation)
	{
		struct ConvertedExpression(Expression expression, IType type) {
			public readonly Expression Expression = expression;
			public readonly IType Type = type;
		}

		public Expression Convert(ILInstruction inst)
		{
			var expr = TransformExpression(inst).Expression;
			expr.AddAnnotation(inst);
			return expr;
		}
		
		ConvertedExpression ConvertArgument(ILInstruction inst)
		{
			var cexpr = TransformExpression(inst);
			cexpr.Expression.AddAnnotation(inst);
			return cexpr;
		}

		ConvertedExpression TransformExpression(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.LdcI4:
					return new ConvertedExpression(
						new PrimitiveExpression(((ConstantI4)inst).Value),
                        compilation.FindType(KnownTypeCode.Int32));
				case OpCode.LogicNot:
					return new ConvertedExpression(
						new UnaryOperatorExpression(UnaryOperatorType.Not, ConvertCondition(((LogicNotInstruction)inst).Operand)),
						compilation.FindType(KnownTypeCode.Boolean));
				default:
					return ErrorExpression("OpCode not supported: " + inst.OpCode);
			}
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
