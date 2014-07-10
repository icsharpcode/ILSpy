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
	class ExpressionBuilder(ICompilation compilation) : ILVisitor<ExpressionBuilder.ConvertedExpression>
	{
		private readonly ICompilation compilation = compilation;

		internal struct ConvertedExpression(Expression expression, IType type) {
			public readonly Expression Expression = expression;
			public readonly IType Type = type;
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
