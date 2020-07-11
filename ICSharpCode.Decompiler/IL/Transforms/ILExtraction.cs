// Copyright (c) 2019 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Context object for the ILInstruction.Extract() operation.
	/// </summary>
	class ExtractionContext
	{
		/// <summary>
		/// Nearest function, used for registering the new locals that are created by extraction.
		/// </summary>
		readonly ILFunction Function;

		/// <summary>
		/// Combined flags of all instructions being moved.
		/// </summary>
		internal InstructionFlags FlagsBeingMoved;

		/// <summary>
		/// List of actions to be executed when performing the extraction.
		/// 
		/// Each function in this list has the side-effect of replacing the instruction-to-be-moved
		/// with a load of a fresh temporary variable; and returns the the store to the temporary variable,
		/// which will be inserted at block-level.
		/// </summary>
		readonly List<Func<ILInstruction>> MoveActions = new List<Func<ILInstruction>>();

		ExtractionContext(ILFunction function)
		{
			Debug.Assert(function != null);
			this.Function = function;
		}

		internal void RegisterMove(ILInstruction predecessor)
		{
			FlagsBeingMoved |= predecessor.Flags;
			MoveActions.Add(delegate {
				var v = Function.RegisterVariable(VariableKind.StackSlot, predecessor.ResultType);
				predecessor.ReplaceWith(new LdLoc(v));
				return new StLoc(v, predecessor);
			});
		}

		internal void RegisterMoveIfNecessary(ILInstruction predecessor)
		{
			if (!CanReorderWithInstructionsBeingMoved(predecessor)) {
				RegisterMove(predecessor);
			}
		}

		/// <summary>
		/// Currently, <c>predecessor</c> is evaluated before the instructions being moved.
		/// If this function returns true, <c>predecessor</c> can stay as-is, despite the move changing the evaluation order.
		/// If this function returns false, <c>predecessor</c> will need to also move, to ensure the evaluation order stays unchanged.
		/// </summary>
		public bool CanReorderWithInstructionsBeingMoved(ILInstruction predecessor)
		{
			// We could track the instructions being moved and be smarter about unnecessary moves,
			// but given the limited scenarios where extraction is used so far,
			// this seems unnecessary.
			return predecessor.Flags == InstructionFlags.None;
		}

		/// <summary>
		/// Extracts the specified instruction:
		///   The instruction is replaced with a load of a new temporary variable;
		///   and the instruction is moved to a store to said variable at block-level.
		/// 
		/// May return null if extraction is not possible.
		/// </summary>
		public static ILVariable Extract(ILInstruction instToExtract)
		{
			var function = instToExtract.Ancestors.OfType<ILFunction>().First();
			ExtractionContext ctx = new ExtractionContext(function);
			ctx.FlagsBeingMoved = instToExtract.Flags;
			ILInstruction inst = instToExtract;
			while (inst != null) {
				if (inst.Parent is IfInstruction ifInst && inst.SlotInfo != IfInstruction.ConditionSlot) {
					// this context doesn't support extraction, but maybe we can create a block here?
					if (ifInst.ResultType == StackType.Void) {
						Block newBlock = new Block();
						inst.ReplaceWith(newBlock);
						newBlock.Instructions.Add(inst);
					}
				}
				if (inst.Parent is Block block && block.Kind == BlockKind.ControlFlow) {
					// We've reached the target block, and extraction is possible all the way.
					int insertIndex = inst.ChildIndex;
					// Move instToExtract itself:
					var v = function.RegisterVariable(VariableKind.StackSlot, instToExtract.ResultType);
					instToExtract.ReplaceWith(new LdLoc(v));
					block.Instructions.Insert(insertIndex, new StLoc(v, instToExtract));
					// Apply the other move actions:
					foreach (var moveAction in ctx.MoveActions) {
						block.Instructions.Insert(insertIndex, moveAction());
					}
					return v;
				}
				if (!inst.Parent.PrepareExtract(inst.ChildIndex, ctx))
					return null;
				inst = inst.Parent;
			}
			return null;
		}
	}
}
