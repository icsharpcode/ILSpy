// Copyright (c) 2020 Daniel Grunwald
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
using System.Linq;
using ICSharpCode.Decompiler.CSharp;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// C# cannot represent `isinst T` directly for value-types.
	/// This transform un-inlines the argument of `isinst` instructions that can't be directly translated to C#,
	/// thus allowing the emulation via "expr is T ? (T)expr : null".
	/// </summary>
	class FixLoneIsInst : IILTransform
	{
		void IILTransform.Run(ILFunction function, ILTransformContext context)
		{
			var instructionsToFix = new List<IsInst>();
			foreach (var isInst in function.Descendants.OfType<IsInst>()) {
				if (isInst.Type.IsReferenceType == true) {
					continue;  // reference-type isinst is always supported
				}
				if (SemanticHelper.IsPure(isInst.Argument.Flags)) {
					continue; // emulated via "expr is T ? (T)expr : null"
				}
				if (isInst.Parent is UnboxAny unboxAny && ExpressionBuilder.IsUnboxAnyWithIsInst(unboxAny, isInst)) {
					continue; // supported pattern "expr as T?"
				}
				if (isInst.Parent.MatchCompEqualsNull(out _) || isInst.Parent.MatchCompNotEqualsNull(out _)) {
					continue; // supported pattern "expr is T"
				}
				if (isInst.Parent is Block { Kind: BlockKind.ControlFlow }) {
					continue; // supported via StatementBuilder.VisitIsInst
				}
				instructionsToFix.Add(isInst);
			}
			// Need to delay fixing until we're done with iteration, because Extract() modifies parents
			foreach (var isInst in instructionsToFix) {
				// Use extraction to turn isInst.Argument into a pure instruction, thus making the emulation possible
				context.Step("FixLoneIsInst", isInst);
				isInst.Argument.Extract();
			}
		}
	}
}
