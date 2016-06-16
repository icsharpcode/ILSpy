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
using System.Collections.Generic;
using System.Diagnostics;
using ICSharpCode.Decompiler.IL;

namespace ICSharpCode.Decompiler.FlowAnalysis
{
	/// <summary>
	/// Interface for use with DataFlowVisitor.
	/// 
	/// A mutable container for the state tracked by the data flow analysis.
	/// </summary>
	/// <remarks>
	/// To ensure the data flow analysis terminates, states must form a partially ordered set (poset)
	/// with finite height.
	/// </remarks>
	public interface IDataFlowState<Self> where Self: IDataFlowState<Self>
	{
		/// <summary>
		/// Gets whether this state is "less than" (or equal to) another state.
		/// This is the partial order of the poset.
		/// </summary>
		/// <remarks>
		/// The exact meaning of this relation is up to the concrete implementation,
		/// but usually "less than" means "has less information than".
		/// A given position in the code starts at the "unreachable state" (=no information)
		/// and then adds more information as the analysis progresses.
		/// After each change to the state, the old state must be less than the new state,
		/// so that the analysis does not run into an infinite loop.
		/// The poset must also have finite height (no infinite ascending chains s1 &lt; s2 &lt; ...),
		/// to ensure the analysis terminates.
		/// </remarks>
		bool LessThanOrEqual(Self otherState);
		
		/// <summary>
		/// Creates a new object with a copy of the state.
		/// </summary>
		Self Clone();
		
		/// <summary>
		/// Replace the contents of this state object with a copy of those in <paramref name="newContent"/>.
		/// </summary>
		/// <remarks>
		/// <c>x = x.Clone(); x.ReplaceWith(newContent);</c>
		/// is equivalent to
		/// <c>x = newContent.Clone();</c>
		/// 
		/// ReplaceWith() is used to avoid allocating new state objects where possible.
		/// </remarks>
		void ReplaceWith(Self newContent);
		
		/// <summary>
		/// Join the incomingState into this state.
		/// </summary>
		/// <remarks>
		/// Postcondition: <c>old(this).LessThan(this) &amp;&amp; incomingState.LessThan(this)</c>
		/// This method generally sets <c>this</c> to the smallest state that is greater than (or equal to)
		/// both input states.
		/// </remarks>
		void JoinWith(Self incomingState);
		
		/// <summary>
		/// Gets whether this is the "unreachable" state.
		/// The unreachable state represents that the data flow analysis has not yet
		/// found a code path from the entry point to this state's position.
		/// </summary>
		/// <remarks>
		/// The unreachable state is the bottom element in the poset: the unreachable state is "less than" all other states.
		/// </remarks>
		bool IsUnreachable { get; }
		
		/// <summary>
		/// Equivalent to <c>this.ReplaceWith(unreachableState)</c>, but may be more efficient.
		/// </summary>
		void MarkUnreachable();
	}
	
	/// <summary>
	/// Generic base class for forward data flow analyses.
	/// </summary>
	/// <typeparam name="State">
	/// The state type used for the data flow analysis. See <see cref="IDataFlowState{Self}"/> for details.
	/// 
	/// <c>DataFlowVisitor</c> expects the state to behave like a mutable reference type.
	/// It might still be a good idea to use a struct to implement it so that .NET uses static dispatch for
	/// method calls on the type parameter, but that struct must consist only of a <c>readonly</c> field
	/// referencing some mutable object, to ensure the type parameter behaves as it if was a mutable reference type.
	/// </typeparam>
	public abstract class DataFlowVisitor<State> : ILVisitor
		where State : IDataFlowState<State>
	{
		// The data flow analysis tracks a 'state'.
		// There are many states (one per source code position, i.e. ILInstruction), but we don't store all of them.
		// We only keep track of:
		//  a) the current state in the RDVisitor
		//     This state corresponds to the instruction currently being visited,
		//     and gets mutated as we traverse the ILAst.
		//  b) the input state for each control flow node
		//     This also gets mutated as the analysis learns about new control flow edges.
		
		/// <summary>
		/// The unreachable state.
		/// Must not be mutated.
		/// </summary>
		readonly State unreachableState;
		
		/// <summary>
		/// Combined state of all possible exceptional control flow paths in the current try block.
		/// Serves as input state for catch blocks.
		/// </summary>
		State stateOnException;
		
		/// <summary>
		/// Current state.
		/// Gets mutated as the visitor traverses the ILAst.
		/// </summary>
		protected State state;
		
		/// <summary>
		/// Creates a new DataFlowVisitor.
		/// </summary>
		/// <param name="initialState">The initial state at the entry point of the analysis.</param>
		protected DataFlowVisitor(State initialState)
		{
			this.state = initialState.Clone();
			this.unreachableState = initialState.Clone();
			this.unreachableState.MarkUnreachable();
			Debug.Assert(unreachableState.IsUnreachable);
			this.stateOnException = unreachableState.Clone();
		}
		
		protected sealed override void Default(ILInstruction inst)
		{
			// This method assumes normal control flow and no branches.
			if ((inst.DirectFlags & (InstructionFlags.ControlFlow | InstructionFlags.MayBranch | InstructionFlags.EndPointUnreachable)) != 0) {
				throw new NotImplementedException("RDVisitor is missing implementation for " + inst.GetType().Name);
			}
			
			// Since this instruction has normal control flow, we can evaluate our children left-to-right.
			foreach (var child in inst.Children) {
				child.AcceptVisitor(this);
				Debug.Assert(state.IsUnreachable || !child.HasFlag(InstructionFlags.EndPointUnreachable));
			}
			
			// If this instruction can throw an exception, handle the exceptional control flow edge.
			if ((inst.DirectFlags & InstructionFlags.MayThrow) != 0) {
				MayThrow();
			}
		}
		
		/// <summary>
		/// Handle control flow when the current instruction throws an exception:
		/// joins the current state into the "exception state" of the current try block.
		/// </summary>
		protected void MayThrow()
		{
			stateOnException.JoinWith(state);
		}
		
		/// <summary>
		/// Holds the state for incoming branches.
		/// </summary>
		/// <remarks>
		/// Only used for blocks in block containers; not for inline blocks.
		/// </remarks>
		readonly Dictionary<Block, State> stateOnBranch = new Dictionary<Block, State>();
		
		/// <summary>
		/// Holds the state at the block container end-point. (=state for incoming 'leave' instructions)
		/// </summary>
		readonly Dictionary<BlockContainer, State> stateOnLeave = new Dictionary<BlockContainer, State>();

		State GetBlockInputState(Block block)
		{
			State s;
			if (stateOnBranch.TryGetValue(block, out s)) {
				return s;
			} else {
				s = unreachableState.Clone();
				stateOnBranch.Add(block, s);
				return s;
			}
		}
		
		/// <summary>
		/// For each block container, stores the set of blocks (via Block.ChildIndex)
		/// that had their incoming state changed and were not processed yet.
		/// </summary>
		readonly Dictionary<BlockContainer, SortedSet<int>> workLists = new Dictionary<BlockContainer, SortedSet<int>>();
		
		protected internal override void VisitBlockContainer(BlockContainer container)
		{
			SortedSet<int> worklist = new SortedSet<int>();
			// register work list so that branches within this container can add to it
			workLists.Add(container, worklist);
			var stateOnEntry = GetBlockInputState(container.EntryPoint);
			if (!state.LessThanOrEqual(stateOnEntry)) {
				// If we have new information for the container's entry point,
				// add the container entry point to the work list.
				stateOnEntry.JoinWith(state);
				worklist.Add(0);
			}
			
			// To handle loops, we need to analyze the loop body before we can know the state for the loop backedge,
			// but we need to know the input state for the loop body (to which the backedge state contributes)
			// before we can analyze the loop body.
			// Solution: we repeat the analysis of the loop body multiple times, until the state no longer changes.
			// To make it terminate reasonably quickly, we need to process the control flow nodes in the correct order:
			// reverse post-order. We use a SortedSet<int> for this, and assume that the block indices used in the SortedSet
			// are ordered appropriately. The caller can use BlockContainer.SortBlocks() for this.
			while (worklist.Count > 0) {
				int blockIndex = worklist.Min;
				worklist.Remove(blockIndex);
				Block block = container.Blocks[blockIndex];
				state.ReplaceWith(stateOnBranch[block]);
				block.AcceptVisitor(this);
			}
			State stateOnExit;
			if (stateOnLeave.TryGetValue(container, out stateOnExit)) {
				state.ReplaceWith(stateOnExit);
			} else {
				state.MarkUnreachable();
			}
		}
		
		protected internal override void VisitBranch(Branch inst)
		{
			var targetBlock = inst.TargetBlock;
			var targetState = GetBlockInputState(targetBlock);
			if (!state.LessThanOrEqual(targetState)) {
				targetState.JoinWith(state);
				
				BlockContainer container = (BlockContainer)targetBlock.Parent;
				workLists[container].Add(targetBlock.ChildIndex);
			}
			state.MarkUnreachable();
		}
		
		protected internal override void VisitLeave(Leave inst)
		{
			State targetState;
			if (stateOnLeave.TryGetValue(inst.TargetContainer, out targetState)) {
				targetState.JoinWith(state);
			} else {
				stateOnLeave.Add(inst.TargetContainer, state.Clone());
			}
			// Note: We don't have to put the block container onto the work queue,
			// because it's an ancestor of the Leave instruction, and hence
			// we are currently somewhere within the VisitBlockContainer() call.
			state.MarkUnreachable();
		}
		
		protected internal override void VisitReturn(Return inst)
		{
			if (inst.ReturnValue != null)
				inst.ReturnValue.AcceptVisitor(this);
			state.MarkUnreachable();
		}
		
		protected internal override void VisitThrow(Throw inst)
		{
			inst.Argument.AcceptVisitor(this);
			MayThrow();
			state.MarkUnreachable();
		}
		
		protected internal override void VisitRethrow(Rethrow inst)
		{
			MayThrow();
			state.MarkUnreachable();
		}
		
		/// <summary>
		/// Visits the TryBlock.
		/// 
		/// Returns a new State object representing the exceptional control flow transfer out of the try block.
		/// </summary>
		protected State HandleTryBlock(TryInstruction inst)
		{
			State oldStateOnException = stateOnException;
			State newStateOnException = unreachableState.Clone();
			
			stateOnException = newStateOnException;
			inst.TryBlock.AcceptVisitor(this);
			stateOnException = oldStateOnException;
			
			return newStateOnException;
		}
		
		protected internal override void VisitTryCatch(TryCatch inst)
		{
			State caughtState = HandleTryBlock(inst);
			State endpointState = state.Clone();
			// The exception might get propagated if no handler matches the type:
			stateOnException.JoinWith(caughtState);
			foreach (var handler in inst.Handlers) {
				state.ReplaceWith(caughtState);
				BeginTryCatchHandler(handler);
				handler.Filter.AcceptVisitor(this);
				// if the filter return false, any mutations done by the filter
				// will be visible by the remaining handlers
				// (but it's also possible that the filter didn't get executed at all
				// because the exception type doesn't match)
				caughtState.JoinWith(state);
				
				handler.Body.AcceptVisitor(this);
				endpointState.JoinWith(state);
			}
			state = endpointState;
		}
		
		protected virtual void BeginTryCatchHandler(TryCatchHandler inst)
		{
		}
		
		/// <summary>
		/// TryCatchHandler is handled directly in VisitTryCatch
		/// </summary>
		protected internal override sealed void VisitTryCatchHandler(TryCatchHandler inst)
		{
			throw new NotImplementedException();
		}
		
		protected internal override void VisitTryFinally(TryFinally inst)
		{
			// I don't think there's a good way to track dataflow across finally blocks
			// without duplicating the whole finally block.
			// We'll just approximate 'try { .. } finally { .. }' as 'try { .. } catch {} .. if (?) rethrow; }'
			State caughtState = HandleTryBlock(inst);
			State onSuccess = state.Clone();
			state.JoinWith(caughtState);
			inst.FinallyBlock.AcceptVisitor(this);
			MayThrow();
			// Our approximation allows the impossible code path where the try block wasn't fully executed
			// and the finally block did not rethrow the exception.
			// We can't fix up the data flow state in general, but specific analyses may be able to do so.
			ExitTryFinally(inst, onSuccess);
		}
		
		/// <summary>
		/// Called when exiting a try-finally construct.
		/// </summary>
		/// <param name="inst">The try-finally instruction.</param>
		/// <param name="onSuccess">The state at the endpoint of the try block.</param>
		/// <c>state</c>: The current state when ExitTryFinally() is called is the state at the
		/// endpoint of the finally block.
		/// After ExitTryFinally() exits, it should be the state at the endpoint
		/// of the whole try-finally construct.
		/// <remarks>
		/// The purpose of this method is to allow the derived class to fix up the state at the end
		/// of the try-finally construct: the end of the finally block can be reached when
		/// the the try block has throws an exception, but the endpoint of the whole try-finally
		/// is only reachable both the try block and the finally block execute normally.
		/// </remarks>
		protected virtual void ExitTryFinally(TryFinally inst, State onSuccess)
		{
			if (onSuccess.IsUnreachable) {
				state.MarkUnreachable();
			}
		}
		
		protected internal override void VisitTryFault(TryFault inst)
		{
			// try-fault executes fault block if an exception occurs in try,
			// and always rethrows the exception at the end.
			State caughtState = HandleTryBlock(inst);
			State noException = state;
			state = caughtState;
			inst.FaultBlock.AcceptVisitor(this);
			MayThrow(); // rethrow the exception after the fault block
			
			// try-fault exits normally only if no exception occurred
			state = noException;
		}
		
		protected internal override void VisitIfInstruction(IfInstruction inst)
		{
			inst.Condition.AcceptVisitor(this);
			State branchState = state.Clone();
			inst.TrueInst.AcceptVisitor(this);
			State afterTrueState = state;
			state = branchState;
			inst.FalseInst.AcceptVisitor(this);
			state.JoinWith(afterTrueState);
		}
		
		protected internal override void VisitSwitchInstruction(SwitchInstruction inst)
		{
			inst.Value.AcceptVisitor(this);
			State beforeSections = state.Clone();
			State afterSections = unreachableState.Clone();
			foreach (var section in inst.Sections) {
				state.ReplaceWith(beforeSections);
				section.AcceptVisitor(this);
				afterSections.JoinWith(state);
			}
			state = afterSections;
		}
	}
}
