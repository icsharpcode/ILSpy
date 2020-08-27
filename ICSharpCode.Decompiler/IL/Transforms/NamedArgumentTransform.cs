using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	using FindResult = ILInlining.FindResult;
	using FindResultType = ILInlining.FindResultType;

	public class NamedArgumentTransform : IStatementTransform
	{
		internal static FindResult CanIntroduceNamedArgument(CallInstruction call, ILInstruction child, ILVariable v, ILInstruction expressionBeingMoved)
		{
			Debug.Assert(child.Parent == call);
			if (call.IsInstanceCall && child.ChildIndex == 0)
				return FindResult.Stop; // cannot use named arg to move expressionBeingMoved before this pointer
			if (call.Method.IsOperator || call.Method.IsAccessor)
				return FindResult.Stop; // cannot use named arg for operators or accessors
			if (call.Method is VarArgInstanceMethod)
				return FindResult.Stop; // CallBuilder doesn't support named args when using varargs
			if (call.Method.IsConstructor)
			{
				IType type = call.Method.DeclaringType;
				if (type.Kind == TypeKind.Delegate || type.IsAnonymousType())
					return FindResult.Stop;
			}
			if (call.Method.Parameters.Any(p => string.IsNullOrEmpty(p.Name)))
				return FindResult.Stop; // cannot use named arguments
			for (int i = child.ChildIndex; i < call.Arguments.Count; i++)
			{
				var r = ILInlining.FindLoadInNext(call.Arguments[i], v, expressionBeingMoved, InliningOptions.None);
				if (r.Type == FindResultType.Found)
				{
					return FindResult.NamedArgument(r.LoadInst, call.Arguments[i]);
				}
			}
			return FindResult.Stop;
		}

		internal static FindResult CanExtendNamedArgument(Block block, ILVariable v, ILInstruction expressionBeingMoved)
		{
			Debug.Assert(block.Kind == BlockKind.CallWithNamedArgs);
			var firstArg = ((StLoc)block.Instructions[0]).Value;
			var r = ILInlining.FindLoadInNext(firstArg, v, expressionBeingMoved, InliningOptions.IntroduceNamedArguments);
			if (r.Type == FindResultType.Found || r.Type == FindResultType.NamedArgument)
			{
				return r; // OK, inline into first instruction of block
			}
			var call = (CallInstruction)block.FinalInstruction;
			if (call.IsInstanceCall)
			{
				// For instance calls, block.Instructions[0] is the argument
				// for the 'this' pointer. We can only insert at position 1.
				if (r.Type == FindResultType.Stop)
				{
					// error: can't move expressionBeingMoved after block.Instructions[0]
					return FindResult.Stop;
				}
				// Because we always ensure block.Instructions[0] is the 'this' argument,
				// it's possible that the place we actually need to inline into
				// is within block.Instructions[1]:
				if (block.Instructions.Count > 1)
				{
					r = ILInlining.FindLoadInNext(block.Instructions[1], v, expressionBeingMoved, InliningOptions.IntroduceNamedArguments);
					if (r.Type == FindResultType.Found || r.Type == FindResultType.NamedArgument)
					{
						return r; // OK, inline into block.Instructions[1]
					}
				}
			}
			foreach (var arg in call.Arguments)
			{
				if (arg.MatchLdLoc(v))
				{
					return FindResult.NamedArgument(arg, arg);
				}
			}
			return FindResult.Stop;
		}

		/// <summary>
		/// Introduce a named argument for 'arg' and evaluate it before the other arguments
		/// (except for the "this" pointer)
		/// </summary>
		internal static void IntroduceNamedArgument(ILInstruction arg, ILTransformContext context)
		{
			var call = (CallInstruction)arg.Parent;
			Debug.Assert(context.Function == call.Ancestors.OfType<ILFunction>().First());
			var v = context.Function.RegisterVariable(VariableKind.NamedArgument, arg.ResultType);
			context.Step($"Introduce named argument '{v.Name}'", arg);
			if (!(call.Parent is Block namedArgBlock) || namedArgBlock.Kind != BlockKind.CallWithNamedArgs)
			{
				// create namedArgBlock:
				namedArgBlock = new Block(BlockKind.CallWithNamedArgs);
				call.ReplaceWith(namedArgBlock);
				namedArgBlock.FinalInstruction = call;
				if (call.IsInstanceCall)
				{
					IType thisVarType = call.Method.DeclaringType;
					if (CallInstruction.ExpectedTypeForThisPointer(thisVarType) == StackType.Ref)
					{
						thisVarType = new ByReferenceType(thisVarType);
					}
					var thisArgVar = context.Function.RegisterVariable(VariableKind.NamedArgument, thisVarType, "this_arg");
					namedArgBlock.Instructions.Add(new StLoc(thisArgVar, call.Arguments[0]));
					call.Arguments[0] = new LdLoc(thisArgVar);
				}
			}
			int argIndex = arg.ChildIndex;
			Debug.Assert(call.Arguments[argIndex] == arg);
			namedArgBlock.Instructions.Insert(call.IsInstanceCall ? 1 : 0, new StLoc(v, arg));
			call.Arguments[argIndex] = new LdLoc(v);
		}

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.NamedArguments)
				return;
			var options = ILInlining.OptionsForBlock(block, pos, context);
			options |= InliningOptions.IntroduceNamedArguments;
			ILInlining.InlineOneIfPossible(block, pos, options, context: context);
		}
	}
}
