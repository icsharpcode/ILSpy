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
		internal static bool IsProtectedIfInst(IfInstruction ifInst)
		{
			// We exclude logic.and to avoid turning
			// "logic.and(comp(interfaces != ldnull), call get_Count(interfaces))"
			// into "if ((interfaces?.Count ?? 0) != 0)".
			return (ifInst.MatchLogicAnd(out _, out _) || ifInst.MatchLogicOr(out _, out _))
				&& IfInstruction.IsInConditionSlot(ifInst);
		}

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

		/// <summary>
		/// if (x != null) x.AccessChain();
		/// => x?.AccessChain();
		/// </summary>
		internal void RunStatements(Block block, int pos)
		{
			var ifInst = block.Instructions[pos] as IfInstruction;
			if (ifInst == null || !ifInst.FalseInst.MatchNop())
				return;
			if (ifInst.Condition is Comp comp && comp.Kind == ComparisonKind.Inequality
				&& comp.Left.MatchLdLoc(out var testedVar) && comp.Right.MatchLdNull()) {
				TryNullPropForVoidCall(testedVar, true, ifInst.TrueInst as Block, ifInst);
			} else if (NullableLiftingTransform.MatchHasValueCall(ifInst.Condition, out testedVar)) {
				TryNullPropForVoidCall(testedVar, false, ifInst.TrueInst as Block, ifInst);
			}
		}

		void TryNullPropForVoidCall(ILVariable testedVar, bool testedVarHasReferenceType, Block body, IfInstruction ifInst)
		{
			if (body == null || body.Instructions.Count != 1)
				return;
			var bodyInst = body.Instructions[0];
			if (bodyInst.MatchNullableRewrap(out var arg)) {
				bodyInst = arg;
			}
			if (!IsValidAccessChain(testedVar, testedVarHasReferenceType, bodyInst, out var varLoad))
				return;
			context.Step("Null-propagation (void call)", body);
			// if (testedVar != null) { testedVar.AccessChain(); }
			// => testedVar?.AccessChain();
			IntroduceUnwrap(testedVar, varLoad);
			ifInst.ReplaceWith(new NullableRewrap(
				bodyInst
			) { ILRange = ifInst.ILRange });
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
					if (call.Method.IsAccessor && !IsGetter(call.Method)) {
						return false; // setter/adder/remover cannot be called with ?. syntax
					}
					inst = call.Arguments[0];
					if ((call.ConstrainedTo ?? call.Method.DeclaringType).IsReferenceType == false && inst.MatchAddressOf(out var arg)) {
						inst = arg;
					}
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
					// either reference type (expect: ldloc(testedVar)) or unconstrained generic type (expect: ldloca(testedVar)).
					return inst.MatchLdLocRef(testedVar);
				} else {
					return NullableLiftingTransform.MatchGetValueOrDefault(inst, testedVar);
				}
			}
		}

		static bool IsGetter(IMethod method)
		{
			return method.AccessorOwner is IProperty p && p.Getter == method;
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
				children[varLoad.ChildIndex] = new NullableUnwrap(varLoad.ResultType, varLoad);
			}
		}
	}

	class NullPropagationStatementTransform : IStatementTransform
	{
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.NullPropagation)
				return;
			new NullPropagationTransform(context).RunStatements(block, pos);
		}
	}
}
