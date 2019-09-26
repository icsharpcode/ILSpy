// Copyright (c) 2011-2017 Daniel Grunwald
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

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	[Flags]
	public enum InliningOptions
	{
		None = 0,
		Aggressive = 1,
		IntroduceNamedArguments = 2,
	}

	/// <summary>
	/// Performs inlining transformations.
	/// </summary>
	public class ILInlining : IILTransform, IBlockTransform, IStatementTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var block in function.Descendants.OfType<Block>()) {
				InlineAllInBlock(function, block, context);
			}
			function.Variables.RemoveDead();
		}

		public void Run(Block block, BlockTransformContext context)
		{
			InlineAllInBlock(context.Function, block, context);
		}

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			InlineOneIfPossible(block, pos, OptionsForBlock(block, pos), context: context);
		}

		internal static InliningOptions OptionsForBlock(Block block, int pos)
		{
			InliningOptions options = InliningOptions.None;
			if (IsCatchWhenBlock(block))
				options |= InliningOptions.Aggressive;
			else {
				var function = block.Ancestors.OfType<ILFunction>().FirstOrDefault();
				var inst = block.Instructions[pos];
				if (IsInConstructorInitializer(function, inst))
					options |= InliningOptions.Aggressive;
			}
			return options;
		}

		public static bool InlineAllInBlock(ILFunction function, Block block, ILTransformContext context)
		{
			bool modified = false;
			var instructions = block.Instructions;
			for (int i = instructions.Count - 1; i >= 0; i--) {
				if (instructions[i] is StLoc inst) {
					InliningOptions options = InliningOptions.None;
					if (IsCatchWhenBlock(block) || IsInConstructorInitializer(function, inst))
						options = InliningOptions.Aggressive;
					if (InlineOneIfPossible(block, i, options, context)) {
						modified = true;
						continue;
					}
				}
			}
			return modified;
		}

		internal static bool IsInConstructorInitializer(ILFunction function, ILInstruction inst)
		{
			int ctorCallStart = function.ChainedConstructorCallILOffset;
			if (inst.EndILOffset > ctorCallStart)
				return false;
			var topLevelInst = inst.Ancestors.LastOrDefault(instr => instr.Parent is Block);
			if (topLevelInst == null)
				return false;
			return topLevelInst.EndILOffset <= ctorCallStart;
		}

		internal static bool IsCatchWhenBlock(Block block)
		{
			var container = BlockContainer.FindClosestContainer(block);
			return container?.Parent is TryCatchHandler handler
				&& handler.Filter == container;
		}

		/// <summary>
		/// Inlines instructions before pos into block.Instructions[pos].
		/// </summary>
		/// <returns>The number of instructions that were inlined.</returns>
		public static int InlineInto(Block block, int pos, InliningOptions options, ILTransformContext context)
		{
			if (pos >= block.Instructions.Count)
				return 0;
			int count = 0;
			while (--pos >= 0) {
				if (InlineOneIfPossible(block, pos, options, context))
					count++;
				else
					break;
			}
			return count;
		}
		
		/// <summary>
		/// Aggressively inlines the stloc instruction at block.Body[pos] into the next instruction, if possible.
		/// </summary>
		public static bool InlineIfPossible(Block block, int pos, ILTransformContext context)
		{
			return InlineOneIfPossible(block, pos, InliningOptions.Aggressive, context);
		}

		/// <summary>
		/// Inlines the stloc instruction at block.Instructions[pos] into the next instruction, if possible.
		/// </summary>
		public static bool InlineOneIfPossible(Block block, int pos, InliningOptions options, ILTransformContext context)
		{
			context.CancellationToken.ThrowIfCancellationRequested();
			StLoc stloc = block.Instructions[pos] as StLoc;
			if (stloc == null || stloc.Variable.Kind == VariableKind.PinnedLocal)
				return false;
			ILVariable v = stloc.Variable;
			// ensure the variable is accessed only a single time
			if (v.StoreCount != 1)
				return false;
			if (v.LoadCount > 1 || v.LoadCount + v.AddressCount != 1)
				return false;
			// TODO: inlining of small integer types might be semantically incorrect,
			// but we can't avoid it this easily without breaking lots of tests.
			//if (v.Type.IsSmallIntegerType())
			//	return false; // stloc might perform implicit truncation
			return InlineOne(stloc, options, context);
		}
		
		/// <summary>
		/// Inlines the stloc instruction at block.Instructions[pos] into the next instruction.
		/// 
		/// Note that this method does not check whether 'v' has only one use;
		/// the caller is expected to validate whether inlining 'v' has any effects on other uses of 'v'.
		/// </summary>
		public static bool InlineOne(StLoc stloc, InliningOptions options, ILTransformContext context)
		{
			ILVariable v = stloc.Variable;
			Block block = (Block)stloc.Parent;
			int pos = stloc.ChildIndex;
			if (DoInline(v, stloc.Value, block.Instructions.ElementAtOrDefault(pos + 1), options, context)) {
				// Assign the ranges of the stloc instruction:
				stloc.Value.AddILRange(stloc);
				// Remove the stloc instruction:
				Debug.Assert(block.Instructions[pos] == stloc);
				block.Instructions.RemoveAt(pos);
				return true;
			} else if (v.LoadCount == 0 && v.AddressCount == 0) {
				// The variable is never loaded
				if (SemanticHelper.IsPure(stloc.Value.Flags)) {
					// Remove completely if the instruction has no effects
					// (except for reading locals)
					context.Step("Remove dead store without side effects", stloc);
					block.Instructions.RemoveAt(pos);
					return true;
				} else if (v.Kind == VariableKind.StackSlot) {
					context.Step("Remove dead store, but keep expression", stloc);
					// Assign the ranges of the stloc instruction:
					stloc.Value.AddILRange(stloc);
					// Remove the stloc, but keep the inner expression
					stloc.ReplaceWith(stloc.Value);
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Inlines 'expr' into 'next', if possible.
		/// 
		/// Note that this method does not check whether 'v' has only one use;
		/// the caller is expected to validate whether inlining 'v' has any effects on other uses of 'v'.
		/// </summary>
		static bool DoInline(ILVariable v, ILInstruction inlinedExpression, ILInstruction next, InliningOptions options, ILTransformContext context)
		{
			var r = FindLoadInNext(next, v, inlinedExpression, options);
			if (r.Type == FindResultType.Found || r.Type == FindResultType.NamedArgument) {
				var loadInst = r.LoadInst;
				if (loadInst.OpCode == OpCode.LdLoca) {
					if (!IsGeneratedValueTypeTemporary((LdLoca)loadInst, v, inlinedExpression))
						return false;
				} else {
					Debug.Assert(loadInst.OpCode == OpCode.LdLoc);
					bool aggressive = (options & InliningOptions.Aggressive) != 0;
					if (!aggressive && v.Kind != VariableKind.StackSlot
						&& !NonAggressiveInlineInto(next, r, inlinedExpression, v)) {
						return false;
					}
				}

				if (r.Type == FindResultType.NamedArgument) {
					NamedArgumentTransform.IntroduceNamedArgument(r.CallArgument, context);
					// Now that the argument is evaluated early, we can inline as usual
				}

				context.Step($"Inline variable '{v.Name}'", inlinedExpression);
				// Assign the ranges of the ldloc instruction:
				inlinedExpression.AddILRange(loadInst);
				
				if (loadInst.OpCode == OpCode.LdLoca) {
					// it was an ldloca instruction, so we need to use the pseudo-opcode 'addressof'
					// to preserve the semantics of the compiler-generated temporary
					Debug.Assert(((LdLoca)loadInst).Variable == v);
					loadInst.ReplaceWith(new AddressOf(inlinedExpression, v.Type));
				} else {
					loadInst.ReplaceWith(inlinedExpression);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Is this a temporary variable generated by the C# compiler for instance method calls on value type values
		/// </summary>
		/// <param name="loadInst">The load instruction (a descendant within 'next')</param>
		/// <param name="v">The variable being inlined.</param>
		static bool IsGeneratedValueTypeTemporary(LdLoca loadInst, ILVariable v, ILInstruction inlinedExpression)
		{
			Debug.Assert(loadInst.Variable == v);
			// Inlining a value type variable is allowed only if the resulting code will maintain the semantics
			// that the method is operating on a copy.
			// Thus, we have to ensure we're operating on an r-value.
			// Additionally, we cannot inline in cases where the C# compiler prohibits the direct use
			// of the rvalue (e.g. M(ref (MyStruct)obj); is invalid).
			if (!IsUsedAsThisPointerInCall(loadInst))
				return false;
			switch (ClassifyExpression(inlinedExpression)) {
				case ExpressionClassification.RValue:
					// For struct method calls on rvalues, the C# compiler always generates temporaries.
					return true;
				case ExpressionClassification.MutableLValue:
					// For struct method calls on mutable lvalues, the C# compiler never generates temporaries.
					return false;
				case ExpressionClassification.ReadonlyLValue:
					// For struct method calls on readonly lvalues, the C# compiler
					// only generates a temporary if it isn't a "readonly struct"
					return !(v.Type.GetDefinition()?.IsReadOnly == true);
				default:
					throw new InvalidOperationException("invalid expression classification");
			}
		}

		internal static bool IsUsedAsThisPointerInCall(LdLoca ldloca)
		{
			if (ldloca.ChildIndex != 0)
				return false;
			if (ldloca.Variable.Type.IsReferenceType ?? false)
				return false;
			ILInstruction inst = ldloca;
			while (inst.Parent is LdFlda ldflda) {
				inst = ldflda;
			}
			switch (inst.Parent.OpCode) {
				case OpCode.Call:
				case OpCode.CallVirt:
					var method = ((CallInstruction)inst.Parent).Method;
					if (method.IsAccessor && method.AccessorKind != MethodSemanticsAttributes.Getter) {
						// C# doesn't allow calling setters on temporary structs
						return false;
					}
					return !method.IsStatic;
				case OpCode.Await:
					return true;
				case OpCode.NullableUnwrap:
					return ((NullableUnwrap)inst.Parent).RefInput;
				default:
					return false;
			}
		}
		
		internal enum ExpressionClassification
		{
			RValue,
			MutableLValue,
			ReadonlyLValue,
		}

		/// <summary>
		/// Gets whether the instruction, when converted into C#, turns into an l-value that can
		/// be used to mutate a value-type.
		/// If this function returns false, the C# compiler would introduce a temporary copy
		/// when calling a method on a value-type (and any mutations performed by the method will be lost)
		/// </summary>
		internal static ExpressionClassification ClassifyExpression(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.LdLoc:
				case OpCode.StLoc:
					if (IsReadonlyRefLocal(((IInstructionWithVariableOperand)inst).Variable)) {
						return ExpressionClassification.ReadonlyLValue;
					} else {
						return ExpressionClassification.MutableLValue;
					}
				case OpCode.LdObj:
					// ldobj typically refers to a storage location,
					// but readonly fields are an exception.
					if (IsReadonlyReference(((LdObj)inst).Target)) {
						return ExpressionClassification.ReadonlyLValue;
					} else {
						return ExpressionClassification.MutableLValue;
					}
				case OpCode.StObj:
					// stobj is the same as ldobj.
					if (IsReadonlyReference(((StObj)inst).Target)) {
						return ExpressionClassification.ReadonlyLValue;
					} else {
						return ExpressionClassification.MutableLValue;
					}
				case OpCode.Call:
					var m = ((CallInstruction)inst).Method;
					// multi-dimensional array getters are lvalues,
					// everything else is an rvalue.
					if (m.DeclaringType.Kind == TypeKind.Array) {
						return ExpressionClassification.MutableLValue;
					} else {
						return ExpressionClassification.RValue;
					}
				default:
					return ExpressionClassification.RValue; // most instructions result in an rvalue
			}
		}

		private static bool IsReadonlyReference(ILInstruction addr)
		{
			switch (addr) {
				case LdFlda ldflda:
					return ldflda.Field.IsReadOnly
						|| (ldflda.Field.DeclaringType.Kind == TypeKind.Struct && IsReadonlyReference(ldflda.Target));
				case LdsFlda ldsflda:
					return ldsflda.Field.IsReadOnly;
				case LdLoc ldloc:
					return IsReadonlyRefLocal(ldloc.Variable);
				case Call call:
					return call.Method.ReturnTypeIsRefReadOnly;
				case AddressOf _:
					// C# doesn't allow mutation of value-type temporaries
					return true;
				default:
					return false;
			}
		}

		private static bool IsReadonlyRefLocal(ILVariable variable)
		{
			if (variable.Kind == VariableKind.Parameter) {
				if (variable.Index == -1) {
					// this parameter in readonly struct
					return variable.Function.Method?.DeclaringTypeDefinition?.IsReadOnly == true;
				} else {
					return variable.Function.Parameters[variable.Index.Value].IsIn;
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether a variable should be inlined in non-aggressive mode, even though it is not a generated variable.
		/// </summary>
		/// <param name="next">The next top-level expression</param>
		/// <param name="v">The variable being eliminated by inlining.</param>
		/// <param name="inlinedExpression">The expression being inlined</param>
		static bool NonAggressiveInlineInto(ILInstruction next, FindResult findResult, ILInstruction inlinedExpression, ILVariable v)
		{
			if (findResult.Type == FindResultType.NamedArgument) {
				var originalStore = (StLoc)inlinedExpression.Parent;
				return !originalStore.ILStackWasEmpty;
			}
			Debug.Assert(findResult.Type == FindResultType.Found);

			var loadInst = findResult.LoadInst;
			Debug.Assert(loadInst.IsDescendantOf(next));
			
			// decide based on the source expression being inlined
			switch (inlinedExpression.OpCode) {
				case OpCode.DefaultValue:
				case OpCode.StObj:
				case OpCode.NumericCompoundAssign:
				case OpCode.UserDefinedCompoundAssign:
				case OpCode.Await:
					return true;
				case OpCode.LdLoc:
					if (v.StateMachineField == null && ((LdLoc)inlinedExpression).Variable.StateMachineField != null) {
						// Roslyn likes to put the result of fetching a state machine field into a temporary variable,
						// so inline more aggressively in such cases.
						return true;
					}
					break;
			}
			if (inlinedExpression.ResultType == StackType.Ref) {
				// VB likes to use ref locals for compound assignment
				// (the C# compiler uses ref stack slots instead).
				// We want to avoid unnecessary ref locals, so we'll always inline them if possible.
				return true;
			}

			var parent = loadInst.Parent;
			if (NullableLiftingTransform.MatchNullableCtor(parent, out _, out _)) {
				// inline into nullable ctor call in lifted operator
				parent = parent.Parent;
			}
			if (parent is ILiftableInstruction liftable && liftable.IsLifted) {
				return true; // inline into lifted operators
			}
			// decide based on the new parent into which we are inlining:
			switch (parent.OpCode) {
				case OpCode.NullCoalescingInstruction:
					if (NullableType.IsNullable(v.Type))
						return true; // inline nullables into ?? operator
					break;
				case OpCode.NullableUnwrap:
					return true; // inline into ?. operator
				case OpCode.UserDefinedLogicOperator:
				case OpCode.DynamicLogicOperatorInstruction:
					return true; // inline into (left slot of) user-defined && or || operator
				case OpCode.DynamicGetMemberInstruction:
				case OpCode.DynamicGetIndexInstruction:
					if (parent.Parent.OpCode == OpCode.DynamicCompoundAssign)
						return true; // inline into dynamic compound assignments
					break;
				case OpCode.DynamicCompoundAssign:
					return true;
				case OpCode.ArrayToPointer:
				case OpCode.LocAllocSpan:
					return true; // inline size-expressions into localloc.span
				case OpCode.Call:
				case OpCode.CallVirt:
					// Aggressive inline into property/indexer getter calls for compound assignment calls
					// (The compiler generates locals for these because it doesn't want to evalute the args twice for getter+setter)
					if (parent.SlotInfo == CompoundAssignmentInstruction.TargetSlot) {
						return true;
					}
					break;
			}
			// decide based on the top-level target instruction into which we are inlining:
			switch (next.OpCode) {
				case OpCode.Leave:
				case OpCode.YieldReturn:
					return parent == next;
				case OpCode.IfInstruction:
					while (parent.MatchLogicNot(out _)) {
						parent = parent.Parent;
					}
					return parent == next;
				case OpCode.BlockContainer:
					if (((BlockContainer)next).EntryPoint.Instructions[0] is SwitchInstruction switchInst) {
						next = switchInst;
						goto case OpCode.SwitchInstruction;
					} else {
						return false;
					}
				case OpCode.SwitchInstruction:
					if (parent == next)
						return true;
					if (parent.MatchBinaryNumericInstruction(BinaryNumericOperator.Sub) && parent.Parent == next)
						return true;
					if (parent is StringToInt stringToInt && stringToInt.Parent == next)
						return true;
					return false;
				default:
					return false;
			}
		}
		
		/// <summary>
		/// Gets whether 'expressionBeingMoved' can be inlined into 'expr'.
		/// </summary>
		public static bool CanInlineInto(ILInstruction expr, ILVariable v, ILInstruction expressionBeingMoved)
		{
			return FindLoadInNext(expr, v, expressionBeingMoved, InliningOptions.None).Type == FindResultType.Found;
		}

		internal enum FindResultType
		{
			/// <summary>
			/// Found a load; inlining is possible.
			/// </summary>
			Found,
			/// <summary>
			/// Load not found and re-ordering not possible. Stop the search.
			/// </summary>
			Stop,
			/// <summary>
			/// Load not found, but the expressionBeingMoved can be re-ordered with regards to the
			/// tested expression, so we may continue searching for the matching load.
			/// </summary>
			Continue,
			/// <summary>
			/// Found a load in call, but re-ordering not possible with regards to the
			/// other call arguments.
			/// Inlining is not possible, but we might convert the call to named arguments.
			/// </summary>
			NamedArgument,
		}

		internal readonly struct FindResult
		{
			public readonly FindResultType Type;
			public readonly ILInstruction LoadInst; // ldloc or ldloca instruction that loads the variable to be inlined
			public readonly ILInstruction CallArgument; // argument of call that needs to be promoted to a named argument

			private FindResult(FindResultType type, ILInstruction loadInst, ILInstruction callArg)
			{
				this.Type = type;
				this.LoadInst = loadInst;
				this.CallArgument = callArg;
			}

			public static readonly FindResult Stop = new FindResult(FindResultType.Stop, null, null);
			public static readonly FindResult Continue = new FindResult(FindResultType.Continue, null, null);

			public static FindResult Found(ILInstruction loadInst)
			{
				Debug.Assert(loadInst.OpCode == OpCode.LdLoc || loadInst.OpCode == OpCode.LdLoca);
				return new FindResult(FindResultType.Found, loadInst, null);
			}

			public static FindResult NamedArgument(ILInstruction loadInst, ILInstruction callArg)
			{
				Debug.Assert(loadInst.OpCode == OpCode.LdLoc || loadInst.OpCode == OpCode.LdLoca);
				Debug.Assert(callArg.Parent is CallInstruction);
				return new FindResult(FindResultType.NamedArgument, loadInst, callArg);
			}
		}

		/// <summary>
		/// Finds the position to inline to.
		/// </summary>
		/// <returns>true = found; false = cannot continue search; null = not found</returns>
		internal static FindResult FindLoadInNext(ILInstruction expr, ILVariable v, ILInstruction expressionBeingMoved, InliningOptions options)
		{
			if (expr == null)
				return FindResult.Stop;
			if (expr.MatchLdLoc(v) || expr.MatchLdLoca(v)) {
				// Match found, we can inline
				return FindResult.Found(expr);
			} else if (expr is Block block) {
				// Inlining into inline-blocks?
				switch (block.Kind) {
					case BlockKind.ArrayInitializer:
					case BlockKind.CollectionInitializer:
					case BlockKind.ObjectInitializer:
					case BlockKind.CallInlineAssign:
						// Allow inlining into the first instruction of the block
						if (block.Instructions.Count == 0)
							return FindResult.Stop;
						return NoContinue(FindLoadInNext(block.Instructions[0], v, expressionBeingMoved, options));
						// If FindLoadInNext() returns null, we still can't continue searching
						// because we can't inline over the remainder of the block.
					case BlockKind.CallWithNamedArgs:
						return NamedArgumentTransform.CanExtendNamedArgument(block, v, expressionBeingMoved);
					default:
						return FindResult.Stop;
				}
			} else if (expr is BlockContainer container && container.EntryPoint.IncomingEdgeCount == 1) {
				// Possibly a switch-container, allow inlining into the switch instruction:
				return NoContinue(FindLoadInNext(container.EntryPoint.Instructions[0], v, expressionBeingMoved, options));
				// If FindLoadInNext() returns null, we still can't continue searching
				// because we can't inline over the remainder of the blockcontainer.
			} else if (expr is NullableRewrap) {
				// Inlining into nullable.rewrap is OK unless the expression being inlined
				// contains a nullable.wrap that isn't being re-wrapped within the expression being inlined.
				if (expressionBeingMoved.HasFlag(InstructionFlags.MayUnwrapNull))
					return FindResult.Stop;
			}
			foreach (var child in expr.Children) {
				if (!child.SlotInfo.CanInlineInto)
					return FindResult.Stop;
				
				// Recursively try to find the load instruction
				FindResult r = FindLoadInNext(child, v, expressionBeingMoved, options);
				if (r.Type != FindResultType.Continue) {
					if (r.Type == FindResultType.Stop && (options & InliningOptions.IntroduceNamedArguments) != 0 && expr is CallInstruction call)
						return NamedArgumentTransform.CanIntroduceNamedArgument(call, child, v, expressionBeingMoved);
					return r;
				}
			}
			if (IsSafeForInlineOver(expr, expressionBeingMoved))
				return FindResult.Continue; // continue searching
			else
				return FindResult.Stop; // abort, inlining not possible
		}

		private static FindResult NoContinue(FindResult findResult)
		{
			if (findResult.Type == FindResultType.Continue)
				return FindResult.Stop;
			else
				return findResult;
		}

		/// <summary>
		/// Determines whether it is safe to move 'expressionBeingMoved' past 'expr'
		/// </summary>
		static bool IsSafeForInlineOver(ILInstruction expr, ILInstruction expressionBeingMoved)
		{
			return SemanticHelper.MayReorder(expressionBeingMoved, expr);
		}

		/// <summary>
		/// Finds the first call instruction within the instructions that were inlined into inst.
		/// </summary>
		internal static CallInstruction FindFirstInlinedCall(ILInstruction inst)
		{
			foreach (var child in inst.Children) {
				if (!child.SlotInfo.CanInlineInto)
					break;
				var call = FindFirstInlinedCall(child);
				if (call != null) {
					return call;
				}
			}
			return inst as CallInstruction;
		}

		/// <summary>
		/// Gets whether arg can be un-inlined out of stmt.
		/// </summary>
		internal static bool CanUninline(ILInstruction arg, ILInstruction stmt)
		{
			Debug.Assert(arg.IsDescendantOf(stmt));
			for (ILInstruction inst = arg; inst != stmt; inst = inst.Parent) {
				if (!inst.SlotInfo.CanInlineInto)
					return false;
				// Check whether re-ordering with predecessors is valid:
				int childIndex = inst.ChildIndex;
				for (int i = 0; i < childIndex; ++i) {
					ILInstruction predecessor = inst.Parent.Children[i];
					if (!SemanticHelper.MayReorder(arg, predecessor))
						return false;
				}
			}
			return true;
		}
	}
}
