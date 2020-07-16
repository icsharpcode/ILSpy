// Copyright (c) 2015 Siegfried Pammer
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
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Constructs compound assignments and inline assignments.
	/// </summary>
	/// <remarks>
	/// This is a statement transform;
	/// but some portions are executed as an expression transform instead
	/// (with HandleCompoundAssign() as entry point)
	/// </remarks>
	public class TransformAssignment : IStatementTransform
	{
		StatementTransformContext context;

		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			this.context = context;
			if (context.Settings.MakeAssignmentExpressions) {
				if (TransformInlineAssignmentStObjOrCall(block, pos) || TransformInlineAssignmentLocal(block, pos)) {
					// both inline assignments create a top-level stloc which might affect inlining
					context.RequestRerun();
					return;
				} 
			}
			if (context.Settings.IntroduceIncrementAndDecrement) {
				if (TransformPostIncDecOperatorWithInlineStore(block, pos)
					|| TransformPostIncDecOperator(block, pos)) {
					// again, new top-level stloc might need inlining:
					context.RequestRerun();
					return;
				}
			}
		}

		/// <code>
		/// stloc s(value)
		/// stloc l(ldloc s)
		/// stobj(..., ldloc s)
		///   where ... is pure and does not use s or l,
		///   and where neither the 'stloc s' nor the 'stobj' truncates
		/// -->
		/// stloc l(stobj (..., value))
		/// </code>
		/// e.g. used for inline assignment to instance field
		/// 
		/// -or-
		/// 
		/// <code>
		/// stloc s(value)
		/// stobj (..., ldloc s)
		///   where ... is pure and does not use s, and where the 'stobj' does not truncate
		/// -->
		/// stloc s(stobj (..., value))
		/// </code>
		/// e.g. used for inline assignment to static field
		/// 
		/// -or-
		/// 
		/// <code>
		/// stloc s(value)
		/// call set_Property(..., ldloc s)
		///   where the '...' arguments are pure and not using 's'
		/// -->
		/// stloc s(Block InlineAssign { call set_Property(..., stloc i(value)); final: ldloc i })
		///   new temporary 'i' has type of the property; transform only valid if 'stloc i' doesn't truncate
		/// </code>
		bool TransformInlineAssignmentStObjOrCall(Block block, int pos)
		{
			var inst = block.Instructions[pos] as StLoc;
			// in some cases it can be a compiler-generated local
			if (inst == null || (inst.Variable.Kind != VariableKind.StackSlot && inst.Variable.Kind != VariableKind.Local))
				return false;
			if (IsImplicitTruncation(inst.Value, inst.Variable.Type, context.TypeSystem)) {
				// 'stloc s' is implicitly truncating the value
				return false;
			}
			ILVariable local;
			int nextPos;
			if (block.Instructions[pos + 1] is StLoc localStore) { // with extra local
				if (localStore.Variable.Kind != VariableKind.Local || !localStore.Value.MatchLdLoc(inst.Variable))
					return false;
				// if we're using an extra local, we'll delete "s", so check that that doesn't have any additional uses
				if (!(inst.Variable.IsSingleDefinition && inst.Variable.LoadCount == 2))
					return false;
				local = localStore.Variable;
				nextPos = pos + 2;
			} else {
				local = inst.Variable;
				localStore = null;
				nextPos = pos + 1;
			}
			if (block.Instructions[nextPos] is StObj stobj) {
				if (!stobj.Value.MatchLdLoc(inst.Variable))
					return false;
				if (!SemanticHelper.IsPure(stobj.Target.Flags) || inst.Variable.IsUsedWithin(stobj.Target))
					return false;
				var pointerType = stobj.Target.InferType(context.TypeSystem);
				IType newType = stobj.Type;
				if (TypeUtils.IsCompatiblePointerTypeForMemoryAccess(pointerType, stobj.Type)) {
					if (pointerType is ByReferenceType byref)
						newType = byref.ElementType;
					else if (pointerType is PointerType pointer)
						newType = pointer.ElementType;
				}
				if (IsImplicitTruncation(inst.Value, newType, context.TypeSystem)) {
					// 'stobj' is implicitly truncating the value
					return false;
				}
				context.Step("Inline assignment stobj", stobj);
				stobj.Type = newType;
				block.Instructions.Remove(localStore);
				block.Instructions.Remove(stobj);
				stobj.Value = inst.Value;
				inst.ReplaceWith(new StLoc(local, stobj));
				// note: our caller will trigger a re-run, which will call HandleStObjCompoundAssign if applicable
				return true;
			} else if (block.Instructions[nextPos] is CallInstruction call) {
				// call must be a setter call:
				if (!(call.OpCode == OpCode.Call || call.OpCode == OpCode.CallVirt))
					return false;
				if (call.ResultType != StackType.Void || call.Arguments.Count == 0)
					return false;
				IProperty property = call.Method.AccessorOwner as IProperty;
				if (property == null)
					return false;
				if (!call.Method.Equals(property.Setter))
					return false;
				if (!(property.IsIndexer || property.Setter.Parameters.Count == 1)) {
					// this is a parameterized property, which cannot be expressed as C# code.
					// setter calls are not valid in expression context, if property syntax cannot be used.
					return false;
				}
				if (!call.Arguments.Last().MatchLdLoc(inst.Variable))
					return false;
				foreach (var arg in call.Arguments.SkipLast(1)) {
					if (!SemanticHelper.IsPure(arg.Flags) || inst.Variable.IsUsedWithin(arg))
						return false;
				}
				if (IsImplicitTruncation(inst.Value, call.Method.Parameters.Last().Type, context.TypeSystem)) {
					// setter call is implicitly truncating the value
					return false;
				}
				// stloc s(Block InlineAssign { call set_Property(..., stloc i(value)); final: ldloc i })
				context.Step("Inline assignment call", call);
				block.Instructions.Remove(localStore);
				block.Instructions.Remove(call);
				var newVar = context.Function.RegisterVariable(VariableKind.StackSlot, call.Method.Parameters.Last().Type);
				call.Arguments[call.Arguments.Count - 1] = new StLoc(newVar, inst.Value);
				var inlineBlock = new Block(BlockKind.CallInlineAssign) {
					Instructions = { call },
					FinalInstruction = new LdLoc(newVar)
				};
				inst.ReplaceWith(new StLoc(local, inlineBlock));
				// because the ExpressionTransforms don't look into inline blocks, manually trigger HandleCallCompoundAssign
				if (HandleCompoundAssign(call, context)) {
					// if we did construct a compound assignment, it should have made our inline block redundant:
					Debug.Assert(!inlineBlock.IsConnected);
				}
				return true;
			} else {
				return false;
			}
		}

		static ILInstruction UnwrapSmallIntegerConv(ILInstruction inst, out Conv conv)
		{
			conv = inst as Conv;
			if (conv != null && conv.Kind == ConversionKind.Truncate && conv.TargetType.IsSmallIntegerType()) {
				// for compound assignments to small integers, the compiler emits a "conv" instruction
				return conv.Argument;
			} else {
				return inst;
			}
		}

		static bool ValidateCompoundAssign(BinaryNumericInstruction binary, Conv conv, IType targetType, DecompilerSettings settings)
		{
			if (!NumericCompoundAssign.IsBinaryCompatibleWithType(binary, targetType, settings))
				return false;
			if (conv != null && !(conv.TargetType == targetType.ToPrimitiveType() && conv.CheckForOverflow == binary.CheckForOverflow))
				return false; // conv does not match binary operation
			return true;
		}

		static bool MatchingGetterAndSetterCalls(CallInstruction getterCall, CallInstruction setterCall, out Action<ILTransformContext> finalizeMatch)
		{
			finalizeMatch = null;
			if (getterCall == null || setterCall == null || !IsSameMember(getterCall.Method.AccessorOwner, setterCall.Method.AccessorOwner))
				return false;
			if (setterCall.OpCode != getterCall.OpCode)
				return false;
			var owner = getterCall.Method.AccessorOwner as IProperty;
			if (owner == null || !IsSameMember(getterCall.Method, owner.Getter) || !IsSameMember(setterCall.Method, owner.Setter))
				return false;
			if (setterCall.Arguments.Count != getterCall.Arguments.Count + 1)
				return false;
			// Ensure that same arguments are passed to getterCall and setterCall:
			for (int j = 0; j < getterCall.Arguments.Count; j++) {
				if (setterCall.Arguments[j].MatchStLoc(out var v) && v.IsSingleDefinition && v.LoadCount == 1) {
					if (getterCall.Arguments[j].MatchLdLoc(v)) {
						// OK, setter call argument is saved in temporary that is re-used for getter call
						if (finalizeMatch == null) {
							finalizeMatch = AdjustArguments;
						}
						continue;
					}
				}
				if (!SemanticHelper.IsPure(getterCall.Arguments[j].Flags))
					return false;
				if (!getterCall.Arguments[j].Match(setterCall.Arguments[j]).Success)
					return false;
			}
			return true;

			void AdjustArguments(ILTransformContext context)
			{
				Debug.Assert(setterCall.Arguments.Count == getterCall.Arguments.Count + 1);
				for (int j = 0; j < getterCall.Arguments.Count; j++) {
					if (setterCall.Arguments[j].MatchStLoc(out var v, out var value)) {
						Debug.Assert(v.IsSingleDefinition && v.LoadCount == 1);
						Debug.Assert(getterCall.Arguments[j].MatchLdLoc(v));
						getterCall.Arguments[j] = value;
					}
				}
			}
		}

		/// <summary>
		/// Transform compound assignments where the return value is not being used,
		/// or where there's an inlined assignment within the setter call.
		/// 
		/// Patterns handled:
		/// 1.
		///   callvirt set_Property(ldloc S_1, binary.op(callvirt get_Property(ldloc S_1), value))
		///   ==> compound.op.new(callvirt get_Property(ldloc S_1), value)
		/// 2.
		///   callvirt set_Property(ldloc S_1, stloc v(binary.op(callvirt get_Property(ldloc S_1), value)))
		///   ==> stloc v(compound.op.new(callvirt get_Property(ldloc S_1), value))
		/// 3.
		///   stobj(target, binary.op(ldobj(target), ...))
		///     where target is pure
		///   => compound.op(target, ...)
		/// </summary>
		/// <remarks>
		/// Called by ExpressionTransforms, or after the inline-assignment transform for setters.
		/// </remarks>
		internal static bool HandleCompoundAssign(ILInstruction compoundStore, StatementTransformContext context)
		{
			if (!context.Settings.MakeAssignmentExpressions || !context.Settings.IntroduceIncrementAndDecrement)
				return false;
			if (compoundStore is CallInstruction && compoundStore.SlotInfo != Block.InstructionSlot) {
				// replacing 'call set_Property' with a compound assignment instruction
				// changes the return value of the expression, so this is only valid on block-level.
				return false;
			}
			if (!IsCompoundStore(compoundStore, out var targetType, out var setterValue, context.TypeSystem))
				return false;
			// targetType = The type of the property/field/etc. being stored to.
			// setterValue = The value being stored.
			var storeInSetter = setterValue as StLoc;
			if (storeInSetter != null) {
				// We'll move the stloc to top-level:
				// callvirt set_Property(ldloc S_1, stloc v(binary.op(callvirt get_Property(ldloc S_1), value)))
				// ==> stloc v(compound.op.new(callvirt get_Property(ldloc S_1), value))
				setterValue = storeInSetter.Value;
				if (storeInSetter.Variable.Type.IsSmallIntegerType()) {
					// 'stloc v' implicitly truncates the value.
					// Ensure that type of 'v' matches the type of the property:
					if (storeInSetter.Variable.Type.GetSize() != targetType.GetSize())
						return false;
					if (storeInSetter.Variable.Type.GetSign() != targetType.GetSign())
						return false;
				}
			}
			ILInstruction newInst;
			if (UnwrapSmallIntegerConv(setterValue, out var smallIntConv) is BinaryNumericInstruction binary) {
				if (compoundStore is StLoc) {
					// transform local variables only for user-defined operators
					return false;
				}
				if (!IsMatchingCompoundLoad(binary.Left, compoundStore, out var target, out var targetKind, out var finalizeMatch, forbiddenVariable: storeInSetter?.Variable))
					return false;
				if (!ValidateCompoundAssign(binary, smallIntConv, targetType, context.Settings))
					return false;
				context.Step($"Compound assignment (binary.numeric)", compoundStore);
				finalizeMatch?.Invoke(context);
				newInst = new NumericCompoundAssign(
					binary, target, targetKind, binary.Right,
					targetType, CompoundEvalMode.EvaluatesToNewValue);
			} else if (setterValue is Call operatorCall && operatorCall.Method.IsOperator) {
				if (operatorCall.Arguments.Count == 0)
					return false;
				if (!IsMatchingCompoundLoad(operatorCall.Arguments[0], compoundStore, out var target, out var targetKind, out var finalizeMatch, forbiddenVariable: storeInSetter?.Variable))
					return false;
				ILInstruction rhs;
				if (operatorCall.Arguments.Count == 2) {
					if (CSharp.ExpressionBuilder.GetAssignmentOperatorTypeFromMetadataName(operatorCall.Method.Name) == null)
						return false;
					rhs = operatorCall.Arguments[1];
				} else if (operatorCall.Arguments.Count == 1) {
					if (!(operatorCall.Method.Name == "op_Increment" || operatorCall.Method.Name == "op_Decrement"))
						return false;
					// use a dummy node so that we don't need a dedicated instruction for user-defined unary operator calls
					rhs = new LdcI4(1);
				} else {
					return false;
				}
				if (operatorCall.IsLifted)
					return false; // TODO: add tests and think about whether nullables need special considerations
				context.Step($"Compound assignment (user-defined binary)", compoundStore);
				finalizeMatch?.Invoke(context);
				newInst = new UserDefinedCompoundAssign(operatorCall.Method, CompoundEvalMode.EvaluatesToNewValue,
					target, targetKind, rhs);
			} else if (setterValue is DynamicBinaryOperatorInstruction dynamicBinaryOp) {
				if (!IsMatchingCompoundLoad(dynamicBinaryOp.Left, compoundStore, out var target, out var targetKind, out var finalizeMatch, forbiddenVariable: storeInSetter?.Variable))
					return false;
				context.Step($"Compound assignment (dynamic binary)", compoundStore);
				finalizeMatch?.Invoke(context);
				newInst = new DynamicCompoundAssign(dynamicBinaryOp.Operation, dynamicBinaryOp.BinderFlags, target, dynamicBinaryOp.LeftArgumentInfo, dynamicBinaryOp.Right, dynamicBinaryOp.RightArgumentInfo, targetKind);
			} else if (setterValue is Call concatCall && UserDefinedCompoundAssign.IsStringConcat(concatCall.Method)) {
				// setterValue is a string.Concat() invocation
				if (compoundStore is StLoc) {
					// transform local variables only for user-defined operators
					return false;
				}
				if (concatCall.Arguments.Count != 2)
					return false; // for now we only support binary compound assignments
				if (!targetType.IsKnownType(KnownTypeCode.String))
					return false;
				if (!IsMatchingCompoundLoad(concatCall.Arguments[0], compoundStore, out var target, out var targetKind, out var finalizeMatch, forbiddenVariable: storeInSetter?.Variable))
					return false;
				context.Step($"Compound assignment (string concatenation)", compoundStore);
				finalizeMatch?.Invoke(context);
				newInst = new UserDefinedCompoundAssign(concatCall.Method, CompoundEvalMode.EvaluatesToNewValue,
					target, targetKind, concatCall.Arguments[1]);
			} else {
				return false;
			}
			newInst.AddILRange(setterValue);
			if (storeInSetter != null) {
				storeInSetter.Value = newInst;
				newInst = storeInSetter;
				context.RequestRerun(); // moving stloc to top-level might trigger inlining
			}
			compoundStore.ReplaceWith(newInst);
			if (newInst.Parent is Block inlineAssignBlock && inlineAssignBlock.Kind == BlockKind.CallInlineAssign) {
				// It's possible that we first replaced the instruction in an inline-assign helper block.
				// In such a situation, we know from the block invariant that we're have a storeInSetter.
				Debug.Assert(storeInSetter != null);
				Debug.Assert(storeInSetter.Variable.IsSingleDefinition && storeInSetter.Variable.LoadCount == 1);
				Debug.Assert(inlineAssignBlock.Instructions.Single() == storeInSetter);
				Debug.Assert(inlineAssignBlock.FinalInstruction.MatchLdLoc(storeInSetter.Variable));
				// Block CallInlineAssign { stloc I_0(compound.op(...)); final: ldloc I_0 }
				// --> compound.op(...)
				inlineAssignBlock.ReplaceWith(storeInSetter.Value);
			}
			return true;
		}
		
		/// <code>
		/// stloc s(value)
		/// stloc l(ldloc s)
		///   where neither 'stloc s' nor 'stloc l' truncates the value
		/// -->
		/// stloc s(stloc l(value))
		/// </code>
		bool TransformInlineAssignmentLocal(Block block, int pos)
		{
			var inst = block.Instructions[pos] as StLoc;
			var nextInst = block.Instructions.ElementAtOrDefault(pos + 1) as StLoc;
			if (inst == null || nextInst == null)
				return false;
			if (inst.Variable.Kind != VariableKind.StackSlot)
				return false;
			if (!(nextInst.Variable.Kind == VariableKind.Local || nextInst.Variable.Kind == VariableKind.Parameter))
				return false;
			if (!nextInst.Value.MatchLdLoc(inst.Variable))
				return false;
			if (IsImplicitTruncation(inst.Value, inst.Variable.Type, context.TypeSystem)) {
				// 'stloc s' is implicitly truncating the stack value
				return false;
			}
			if (IsImplicitTruncation(inst.Value, nextInst.Variable.Type, context.TypeSystem)) {
				// 'stloc l' is implicitly truncating the stack value
				return false;
			}
			if (nextInst.Variable.StackType == StackType.Ref) {
				// ref locals need to be initialized when they are declared, so
				// we can only use inline assignments when we know that the
				// ref local is definitely assigned.
				// We don't have an easy way to check for that in this transform,
				// so avoid inline assignments to ref locals for now.
				return false;
			}
			context.Step("Inline assignment to local variable", inst);
			var value = inst.Value;
			var var = nextInst.Variable;
			var stackVar = inst.Variable;
			block.Instructions.RemoveAt(pos);
			nextInst.ReplaceWith(new StLoc(stackVar, new StLoc(var, value)));
			return true;
		}

		/// <summary>
		/// Gets whether 'stobj type(..., value)' would evaluate to a different value than 'value'
		/// due to implicit truncation.
		/// </summary>
		static internal bool IsImplicitTruncation(ILInstruction value, IType type, ICompilation compilation, bool allowNullableValue = false)
		{
			if (!type.IsSmallIntegerType()) {
				// Implicit truncation in ILAst only happens for small integer types;
				// other types of implicit truncation in IL cause the ILReader to insert
				// conv instructions.
				return false;
			}
			// With small integer types, test whether the value might be changed by
			// truncation (based on type.GetSize()) followed by sign/zero extension (based on type.GetSign()).
			// (it's OK to have false-positives here if we're unsure)
			if (value.MatchLdcI4(out int val)) {
				switch (type.GetEnumUnderlyingType().GetDefinition()?.KnownTypeCode) {
					case KnownTypeCode.Boolean:
						return !(val == 0 || val == 1);
					case KnownTypeCode.Byte:
						return !(val >= byte.MinValue && val <= byte.MaxValue);
					case KnownTypeCode.SByte:
						return !(val >= sbyte.MinValue && val <= sbyte.MaxValue);
					case KnownTypeCode.Int16:
						return !(val >= short.MinValue && val <= short.MaxValue);
					case KnownTypeCode.UInt16:
					case KnownTypeCode.Char:
						return !(val >= ushort.MinValue && val <= ushort.MaxValue);
				}
			} else if (value is Conv conv) {
				return conv.TargetType != type.ToPrimitiveType();
			} else if (value is Comp) {
				return false; // comp returns 0 or 1, which always fits
			} else if (value is BinaryNumericInstruction bni) {
				switch (bni.Operator) {
					case BinaryNumericOperator.BitAnd:
					case BinaryNumericOperator.BitOr:
					case BinaryNumericOperator.BitXor:
						// If both input values fit into the type without truncation,
						// the result of a binary operator will also fit.
						return IsImplicitTruncation(bni.Left, type, compilation, allowNullableValue)
							|| IsImplicitTruncation(bni.Right, type, compilation, allowNullableValue);
				}
			} else if (value is IfInstruction ifInst) {
				return IsImplicitTruncation(ifInst.TrueInst, type, compilation, allowNullableValue)
					|| IsImplicitTruncation(ifInst.FalseInst, type, compilation, allowNullableValue);
			} else {
				IType inferredType = value.InferType(compilation);
				if (allowNullableValue) {
					inferredType = NullableType.GetUnderlyingType(inferredType);
				}
				if (inferredType.Kind != TypeKind.Unknown) {
					return !(inferredType.GetSize() <= type.GetSize() && inferredType.GetSign() == type.GetSign());
				}
			}
			return true;
		}

		/// <summary>
		/// Gets whether 'inst' is a possible store for use as a compound store.
		/// </summary>
		/// <remarks>
		/// Output parameters:
		/// storeType: The type of the value being stored.
		/// value: The value being stored (will be analyzed further to detect compound assignments)
		/// 
		/// Every IsCompoundStore() call should be followed by an IsMatchingCompoundLoad() call.
		/// </remarks>
		static bool IsCompoundStore(ILInstruction inst, out IType storeType, 
			out ILInstruction value, ICompilation compilation)
		{
			value = null;
			storeType = null;
			if (inst is StObj stobj) {
				// stobj.Type may just be 'int' (due to stind.i4) when we're actually operating on a 'ref MyEnum'.
				// Try to determine the real type of the object we're modifying:
				storeType = stobj.Target.InferType(compilation);
				if (storeType is ByReferenceType refType) {
					if (TypeUtils.IsCompatibleTypeForMemoryAccess(refType.ElementType, stobj.Type)) {
						storeType = refType.ElementType;
					} else {
						storeType = stobj.Type;
					}
				} else if (storeType is PointerType pointerType) {
					if (TypeUtils.IsCompatibleTypeForMemoryAccess(pointerType.ElementType, stobj.Type)) {
						storeType = pointerType.ElementType;
					} else {
						storeType = stobj.Type;
					}
				} else {
					storeType = stobj.Type;
				}
				value = stobj.Value;
				return SemanticHelper.IsPure(stobj.Target.Flags);
			} else if (inst is CallInstruction call && (call.OpCode == OpCode.Call || call.OpCode == OpCode.CallVirt)) {
				if (call.Method.Parameters.Count == 0) {
					return false;
				}
				foreach (var arg in call.Arguments.SkipLast(1)) {
					if (arg.MatchStLoc(out var v) && v.IsSingleDefinition && v.LoadCount == 1) {
						continue; // OK, IsMatchingCompoundLoad can perform an adjustment in this special case
					}
					if (!SemanticHelper.IsPure(arg.Flags)) {
						return false;
					}
				}
				storeType = call.Method.Parameters.Last().Type;
				value = call.Arguments.Last();
				return IsSameMember(call.Method, (call.Method.AccessorOwner as IProperty)?.Setter);
			} else if (inst is StLoc stloc && (stloc.Variable.Kind == VariableKind.Local || stloc.Variable.Kind == VariableKind.Parameter)) {
				storeType = stloc.Variable.Type;
				value = stloc.Value;
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Checks whether 'load' and 'store' both access the same store, and can be combined to a compound assignment.
		/// </summary>
		/// <param name="load">The load instruction to test.</param>
		/// <param name="store">The compound store to test against. Must have previously been tested via IsCompoundStore()</param>
		/// <param name="target">The target to use for the compound assignment instruction.</param>
		/// <param name="targetKind">The target kind to use for the compound assignment instruction.</param>
		/// <param name="finalizeMatch">If set to a non-null value, call this delegate to fix up minor mismatches between getter and setter.</param>
		/// <param name="forbiddenVariable">
		/// If given a non-null value, this function returns false if the forbiddenVariable is used in the load/store instructions.
		/// Some transforms effectively move a store around,
		/// which is only valid if the variable stored to does not occur in the compound load/store.
		/// </param>
		/// <param name="previousInstruction">
		/// Instruction preceding the load.
		/// </param>
		static bool IsMatchingCompoundLoad(ILInstruction load, ILInstruction store,
			out ILInstruction target, out CompoundTargetKind targetKind,
			out Action<ILTransformContext> finalizeMatch,
			ILVariable forbiddenVariable = null,
			ILInstruction previousInstruction = null)
		{
			target = null;
			targetKind = 0;
			finalizeMatch = null;
			if (load is LdObj ldobj && store is StObj stobj) {
				Debug.Assert(SemanticHelper.IsPure(stobj.Target.Flags));
				if (!SemanticHelper.IsPure(ldobj.Target.Flags))
					return false;
				if (forbiddenVariable != null && forbiddenVariable.IsUsedWithin(ldobj.Target))
					return false;
				target = ldobj.Target;
				targetKind = CompoundTargetKind.Address;
				if (ldobj.Target.Match(stobj.Target).Success) {
					return true;
				} else if (IsDuplicatedAddressComputation(stobj.Target, ldobj.Target)) {
					// Use S_0 as target, so that S_0 can later be eliminated by inlining.
					// (we can't eliminate previousInstruction right now, because it's before the transform's starting instruction)
					target = stobj.Target;
					return true;
				} else {
					return false;
				}
			} else if (MatchingGetterAndSetterCalls(load as CallInstruction, store as CallInstruction, out finalizeMatch)) {
				if (forbiddenVariable != null && forbiddenVariable.IsUsedWithin(load))
					return false;
				target = load;
				targetKind = CompoundTargetKind.Property;
				return true;
			} else if (load is LdLoc ldloc && store is StLoc stloc && ILVariableEqualityComparer.Instance.Equals(ldloc.Variable, stloc.Variable)) {
				if (ILVariableEqualityComparer.Instance.Equals(ldloc.Variable, forbiddenVariable))
					return false;
				target = new LdLoca(ldloc.Variable).WithILRange(ldloc);
				targetKind = CompoundTargetKind.Address;
				finalizeMatch = context => context.Function.RecombineVariables(ldloc.Variable, stloc.Variable);
				return true;
			} else {
				return false;
			}

			bool IsDuplicatedAddressComputation(ILInstruction storeTarget, ILInstruction loadTarget)
			{
				// Sometimes roslyn duplicates the address calculation:
				// stloc S_0(ldloc refParam)
				// stloc V_0(ldobj System.Int32(ldloc refParam))
				// stobj System.Int32(ldloc S_0, binary.add.i4(ldloc V_0, ldc.i4 1))
				while (storeTarget is LdFlda storeLdFlda && loadTarget is LdFlda loadLdFlda) {
					if (!storeLdFlda.Field.Equals(loadLdFlda.Field))
						return false;
					storeTarget = storeLdFlda.Target;
					loadTarget = loadLdFlda.Target;
				}
				if (!storeTarget.MatchLdLoc(out var s))
					return false;
				if (!(s.Kind == VariableKind.StackSlot && s.IsSingleDefinition && s != forbiddenVariable))
					return false;
				if (s.StoreInstructions.SingleOrDefault() != previousInstruction)
					return false;
				return previousInstruction is StLoc addressStore && addressStore.Value.Match(loadTarget).Success;
			}
		}

		/// <code>
		/// stobj(target, binary.add(stloc l(ldobj(target)), ldc.i4 1))
		///   where target is pure and does not use 'l', and the 'stloc l' does not truncate
		/// -->
		/// stloc l(compound.op.old(ldobj(target), ldc.i4 1))
		/// 
		///  -or-
		/// 
		/// call set_Prop(args..., binary.add(stloc l(call get_Prop(args...)), ldc.i4 1))
		///   where args.. are pure and do not use 'l', and the 'stloc l' does not truncate
		/// -->
		/// stloc l(compound.op.old(call get_Prop(target), ldc.i4 1))
		/// </code>
		/// <remarks>
		/// This pattern is used for post-increment by legacy csc.
		/// 
		/// Even though this transform operates only on a single expression, it's not an expression transform
		/// as the result value of the expression changes (this is OK only for statements in a block).
		/// </remarks>
		bool TransformPostIncDecOperatorWithInlineStore(Block block, int pos)
		{
			var store = block.Instructions[pos];
			if (!IsCompoundStore(store, out var targetType, out var value, context.TypeSystem)) {
				return false;
			}
			StLoc stloc;
			var binary = UnwrapSmallIntegerConv(value, out var conv) as BinaryNumericInstruction;
			if (binary != null && (binary.Right.MatchLdcI(1) || binary.Right.MatchLdcF4(1) || binary.Right.MatchLdcF8(1))) {
				if (!(binary.Operator == BinaryNumericOperator.Add || binary.Operator == BinaryNumericOperator.Sub))
					return false;
				if (!ValidateCompoundAssign(binary, conv, targetType, context.Settings))
					return false;
				stloc = binary.Left as StLoc;
			} else if (value is Call operatorCall && operatorCall.Method.IsOperator && operatorCall.Arguments.Count == 1) {
				if (!(operatorCall.Method.Name == "op_Increment" || operatorCall.Method.Name == "op_Decrement"))
					return false;
				if (operatorCall.IsLifted)
					return false; // TODO: add tests and think about whether nullables need special considerations
				stloc = operatorCall.Arguments[0] as StLoc;
			} else {
				return false;
			}
			if (stloc == null)
				return false;
			if (!(stloc.Variable.Kind == VariableKind.Local || stloc.Variable.Kind == VariableKind.StackSlot))
				return false;
			if (!IsMatchingCompoundLoad(stloc.Value, store, out var target, out var targetKind, out var finalizeMatch, forbiddenVariable: stloc.Variable))
				return false;
			if (IsImplicitTruncation(stloc.Value, stloc.Variable.Type, context.TypeSystem))
				return false;
			context.Step("TransformPostIncDecOperatorWithInlineStore", store);
			finalizeMatch?.Invoke(context);
			if (binary != null) {
				block.Instructions[pos] = new StLoc(stloc.Variable, new NumericCompoundAssign(
					binary, target, targetKind, binary.Right, targetType, CompoundEvalMode.EvaluatesToOldValue));
			} else {
				Call operatorCall = (Call)value;
				block.Instructions[pos] = new StLoc(stloc.Variable, new UserDefinedCompoundAssign(
					operatorCall.Method, CompoundEvalMode.EvaluatesToOldValue, target, targetKind, new LdcI4(1)));
			}
			return true;
		}

		/// <code>
		/// stloc tmp(ldobj(target))
		/// stobj(target, binary.op(ldloc tmp, ldc.i4 1))
		///   target is pure and does not use 'tmp', 'stloc does not truncate'
		/// -->
		/// stloc tmp(compound.op.old(ldobj(target), ldc.i4 1))
		/// </code>
		/// This is usually followed by inlining or eliminating 'tmp'.
		/// 
		/// Local variables use a similar pattern, also detected by this function:
		/// <code>
		/// stloc tmp(ldloc target)
		/// stloc target(binary.op(ldloc tmp, ldc.i4 1))
		/// -->
		/// stloc tmp(compound.op.old(ldloca target, ldc.i4 1))
		/// </code>
		/// <remarks>
		/// This pattern occurs with legacy csc for static fields, and with Roslyn for most post-increments.
		/// </remarks>
		bool TransformPostIncDecOperator(Block block, int i)
		{
			var inst = block.Instructions[i] as StLoc;
			var store = block.Instructions.ElementAtOrDefault(i + 1);
			if (inst == null || store == null)
				return false;
			var tmpVar = inst.Variable;
			if (!IsCompoundStore(store, out var targetType, out var value, context.TypeSystem))
				return false;
			if (IsImplicitTruncation(inst.Value, targetType, context.TypeSystem)) {
				// 'stloc tmp' is implicitly truncating the value
				return false;
			}
			if (!IsMatchingCompoundLoad(inst.Value, store, out var target, out var targetKind, out var finalizeMatch,
				forbiddenVariable: inst.Variable,
				previousInstruction: block.Instructions.ElementAtOrDefault(i - 1))) {
				return false;
			}
			if (UnwrapSmallIntegerConv(value, out var conv) is BinaryNumericInstruction binary) {
				if (!binary.Left.MatchLdLoc(tmpVar) || !(binary.Right.MatchLdcI(1) || binary.Right.MatchLdcF4(1) || binary.Right.MatchLdcF8(1)))
					return false;
				if (!(binary.Operator == BinaryNumericOperator.Add || binary.Operator == BinaryNumericOperator.Sub))
					return false;
				if (!ValidateCompoundAssign(binary, conv, targetType, context.Settings))
					return false;
				context.Step("TransformPostIncDecOperator (builtin)", inst);
				finalizeMatch?.Invoke(context);
				inst.Value = new NumericCompoundAssign(binary, target, targetKind, binary.Right,
					targetType, CompoundEvalMode.EvaluatesToOldValue);
			} else if (value is Call operatorCall && operatorCall.Method.IsOperator && operatorCall.Arguments.Count == 1) {
				if (!operatorCall.Arguments[0].MatchLdLoc(tmpVar))
					return false;
				if (!(operatorCall.Method.Name == "op_Increment" || operatorCall.Method.Name == "op_Decrement"))
					return false;
				if (operatorCall.IsLifted)
					return false; // TODO: add tests and think about whether nullables need special considerations
				context.Step("TransformPostIncDecOperator (user-defined)", inst);
				finalizeMatch?.Invoke(context);
				inst.Value = new UserDefinedCompoundAssign(operatorCall.Method,
					CompoundEvalMode.EvaluatesToOldValue, target, targetKind, new LdcI4(1));
			} else {
				return false;
			}
			block.Instructions.RemoveAt(i + 1);
			if (inst.Variable.IsSingleDefinition && inst.Variable.LoadCount == 0) {
				// dead store -> it was a statement-level post-increment
				inst.ReplaceWith(inst.Value);
			}
			return true;
		}
		
		static bool IsSameMember(IMember a, IMember b)
		{
			if (a == null || b == null)
				return false;
			a = a.MemberDefinition;
			b = b.MemberDefinition;
			return a.Equals(b);
		}
	}
}
