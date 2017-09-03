// Copyright (c) 2016 Daniel Grunwald
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

using ICSharpCode.Decompiler.FlowAnalysis;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Remove <c>HasInitialValue</c> from locals that are definitely assigned before every use
	/// (=the initial value is a dead store).
	/// 
	/// In yield return generators, additionally removes dead 'V = null;' assignments.
	/// </summary>
	public class RemoveDeadVariableInit : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			var visitor = new DefiniteAssignmentVisitor(function, context.CancellationToken);
			function.Body.AcceptVisitor(visitor);
			foreach (var v in function.Variables) {
				if (v.Kind != VariableKind.Parameter && !visitor.IsPotentiallyUsedUninitialized(v)) {
					v.HasInitialValue = false;
				}
			}
			if (function.IsIterator || function.IsAsync) {
				// In yield return + async, the C# compiler tends to store null/default(T) to variables
				// when the variable goes out of scope. Remove such useless stores.
				foreach (var v in function.Variables) {
					if (v.Kind == VariableKind.Local && v.StoreCount == 1 && v.LoadCount == 0 && v.AddressCount == 0) {
						if (v.StoreInstructions[0] is StLoc stloc && (stloc.Value.MatchLdNull() || stloc.Value is DefaultValue) && stloc.Parent is Block block) {
							block.Instructions.Remove(stloc);
						}
					}
				}
			}
		}
	}
}
