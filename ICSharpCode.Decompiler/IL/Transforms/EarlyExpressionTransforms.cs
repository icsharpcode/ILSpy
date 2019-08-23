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
using System.Text;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	class EarlyExpressionTransforms : ILVisitor, IILTransform
	{
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			Default(function);
		}

		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children) {
				child.AcceptVisitor(this);
			}
		}

		protected internal override void VisitStObj(StObj inst)
		{
			base.VisitStObj(inst);
			StObjToStLoc(inst, context);
		}
		
		// This transform is required because ILInlining only works with stloc/ldloc
		internal static bool StObjToStLoc(StObj inst, ILTransformContext context)
		{
			if (inst.Target.MatchLdLoca(out ILVariable v)
				&& TypeUtils.IsCompatibleTypeForMemoryAccess(v.Type, inst.Type)
				&& inst.UnalignedPrefix == 0
				&& !inst.IsVolatile)
			{
				context.Step($"stobj(ldloca {v.Name}, ...) => stloc {v.Name}(...)", inst);
				inst.ReplaceWith(new StLoc(v, inst.Value).WithILRange(inst));
				return true;
			}
			return false;
		}

		protected internal override void VisitLdObj(LdObj inst)
		{
			base.VisitLdObj(inst);
			AddressOfLdLocToLdLoca(inst, context);
			LdObjToLdLoc(inst, context);
		}

		internal static bool LdObjToLdLoc(LdObj inst, ILTransformContext context)
		{
			if (inst.Target.MatchLdLoca(out ILVariable v)
				&& TypeUtils.IsCompatibleTypeForMemoryAccess(v.Type, inst.Type)
				&& inst.UnalignedPrefix == 0
				&& !inst.IsVolatile)
			{
				context.Step($"ldobj(ldloca {v.Name}) => ldloc {v.Name}", inst);
				inst.ReplaceWith(new LdLoc(v).WithILRange(inst));
				return true;
			}
			return false;
		}

		internal static void AddressOfLdLocToLdLoca(LdObj inst, ILTransformContext context)
		{
			// ldobj(...(addressof(ldloc V))) where ... can be zero or more ldflda instructions
			// =>
			// ldobj(...(ldloca V))
			var temp = inst.Target;
			var range = temp.ILRanges;
			while (temp.MatchLdFlda(out var ldfldaTarget, out _)) {
				temp = ldfldaTarget;
				range = range.Concat(temp.ILRanges);
			}
			if (temp.MatchAddressOf(out var addressOfTarget, out _) && addressOfTarget.MatchLdLoc(out var v)) {
				context.Step($"ldobj(...(addressof(ldloca {v.Name}))) => ldobj(...(ldloca {v.Name}))", inst);
				var replacement = new LdLoca(v).WithILRange(addressOfTarget);
				foreach (var r in range) {
					replacement = replacement.WithILRange(r);
				}
				temp.ReplaceWith(replacement);
			}
		}

		protected internal override void VisitCall(Call inst)
		{
			var expr = HandleCall(inst, context);
			if (expr != null) {
				// The resulting expression may trigger further rules, so continue visiting the replacement:
				expr.AcceptVisitor(this);
			} else {
				base.VisitCall(inst);
			}
		}

		internal static ILInstruction HandleCall(Call inst, ILTransformContext context)
		{
			if (inst.Method.IsConstructor && !inst.Method.IsStatic && inst.Method.DeclaringType.Kind == TypeKind.Struct) {
				Debug.Assert(inst.Arguments.Count == inst.Method.Parameters.Count + 1);
				context.Step("Transform call to struct constructor", inst);
				// call(ref, ...)
				// => stobj(ref, newobj(...))
				var newObj = new NewObj(inst.Method);
				newObj.AddILRange(inst);
				newObj.Arguments.AddRange(inst.Arguments.Skip(1));
				newObj.ILStackWasEmpty = inst.ILStackWasEmpty;
				var expr = new StObj(inst.Arguments[0], newObj, inst.Method.DeclaringType);
				inst.ReplaceWith(expr);
				return expr;
			}
			return null;
		}
	}
}
