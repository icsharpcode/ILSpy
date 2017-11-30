// Copyright (c) 2014-2017 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Collection of transforms that detect simple expression patterns
	/// (e.g. 'cgt.un(..., ld.null)') and replace them with different instructions.
	/// </summary>
	/// <remarks>
	/// Should run after inlining so that the expression patterns can be detected.
	/// </remarks>
	public class ExpressionTransforms : ILVisitor, IStatementTransform
	{
		internal StatementTransformContext context;

		public static void RunOnSingleStatment(ILInstruction statement, ILTransformContext context)
		{
			if (statement == null)
				throw new ArgumentNullException(nameof(statement));
			if (!(statement.Parent is Block parent))
				throw new ArgumentException("ILInstruction must be a statement, i.e., direct child of a block.");
			new ExpressionTransforms().Run(parent, statement.ChildIndex, new StatementTransformContext(new BlockTransformContext(context)));
		}

		public void Run(Block block, int pos, StatementTransformContext context)
		{
			this.context = context;
			context.StepStartGroup($"ExpressionTransforms ({block.Label}:{pos})", block.Instructions[pos]);
			block.Instructions[pos].AcceptVisitor(this);
			context.StepEndGroup(keepIfEmpty: true);
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
			// "logic.not(arg)" is sugar for "comp(arg != ldc.i4 0)"
			if (inst.MatchLogicNot(out var arg)) {
				VisitLogicNot(inst, arg);
				return;
			} else if (inst.Kind == ComparisonKind.Inequality && inst.LiftingKind == ComparisonLiftingKind.None
				&& inst.Right.MatchLdcI4(0) && (IfInstruction.IsInConditionSlot(inst) || inst.Left is Comp)
			) {
				// if (comp(x != 0)) ==> if (x)
				// comp(comp(...) != 0) => comp(...)
				context.Step("Remove redundant comp(... != 0)", inst);
				inst.Left.AddILRange(inst.ILRange);
				inst.ReplaceWith(inst.Left);
				inst.Left.AcceptVisitor(this);
				return;
			}

			base.VisitComp(inst);
			if (inst.IsLifted) {
				return;
			}
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
				if (inst.Kind == ComparisonKind.GreaterThan) {
					context.Step("comp.unsigned(left > ldc.i4 0) => comp(left != ldc.i4 0)", inst);
					inst.Kind = ComparisonKind.Inequality;
					VisitComp(inst);
					return;
				} else if (inst.Kind == ComparisonKind.LessThanOrEqual) {
					context.Step("comp.unsigned(left <= ldc.i4 0) => comp(left == ldc.i4 0)", inst);
					inst.Kind = ComparisonKind.Equality;
					VisitComp(inst);
					return;
				}
			} else if (rightWithoutConv.MatchLdcI4(0) && inst.Kind.IsEqualityOrInequality()) {
				if (inst.Left.MatchLdLen(StackType.I, out ILInstruction array)) {
					// comp.unsigned(ldlen array == conv i4->i(ldc.i4 0))
					// => comp(ldlen.i4 array == ldc.i4 0)
					// This is a special case where the C# compiler doesn't generate conv.i4 after ldlen.
					context.Step("comp(ldlen.i4 array == ldc.i4 0)", inst);
					inst.InputType = StackType.I4;
					inst.Left.ReplaceWith(new LdLen(StackType.I4, array) { ILRange = inst.Left.ILRange });
					inst.Right = rightWithoutConv;
				}
			}

			if (inst.Right.MatchLdNull() && inst.Left.MatchBox(out arg, out var type) && type.Kind == TypeKind.TypeParameter) {
				if (inst.Kind == ComparisonKind.Equality) {
					context.Step("comp(box T(..) == ldnull) -> comp(.. == ldnull)", inst);
					inst.Left = arg;
				}
				if (inst.Kind == ComparisonKind.Inequality) {
					context.Step("comp(box T(..) != ldnull) -> comp(.. != ldnull)", inst);
					inst.Left = arg;
				}
			}
		}
		
		protected internal override void VisitConv(Conv inst)
		{
			inst.Argument.AcceptVisitor(this);
			if (inst.Argument.MatchLdLen(StackType.I, out ILInstruction array) && inst.TargetType.IsIntegerType() && !inst.CheckForOverflow) {
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

		void VisitLogicNot(Comp inst, ILInstruction arg)
		{
			ILInstruction lhs, rhs;
			if (arg is Comp comp) {
				if ((!comp.InputType.IsFloatType() && !comp.IsLifted) || comp.Kind.IsEqualityOrInequality()) {
					context.Step("push negation into comparison", inst);
					comp.Kind = comp.Kind.Negate();
					comp.AddILRange(inst.ILRange);
					inst.ReplaceWith(comp);
				}
				comp.AcceptVisitor(this);
			} else if (arg.MatchLogicAnd(out lhs, out rhs)) {
				// logic.not(if (lhs) rhs else ldc.i4 0)
				// ==> if (logic.not(lhs)) ldc.i4 1 else logic.not(rhs)
				context.Step("push negation into logic.and", inst);
				IfInstruction ifInst = (IfInstruction)arg;
				var ldc0 = ifInst.FalseInst;
				Debug.Assert(ldc0.MatchLdcI4(0));
				ifInst.Condition = Comp.LogicNot(lhs, inst.ILRange);
				ifInst.TrueInst = new LdcI4(1) { ILRange = ldc0.ILRange };
				ifInst.FalseInst = Comp.LogicNot(rhs, inst.ILRange);
				inst.ReplaceWith(ifInst);
				ifInst.AcceptVisitor(this);
			} else if (arg.MatchLogicOr(out lhs, out rhs)) {
				// logic.not(if (lhs) ldc.i4 1 else rhs)
				// ==> if (logic.not(lhs)) logic.not(rhs) else ldc.i4 0)
				context.Step("push negation into logic.or", inst);
				IfInstruction ifInst = (IfInstruction)arg;
				var ldc1 = ifInst.TrueInst;
				Debug.Assert(ldc1.MatchLdcI4(1));
				ifInst.Condition = Comp.LogicNot(lhs, inst.ILRange);
				ifInst.TrueInst = Comp.LogicNot(rhs, inst.ILRange);
				ifInst.FalseInst = new LdcI4(0) { ILRange = ldc1.ILRange };
				inst.ReplaceWith(ifInst);
				ifInst.AcceptVisitor(this);
			} else {
				arg.AcceptVisitor(this);
			}
		}
		
		protected internal override void VisitCall(Call inst)
		{
			var expr = EarlyExpressionTransforms.HandleCall(inst, context);
			if (expr != null) {
				// The resulting expression may trigger further rules, so continue visiting the replacement:
				expr.AcceptVisitor(this);
			} else {
				base.VisitCall(inst);
				TransformAssignment.HandleCallCompoundAssign(inst, context);
			}
		}

		protected internal override void VisitCallVirt(CallVirt inst)
		{
			base.VisitCallVirt(inst);
			TransformAssignment.HandleCallCompoundAssign(inst, context);
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

		protected internal override void VisitLdObj(LdObj inst)
		{
			base.VisitLdObj(inst);
			EarlyExpressionTransforms.LdObjToLdLoc(inst, context);
		}

		protected internal override void VisitStObj(StObj inst)
		{
			base.VisitStObj(inst);
			if (EarlyExpressionTransforms.StObjToStLoc(inst, context)) {
				context.RequestRerun();
				return;
			}
			TransformAssignment.HandleStObjCompoundAssign(inst, context);
		}

		protected internal override void VisitIfInstruction(IfInstruction inst)
		{
			inst.TrueInst.AcceptVisitor(this);
			inst.FalseInst.AcceptVisitor(this);
			inst = HandleConditionalOperator(inst);

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
				inst.Condition = Comp.LogicNot(inst.Condition);
			}
			// Process condition after our potential modifications.
			inst.Condition.AcceptVisitor(this);

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
				var newIf = new IfInstruction(Comp.LogicNot(inst.Condition), value2, value1);
				newIf.ILRange = inst.ILRange;
				inst.ReplaceWith(new StLoc(v, newIf));
				context.RequestRerun();  // trigger potential inlining of the newly created StLoc
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
