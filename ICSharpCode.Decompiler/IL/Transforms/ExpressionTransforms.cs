// Copyright (c) 2014 Daniel Grunwald
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

using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Collection of transforms that detect simple expression patterns
	/// (e.g. 'cgt.un(..., ld.null)') and replace them with different instructions.
	/// </summary>
	/// <remarks>
	/// Should run after inlining so that the expression patterns can be detected.
	/// </remarks>
	public class ExpressionTransforms : ILVisitor, IBlockTransform
	{
		internal ILTransformContext context;
		
		public void Run(Block block, BlockTransformContext context)
		{
			this.context = context;
			Default(block);
		}
		
		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children) {
				child.AcceptVisitor(this);
			}
		}

		protected internal override void VisitBlock(Block block)
		{
			// Don't visit child blocks; since this is a block transform
			// we know those were already handled previously.
		}

		protected internal override void VisitComp(Comp inst)
		{
			base.VisitComp(inst);
			if (inst.Right.MatchLdNull()) {
				if (inst.Kind == ComparisonKind.GreaterThan) {
					context.Step("comp(left > ldnull)  => comp(left != ldnull)", inst);
					inst.Kind = ComparisonKind.Inequality;
				} else if (inst.Kind == ComparisonKind.LessThanOrEqual) {
					context.Step("comp(left <= ldnull) => comp(left == ldnull)", inst);
					inst.Kind = ComparisonKind.Equality;
				}
			} else if (inst.Left.MatchLdNull()) {
				if (inst.Kind == ComparisonKind.LessThan) {
					context.Step("comp(ldnull < right)  => comp(ldnull != right)", inst);
					inst.Kind = ComparisonKind.Inequality;
				} else if (inst.Kind == ComparisonKind.GreaterThanOrEqual) {
					context.Step("comp(ldnull >= right) => comp(ldnull == right)", inst);
					inst.Kind = ComparisonKind.Equality;
				}
			}
			
			var rightWithoutConv = inst.Right.UnwrapConv(ConversionKind.SignExtend).UnwrapConv(ConversionKind.ZeroExtend);
			if (rightWithoutConv.MatchLdcI4(0)
			    && inst.Sign == Sign.Unsigned
			    && (inst.Kind == ComparisonKind.GreaterThan || inst.Kind == ComparisonKind.LessThanOrEqual))
			{
				ILInstruction array;
				if (inst.Left.MatchLdLen(StackType.I, out array)) {
					// comp.unsigned(ldlen array > conv i4->i(ldc.i4 0))
					// => comp(ldlen.i4 array > ldc.i4 0)
					// This is a special case where the C# compiler doesn't generate conv.i4 after ldlen.
					context.Step("comp(ldlen.i4 array > ldc.i4 0)", inst);
					inst.Left.ReplaceWith(new LdLen(StackType.I4, array) { ILRange = inst.Left.ILRange });
					inst.Right = rightWithoutConv;
				}
				if (inst.Kind == ComparisonKind.GreaterThan) {
					context.Step("comp.unsigned(left > ldc.i4 0) => comp(left != ldc.i4 0)", inst);
					inst.Kind = ComparisonKind.Inequality;
				} else if (inst.Kind == ComparisonKind.LessThanOrEqual) {
					context.Step("comp.unsigned(left <= ldc.i4 0) => comp(left == ldc.i4 0)", inst);
					inst.Kind = ComparisonKind.Equality;
				}
			}
		}
		
		protected internal override void VisitConv(Conv inst)
		{
			inst.Argument.AcceptVisitor(this);
			ILInstruction array;
			if (inst.Argument.MatchLdLen(StackType.I, out array) && inst.TargetType.IsIntegerType() && !inst.CheckForOverflow) {
				context.Step("conv.i4(ldlen array) => ldlen.i4(array)", inst);
				inst.AddILRange(inst.Argument.ILRange);
				inst.ReplaceWith(new LdLen(inst.TargetType.GetStackType(), array) { ILRange = inst.ILRange });
			}
		}

		protected internal override void VisitLdElema(LdElema inst)
		{
			base.VisitLdElema(inst);
			CleanUpArrayIndices(inst.Indices);
		}

		protected internal override void VisitNewArr(NewArr inst)
		{
			base.VisitNewArr(inst);
			CleanUpArrayIndices(inst.Indices);
		}

		void CleanUpArrayIndices(InstructionCollection<ILInstruction> indices)
		{
			foreach (ILInstruction index in indices) {
				if (index is Conv conv && conv.ResultType == StackType.I
					&& (conv.Kind == ConversionKind.Truncate && conv.CheckForOverflow
						|| conv.Kind == ConversionKind.ZeroExtend || conv.Kind == ConversionKind.SignExtend)
				) {
					context.Step("Remove conv.i from array index", index);
					index.ReplaceWith(conv.Argument);
				}
			}
		}

		protected internal override void VisitLogicNot(LogicNot inst)
		{
			ILInstruction arg, lhs, rhs;
			if (inst.Argument.MatchLogicNot(out arg)) {
				context.Step("logic.not(logic.not(arg)) => arg", inst);
				Debug.Assert(arg.ResultType == StackType.I4);
				arg.AddILRange(inst.ILRange);
				arg.AddILRange(inst.Argument.ILRange);
				inst.ReplaceWith(arg);
				arg.AcceptVisitor(this);
			} else if (inst.Argument is Comp) {
				Comp comp = (Comp)inst.Argument;
				if (comp.InputType != StackType.F || comp.Kind.IsEqualityOrInequality()) {
					context.Step("push negation into comparison", inst);
					comp.Kind = comp.Kind.Negate();
					comp.AddILRange(inst.ILRange);
					inst.ReplaceWith(comp);
				}
				comp.AcceptVisitor(this);
			} else if (inst.Argument.MatchLogicAnd(out lhs, out rhs)) {
				// logic.not(if (lhs) rhs else ldc.i4 0)
				// ==> if (logic.not(lhs)) ldc.i4 1 else logic.not(rhs)
				context.Step("push negation into logic.and", inst);
				IfInstruction ifInst = (IfInstruction)inst.Argument;
				var ldc0 = ifInst.FalseInst;
				Debug.Assert(ldc0.MatchLdcI4(0));
				ifInst.Condition = new LogicNot(lhs) { ILRange = inst.ILRange };
				ifInst.TrueInst = new LdcI4(1) { ILRange = ldc0.ILRange };
				ifInst.FalseInst = new LogicNot(rhs) { ILRange = inst.ILRange };
				inst.ReplaceWith(ifInst);
				ifInst.AcceptVisitor(this);
			} else if (inst.Argument.MatchLogicOr(out lhs, out rhs)) {
				// logic.not(if (lhs) ldc.i4 1 else rhs)
				// ==> if (logic.not(lhs)) logic.not(rhs) else ldc.i4 0)
				context.Step("push negation into logic.or", inst);
				IfInstruction ifInst = (IfInstruction)inst.Argument;
				var ldc1 = ifInst.TrueInst;
				Debug.Assert(ldc1.MatchLdcI4(1));
				ifInst.Condition = new LogicNot(lhs) { ILRange = inst.ILRange };
				ifInst.TrueInst = new LogicNot(rhs) { ILRange = inst.ILRange };
				ifInst.FalseInst = new LdcI4(0) { ILRange = ldc1.ILRange };
				inst.ReplaceWith(ifInst);
				ifInst.AcceptVisitor(this);
			} else {
				inst.Argument.AcceptVisitor(this);
			}
		}
		
		protected internal override void VisitCall(Call inst)
		{
			if (inst.Method.IsConstructor && !inst.Method.IsStatic && inst.Method.DeclaringType.Kind == TypeKind.Struct) {
				Debug.Assert(inst.Arguments.Count == inst.Method.Parameters.Count + 1);
				context.Step("Transform call to struct constructor", inst);
				// call(ref, ...)
				// => stobj(ref, newobj(...))
				var newObj = new NewObj(inst.Method);
				newObj.ILRange = inst.ILRange;
				newObj.Arguments.AddRange(inst.Arguments.Skip(1));
				var expr = new StObj(inst.Arguments[0], newObj, inst.Method.DeclaringType);
				inst.ReplaceWith(expr);
				// Both the StObj and the NewObj may trigger further rules, so continue visiting the replacement:
				VisitStObj(expr);
			} else {
				base.VisitCall(inst);
			}
		}
		
		protected internal override void VisitNewObj(NewObj inst)
		{
			LdcDecimal decimalConstant;
			if (TransformDecimalCtorToConstant(inst, out decimalConstant)) {
				context.Step("TransformDecimalCtorToConstant", inst);
				inst.ReplaceWith(decimalConstant);
				return;
			}
			base.VisitNewObj(inst);
		}
		
		bool TransformDecimalCtorToConstant(NewObj inst, out LdcDecimal result)
		{
			IType t = inst.Method.DeclaringType;
			result = null;
			if (!t.IsKnownType(KnownTypeCode.Decimal))
				return false;
			var args = inst.Arguments;
			if (args.Count == 1) {
				int val;
				if (args[0].MatchLdcI4(out val)) {
					result = new LdcDecimal(val);
					return true;
				}
			} else if (args.Count == 5) {
				int lo, mid, hi, isNegative, scale;
				if (args[0].MatchLdcI4(out lo) && args[1].MatchLdcI4(out mid) &&
				    args[2].MatchLdcI4(out hi) && args[3].MatchLdcI4(out isNegative) &&
				    args[4].MatchLdcI4(out scale))
				{
					result = new LdcDecimal(new decimal(lo, mid, hi, isNegative != 0, (byte)scale));
					return true;
				}
			}
			return false;
		}
		
		// This transform is required because ILInlining only works with stloc/ldloc
		protected internal override void VisitStObj(StObj inst)
		{
			base.VisitStObj(inst);
			if (StObjToStLoc(inst, context)) {
				return;
			}

			if (inst.Value is BinaryNumericInstruction binary
				&& binary.Left.MatchLdObj(out ILInstruction target, out IType t)
				&& inst.Target.Match(target).Success) 
			{
				context.Step("compound assignment", inst);
				// stobj(target, binary.op(ldobj(target), ...))
				// => compound.op(target, ...)
				inst.ReplaceWith(new CompoundAssignmentInstruction(binary.Operator, binary.Left, binary.Right, t, binary.CheckForOverflow, binary.Sign, CompoundAssignmentType.EvaluatesToNewValue));
			}
		}

		internal static bool StObjToStLoc(StObj inst, ILTransformContext context)
		{
			if (inst.Target.MatchLdLoca(out ILVariable v)
				&& TypeUtils.IsCompatibleTypeForMemoryAccess(new ByReferenceType(v.Type), inst.Type)
				&& inst.UnalignedPrefix == 0
				&& !inst.IsVolatile) {
				context.Step($"stobj(ldloca {v.Name}, ...) => stloc {v.Name}(...)", inst);
				inst.ReplaceWith(new StLoc(v, inst.Value));
				return true;
			}
			return false;
		}
		
		protected internal override void VisitIfInstruction(IfInstruction inst)
		{
			// Bring LogicAnd/LogicOr into their canonical forms:
			// if (cond) ldc.i4 0 else RHS --> if (!cond) RHS else ldc.i4 0
			// if (cond) RHS else ldc.i4 1 --> if (!cond) ldc.i4 1 else RHS
			// Be careful: when both LHS and RHS are the constant 1, we must not
			// swap the arguments as it would lead to an infinite transform loop.
			if (inst.TrueInst.MatchLdcI4(0) && !inst.FalseInst.MatchLdcI4(0)
				|| inst.FalseInst.MatchLdcI4(1) && !inst.TrueInst.MatchLdcI4(1))
			{
				context.Step("canonicalize logic and/or", inst);
				var t = inst.TrueInst;
				inst.TrueInst = inst.FalseInst;
				inst.FalseInst = t;
				inst.Condition = new LogicNot(inst.Condition);
			}

			base.VisitIfInstruction(inst);

			inst = HandleConditionalOperator(inst);
			if (new NullableLiftingTransform(context).Run(inst))
				return;
		}

		IfInstruction HandleConditionalOperator(IfInstruction inst)
		{
			// if (cond) stloc (A, V1) else stloc (A, V2) --> stloc (A, if (cond) V1 else V2)
			Block trueInst = inst.TrueInst as Block;
			if (trueInst == null || trueInst.Instructions.Count != 1)
				return inst;
			Block falseInst = inst.FalseInst as Block;
			if (falseInst == null || falseInst.Instructions.Count != 1)
				return inst;
			ILVariable v;
			ILInstruction value1, value2;
			if (trueInst.Instructions[0].MatchStLoc(out v, out value1) && falseInst.Instructions[0].MatchStLoc(v, out value2)) {
				context.Step("conditional operator", inst);
				var newIf = new IfInstruction(new LogicNot(inst.Condition), value2, value1);
				newIf.ILRange = inst.ILRange;
				inst.ReplaceWith(new StLoc(v, newIf));
				return newIf;
			}
			return inst;
		}

		protected internal override void VisitBinaryNumericInstruction(BinaryNumericInstruction inst)
		{
			base.VisitBinaryNumericInstruction(inst);
			switch (inst.Operator) {
				case BinaryNumericOperator.ShiftLeft:
				case BinaryNumericOperator.ShiftRight:
					if (inst.Right.MatchBinaryNumericInstruction(BinaryNumericOperator.BitAnd, out var lhs, out var rhs)
						&& rhs.MatchLdcI4(inst.ResultType == StackType.I8 ? 63 : 31))
					{
						// a << (b & 31) => a << b
						context.Step("Combine bit.and into shift", inst);
						inst.Right = lhs;
					}
					break;
			}
		}

		protected internal override void VisitTryCatchHandler(TryCatchHandler inst)
		{
			base.VisitTryCatchHandler(inst);
			if (inst.Filter is BlockContainer filterContainer && filterContainer.Blocks.Count == 1) {
				TransformCatchWhen(inst, filterContainer.EntryPoint);
			}
			if (inst.Body is BlockContainer catchContainer)
				TransformCatchVariable(inst, catchContainer.EntryPoint);
		}

		/// <summary>
		/// Transform local exception variable.
		/// </summary>
		void TransformCatchVariable(TryCatchHandler handler, Block entryPoint)
		{
			if (!entryPoint.Instructions[0].MatchStLoc(out var exceptionVar, out var exceptionSlotLoad))
				return;
			if (!exceptionVar.IsSingleDefinition || exceptionVar.Kind != VariableKind.Local)
				return;
			if (!exceptionSlotLoad.MatchLdLoc(handler.Variable) || !handler.Variable.IsSingleDefinition || handler.Variable.LoadCount != 1)
				return;
			handler.Variable = exceptionVar;
			exceptionVar.Kind = VariableKind.Exception;
			entryPoint.Instructions.RemoveAt(0);
		}

		/// <summary>
		/// Inline condition from catch-when condition BlockContainer, if possible.
		/// </summary>
		void TransformCatchWhen(TryCatchHandler handler, Block entryPoint)
		{
			TransformCatchVariable(handler, entryPoint);
			if (entryPoint.Instructions.Count == 1 && entryPoint.Instructions[0].MatchLeave(out _, out var condition)) {
				handler.Filter = condition;
			}
		}
	}
}
