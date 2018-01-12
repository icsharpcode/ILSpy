// Copyright (c) 2018 Daniel Grunwald
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
using System.Text;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transform that converts code patterns like "v != null ? v.M() : null" to "v?.M()"
	/// </summary>
    struct NullPropagationTransform
    {
		readonly ILTransformContext context;
		
		public NullPropagationTransform(ILTransformContext context)
		{
			this.context = context;
		}

		/// <summary>
		/// Check if "condition ? trueInst : falseInst" can be simplified using the null-conditional operator.
		/// Returns the replacement instruction, or null if no replacement is possible.
		/// </summary>
		internal ILInstruction Run(ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst, Interval ilRange)
		{
			Debug.Assert(context.Settings.NullPropagation);
			Debug.Assert(!condition.MatchLogicNot(out _), "Caller should pass in positive condition");
			if (condition is Comp comp && comp.Left.MatchLdLoc(out var testedVar) && comp.Right.MatchLdNull()) {
				if (comp.LiftingKind != ComparisonLiftingKind.None)
					return null;
				if (comp.Kind == ComparisonKind.Equality) {
					// testedVar == null ? trueInst : falseInst
					return TryNullPropagation(testedVar, falseInst, trueInst, true, ilRange);
				} else if (comp.Kind == ComparisonKind.Inequality) {
					return TryNullPropagation(testedVar, trueInst, falseInst, true, ilRange);
				}
			} else if (NullableLiftingTransform.MatchHasValueCall(condition, out testedVar)) {
				// testedVar.HasValue ? trueInst : falseInst
				return TryNullPropagation(testedVar, trueInst, falseInst, false, ilRange);
			}
			return null;
		}

		/// <summary>
		/// testedVar != null ? nonNullInst : nullInst
		/// </summary>
		ILInstruction TryNullPropagation(ILVariable testedVar, ILInstruction nonNullInst, ILInstruction nullInst,
			bool testedVarHasReferenceType, Interval ilRange)
		{
			bool removedRewrapOrNullableCtor = false;
			if (NullableLiftingTransform.MatchNullableCtor(nonNullInst, out _, out var arg)) {
				nonNullInst = arg;
				removedRewrapOrNullableCtor = true;
			} else if (nonNullInst.MatchNullableRewrap(out arg)) {
				nonNullInst = arg;
				removedRewrapOrNullableCtor = true;
			}
			if (!IsValidAccessChain(testedVar, testedVarHasReferenceType, nonNullInst, out var varLoad))
				return null;
			// note: InferType will be accurate in this case because the access chain consists of calls and field accesses
			IType returnType = nonNullInst.InferType();
			if (nullInst.MatchLdNull()) {
				context.Step("Null propagation (reference type)", nonNullInst);
				// testedVar != null ? testedVar.AccessChain : null
				// => testedVar?.AccessChain
				IntroduceUnwrap(testedVar, varLoad);
				return new NullableRewrap(nonNullInst) { ILRange = ilRange };
			} else if (nullInst.MatchDefaultValue(out var type) && type.IsKnownType(KnownTypeCode.NullableOfT)) {
				context.Step("Null propagation (value type)", nonNullInst);
				// testedVar != null ? testedVar.AccessChain : default(T?)
				// => testedVar?.AccessChain
				IntroduceUnwrap(testedVar, varLoad);
				return new NullableRewrap(nonNullInst) { ILRange = ilRange };
			} else if (!removedRewrapOrNullableCtor && NullableType.IsNonNullableValueType(returnType)) {
				context.Step("Null propagation with null coalescing", nonNullInst);
				// testedVar != null ? testedVar.AccessChain : nullInst
				// => testedVar?.AccessChain ?? nullInst
				// (only valid if AccessChain returns a non-nullable value)
				IntroduceUnwrap(testedVar, varLoad);
				return new NullCoalescingInstruction(
					NullCoalescingKind.NullableWithValueFallback,
					new NullableRewrap(nonNullInst),
					nullInst
				) {
					UnderlyingResultType = nullInst.ResultType,
					ILRange = ilRange
				};
			}
			return null;
		}

		bool IsValidAccessChain(ILVariable testedVar, bool testedVarHasReferenceType, ILInstruction inst, out ILInstruction finalLoad)
		{
			finalLoad = null;
			int chainLength = 0;
			while (true) {
				if (IsValidEndOfChain()) {
					// valid end of chain
					finalLoad = inst;
					return chainLength >= 1;
				} else if (inst.MatchLdFld(out var target, out _)) {
					inst = target;
				} else if (inst is CallInstruction call && call.OpCode != OpCode.NewObj) {
					if (call.Arguments.Count == 0) {
						return false;
					}
					if (call.Method.IsStatic && !call.Method.IsExtensionMethod) {
						return false; // only instance or extension methods can be called with ?. syntax
					}
					inst = call.Arguments[0];
					// ensure the access chain does not contain any 'nullable.unwrap' that aren't directly part of the chain
					for (int i = 1; i < call.Arguments.Count; ++i) {
						if (call.Arguments[i].HasFlag(InstructionFlags.MayUnwrapNull)) {
							return false;
						}
					}
				} else if (inst is NullableUnwrap unwrap) {
					inst = unwrap.Argument;
				} else {
					// unknown node -> invalid chain
					return false;
				}
				chainLength++;
			}

			bool IsValidEndOfChain()
			{
				if (testedVarHasReferenceType) {
					return inst.MatchLdLoc(testedVar);
				} else {
					return NullableLiftingTransform.MatchGetValueOrDefault(inst, testedVar);
				}
			}
		}

		private void IntroduceUnwrap(ILVariable testedVar, ILInstruction varLoad)
		{
			if (NullableLiftingTransform.MatchGetValueOrDefault(varLoad, testedVar)) {
				varLoad.ReplaceWith(new NullableUnwrap(
					varLoad.ResultType,
					new LdLoc(testedVar) { ILRange = varLoad.Children[0].ILRange }
				) { ILRange = varLoad.ILRange });
			} else {
				// Wrap varLoad in nullable.unwrap:
				var children = varLoad.Parent.Children;
				children[varLoad.ChildIndex] = new NullableUnwrap(testedVar.StackType, varLoad);
			}
		}
	}
}
