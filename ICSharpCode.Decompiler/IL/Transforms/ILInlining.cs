// Copyright (c) 2011-2015 Daniel Grunwald
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
	public class ILInlining : IILTransform, IBlockTransform
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

		public static bool InlineAllInBlock(Block block, ILTransformContext context)
		{
			bool modified = false;
			int i = 0;
			while (i < block.Instructions.Count) {
				if (InlineOneIfPossible(block, i, aggressive: false, context: context)) {
					modified = true;
					i = Math.Max(0, i - 1);
					// Go back one step
				} else {
					i++;
				}
			}
			return modified;
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
		/// If inlining was possible; we will continue to inline (non-aggressively) into the the combined instruction.
		/// </summary>
		/// <remarks>
		/// After the operation, pos will point to the new combined instruction.
		/// </remarks>
		public static bool InlineIfPossible(Block block, ref int pos, ILTransformContext context)
		{
			if (InlineOneIfPossible(block, pos, true, context)) {
				pos -= InlineInto(block, pos, false, context);
				return true;
			}
			return false;
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
					if (!IsGeneratedValueTypeTemporary(next, loadInst.Parent, loadInst.ChildIndex, v, inlinedExpression))
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
		/// <param name="parent">The direct parent of the load within 'next'</param>
		/// <param name="pos">Index of the load within 'parent'</param>
		/// <param name="v">The variable being inlined.</param>
		static bool IsGeneratedValueTypeTemporary(ILInstruction next, ILInstruction parent, int pos, ILVariable v, ILInstruction inlinedExpression)
		{
			if (pos == 0 && v.Type != null && v.Type.IsReferenceType == false) {
				// Inlining a value type variable is allowed only if the resulting code will maintain the semantics
				// that the method is operating on a copy.
				// Thus, we have to disallow inlining of other locals, fields, array elements, dereferenced pointers
				switch (inlinedExpression.OpCode) {
					case OpCode.LdLoc:
					case OpCode.StLoc:
						return false;
					case OpCode.LdObj:
						// allow inlining field access only if it's a readonly field
						IField f = (((LdObj)inlinedExpression).Target as IInstructionWithFieldOperand)?.Field;
						if (f != null && f.IsReadOnly)
							return true;
						return f != null && f.IsReadOnly;
					case OpCode.Call:
						var m = ((CallInstruction)inlinedExpression).Method;
						// ensure that it's not an multi-dimensional array getter
						if (m.DeclaringType.Kind == TypeKind.Array)
							return false;
						goto case OpCode.CallVirt;
					case OpCode.CallVirt:
						// don't inline foreach loop variables:
						m = ((CallInstruction)inlinedExpression).Method;
						if (m.Name == "get_Current" && !m.IsStatic)
							return false;
						break;
					case OpCode.CastClass:
					case OpCode.UnboxAny:
						// These are valid, but might occur as part of a foreach loop variable.
						ILInstruction arg = inlinedExpression.Children[0];
						if (arg.OpCode == OpCode.Call || arg.OpCode == OpCode.CallVirt) {
							m = ((CallInstruction)arg).Method;
							if (m.Name == "get_Current" && !m.IsStatic)
								return false; // looks like a foreach loop variable, so don't inline it
						}
						break;
				}
				
				// inline the compiler-generated variable that are used when accessing a member on a value type:
				switch (parent.OpCode) {
					case OpCode.Call:
						return !((Call)parent).Method.IsStatic;
					case OpCode.CallVirt:
						return !((CallVirt)parent).Method.IsStatic;
					case OpCode.LdFlda:
					case OpCode.Await:
						return true;
				}
			}
			return false;
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
			
			// decide based on the target into which we are inlining
			var parent = loadInst.Parent;
			switch (next.OpCode) {
				case OpCode.Return:
					return parent == next;
				case OpCode.IfInstruction:
					while (parent.OpCode == OpCode.LogicNot) {
						parent = parent.Parent;
					}
					return parent == next;
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
	}
}
