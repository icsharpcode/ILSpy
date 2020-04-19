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
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Transform for the C# 8 System.Index / System.Range feature
	/// </summary>
	class IndexRangeTransform : IStatementTransform
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

		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			int startPos = pos;
			// The container length access may be a separate instruction, or it may be inline with the variable's use
			if (MatchContainerLengthStore(block.Instructions[pos], out ILVariable containerLengthVar, out ILVariable containerVar)) {
				pos++;
			} else {
				// Reset if MatchContainerLengthStore only had a partial match. MatchGetOffset() will then set `containerVar`.
				containerLengthVar = null;
				containerVar = null;
			}
			var startIndexKind = MatchGetOffset(block.Instructions[pos], out ILVariable startOffsetVar, out ILInstruction startIndexLoad, containerLengthVar, ref containerVar);
			pos++;
			if (startIndexKind == IndexKind.None)
				return;
			if (startOffsetVar.LoadCount == 1) {
				// complex_expr(call get_Item(ldloc container, ldloc startOffsetVar))
				
				// startOffsetVar might be used deep inside a complex statement, ensure we can inline up to that point:
				for (int i = startPos; i < pos; i++) {
					if (!ILInlining.CanInlineInto(block.Instructions[pos], startOffsetVar, block.Instructions[i]))
						return;
				}
				if (!(startOffsetVar.LoadInstructions.Single().Parent is CallInstruction call))
					return;
				if (call.Method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter && call.Arguments.Count == 2) {
					if (call.Method.AccessorOwner?.SymbolKind != SymbolKind.Indexer)
						return;
					if (call.Method.Parameters.Count != 1)
						return;
				} else if (call.Method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Setter && call.Arguments.Count == 3) {
					if (call.Method.AccessorOwner?.SymbolKind != SymbolKind.Indexer)
						return;
					if (call.Method.Parameters.Count != 2)
						return;
				} else {
					return;
				}
				if (!call.Method.Parameters[0].Type.IsKnownType(KnownTypeCode.Int32))
					return;
				if (!call.Arguments[0].MatchLdLoc(containerVar) && !call.Arguments[0].MatchLdLoca(containerVar))
					return;
				if (!call.Arguments[1].MatchLdLoc(startOffsetVar))
					return;
				var indexType = context.TypeSystem.FindType(KnownTypeCode.Index);
				var indexCtor = FindIndexConstructor(context.TypeSystem);
				if (indexCtor == null)
					return;
				if (!CSharpWillGenerateIndexer(call.Method.DeclaringType))
					return;

				context.Step($"{call.Method.Name} indexed with {startIndexKind}", call);
				var newMethod = new SyntheticRangeIndexAccessor(call.Method, indexType);
				var newCall = CallInstruction.Create(call.OpCode, newMethod);
				newCall.ConstrainedTo = call.ConstrainedTo;
				newCall.ILStackWasEmpty = call.ILStackWasEmpty;
				newCall.Arguments.Add(call.Arguments[0]);
				if (startIndexKind == IndexKind.RefSystemIndex) {
					//  stloc length(call get_Length/get_Count(ldloc container))
					//  stloc startOffsetVar(call GetOffset(startIndexLoad, ldloc length))
					//  complex_expr(call get_Item(ldloc container, ldloc startOffsetVar))
					// -->
					//  complex_expr(call get_Item(ldloc container, ldobj startIndexLoad))
					newCall.Arguments.Add(new LdObj(startIndexLoad, indexType));
				} else {
					//  stloc offsetVar(binary.sub.i4(ldloc containerLengthVar, startIndexLoad))
					//  complex_expr(call get_Item(ldloc container, ldloc startOffsetVar))
					// -->
					//  complex_expr(call get_Item(ldloc container, newobj System.Index(startIndexLoad, fromEnd: true)))
					Debug.Assert(startIndexKind == IndexKind.FromEnd);
					newCall.Arguments.Add(new NewObj(indexCtor) { Arguments = { startIndexLoad, new LdcI4(1) } });
				}
				newCall.Arguments.AddRange(call.Arguments.Skip(2));
				newCall.AddILRange(call);
				for (int i = startPos; i < pos; i++) {
					newCall.AddILRange(block.Instructions[i]);
				}
				call.ReplaceWith(newCall);
				block.Instructions.RemoveRange(startPos, pos - startPos);
			}
		}

		/// <summary>
		/// Gets whether the C# compiler will call `container[int]` when using `container[Index]`.
		/// </summary>
		private bool CSharpWillGenerateIndexer(IType declaringType)
		{
			bool foundInt32Overload = false;
			bool foundIndexOverload = false;
			bool foundCountProperty = false;
			foreach (var prop in declaringType.GetProperties(p => p.IsIndexer || (p.Name == "Length" || p.Name == "Count"))) {
				if (prop.IsIndexer && prop.Parameters.Count == 1) {
					var p = prop.Parameters[0];
					if (p.Type.IsKnownType(KnownTypeCode.Int32)) {
						foundInt32Overload = true;
					} else if (p.Type.IsKnownType(KnownTypeCode.Index)) {
						foundIndexOverload = true;
					}
				} else if (prop.Name == "Length" || prop.Name=="Count") {
					foundCountProperty = true;
				}
			}
			return foundInt32Overload && foundCountProperty && !foundIndexOverload;
		}

		/// <summary>
		/// Matches the instruction:
		///    stloc containerLengthVar(call get_Length/get_Count(ldloc containerVar))
		/// </summary>
		static bool MatchContainerLengthStore(ILInstruction inst, out ILVariable lengthVar, out ILVariable containerVar)
		{
			containerVar = null;
			if (!inst.MatchStLoc(out lengthVar, out var init))
				return false;
			if (!(lengthVar.IsSingleDefinition && lengthVar.StackType == StackType.I4))
				return false;
			return MatchContainerLength(init, null, ref containerVar);
		}

		/// <summary>
		/// If lengthVar is non-null, matches 'ldloc lengthVar'.
		/// 
		///	Otherwise, matches the instruction:
		///		call get_Length/get_Count(ldloc containerVar)
		/// </summary>
		static bool MatchContainerLength(ILInstruction init, ILVariable lengthVar, ref ILVariable containerVar)
		{
			if (lengthVar != null) {
				Debug.Assert(containerVar != null);
				return init.MatchLdLoc(lengthVar);
			}
			if (!(init is CallInstruction call))
				return false;
			if (call.ResultType != StackType.I4)
				return false;
			if (!(call.Method.IsAccessor && call.Method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter))
				return false;
			if (!(call.Method.AccessorOwner is IProperty lengthProp))
				return false;
			if (lengthProp.Name == "Length") {
				// OK, Length is preferred
			} else if (lengthProp.Name == "Count") {
				// Also works, but only if the type doesn't have "Length"
				if (lengthProp.DeclaringType.GetProperties(p => p.Name == "Length").Any())
					return false;
			}
			if (!lengthProp.ReturnType.IsKnownType(KnownTypeCode.Int32))
				return false;
			if (lengthProp.IsVirtual && call.OpCode != OpCode.CallVirt)
				return false;
			if (call.Arguments.Count != 1)
				return false;
			if (containerVar != null) {
				return call.Arguments[0].MatchLdLoc(containerVar) || call.Arguments[0].MatchLdLoca(containerVar);
			} else {
				return call.Arguments[0].MatchLdLoc(out containerVar) || call.Arguments[0].MatchLdLoca(out containerVar);
			}
		}

		enum IndexKind
		{
			None,
			/// <summary>
			/// indexLoad is loading the address of a System.Index struct
			/// </summary>
			RefSystemIndex,
			/// <summary>
			/// indexLoad is an integer, from the end of the container
			/// </summary>
			FromEnd
		}

		/// <summary>
		/// Matches an instruction computing an offset:
		///    stloc offsetVar(call System.Index.GetOffset(indexLoad, ldloc containerLengthVar))
		/// or
		///    stloc offsetVar(binary.sub.i4(ldloc containerLengthVar, indexLoad))
		/// </summary>
		static IndexKind MatchGetOffset(ILInstruction inst, out ILVariable offsetVar, out ILInstruction indexLoad,
			ILVariable containerLengthVar, ref ILVariable containerVar)
		{
			indexLoad = null;
			if (!inst.MatchStLoc(out offsetVar, out var offsetValue))
				return IndexKind.None;
			if (!(offsetVar.IsSingleDefinition && offsetVar.StackType == StackType.I4))
				return IndexKind.None;
			if (offsetValue is CallInstruction call) {
				// call System.Index.GetOffset(indexLoad, ldloc containerLengthVar)
				if (call.Method.Name != "GetOffset")
					return IndexKind.None;
				if (!call.Method.DeclaringType.IsKnownType(KnownTypeCode.Index))
					return IndexKind.None;
				if (call.Arguments.Count != 2)
					return IndexKind.None;
				if (!MatchContainerLength(call.Arguments[1], containerLengthVar, ref containerVar))
					return IndexKind.None;
				indexLoad = call.Arguments[0];
				return IndexKind.RefSystemIndex;
			} else if (offsetValue is BinaryNumericInstruction bni && bni.Operator == BinaryNumericOperator.Sub) {
				if (bni.CheckForOverflow || bni.ResultType != StackType.I4 || bni.IsLifted)
					return IndexKind.None;
				// binary.sub.i4(ldloc containerLengthVar, indexLoad)
				if (!MatchContainerLength(bni.Left, containerLengthVar, ref containerVar))
					return IndexKind.None;
				indexLoad = bni.Right;
				return IndexKind.FromEnd;
			} else {
				return IndexKind.None;
			}
		}
	}
}
