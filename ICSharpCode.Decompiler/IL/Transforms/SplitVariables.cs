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
using ICSharpCode.Decompiler.FlowAnalysis;
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
			var groupStores = new GroupStores(function);
			function.Body.AcceptVisitor(groupStores);
			var newVariables = new Dictionary<ILInstruction, ILVariable>();
			// Replace analyzed variables with their split versions:
			foreach (var inst in function.Descendants.OfType<IInstructionWithVariableOperand>()) {
				if (groupStores.IsAnalyzedVariable(inst.Variable)) {
					inst.Variable = groupStores.GetNewVariable(inst);
				}
			}
			function.Variables.RemoveDead();
		}

		static bool IsCandidateVariable(ILVariable v)
		{
			switch (v.Kind) {
				case VariableKind.Local:
				case VariableKind.Exception:
					return v.AddressCount == 0;
				default:
					// parameters: avoid splitting parameters
					// stack slots: are already split by construction
					// pinned locals: must not split (doing so would extend the life of the pin to the end of the method)
					return false;
			}
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
			readonly HashSet<IInstructionWithVariableOperand> uninitVariableUsage = new HashSet<IInstructionWithVariableOperand>();
			
			public GroupStores(ILVariableScope scope) : base(scope, IsCandidateVariable)
			{
			}
			
			protected internal override void VisitLdLoc(LdLoc inst)
			{
				base.VisitLdLoc(inst);
				if (IsAnalyzedVariable(inst.Variable)) {
					if (IsPotentiallyUninitialized(state, inst.Variable)) {
						uninitVariableUsage.Add(inst);
					}
					foreach (var store in GetStores(state, inst.Variable)) {
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
				if (!newVariables.TryGetValue(representative, out v)) {
					v = new ILVariable(inst.Variable.Kind, inst.Variable.Type, inst.Variable.StackType, inst.Variable.Index);
					v.Name = inst.Variable.Name;
					v.HasInitialValue = false; // we'll set HasInitialValue when we encounter an uninit load
					newVariables.Add(representative, v);
					inst.Variable.Scope.Variables.Add(v);
				}
				if (uninitVariableUsage.Contains(inst)) {
					v.HasInitialValue = true;
				}
				return v;
			}
		}
	}
}
