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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Inline compiler-generated variables.
	/// Unlike normal inlining that acts on stack variables which have only one use,
	/// this transform will also perform inlining if the variables are re-used.
	/// </summary>
	/// <remarks>
	/// Should run after ControlFlowSimplification duplicates the return blocks.
	/// Should run after a regular round of inlining, so that stack variables
	/// don't prevent us from determining whether a variable is likely to be compiler-generated.
	/// </remarks>
	public class InlineCompilerGeneratedVariables : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			List<StLoc> storesToInline = new List<StLoc>();
			List<StLoc> deadStores = new List<StLoc>();
			foreach (var g in function.Descendants.OfType<StLoc>().GroupBy(inst => inst.Variable)) {
				storesToInline.Clear();
				deadStores.Clear();
				if (CanInlineVariable(g.Key, g, storesToInline, deadStores)) {
					foreach (var stloc in storesToInline) {
						ILInlining.InlineOne(stloc, aggressive: false, context: context);
					}
					foreach (var stloc in deadStores) {
						if (SemanticHelper.IsPure(stloc.Flags)) {
							((Block)stloc.Parent).Instructions.RemoveAt(stloc.ChildIndex);
						}
					}
				}
			}
		}

		bool CanInlineVariable(ILVariable v, IEnumerable<StLoc> stores, /*out*/ List<StLoc> storesToInline, /*out*/ List<StLoc> deadStores)
		{
			if (v.Kind != VariableKind.Local) {
				return false;
			}
			if (v.HasInitialValue) {
				return false; // cannot handle variables that are implicitly initialized at the beginning of the function
			}
			Debug.Assert(v.StoreCount == stores.Count());
			// We expect there to be one store for every load,
			// and potentially also some dead stores.
			if (v.StoreCount < v.LoadCount || v.AddressCount != 0)
				return false;
			int loadsAccountedFor = 0;
			foreach (var stloc in stores) {
				Block block = stloc.Parent as Block;
				if (block == null)
					return false;
				ILInstruction next = block.Instructions.ElementAtOrDefault(stloc.ChildIndex + 1);
				// NB: next==null is considerd as a dead store
				if (ILInlining.CanInlineInto(next, v, stloc.Value)) {
					loadsAccountedFor++;
					storesToInline.Add(stloc);
				} else {
					// a dead store (but only if this method returns true)
					deadStores.Add(stloc);
				}
			}
			return v.LoadCount == loadsAccountedFor;
		}
	}
}
