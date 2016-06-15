// Copyright (c) 2016 Daniel Grunwald
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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.IL;

namespace ICSharpCode.Decompiler.FlowAnalysis
{
	/// <summary>
	/// Implements the "reaching definitions" analysis.
	/// 
	/// https://en.wikipedia.org/wiki/Reaching_definition
	/// 
	/// By "definitions", we mean stores to local variables.
	/// </summary>
	/// <remarks>
	/// Possible "definitions" that store to a variable are:
	/// * <c>StLoc</c>
	/// * <c>TryCatchHandler</c> (for the exception variable)
	/// * <c>ReachingDefinitions.UninitializedVariable</c> for uninitialized variables.
	/// Note that we do not keep track of <c>LdLoca</c>/references/pointers.
	/// The analysis will likely be wrong/incomplete for variables with <c>AddressCount != 0</c>.
	/// </remarks>
	public class ReachingDefinitions
	{
		#region Documentation + member fields
		/// <summary>
		/// A special Nop instruction that gets used as a fake store if the variable
		/// is possibly uninitialized.
		/// </summary>
		public static readonly ILInstruction UninitializedVariable = new Nop();
		
		// The reaching definition analysis tracks a 'state'.
		// This 'state' represents the reaching definitions at a given source code position.
		// There are many states (one per source code position, i.e. ILInstruction),
		// but we don't store all of them.
		// We only keep track of:
		//  a) the current state in the RDVisitor
		//     This state corresponds to the instruction currently being visited,
		//     and gets mutated as we traverse the ILAst.
		//  b) the input state for each control flow node
		//     This also gets mutated as the analysis learns about new control flow edges.
		//
		// A state can either be reachable, or unreachable:
		//  1) unreachable
		//     Note that during the analysis, "unreachable" just means we have not yet found a path
		//     from the entry point to the node. States transition from unreachable to reachable as
		//     the analysis processes more control flow paths.
		//  2) reachable
		//     In this case, the state contains, for each variable, the set of stores that might have
		//     written to the variable before the control flow reached the state's source code position.
		//     This set does not include stores that were definitely overwritten by other stores to the
		//     same variable.
		//     During the analysis, the set of stores gets extended as the analysis processes more code paths.
		// For loops, we need to analyze the loop body before we can know the state for the loop backedge,
		// but we need to know the input state for the loop body (to which the backedge state contributes)
		// before we can analyze the loop body.
		// Solution: we repeat the analysis of the loop body multiple times, until the state no longer changes.
		// Because all state changes are irreversible (we only add to the set of stores, we never remove from it),
		// this algorithm will eventually terminate.
		// To make it terminate reasonably quickly, we need to process the control flow nodes in the correct order:
		// reverse post-order. This class assumes the blocks in each block container are already sorted appropriately.
		// The caller can use BlockContainer.SortBlocks() for this.
		
		// The state as described above could be represented as a `Dictionary<ILVariable, ImmutableHashSet<ILInstruction>>`.
		// To consume less memory, we instead assign an integer index to all stores in the analyzed function ("store index"),
		// and store the state as a `BitSet` instead.
		// Each bit in the set corresponds to one store instruction, and is `true` iff the store is a reaching definition
		// for the variable it is storing to.
		// The `allStores` array has the same length as the bit sets and holds the corresponding `ILInstruction` objects (store instructions).
		// All stores for a single variable occupy a contiguous segment of the `allStores` array (and thus also of the `state`).

		/// <summary>
		/// To distinguish unreachable from reachable states, we use the first bit in the bitset to store the 'reachable bit'.
		/// If this bit is set, the state is reachable, and the remaining bits
		/// </summary>
		const int ReachableBit = 0;
		
		/// <summary>
		/// Because bit number 0 is the ReachableBit, we start counting store indices at 1.
		/// </summary>
		const int FirstStoreIndex = 1;

		/// <summary>
		/// The function being analyzed.
		/// </summary>
		readonly ILVariableScope scope;
		
		/// <summary>
		/// All stores for all variables in the scope.
		/// 
		/// <c>state[storeIndex]</c> is true iff <c>allStores[storeIndex]</c> is a reaching definition.
		/// Invariant: <c>state.Length == allStores.Length</c>.
		/// </summary>
		readonly ILInstruction[] allStores;
		
		/// <summary>
		/// Maps instructions appearing in <c>allStores</c> to their index.
		/// 
		/// Invariant: <c>allStores[storeIndexMap[inst]] == inst</c>
		/// 
		/// Does not contain <c>UninitializedVariable</c> (as that special instruction has multiple store indices, one per variable)
		/// </summary>
		readonly Dictionary<ILInstruction, int> storeIndexMap = new Dictionary<ILInstruction, int>();
		
		/// <summary>
		/// For all variables <c>v</c>: <c>allStores[firstStoreIndexForVariable[v.IndexInScope]]</c> is the <c>UninitializedVariable</c> entry for <c>v</c>.
		/// The next few stores (up to <c>firstStoreIndexForVariable[v.IndexInScope + 1]</c>, exclusive) are the full list of stores for <c>v</c>.
		/// </summary>
		/// <remarks>
		/// Invariant: <c>firstStoreIndexForVariable[scope.Variables.Count] == allStores.Length</c>
		/// </remarks>
		readonly int[] firstStoreIndexForVariable;
		
		/// <summary>
		/// <c>activeVariable[v.IndexInScope]</c> is true iff RD analysis is enabled for the variable.
		/// </summary>
		readonly BitSet activeVariables;
		
		/// <summary>
		/// Holds the state for incoming branches.
		/// </summary>
		/// <remarks>
		/// Only used for blocks in block containers; not for inline blocks.
		/// </remarks>
		readonly Dictionary<Block, BitSet> stateOnBranch = new Dictionary<Block, BitSet>();
		
		/// <summary>
		/// Holds the state at the block container end-point. (=state for incoming 'leave' instructions)
		/// </summary>
		readonly Dictionary<BlockContainer, BitSet> stateOnLeave = new Dictionary<BlockContainer, BitSet>();
		#endregion
		
		#region Constructor
		/// <summary>
		/// Run reaching definitions analysis for the specified variable scope.
		/// </summary>
		public ReachingDefinitions(ILVariableScope scope, Predicate<ILVariable> pred)
			: this(scope, GetActiveVariableBitSet(scope, pred))
		{
		}

		static BitSet GetActiveVariableBitSet(ILVariableScope scope, Predicate<ILVariable> pred)
		{
			if (scope == null)
				throw new ArgumentNullException("scope");
			BitSet activeVariables = new BitSet(scope.Variables.Count);
			for (int vi = 0; vi < scope.Variables.Count; vi++) {
				activeVariables[vi] = pred(scope.Variables[vi]);
			}
			return activeVariables;
		}

		/// <summary>
		/// Run reaching definitions analysis for the specified variable scope.
		/// </summary>
		public ReachingDefinitions(ILVariableScope scope, BitSet activeVariables)
		{
			if (scope == null)
				throw new ArgumentNullException("scope");
			if (activeVariables == null)
				throw new ArgumentNullException("activeVariables");
			this.scope = scope;
			this.activeVariables = activeVariables;
			
			// Fill `allStores` and `storeIndexMap` and `firstStoreIndexForVariable`.
			var storesByVar = FindAllStoresByVariable(scope, activeVariables);
			allStores = new ILInstruction[FirstStoreIndex + storesByVar.Sum(l => l != null ? l.Count : 0)];
			firstStoreIndexForVariable = new int[scope.Variables.Count + 1];
			int si = FirstStoreIndex;
			for (int vi = 0; vi < storesByVar.Length; vi++) {
				firstStoreIndexForVariable[vi] = si;
				var stores = storesByVar[vi];
				if (stores != null) {
					int expectedStoreCount = scope.Variables[vi].StoreCount;
					if (scope.Variables[vi].Kind != VariableKind.Parameter && scope.Variables[vi].Kind != VariableKind.This) {
						// Extra store for UninitializedVariable
						expectedStoreCount += 1;
						// Note that for VariableKind.Parameter/This, this extra store
						// is already accounted for in ILVariable.StoreCount.
					}
					Debug.Assert(stores.Count == expectedStoreCount);
					stores.CopyTo(allStores, si);
					// Add all stores except for UninitializedVariable to storeIndexMap.
					for (int i = 1; i < stores.Count; i++) {
						storeIndexMap.Add(stores[i], si + i);
					}
					si += stores.Count;
				}
			}
			firstStoreIndexForVariable[scope.Variables.Count] = si;
			Debug.Assert(si == allStores.Length);
			
			this.workList = CreateWorkLists(scope);
			InitStateDictionaries();
			RDVisitor visitor = new RDVisitor(this);
			scope.Children.Single().AcceptVisitor(visitor);
		}

		/// <summary>
		/// Fill <c>allStores</c> and <c>storeIndexMap</c>.
		/// </summary>
		static List<ILInstruction>[] FindAllStoresByVariable(ILVariableScope scope, BitSet activeVariables)
		{
			// For each variable, find the list of ILInstructions storing to that variable
			List<ILInstruction>[] storesByVar = new List<ILInstruction>[scope.Variables.Count];
			for (int vi = 0; vi < storesByVar.Length; vi++) {
				if (activeVariables[vi])
					storesByVar[vi] = new List<ILInstruction> { UninitializedVariable };
			}
			foreach (var inst in scope.Descendants) {
				ILVariable v;
				if (inst.MatchStLoc(out v) || inst.MatchTryCatchHandler(out v)) {
					if (v.Scope == scope && activeVariables[v.IndexInScope]) {
						storesByVar[v.IndexInScope].Add(inst);
					}
				}
			}
			return storesByVar;
		}
		#endregion
		
		#region State Management
		/// <summary>
		/// Create the initial state (reachable + all variables uninitialized).
		/// </summary>
		BitSet CreateInitialState()
		{
			BitSet initialState = new BitSet(allStores.Length);
			initialState.Set(ReachableBit);
			for (int vi = 0; vi < scope.Variables.Count; vi++) {
				if (activeVariables[vi]) {
					Debug.Assert(allStores[firstStoreIndexForVariable[vi]] == UninitializedVariable);
					initialState.Set(firstStoreIndexForVariable[vi]);
				}
			}
			return initialState;
		}
		
		BitSet CreateUnreachableState()
		{
			return new BitSet(allStores.Length);
		}
		
		void InitStateDictionaries()
		{
			foreach (var container in scope.Descendants.OfType<BlockContainer>()) {
				foreach (var block in container.Blocks) {
					stateOnBranch.Add(block, CreateUnreachableState());
				}
				stateOnLeave.Add(container, CreateUnreachableState());
			}
		}
		
		/// <summary>
		/// Merge <c>incomingState</c> into <c>state</c>.
		/// </summary>
		/// <returns>
		/// Returns true if <c>state</c> was modified.
		/// </returns>
		static bool MergeState(BitSet state, BitSet incomingState)
		{
			if (!incomingState[ReachableBit]) {
				// MergeState() is a no-op if the incoming state is unreachable
				return false;
			}
			if (state[ReachableBit]) {
				// both reachable: state |= incomingState;
				if (state.IsSupersetOf(incomingState)) {
					// UnionWith() wouldn't actually change the state,
					// so we have to return false
					return false;
				}
				state.UnionWith(incomingState);
			} else {
				state.ReplaceWith(incomingState);
			}
			return true;
		}
		#endregion
		
		#region Worklist
		/// <summary>
		/// For each block container, stores the set of blocks (via Block.ChildIndex) that had their incoming state
		/// changed and were not processed yet.
		/// </summary>
		readonly Dictionary<BlockContainer, SortedSet<int>> workList;
		
		static Dictionary<BlockContainer, SortedSet<int>> CreateWorkLists(ILVariableScope scope)
		{
			var worklists = new Dictionary<BlockContainer, SortedSet<int>>();
			foreach (var container in scope.Descendants.OfType<BlockContainer>()) {
				worklists.Add(container, new SortedSet<int>());
			}
			return worklists;
		}
		
		/// <summary>
		/// The work list keeps track of which blocks had their incoming state updated,
		/// but did not run yet.
		/// </summary>
		void AddToWorkList(Block block)
		{
			BlockContainer container = (BlockContainer)block.Parent;
			workList[container].Add(block.ChildIndex);
		}
		#endregion
		
		/// <summary>
		/// Visitor that traverses the ILInstruction tree.
		/// </summary>
		class RDVisitor : ILVisitor
		{
			readonly ReachingDefinitions rd;
			
			internal RDVisitor(ReachingDefinitions rd)
			{
				this.rd = rd;
				this.state = rd.CreateInitialState();
				this.stateOnException = rd.CreateUnreachableState();
			}
			
			/// <summary>
			/// Combines state of all possible exceptional control flow paths in the current try block.
			/// </summary>
			BitSet stateOnException;
			
			/// <summary>
			/// Current state.
			/// Gets mutated as the visitor traverses the ILAst.
			/// </summary>
			BitSet state;
			
			protected override void Default(ILInstruction inst)
			{
				// This method assumes normal control flow and no branches.
				if ((inst.DirectFlags & (InstructionFlags.ControlFlow | InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable)) != 0) {
					throw new NotImplementedException("RDVisitor is missing implementation for " + inst.GetType().Name);
				}
				
				// Since this instruction has normal control flow, we can evaluate our children left-to-right.
				foreach (var child in inst.Children) {
					child.AcceptVisitor(this);
					Debug.Assert(!(state[ReachableBit] && child.HasFlag(InstructionFlags.EndPointUnreachable)));
				}
				
				// If this instruction can throw an exception, handle the exceptional control flow edge.
				if ((inst.DirectFlags & InstructionFlags.MayThrow) != 0) {
					MayThrow();
				}
			}
			
			/// <summary>
			/// Handle control flow when `throwInst` throws an exception.
			/// </summary>
			void MayThrow()
			{
				MergeState(stateOnException, state);
			}
			
			void MarkUnreachable()
			{
				state.Clear(ReachableBit);
			}
			
			protected internal override void VisitStLoc(StLoc inst)
			{
				Default(inst);
				ILVariable v = inst.Variable;
				if (v.Scope == rd.scope && rd.activeVariables[v.IndexInScope]) {
					// Clear the set of stores for this variable:
					state.Clear(rd.firstStoreIndexForVariable[v.IndexInScope],
					            rd.firstStoreIndexForVariable[v.IndexInScope + 1]);
					// And replace it with this store:
					state.Set(rd.storeIndexMap[inst]);
				}
			}
			
			protected internal override void VisitBlockContainer(BlockContainer container)
			{
				SortedSet<int> worklist = rd.workList[container];
				if (MergeState(rd.stateOnBranch[container.EntryPoint], state)) {
					worklist.Add(0); // add container entry point to work list
				}
				// Because we use a SortedSet for the work list,
				// we always process the blocks in the same order as they are in the container
				// (usually reverse post-order).
				while (worklist.Count > 0) {
					int blockIndex = worklist.Min;
					worklist.Remove(blockIndex);
					Block block = container.Blocks[blockIndex];
					state.ReplaceWith(rd.stateOnBranch[block]);
					block.AcceptVisitor(this);
				}
				state.ReplaceWith(rd.stateOnLeave[container]);
			}
			
			protected internal override void VisitBranch(Branch inst)
			{
				if (MergeState(rd.stateOnBranch[inst.TargetBlock], state)) {
					rd.AddToWorkList(inst.TargetBlock);
				}
				MarkUnreachable();
			}
			
			protected internal override void VisitLeave(Leave inst)
			{
				Debug.Assert(inst.IsDescendantOf(inst.TargetContainer));
				MergeState(rd.stateOnLeave[inst.TargetContainer], state);
				// Note: We don't have to put the block container onto the work queue,
				// because it's an ancestor of the Leave instruction, and hence
				// we are currently somewhere within the VisitBlockContainer() call.
				MarkUnreachable();
			}
			
			protected internal override void VisitReturn(Return inst)
			{
				if (inst.ReturnValue != null)
					inst.ReturnValue.AcceptVisitor(this);
				MarkUnreachable();
			}
			
			protected internal override void VisitThrow(Throw inst)
			{
				inst.Argument.AcceptVisitor(this);
				MayThrow();
				MarkUnreachable();
			}
			
			protected internal override void VisitRethrow(Rethrow inst)
			{
				MayThrow();
				MarkUnreachable();
			}
			
			/// <summary>
			/// Visits the TryBlock.
			/// 
			/// Returns a new BitSet representing the state of exceptional control flow transfer
			/// out of the try block.
			/// </summary>
			BitSet HandleTryBlock(TryInstruction inst)
			{
				BitSet oldStateOnException = stateOnException;
				BitSet newStateOnException = rd.CreateUnreachableState();
				
				stateOnException = newStateOnException;
				inst.TryBlock.AcceptVisitor(this);
				stateOnException = oldStateOnException;
				
				return newStateOnException;
			}
			
			protected internal override void VisitTryCatch(TryCatch inst)
			{
				BitSet caughtState = HandleTryBlock(inst);
				BitSet endpointState = state.Clone();
				// The exception might get propagated if no handler matches the type:
				MergeState(stateOnException, caughtState);
				foreach (var handler in inst.Handlers) {
					state.ReplaceWith(caughtState);
					handler.Filter.AcceptVisitor(this);
					// if the filter return false, any mutations done by the filter
					// will be visible by the remaining handlers
					// (but it's also possible that the filter didn't get executed at all
					// because the exception type doesn't match)
					MergeState(caughtState, state);
					
					handler.Body.AcceptVisitor(this);
					MergeState(endpointState, state);
				}
				state = endpointState;
			}
			
			protected internal override void VisitTryFinally(TryFinally inst)
			{
				// I don't think there's a good way to track dataflow across finally blocks
				// without duplicating the whole finally block.
				// We'll just approximate 'try { .. } finally { .. }' as 'try { .. } catch {} .. if (?) rethrow; }'
				BitSet caughtState = HandleTryBlock(inst);
				MergeState(state, caughtState);
				inst.FinallyBlock.AcceptVisitor(this);
				MayThrow();
				// Our approximation allows the impossible code path where the try block wasn't fully executed
				// and the finally block did not rethrow the exception.
				// This can cause us to not be marked unreachable in cases where the simple InstructionFlags
				// know the path is unreachable -- so use the flag to fix the reachable bit.
				if (inst.HasFlag(InstructionFlags.EndPointUnreachable)) {
					MarkUnreachable();
				}
			}
			
			protected internal override void VisitTryFault(TryFault inst)
			{
				// try-fault executes fault block if an exception occurs in try,
				// and always rethrows the exception at the end.
				BitSet caughtState = HandleTryBlock(inst);
				BitSet noException = state;
				state = caughtState;
				inst.FaultBlock.AcceptVisitor(this);
				MayThrow(); // rethrow the exception after the fault block
				
				// try-fault exits normally only if no exception occurred
				state = noException;
			}
			
			protected internal override void VisitIfInstruction(IfInstruction inst)
			{
				inst.Condition.AcceptVisitor(this);
				BitSet branchState = state.Clone();
				inst.TrueInst.AcceptVisitor(this);
				BitSet afterTrueState = state;
				state = branchState;
				inst.FalseInst.AcceptVisitor(this);
				MergeState(state, afterTrueState);
			}
			
			protected internal override void VisitSwitchInstruction(SwitchInstruction inst)
			{
				inst.Value.AcceptVisitor(this);
				BitSet beforeSections = state.Clone();
				BitSet afterSections = rd.CreateUnreachableState();
				foreach (var section in inst.Sections) {
					state.ReplaceWith(beforeSections);
					section.AcceptVisitor(this);
					MergeState(afterSections, state);
				}
				state = afterSections;
			}
		}
	}
}
