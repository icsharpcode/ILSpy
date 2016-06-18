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
		
		protected internal override void VisitCgt_Un(Cgt_Un inst)
		{
			base.VisitCgt_Un(inst);
			if (inst.Right.MatchLdNull()) {
				// cgt.un(left, ldnull)
				// => logic.not(ceq(left, ldnull))
				inst.ReplaceWith(new LogicNot(new Ceq(inst.Left, inst.Right) { ILRange = inst.ILRange }));
			}
			if (inst.Right.MatchLdcI4(0)) {
				// cgt.un(left, ldc.i4 0)
				// => logic.not(ceq(left, ldc.i4 0))
				ILInstruction array;
				if (inst.Left.MatchLdLen(StackType.I, out array)) {
					// cgt.un(ldlen array, ldc.i4 0)
					// => logic.not(ceq(ldlen.i4 array, ldc.i4 0))
					inst.Left.ReplaceWith(new LdLen(StackType.I4, array) { ILRange = inst.Left.ILRange });
				}
				inst.ReplaceWith(new LogicNot(new Ceq(inst.Left, inst.Right) { ILRange = inst.ILRange }));
			}
		}
		
		protected internal override void VisitClt_Un(Clt_Un inst)
		{
			base.VisitClt_Un(inst);
			if (inst.Left.MatchLdNull()) {
				// clt.un(ldnull, right)
				// => logic.not(ceq(ldnull, right))
				inst.ReplaceWith(new LogicNot(new Ceq(inst.Left, inst.Right) { ILRange = inst.ILRange }));
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
				LdcDecimal decimalConstant;
				ILInstruction result;
				if (TransformDecimalCtorToConstant(newObj, out decimalConstant)) {
					result = decimalConstant;
				} else {
					result = newObj;
				}
				var expr = new StObj(inst.Arguments[0], result, inst.Method.DeclaringType);
				inst.ReplaceWith(expr);
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
			ILVariable v;
			if (inst.Target.MatchLdLoca(out v) && TypeUtils.IsCompatibleTypeForMemoryAccess(new ByReferenceType(v.Type), inst.Type) && inst.UnalignedPrefix == 0 && !inst.IsVolatile) {
				inst.ReplaceWith(new StLoc(v, inst.Value.Clone()));
			}
			base.VisitStObj(inst);
		}
	}
}
