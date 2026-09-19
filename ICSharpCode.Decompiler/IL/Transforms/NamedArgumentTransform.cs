// Copyright (c) 2018 Daniel Grunwald
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

using System.Diagnostics;
using System.Linq;
using System.Reflection;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	using FindResult = ILInlining.FindResult;
	using FindResultType = ILInlining.FindResultType;

	public class NamedArgumentTransform : IStatementTransform
	{
		/// <summary>Whether CallBuilder can name every argument. Reordering and then failing to
		/// name would emit the arguments positionally, in the order the reordering put them.</summary>
		static bool CanWriteArgumentNames(IMethod method, bool isSetter)
		{
			var namedParameters = CallBuilder.GetNamedParameters(method);
			if (namedParameters.Count < method.Parameters.Count - (isSetter ? 1 : 0))
				return false;
			return namedParameters.All(p => AssignVariableNames.IsValidName(p.Name));
		}

		internal static FindResult CanIntroduceNamedArgument(CallInstruction call, ILInstruction child, ILVariable v, ILInstruction expressionBeingMoved)
		{
			Debug.Assert(child.Parent == call);
			if (call.IsInstanceCall && child.ChildIndex == 0)
				return FindResult.Stop; // cannot use named arg to move expressionBeingMoved before this pointer
			if (call.Method.IsOperator)
				return FindResult.Stop; // cannot use named arg for operators
			if (call.Method.IsAccessor)
			{
				// Only an indexer access has an argument list that can carry names.
				if (call.Method.AccessorOwner!.SymbolKind != SymbolKind.Indexer)
					return FindResult.Stop;
				// A name replaces the call with a block: a call-inline-assign block is matched by
				// the call it holds, and a compound assignment requires a call in its target.
				if (call.Parent is Block { Kind: BlockKind.CallInlineAssign })
					return FindResult.Stop;
				if (call.Parent is CompoundAssignmentInstruction { TargetKind: CompoundTargetKind.Property } compoundAssignment
					&& compoundAssignment.Target == call)
				{
					return FindResult.Stop;
				}
			}
			if (call.Method is VarArgInstanceMethod)
				return FindResult.Stop; // CallBuilder doesn't support named args when using varargs
			if (call.Method.IsConstructor)
			{
				IType type = call.Method.DeclaringType;
				if (type.Kind == TypeKind.Delegate || type.IsAnonymousType())
					return FindResult.Stop;
			}
			// A setter's last argument is the assigned value, written as the right-hand side.
			bool isSetter = call.Method.AccessorKind == MethodSemanticsAttributes.Setter;
			if (!CanWriteArgumentNames(call.Method, isSetter))
				return FindResult.Stop;
			int nameableArgumentCount = call.Arguments.Count - (isSetter ? 1 : 0);
			for (int i = child.ChildIndex; i < nameableArgumentCount; i++)
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
			// A setter's last argument is the assigned value, written as the right-hand side.
			int nameableArgumentCount = call.Arguments.Count
				- (call.Method.AccessorKind == MethodSemanticsAttributes.Setter ? 1 : 0);
			for (int i = 0; i < nameableArgumentCount; i++)
			{
				if (call.Arguments[i].MatchLdLoc(v))
				{
					return FindResult.NamedArgument(call.Arguments[i], call.Arguments[i]);
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
			var type = arg.InferType(context.TypeSystem);
			var v = context.Function.RegisterVariable(VariableKind.NamedArgument, type);
			context.Step($"Introduce named argument '{v.Name}'", arg);
			if (!(call.Parent is Block namedArgBlock) || namedArgBlock.Kind != BlockKind.CallWithNamedArgs)
			{
				// create namedArgBlock:
				namedArgBlock = new Block(BlockKind.CallWithNamedArgs);
				call.ReplaceWith(namedArgBlock);
				namedArgBlock.FinalInstruction = call;
				if (call.IsInstanceCall)
				{
					IType thisVarType = call.ConstrainedTo ?? call.Method.DeclaringType;
					if (CallInstruction.ExpectedTypeForThisPointer(call.Method.DeclaringType, call.ConstrainedTo) == StackType.Ref)
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
			var newInst = new StLoc(v, arg);
			namedArgBlock.Instructions.Insert(call.IsInstanceCall ? 1 : 0, newInst);
			call.Arguments[argIndex] = new LdLoc(v);
			context.EndStep(newInst);
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
