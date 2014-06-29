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
	class ExpressionBuilder
	{
		struct ConvertedExpression(public readonly Expression Expression, public readonly IType Type) { }

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
	}
}
