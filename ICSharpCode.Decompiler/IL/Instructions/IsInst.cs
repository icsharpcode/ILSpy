#nullable enable
// Copyright (c) 2025 Daniel Grunwald
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

using ICSharpCode.Decompiler.CSharp;

namespace ICSharpCode.Decompiler.IL;

partial class IsInst
{
	internal override bool CanInlineIntoSlot(int childIndex, ILInstruction newChild)
	{
		Debug.Assert(childIndex == 0);
		Debug.Assert(base.CanInlineIntoSlot(childIndex, newChild));
		if (this.Type.IsReferenceType == true)
		{
			return true;  // reference-type isinst is always supported
		}
		if (SemanticHelper.IsPure(newChild.Flags))
		{
			return true; // emulated via "expr is T ? (T)expr : null"
		}
		else if (newChild is Box box && SemanticHelper.IsPure(box.Argument.Flags) && this.Argument.Children.Count == 0)
		{
			// Also emulated via "expr is T ? (T)expr : null".
			// This duplicates the boxing side-effect, but that's harmless as one of the boxes is only
			// used in the `expr is T` type test where the object identity can never be observed.
			// This appears as part of C# pattern matching, inlining early makes those code patterns easier to detect.

			// We can only do this if the Box appears directly top-level in the IsInst, we cannot inline Box instructions
			// deeper into our Argument subtree. So restricts to the case were the previous argument has no children
			// (which means inlining can only replace the argument, not insert within it).
			return true;
		}
		if (this.Parent is UnboxAny unboxAny && ExpressionBuilder.IsUnboxAnyWithIsInst(unboxAny, this.Type))
		{
			return true; // supported pattern "expr as T?"
		}
		if (this.Parent != null && (this.Parent.MatchCompEqualsNull(out _) || this.Parent.MatchCompNotEqualsNull(out _)))
		{
			return true; // supported pattern "expr is T"
		}
		if (this.Parent is Block { Kind: BlockKind.ControlFlow })
		{
			return true; // supported via StatementBuilder.VisitIsInst
		}
		return false;
	}
}