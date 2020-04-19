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

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transform for the C# 8 System.Index / System.Range feature
	/// </summary>
	class IndexRangeTransform
	{
		/// <summary>
		/// Called by expression transforms.
		/// Handles the `array[System.Index]` cases.
		/// </summary>
		public static bool HandleLdElema(LdElema ldelema, ILTransformContext context)
		{
			if (!context.Settings.Ranges)
				return false;
			if (!ldelema.Array.MatchLdLoc(out ILVariable array))
				return false;
			if (ldelema.Indices.Count != 1)
				return false; // the index/range feature doesn't support multi-dimensional arrays
			var index = ldelema.Indices[0];
			if (index is CallInstruction call && call.Method.Name == "GetOffset" && call.Method.DeclaringType.IsKnownType(KnownTypeCode.Index)) {
				// ldelema T(ldloc array, call GetOffset(..., ldlen.i4(ldloc array)))
				// -> withsystemindex.ldelema T(ldloc array, ...)
				if (call.Arguments.Count != 2)
					return false;
				if (!(call.Arguments[1].MatchLdLen(StackType.I4, out var arrayLoad) && arrayLoad.MatchLdLoc(array)))
					return false;
				context.Step("ldelema with System.Index", ldelema);
				foreach (var node in call.Arguments[1].Descendants)
					ldelema.AddILRange(node);
				ldelema.AddILRange(call);
				ldelema.WithSystemIndex = true;
				// The method call had a `ref System.Index` argument for the this pointer, but we want a `System.Index` by-value.
				ldelema.Indices[0] = new LdObj(call.Arguments[0], call.Method.DeclaringType);
				return true;
			} else if (index is BinaryNumericInstruction bni && bni.Operator == BinaryNumericOperator.Sub && !bni.IsLifted && !bni.CheckForOverflow) {
				// ldelema T(ldloc array, binary.sub.i4(ldlen.i4(ldloc array), ...))
				// -> withsystemindex.ldelema T(ldloc array, newobj System.Index(..., fromEnd: true))
				if (!(bni.Left.MatchLdLen(StackType.I4, out var arrayLoad) && arrayLoad.MatchLdLoc(array)))
					return false;
				var indexCtor = FindIndexConstructor(context.TypeSystem);
				if (indexCtor == null)
					return false; // don't use System.Index if not supported by the target framework
				context.Step("ldelema indexed from end", ldelema);
				foreach (var node in bni.Left.Descendants)
					ldelema.AddILRange(node);
				ldelema.AddILRange(bni);
				ldelema.WithSystemIndex = true;
				ldelema.Indices[0] = new NewObj(indexCtor) { Arguments = { bni.Right, new LdcI4(1) } };
				return true;
			}

			return false;
		}

		static IMethod FindIndexConstructor(ICompilation compilation)
		{
			var indexType = compilation.FindType(KnownTypeCode.Index);
			foreach (var ctor in indexType.GetConstructors(m => m.Parameters.Count == 2)) {
				if (ctor.Parameters[0].Type.IsKnownType(KnownTypeCode.Int32)
					&& ctor.Parameters[1].Type.IsKnownType(KnownTypeCode.Boolean)) {
					return ctor;
				}
			}
			return null;
		}
	}
}
