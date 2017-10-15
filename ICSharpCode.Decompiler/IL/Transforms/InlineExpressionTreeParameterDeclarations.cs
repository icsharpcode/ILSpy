using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class InlineExpressionTreeParameterDeclarationsTransform : IStatementTransform
	{
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			throw new NotImplementedException();
		}

		#region InlineExpressionTreeParameterDeclarations
		bool InlineExpressionTreeParameterDeclarations(List<ILNode> body, ILExpression expr, int pos)
		{
			// When there is a Expression.Lambda() call, and the parameters are declared in the
			// IL statement immediately prior to the one containing the Lambda() call,
			// using this code for the3 declaration:
			//   stloc(v, call(Expression::Parameter, call(Type::GetTypeFromHandle, ldtoken(...)), ldstr(...)))
			// and the variables v are assigned only once (in that statements), and read only in a Expression::Lambda
			// call that immediately follows the assignment statements, then we will inline those assignments
			// into the Lambda call using ILCode.ExpressionTreeParameterDeclarations.

			// This is sufficient to allow inlining over the expression tree construction. The remaining translation
			// of expression trees into C# will be performed by a C# AST transformer.

			for (int i = expr.Arguments.Count - 1; i >= 0; i--) {
				if (InlineExpressionTreeParameterDeclarations(body, expr.Arguments[i], pos))
					return true;
			}

			MethodReference mr;
			ILExpression lambdaBodyExpr, parameterArray;
			if (!(expr.Match(ILCode.Call, out mr, out lambdaBodyExpr, out parameterArray) && mr.Name == "Lambda"))
				return false;
			if (!(parameterArray.Code == ILCode.InitArray && mr.DeclaringType.FullName == "System.Linq.Expressions.Expression"))
				return false;
			int firstParameterPos = pos - parameterArray.Arguments.Count;
			if (firstParameterPos < 0)
				return false;

			ILExpression[] parameterInitExpressions = new ILExpression[parameterArray.Arguments.Count + 1];
			for (int i = 0; i < parameterArray.Arguments.Count; i++) {
				parameterInitExpressions[i] = body[firstParameterPos + i] as ILExpression;
				if (!MatchParameterVariableAssignment(parameterInitExpressions[i]))
					return false;
				ILVariable v = (ILVariable)parameterInitExpressions[i].Operand;
				if (!parameterArray.Arguments[i].MatchLdloc(v))
					return false;
				// TODO: validate that the variable is only used here and within 'body'
			}

			parameterInitExpressions[parameterInitExpressions.Length - 1] = lambdaBodyExpr;
			Debug.Assert(expr.Arguments[0] == lambdaBodyExpr);
			expr.Arguments[0] = new ILExpression(ILCode.ExpressionTreeParameterDeclarations, null, parameterInitExpressions);

			body.RemoveRange(firstParameterPos, parameterArray.Arguments.Count);

			return true;
		}

		bool MatchParameterVariableAssignment(ILExpression expr)
		{
			// stloc(v, call(Expression::Parameter, call(Type::GetTypeFromHandle, ldtoken(...)), ldstr(...)))
			ILVariable v;
			ILExpression init;
			if (!expr.Match(ILCode.Stloc, out v, out init))
				return false;
			if (v.IsGenerated || v.IsParameter || v.IsPinned)
				return false;
			if (v.Type == null || v.Type.FullName != "System.Linq.Expressions.ParameterExpression")
				return false;
			MethodReference parameterMethod;
			ILExpression typeArg, nameArg;
			if (!init.Match(ILCode.Call, out parameterMethod, out typeArg, out nameArg))
				return false;
			if (!(parameterMethod.Name == "Parameter" && parameterMethod.DeclaringType.FullName == "System.Linq.Expressions.Expression"))
				return false;
			MethodReference getTypeFromHandle;
			ILExpression typeToken;
			if (!typeArg.Match(ILCode.Call, out getTypeFromHandle, out typeToken))
				return false;
			if (!(getTypeFromHandle.Name == "GetTypeFromHandle" && getTypeFromHandle.DeclaringType.FullName == "System.Type"))
				return false;
			return typeToken.Code == ILCode.Ldtoken && nameArg.Code == ILCode.Ldstr;
		}
		#endregion
	}
}
