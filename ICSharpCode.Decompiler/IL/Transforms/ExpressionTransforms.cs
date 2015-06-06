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
	/// 
	/// The transforms here do not open up new inlining opportunities.
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
			base.VisitCall(inst);
			if (!inst.Method.IsConstructor || inst.Method.DeclaringType.Kind != TypeKind.Struct)
				return;
			if (inst.Arguments.Count != inst.Method.Parameters.Count + 1)
				return;
			var newObj = new NewObj(inst.Method);
			newObj.Arguments.AddRange(inst.Arguments.Skip(1));
			var expr = new StObj(inst.Arguments[0], newObj, inst.Method.DeclaringType);
			inst.ReplaceWith(expr);
		}
	}
}
