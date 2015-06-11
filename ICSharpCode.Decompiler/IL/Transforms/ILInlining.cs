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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Cecil;
using ICSharpCode.Decompiler.IL;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Performs inlining transformations.
	/// </summary>
	public class ILInlining : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var block in function.Descendants.OfType<Block>()) {
				InlineAllInBlock(block);
			}
		}

		public bool InlineAllInBlock(Block block)
		{
			bool modified = false;
			int i = 0;
			while (i < block.Instructions.Count) {
				if (InlineOneIfPossible(block, i, aggressive: false)) {
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
		public int InlineInto(Block block, int pos, bool aggressive)
		{
			if (pos >= block.Instructions.Count)
				return 0;
			int count = 0;
			while (--pos >= 0) {
				if (InlineOneIfPossible(block, pos, aggressive))
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
		public bool InlineIfPossible(Block block, ref int pos)
		{
			if (InlineOneIfPossible(block, pos, true)) {
				pos -= InlineInto(block, pos, false);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Inlines the stloc instruction at block.Instructions[pos] into the next instruction, if possible.
		/// </summary>
		public bool InlineOneIfPossible(Block block, int pos, bool aggressive)
		{
			StLoc stloc = block.Instructions[pos] as StLoc;
			if (stloc != null && stloc.Variable.Kind != VariableKind.PinnedLocal) {
				ILVariable v = stloc.Variable;
				if (InlineIfPossible(v, stloc.Value, block.Instructions.ElementAtOrDefault(pos+1), aggressive)) {
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
						block.Instructions.RemoveAt(pos);
						return true;
					} else if (v.Kind == VariableKind.StackSlot) {
						// Assign the ranges of the stloc instruction:
						stloc.Value.AddILRange(stloc.ILRange);
						// Remove the stloc, but keep the inner expression
						stloc.ReplaceWith(stloc.Value);
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Inlines 'expr' into 'next', if possible.
		/// </summary>
		bool InlineIfPossible(ILVariable v, ILInstruction inlinedExpression, ILInstruction next, bool aggressive)
		{
			// ensure the variable is accessed only a single time
			if (v.StoreCount != 1)
				return false;
			if (v.LoadCount > 1 || v.LoadCount + v.AddressCount != 1)
				return false;
			
			ILInstruction loadInst;
			if (FindLoadInNext(next, v, inlinedExpression, out loadInst) == true) {
				if (loadInst.OpCode == OpCode.LdLoca) {
					//if (!IsGeneratedValueTypeTemporary((ILInstruction)next, loadInst, v, inlinedExpression))
					return false;
				} else {
					Debug.Assert(loadInst.OpCode == OpCode.LdLoc);
					if (!aggressive && v.Kind != VariableKind.StackSlot && !NonAggressiveInlineInto(next, loadInst, inlinedExpression))
						return false;
				}
				
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
		/*
		/// <summary>
		/// Is this a temporary variable generated by the C# compiler for instance method calls on value type values
		/// </summary>
		/// <param name="next">The next top-level expression</param>
		/// <param name="parent">The direct parent of the load within 'next'</param>
		/// <param name="pos">Index of the load within 'parent'</param>
		/// <param name="v">The variable being inlined.</param>
		/// <param name="inlinedExpression">The expression being inlined</param>
		bool IsGeneratedValueTypeTemporary(ILInstruction next, ILInstruction parent, int pos, ILVariable v, ILInstruction inlinedExpression)
		{
			if (pos == 0 && v.Type != null && v.Type.IsValueType) {
				// Inlining a value type variable is allowed only if the resulting code will maintain the semantics
				// that the method is operating on a copy.
				// Thus, we have to disallow inlining of other locals, fields, array elements, dereferenced pointers
				switch (inlinedExpression.Code) {
					case ILCode.Ldloc:
					case ILCode.Stloc:
					case ILCode.CompoundAssignment:
					case ILCode.Ldelem_Any:
					case ILCode.Ldelem_I:
					case ILCode.Ldelem_I1:
					case ILCode.Ldelem_I2:
					case ILCode.Ldelem_I4:
					case ILCode.Ldelem_I8:
					case ILCode.Ldelem_R4:
					case ILCode.Ldelem_R8:
					case ILCode.Ldelem_Ref:
					case ILCode.Ldelem_U1:
					case ILCode.Ldelem_U2:
					case ILCode.Ldelem_U4:
					case ILCode.Ldobj:
					case ILCode.Ldind_Ref:
						return false;
					case ILCode.Ldfld:
					case ILCode.Stfld:
					case ILCode.Ldsfld:
					case ILCode.Stsfld:
						// allow inlining field access only if it's a readonly field
						FieldDefinition f = ((FieldReference)inlinedExpression.Operand).Resolve();
						if (!(f != null && f.IsInitOnly))
							return false;
						break;
					case ILCode.Call:
					case ILCode.CallGetter:
						// inlining runs both before and after IntroducePropertyAccessInstructions,
						// so we have to handle both 'call' and 'callgetter'
						MethodReference mr = (MethodReference)inlinedExpression.Operand;
						// ensure that it's not an multi-dimensional array getter
						if (mr.DeclaringType is ArrayType)
							return false;
						goto case ILCode.Callvirt;
					case ILCode.Callvirt:
					case ILCode.CallvirtGetter:
						// don't inline foreach loop variables:
						mr = (MethodReference)inlinedExpression.Operand;
						if (mr.Name == "get_Current" && mr.HasThis)
							return false;
						break;
					case ILCode.Castclass:
					case ILCode.Unbox_Any:
						// These are valid, but might occur as part of a foreach loop variable.
						ILInstruction arg = inlinedExpression.Arguments[0];
						if (arg.Code == ILCode.CallGetter || arg.Code == ILCode.CallvirtGetter || arg.Code == ILCode.Call || arg.Code == ILCode.Callvirt) {
							mr = (MethodReference)arg.Operand;
							if (mr.Name == "get_Current" && mr.HasThis)
								return false; // looks like a foreach loop variable, so don't inline it
						}
						break;
				}
				
				// inline the compiler-generated variable that are used when accessing a member on a value type:
				switch (parent.Code) {
					case ILCode.Call:
					case ILCode.CallGetter:
					case ILCode.CallSetter:
					case ILCode.Callvirt:
					case ILCode.CallvirtGetter:
					case ILCode.CallvirtSetter:
						MethodReference mr = (MethodReference)parent.Operand;
						return mr.HasThis;
					case ILCode.Stfld:
					case ILCode.Ldfld:
					case ILCode.Ldflda:
					case ILCode.Await:
						return true;
				}
			}
			return false;
		}
		*/
		
		/// <summary>
		/// Determines whether a variable should be inlined in non-aggressive mode, even though it is not a generated variable.
		/// </summary>
		/// <param name="next">The next top-level expression</param>
		/// <param name="loadInst">The load within 'next'</param>
		/// <param name="inlinedExpression">The expression being inlined</param>
		bool NonAggressiveInlineInto(ILInstruction next, ILInstruction loadInst, ILInstruction inlinedExpression)
		{
			Debug.Assert(loadInst.IsDescendantOf(next));
			
			if (inlinedExpression.OpCode == OpCode.DefaultValue)
				return true;
			
			var parent = loadInst.Parent;
			switch (next.OpCode) {
				case OpCode.Return:
					return parent == next;
				case OpCode.IfInstruction:
					return parent == next || (parent.OpCode == OpCode.LogicNot && parent.Parent == next);
				case OpCode.SwitchInstruction:
					return parent == next || (parent.OpCode == OpCode.Sub && parent.Parent == next);
				default:
					return false;
			}
		}
		
		/// <summary>
		/// Gets whether 'expressionBeingMoved' can be inlined into 'expr'.
		/// </summary>
		public bool CanInlineInto(ILInstruction expr, ILVariable v, ILInstruction expressionBeingMoved)
		{
			ILInstruction loadInst;
			return FindLoadInNext(expr, v, expressionBeingMoved, out loadInst) == true;
		}
		
		/// <summary>
		/// Finds the position to inline to.
		/// </summary>
		/// <returns>true = found; false = cannot continue search; null = not found</returns>
		bool? FindLoadInNext(ILInstruction expr, ILVariable v, ILInstruction expressionBeingMoved, out ILInstruction loadInst)
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
		bool IsSafeForInlineOver(ILInstruction expr, ILInstruction expressionBeingMoved)
		{
			ILVariable v;
			if (expr.MatchLdLoc(out v) && v.AddressCount == 0) {
				// MayReorder() only looks at flags, so it doesn't
				// allow reordering 'stloc x y; ldloc v' to 'ldloc v; stloc x y'
				
				// We'll allow the reordering unless x==v
				if (expressionBeingMoved.HasFlag(InstructionFlags.MayWriteLocals)) {
					foreach (var stloc in expressionBeingMoved.Descendants.OfType<StLoc>()) {
						if (stloc.Variable == v)
							return false;
					}
				}
				return true;
			}
			return SemanticHelper.MayReorder(expressionBeingMoved.Flags, expr.Flags);
		}
		/*
		/// <summary>
		/// Runs a very simple form of copy propagation.
		/// Copy propagation is used in two cases:
		/// 1) assignments from arguments to local variables
		///    If the target variable is assigned to only once (so always is that argument) and the argument is never changed (no ldarga/starg),
		///    then we can replace the variable with the argument.
		/// 2) assignments of address-loading instructions to local variables
		/// </summary>
		public void CopyPropagation()
		{
			foreach (ILBlock block in method.GetSelfAndChildrenRecursive<ILBlock>()) {
				for (int i = 0; i < block.Body.Count; i++) {
					ILVariable v;
					ILInstruction copiedExpr;
					if (block.Body[i].Match(ILCode.Stloc, out v, out copiedExpr)
					    && !v.IsParameter && numStloc.GetOrDefault(v) == 1 && numLdloca.GetOrDefault(v) == 0
					    && CanPerformCopyPropagation(copiedExpr, v))
					{
						// un-inline the arguments of the ldArg instruction
						ILVariable[] uninlinedArgs = new ILVariable[copiedExpr.Arguments.Count];
						for (int j = 0; j < uninlinedArgs.Length; j++) {
							uninlinedArgs[j] = new ILVariable { IsGenerated = true, Name = v.Name + "_cp_" + j };
							block.Body.Insert(i++, new ILInstruction(ILCode.Stloc, uninlinedArgs[j], copiedExpr.Arguments[j]));
						}
						
						// perform copy propagation:
						foreach (var expr in method.GetSelfAndChildrenRecursive<ILInstruction>()) {
							if (expr.Code == ILCode.Ldloc && expr.Operand == v) {
								expr.Code = copiedExpr.Code;
								expr.Operand = copiedExpr.Operand;
								for (int j = 0; j < uninlinedArgs.Length; j++) {
									expr.Arguments.Add(new ILInstruction(ILCode.Ldloc, uninlinedArgs[j]));
								}
							}
						}
						
						block.Body.RemoveAt(i);
						if (uninlinedArgs.Length > 0) {
							// if we un-inlined stuff; we need to update the usage counters
							AnalyzeMethod();
						}
						InlineInto(block.Body, i, aggressive: false); // maybe inlining gets possible after the removal of block.Body[i]
						i -= uninlinedArgs.Length + 1;
					}
				}
			}
		}
		
		bool CanPerformCopyPropagation(ILInstruction expr, ILVariable copyVariable)
		{
			switch (expr.Code) {
				case ILCode.Ldloca:
				case ILCode.Ldelema:
				case ILCode.Ldflda:
				case ILCode.Ldsflda:
					// All address-loading instructions always return the same value for a given operand/argument combination,
					// so they can be safely copied.
					return true;
				case ILCode.Ldloc:
					ILVariable v = (ILVariable)expr.Operand;
					if (v.IsParameter) {
						// Parameters can be copied only if they aren't assigned to (directly or indirectly via ldarga)
						return numLdloca.GetOrDefault(v) == 0 && numStloc.GetOrDefault(v) == 0;
					} else {
						// Variables are be copied only if both they and the target copy variable are generated,
						// and if the variable has only a single assignment
						return v.IsGenerated && copyVariable.IsGenerated && numLdloca.GetOrDefault(v) == 0 && numStloc.GetOrDefault(v) == 1;
					}
				default:
					return false;
			}
		}*/
	}
}
