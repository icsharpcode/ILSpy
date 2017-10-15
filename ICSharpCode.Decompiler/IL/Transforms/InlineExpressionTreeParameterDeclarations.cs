using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ICSharpCode.Decompiler.TypeSystem;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class InlineExpressionTreeParameterDeclarationsTransform : IStatementTransform
	{
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (InlineExpressionTreeParameterDeclarations(block.Instructions, block.Instructions[pos] as CallInstruction, pos)) {
				context.RequestRerun(0);
			}
		}

		#region InlineExpressionTreeParameterDeclarations
		bool InlineExpressionTreeParameterDeclarations(InstructionCollection<ILInstruction> body, CallInstruction expr, int pos)
		{
			if (expr == null)
				return false;
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
				if (InlineExpressionTreeParameterDeclarations(body, expr.Arguments[i] as CallInstruction, pos))
					return true;
			}

			IMethod mr;
			ILInstruction lambdaBodyExpr;
			Block parameterArrayBlock;
			mr = expr.Method;
			if (expr.Arguments.Count != 2)
				return false;
			lambdaBodyExpr = expr.Arguments[0];
			parameterArrayBlock = expr.Arguments[1] as Block; 
			if (mr.Name != "Lambda")
				return false;
			if (parameterArrayBlock == null)
				return false;
			if (parameterArrayBlock.Type != BlockType.ArrayInitializer)
				return false;
			List<ILInstruction> parameterList = new List<ILInstruction>(parameterArrayBlock.Instructions);
			parameterList.RemoveAt(0); // newarr
			if (mr.DeclaringType.FullName != "System.Linq.Expressions.Expression")
				return false;
			int firstParameterPos = pos - parameterList.Count;
			if (firstParameterPos < 0)
				return false;

			ILInstruction[] parameterInitExpressions = new ILInstruction[parameterList.Count + 1];
			for (int i = 0; i < parameterList.Count; i++) {
				parameterInitExpressions[i] = body[firstParameterPos + i];
				if (!MatchParameterVariableAssignment(parameterInitExpressions[i]))
					return false;
				ILVariable v = (parameterInitExpressions[i] as StLoc).Variable;
				if (!(parameterList[i] as StObj).Value.MatchLdLoc(v)) // stobj System.Linq.Expressions.ParameterExpression(ldelema System.Linq.Expressions.ParameterExpression(ldloc I_0, ldc.i4 0), ldloc parameterExpression)
					return false;
				// TODO: validate that the variable is only used here and within 'body'
			}
			parameterInitExpressions[parameterInitExpressions.Length - 1] = lambdaBodyExpr;
			Debug.Assert(expr.Arguments[0] == lambdaBodyExpr);
			Block block = new Block(); // TODO do we need a new BlockType?
			block.Instructions.AddRange(parameterInitExpressions);
			expr.Arguments[0] = block;

			body.RemoveRange(firstParameterPos, parameterList.Count);

			return true;
		}

		bool MatchParameterVariableAssignment(ILInstruction expr)
		{
			// stloc(v, call(Expression::Parameter, call(Type::GetTypeFromHandle, ldtoken(...)), ldstr(...)))
			if (!expr.MatchStLoc(out ILVariable v, out ILInstruction init))
				return false;
			if (/*v.HasGeneratedName || */v.Kind == VariableKind.Parameter /*|| v.IsPinned*/) // TODO
				return false;
			if (v.Type == null || v.Type.FullName != "System.Linq.Expressions.ParameterExpression")
				return false;
			if (!(init is CallInstruction initCall))
				return false;
			if (initCall.Arguments.Count != 2)
				return false;
			IMethod parameterMethod = initCall.Method;
			CallInstruction typeArg = initCall.Arguments[0] as CallInstruction;
			ILInstruction nameArg = initCall.Arguments[1];
			if (!(parameterMethod.Name == "Parameter" && parameterMethod.DeclaringType.FullName == "System.Linq.Expressions.Expression"))
				return false;
			if (typeArg == null)
				return false;
			if (typeArg.Arguments.Count != 1)
				return false;
			IMethod getTypeFromHandle = typeArg.Method;
			ILInstruction typeToken = typeArg.Arguments[0];
			if (!(getTypeFromHandle.Name == "GetTypeFromHandle" && getTypeFromHandle.DeclaringType.FullName == "System.Type"))
				return false;
			return typeToken.OpCode == OpCode.LdTypeToken && nameArg.OpCode == OpCode.LdStr;
		}
		#endregion
	}
}
