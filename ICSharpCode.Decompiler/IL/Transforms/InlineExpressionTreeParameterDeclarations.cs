using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class InlineExpressionTreeParameterDeclarationsTransform : IStatementTransform
	{
		public bool TryInline(ILInstruction LdParameterVariableInst, ILVariable v, ILInstruction init)
		{
			ILInstruction parent = LdParameterVariableInst.Parent;
			while (parent != null) {
				if (parent is CallInstruction call
					&& call.Method.FullName.Equals("System.Linq.Expressions.Expression.Lambda")) {
					if (call.Parent is CallInstruction parentCall
						&& (parentCall.Method.FullName.Equals("System.Linq.Expressions.LambdaExpression.Compile")
						|| parentCall.Method.FullName.Equals("System.Linq.Expressions.Expression.Compile"))) {
						return false; // Compiled ParameterDeclarations can't be inlined.
					}
					LdParameterVariableInst.ReplaceWith(init.Clone());
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (block.Instructions[pos] is StLoc parameterVariableInst) {
				bool alwaysInlined = true;
				if (MatchParameterVariableAssignment(parameterVariableInst, out ILVariable v, out ILInstruction init)) {
					foreach (LdLoc LdParameterVariableInst in v.LoadInstructions.ToArray()) {
						if (!TryInline(LdParameterVariableInst, v, init)) {
							alwaysInlined = false;
						}
					}
					if (alwaysInlined)
						block.Instructions.RemoveAt(pos);
				}
			}
		}

		bool MatchParameterVariableAssignment(ILInstruction expr, out ILVariable v, out ILInstruction init)
		{
			// stloc(v, call(Expression::Parameter, call(Type::GetTypeFromHandle, ldtoken(...)), ldstr(...)))
			if (!expr.MatchStLoc(out v, out init))
				return false;
			if (!(v.Kind == VariableKind.Local || v.Kind == VariableKind.StackSlot))
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
	}
}
