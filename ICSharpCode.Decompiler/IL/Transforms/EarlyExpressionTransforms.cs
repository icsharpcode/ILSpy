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

using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class EarlyExpressionTransforms : ILVisitor, IILTransform
	{
		ILTransformContext context;

		public void Run(ILFunction function, ILTransformContext context)
		{
			this.context = context;
			Default(function);
		}

		protected override void Default(ILInstruction inst)
		{
			foreach (var child in inst.Children)
			{
				child.AcceptVisitor(this);
			}
		}

		protected internal override void VisitComp(Comp inst)
		{
			base.VisitComp(inst);
			FixComparisonKindLdNull(inst, context);
		}

		internal static void FixComparisonKindLdNull(Comp inst, ILTransformContext context)
		{
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

			if (inst.Right.MatchLdNull() && inst.Left.MatchBox(out var arg, out var type) && type.Kind == TypeKind.TypeParameter)
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
				ILInstruction replacement = new StLoc(v, inst.Value).WithILRange(inst);
				if (v.StackType == StackType.Unknown && inst.Type.Kind != TypeKind.Unknown
					&& inst.SlotInfo != Block.InstructionSlot)
				{
					replacement = new Conv(replacement, inst.Type.ToPrimitiveType(),
						checkForOverflow: false, Sign.None);
				}
				inst.ReplaceWith(replacement);
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
				ILInstruction replacement = new LdLoc(v).WithILRange(inst);
				if (v.StackType == StackType.Unknown && inst.Type.Kind != TypeKind.Unknown)
				{
					replacement = new Conv(replacement, inst.Type.ToPrimitiveType(),
						checkForOverflow: false, Sign.None);
				}
				inst.ReplaceWith(replacement);
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
			while (temp.MatchLdFlda(out var ldfldaTarget, out _))
			{
				temp = ldfldaTarget;
				range = range.Concat(temp.ILRanges);
			}
			if (temp.MatchAddressOf(out var addressOfTarget, out _) && addressOfTarget.MatchLdLoc(out var v))
			{
				context.Step($"ldobj(...(addressof(ldloca {v.Name}))) => ldobj(...(ldloca {v.Name}))", inst);
				var replacement = new LdLoca(v).WithILRange(addressOfTarget);
				foreach (var r in range)
				{
					replacement = replacement.WithILRange(r);
				}
				temp.ReplaceWith(replacement);
			}
		}
	}
}
