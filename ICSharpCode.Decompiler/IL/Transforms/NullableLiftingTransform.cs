// Copyright (c) 2017 Daniel Grunwald
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
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Nullable lifting gets run in two places:
	///  * the usual form looks at an if-else, and runs within the ExpressionTransforms.
	///  * the NullableLiftingBlockTransform handles the cases where Roslyn generates
	///    two 'ret' statements for the null/non-null cases of a lifted operator.
	/// 
	/// The transform handles the following languages constructs:
	///  * lifted conversions
	///  * lifted unary and binary operators
	///  * lifted comparisons
	///  * the ?? operator with type Nullable{T} on the left-hand-side
	///  * the ?. operator (via NullPropagationTransform)
	/// </summary>
	struct NullableLiftingTransform
	{
		readonly ILTransformContext context;
		List<ILVariable> nullableVars;

		public NullableLiftingTransform(ILTransformContext context)
		{
			this.context = context;
			this.nullableVars = null;
		}

		#region Run
		/// <summary>
		/// Main entry point into the normal code path of this transform.
		/// Called by expression transform.
		/// </summary>
		public bool Run(IfInstruction ifInst)
		{
			var lifted = Lift(ifInst, ifInst.Condition, ifInst.TrueInst, ifInst.FalseInst);
			if (lifted != null) {
				ifInst.ReplaceWith(lifted);
				return true;
			}
			return false;
		}

		/// <summary>
		/// VS2017.8 / Roslyn 2.9 started optimizing some cases of
		///   "a.GetValueOrDefault() == b.GetValueOrDefault() &amp;&amp; (a.HasValue &amp; b.HasValue)"
		/// to
		///   "(a.GetValueOrDefault() == b.GetValueOrDefault()) &amp; (a.HasValue &amp; b.HasValue)"
		/// so this secondary entry point analyses logic.and as-if it was a short-circuting &amp;&amp;.
		/// </summary>
		public bool Run(BinaryNumericInstruction bni)
		{
			Debug.Assert(!bni.IsLifted && bni.Operator == BinaryNumericOperator.BitAnd);
			// caller ensures that bni.Left/bni.Right are booleans
			var lifted = Lift(bni, bni.Left, bni.Right, new LdcI4(0));
			if (lifted != null) {
				bni.ReplaceWith(lifted);
				return true;
			}
			return false;
		}

		public bool RunStatements(Block block, int pos)
		{
			// e.g.:
			//  if (!condition) Block {
			//    leave IL_0000 (default.value System.Nullable`1[[System.Int64]])
			//  }
			//  leave IL_0000 (newobj .ctor(exprToLift))
			if (pos != block.Instructions.Count - 2)
				return false;
			if (!(block.Instructions[pos] is IfInstruction ifInst))
				return false;
			if (!(Block.Unwrap(ifInst.TrueInst) is Leave thenLeave))
				return false;
			if (!ifInst.FalseInst.MatchNop())
				return false;

			if (!(block.Instructions[pos + 1] is Leave elseLeave))
				return false;
			if (elseLeave.TargetContainer != thenLeave.TargetContainer)
				return false;

			var lifted = Lift(ifInst, ifInst.Condition, thenLeave.Value, elseLeave.Value);
			if (lifted != null) {
				thenLeave.Value = lifted;
				ifInst.ReplaceWith(thenLeave);
				block.Instructions.Remove(elseLeave);
				return true;
			}
			return false;
		}
		#endregion

		#region AnalyzeCondition
		bool AnalyzeCondition(ILInstruction condition)
		{
			if (MatchHasValueCall(condition, out ILVariable v)) {
				if (nullableVars == null)
					nullableVars = new List<ILVariable>();
				nullableVars.Add(v);
				return true;
			} else if (condition is BinaryNumericInstruction bitand) {
				if (!(bitand.Operator == BinaryNumericOperator.BitAnd && bitand.ResultType == StackType.I4))
					return false;
				return AnalyzeCondition(bitand.Left) && AnalyzeCondition(bitand.Right);
			}
			return false;
		}
		#endregion

		#region Main lifting logic
		/// <summary>
		/// Main entry point for lifting; called by both the expression-transform
		/// and the block transform.
		/// </summary>
		ILInstruction Lift(ILInstruction ifInst, ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst)
		{
			// ifInst is usually the IfInstruction to which condition belongs;
			// but can also be a BinaryNumericInstruction.
			while (condition.MatchLogicNot(out var arg)) {
				condition = arg;
				ExtensionMethods.Swap(ref trueInst, ref falseInst);
			}
			if (context.Settings.NullPropagation && !NullPropagationTransform.IsProtectedIfInst(ifInst as IfInstruction)) {
				var nullPropagated = new NullPropagationTransform(context)
					.Run(condition, trueInst, falseInst)?.WithILRange(ifInst);
				if (nullPropagated != null)
					return nullPropagated;
			}
			if (!context.Settings.LiftNullables)
				return null;
			if (AnalyzeCondition(condition)) {
				// (v1 != null && ... && vn != null) ? trueInst : falseInst
				// => normal lifting
				return LiftNormal(trueInst, falseInst)?.WithILRange(ifInst);
			}
			if (MatchCompOrDecimal(condition, out var comp)) {
				// This might be a C#-style lifted comparison
				// (C# checks the underlying value before checking the HasValue bits)
				if (comp.Kind.IsEqualityOrInequality()) {
					// for equality/inequality, the HasValue bits must also compare equal/inequal
					if (comp.Kind == ComparisonKind.Inequality) {
						// handle inequality by swapping one last time
						ExtensionMethods.Swap(ref trueInst, ref falseInst);
					}
					if (falseInst.MatchLdcI4(0)) {
						// (a.GetValueOrDefault() == b.GetValueOrDefault()) ? (a.HasValue == b.HasValue) : false
						// => a == b
						return LiftCSharpEqualityComparison(comp, ComparisonKind.Equality, trueInst)
							?? LiftCSharpUserEqualityComparison(comp, ComparisonKind.Equality, trueInst);
					} else if (falseInst.MatchLdcI4(1)) {
						// (a.GetValueOrDefault() == b.GetValueOrDefault()) ? (a.HasValue != b.HasValue) : true
						// => a != b
						return LiftCSharpEqualityComparison(comp, ComparisonKind.Inequality, trueInst)
							?? LiftCSharpUserEqualityComparison(comp, ComparisonKind.Inequality, trueInst);
					} else if (IsGenericNewPattern(comp.Left, comp.Right, trueInst, falseInst)) {
						// (default(T) == null) ? Activator.CreateInstance<T>() : default(T)
						// => Activator.CreateInstance<T>()
						return trueInst;
					}
				} else {
					// Not (in)equality, but one of < <= > >=.
					// Returns false unless all HasValue bits are true.
					if (falseInst.MatchLdcI4(0) && AnalyzeCondition(trueInst)) {
						// comp(lhs, rhs) ? (v1 != null && ... && vn != null) : false
						// => comp.lifted[C#](lhs, rhs)
						return LiftCSharpComparison(comp, comp.Kind);
					} else if (trueInst.MatchLdcI4(0) && AnalyzeCondition(falseInst)) {
						// comp(lhs, rhs) ? false : (v1 != null && ... && vn != null)
						return LiftCSharpComparison(comp, comp.Kind.Negate());
					}
				}
			}
			ILVariable v;
			// Handle equality comparisons with bool?:
			if (MatchGetValueOrDefault(condition, out v)
				&& NullableType.GetUnderlyingType(v.Type).IsKnownType(KnownTypeCode.Boolean))
			{
				if (MatchHasValueCall(trueInst, v) && falseInst.MatchLdcI4(0)) {
					// v.GetValueOrDefault() ? v.HasValue : false
					// ==> v == true
					context.Step("NullableLiftingTransform: v == true", ifInst);
					return new Comp(ComparisonKind.Equality, ComparisonLiftingKind.CSharp,
						StackType.I4, Sign.None,
						new LdLoc(v).WithILRange(trueInst),
						new LdcI4(1).WithILRange(falseInst)
					).WithILRange(ifInst);
				} else if (trueInst.MatchLdcI4(0) && MatchHasValueCall(falseInst, v)) {
					// v.GetValueOrDefault() ? false : v.HasValue
					// ==> v == false
					context.Step("NullableLiftingTransform: v == false", ifInst);
					return new Comp(ComparisonKind.Equality, ComparisonLiftingKind.CSharp,
						StackType.I4, Sign.None,
						new LdLoc(v).WithILRange(falseInst),
						trueInst // LdcI4(0)
					).WithILRange(ifInst);
				} else if (MatchNegatedHasValueCall(trueInst, v) && falseInst.MatchLdcI4(1)) {
					// v.GetValueOrDefault() ? !v.HasValue : true
					// ==> v != true
					context.Step("NullableLiftingTransform: v != true", ifInst);
					return new Comp(ComparisonKind.Inequality, ComparisonLiftingKind.CSharp,
						StackType.I4, Sign.None,
						new LdLoc(v).WithILRange(trueInst),
						falseInst // LdcI4(1)
					).WithILRange(ifInst);
				} else if (trueInst.MatchLdcI4(1) && MatchNegatedHasValueCall(falseInst, v)) {
					// v.GetValueOrDefault() ? true : !v.HasValue
					// ==> v != false
					context.Step("NullableLiftingTransform: v != false", ifInst);
					return new Comp(ComparisonKind.Inequality, ComparisonLiftingKind.CSharp,
						StackType.I4, Sign.None,
						new LdLoc(v).WithILRange(falseInst),
						new LdcI4(0).WithILRange(trueInst)
					).WithILRange(ifInst);
				}
			}
			// Handle & and | on bool?:
			if (trueInst.MatchLdLoc(out v)) {
				if (MatchNullableCtor(falseInst, out var utype, out var arg)
					&& utype.IsKnownType(KnownTypeCode.Boolean) && arg.MatchLdcI4(0))
				{
					// condition ? v : (bool?)false
					// => condition & v
					context.Step("NullableLiftingTransform: 3vl.bool.and(bool, bool?)", ifInst);
					return new ThreeValuedBoolAnd(condition, trueInst).WithILRange(ifInst);
				}
				if (falseInst.MatchLdLoc(out var v2)) {
					// condition ? v : v2
					if (MatchThreeValuedLogicConditionPattern(condition, out var nullable1, out var nullable2)) {
						// (nullable1.GetValueOrDefault() || (!nullable2.GetValueOrDefault() && !nullable1.HasValue)) ? v : v2
						if (v == nullable1 && v2 == nullable2) {
							context.Step("NullableLiftingTransform: 3vl.bool.or(bool?, bool?)", ifInst);
							return new ThreeValuedBoolOr(trueInst, falseInst).WithILRange(ifInst);
						} else if (v == nullable2 && v2 == nullable1) {
							context.Step("NullableLiftingTransform: 3vl.bool.and(bool?, bool?)", ifInst);
							return new ThreeValuedBoolAnd(falseInst, trueInst).WithILRange(ifInst);
						}
					}
				}
			} else if (falseInst.MatchLdLoc(out v)) {
				if (MatchNullableCtor(trueInst, out var utype, out var arg)
					&& utype.IsKnownType(KnownTypeCode.Boolean) && arg.MatchLdcI4(1)) {
					// condition ? (bool?)true : v
					// => condition | v
					context.Step("NullableLiftingTransform: 3vl.logic.or(bool, bool?)", ifInst);
					return new ThreeValuedBoolOr(condition, falseInst).WithILRange(ifInst);
				}
			}
			return null;
		}

		private bool IsGenericNewPattern(ILInstruction compLeft, ILInstruction compRight, ILInstruction trueInst, ILInstruction falseInst)
		{
			// (default(T) == null) ? Activator.CreateInstance<T>() : default(T)
			return falseInst.MatchDefaultValue(out var type) &&
				(trueInst is Call c && c.Method.FullName == "System.Activator.CreateInstance" && c.Method.TypeArguments.Count == 1) &&
				type.Kind == TypeKind.TypeParameter &&
				compLeft.MatchDefaultValue(out var type2) &&
				type.Equals(type2) &&
				compRight.MatchLdNull();
		}

		private bool MatchThreeValuedLogicConditionPattern(ILInstruction condition, out ILVariable nullable1, out ILVariable nullable2)
		{
			// Try to match: nullable1.GetValueOrDefault() || (!nullable2.GetValueOrDefault() && !nullable1.HasValue)
			nullable1 = null;
			nullable2 = null;
			if (!condition.MatchLogicOr(out var lhs, out var rhs))
				return false;
			if (!MatchGetValueOrDefault(lhs, out nullable1))
				return false;
			if (!NullableType.GetUnderlyingType(nullable1.Type).IsKnownType(KnownTypeCode.Boolean))
				return false;
			if (!rhs.MatchLogicAnd(out lhs, out rhs))
				return false;
			if (!lhs.MatchLogicNot(out var arg))
				return false;
			if (!MatchGetValueOrDefault(arg, out nullable2))
				return false;
			if (!NullableType.GetUnderlyingType(nullable2.Type).IsKnownType(KnownTypeCode.Boolean))
				return false;
			if (!rhs.MatchLogicNot(out arg))
				return false;
			return MatchHasValueCall(arg, nullable1);
		}
		#endregion

		#region CSharpComp
		static bool MatchCompOrDecimal(ILInstruction inst, out CompOrDecimal result)
		{
			result = default(CompOrDecimal);
			result.Instruction = inst;
			if (inst is Comp comp && !comp.IsLifted) {
				result.Kind = comp.Kind;
				result.Left = comp.Left;
				result.Right = comp.Right;
				return true;
			} else if (inst is Call call && call.Method.IsOperator && call.Arguments.Count == 2 && !call.IsLifted) {
				switch (call.Method.Name) {
					case "op_Equality":
						result.Kind = ComparisonKind.Equality;
						break;
					case "op_Inequality":
						result.Kind = ComparisonKind.Inequality;
						break;
					case "op_LessThan":
						result.Kind = ComparisonKind.LessThan;
						break;
					case "op_LessThanOrEqual":
						result.Kind = ComparisonKind.LessThanOrEqual;
						break;
					case "op_GreaterThan":
						result.Kind = ComparisonKind.GreaterThan;
						break;
					case "op_GreaterThanOrEqual":
						result.Kind = ComparisonKind.GreaterThanOrEqual;
						break;
					default:
						return false;
				}
				result.Left = call.Arguments[0];
				result.Right = call.Arguments[1];
				return call.Method.DeclaringType.IsKnownType(KnownTypeCode.Decimal);
			}
			return false;
		}

		/// <summary>
		/// Represents either non-lifted IL `Comp` or a call to one of the (non-lifted) 6 comparison operators on `System.Decimal`.
		/// </summary>
		struct CompOrDecimal
		{
			public ILInstruction Instruction;
			public ComparisonKind Kind;
			public ILInstruction Left;
			public ILInstruction Right;

			public IType LeftExpectedType {
				get {
					if (Instruction is Call call) {
						return call.Method.Parameters[0].Type;
					} else {
						return SpecialType.UnknownType;
					}
				}
			}

			public IType RightExpectedType {
				get {
					if (Instruction is Call call) {
						return call.Method.Parameters[1].Type;
					} else {
						return SpecialType.UnknownType;
					}
				}
			}

			internal ILInstruction MakeLifted(ComparisonKind newComparisonKind, ILInstruction left, ILInstruction right)
			{
				if (Instruction is Comp comp) {
					return new Comp(newComparisonKind, ComparisonLiftingKind.CSharp, comp.InputType, comp.Sign, left, right).WithILRange(Instruction);
				} else if (Instruction is Call call) {
					IMethod method;
					if (newComparisonKind == Kind) {
						method = call.Method;
					} else if (newComparisonKind == ComparisonKind.Inequality && call.Method.Name == "op_Equality") {
						method = call.Method.DeclaringType.GetMethods(m => m.Name == "op_Inequality")
							.FirstOrDefault(m => ParameterListComparer.Instance.Equals(m.Parameters, call.Method.Parameters));
						if (method == null)
							return null;
					} else {
						return null;
					}
					return new Call(CSharp.Resolver.CSharpOperators.LiftUserDefinedOperator(method)) {
						Arguments = { left, right },
						ConstrainedTo = call.ConstrainedTo,
						ILStackWasEmpty = call.ILStackWasEmpty,
						IsTail = call.IsTail
					}.WithILRange(call);
				} else {
					return null;
				}
			}
		}
		#endregion

		#region Lift...Comparison
		ILInstruction LiftCSharpEqualityComparison(CompOrDecimal valueComp, ComparisonKind newComparisonKind, ILInstruction hasValueTest)
		{
			Debug.Assert(newComparisonKind.IsEqualityOrInequality());
			bool hasValueTestNegated = false;
			while (hasValueTest.MatchLogicNot(out var arg)) {
				hasValueTest = arg;
				hasValueTestNegated = !hasValueTestNegated;
			}
			// The HasValue comparison must be the same operator as the Value comparison.
			if (hasValueTest is Comp hasValueComp) {
				// Comparing two nullables: HasValue comparison must be the same operator as the Value comparison
				if ((hasValueTestNegated ? hasValueComp.Kind.Negate() : hasValueComp.Kind) != newComparisonKind)
					return null;
				if (!MatchHasValueCall(hasValueComp.Left, out ILVariable leftVar))
					return null;
				if (!MatchHasValueCall(hasValueComp.Right, out ILVariable rightVar))
					return null;
				nullableVars = new List<ILVariable> { leftVar };
				var (left, leftBits) = DoLift(valueComp.Left);
				nullableVars[0] = rightVar;
				var (right, rightBits) = DoLift(valueComp.Right);
				if (left != null && right != null && leftBits[0] && rightBits[0]
					&& SemanticHelper.IsPure(left.Flags) && SemanticHelper.IsPure(right.Flags)
				) {
					context.Step("NullableLiftingTransform: C# (in)equality comparison", valueComp.Instruction);
					return valueComp.MakeLifted(newComparisonKind, left, right);
				}
			} else if (newComparisonKind == ComparisonKind.Equality && !hasValueTestNegated && MatchHasValueCall(hasValueTest, out ILVariable v)) {
				// Comparing nullable with non-nullable -> we can fall back to the normal comparison code.
				nullableVars = new List<ILVariable> { v };
				return LiftCSharpComparison(valueComp, newComparisonKind);
			} else if (newComparisonKind == ComparisonKind.Inequality && hasValueTestNegated && MatchHasValueCall(hasValueTest, out v)) {
				// Comparing nullable with non-nullable -> we can fall back to the normal comparison code.
				nullableVars = new List<ILVariable> { v };
				return LiftCSharpComparison(valueComp, newComparisonKind);
			}
			return null;
		}

		/// <summary>
		/// Lift a C# comparison.
		/// This method cannot be used for (in)equality comparisons where both sides are nullable
		/// (these special cases are handled in LiftCSharpEqualityComparison instead).
		/// 
		/// The output instructions should evaluate to <c>false</c> when any of the <c>nullableVars</c> is <c>null</c>
		///   (except for newComparisonKind==Inequality, where this case should evaluate to <c>true</c> instead).
		/// Otherwise, the output instruction should evaluate to the same value as the input instruction.
		/// The output instruction should have the same side-effects (incl. exceptions being thrown) as the input instruction.
		/// This means unlike LiftNormal(), we cannot rely on the input instruction not being evaluated if
		/// a variable is <c>null</c>.
		/// </summary>
		ILInstruction LiftCSharpComparison(CompOrDecimal comp, ComparisonKind newComparisonKind)
		{
			var (left, right, bits) = DoLiftBinary(comp.Left, comp.Right, comp.LeftExpectedType, comp.RightExpectedType);
			// due to the restrictions on side effects, we only allow instructions that are pure after lifting.
			// (we can't check this before lifting due to the calls to GetValueOrDefault())
			if (left != null && right != null && SemanticHelper.IsPure(left.Flags) && SemanticHelper.IsPure(right.Flags)) {
				if (!bits.All(0, nullableVars.Count)) {
					// don't lift if a nullableVar doesn't contribute to the result
					return null;
				}
				context.Step("NullableLiftingTransform: C# comparison", comp.Instruction);
				return comp.MakeLifted(newComparisonKind, left, right);
			}
			return null;
		}

		Call LiftCSharpUserEqualityComparison(CompOrDecimal hasValueComp, ComparisonKind newComparisonKind, ILInstruction nestedIfInst)
		{
			// User-defined equality operator:
			//   if (comp(call get_HasValue(ldloca nullable1) == call get_HasValue(ldloca nullable2)))
			//      if (logic.not(call get_HasValue(ldloca nullable)))
			//          ldc.i4 1
			//      else
			//          call op_Equality(call GetValueOrDefault(ldloca nullable1), call GetValueOrDefault(ldloca nullable2)
			//   else
			//      ldc.i4 0

			// User-defined inequality operator:
			//   if (comp(call get_HasValue(ldloca nullable1) != call get_HasValue(ldloca nullable2)))
			//      ldc.i4 1
			//   else
			//      if (call get_HasValue(ldloca nullable))
			//         call op_Inequality(call GetValueOrDefault(ldloca nullable1), call GetValueOrDefault(ldloca nullable2))
			//      else
			//         ldc.i4 0

			if (!MatchHasValueCall(hasValueComp.Left, out ILVariable nullable1))
				return null;
			if (!MatchHasValueCall(hasValueComp.Right, out ILVariable nullable2))
				return null;
			if (!nestedIfInst.MatchIfInstructionPositiveCondition(out var condition, out var trueInst, out var falseInst))
				return null;
			if (!MatchHasValueCall(condition, out ILVariable nullable))
				return null;
			if (nullable != nullable1 && nullable != nullable2)
				return null;
			if (!falseInst.MatchLdcI4(newComparisonKind == ComparisonKind.Equality ? 1 : 0))
				return null;
			if (!(trueInst is Call call))
				return null;
			if (!(call.Method.IsOperator && call.Arguments.Count == 2))
				return null;
			if (call.Method.Name != (newComparisonKind == ComparisonKind.Equality ? "op_Equality" : "op_Inequality"))
				return null;
			var liftedOperator = CSharp.Resolver.CSharpOperators.LiftUserDefinedOperator(call.Method);
			if (liftedOperator == null)
				return null;
			nullableVars = new List<ILVariable> { nullable1 };
			var (left, leftBits) = DoLift(call.Arguments[0]);
			nullableVars[0] = nullable2;
			var (right, rightBits) = DoLift(call.Arguments[1]);
			if (left != null && right != null && leftBits[0] && rightBits[0]
				&& SemanticHelper.IsPure(left.Flags) && SemanticHelper.IsPure(right.Flags)
			) {
				context.Step("NullableLiftingTransform: C# user-defined (in)equality comparison", nestedIfInst);
				return new Call(liftedOperator) {
					Arguments = { left, right },
					ConstrainedTo = call.ConstrainedTo,
					ILStackWasEmpty = call.ILStackWasEmpty,
					IsTail = call.IsTail,
				}.WithILRange(call);
			}
			return null;
		}
		#endregion

		#region LiftNormal / DoLift
		/// <summary>
		/// Performs nullable lifting.
		/// 
		/// Produces a lifted instruction with semantics equivalent to:
		///   (v1 != null &amp;&amp; ... &amp;&amp; vn != null) ? trueInst : falseInst,
		/// where the v1,...,vn are the <c>this.nullableVars</c>.
		/// If lifting fails, returns <c>null</c>.
		/// </summary>
		ILInstruction LiftNormal(ILInstruction trueInst, ILInstruction falseInst)
		{
			if (trueInst.MatchIfInstructionPositiveCondition(out var nestedCondition, out var nestedTrue, out var nestedFalse)) {
				// Sometimes Roslyn generates pointless conditions like:
				//   if (nullable.HasValue && (!nullable.HasValue || nullable.GetValueOrDefault() == b))
				if (MatchHasValueCall(nestedCondition, out ILVariable v) && nullableVars.Contains(v)) {
					trueInst = nestedTrue;
				}
			}

			bool isNullCoalescingWithNonNullableFallback = false;
			if (!MatchNullableCtor(trueInst, out var utype, out var exprToLift)) {
				isNullCoalescingWithNonNullableFallback = true;
				utype = context.TypeSystem.FindType(trueInst.ResultType.ToKnownTypeCode());
				exprToLift = trueInst;
				if (nullableVars.Count == 1 && exprToLift.MatchLdLoc(nullableVars[0])) {
					// v.HasValue ? ldloc v : fallback
					// => v ?? fallback
					context.Step("v.HasValue ? v : fallback => v ?? fallback", trueInst);
					return new NullCoalescingInstruction(NullCoalescingKind.Nullable, trueInst, falseInst) {
						UnderlyingResultType = NullableType.GetUnderlyingType(nullableVars[0].Type).GetStackType()
					};
				} else if (trueInst is Call call && !call.IsLifted
					&& CSharp.Resolver.CSharpOperators.IsComparisonOperator(call.Method)
					&& falseInst.MatchLdcI4(call.Method.Name == "op_Inequality" ? 1 : 0))
				{
					// (v1 != null && ... && vn != null) ? call op_LessThan(lhs, rhs) : ldc.i4(0)
					var liftedOperator = CSharp.Resolver.CSharpOperators.LiftUserDefinedOperator(call.Method);
					if ((call.Method.Name == "op_Equality" || call.Method.Name == "op_Inequality") && nullableVars.Count != 1) {
						// Equality is special (returns true if both sides are null), only handle it
						// in the normal code path if we're dealing with only a single nullable var
						// (comparing nullable with non-nullable).
						liftedOperator = null;
					}
					if (liftedOperator != null) {
						context.Step("Lift user-defined comparison operator", trueInst);
						var (left, right, bits) = DoLiftBinary(call.Arguments[0], call.Arguments[1],
							call.Method.Parameters[0].Type, call.Method.Parameters[1].Type);
						if (left != null && right != null && bits.All(0, nullableVars.Count)) {
							return new Call(liftedOperator) {
								Arguments = { left, right },
								ConstrainedTo = call.ConstrainedTo,
								ILStackWasEmpty = call.ILStackWasEmpty,
								IsTail = call.IsTail
							}.WithILRange(call);
						}
					}
				}
			}
			ILInstruction lifted;
			if (nullableVars.Count == 1 && MatchGetValueOrDefault(exprToLift, nullableVars[0])) {
				// v.HasValue ? call GetValueOrDefault(ldloca v) : fallback
				// => conv.nop.lifted(ldloc v) ?? fallback
				// This case is handled separately from DoLift() because
				// that doesn't introduce nop-conversions.
				context.Step("v.HasValue ? v.GetValueOrDefault() : fallback => v ?? fallback", trueInst);
				var inputUType = NullableType.GetUnderlyingType(nullableVars[0].Type);
				lifted = new LdLoc(nullableVars[0]);
				if (!inputUType.Equals(utype) && utype.ToPrimitiveType() != PrimitiveType.None) {
					// While the ILAst allows implicit conversions between short and int
					// (because both map to I4); it does not allow implicit conversions
					// between short? and int? (structs of different types).
					// So use 'conv.nop.lifted' to allow the conversion.
					lifted = new Conv(
						lifted,
						inputUType.GetStackType(), inputUType.GetSign(), utype.ToPrimitiveType(),
						checkForOverflow: false,
						isLifted: true
					);
				}
			} else {
				context.Step("NullableLiftingTransform.DoLift", trueInst);
				BitSet bits;
				(lifted, bits) = DoLift(exprToLift);
				if (lifted == null) {
					return null;
				}
				if (!bits.All(0, nullableVars.Count)) {
					// don't lift if a nullableVar doesn't contribute to the result
					return null;
				}
				Debug.Assert(lifted is ILiftableInstruction liftable && liftable.IsLifted
					&& liftable.UnderlyingResultType == exprToLift.ResultType);
			}
			if (isNullCoalescingWithNonNullableFallback) {
				lifted = new NullCoalescingInstruction(NullCoalescingKind.NullableWithValueFallback, lifted, falseInst) {
					UnderlyingResultType = exprToLift.ResultType
				};
			} else if (!MatchNull(falseInst, utype)) {
				// Normal lifting, but the falseInst isn't `default(utype?)`
				// => use the `??` operator to provide the fallback value.
				lifted = new NullCoalescingInstruction(NullCoalescingKind.Nullable, lifted, falseInst) {
					UnderlyingResultType = exprToLift.ResultType
				};
			}
			return lifted;
		}

		/// <summary>
		/// Recursive function that lifts the specified instruction.
		/// The input instruction is expected to a subexpression of the trueInst
		/// (so that all nullableVars are guaranteed non-null within this expression).
		/// 
		/// Creates a new lifted instruction without modifying the input instruction.
		/// On success, returns (new lifted instruction, bitset).
		/// If lifting fails, returns (null, null).
		/// 
		/// The returned bitset specifies which nullableVars were considered "relevant" for this instruction.
		/// bitSet[i] == true means nullableVars[i] was relevant.
		/// 
		/// The new lifted instruction will have equivalent semantics to the input instruction
		/// if all relevant variables are non-null [except that the result will be wrapped in a Nullable{T} struct].
		/// If any relevant variable is null, the new instruction is guaranteed to evaluate to <c>null</c>
		/// without having any other effect.
		/// </summary>
		(ILInstruction, BitSet) DoLift(ILInstruction inst)
		{
			if (MatchGetValueOrDefault(inst, out ILVariable inputVar)) {
				// n.GetValueOrDefault() lifted => n.
				BitSet foundIndices = new BitSet(nullableVars.Count);
				for (int i = 0; i < nullableVars.Count; i++) {
					if (nullableVars[i] == inputVar) {
						foundIndices[i] = true;
					}
				}
				if (foundIndices.Any())
					return (new LdLoc(inputVar).WithILRange(inst), foundIndices);
				else
					return (null, null);
			} else if (inst is Conv conv) {
				var (arg, bits) = DoLift(conv.Argument);
				if (arg != null) {
					if (conv.HasDirectFlag(InstructionFlags.MayThrow) && !bits.All(0, nullableVars.Count)) {
						// Cannot execute potentially-throwing instruction unless all
						// the nullableVars are arguments to the instruction
						// (thus causing it not to throw when any of them is null).
						return (null, null);
					}
					var newInst = new Conv(arg, conv.InputType, conv.InputSign, conv.TargetType, conv.CheckForOverflow, isLifted: true).WithILRange(conv);
					return (newInst, bits);
				}
			} else if (inst is BitNot bitnot) {
				var (arg, bits) = DoLift(bitnot.Argument);
				if (arg != null) {
					var newInst = new BitNot(arg, isLifted: true, stackType: bitnot.ResultType).WithILRange(bitnot);
					return (newInst, bits);
				}
			} else if (inst is BinaryNumericInstruction binary) {
				var (left, right, bits) = DoLiftBinary(binary.Left, binary.Right, SpecialType.UnknownType, SpecialType.UnknownType);
				if (left != null && right != null) {
					if (binary.HasDirectFlag(InstructionFlags.MayThrow) && !bits.All(0, nullableVars.Count)) {
						// Cannot execute potentially-throwing instruction unless all
						// the nullableVars are arguments to the instruction
						// (thus causing it not to throw when any of them is null).
						return (null, null);
					}
					var newInst = new BinaryNumericInstruction(
						binary.Operator, left, right,
						binary.LeftInputType, binary.RightInputType,
						binary.CheckForOverflow, binary.Sign,
						isLifted: true
					).WithILRange(binary);
					return (newInst, bits);
				}
			} else if (inst is Comp comp && !comp.IsLifted && comp.Kind == ComparisonKind.Equality
				&& MatchGetValueOrDefault(comp.Left, out ILVariable v) && nullableVars.Contains(v)
				&& NullableType.GetUnderlyingType(v.Type).IsKnownType(KnownTypeCode.Boolean)
				&& comp.Right.MatchLdcI4(0)
			) {
				// C# doesn't support ComparisonLiftingKind.ThreeValuedLogic,
				// except for operator! on bool?.
				var (arg, bits) = DoLift(comp.Left);
				Debug.Assert(arg != null);
				var newInst = new Comp(comp.Kind, ComparisonLiftingKind.ThreeValuedLogic, comp.InputType, comp.Sign, arg, comp.Right.Clone()).WithILRange(comp);
				return (newInst, bits);
			} else if (inst is Call call && call.Method.IsOperator) {
				// Lifted user-defined operators, except for comparison operators (as those return bool, not bool?)
				var liftedOperator = CSharp.Resolver.CSharpOperators.LiftUserDefinedOperator(call.Method);
				if (liftedOperator == null || !NullableType.IsNullable(liftedOperator.ReturnType))
					return (null, null);
				ILInstruction[] newArgs;
				BitSet newBits;
				if (call.Arguments.Count == 1) {
					var (arg, bits) = DoLift(call.Arguments[0]);
					newArgs = new[] { arg };
					newBits = bits;
				} else if (call.Arguments.Count == 2) {
					var (left, right, bits) = DoLiftBinary(call.Arguments[0], call.Arguments[1],
						call.Method.Parameters[0].Type, call.Method.Parameters[1].Type);
					newArgs = new[] { left, right };
					newBits = bits;
				} else {
					return (null, null);
				}
				if (newBits == null || !newBits.All(0, nullableVars.Count)) {
					// all nullable vars must be involved when calling a method (side effect)
					return (null, null);
				}
				var newInst = new Call(liftedOperator) {
					ConstrainedTo = call.ConstrainedTo,
					IsTail = call.IsTail,
					ILStackWasEmpty = call.ILStackWasEmpty,
				}.WithILRange(call);
				newInst.Arguments.AddRange(newArgs);
				return (newInst, newBits);
			}
			return (null, null);
		}

		(ILInstruction, ILInstruction, BitSet) DoLiftBinary(ILInstruction lhs, ILInstruction rhs, IType leftExpectedType, IType rightExpectedType)
		{
			var (left, leftBits) = DoLift(lhs);
			var (right, rightBits) = DoLift(rhs);
			if (left != null && right == null && SemanticHelper.IsPure(rhs.Flags)) {
				// Embed non-nullable pure expression in lifted expression.
				right = NewNullable(rhs.Clone(), rightExpectedType);
			}
			if (left == null && right != null && SemanticHelper.IsPure(lhs.Flags)) {
				// Embed non-nullable pure expression in lifted expression.
				left = NewNullable(lhs.Clone(), leftExpectedType);
			}
			if (left != null && right != null) {
				var bits = leftBits ?? rightBits;
				if (rightBits != null)
					bits.UnionWith(rightBits);
				return (left, right, bits);
			} else {
				return (null, null, null);
			}
		}

		private ILInstruction NewNullable(ILInstruction inst, IType underlyingType)
		{
			if (underlyingType == SpecialType.UnknownType)
				return inst;
			var nullable = context.TypeSystem.FindType(KnownTypeCode.NullableOfT).GetDefinition();
			var ctor = nullable?.Methods.FirstOrDefault(m => m.IsConstructor && m.Parameters.Count == 1);
			if (ctor != null) {
				ctor = ctor.Specialize(new TypeParameterSubstitution(new[] { underlyingType }, null));
				return new NewObj(ctor) { Arguments = { inst } };
			} else {
				return inst;
			}
		}
		#endregion

		#region Match...Call
		/// <summary>
		/// Matches 'call get_HasValue(arg)'
		/// </summary>
		internal static bool MatchHasValueCall(ILInstruction inst, out ILInstruction arg)
		{
			arg = null;
			if (!(inst is Call call))
				return false;
			if (call.Arguments.Count != 1)
				return false;
			if (call.Method.Name != "get_HasValue")
				return false;
			if (call.Method.DeclaringTypeDefinition?.KnownTypeCode != KnownTypeCode.NullableOfT)
				return false;
			arg = call.Arguments[0];
			return true;
		}

		/// <summary>
		/// Matches 'call get_HasValue(ldloca v)'
		/// </summary>
		internal static bool MatchHasValueCall(ILInstruction inst, out ILVariable v)
		{
			if (MatchHasValueCall(inst, out ILInstruction arg)) {
				return arg.MatchLdLoca(out v);
			}
			v = null;
			return false;
		}

		/// <summary>
		/// Matches 'call get_HasValue(ldloca v)'
		/// </summary>
		internal static bool MatchHasValueCall(ILInstruction inst, ILVariable v)
		{
			return MatchHasValueCall(inst, out ILVariable v2) && v == v2;
		}

		/// <summary>
		/// Matches 'logic.not(call get_HasValue(ldloca v))'
		/// </summary>
		static bool MatchNegatedHasValueCall(ILInstruction inst, ILVariable v)
		{
			return inst.MatchLogicNot(out var arg) && MatchHasValueCall(arg, v);
		}

		/// <summary>
		/// Matches 'newobj Nullable{underlyingType}.ctor(arg)'
		/// </summary>
		internal static bool MatchNullableCtor(ILInstruction inst, out IType underlyingType, out ILInstruction arg)
		{
			underlyingType = null;
			arg = null;
			if (!(inst is NewObj newobj))
				return false;
			if (!newobj.Method.IsConstructor || newobj.Arguments.Count != 1)
				return false;
			if (newobj.Method.DeclaringTypeDefinition?.KnownTypeCode != KnownTypeCode.NullableOfT)
				return false;
			arg = newobj.Arguments[0];
			underlyingType = NullableType.GetUnderlyingType(newobj.Method.DeclaringType);
			return true;
		}

		/// <summary>
		/// Matches 'call Nullable{T}.GetValueOrDefault(arg)'
		/// </summary>
		internal static bool MatchGetValueOrDefault(ILInstruction inst, out ILInstruction arg)
		{
			arg = null;
			if (!(inst is Call call))
				return false;
			if (call.Method.Name != "GetValueOrDefault" || call.Arguments.Count != 1)
				return false;
			if (call.Method.DeclaringTypeDefinition?.KnownTypeCode != KnownTypeCode.NullableOfT)
				return false;
			arg = call.Arguments[0];
			return true;
		}

		/// <summary>
		/// Matches 'call Nullable{T}.GetValueOrDefault(ldloca v)'
		/// </summary>
		internal static bool MatchGetValueOrDefault(ILInstruction inst, out ILVariable v)
		{
			v = null;
			return MatchGetValueOrDefault(inst, out ILInstruction arg)
				&& arg.MatchLdLoca(out v);
		}

		/// <summary>
		/// Matches 'call Nullable{T}.GetValueOrDefault(ldloca v)'
		/// </summary>
		internal static bool MatchGetValueOrDefault(ILInstruction inst, ILVariable v)
		{
			return MatchGetValueOrDefault(inst, out ILVariable v2) && v == v2;
		}

		static bool MatchNull(ILInstruction inst, out IType underlyingType)
		{
			underlyingType = null;
			if (inst.MatchDefaultValue(out IType type)) {
				underlyingType = NullableType.GetUnderlyingType(type);
				return NullableType.IsNullable(type);
			}
			underlyingType = null;
			return false;
		}

		static bool MatchNull(ILInstruction inst, IType underlyingType)
		{
			return MatchNull(inst, out var utype) && utype.Equals(underlyingType);
		}
		#endregion
	}

	class NullableLiftingStatementTransform : IStatementTransform
	{
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			new NullableLiftingTransform(context).RunStatements(block, pos);
		}
	}
}
