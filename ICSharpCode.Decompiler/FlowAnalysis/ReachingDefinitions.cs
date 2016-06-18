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
		#region State representation
		/// <summary>
		/// The state during the reaching definitions analysis.
		/// </summary>
		/// <remarks>
		/// A state can either be reachable, or unreachable:
		///  1) unreachable
		///     Note that during the analysis, "unreachable" just means we have not yet found a path
		///     from the entry point to the node. States transition from unreachable to reachable as
		///     the analysis processes more control flow paths.
		///  2) reachable
		///     In this case, the state contains, for each variable, the set of stores that might have
		///     written to the variable before the control flow reached the state's source code position.
		///     This set does not include stores that were definitely overwritten by other stores to the
		///     same variable.
		///     During the analysis, the set of stores gets extended as the analysis processes more code paths.
		/// 
		/// The reachable state could be represented as a `Dictionary{ILVariable, ISet{ILInstruction}}`.
		/// To consume less memory, we instead assign an integer index to all stores in the analyzed function ("store index"),
		/// and store the state as a `BitSet` instead.
		/// Each bit in the set corresponds to one store instruction, and is `true` iff the store is a reaching definition
		/// for the variable it is storing to.
		/// The `allStores` array has the same length as the bit sets and holds the corresponding `ILInstruction` objects (store instructions).
		/// All stores for a single variable occupy a contiguous segment of the `allStores` array (and thus also of the `state`),
		/// which allows us to efficient clear out all stores that get overwritten by a new store.
		/// </remarks>
		[DebuggerDisplay("{bits}")]
		struct State : IDataFlowState<State>
		{
			/// <summary>
			/// bit 0: This state's position is reachable from the entry point.
			/// bit i+1: There is a code path from the entry point to this state's position
			///          that passes through through <c>allStores[i]</c> and does not pass through another
			///          store to <c>allStores[i].Variable</c>.
			/// </summary>
			readonly BitSet bits;
			
			public State(BitSet bits)
			{
				this.bits = bits;
			}
			
			public bool LessThanOrEqual(State otherState)
			{
				return bits.IsSubsetOf(otherState.bits);
			}
			
			public State Clone()
			{
				return new State(bits.Clone());
			}
			
			public void ReplaceWith(State newContent)
			{
				bits.ReplaceWith(newContent.bits);
			}
			
			public void JoinWith(State incomingState)
			{
				bits.UnionWith(incomingState.bits);
			}
			
			public void MeetWith(State incomingState)
			{
				bits.IntersectWith(incomingState.bits);
			}
			
			public bool IsBottom {
				get { return !bits[ReachableBit]; }
			}
			
			public void ReplaceWithBottom()
			{
				// We need to clear all bits, not just ReachableBit, so that
				// the bottom state behaves as expected in joins/meets.
				bits.ClearAll();
			}
			
			public bool IsReachable {
				get { return bits[ReachableBit]; }
			}
			
			public void KillStores(int startStoreIndex, int endStoreIndex)
			{
				Debug.Assert(startStoreIndex >= FirstStoreIndex);
				Debug.Assert(endStoreIndex >= startStoreIndex);
				bits.Clear(startStoreIndex, endStoreIndex);
			}
			
			public void SetStore(int storeIndex)
			{
				Debug.Assert(storeIndex >= FirstStoreIndex);
				bits.Set(storeIndex);
			}
		}
		
		/// <summary>
		/// To distinguish unreachable from reachable states, we use the first bit in the bitset to store the 'reachable bit'.
		/// If this bit is set, the state is reachable, and the remaining bits
		/// </summary>
		const int ReachableBit = 0;
		
		/// <summary>
		/// Because bit number 0 is the ReachableBit, we start counting store indices at 1.
		/// </summary>
		const int FirstStoreIndex = 1;
		#endregion
		
		#region Documentation + member fields
		/// <summary>
		/// A special Nop instruction that gets used as a fake store if the variable
		/// is possibly uninitialized.
		/// </summary>
		public static readonly ILInstruction UninitializedVariable = new Nop();

		/// <summary>
		/// The function being analyzed.
		/// </summary>
		readonly ILVariableScope scope;
		
		/// <summary>
		/// All stores for all variables in the scope.
		/// 
		/// <c>state[storeIndex]</c> is true iff <c>allStores[storeIndex]</c> is a reaching definition.
		/// Invariant: <c>state.bits.Length == allStores.Length</c>.
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
					if (!scope.Variables[vi].HasInitialValue) {
						// Extra store for UninitializedVariable
						expectedStoreCount += 1;
						// Note that for variables with HasInitialValue=true,
						// this extra store is already accounted for in ILVariable.StoreCount.
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
		
		#region CreateInitialState
		/// <summary>
		/// Create the initial state (reachable + all variables uninitialized).
		/// </summary>
		State CreateInitialState()
		{
			BitSet initialState = new BitSet(allStores.Length);
			initialState.Set(ReachableBit);
			for (int vi = 0; vi < scope.Variables.Count; vi++) {
				if (activeVariables[vi]) {
					Debug.Assert(allStores[firstStoreIndexForVariable[vi]] == UninitializedVariable);
					initialState.Set(firstStoreIndexForVariable[vi]);
				}
			}
			return new State(initialState);
		}
		#endregion
		
		/// <summary>
		/// Visitor that traverses the ILInstruction tree.
		/// </summary>
		class RDVisitor : DataFlowVisitor<State>
		{
			readonly ReachingDefinitions rd;
			
			internal RDVisitor(ReachingDefinitions rd) : base(rd.CreateInitialState())
			{
				this.rd = rd;
			}

			void HandleStore(ILInstruction inst, ILVariable v)
			{
				if (v.Scope == rd.scope && rd.activeVariables[v.IndexInScope] && state.IsReachable) {
					// Clear the set of stores for this variable:
					state.KillStores(rd.firstStoreIndexForVariable[v.IndexInScope], rd.firstStoreIndexForVariable[v.IndexInScope + 1]);
					// And replace it with this store:
					state.SetStore(rd.storeIndexMap[inst]);
				}
			}
			
			protected internal override void VisitStLoc(StLoc inst)
			{
				base.VisitStLoc(inst);
				HandleStore(inst, inst.Variable);
			}
			
			protected override void BeginTryCatchHandler(TryCatchHandler inst)
			{
				HandleStore(inst, inst.Variable);
			}
		}
	}
}
