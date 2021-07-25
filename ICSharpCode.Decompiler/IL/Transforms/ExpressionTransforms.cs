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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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
	public class ExpressionTransforms : ILVisitor, IStatementTransform
	{
		internal StatementTransformContext context;

		public static void RunOnSingleStatement(ILInstruction statement, ILTransformContext context)
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
			foreach (var child in inst.Children)
			{
				child.AcceptVisitor(this);
			}
		}

		protected internal override void VisitBlockContainer(BlockContainer container)
		{
			if (container.Kind == ContainerKind.Switch)
			{
				// Special case for switch: Only visit the switch condition block.
				var switchInst = (SwitchInstruction)container.EntryPoint.Instructions[0];
				switchInst.Value.AcceptVisitor(this);

				HandleSwitchExpression(container, switchInst);
			}
			// No need to call base.VisitBlockContainer, see comment in VisitBlock.
		}

		protected internal override void VisitBlock(Block block)
		{
			if (block.Kind == BlockKind.ControlFlow)
			{
				// Don't visit child control flow blocks;
				// since this is a block transform
				// we know those were already handled previously.
				return;
			}
			base.VisitBlock(block);
		}

		protected internal override void VisitComp(Comp inst)
		{
			// "logic.not(arg)" is sugar for "comp(arg != ldc.i4 0)"
			if (inst.MatchLogicNot(out var arg))
			{
				VisitLogicNot(inst, arg);
				return;
			}
			else if (inst.Kind == ComparisonKind.Inequality && inst.LiftingKind == ComparisonLiftingKind.None
			  && inst.Right.MatchLdcI4(0) && (IfInstruction.IsInConditionSlot(inst) || inst.Left is Comp)
		  )
			{
				// if (comp(x != 0)) ==> if (x)
				// comp(comp(...) != 0) => comp(...)
				context.Step("Remove redundant comp(... != 0)", inst);
				inst.Left.AddILRange(inst);
				inst.ReplaceWith(inst.Left);
				inst.Left.AcceptVisitor(this);
				return;
			}

			base.VisitComp(inst);
			if (inst.IsLifted)
			{
				return;
			}
			if (inst.Right.MatchLdNull())
			{
				if (inst.Kind == ComparisonKind.GreaterThan)
				{
					context.Step("comp(left > ldnull)  => comp(left != ldnull)", inst);
					inst.Kind = ComparisonKind.Inequality;
				}
				else if (inst.Kind == ComparisonKind.LessThanOrEqual)
				{
					context.Step("comp(left <= ldnull) => comp(left == ldnull)", inst);
					inst.Kind = ComparisonKind.Equality;
				}
			}
			else if (inst.Left.MatchLdNull())
			{
				if (inst.Kind == ComparisonKind.LessThan)
				{
					context.Step("comp(ldnull < right)  => comp(ldnull != right)", inst);
					inst.Kind = ComparisonKind.Inequality;
				}
				else if (inst.Kind == ComparisonKind.GreaterThanOrEqual)
				{
					context.Step("comp(ldnull >= right) => comp(ldnull == right)", inst);
					inst.Kind = ComparisonKind.Equality;
				}
			}

			var rightWithoutConv = inst.Right.UnwrapConv(ConversionKind.SignExtend).UnwrapConv(ConversionKind.ZeroExtend);
			if (rightWithoutConv.MatchLdcI4(0)
				&& inst.Sign == Sign.Unsigned
				&& (inst.Kind == ComparisonKind.GreaterThan || inst.Kind == ComparisonKind.LessThanOrEqual))
			{
				if (inst.Kind == ComparisonKind.GreaterThan)
				{
					context.Step("comp.unsigned(left > ldc.i4 0) => comp(left != ldc.i4 0)", inst);
					inst.Kind = ComparisonKind.Inequality;
					VisitComp(inst);
					return;
				}
				else if (inst.Kind == ComparisonKind.LessThanOrEqual)
				{
					context.Step("comp.unsigned(left <= ldc.i4 0) => comp(left == ldc.i4 0)", inst);
					inst.Kind = ComparisonKind.Equality;
					VisitComp(inst);
					return;
				}
			}
			else if (rightWithoutConv.MatchLdcI4(0) && inst.Kind.IsEqualityOrInequality())
			{
				if (inst.Left.MatchLdLen(StackType.I, out ILInstruction array))
				{
					// comp.unsigned(ldlen array == conv i4->i(ldc.i4 0))
					// => comp(ldlen.i4 array == ldc.i4 0)
					// This is a special case where the C# compiler doesn't generate conv.i4 after ldlen.
					context.Step("comp(ldlen.i4 array == ldc.i4 0)", inst);
					inst.InputType = StackType.I4;
					inst.Left.ReplaceWith(new LdLen(StackType.I4, array).WithILRange(inst.Left));
					inst.Right = rightWithoutConv;
				}
				else if (inst.Left is Conv conv && conv.TargetType == PrimitiveType.I && conv.Argument.ResultType == StackType.O)
				{
					// C++/CLI sometimes uses this weird comparison with null:
					context.Step("comp(conv o->i (ldloc obj) == conv i4->i <sign extend>(ldc.i4 0))", inst);
					// -> comp(ldloc obj == ldnull)
					inst.InputType = StackType.O;
					inst.Left = conv.Argument;
					inst.Right = new LdNull().WithILRange(inst.Right);
					inst.Right.AddILRange(rightWithoutConv);
				}
			}

			if (inst.Right.MatchLdNull() && inst.Left.MatchBox(out arg, out var type) && type.Kind == TypeKind.TypeParameter)
			{
				if (inst.Kind == ComparisonKind.Equality)
				{
					context.Step("comp(box T(..) == ldnull) -> comp(.. == ldnull)", inst);
					inst.Left = arg;
				}
				if (inst.Kind == ComparisonKind.Inequality)
				{
					context.Step("comp(box T(..) != ldnull) -> comp(.. != ldnull)", inst);
					inst.Left = arg;
				}
			}
		}

		protected internal override void VisitConv(Conv inst)
		{
			inst.Argument.AcceptVisitor(this);
			if (inst.Argument.MatchLdLen(StackType.I, out ILInstruction array) && inst.TargetType.IsIntegerType()
				&& (!inst.CheckForOverflow || context.Settings.AssumeArrayLengthFitsIntoInt32))
			{
				context.Step("conv.i4(ldlen array) => ldlen.i4(array)", inst);
				inst.AddILRange(inst.Argument);
				inst.ReplaceWith(new LdLen(inst.TargetType.GetStackType(), array).WithILRange(inst));
				return;
			}
			if (inst.TargetType.IsFloatType() && inst.Argument is Conv conv
				&& conv.Kind == ConversionKind.IntToFloat && conv.TargetType == PrimitiveType.R)
			{
				// IL conv.r.un does not indicate whether to convert the target type to R4 or R8,
				// so the C# compiler usually follows it with an explicit conv.r4 or conv.r8.
				// To avoid emitting '(float)(double)val', we combine these two conversions:
				context.Step("conv.rN(conv.r.un(...)) => conv.rN.un(...)", inst);
				inst.ReplaceWith(new Conv(conv.Argument, conv.InputType, conv.InputSign, inst.TargetType, inst.CheckForOverflow, inst.IsLifted | conv.IsLifted));
				return;
			}
		}

		protected internal override void VisitBox(Box inst)
		{
			inst.Argument.AcceptVisitor(this);
			if (inst.Type.IsReferenceType == true && inst.Argument.ResultType == inst.ResultType)
			{
				// For reference types, box is a no-op.
				context.Step("box ref-type(arg) => arg", inst);
				inst.Argument.AddILRange(inst);
				inst.ReplaceWith(inst.Argument);
			}
		}

		protected internal override void VisitLdElema(LdElema inst)
		{
			base.VisitLdElema(inst);
			CleanUpArrayIndices(inst.Indices);
			if (IndexRangeTransform.HandleLdElema(inst, context))
				return;
		}

		protected internal override void VisitNewArr(NewArr inst)
		{
			base.VisitNewArr(inst);
			CleanUpArrayIndices(inst.Indices);
		}

		void CleanUpArrayIndices(InstructionCollection<ILInstruction> indices)
		{
			foreach (ILInstruction index in indices)
			{
				if (index is Conv conv && conv.ResultType == StackType.I
					&& (conv.Kind == ConversionKind.Truncate && conv.CheckForOverflow
						|| conv.Kind == ConversionKind.ZeroExtend || conv.Kind == ConversionKind.SignExtend)
				)
				{
					context.Step("Remove conv.i from array index", index);
					index.ReplaceWith(conv.Argument);
				}
			}
		}

		void VisitLogicNot(Comp inst, ILInstruction arg)
		{
			ILInstruction lhs, rhs;
			if (arg is Comp comp)
			{
				if ((!comp.InputType.IsFloatType() && !comp.IsLifted) || comp.Kind.IsEqualityOrInequality())
				{
					context.Step("push negation into comparison", inst);
					comp.Kind = comp.Kind.Negate();
					comp.AddILRange(inst);
					inst.ReplaceWith(comp);
				}
				comp.AcceptVisitor(this);
			}
			else if (arg.MatchLogicAnd(out lhs, out rhs))
			{
				// logic.not(if (lhs) rhs else ldc.i4 0)
				// ==> if (logic.not(lhs)) ldc.i4 1 else logic.not(rhs)
				context.Step("push negation into logic.and", inst);
				IfInstruction ifInst = (IfInstruction)arg;
				var ldc0 = ifInst.FalseInst;
				Debug.Assert(ldc0.MatchLdcI4(0));
				ifInst.Condition = Comp.LogicNot(lhs).WithILRange(inst);
				ifInst.TrueInst = new LdcI4(1).WithILRange(ldc0);
				ifInst.FalseInst = Comp.LogicNot(rhs).WithILRange(inst);
				inst.ReplaceWith(ifInst);
				ifInst.AcceptVisitor(this);
			}
			else if (arg.MatchLogicOr(out lhs, out rhs))
			{
				// logic.not(if (lhs) ldc.i4 1 else rhs)
				// ==> if (logic.not(lhs)) logic.not(rhs) else ldc.i4 0)
				context.Step("push negation into logic.or", inst);
				IfInstruction ifInst = (IfInstruction)arg;
				var ldc1 = ifInst.TrueInst;
				Debug.Assert(ldc1.MatchLdcI4(1));
				ifInst.Condition = Comp.LogicNot(lhs).WithILRange(inst);
				ifInst.TrueInst = Comp.LogicNot(rhs).WithILRange(inst);
				ifInst.FalseInst = new LdcI4(0).WithILRange(ldc1);
				inst.ReplaceWith(ifInst);
				ifInst.AcceptVisitor(this);
			}
			else
			{
				arg.AcceptVisitor(this);
			}
		}

		protected internal override void VisitCall(Call inst)
		{
			base.VisitCall(inst);
			TransformAssignment.HandleCompoundAssign(inst, context);
		}

		protected internal override void VisitCallVirt(CallVirt inst)
		{
			base.VisitCallVirt(inst);
			TransformAssignment.HandleCompoundAssign(inst, context);
		}

		protected internal override void VisitNewObj(NewObj inst)
		{
			if (TransformDecimalCtorToConstant(inst, out LdcDecimal decimalConstant))
			{
				context.Step("TransformDecimalCtorToConstant", inst);
				inst.ReplaceWith(decimalConstant);
				return;
			}
			Block block;
			if (TransformSpanTCtorContainingStackAlloc(inst, out ILInstruction locallocSpan))
			{
				context.Step("new Span<T>(stackalloc) -> stackalloc Span<T>", inst);
				inst.ReplaceWith(locallocSpan);
				block = null;
				ILInstruction stmt = locallocSpan;
				while (stmt.Parent != null)
				{
					if (stmt.Parent is Block b)
					{
						block = b;
						break;
					}
					stmt = stmt.Parent;
				}
				// Special case to eliminate extra store
				if (stmt.GetNextSibling() is StLoc storeStmt && storeStmt.Value is LdLoc)
					ILInlining.InlineIfPossible(block, stmt.ChildIndex, context);
				return;
			}
			if (TransformArrayInitializers.TransformSpanTArrayInitialization(inst, context, out block))
			{
				context.Step("TransformSpanTArrayInitialization: single-dim", inst);
				inst.ReplaceWith(block);
				return;
			}
			if (TransformDelegateCtorLdVirtFtnToLdVirtDelegate(inst, out LdVirtDelegate ldVirtDelegate))
			{
				context.Step("new Delegate(target, ldvirtftn Method) -> ldvirtdelegate Delegate Method(target)", inst);
				inst.ReplaceWith(ldVirtDelegate);
				return;
			}
			base.VisitNewObj(inst);
		}

		/// <summary>
		/// newobj Delegate..ctor(target, ldvirtftn TargetMethod(target))
		/// =>
		/// ldvirtdelegate System.Delegate TargetMethod(target)
		/// </summary>
		bool TransformDelegateCtorLdVirtFtnToLdVirtDelegate(NewObj inst, out LdVirtDelegate ldVirtDelegate)
		{
			ldVirtDelegate = null;
			if (inst.Method.DeclaringType.Kind != TypeKind.Delegate)
				return false;
			if (inst.Arguments.Count != 2)
				return false;
			if (!(inst.Arguments[1] is LdVirtFtn ldVirtFtn))
				return false;
			if (!SemanticHelper.IsPure(inst.Arguments[0].Flags))
				return false;
			if (!inst.Arguments[0].Match(ldVirtFtn.Argument).Success)
				return false;
			ldVirtDelegate = new LdVirtDelegate(inst.Arguments[0], inst.Method.DeclaringType, ldVirtFtn.Method)
				.WithILRange(inst).WithILRange(ldVirtFtn).WithILRange(ldVirtFtn.Argument);
			return true;
		}

		/// <summary>
		/// newobj Span..ctor(localloc(conv i4->u &lt;zero extend&gt;(ldc.i4 sizeInBytes)), numberOfElementsExpr)
		/// =>
		/// localloc.span T(numberOfElementsExpr)
		/// 
		/// -or-
		/// 
		/// newobj Span..ctor(Block IL_0000 (StackAllocInitializer) {
		///		stloc I_0(localloc(conv i4->u&lt;zero extend>(ldc.i4 sizeInBytes)))
		///		...
		///		final: ldloc I_0
		///	}, numberOfElementsExpr)
		/// =>
		/// Block IL_0000 (StackAllocInitializer) {
		///		stloc I_0(localloc.span T(numberOfElementsExpr))
		///		...
		///		final: ldloc I_0
		/// }
		/// </summary>
		bool TransformSpanTCtorContainingStackAlloc(NewObj newObj, out ILInstruction locallocSpan)
		{
			locallocSpan = null;
			IType type = newObj.Method.DeclaringType;
			if (!type.IsKnownType(KnownTypeCode.SpanOfT) && !type.IsKnownType(KnownTypeCode.ReadOnlySpanOfT))
				return false;
			if (newObj.Arguments.Count != 2 || type.TypeArguments.Count != 1)
				return false;
			IType elementType = type.TypeArguments[0];
			if (newObj.Arguments[0].MatchLocAlloc(out var sizeInBytes) && MatchesElementCount(sizeInBytes, elementType, newObj.Arguments[1]))
			{
				locallocSpan = new LocAllocSpan(newObj.Arguments[1], type);
				return true;
			}
			if (newObj.Arguments[0] is Block initializer && initializer.Kind == BlockKind.StackAllocInitializer)
			{
				if (!initializer.Instructions[0].MatchStLoc(out var initializerVariable, out var value))
					return false;
				if (!(value.MatchLocAlloc(out sizeInBytes) && MatchesElementCount(sizeInBytes, elementType, newObj.Arguments[1])))
					return false;
				var newVariable = initializerVariable.Function.RegisterVariable(VariableKind.InitializerTarget, type);
				foreach (var load in initializerVariable.LoadInstructions.ToArray())
				{
					ILInstruction newInst = new LdLoc(newVariable);
					newInst.AddILRange(load);
					if (load.Parent != initializer)
						newInst = new Conv(newInst, PrimitiveType.I, false, Sign.None);
					load.ReplaceWith(newInst);
				}
				foreach (var store in initializerVariable.StoreInstructions.ToArray())
				{
					store.Variable = newVariable;
				}
				value.ReplaceWith(new LocAllocSpan(newObj.Arguments[1], type));
				locallocSpan = initializer;
				return true;
			}
			return false;
		}

		bool MatchesElementCount(ILInstruction sizeInBytesInstr, IType elementType, ILInstruction elementCountInstr2)
		{
			var pointerType = new PointerType(elementType);
			var elementCountInstr = PointerArithmeticOffset.Detect(sizeInBytesInstr, pointerType.ElementType, checkForOverflow: true, unwrapZeroExtension: true);
			if (elementCountInstr == null || !elementCountInstr.UnwrapConv(ConversionKind.ZeroExtend).Match(elementCountInstr2).Success)
				return false;
			return true;
		}

		bool TransformDecimalCtorToConstant(NewObj inst, out LdcDecimal result)
		{
			IType t = inst.Method.DeclaringType;
			result = null;
			if (!t.IsKnownType(KnownTypeCode.Decimal))
				return false;
			var args = inst.Arguments;
			if (args.Count == 1)
			{
				int val;
				if (args[0].MatchLdcI4(out val))
				{
					result = new LdcDecimal(val);
					return true;
				}
			}
			else if (args.Count == 5)
			{
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

		bool TransformDecimalFieldToConstant(LdObj inst, out LdcDecimal result)
		{
			if (inst.MatchLdsFld(out var field) && field.DeclaringType.IsKnownType(KnownTypeCode.Decimal))
			{
				decimal? value = null;
				if (field.Name == "One")
				{
					value = decimal.One;
				}
				else if (field.Name == "MinusOne")
				{
					value = decimal.MinusOne;
				}
				else if (field.Name == "Zero")
				{
					value = decimal.Zero;
				}
				if (value != null)
				{
					result = new LdcDecimal(value.Value).WithILRange(inst).WithILRange(inst.Target);
					return true;
				}
			}
			result = null;
			return false;
		}

		protected internal override void VisitLdObj(LdObj inst)
		{
			base.VisitLdObj(inst);
			EarlyExpressionTransforms.AddressOfLdLocToLdLoca(inst, context);
			if (EarlyExpressionTransforms.LdObjToLdLoc(inst, context))
				return;
			if (TransformDecimalFieldToConstant(inst, out LdcDecimal decimalConstant))
			{
				context.Step("TransformDecimalFieldToConstant", inst);
				inst.ReplaceWith(decimalConstant);
				return;
			}
		}

		protected internal override void VisitStObj(StObj inst)
		{
			base.VisitStObj(inst);
			if (EarlyExpressionTransforms.StObjToStLoc(inst, context))
			{
				context.RequestRerun();
				return;
			}
			TransformAssignment.HandleCompoundAssign(inst, context);
		}

		protected internal override void VisitStLoc(StLoc inst)
		{
			base.VisitStLoc(inst);
			TransformAssignment.HandleCompoundAssign(inst, context);
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

			if (TransformDynamicAddAssignOrRemoveAssign(inst))
				return;
			if (inst.MatchIfInstructionPositiveCondition(out var condition, out var trueInst, out var falseInst))
			{
				ILInstruction transformed = UserDefinedLogicTransform.Transform(condition, trueInst, falseInst);
				if (transformed == null)
				{
					transformed = UserDefinedLogicTransform.TransformDynamic(condition, trueInst, falseInst);
				}
				if (transformed != null)
				{
					context.Step("User-defined short-circuiting logic operator (roslyn pattern)", condition);
					transformed.AddILRange(inst);
					inst.ReplaceWith(transformed);
					return;
				}
			}
			if (MatchInstruction.IsPatternMatch(inst.Condition, out _)
				&& inst.TrueInst.MatchLdcI4(1) && inst.FalseInst.MatchLdcI4(0))
			{
				context.Step("match(x) ? true : false -> match(x)", inst);
				inst.Condition.AddILRange(inst);
				inst.ReplaceWith(inst.Condition);
				return;
			}
		}

		IfInstruction HandleConditionalOperator(IfInstruction inst)
		{
			// if (cond) stloc A(V1) else stloc A(V2) --> stloc A(if (cond) V1 else V2)
			Block trueInst = inst.TrueInst as Block;
			if (trueInst == null || trueInst.Instructions.Count != 1)
				return inst;
			Block falseInst = inst.FalseInst as Block;
			if (falseInst == null || falseInst.Instructions.Count != 1)
				return inst;
			ILVariable v;
			ILInstruction value1, value2;
			if (trueInst.Instructions[0].MatchStLoc(out v, out value1) && falseInst.Instructions[0].MatchStLoc(v, out value2))
			{
				context.Step("conditional operator", inst);
				var newIf = new IfInstruction(Comp.LogicNot(inst.Condition), value2, value1);
				newIf.AddILRange(inst);
				inst.ReplaceWith(new StLoc(v, newIf));
				context.RequestRerun();  // trigger potential inlining of the newly created StLoc
				return newIf;
			}
			return inst;
		}

		private void HandleSwitchExpression(BlockContainer container, SwitchInstruction switchInst)
		{
			if (!context.Settings.SwitchExpressions)
				return;
			Debug.Assert(container.Kind == ContainerKind.Switch);
			Debug.Assert(container.ResultType == StackType.Void);
			var defaultSection = switchInst.GetDefaultSection();
			StackType resultType = StackType.Void;
			BlockContainer leaveTarget = null;
			ILVariable resultVariable = null;
			foreach (var section in switchInst.Sections)
			{
				if (section != defaultSection)
				{
					// every section except for the default must have exactly 1 label
					if (section.Labels.Count() != (section.HasNullLabel ? 0u : 1u))
						return;
				}
				if (!section.Body.MatchBranch(out var sectionBlock))
					return;
				if (sectionBlock.IncomingEdgeCount != 1)
					return;
				if (sectionBlock.Parent != container)
					return;
				if (sectionBlock.Instructions.Count == 1)
				{
					if (sectionBlock.Instructions[0] is Throw)
					{
						// OK
					}
					else if (sectionBlock.Instructions[0] is Leave leave)
					{
						if (!leave.IsLeavingFunction)
							return;
						leaveTarget ??= leave.TargetContainer;
						Debug.Assert(leaveTarget == leave.TargetContainer);
						resultType = leave.Value.ResultType;
					}
					else
					{
						return;
					}
				}
				else if (sectionBlock.Instructions.Count == 2)
				{
					if (!sectionBlock.Instructions[0].MatchStLoc(out var v, out _))
						return;
					if (!sectionBlock.Instructions[1].MatchLeave(container))
						return;
					resultVariable ??= v;
					if (resultVariable != v)
						return;
					resultType = resultVariable.StackType;
				}
				else
				{
					return;
				}
			}
			// Exactly one of resultVariable/leaveTarget must be null
			if ((resultVariable == null) == (leaveTarget == null))
				return;
			if (switchInst.Value is StringToInt str2int)
			{
				// validate that each integer is used for exactly one value
				var integersUsed = new HashSet<int>();
				foreach ((string key, int val) in str2int.Map)
				{
					if (!integersUsed.Add(val))
						return;
				}
			}

			context.Step("Switch Expression", switchInst);
			switchInst.SetResultType(resultType);
			foreach (var section in switchInst.Sections)
			{
				var block = ((Branch)section.Body).TargetBlock;
				if (block.Instructions.Count == 1)
				{
					if (block.Instructions[0] is Throw t)
					{
						t.resultType = resultType;
						section.Body = t;
					}
					else if (block.Instructions[0] is Leave leave)
					{
						section.Body = leave.Value;
					}
					else
					{
						throw new InvalidOperationException();
					}
				}
				else
				{
					section.Body = ((StLoc)block.Instructions[0]).Value;
				}
			}
			if (resultVariable != null)
			{
				container.ReplaceWith(new StLoc(resultVariable, switchInst));
			}
			else
			{
				container.ReplaceWith(new Leave(leaveTarget, switchInst));
			}
			context.RequestRerun(); // new StLoc might trigger inlining
		}

		/// <summary>
		/// op is either add or remove/subtract:
		/// if (dynamic.isevent (target)) {
		///     dynamic.invokemember.invokespecial.discard op_Name(target, value)
		/// } else {
		///     dynamic.compound.op (dynamic.getmember Name(target), value)
		/// }
		/// =>
		/// dynamic.compound.op (dynamic.getmember Name(target), value)
		/// </summary>
		bool TransformDynamicAddAssignOrRemoveAssign(IfInstruction inst)
		{
			if (!inst.MatchIfInstructionPositiveCondition(out var condition, out var trueInst, out var falseInst))
				return false;
			if (!(condition is DynamicIsEventInstruction isEvent))
				return false;
			trueInst = Block.Unwrap(trueInst);
			falseInst = Block.Unwrap(falseInst);
			if (!(falseInst is DynamicCompoundAssign dynamicCompoundAssign))
				return false;
			if (!(dynamicCompoundAssign.Target is DynamicGetMemberInstruction getMember))
				return false;
			if (!SemanticHelper.IsPure(isEvent.Argument.Flags))
				return false;
			if (!isEvent.Argument.Match(getMember.Target).Success)
				return false;
			if (!(trueInst is DynamicInvokeMemberInstruction invokeMember))
				return false;
			if (!(invokeMember.BinderFlags.HasFlag(CSharpBinderFlags.InvokeSpecialName) && invokeMember.BinderFlags.HasFlag(CSharpBinderFlags.ResultDiscarded)))
				return false;
			switch (dynamicCompoundAssign.Operation)
			{
				case ExpressionType.AddAssign:
					if (invokeMember.Name != "add_" + getMember.Name)
						return false;
					break;
				case ExpressionType.SubtractAssign:
					if (invokeMember.Name != "remove_" + getMember.Name)
						return false;
					break;
				default:
					return false;
			}
			if (!dynamicCompoundAssign.Value.Match(invokeMember.Arguments[1]).Success)
				return false;
			if (!invokeMember.Arguments[0].Match(getMember.Target).Success)
				return false;
			context.Step("+= / -= dynamic.isevent pattern -> dynamic.compound.op", inst);
			inst.ReplaceWith(dynamicCompoundAssign);
			return true;
		}

		/// <summary>
		/// dynamic.setmember.compound Name(target, dynamic.binary.operator op(dynamic.getmember Name(target), value))
		/// =>
		/// dynamic.compound.op (dynamic.getmember Name(target), value)
		/// </summary>
		protected internal override void VisitDynamicSetMemberInstruction(DynamicSetMemberInstruction inst)
		{
			base.VisitDynamicSetMemberInstruction(inst);
			TransformDynamicSetMemberInstruction(inst, context);
		}

		internal static void TransformDynamicSetMemberInstruction(DynamicSetMemberInstruction inst, StatementTransformContext context)
		{
			if (!inst.BinderFlags.HasFlag(CSharpBinderFlags.ValueFromCompoundAssignment))
				return;
			if (!(inst.Value is DynamicBinaryOperatorInstruction binaryOp))
				return;
			if (!(binaryOp.Left is DynamicGetMemberInstruction dynamicGetMember))
				return;
			if (!dynamicGetMember.Target.Match(inst.Target).Success)
				return;
			if (!SemanticHelper.IsPure(dynamicGetMember.Target.Flags))
				return;
			if (inst.Name != dynamicGetMember.Name || !DynamicCompoundAssign.IsExpressionTypeSupported(binaryOp.Operation))
				return;
			context.Step("dynamic.setmember.compound -> dynamic.compound.op", inst);
			inst.ReplaceWith(new DynamicCompoundAssign(binaryOp.Operation, binaryOp.BinderFlags, binaryOp.Left, binaryOp.LeftArgumentInfo, binaryOp.Right, binaryOp.RightArgumentInfo));
		}

		/// <summary>
		/// dynamic.setindex.compound(target, index, dynamic.binary.operator op(dynamic.getindex(target, index), value))
		/// =>
		/// dynamic.compound.op (dynamic.getindex(target, index), value)
		/// </summary>
		protected internal override void VisitDynamicSetIndexInstruction(DynamicSetIndexInstruction inst)
		{
			base.VisitDynamicSetIndexInstruction(inst);

			if (!inst.BinderFlags.HasFlag(CSharpBinderFlags.ValueFromCompoundAssignment))
				return;
			if (!(inst.Arguments.LastOrDefault() is DynamicBinaryOperatorInstruction binaryOp))
				return;
			if (!(binaryOp.Left is DynamicGetIndexInstruction dynamicGetIndex))
				return;
			if (inst.Arguments.Count != dynamicGetIndex.Arguments.Count + 1)
				return;
			// Ensure that same arguments are passed to dynamicGetIndex and inst:
			for (int j = 0; j < dynamicGetIndex.Arguments.Count; j++)
			{
				if (!SemanticHelper.IsPure(dynamicGetIndex.Arguments[j].Flags))
					return;
				if (!dynamicGetIndex.Arguments[j].Match(inst.Arguments[j]).Success)
					return;
			}
			if (!DynamicCompoundAssign.IsExpressionTypeSupported(binaryOp.Operation))
				return;
			context.Step("dynamic.setindex.compound -> dynamic.compound.op", inst);
			inst.ReplaceWith(new DynamicCompoundAssign(binaryOp.Operation, binaryOp.BinderFlags, binaryOp.Left, binaryOp.LeftArgumentInfo, binaryOp.Right, binaryOp.RightArgumentInfo));
		}

		protected internal override void VisitBinaryNumericInstruction(BinaryNumericInstruction inst)
		{
			base.VisitBinaryNumericInstruction(inst);
			switch (inst.Operator)
			{
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
				case BinaryNumericOperator.BitAnd:
					if (inst.Left.InferType(context.TypeSystem).IsKnownType(KnownTypeCode.Boolean)
						&& inst.Right.InferType(context.TypeSystem).IsKnownType(KnownTypeCode.Boolean))
					{
						if (new NullableLiftingTransform(context).Run(inst))
						{
							// e.g. "(a.GetValueOrDefault() == b.GetValueOrDefault()) & (a.HasValue & b.HasValue)"
						}
					}
					break;
			}
		}

		protected internal override void VisitTryCatchHandler(TryCatchHandler inst)
		{
			base.VisitTryCatchHandler(inst);
			if (inst.Filter is BlockContainer filterContainer && filterContainer.Blocks.Count == 1)
			{
				TransformCatchWhen(inst, filterContainer.EntryPoint);
			}
			if (inst.Body is BlockContainer catchContainer)
				TransformCatchVariable(inst, catchContainer.EntryPoint, isCatchBlock: true);
		}

		/// <summary>
		/// catch ex : TException when (...) BlockContainer {
		/// 	Block entryPoint (incoming: 1) {
		/// 		stloc v(ldloc ex)
		/// 		...
		/// 	}
		/// }
		/// =>
		/// catch v : TException when (...) BlockContainer {
		/// 	Block entryPoint (incoming: 1) {
		/// 		...
		/// 	}
		/// }
		/// </summary>
		void TransformCatchVariable(TryCatchHandler handler, Block entryPoint, bool isCatchBlock)
		{
			if (!handler.Variable.IsSingleDefinition || handler.Variable.LoadCount != 1)
				return; // handler.Variable already has non-trivial uses
			if (!entryPoint.Instructions[0].MatchStLoc(out var exceptionVar, out var exceptionSlotLoad))
			{
				// Not the pattern with a second exceptionVar.
				// However, it is still possible that we need to remove a pointless UnboxAny:
				if (handler.Variable.LoadInstructions.Single().Parent is UnboxAny inlinedUnboxAny)
				{
					if (inlinedUnboxAny.Type.Equals(handler.Variable.Type))
					{
						context.Step("TransformCatchVariable - remove inlined UnboxAny", inlinedUnboxAny);
						inlinedUnboxAny.ReplaceWith(inlinedUnboxAny.Argument);
						foreach (var range in inlinedUnboxAny.ILRanges)
							handler.AddExceptionSpecifierILRange(range);
					}
				}
				return;
			}
			if (exceptionVar.Kind != VariableKind.Local && exceptionVar.Kind != VariableKind.StackSlot)
				return;
			if (exceptionSlotLoad is UnboxAny unboxAny)
			{
				// When catching a type parameter, csc emits an unbox.any instruction
				if (!unboxAny.Type.Equals(handler.Variable.Type))
					return;
				exceptionSlotLoad = unboxAny.Argument;
			}
			if (!exceptionSlotLoad.MatchLdLoc(handler.Variable))
				return;
			// Check that exceptionVar is only used within the catch block:
			var allUses = exceptionVar.LoadInstructions
				.Concat(exceptionVar.StoreInstructions.Cast<ILInstruction>())
				.Concat(exceptionVar.AddressInstructions);
			foreach (var inst in allUses)
			{
				if (!inst.IsDescendantOf(handler))
					return;
			}
			context.Step("TransformCatchVariable", entryPoint.Instructions[0]);
			exceptionVar.Kind = VariableKind.ExceptionLocal;
			exceptionVar.Type = handler.Variable.Type;
			handler.Variable = exceptionVar;
			if (isCatchBlock)
			{
				foreach (var offset in entryPoint.Instructions[0].Descendants.SelectMany(o => o.ILRanges))
					handler.AddExceptionSpecifierILRange(offset);
			}
			entryPoint.Instructions.RemoveAt(0);
		}

		/// <summary>
		/// Inline condition from catch-when condition BlockContainer, if possible.
		/// </summary>
		void TransformCatchWhen(TryCatchHandler handler, Block entryPoint)
		{
			TransformCatchVariable(handler, entryPoint, isCatchBlock: false);
			if (entryPoint.Instructions.Count == 1 && entryPoint.Instructions[0].MatchLeave(out _, out var condition))
			{
				context.Step("TransformCatchWhen", entryPoint.Instructions[0]);
				handler.Filter = condition;
			}
		}
	}
}
