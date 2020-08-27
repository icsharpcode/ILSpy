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

using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Live range splitting for IL variables.
	/// </summary>
	public class SplitVariables : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			var groupStores = new GroupStores(function, context.CancellationToken);
			function.Body.AcceptVisitor(groupStores);
			// Replace analyzed variables with their split versions:
			foreach (var inst in function.Descendants.OfType<IInstructionWithVariableOperand>())
			{
				if (groupStores.IsAnalyzedVariable(inst.Variable))
				{
					inst.Variable = groupStores.GetNewVariable(inst);
				}
			}
			function.Variables.RemoveDead();
		}

		static bool IsCandidateVariable(ILVariable v)
		{
			switch (v.Kind)
			{
				case VariableKind.Local:
					foreach (var ldloca in v.AddressInstructions)
					{
						if (DetermineAddressUse(ldloca, ldloca.Variable) == AddressUse.Unknown)
						{
							// If we don't understand how the address is being used,
							// we can't split the variable.
							return false;
						}
					}
					return true;
				case VariableKind.StackSlot:
					// stack slots: are already split by construction,
					// except for the locals-turned-stackslots in async functions
					if (v.Function.IsAsync)
						goto case VariableKind.Local;
					else
						return false;
				default:
					// parameters: avoid splitting parameters
					// pinned locals: must not split (doing so would extend the life of the pin to the end of the method)
					return false;
			}
		}

		enum AddressUse
		{
			Unknown,
			/// <summary>
			/// Address is immediately used for reading and/or writing,
			/// without the possibility of the variable being directly stored to (via 'stloc')
			/// in between the 'ldloca' and the use of the address.
			/// </summary>
			Immediate,
			/// <summary>
			/// We support some limited form of ref locals referring to a target variable,
			/// without giving up splitting of the target variable.
			/// Requirements:
			///  * the ref local is single-assignment
			///  * the ref local is initialized directly with 'ldloca target; stloc ref_local',
			///       not a derived pointer (e.g. 'ldloca target; ldflda F; stloc ref_local').
			///  * all uses of the ref_local are immediate.
			/// There may be stores to the target variable in between the 'stloc ref_local' and its uses,
			/// but we handle that case by treating each use of the ref_local as an address access
			/// of the target variable (as if the ref_local was eliminated via copy propagation).
			/// </summary>
			WithSupportedRefLocals,
		}

		static AddressUse DetermineAddressUse(ILInstruction addressLoadingInstruction, ILVariable targetVar)
		{
			switch (addressLoadingInstruction.Parent)
			{
				case LdObj ldobj:
					return AddressUse.Immediate;
				case LdFlda ldflda:
					return DetermineAddressUse(ldflda, targetVar);
				case Await await:
					// GetAwaiter() may write to the struct, but shouldn't store the address for later use
					return AddressUse.Immediate;
				case CallInstruction call:
					return HandleCall(addressLoadingInstruction, targetVar, call);
				case StLoc stloc when stloc.Variable.IsSingleDefinition:
					// Address stored in local variable: also check all uses of that variable.
					if (!(stloc.Variable.Kind == VariableKind.StackSlot || stloc.Variable.Kind == VariableKind.Local))
						return AddressUse.Unknown;
					var value = stloc.Value;
					while (value is LdFlda ldFlda)
					{
						value = ldFlda.Target;
					}
					if (value.OpCode != OpCode.LdLoca)
					{
						// GroupStores only handles ref-locals correctly when they are supported by GetAddressLoadForRefLocalUse(),
						// which only works for ldflda*(ldloca)
						return AddressUse.Unknown;
					}
					foreach (var load in stloc.Variable.LoadInstructions)
					{
						if (DetermineAddressUse(load, targetVar) != AddressUse.Immediate)
							return AddressUse.Unknown;
					}
					return AddressUse.WithSupportedRefLocals;
				default:
					return AddressUse.Unknown;
			}
		}

		static AddressUse HandleCall(ILInstruction addressLoadingInstruction, ILVariable targetVar, CallInstruction call)
		{
			// Address is passed to method.
			// We'll assume the method only uses the address locally,
			// unless we can see an address being returned from the method:
			IType returnType = (call is NewObj) ? call.Method.DeclaringType : call.Method.ReturnType;
			if (returnType.IsByRefLike)
			{
				// If the address is returned from the method, it check whether it's consumed immediately.
				// This can still be fine, as long as we also check the consumer's other arguments for 'stloc targetVar'.
				if (DetermineAddressUse(call, targetVar) != AddressUse.Immediate)
					return AddressUse.Unknown;
			}
			foreach (var p in call.Method.Parameters)
			{
				// catch "out Span<int>" and similar
				if (p.Type.SkipModifiers() is ByReferenceType brt && brt.ElementType.IsByRefLike)
					return AddressUse.Unknown;
			}
			// ensure there's no 'stloc target' in between the ldloca and the call consuming the address
			for (int i = addressLoadingInstruction.ChildIndex + 1; i < call.Arguments.Count; i++)
			{
				foreach (var inst in call.Arguments[i].Descendants)
				{
					if (inst is StLoc store && store.Variable == targetVar)
						return AddressUse.Unknown;
				}
			}
			return AddressUse.Immediate;
		}

		/// <summary>
		/// Given 'ldloc ref_local' and 'ldloca target; stloc ref_local', returns the ldloca.
		/// This function must return a non-null LdLoca for every use of a SupportedRefLocal.
		/// </summary>
		static LdLoca GetAddressLoadForRefLocalUse(LdLoc ldloc)
		{
			if (!ldloc.Variable.IsSingleDefinition)
				return null; // only single-definition variables can be supported ref locals
			var store = ldloc.Variable.StoreInstructions.SingleOrDefault();
			if (store is StLoc stloc)
			{
				var value = stloc.Value;
				while (value is LdFlda ldFlda)
				{
					value = ldFlda.Target;
				}
				return value as LdLoca;
			}
			return null;
		}

		/// <summary>
		/// Use the union-find structure to merge
		/// </summary>
		/// <remarks>
		/// Instructions in a group are stores to the same variable that must stay together (cannot be split).
		/// </remarks>
		class GroupStores : ReachingDefinitionsVisitor
		{
			readonly UnionFind<IInstructionWithVariableOperand> unionFind = new UnionFind<IInstructionWithVariableOperand>();

			/// <summary>
			/// For each uninitialized variable, one representative instruction that
			/// potentially observes the unintialized value of the variable.
			/// Used to merge together all such loads of the same uninitialized value.
			/// </summary>
			readonly Dictionary<ILVariable, IInstructionWithVariableOperand> uninitVariableUsage = new Dictionary<ILVariable, IInstructionWithVariableOperand>();

			public GroupStores(ILFunction scope, CancellationToken cancellationToken) : base(scope, IsCandidateVariable, cancellationToken)
			{
			}

			protected internal override void VisitLdLoc(LdLoc inst)
			{
				base.VisitLdLoc(inst);
				HandleLoad(inst);
				var refLocalAddressLoad = GetAddressLoadForRefLocalUse(inst);
				if (refLocalAddressLoad != null)
				{
					// SupportedRefLocal: act as if we copy-propagated the ldloca
					// to the point of use:
					HandleLoad(refLocalAddressLoad);
				}
			}

			protected internal override void VisitLdLoca(LdLoca inst)
			{
				base.VisitLdLoca(inst);
				HandleLoad(inst);
			}

			void HandleLoad(IInstructionWithVariableOperand inst)
			{
				if (IsAnalyzedVariable(inst.Variable))
				{
					if (IsPotentiallyUninitialized(state, inst.Variable))
					{
						// merge all uninit loads together:
						if (uninitVariableUsage.TryGetValue(inst.Variable, out var uninitLoad))
						{
							unionFind.Merge(inst, uninitLoad);
						}
						else
						{
							uninitVariableUsage.Add(inst.Variable, inst);
						}
					}
					foreach (var store in GetStores(state, inst.Variable))
					{
						unionFind.Merge(inst, (IInstructionWithVariableOperand)store);
					}
				}
			}

			readonly Dictionary<IInstructionWithVariableOperand, ILVariable> newVariables = new Dictionary<IInstructionWithVariableOperand, ILVariable>();

			/// <summary>
			/// Gets the new variable for a LdLoc, StLoc or TryCatchHandler instruction.
			/// </summary>
			internal ILVariable GetNewVariable(IInstructionWithVariableOperand inst)
			{
				var representative = unionFind.Find(inst);
				ILVariable v;
				if (!newVariables.TryGetValue(representative, out v))
				{
					v = new ILVariable(inst.Variable.Kind, inst.Variable.Type, inst.Variable.StackType, inst.Variable.Index);
					v.Name = inst.Variable.Name;
					v.HasGeneratedName = inst.Variable.HasGeneratedName;
					v.StateMachineField = inst.Variable.StateMachineField;
					v.HasInitialValue = false; // we'll set HasInitialValue when we encounter an uninit load
					newVariables.Add(representative, v);
					inst.Variable.Function.Variables.Add(v);
				}
				if (inst.Variable.HasInitialValue && uninitVariableUsage.TryGetValue(inst.Variable, out var uninitLoad) && uninitLoad == inst)
				{
					v.HasInitialValue = true;
				}
				return v;
			}
		}
	}
}
