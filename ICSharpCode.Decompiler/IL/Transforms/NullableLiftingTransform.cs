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
	///  * the ?? operator with type Nullable{T} on the left-hand-side
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
			if (!context.Settings.LiftNullables)
				return false;
			// Detect pattern:
			// if (condition)
			//      newobj Nullable<utype>..ctor(exprToLift)
			// else
			//      default.value System.Nullable<utype>
			if (!AnalyzeTopLevelCondition(ifInst.Condition, out bool negativeCondition))
				return false;
			ILInstruction trueInst = negativeCondition ? ifInst.FalseInst : ifInst.TrueInst;
			ILInstruction falseInst = negativeCondition ? ifInst.TrueInst : ifInst.FalseInst;
			var lifted = Lift(trueInst, falseInst, ifInst.ILRange);
			if (lifted != null) {
				ifInst.ReplaceWith(lifted);
				return true;
			}
			return false;
		}

		public bool RunBlock(Block block)
		{
			if (!context.Settings.LiftNullables)
				return false;
			//  if (!condition) Block {
			//    leave IL_0000 (default.value System.Nullable`1[[System.Int64]])
			//  }
			//  leave IL_0000 (newobj .ctor(exprToLift))
			IfInstruction ifInst;
			if (block.Instructions.Last() is Leave elseLeave) {
				ifInst = block.Instructions.SecondToLastOrDefault() as IfInstruction;
				if (ifInst == null || !ifInst.FalseInst.MatchNop())
					return false;
			} else {
				return false;
			}
			if (!(Block.Unwrap(ifInst.TrueInst) is Leave thenLeave))
				return false;
			if (elseLeave.TargetContainer != thenLeave.TargetContainer)
				return false;
			if (!AnalyzeTopLevelCondition(ifInst.Condition, out bool negativeCondition))
				return false;
			ILInstruction trueInst = negativeCondition ? elseLeave.Value : thenLeave.Value;
			ILInstruction falseInst = negativeCondition ? thenLeave.Value : elseLeave.Value;
			var lifted = Lift(trueInst, falseInst, ifInst.ILRange);
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
		bool AnalyzeTopLevelCondition(ILInstruction condition, out bool negativeCondition)
		{
			negativeCondition = false;
			while (condition.MatchLogicNot(out var arg)) {
				condition = arg;
				negativeCondition = !negativeCondition;
			}
			return AnalyzeCondition(condition);
		}

		bool AnalyzeCondition(ILInstruction condition)
		{
			if (MatchHasValueCall(condition, out var v)) {
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

		#region DoLift
		ILInstruction Lift(ILInstruction trueInst, ILInstruction falseInst, Interval ilrange)
		{
			bool isNullCoalescingWithNonNullableFallback = false;
			if (!MatchNullableCtor(trueInst, out var utype, out var exprToLift)) {
				isNullCoalescingWithNonNullableFallback = true;
				utype = context.TypeSystem.Compilation.FindType(trueInst.ResultType.ToKnownTypeCode());
				exprToLift = trueInst;
				if (nullableVars.Count == 1 && exprToLift.MatchLdLoc(nullableVars[0])) {
					// v.HasValue ? ldloc v : fallback
					// => v ?? fallback
					context.Step("v.HasValue ? v : fallback => v ?? fallback", trueInst);
					return new NullCoalescingInstruction(NullCoalescingKind.Nullable, trueInst, falseInst) {
						UnderlyingResultType = NullableType.GetUnderlyingType(nullableVars[0].Type).GetStackType(),
						ILRange = ilrange
					};
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
					) {
						ILRange = ilrange
					};
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
					UnderlyingResultType = exprToLift.ResultType,
					ILRange = ilrange
				};
			} else if (!MatchNull(falseInst, utype)) {
				// Normal lifting, but the falseInst isn't `default(utype?)`
				// => use the `??` operator to provide the fallback value.
				lifted = new NullCoalescingInstruction(NullCoalescingKind.Nullable, lifted, falseInst) {
					UnderlyingResultType = exprToLift.ResultType,
					ILRange = ilrange
				};
			}
			return lifted;
		}

		// Lifts the specified instruction.
		// Creates a new lifted instruction without modifying the input instruction.
		//   Returns (new lifted instruction, bitset of nullableVars that will cause the expression to evaluate to null).
		// If lifting fails, returns (null, null).
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
					return (new LdLoc(inputVar) { ILRange = inst.ILRange }, foundIndices);
				else
					return (null, null);
			} else if (inst is Conv conv) {
				var (arg, bits) = DoLift(conv.Argument);
				if (arg != null) {
					if (conv.HasFlag(InstructionFlags.MayThrow) && !bits.All(0, nullableVars.Count)) {
						// Cannot execute potentially-throwing instruction unless all
						// the nullableVars are arguments to the instruction
						// (thus causing it not to throw when any of them is null).
						return (null, null);
					}
					var newInst = new Conv(arg, conv.InputType, conv.InputSign, conv.TargetType, conv.CheckForOverflow, isLifted: true) {
						ILRange = conv.ILRange
					};
					return (newInst, bits);
				}
			} else if (inst is BinaryNumericInstruction binary) {
				var (left, leftBits) = DoLift(binary.Left);
				var (right, rightBits) = DoLift(binary.Right);
				if (left != null && right == null && SemanticHelper.IsPure(binary.Right.Flags)) {
					// Embed non-nullable pure expression in lifted expression.
					right = binary.Right.Clone();
				}
				if (left == null && right != null && SemanticHelper.IsPure(binary.Left.Flags)) {
					// Embed non-nullable pure expression in lifted expression.
					left = binary.Left.Clone();
				}
				if (left != null && right != null) {
					var bits = leftBits ?? rightBits;
					if (rightBits != null)
						bits.UnionWith(rightBits);
					if (binary.HasFlag(InstructionFlags.MayThrow) && !bits.All(0, nullableVars.Count)) {
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
					) {
						ILRange = binary.ILRange
					};
					return (newInst, bits);
				}
			}
			return (null, null);
		}
		#endregion

		#region Match...Call
		/// <summary>
		/// Matches 'call get_HasValue(ldloca v)'
		/// </summary>
		static bool MatchHasValueCall(ILInstruction inst, out ILVariable v)
		{
			v = null;
			if (!(inst is Call call))
				return false;
			if (call.Arguments.Count != 1)
				return false;
			if (call.Method.Name != "get_HasValue")
				return false;
			if (call.Method.DeclaringTypeDefinition?.KnownTypeCode != KnownTypeCode.NullableOfT)
				return false;
			return call.Arguments[0].MatchLdLoca(out v);
		}

		/// <summary>
		/// Matches 'newobj Nullable{underlyingType}.ctor(arg)'
		/// </summary>
		static bool MatchNullableCtor(ILInstruction inst, out IType underlyingType, out ILInstruction arg)
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
		static bool MatchGetValueOrDefault(ILInstruction inst, out ILInstruction arg)
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
		static bool MatchGetValueOrDefault(ILInstruction inst, out ILVariable v)
		{
			v = null;
			return MatchGetValueOrDefault(inst, out ILInstruction arg)
				&& arg.MatchLdLoca(out v);
		}

		/// <summary>
		/// Matches 'call Nullable{T}.GetValueOrDefault(ldloca v)'
		/// </summary>
		static bool MatchGetValueOrDefault(ILInstruction inst, ILVariable v)
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

	class NullableLiftingBlockTransform : IBlockTransform
	{
		public void Run(Block block, BlockTransformContext context)
		{
			new NullableLiftingTransform(context).RunBlock(block);
		}
	}
}
