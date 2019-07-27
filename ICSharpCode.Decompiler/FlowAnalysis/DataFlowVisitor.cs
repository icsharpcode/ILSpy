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
	/// States must form a join-semilattice: https://en.wikipedia.org/wiki/Semilattice
	/// 
	/// To handle <c>try{} finally{}</c> properly, states should implement <c>MeetWith()</c> as well,
	/// and thus should form a lattice.
	/// 
	/// <c>DataFlowVisitor</c> expects the state to behave like a mutable reference type.
	/// It might still be a good idea to use a struct to implement it so that .NET uses static dispatch for
	/// method calls on the type parameter, but that struct must consist only of a <c>readonly</c> field
	/// referencing some mutable object, to ensure the type parameter behaves as it if was a mutable reference type.
	/// </remarks>
	public interface IDataFlowState<Self> where Self: IDataFlowState<Self>
	{
		/// <summary>
		/// Gets whether this state is "less than" (or equal to) another state.
		/// This is the partial order of the semi-lattice.
		/// </summary>
		/// <remarks>
		/// The exact meaning of this relation is up to the concrete implementation,
		/// but usually "less than" means "has less information than".
		/// A given position in the code starts at the "bottom state" (=no information)
		/// and then adds more information as the analysis progresses.
		/// After each change to the state, the old state must be less than the new state,
		/// so that the analysis does not run into an infinite loop.
		/// The partially ordered set must also have finite height (no infinite ascending chains s1 &lt; s2 &lt; ...),
		/// to ensure the analysis terminates.
		/// </remarks>
		/// <example>
		/// The simplest possible non-trivial state, <c>bool isReachable</c>, would implement <c>LessThanOrEqual</c> as:
		/// <code>return (this.isReachable ? 1 : 0) &lt;= (otherState.isReachable ? 1 : 0);</code>
		/// <para>Which can be simpified to:</para>
		/// <code>return !this.isReachable || otherState.isReachable;</code>
		/// </example>
		bool LessThanOrEqual(Self otherState);
		
		/// <summary>
		/// Creates a new object with a copy of the state.
		/// </summary>
		/// <remarks>
		/// Mutating methods such as <c>ReplaceWith</c> or <c>JoinWith</c> modify the contents of a state object.
		/// Cloning the object allows the analysis to track multiple independent states,
		/// such as the
		/// </remarks>
		/// <example>
		/// The simple state "<c>bool isReachable</c>", would implement <c>Clone</c> as:
		/// <code>return new MyState(this.isReachable);</code>
		/// </example>
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
		/// <example>
		/// The simple state "<c>bool isReachable</c>", would implement <c>ReplaceWith</c> as:
		/// <code>this.isReachable = newContent.isReachable;</code>
		/// </example>
		void ReplaceWith(Self newContent);
		
		/// <summary>
		/// Join the incomingState into this state.
		/// </summary>
		/// <remarks>
		/// Postcondition: <c>old(this).LessThanOrEqual(this) &amp;&amp; incomingState.LessThanOrEqual(this)</c>
		/// This method should set <c>this</c> to the smallest state that is greater than (or equal to)
		/// both input states.
		/// 
		/// <c>JoinWith()</c> is used when multiple control flow paths are joined together.
		/// For example, it is used to combine the <c>thenState</c> with the <c>elseState</c>
		/// at the end of a if-else construct.
		/// </remarks>
		/// <example>
		/// The simple state "<c>bool isReachable</c>", would implement <c>JoinWith</c> as:
		/// <code>this.isReachable |= incomingState.isReachable;</code>
		/// </example>
		void JoinWith(Self incomingState);

		/// <summary>
		/// A special operation to merge the end-state of the finally-block with the end state of
		/// a branch leaving the try-block.
		/// 
		/// If either input state is unreachable, this call must result in an unreachable state.
		/// </summary>
		/// <example>
		/// The simple state "<c>bool isReachable</c>", would implement <c>TriggerFinally</c> as:
		/// <code>this.isReachable &amp;= finallyState.isReachable;</code>
		/// </example>
		void TriggerFinally(Self finallyState);
		
		/// <summary>
		/// Gets whether this is the bottom state.
		/// 
		/// The bottom state represents that the data flow analysis has not yet
		/// found a code path from the entry point to this state's position.
		/// It thus contains no information, and is "less than" all other states.
		/// </summary>
		/// <remarks>
		/// The bottom state is the bottom element in the semi-lattice.
		/// 
		/// Initially, all code blocks not yet visited by the analysis will be in the bottom state.
		/// Unreachable code will always remain in the bottom state.
		/// Some analyses may also use the bottom state for reachable code after it was processed by the analysis.
		/// For example, in <c>DefiniteAssignmentVisitor</c> the bottom states means
		/// "either this code is unreachable, or all variables are definitely initialized".
		/// </remarks>
		/// <example>
		/// The simple state "<c>bool isReachable</c>", would implement <c>IsBottom</c> as:
		/// <code>return !this.isReachable;</code>
		/// </example>
		bool IsBottom { get; }
		
		/// <summary>
		/// Equivalent to <c>this.ReplaceWith(bottomState)</c>, but may be implemented more efficiently.
		/// </summary>
		/// <remarks>
		/// Since the <c>DataFlowVisitor</c> can only create states by cloning from the initial state,
		/// this method is necessary for the <c>DataFlowVisitor</c> to gain access to the bottom element in
		/// the first place.
		/// </remarks>
		/// <example>
		/// The simple state "<c>bool isReachable</c>", would implement <c>ReplaceWithBottom</c> as:
		/// <code>this.isReachable = false;</code>
		/// </example>
		void ReplaceWithBottom();
	}
	
	/// <summary>
	/// Generic base class for forward data flow analyses.
	/// </summary>
	/// <typeparam name="State">
	/// The state type used for the data flow analysis. See <see cref="IDataFlowState{Self}"/> for details.
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
		//     These also gets mutated as the analysis learns about new control flow edges.
		
		/// <summary>
		/// The bottom state.
		/// Must not be mutated.
		/// </summary>
		State bottomState;
		
		/// <summary>
		/// Current state.
		/// 
		/// Caution: any state object assigned to this member gets mutated as the visitor traverses the ILAst!
		/// </summary>
		protected State state;
		
		/// <summary>
		/// Combined state of all possible exceptional control flow paths in the current try block.
		/// Serves as input state for catch blocks.
		/// 
		/// Caution: any state object assigned to this member gets mutated as the visitor encounters instructions that may throw exceptions!
		/// 
		/// Within a try block, <c>currentStateOnException == stateOnException[tryBlock.Parent]</c>.
		/// </summary>
		/// <seealso cref="PropagateStateOnException"/>
		protected State currentStateOnException;
		
		bool initialized;
		
		/// <summary>
		/// Initializes the DataFlowVisitor.
		/// This method must be called once before any Visit()-methods can be called.
		/// It must not be called more than once.
		/// </summary>
		/// <param name="initialState">The initial state at the entry point of the analysis.</param>
		/// <remarks>
		/// This is a method instead of a constructor because derived classes might need complex initialization
		/// before they can construct the initial state.
		/// </remarks>
		protected void Initialize(State initialState)
		{
			Debug.Assert(!initialized);
			initialized = true;
			this.state = initialState.Clone();
			this.bottomState = initialState.Clone();
			this.bottomState.ReplaceWithBottom();
			Debug.Assert(bottomState.IsBottom);
			this.stateOnNullableRewrap = bottomState.Clone();
			this.currentStateOnException = state.Clone();
		}
		
		#if DEBUG
		// For debugging, capture the input + output state at every instruction.
		readonly Dictionary<ILInstruction, State> debugInputState = new Dictionary<ILInstruction, State>();
		readonly Dictionary<ILInstruction, State> debugOutputState = new Dictionary<ILInstruction, State>();
		
		void DebugPoint(Dictionary<ILInstruction, State> debugDict, ILInstruction inst)
		{
			#if DEBUG
			Debug.Assert(initialized, "Initialize() was not called");
			
			State previousState;
			if (debugDict.TryGetValue(inst, out previousState)) {
				Debug.Assert(previousState.LessThanOrEqual(state));
				previousState.JoinWith(state);
			} else {
				// limit the number of tracked instructions to make memory usage in debug builds less horrible
				if (debugDict.Count < 1000) {
					debugDict.Add(inst, state.Clone());
				}
			}
			
			// currentStateOnException should be all states within the try block joined together
			// -> state should already have been joined into currentStateOnException.
			Debug.Assert(state.LessThanOrEqual(currentStateOnException));
			#endif
		}
		#endif
		
		[Conditional("DEBUG")]
		protected void DebugStartPoint(ILInstruction inst)
		{
			#if DEBUG
			DebugPoint(debugInputState, inst);
			#endif
		}
		
		[Conditional("DEBUG")]
		protected void DebugEndPoint(ILInstruction inst)
		{
			#if DEBUG
			DebugPoint(debugOutputState, inst);
			#endif
		}
		
		/// <summary>
		/// Derived classes may add to this set of flags to ensure they don't forget to override an interesting method.
		/// </summary>
		protected InstructionFlags flagsRequiringManualImpl = InstructionFlags.ControlFlow | InstructionFlags.MayBranch | InstructionFlags.MayUnwrapNull | InstructionFlags.EndPointUnreachable;
		
		protected sealed override void Default(ILInstruction inst)
		{
			DebugStartPoint(inst);
			// This method assumes normal control flow and no branches.
			if ((inst.DirectFlags & flagsRequiringManualImpl) != 0) {
				throw new NotImplementedException(GetType().Name + " is missing implementation for " + inst.GetType().Name);
			}
			
			// Since this instruction has normal control flow, we can evaluate our children left-to-right.
			foreach (var child in inst.Children) {
				child.AcceptVisitor(this);
				Debug.Assert(state.IsBottom || !child.HasFlag(InstructionFlags.EndPointUnreachable),
							 "Unreachable code must be in the bottom state.");
			}
			
			DebugEndPoint(inst);
		}
		
		/// <summary>
		/// Handle control flow when the current instruction throws an exception:
		/// joins the current state into the "exception state" of the current try block.
		/// </summary>
		/// <remarks>
		/// This should not only be called for instructions that may throw an exception,
		/// but for all instructions (due to async exceptions like ThreadAbortException)!
		/// 
		/// To avoid redundant calls, every Visit() call may assume that the current state
		/// is already propagated, and has to guarantee the same at the end.
		/// This means this method should be called after every state change.
		/// Alternatively, derived classes may directly modify both <c>state</c>
		/// and <c>currentStateOnException</c>, so that a full <c>JoinWith()</c> call
		/// is not necessary.
		/// </remarks>
		protected void PropagateStateOnException()
		{
			currentStateOnException.JoinWith(state);
		}
		
		/// <summary>
		/// Replace the current state with the bottom state.
		/// </summary>
		protected void MarkUnreachable()
		{
			state.ReplaceWithBottom();
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

		/// <summary>
		/// Gets the state object that holds the state for incoming branches to the block.
		/// </summary>
		/// <remarks>
		/// Returns the a clone of the bottom state on the first call for a given block,
		/// then returns the same object instance on further calls.
		/// The caller is expected to mutate the returned state by calling <c>JoinWith()</c>.
		/// </remarks>
		State GetBlockInputState(Block block)
		{
			State s;
			if (stateOnBranch.TryGetValue(block, out s)) {
				return s;
			} else {
				s = bottomState.Clone();
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
			DebugStartPoint(container);
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
				MarkUnreachable();
			}
			DebugEndPoint(container);
			workLists.Remove(container);
		}

		readonly List<(IBranchOrLeaveInstruction, State)> branchesTriggeringFinally = new List<(IBranchOrLeaveInstruction, State)>();

		protected internal override void VisitBranch(Branch inst)
		{
			if (inst.TriggersFinallyBlock) {
				Debug.Assert(state.LessThanOrEqual(currentStateOnException));
				branchesTriggeringFinally.Add((inst, state.Clone()));
			} else {
				MergeBranchStateIntoTargetBlock(inst, state);
			}
			MarkUnreachable();
		}

		void MergeBranchStateIntoTargetBlock(Branch inst, State branchState)
		{
			var targetBlock = inst.TargetBlock;
			var targetState = GetBlockInputState(targetBlock);
			if (!branchState.LessThanOrEqual(targetState)) {
				targetState.JoinWith(branchState);

				BlockContainer container = (BlockContainer)targetBlock.Parent;
				workLists[container].Add(targetBlock.ChildIndex);
			}
		}
		
		protected internal override void VisitLeave(Leave inst)
		{
			inst.Value.AcceptVisitor(this);
			if (inst.TriggersFinallyBlock) {
				Debug.Assert(state.LessThanOrEqual(currentStateOnException));
				branchesTriggeringFinally.Add((inst, state.Clone()));
			} else {
				MergeBranchStateIntoStateOnLeave(inst, state);
			}
			MarkUnreachable();
		}

		void MergeBranchStateIntoStateOnLeave(Leave inst, State branchState)
		{
			if (stateOnLeave.TryGetValue(inst.TargetContainer, out State targetState)) {
				targetState.JoinWith(branchState);
			} else {
				stateOnLeave.Add(inst.TargetContainer, branchState.Clone());
			}
			// Note: We don't have to put the block container onto the work queue,
			// because it's an ancestor of the Leave instruction, and hence
			// we are currently somewhere within the VisitBlockContainer() call.
		}

		protected internal override void VisitThrow(Throw inst)
		{
			inst.Argument.AcceptVisitor(this);
			MarkUnreachable();
		}
		
		protected internal override void VisitRethrow(Rethrow inst)
		{
			MarkUnreachable();
		}
		
		protected internal override void VisitInvalidBranch(InvalidBranch inst)
		{
			MarkUnreachable();
		}
		
		/// <summary>
		/// Stores the stateOnException per try instruction.
		/// </summary>
		readonly Dictionary<TryInstruction, State> stateOnException = new Dictionary<TryInstruction, State>();
		
		/// <summary>
		/// Visits the TryBlock.
		/// 
		/// Returns a new State object representing the exceptional control flow transfer out of the try block.
		/// </summary>
		protected State HandleTryBlock(TryInstruction inst)
		{
			State oldStateOnException = currentStateOnException;
			State newStateOnException;
			if (stateOnException.TryGetValue(inst, out newStateOnException)) {
				newStateOnException.JoinWith(state);
			} else {
				newStateOnException = state.Clone();
				stateOnException.Add(inst, newStateOnException);
			}
			
			currentStateOnException = newStateOnException;
			inst.TryBlock.AcceptVisitor(this);
			// swap back to the old object instance
			currentStateOnException = oldStateOnException;
			
			// No matter what kind of try-instruction this is, it's possible
			// that an async exception is thrown immediately in the handler block,
			// so propagate the state:
			oldStateOnException.JoinWith(newStateOnException);

			// Return a copy, so that the caller mutating the returned state
			// does not influence the 'stateOnException' dict
			return newStateOnException.Clone();
		}
		
		protected internal override void VisitTryCatch(TryCatch inst)
		{
			DebugStartPoint(inst);
			State onException = HandleTryBlock(inst);
			State endpoint = state.Clone();
			foreach (var handler in inst.Handlers) {
				state.ReplaceWith(onException);
				BeginTryCatchHandler(handler);
				handler.Filter.AcceptVisitor(this);
				// if the filter return false, any mutations done by the filter
				// will be visible by the remaining handlers
				// (but it's also possible that the filter didn't get executed at all
				// because the exception type doesn't match)
				onException.JoinWith(state);
				
				handler.Body.AcceptVisitor(this);
				endpoint.JoinWith(state);
			}
			state = endpoint;
			DebugEndPoint(inst);
		}
		
		protected virtual void BeginTryCatchHandler(TryCatchHandler inst)
		{
		}
		
		/// <summary>
		/// TryCatchHandler is handled directly in VisitTryCatch
		/// </summary>
		protected internal override sealed void VisitTryCatchHandler(TryCatchHandler inst)
		{
			throw new NotSupportedException();
		}

		protected internal override void VisitTryFinally(TryFinally inst)
		{
			DebugStartPoint(inst);
			int branchesTriggeringFinallyOldCount = branchesTriggeringFinally.Count;
			// At first, handle 'try { .. } finally { .. }' like 'try { .. } catch {} .. if (?) rethrow; }'
			State onException = HandleTryBlock(inst);
			State onSuccess = state.Clone();
			state.JoinWith(onException);
			inst.FinallyBlock.AcceptVisitor(this);
			//PropagateStateOnException(); // rethrow the exception after the finally block -- should be redundant
			Debug.Assert(state.LessThanOrEqual(currentStateOnException));

			ProcessBranchesLeavingTryFinally(inst, branchesTriggeringFinallyOldCount);

			// Use TriggerFinally() to ensure points after the try-finally are reachable only if both the
			// try and the finally endpoints are reachable.
			onSuccess.TriggerFinally(state);
			state = onSuccess;
			DebugEndPoint(inst);
		}

		/// <summary>
		/// Process branches leaving the try-finally,
		///  * Calls TriggerFinally() on each branchesTriggeringFinally
		///  * Removes entries from branchesTriggeringFinally if they won't trigger additional finally blocks.
		///  * After all finallies are applied, the branch state is merged into the target block.
		/// </summary>
		void ProcessBranchesLeavingTryFinally(TryFinally tryFinally, int branchesTriggeringFinallyOldCount)
		{
			int outPos = branchesTriggeringFinallyOldCount;
			for (int i = branchesTriggeringFinallyOldCount; i < branchesTriggeringFinally.Count; ++i) {
				var (branch, stateOnBranch) = branchesTriggeringFinally[i];
				Debug.Assert(((ILInstruction)branch).IsDescendantOf(tryFinally));
				Debug.Assert(tryFinally.IsDescendantOf(branch.TargetContainer));
				stateOnBranch.TriggerFinally(state);
				bool triggersAnotherFinally = Branch.GetExecutesFinallyBlock(tryFinally, branch.TargetContainer);
				if (triggersAnotherFinally) {
					branchesTriggeringFinally[outPos++] = (branch, stateOnBranch);
				} else {
					// Merge state into target block.
					if (branch is Leave leave) {
						MergeBranchStateIntoStateOnLeave((Leave)branch, stateOnBranch);
					} else {
						MergeBranchStateIntoTargetBlock((Branch)branch, stateOnBranch);
					}
				}
			}
			branchesTriggeringFinally.RemoveRange(outPos, branchesTriggeringFinally.Count - outPos);
		}

		protected internal override void VisitTryFault(TryFault inst)
		{
			DebugStartPoint(inst);
			// try-fault executes fault block if an exception occurs in try,
			// and always rethrows the exception at the end.
			State onException = HandleTryBlock(inst);
			State onSuccess = state;
			state = onException;
			inst.FaultBlock.AcceptVisitor(this);
			//PropagateStateOnException(); // rethrow the exception after the fault block
			Debug.Assert(state.LessThanOrEqual(currentStateOnException));

			// try-fault exits normally only if no exception occurred
			state = onSuccess;
			DebugEndPoint(inst);
		}
		
		protected internal override void VisitIfInstruction(IfInstruction inst)
		{
			DebugStartPoint(inst);
			inst.Condition.AcceptVisitor(this);
			State branchState = state.Clone();
			inst.TrueInst.AcceptVisitor(this);
			State afterTrueState = state;
			state = branchState;
			inst.FalseInst.AcceptVisitor(this);
			state.JoinWith(afterTrueState);
			DebugEndPoint(inst);
		}

		protected internal override void VisitNullCoalescingInstruction(NullCoalescingInstruction inst)
		{
			HandleBinaryWithOptionalEvaluation(inst, inst.ValueInst, inst.FallbackInst);
		}

		protected internal override void VisitDynamicLogicOperatorInstruction(DynamicLogicOperatorInstruction inst)
		{
			HandleBinaryWithOptionalEvaluation(inst, inst.Left, inst.Right);
		}

		protected internal override void VisitUserDefinedLogicOperator(UserDefinedLogicOperator inst)
		{
			HandleBinaryWithOptionalEvaluation(inst, inst.Left, inst.Right);
		}

		void HandleBinaryWithOptionalEvaluation(ILInstruction parent, ILInstruction left, ILInstruction right)
		{
			DebugStartPoint(parent);
			left.AcceptVisitor(this);
			State branchState = state.Clone();
			right.AcceptVisitor(this);
			state.JoinWith(branchState);
			DebugEndPoint(parent);
		}

		State stateOnNullableRewrap;

		protected internal override void VisitNullableRewrap(NullableRewrap inst)
		{
			DebugStartPoint(inst);
			var oldState = stateOnNullableRewrap.Clone();
			stateOnNullableRewrap.ReplaceWithBottom();
			inst.Argument.AcceptVisitor(this);
			state.JoinWith(stateOnNullableRewrap);
			stateOnNullableRewrap = oldState;
			DebugEndPoint(inst);
		}

		protected internal override void VisitNullableUnwrap(NullableUnwrap inst)
		{
			DebugStartPoint(inst);
			inst.Argument.AcceptVisitor(this);
			stateOnNullableRewrap.JoinWith(state);
			DebugEndPoint(inst);
		}

		protected internal override void VisitSwitchInstruction(SwitchInstruction inst)
		{
			DebugStartPoint(inst);
			inst.Value.AcceptVisitor(this);
			State beforeSections = state.Clone();
			inst.Sections[0].AcceptVisitor(this);
			State afterSections = state.Clone();
			for (int i = 1; i < inst.Sections.Count; ++i) {
				state.ReplaceWith(beforeSections);
				inst.Sections[i].AcceptVisitor(this);
				afterSections.JoinWith(state);
			}
			state = afterSections;
			DebugEndPoint(inst);
		}
		
		protected internal override void VisitYieldReturn(YieldReturn inst)
		{
			DebugStartPoint(inst);
			inst.Value.AcceptVisitor(this);
			DebugEndPoint(inst);
		}

		protected internal override void VisitUsingInstruction(UsingInstruction inst)
		{
			DebugStartPoint(inst);
			inst.ResourceExpression.AcceptVisitor(this);
			inst.Body.AcceptVisitor(this);
			DebugEndPoint(inst);
		}

		protected internal override void VisitLockInstruction(LockInstruction inst)
		{
			DebugStartPoint(inst);
			inst.OnExpression.AcceptVisitor(this);
			inst.Body.AcceptVisitor(this);
			DebugEndPoint(inst);
		}

		protected internal override void VisitILFunction(ILFunction function)
		{
			throw new NotImplementedException();
		}
	}
}
