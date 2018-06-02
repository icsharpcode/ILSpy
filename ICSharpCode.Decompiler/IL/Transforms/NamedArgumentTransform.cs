using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	using FindResult = ILInlining.FindResult;

	class NamedArgumentTransform : IStatementTransform
	{
		public static FindResult CanIntroduceNamedArgument(CallInstruction call, ILInstruction child, ILVariable v, out ILInstruction loadInst)
		{
			loadInst = null;
			Debug.Assert(child.Parent == call);
			if (call.IsInstanceCall && child.ChildIndex == 0)
				return FindResult.Stop; // cannot use named arg to move expressionBeingMoved before this pointer
			if (call.Method.IsOperator || call.Method.IsAccessor)
				return FindResult.Stop; // cannot use named arg for operators or accessors
			if (call.Method is VarArgInstanceMethod)
				return FindResult.Stop; // CallBuilder doesn't support named args when using varargs
			if (call.Method.IsConstructor) {
				IType type = call.Method.DeclaringType;
				if (type.Kind == TypeKind.Delegate || type.IsAnonymousType())
					return FindResult.Stop;
			}
			if (call.Method.Parameters.Any(p => string.IsNullOrEmpty(p.Name)))
				return FindResult.Stop; // cannot use named arguments
			for (int i = child.ChildIndex; i < call.Arguments.Count; i++) {
				if (call.Arguments[i] is LdLoc ldloc && ldloc.Variable == v) {
					loadInst = ldloc;
					return FindResult.NamedArgument;
				}
			}
			return FindResult.Stop;
		}

		internal static FindResult CanExtendNamedArgument(Block block, ILVariable v, ILInstruction expressionBeingMoved, out ILInstruction loadInst)
		{
			Debug.Assert(block.Kind == BlockKind.CallWithNamedArgs);
			var firstArg = ((StLoc)block.Instructions[0]).Value;
			var r = ILInlining.FindLoadInNext(firstArg, v, expressionBeingMoved, out loadInst);
			if (r == FindResult.Found || r == FindResult.NamedArgument) {
				return r; // OK, inline into first instruction of block
			}
			var call = (CallInstruction)block.FinalInstruction;
			if (call.IsInstanceCall) {
				// For instance calls, block.Instructions[0] is the argument
				// for the 'this' pointer. We can only insert at position 1.
				if (r == FindResult.Stop) {
					// error: can't move expressionBeingMoved after block.Instructions[0]
					return FindResult.Stop;
				}
				// Because we always ensure block.Instructions[0] is the 'this' argument,
				// it's possible that the place we actually need to inline into
				// is within block.Instructions[1]:
				if (block.Instructions.Count > 1) {
					r = ILInlining.FindLoadInNext(block.Instructions[1], v, expressionBeingMoved, out loadInst);
					if (r == FindResult.Found || r == FindResult.NamedArgument) {
						return r; // OK, inline into block.Instructions[1]
					}
				}
			}
			foreach (var arg in call.Arguments) {
				if (arg.MatchLdLoc(v)) {
					loadInst = arg;
					return FindResult.NamedArgument;
				}
			}
			return FindResult.Stop;
		}

		internal static bool DoInline(ILVariable v, StLoc originalStore, LdLoc loadInst, InliningOptions options, ILTransformContext context)
		{
			if ((options & InliningOptions.Aggressive) == 0 && originalStore.ILStackWasEmpty)
				return false;
			context.Step($"Introduce named argument '{v.Name}'", originalStore);
			var call = (CallInstruction)loadInst.Parent;
			if (!(call.Parent is Block namedArgBlock) || namedArgBlock.Kind != BlockKind.CallWithNamedArgs) {
				// create namedArgBlock:
				namedArgBlock = new Block(BlockKind.CallWithNamedArgs);
				call.ReplaceWith(namedArgBlock);
				namedArgBlock.FinalInstruction = call;
				if (call.IsInstanceCall) {
					IType thisVarType = call.Method.DeclaringType;
					if (CallInstruction.ExpectedTypeForThisPointer(thisVarType) == StackType.Ref) {
						thisVarType = new ByReferenceType(thisVarType);
					}
					var function = call.Ancestors.OfType<ILFunction>().First();
					var thisArgVar = function.RegisterVariable(VariableKind.NamedArgument, thisVarType, "this_arg");
					namedArgBlock.Instructions.Add(new StLoc(thisArgVar, call.Arguments[0]));
					call.Arguments[0] = new LdLoc(thisArgVar);
				}
			}
			v.Kind = VariableKind.NamedArgument;
			namedArgBlock.Instructions.Insert(call.IsInstanceCall ? 1 : 0, originalStore);
			return true;
		}

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.NamedArguments)
				return;
			var options = ILInlining.OptionsForBlock(block);
			options |= InliningOptions.IntroduceNamedArguments;
			ILInlining.InlineOneIfPossible(block, pos, options, context: context);
		}
	}
}
