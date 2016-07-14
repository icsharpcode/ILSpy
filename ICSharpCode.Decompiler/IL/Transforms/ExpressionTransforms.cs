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

using System;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Collection of transforms that detect simple expression patterns
	/// (e.g. 'cgt.un(..., ld.null)') and replace them with different instructions.
	/// </summary>
	/// <remarks>
	/// Should run after inlining so that the expression patterns can be detected.
	/// </remarks>
	public class ExpressionTransforms : ILVisitor, IILTransform
	{
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			function.AcceptVisitor(this);
		}
		
		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children) {
				child.AcceptVisitor(this);
			}
		}
		
		protected internal override void VisitComp(Comp inst)
		{
			base.VisitComp(inst);
			if (inst.Right.MatchLdNull()) {
				// comp(left > ldnull)  => comp(left != ldnull)
				// comp(left <= ldnull) => comp(left == ldnull)
				if (inst.Kind == ComparisonKind.GreaterThan)
					inst.Kind = ComparisonKind.Inequality;
				else if (inst.Kind == ComparisonKind.LessThanOrEqual)
					inst.Kind = ComparisonKind.Equality;
			} else if (inst.Left.MatchLdNull()) {
				// comp(ldnull < right)  => comp(ldnull != right)
				// comp(ldnull >= right) => comp(ldnull == right)
				if (inst.Kind == ComparisonKind.LessThan)
					inst.Kind = ComparisonKind.Inequality;
				else if (inst.Kind == ComparisonKind.GreaterThanOrEqual)
					inst.Kind = ComparisonKind.Equality;
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
					inst.Left.ReplaceWith(new LdLen(StackType.I4, array) { ILRange = inst.Left.ILRange });
					inst.InputType = StackType.I4;
					inst.Right = rightWithoutConv;
				}
				// comp.unsigned(left > ldc.i4 0) => comp(left != ldc.i4 0)
				// comp.unsigned(left <= ldc.i4 0) => comp(left == ldc.i4 0)
				if (inst.Kind == ComparisonKind.GreaterThan)
					inst.Kind = ComparisonKind.Inequality;
				else if (inst.Kind == ComparisonKind.LessThanOrEqual)
					inst.Kind = ComparisonKind.Equality;
			}
		}
		
		protected internal override void VisitConv(Conv inst)
		{
			inst.Argument.AcceptVisitor(this);
			ILInstruction array;
			if (inst.Argument.MatchLdLen(StackType.I, out array) && inst.TargetType.IsIntegerType() && !inst.CheckForOverflow) {
				// conv.i4(ldlen array) => ldlen.i4(array)
				inst.AddILRange(inst.Argument.ILRange);
				inst.ReplaceWith(new LdLen(inst.TargetType.GetStackType(), array) { ILRange = inst.ILRange });
			}
		}
		
		protected internal override void VisitLogicNot(LogicNot inst)
		{
			inst.Argument.AcceptVisitor(this);
			ILInstruction arg;
			if (inst.Argument.MatchLogicNot(out arg)) {
				// logic.not(logic.not(arg))
				// ==> arg
				Debug.Assert(arg.ResultType == StackType.I4);
				arg.AddILRange(inst.ILRange);
				arg.AddILRange(inst.Argument.ILRange);
				inst.ReplaceWith(arg);
			} else if (inst.Argument is Comp) {
				Comp comp = (Comp)inst.Argument;
				if (comp.InputType != StackType.F || comp.Kind.IsEqualityOrInequality()) {
					// push negation into comparison:
					comp.Kind = comp.Kind.Negate();
					comp.AddILRange(inst.ILRange);
					inst.ReplaceWith(comp);
				}
			}
		}
		
		protected internal override void VisitCall(Call inst)
		{
			if (inst.Method.IsConstructor && !inst.Method.IsStatic && inst.Method.DeclaringType.Kind == TypeKind.Struct) {
				Debug.Assert(inst.Arguments.Count == inst.Method.Parameters.Count + 1);
				// Transform call to struct constructor:
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
			ILVariable v;
			if (inst.Target.MatchLdLoca(out v)
			    && TypeUtils.IsCompatibleTypeForMemoryAccess(new ByReferenceType(v.Type), inst.Type)
			    && inst.UnalignedPrefix == 0
			    && !inst.IsVolatile)
			{
				// stobj(ldloca(v), ...)
				// => stloc(v, ...)
				inst.ReplaceWith(new StLoc(v, inst.Value));
			}
			
			ILInstruction target;
			IType t;
			BinaryNumericInstruction binary = inst.Value as BinaryNumericInstruction;
			if (binary != null && binary.Left.MatchLdObj(out target, out t) && IsSameTarget(inst.Target, target)) {
				// stobj(target, binary.op(ldobj(target), ...))
				// => compound.op(target, ...)
				inst.ReplaceWith(new CompoundAssignmentInstruction(binary.Operator, binary.Left, binary.Right, binary.CheckForOverflow, binary.Sign, CompoundAssignmentType.EvaluatesToNewValue));
			}
		}

		bool IsSameTarget(ILInstruction target, ILInstruction left)
		{
			IField f, f2;
			ILInstruction t, t2, a, a2;
			IType type, type2;
			if (target.MatchLdFlda(out t, out f) && left.MatchLdFlda(out t2, out f2) && f.Equals(f2))
				return true;
			// match ldelmena(ldobj(...))
			if (target.MatchLdElema(out type, out a) && left.MatchLdElema(out type2, out a2) && type.Equals(type2))
				return a.MatchLdObj(out target, out type) && a2.MatchLdObj(out left, out type2) && IsSameTarget(target, left);
			return false;
		}
		
		protected internal override void VisitIfInstruction(IfInstruction inst)
		{
			base.VisitIfInstruction(inst);
			// if (cond) stloc (A, V1) else stloc (A, V2) --> stloc (A, if (cond) V1 else V2)
			Block trueInst = inst.TrueInst as Block;
			if (trueInst == null || trueInst.Instructions.Count != 1)
				return;
			Block falseInst = inst.FalseInst as Block;
			if (falseInst == null || falseInst.Instructions.Count != 1)
				return;
			ILVariable v1, v2;
			ILInstruction value1, value2;
			if (trueInst.Instructions[0].MatchStLoc(out v1, out value1) && falseInst.Instructions[0].MatchStLoc(out v2, out value2) && v1 == v2) {
				inst.ReplaceWith(new StLoc(v1, new IfInstruction(inst.Condition, value1, value2)));
			}
		}
	}
}
