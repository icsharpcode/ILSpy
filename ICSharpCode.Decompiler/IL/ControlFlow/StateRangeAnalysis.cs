// Copyright (c) 2012 AlphaSierraPapa for the SharpDevelop Team
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	enum StateRangeAnalysisMode
	{
		IteratorMoveNext,
		IteratorDispose,
		AsyncMoveNext
	}

	/// <summary>
	/// Symbolically executes code to determine which blocks are reachable for which values
	/// of the 'state' field.
	/// </summary>
	/// <remarks>
	/// Assumption: there are no loops/backward jumps
	/// We 'run' the code, with "state" being a symbolic variable
	/// so it can form expressions like "state + x" (when there's a sub instruction)
	/// 
	/// For each block, we maintain the set of values for state for which the block is reachable.
	/// This is (int.MinValue, int.MaxValue) for the first instruction.
	/// These ranges are propagated depending on the conditional jumps performed by the code.
	/// </remarks>
	class StateRangeAnalysis
	{
		public CancellationToken CancellationToken;
		readonly StateRangeAnalysisMode mode;
		readonly IField stateField;
		readonly SymbolicEvaluationContext evalContext;

		readonly Dictionary<Block, LongSet> ranges = new Dictionary<Block, LongSet>();
		readonly internal Dictionary<IMethod, LongSet> finallyMethodToStateRange; // used only for IteratorDispose

		internal ILVariable doFinallyBodies;
		internal ILVariable skipFinallyBodies;

		public StateRangeAnalysis(StateRangeAnalysisMode mode, IField stateField, ILVariable cachedStateVar = null)
		{
			this.mode = mode;
			this.stateField = stateField;
			if (mode == StateRangeAnalysisMode.IteratorDispose)
			{
				finallyMethodToStateRange = new Dictionary<IMethod, LongSet>();
			}

			evalContext = new SymbolicEvaluationContext(stateField);
			if (cachedStateVar != null)
				evalContext.AddStateVariable(cachedStateVar);
		}

		public IEnumerable<ILVariable> CachedStateVars { get => evalContext.StateVariables; }

		/// <summary>
		/// Creates a new StateRangeAnalysis with the same settings, including any cached state vars
		/// discovered by this analysis.
		/// However, the new analysis has a fresh set of result ranges.
		/// </summary>
		internal StateRangeAnalysis CreateNestedAnalysis()
		{
			var sra = new StateRangeAnalysis(mode, stateField);
			sra.doFinallyBodies = this.doFinallyBodies;
			sra.skipFinallyBodies = this.skipFinallyBodies;
			foreach (var v in this.evalContext.StateVariables)
			{
				sra.evalContext.AddStateVariable(v);
			}
			return sra;
		}

		/// <summary>
		/// Assign state ranges for all blocks within 'inst'.
		/// </summary>
		/// <returns>
		/// The set of states for which the exit point of the instruction is reached.
		/// This must be a subset of the input set.
		/// 
		/// Returns an empty set for unsupported instructions.
		/// </returns>
		public LongSet AssignStateRanges(ILInstruction inst, LongSet stateRange)
		{
			CancellationToken.ThrowIfCancellationRequested();
			switch (inst)
			{
				case BlockContainer blockContainer:
					AddStateRange(blockContainer.EntryPoint, stateRange);
					foreach (var block in blockContainer.Blocks)
					{
						// We assume that there are no jumps to blocks already processed.
						// TODO: is SortBlocks() guaranteeing this, even if the user code has loops?
						if (ranges.TryGetValue(block, out stateRange))
						{
							AssignStateRanges(block, stateRange);
						}
					}
					// Since we don't track 'leave' edges, we can only conservatively
					// return LongSet.Empty.
					return LongSet.Empty;
				case Block block:
					foreach (var instInBlock in block.Instructions)
					{
						if (stateRange.IsEmpty)
							break;
						var oldStateRange = stateRange;
						stateRange = AssignStateRanges(instInBlock, stateRange);
						// End-point can only be reachable in a subset of the states where the start-point is reachable.
						Debug.Assert(stateRange.IsSubsetOf(oldStateRange));
						// If the end-point is unreachable, it must be reachable in no states.
						Debug.Assert(stateRange.IsEmpty || !instInBlock.HasFlag(InstructionFlags.EndPointUnreachable));
					}
					return stateRange;
				case TryFinally tryFinally when mode == StateRangeAnalysisMode.IteratorDispose:
					var afterTry = AssignStateRanges(tryFinally.TryBlock, stateRange);
					// really finally should start with 'stateRange.UnionWith(afterTry)', but that's
					// equal to 'stateRange'.
					Debug.Assert(afterTry.IsSubsetOf(stateRange));
					var afterFinally = AssignStateRanges(tryFinally.FinallyBlock, stateRange);
					return afterTry.IntersectWith(afterFinally);
				case SwitchInstruction switchInst:
					SymbolicValue val = evalContext.Eval(switchInst.Value);
					if (val.Type != SymbolicValueType.State)
						goto default;
					List<LongInterval> exitIntervals = new List<LongInterval>();
					foreach (var section in switchInst.Sections)
					{
						// switch (state + Constant)
						// matches 'case VALUE:'
						// iff (state + Constant == value)
						// iff (state == value - Constant)
						var effectiveLabels = section.Labels.AddOffset(unchecked(-val.Constant));
						var result = AssignStateRanges(section.Body, stateRange.IntersectWith(effectiveLabels));
						exitIntervals.AddRange(result.Intervals);
					}
					// exitIntervals = union of exits of all sections
					return new LongSet(exitIntervals);
				case IfInstruction ifInst:
					val = evalContext.Eval(ifInst.Condition).AsBool();
					if (val.Type != SymbolicValueType.StateInSet)
					{
						goto default;
					}
					LongSet trueRanges = val.ValueSet;
					var afterTrue = AssignStateRanges(ifInst.TrueInst, stateRange.IntersectWith(trueRanges));
					var afterFalse = AssignStateRanges(ifInst.FalseInst, stateRange.ExceptWith(trueRanges));
					return afterTrue.UnionWith(afterFalse);
				case Branch br:
					AddStateRange(br.TargetBlock, stateRange);
					return LongSet.Empty;
				case Nop nop:
					return stateRange;
				case StLoc stloc when stloc.Variable == doFinallyBodies || stloc.Variable == skipFinallyBodies:
					// pre-roslyn async/await uses a generated 'bool doFinallyBodies';
					// do not treat this as user code.
					// Mono also does this for yield-return.
					return stateRange;
				case StLoc stloc:
					val = evalContext.Eval(stloc.Value);
					if (val.Type == SymbolicValueType.State && val.Constant == 0)
					{
						evalContext.AddStateVariable(stloc.Variable);
						return stateRange;
					}
					else
					{
						goto default; // user code
					}
				case Call call when mode == StateRangeAnalysisMode.IteratorDispose:
					// Call to finally method.
					// Usually these are in finally blocks, but sometimes (e.g. foreach over array),
					// the C# compiler puts the call to a finally method outside the try-finally block.
					finallyMethodToStateRange.Add((IMethod)call.Method.MemberDefinition, stateRange);
					return LongSet.Empty; // return Empty since we executed user code (the finally method)
				case StObj stobj when mode == StateRangeAnalysisMode.IteratorMoveNext:
				{
					if (stobj.MatchStFld(out var target, out var field, out var value)
						&& target.MatchLdThis() && field.MemberDefinition == stateField && value.MatchLdcI4(-1))
					{
						// Mono resets the state field during MoveNext();
						// don't consider this user code.
						return stateRange;
					}
					else
					{
						goto default;
					}
				}
				default:
					// User code - abort analysis
					if (mode == StateRangeAnalysisMode.IteratorDispose && !(inst is Leave l && l.IsLeavingFunction))
					{
						throw new SymbolicAnalysisFailedException("Unexpected instruction in Iterator.Dispose()");
					}
					return LongSet.Empty;
			}
		}

		private void AddStateRange(Block block, LongSet stateRange)
		{
			if (ranges.TryGetValue(block, out var existingRange))
				ranges[block] = stateRange.UnionWith(existingRange);
			else
				ranges.Add(block, stateRange);
		}

		/// <summary>
		/// Gets a mapping from states to blocks.
		/// 
		/// Within the given container (which should be the container that was analyzed),
		/// the mapping prefers the last block.
		/// Blocks outside of the given container are preferred over blocks within the container.
		/// </summary>
		public LongDict<Block> GetBlockStateSetMapping(BlockContainer container)
		{
			return LongDict.Create(GetMapping());

			IEnumerable<(LongSet, Block)> GetMapping()
			{
				// First, consider container exits:
				foreach (var (block, states) in ranges)
				{
					if (block.Parent != container)
						yield return (states, block);
				}
				// Then blocks within the container:
				foreach (var block in container.Blocks.Reverse())
				{
					if (ranges.TryGetValue(block, out var states))
					{
						yield return (states, block);
					}
				}
			}
		}
	}
}
