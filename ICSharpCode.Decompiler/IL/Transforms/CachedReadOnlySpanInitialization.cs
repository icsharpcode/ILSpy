// Copyright (c) 2026 Sebastien Lebreton
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

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Collapses the compiler-generated lazy cache Roslyn emits for a <c>ReadOnlySpan&lt;T&gt;</c> that is
	/// created from an array literal on target frameworks without <c>RuntimeHelpers.CreateSpan</c> (e.g.
	/// .NET Framework or netstandard2.0 + System.Memory):
	/// <code>
	/// stloc V(ldobj T[](ldsflda &lt;PrivateImplementationDetails&gt;.cache))
	/// if (V == null) {
	///     stloc V(arrayInitializer)
	///     stobj T[](ldsflda &lt;PrivateImplementationDetails&gt;.cache, ldloc V)
	/// }
	/// ... single usage of V, e.g. newobj ReadOnlySpan&lt;T&gt;(ldloc V) ...
	/// </code>
	/// is turned into:
	/// <code>
	/// stloc V(arrayInitializer)
	/// ... single usage of V ...
	/// </code>
	/// Afterwards the existing array-initializer transforms recover the array literal, so the reference to
	/// the compiler-synthesized <c>&lt;PrivateImplementationDetails&gt;</c> cache field disappears. Without
	/// this transform the decompiled output references that field, whose escaped name
	/// (<c>&lt;PrivateImplementationDetails&gt;</c>) is not expressible in C# and is never declared, so the
	/// output does not recompile (CS0400).
	///
	/// This mirrors <see cref="CachedDelegateInitialization"/>, which collapses the analogous lazy cache for
	/// anonymous-method delegates, and therefore runs right after it.
	/// </summary>
	public class CachedReadOnlySpanInitialization : IBlockTransform
	{
		public void Run(Block block, BlockTransformContext context)
		{
			if (!context.Settings.ArrayInitializers)
				return;

			// The store that loads the cache field precedes the if, at block.Instructions[i - 1],
			// so there is nothing to match when i == 0.
			for (int i = context.IndexOfFirstAlreadyTransformedInstruction - 1; i >= 1; i--)
			{
				if (block.Instructions[i] is IfInstruction inst && DoTransform(block, i, inst, context))
				{
					context.IndexOfFirstAlreadyTransformedInstruction = block.Instructions.Count;
				}
			}
		}

		/// <summary>
		/// Matches
		/// <code>
		/// stloc V(ldobj(ldsflda cacheField))                       // block.Instructions[i - 1]
		/// if (comp(ldloc V == ldnull)) {                           // block.Instructions[i]
		///     stloc V(value)
		///     stobj(ldsflda cacheField, ldloc V)
		/// }
		/// </code>
		/// and replaces the load-from-cache with the initializer value, dropping the if:
		/// <code>
		/// stloc V(value)
		/// </code>
		/// </summary>
		static bool DoTransform(Block block, int i, IfInstruction inst, BlockTransformContext context)
		{
			// storeBeforeIf: stloc V(ldobj(ldsflda cacheField)), cacheField a compiler-generated static field.
			if (block.Instructions[i - 1] is not StLoc { Value: LdObj { Target: LdsFlda { Field: var cacheField } } } storeBeforeIf)
				return false;

			if (!cacheField.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;

			// V is assigned exactly twice (before-if load + in-if init) and read exactly three times
			// (null-check condition + cache write-back + one real downstream usage), with no address-of.
			var v = storeBeforeIf.Variable;
			if (v.StoreCount != 2 || v.LoadCount != 3 || v.AddressCount != 0)
				return false;

			// The if must be a simple `if (...) { ... }` (no else) with a two-instruction body.
			if (!inst.FalseInst.MatchNop() || inst.TrueInst is not Block trueBlock || trueBlock.Instructions.Count != 2)
				return false;

			// condition: V == null (MatchCompEqualsNull also accepts null == V and the negated forms).
			if (!inst.Condition.MatchCompEqualsNull(out var nullCheckArg) || !nullCheckArg.MatchLdLoc(v))
				return false;

			// trueBlock[0]: stloc V(value)
			if (trueBlock.Instructions[0] is not StLoc storeValue || storeValue.Variable != v)
				return false;

			// trueBlock[1]: stobj(ldsflda cacheField, ldloc V) -> the write-back to the same cache field.
			if (trueBlock.Instructions[1] is not StObj stobj || !stobj.Target.MatchLdsFlda(out var cacheField2)
				|| !cacheField.Equals(cacheField2) || !stobj.Value.MatchLdLoc(v))
			{
				return false;
			}

			context.Step("CachedReadOnlySpanInitialization", inst);
			storeBeforeIf.Value = storeValue.Value;
			block.Instructions.RemoveAt(i);
			return true;
		}
	}
}
