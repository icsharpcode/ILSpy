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
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Performs inlining transformations.
	/// </summary>
	public class ILInlining : IILTransform, IBlockTransform, IStatementTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var block in function.Descendants.OfType<Block>()) {
				InlineAllInBlock(block, context);
			}
			function.Variables.RemoveDead();
		}

		public void Run(Block block, BlockTransformContext context)
		{
			InlineAllInBlock(block, context);
		}

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			InlineOneIfPossible(block, pos, aggressive: IsCatchWhenBlock(block), context: context);
		}

		public static bool InlineAllInBlock(Block block, ILTransformContext context)
		{
			bool modified = false;
			int i = 0;
			while (i < block.Instructions.Count) {
				if (InlineOneIfPossible(block, i, aggressive: IsCatchWhenBlock(block), context: context)) {
					modified = true;
					i = Math.Max(0, i - 1);
					// Go back one step
				} else {
					i++;
				}
			}
			return modified;
		}

		static bool IsCatchWhenBlock(Block block)
		{
			var container = BlockContainer.FindClosestContainer(block);
			return container?.Parent is TryCatchHandler handler
				&& handler.Filter == container;
		}

		/// <summary>
		/// Inlines instructions before pos into block.Instructions[pos].
		/// </summary>
		/// <returns>The number of instructions that were inlined.</returns>
		public static int InlineInto(Block block, int pos, bool aggressive, ILTransformContext context)
		{
			if (pos >= block.Instructions.Count)
				return 0;
			int count = 0;
			while (--pos >= 0) {
				if (InlineOneIfPossible(block, pos, aggressive, context))
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
			return InlineOneIfPossible(block, pos, true, context);
		}

		/// <summary>
		/// Inlines the stloc instruction at block.Instructions[pos] into the next instruction, if possible.
		/// </summary>
		public static bool InlineOneIfPossible(Block block, int pos, bool aggressive, ILTransformContext context)
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
			return InlineOne(stloc, aggressive, context);
		}
		
		/// <summary>
		/// Inlines the stloc instruction at block.Instructions[pos] into the next instruction.
		/// 
		/// Note that this method does not check whether 'v' has only one use;
		/// the caller is expected to validate whether inlining 'v' has any effects on other uses of 'v'.
		/// </summary>
		public static bool InlineOne(StLoc stloc, bool aggressive, ILTransformContext context)
		{
			ILVariable v = stloc.Variable;
			Block block = (Block)stloc.Parent;
			int pos = stloc.ChildIndex;
			if (DoInline(v, stloc.Value, block.Instructions.ElementAtOrDefault(pos + 1), aggressive, context)) {
				// Assign the ranges of the stloc instruction:
				stloc.Value.AddILRange(stloc.ILRange);
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
					stloc.Value.AddILRange(stloc.ILRange);
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
		static bool DoInline(ILVariable v, ILInstruction inlinedExpression, ILInstruction next, bool aggressive, ILTransformContext context)
		{
			ILInstruction loadInst;
			if (FindLoadInNext(next, v, inlinedExpression, out loadInst) == true) {
				if (loadInst.OpCode == OpCode.LdLoca) {
					if (!IsGeneratedValueTypeTemporary(next, (LdLoca)loadInst, v, inlinedExpression))
						return false;
				} else {
					Debug.Assert(loadInst.OpCode == OpCode.LdLoc);
					if (!aggressive && v.Kind != VariableKind.StackSlot && !NonAggressiveInlineInto(next, loadInst, inlinedExpression, v))
						return false;
				}

				context.Step($"Inline variable '{v.Name}'", inlinedExpression);
				// Assign the ranges of the ldloc instruction:
				inlinedExpression.AddILRange(loadInst.ILRange);
				
				if (loadInst.OpCode == OpCode.LdLoca) {
					// it was an ldloca instruction, so we need to use the pseudo-opcode 'addressof'
					// to preserve the semantics of the compiler-generated temporary
					loadInst.ReplaceWith(new AddressOf(inlinedExpression));
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
		/// <param name="next">The next top-level expression</param>
		/// <param name="loadInst">The load instruction (a descendant within 'next')</param>
		/// <param name="v">The variable being inlined.</param>
		static bool IsGeneratedValueTypeTemporary(ILInstruction next, LdLoca loadInst, ILVariable v, ILInstruction inlinedExpression)
		{
			Debug.Assert(loadInst.Variable == v);
			// Inlining a value type variable is allowed only if the resulting code will maintain the semantics
			// that the method is operating on a copy.
			// Thus, we have to ensure we're operating on an r-value.
			// Additionally, we cannot inline in cases where the C# compiler prohibits the direct use
			// of the rvalue (e.g. M(ref (MyStruct)obj); is invalid).
			return IsUsedAsThisPointerInCall(loadInst) && !IsLValue(inlinedExpression);
		}

		internal static bool IsUsedAsThisPointerInCall(LdLoca ldloca)
		{
			if (ldloca.ChildIndex != 0)
				return false;
			if (ldloca.Variable.Type.IsReferenceType ?? false)
				return false;
			switch (ldloca.Parent.OpCode) {
				case OpCode.Call:
				case OpCode.CallVirt:
					return !((CallInstruction)ldloca.Parent).Method.IsStatic;
				case OpCode.Await:
					return true;
				default:
					return false;
			}
		}
		
		/// <summary>
		/// Gets whether the instruction, when converted into C#, turns into an l-value that can
		/// be used to mutate a value-type.
		/// If this function returns false, the C# compiler would introduce a temporary copy
		/// when calling a method on a value-type (and any mutations performed by the method will be lost)
		/// </summary>
		static bool IsLValue(ILInstruction inst)
		{
			switch (inst.OpCode) {
				case OpCode.LdLoc:
				case OpCode.StLoc:
					return true;
				case OpCode.LdObj:
					// ldobj typically refers to a storage location,
					// but readonly fields are an exception.
					IField f = (((LdObj)inst).Target as IInstructionWithFieldOperand)?.Field;
					return !(f != null && f.IsReadOnly);
				case OpCode.StObj:
					// stobj is the same as ldobj.
					f = (((StObj)inst).Target as IInstructionWithFieldOperand)?.Field;
					return !(f != null && f.IsReadOnly);
				case OpCode.Call:
					var m = ((CallInstruction)inst).Method;
					// multi-dimensional array getters are lvalues,
					// everything else is an rvalue.
					return m.DeclaringType.Kind == TypeKind.Array;
				default:
					return false; // most instructions result in an rvalue
			}
		}

		/// <summary>
		/// Determines whether a variable should be inlined in non-aggressive mode, even though it is not a generated variable.
		/// </summary>
		/// <param name="next">The next top-level expression</param>
		/// <param name="loadInst">The load within 'next'</param>
		/// <param name="inlinedExpression">The expression being inlined</param>
		static bool NonAggressiveInlineInto(ILInstruction next, ILInstruction loadInst, ILInstruction inlinedExpression, ILVariable v)
		{
			Debug.Assert(loadInst.IsDescendantOf(next));
			
			// decide based on the source expression being inlined
			switch (inlinedExpression.OpCode) {
				case OpCode.DefaultValue:
				case OpCode.StObj:
				case OpCode.CompoundAssignmentInstruction:
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
			
			var parent = loadInst.Parent;
			if (NullableLiftingTransform.MatchNullableCtor(parent, out _, out _)) {
				// inline into nullable ctor call in lifted operator
				parent = parent.Parent;
			}
			if (parent is ILiftableInstruction liftable && liftable.IsLifted) {
				return true; // inline into lifted operators
			}
			if (parent is NullCoalescingInstruction && NullableType.IsNullable(v.Type)) {
				return true; // inline nullables into ?? operator
			}
			// decide based on the target into which we are inlining
			switch (next.OpCode) {
				case OpCode.Leave:
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
					return parent == next || (parent.MatchBinaryNumericInstruction(BinaryNumericOperator.Sub) && parent.Parent == next);
				default:
					return false;
			}
		}
		
		/// <summary>
		/// Gets whether 'expressionBeingMoved' can be inlined into 'expr'.
		/// </summary>
		public static bool CanInlineInto(ILInstruction expr, ILVariable v, ILInstruction expressionBeingMoved)
		{
			ILInstruction loadInst;
			return FindLoadInNext(expr, v, expressionBeingMoved, out loadInst) == true;
		}

		/// <summary>
		/// Finds the position to inline to.
		/// </summary>
		/// <returns>true = found; false = cannot continue search; null = not found</returns>
		static bool? FindLoadInNext(ILInstruction expr, ILVariable v, ILInstruction expressionBeingMoved, out ILInstruction loadInst)
		{
			loadInst = null;
			if (expr == null)
				return false;
			if (expr.MatchLdLoc(v) || expr.MatchLdLoca(v)) {
				// Match found, we can inline
				loadInst = expr;
				return true;
			} else if (expr is Block block && block.Instructions.Count > 0) {
				// Inlining into inline-blocks? only for some block types, and only into the first instruction.
				switch (block.Kind) {
					case BlockKind.ArrayInitializer:
					case BlockKind.CollectionInitializer:
					case BlockKind.ObjectInitializer:
						return FindLoadInNext(block.Instructions[0], v, expressionBeingMoved, out loadInst) ?? false;
						// If FindLoadInNext() returns null, we still can't continue searching
						// because we can't inline over the remainder of the block.
					default:
						return false;
				}
			} else if (expr is BlockContainer container && container.EntryPoint.IncomingEdgeCount == 1) {
				// Possibly a switch-container, allow inlining into the switch instruction:
				return FindLoadInNext(container.EntryPoint.Instructions[0], v, expressionBeingMoved, out loadInst) ?? false;
				// If FindLoadInNext() returns null, we still can't continue searching
				// because we can't inline over the remainder of the blockcontainer.
			}
			foreach (var child in expr.Children) {
				if (!child.SlotInfo.CanInlineInto)
					return false;
				
				// Recursively try to find the load instruction
				bool? r = FindLoadInNext(child, v, expressionBeingMoved, out loadInst);
				if (r != null)
					return r;
			}
			if (IsSafeForInlineOver(expr, expressionBeingMoved))
				return null; // continue searching
			else
				return false; // abort, inlining not possible
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
