﻿// Copyright (c) 2018 Daniel Grunwald
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transform that converts code patterns like "v != null ? v.M() : null" to "v?.M()"
	/// </summary>
	readonly struct NullPropagationTransform
	{
		internal static bool IsProtectedIfInst(IfInstruction ifInst)
		{
			// We exclude logic.and to avoid turning
			// "logic.and(comp(interfaces != ldnull), call get_Count(interfaces))"
			// into "if ((interfaces?.Count ?? 0) != 0)".
			return ifInst != null
				&& (ifInst.MatchLogicAnd(out _, out _) || ifInst.MatchLogicOr(out _, out _))
				&& IfInstruction.IsInConditionSlot(ifInst);
		}

		readonly ILTransformContext context;

		public NullPropagationTransform(ILTransformContext context)
		{
			this.context = context;
		}

		enum Mode
		{
			/// <summary>
			/// reference type or generic type (comparison is 'comp(ldloc(testedVar) == null)')
			/// </summary>
			ReferenceType,
			/// <summary>
			/// nullable type, used by value (comparison is 'call get_HasValue(ldloca(testedVar))')
			/// </summary>
			NullableByValue,
			/// <summary>
			/// nullable type, used by reference (comparison is 'call get_HasValue(ldloc(testedVar))')
			/// </summary>
			NullableByReference,
			/// <summary>
			/// unconstrained generic type (see the pattern described in TransformNullPropagationOnUnconstrainedGenericExpression)
			/// </summary>
			UnconstrainedType,
		}

		/// <summary>
		/// Check if "condition ? trueInst : falseInst" can be simplified using the null-conditional operator.
		/// Returns the replacement instruction, or null if no replacement is possible.
		/// </summary>
		internal ILInstruction? Run(ILInstruction condition, ILInstruction trueInst, ILInstruction falseInst)
		{
			Debug.Assert(context.Settings.NullPropagation);
			Debug.Assert(!condition.MatchLogicNot(out _), "Caller should pass in positive condition");
			if (condition is Comp comp && comp.Left.MatchLdLoc(out var testedVar) && comp.Right.MatchLdNull())
			{
				if (comp.LiftingKind != ComparisonLiftingKind.None)
					return null;
				if (comp.Kind == ComparisonKind.Equality)
				{
					// testedVar == null ? trueInst : falseInst
					return TryNullPropagation(testedVar, falseInst, trueInst, Mode.ReferenceType);
				}
				else if (comp.Kind == ComparisonKind.Inequality)
				{
					return TryNullPropagation(testedVar, trueInst, falseInst, Mode.ReferenceType);
				}
			}
			else if (NullableLiftingTransform.MatchHasValueCall(condition, out ILInstruction loadInst))
			{
				// loadInst.HasValue ? trueInst : falseInst
				if (loadInst.MatchLdLoca(out testedVar))
				{
					return TryNullPropagation(testedVar, trueInst, falseInst, Mode.NullableByValue);
				}
				else if (loadInst.MatchLdLoc(out testedVar))
				{
					return TryNullPropagation(testedVar, trueInst, falseInst, Mode.NullableByReference);
				}
			}
			return null;
		}

		/// <summary>
		/// testedVar != null ? nonNullInst : nullInst
		/// </summary>
		ILInstruction? TryNullPropagation(ILVariable testedVar, ILInstruction nonNullInst, ILInstruction nullInst,
			Mode mode)
		{
			bool removedRewrapOrNullableCtor = false;
			if (NullableLiftingTransform.MatchNullableCtor(nonNullInst, out _, out var arg))
			{
				nonNullInst = arg;
				removedRewrapOrNullableCtor = true;
			}
			else if (nonNullInst.MatchNullableRewrap(out arg))
			{
				nonNullInst = arg;
				removedRewrapOrNullableCtor = true;
			}
			if (!IsValidAccessChain(testedVar, mode, nonNullInst, out var varLoad))
				return null;
			// note: InferType will be accurate in this case because the access chain consists of calls and field accesses
			IType returnType = nonNullInst.InferType(context.TypeSystem);
			if (nullInst.MatchLdNull())
			{
				context.Step($"Null propagation (mode={mode}, output=reference type)", nonNullInst);
				// testedVar != null ? testedVar.AccessChain : null
				// => testedVar?.AccessChain
				IntroduceUnwrap(testedVar, varLoad, mode);
				return new NullableRewrap(nonNullInst);
			}
			else if (nullInst.MatchDefaultValue(out var type) && type.IsKnownType(KnownTypeCode.NullableOfT))
			{
				context.Step($"Null propagation (mode={mode}, output=value type)", nonNullInst);
				// testedVar != null ? testedVar.AccessChain : default(T?)
				// => testedVar?.AccessChain
				IntroduceUnwrap(testedVar, varLoad, mode);
				return new NullableRewrap(nonNullInst);
			}
			else if (!removedRewrapOrNullableCtor && NullableType.IsNonNullableValueType(returnType))
			{
				context.Step($"Null propagation (mode={mode}, output=null coalescing)", nonNullInst);
				// testedVar != null ? testedVar.AccessChain : nullInst
				// => testedVar?.AccessChain ?? nullInst
				// (only valid if AccessChain returns a non-nullable value)
				IntroduceUnwrap(testedVar, varLoad, mode);
				return new NullCoalescingInstruction(
					NullCoalescingKind.NullableWithValueFallback,
					new NullableRewrap(nonNullInst),
					nullInst
				) {
					UnderlyingResultType = nullInst.ResultType
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
			if (block.Instructions[pos] is IfInstruction ifInst && ifInst.FalseInst.MatchNop())
			{
				if (ifInst.Condition is Comp comp && comp.Kind == ComparisonKind.Inequality
					&& comp.Left.MatchLdLoc(out var testedVar) && comp.Right.MatchLdNull())
				{
					TryNullPropForVoidCall(testedVar, Mode.ReferenceType, ifInst.TrueInst as Block, ifInst);
				}
				else if (NullableLiftingTransform.MatchHasValueCall(ifInst.Condition, out ILInstruction arg))
				{
					if (arg.MatchLdLoca(out testedVar))
					{
						TryNullPropForVoidCall(testedVar, Mode.NullableByValue, ifInst.TrueInst as Block, ifInst);
					}
					else if (arg.MatchLdLoc(out testedVar))
					{
						TryNullPropForVoidCall(testedVar, Mode.NullableByReference, ifInst.TrueInst as Block, ifInst);
					}
				}
			}
			if (TransformNullPropagationOnUnconstrainedGenericExpression(block, pos, out var testedVariable, out var nonNullInst, out var nullInst, out var endBlock))
			{
				var parentInstruction = nonNullInst.Parent;
				var replacement = TryNullPropagation(testedVariable, nonNullInst, nullInst, Mode.UnconstrainedType);
				if (replacement == null)
					return;
				context.Step("TransformNullPropagationOnUnconstrainedGenericExpression", block);
				switch (parentInstruction)
				{
					case StLoc stloc:
						stloc.Value = replacement;
						break;
					case Leave leave:
						leave.Value = replacement;
						break;
					default:
						// if this ever happens, the pattern checked by TransformNullPropagationOnUnconstrainedGenericExpression
						// has changed, but this part of the code was not properly adjusted.
						throw new NotSupportedException();
				}
				// Remove the fallback conditions and blocks
				block.Instructions.RemoveRange(pos + 1, 2);
				// if the endBlock is only reachable through the current block,
				// combine both blocks.
				if (endBlock?.IncomingEdgeCount == 1)
				{
					block.Instructions.AddRange(endBlock.Instructions);
					block.Instructions.RemoveAt(pos + 2);
					endBlock.Remove();
				}
				ILInlining.InlineIfPossible(block, pos, context);
			}
		}

		void TryNullPropForVoidCall(ILVariable testedVar, Mode mode, Block body, IfInstruction ifInst)
		{
			if (body == null || body.Instructions.Count != 1)
				return;
			var bodyInst = body.Instructions[0];
			if (bodyInst.MatchNullableRewrap(out var arg))
			{
				bodyInst = arg;
			}
			if (!IsValidAccessChain(testedVar, mode, bodyInst, out var varLoad))
				return;
			context.Step($"Null-propagation (mode={mode}, output=void call)", body);
			// if (testedVar != null) { testedVar.AccessChain(); }
			// => testedVar?.AccessChain();
			IntroduceUnwrap(testedVar, varLoad, mode);
			ifInst.ReplaceWith(new NullableRewrap(
				bodyInst
			).WithILRange(ifInst));
		}

		bool IsValidAccessChain(ILVariable testedVar, Mode mode, ILInstruction inst, [NotNullWhen(true)] out ILInstruction? finalLoad)
		{
			finalLoad = null;
			int chainLength = 0;
			while (true)
			{
				if (IsValidEndOfChain())
				{
					// valid end of chain
					finalLoad = inst;
					return chainLength >= 1;
				}
				else if (inst.MatchLdFld(out var target, out _))
				{
					inst = target;
				}
				else if (inst.MatchLdFlda(out target, out var f))
				{
					if (target is AddressOf addressOf && f.DeclaringType.Kind == TypeKind.Struct)
					{
						inst = addressOf.Value;
					}
					else
					{
						inst = target;
					}
				}
				else if (inst is CallInstruction call && call.OpCode != OpCode.NewObj)
				{
					if (call.Arguments.Count == 0)
					{
						return false;
					}
					if (call.Method.IsStatic && (!call.Method.IsExtensionMethod || !CanTransformToExtensionMethodCall(call, context)))
					{
						return false; // only instance or extension methods can be called with ?. syntax
					}
					if (call.Method.IsAccessor && !IsGetter(call.Method))
					{
						return false; // setter/adder/remover cannot be called with ?. syntax
					}
					inst = call.Arguments[0];
					if ((call.ConstrainedTo ?? call.Method.DeclaringType).IsReferenceType == false && inst.MatchAddressOf(out var arg, out _))
					{
						inst = arg;
					}
					// ensure the access chain does not contain any 'nullable.unwrap' that aren't directly part of the chain
					if (ArgumentsAfterFirstMayUnwrapNull(call.Arguments))
						return false;
				}
				else if (inst is LdLen ldLen)
				{
					inst = ldLen.Array;
				}
				else if (inst is LdElema ldElema)
				{
					inst = ldElema.Array;
					// ensure the access chain does not contain any 'nullable.unwrap' that aren't directly part of the chain
					if (ldElema.Indices.Any(i => i.HasFlag(InstructionFlags.MayUnwrapNull)))
						return false;
				}
				else if (inst is NullableUnwrap unwrap)
				{
					inst = unwrap.Argument;
					if (unwrap.RefInput && inst is AddressOf addressOf)
					{
						inst = addressOf.Value;
					}
					// The argument of the unwrap instruction cannot be the end of the chain, because this
					// would result in two null-conditional operators immediately following each other,
					// which is not valid in C#.
					if (IsValidEndOfChain())
						return false;
				}
				else if (inst is DynamicGetMemberInstruction dynGetMember)
				{
					inst = dynGetMember.Target;
				}
				else if (inst is DynamicInvokeMemberInstruction dynInvokeMember)
				{
					inst = dynInvokeMember.Arguments[0];
					if (ArgumentsAfterFirstMayUnwrapNull(dynInvokeMember.Arguments))
						return false;
				}
				else if (inst is DynamicGetIndexInstruction dynGetIndex)
				{
					inst = dynGetIndex.Arguments[0];
					if (ArgumentsAfterFirstMayUnwrapNull(dynGetIndex.Arguments))
						return false;
				}
				else
				{
					// unknown node -> invalid chain
					return false;
				}
				chainLength++;
			}

			bool ArgumentsAfterFirstMayUnwrapNull(InstructionCollection<ILInstruction> arguments)
			{
				// ensure the access chain does not contain any 'nullable.unwrap' that aren't directly part of the chain
				for (int i = 1; i < arguments.Count; ++i)
				{
					if (arguments[i].HasFlag(InstructionFlags.MayUnwrapNull))
					{
						return true;
					}
				}
				return false;
			}

			bool IsValidEndOfChain()
			{
				switch (mode)
				{
					case Mode.ReferenceType:
						// either reference type (expect: ldloc(testedVar)) or unconstrained generic type (expect: ldloca(testedVar)).
						return inst.MatchLdLocRef(testedVar);
					case Mode.NullableByValue:
						return NullableLiftingTransform.MatchGetValueOrDefault(inst, testedVar);
					case Mode.NullableByReference:
						return NullableLiftingTransform.MatchGetValueOrDefault(inst, out ILInstruction arg)
							&& arg.MatchLdLoc(testedVar);
					case Mode.UnconstrainedType:
						// unconstrained generic type (expect: ldloc(testedVar))
						return inst.MatchLdLoc(testedVar);
					default:
						throw new ArgumentOutOfRangeException(nameof(mode));
				}
			}

			bool CanTransformToExtensionMethodCall(CallInstruction call, ILTransformContext context)
			{
				return CSharp.Transforms.IntroduceExtensionMethods.CanTransformToExtensionMethodCall(
					call.Method, new CSharp.TypeSystem.CSharpTypeResolveContext(
						context.TypeSystem.MainModule, context.UsingScope
					)
				);
			}
		}

		static bool IsGetter(IMethod method)
		{
			return method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter;
		}

		private void IntroduceUnwrap(ILVariable testedVar, ILInstruction varLoad, Mode mode)
		{
			var oldParentChildren = varLoad.Parent.Children;
			var oldChildIndex = varLoad.ChildIndex;
			ILInstruction replacement;
			switch (mode)
			{
				case Mode.ReferenceType:
				case Mode.UnconstrainedType:
					// Wrap varLoad in nullable.unwrap:
					replacement = new NullableUnwrap(varLoad.ResultType, varLoad, refInput: varLoad.ResultType == StackType.Ref);
					break;
				case Mode.NullableByValue:
					Debug.Assert(NullableLiftingTransform.MatchGetValueOrDefault(varLoad, testedVar));
					replacement = new NullableUnwrap(
						varLoad.ResultType,
						new LdLoc(testedVar).WithILRange(varLoad.Children[0])
					).WithILRange(varLoad);
					break;
				case Mode.NullableByReference:
					replacement = new NullableUnwrap(
						varLoad.ResultType,
						new LdLoc(testedVar).WithILRange(varLoad.Children[0]),
						refInput: true
					).WithILRange(varLoad);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(mode));
			}
			oldParentChildren[oldChildIndex] = replacement;
		}

		// stloc target(targetInst)
		// stloc defaultTemporary(default.value type)
		// if (logic.not(comp.o(box `0(ldloc defaultTemporary) != ldnull))) Block fallbackBlock {
		// 	stloc defaultTemporary(ldobj type(ldloc target))
		// 	stloc target(ldloca defaultTemporary)
		// 	if (comp.o(ldloc defaultTemporary == ldnull)) Block fallbackBlock2 {
		// 		stloc resultTemporary(nullInst)
		// 		br endBlock
		// 	}
		// }
		// stloc resultTemporary(constrained[type].call_instruction(ldloc target, ...))
		// br endBlock
		// =>
		// stloc resultTemporary(nullable.rewrap(constrained[type].call_instruction(nullable.unwrap(targetInst), ...)))
		//
		// -or-
		//
		// stloc target(targetInst)
		// stloc defaultTemporary(default.value type)
		// if (logic.not(comp.o(box `0(ldloc defaultTemporary) != ldnull))) Block fallbackBlock {
		// 	stloc defaultTemporary(ldobj type(ldloc target))
		// 	stloc target(ldloca defaultTemporary)
		// 	if (comp.o(ldloc defaultTemporary == ldnull)) Block fallbackBlock2 {
		// 		leave(nullInst)
		// 	}
		// }
		// leave (constrained[type].call_instruction(ldloc target, ...))
		// =>
		// leave (nullable.rewrap(constrained[type].call_instruction(nullable.unwrap(targetInst), ...)))
		private bool TransformNullPropagationOnUnconstrainedGenericExpression(Block block, int pos,
			out ILVariable? target, out ILInstruction? nonNullInst, out ILInstruction? nullInst, out Block? endBlock)
		{
			target = null;
			nonNullInst = null;
			nullInst = null;
			endBlock = null;
			if (pos + 3 >= block.Instructions.Count)
				return false;
			// stloc target(...)
			if (!block.Instructions[pos].MatchStLoc(out target, out _))
				return false;
			if (!(target.Kind == VariableKind.StackSlot && target.LoadCount == 2 && target.StoreCount == 2))
				return false;
			// stloc defaultTemporary(default.value type)
			if (!(block.Instructions[pos + 1].MatchStLoc(out var defaultTemporary, out var defaultExpression) && defaultExpression.MatchDefaultValue(out var type)))
				return false;
			// In the above pattern the defaultTemporary variable is used two times in stloc and ldloc instructions and once in a ldloca instruction
			if (!(defaultTemporary.Kind == VariableKind.Local && defaultTemporary.LoadCount == 2 && defaultTemporary.StoreCount == 2 && defaultTemporary.AddressCount == 1))
				return false;
			// if (logic.not(comp.o(box `0(ldloc defaultTemporary) != ldnull))) Block fallbackBlock
			if (!(block.Instructions[pos + 2].MatchIfInstruction(out var condition, out var fallbackBlock1) && condition.MatchCompEqualsNull(out var arg) && arg.MatchLdLoc(defaultTemporary)))
				return false;
			if (!MatchStLocResultTemporary(block, pos, type, target, defaultTemporary, fallbackBlock1, out nonNullInst, out nullInst, out endBlock)
				&& !MatchLeaveResult(block, pos, type, target, defaultTemporary, fallbackBlock1, out nonNullInst, out nullInst))
				return false;
			return true;
		}

		// stloc resultTemporary(constrained[type].call_instruction(ldloc target, ...))
		// br endBlock
		private bool MatchStLocResultTemporary(Block block, int pos, IType type, ILVariable target, ILVariable defaultTemporary, ILInstruction fallbackBlock, [NotNullWhen(true)] out ILInstruction? nonNullInst, [NotNullWhen(true)] out ILInstruction? nullInst, [NotNullWhen(true)] out Block? endBlock)
		{
			endBlock = null;
			nonNullInst = null;
			nullInst = null;

			if (pos + 4 >= block.Instructions.Count)
				return false;
			// stloc resultTemporary(constrained[type].call_instruction(ldloc target, ...))
			if (!(block.Instructions[pos + 3].MatchStLoc(out var resultTemporary, out nonNullInst)))
				return false;
			// br endBlock
			if (!(block.Instructions[pos + 4].MatchBranch(out endBlock)))
				return false;
			// Analyze Block fallbackBlock
			if (!(fallbackBlock is Block b && IsFallbackBlock(b, type, target, defaultTemporary, resultTemporary, endBlock, out nullInst)))
				return false;

			return true;
		}

		private bool MatchLeaveResult(Block block, int pos, IType type, ILVariable target, ILVariable defaultTemporary, ILInstruction fallbackBlock, [NotNullWhen(true)] out ILInstruction? nonNullInst, [NotNullWhen(true)] out ILInstruction? nullInst)
		{
			nonNullInst = null;
			nullInst = null;

			// leave (constrained[type].call_instruction(ldloc target, ...))
			if (!(block.Instructions[pos + 3] is Leave leave && leave.IsLeavingFunction))
				return false;
			nonNullInst = leave.Value;
			// Analyze Block fallbackBlock
			if (!(fallbackBlock is Block b && IsFallbackBlock(b, type, target, defaultTemporary, null, leave.TargetContainer, out nullInst)))
				return false;

			return true;
		}

		// Block fallbackBlock {
		// 	stloc defaultTemporary(ldobj type(ldloc target))
		// 	stloc target(ldloca defaultTemporary)
		// 	if (comp.o(ldloc defaultTemporary == ldnull)) Block fallbackBlock {
		// 		stloc resultTemporary(ldnull)
		// 		br endBlock
		// 	}
		// }
		private bool IsFallbackBlock(Block block, IType type, ILVariable target, ILVariable defaultTemporary, ILVariable? resultTemporary, ILInstruction endBlockOrLeaveContainer, [NotNullWhen(true)] out ILInstruction? nullInst)
		{
			nullInst = null;
			if (!(block.Instructions.Count == 3))
				return false;
			// stloc defaultTemporary(ldobj type(ldloc target))
			if (!(block.Instructions[0].MatchStLoc(defaultTemporary, out var value)))
				return false;
			if (!(value.MatchLdObj(out var inst, out var t) && type.Equals(t) && inst.MatchLdLoc(target)))
				return false;
			// stloc target(ldloca defaultTemporary)
			if (!(block.Instructions[1].MatchStLoc(target, out var defaultAddress) && defaultAddress.MatchLdLoca(defaultTemporary)))
				return false;
			// if (comp.o(ldloc defaultTemporary == ldnull)) Block fallbackBlock
			if (!(block.Instructions[2].MatchIfInstruction(out var condition, out var tmp) && condition.MatchCompEqualsNull(out var arg) && arg.MatchLdLoc(defaultTemporary)))
				return false;
			// Block fallbackBlock {
			//   stloc resultTemporary(nullInst)
			//   br endBlock
			// }
			var fallbackInst = Block.Unwrap(tmp);
			if (fallbackInst is Block fallbackBlock && endBlockOrLeaveContainer is Block endBlock)
			{
				if (!(fallbackBlock.Instructions.Count == 2))
					return false;
				if (!fallbackBlock.Instructions[0].MatchStLoc(resultTemporary, out nullInst))
					return false;
				if (!fallbackBlock.Instructions[1].MatchBranch(endBlock))
					return false;
			}
			else
			{
				if (!(fallbackInst is Leave fallbackLeave && endBlockOrLeaveContainer is BlockContainer leaveContainer
					&& fallbackLeave.TargetContainer == leaveContainer && !fallbackLeave.Value.MatchNop()))
					return false;
				nullInst = fallbackLeave.Value;
			}
			return true;
		}
	}

	public class NullPropagationStatementTransform : IStatementTransform
	{
		public void Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.NullPropagation)
				return;
			new NullPropagationTransform(context).RunStatements(block, pos);
		}
	}
}
