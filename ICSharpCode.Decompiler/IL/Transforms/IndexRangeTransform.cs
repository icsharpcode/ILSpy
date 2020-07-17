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
				var indexMethods = new IndexMethods(context.TypeSystem);
				if (!indexMethods.IsValid)
					return false; // don't use System.Index if not supported by the target framework
				context.Step("ldelema indexed from end", ldelema);
				foreach (var node in bni.Left.Descendants)
					ldelema.AddILRange(node);
				ldelema.AddILRange(bni);
				ldelema.WithSystemIndex = true;
				ldelema.Indices[0] = MakeIndex(IndexKind.FromEnd, bni.Right, indexMethods);
				return true;
			}

			return false;
		}

		class IndexMethods
		{
			public readonly IMethod IndexCtor;
			public readonly IMethod IndexImplicitConv;
			public readonly IMethod RangeCtor;
			public IType IndexType => IndexCtor?.DeclaringType;
			public IType RangeType => RangeCtor?.DeclaringType;
			public bool IsValid => IndexCtor != null && IndexImplicitConv != null && RangeCtor != null;

			public readonly IMethod RangeStartAt;
			public readonly IMethod RangeEndAt;
			public readonly IMethod RangeGetAll;

			public IndexMethods(ICompilation compilation)
			{
				var indexType = compilation.FindType(KnownTypeCode.Index);
				foreach (var ctor in indexType.GetConstructors(m => m.Parameters.Count == 2)) {
					if (ctor.Parameters[0].Type.IsKnownType(KnownTypeCode.Int32)
						&& ctor.Parameters[1].Type.IsKnownType(KnownTypeCode.Boolean)) {
						this.IndexCtor = ctor;
					}
				}
				foreach (var op in indexType.GetMethods(m => m.IsOperator && m.Name == "op_Implicit")) {
					if (op.Parameters.Count == 1 && op.Parameters[0].Type.IsKnownType(KnownTypeCode.Int32)) {
						this.IndexImplicitConv = op;
					}
				}
				var rangeType = compilation.FindType(KnownTypeCode.Range);
				foreach (var ctor in rangeType.GetConstructors(m => m.Parameters.Count == 2)) {
					if (ctor.Parameters[0].Type.IsKnownType(KnownTypeCode.Index)
						&& ctor.Parameters[1].Type.IsKnownType(KnownTypeCode.Index)) {
						this.RangeCtor = ctor;
					}
				}
				foreach (var m in rangeType.GetMethods(m => m.Parameters.Count == 1)) {
					if (m.Parameters.Count == 1 && m.Parameters[0].Type.IsKnownType(KnownTypeCode.Index)) {
						if (m.Name == "StartAt")
							this.RangeStartAt = m;
						else if (m.Name == "EndAt")
							this.RangeEndAt = m;
					}
				}
				foreach (var p in rangeType.GetProperties(p => p.IsStatic && p.Name == "All")) {
					this.RangeGetAll = p.Getter;
				}
			}

			public static bool IsRangeCtor(IMethod method)
			{
				return method.SymbolKind == SymbolKind.Constructor
					&& method.Parameters.Count == 2
					&& method.DeclaringType.IsKnownType(KnownTypeCode.Range)
					&& method.Parameters.All(p => p.Type.IsKnownType(KnownTypeCode.Index));
			}
		}

		void IStatementTransform.Run(Block block, int pos, StatementTransformContext context)
		{
			if (!context.Settings.Ranges)
				return;
			int startPos = pos;
			ILVariable containerVar = null;
			// The container length access may be a separate instruction, or it may be inline with the variable's use
			if (MatchContainerLengthStore(block.Instructions[pos], out ILVariable containerLengthVar, ref containerVar)) {
				//  stloc containerLengthVar(call get_Length/get_Count(ldloc container))
				pos++;
			} else {
				// Reset if MatchContainerLengthStore only had a partial match. MatchGetOffset() will then set `containerVar`.
				containerLengthVar = null;
				containerVar = null;
			}
			if (block.Instructions[pos].MatchStLoc(out var rangeVar, out var rangeVarInit) && rangeVar.Type.IsKnownType(KnownTypeCode.Range)) {
				// stloc rangeVar(rangeVarInit)
				pos++;
			} else {
				rangeVar = null;
				rangeVarInit = null;
			}

			// stloc startOffsetVar(call GetOffset(startIndexLoad, ldloc length))
			if (!block.Instructions[pos].MatchStLoc(out ILVariable startOffsetVar, out ILInstruction startOffsetVarInit)) {
				// Not our primary indexing/slicing pattern.
				// However, we might be dealing with a partially-transformed pattern that needs to be extended.
				ExtendSlicing();
				return;
			}
			if (!(startOffsetVar.IsSingleDefinition && startOffsetVar.StackType == StackType.I4))
				return;
			var startIndexKind = MatchGetOffset(startOffsetVarInit, out ILInstruction startIndexLoad, containerLengthVar, ref containerVar);
			pos++;
			if (startOffsetVar.LoadCount == 1) {
				TransformIndexing();
			} else if (startOffsetVar.LoadCount == 2) {
				// might be slicing: startOffset is used once for the slice length calculation, and once for the Slice() method call
				TransformSlicing();
			}

			void TransformIndexing()
			{
				// complex_expr(call get_Item(ldloc container, ldloc startOffsetVar))

				if (rangeVar != null)
					return;
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
				} else if (IsSlicingMethod(call.Method)) {
					TransformSlicing(sliceLengthWasMisdetectedAsStartOffset: true);
					return;
				} else {
					return;
				}
				if (startIndexKind == IndexKind.FromStart) {
					// FromStart is only relevant for slicing; indexing from the start does not involve System.Index at all.
					return;
				}
				if (!CheckContainerLengthVariableUseCount(containerLengthVar, startIndexKind)) {
					return;
				}
				// startOffsetVar might be used deep inside a complex statement, ensure we can inline up to that point:
				for (int i = startPos; i < pos; i++) {
					if (!ILInlining.CanInlineInto(block.Instructions[pos], startOffsetVar, block.Instructions[i]))
						return;
				}
				if (!call.Method.Parameters[0].Type.IsKnownType(KnownTypeCode.Int32))
					return;
				if (!MatchContainerVar(call.Arguments[0], ref containerVar))
					return;
				if (!call.Arguments[1].MatchLdLoc(startOffsetVar))
					return;
				var specialMethods = new IndexMethods(context.TypeSystem);
				if (!specialMethods.IsValid)
					return;
				if (!CSharpWillGenerateIndexer(call.Method.DeclaringType, slicing: false))
					return;

				context.Step($"{call.Method.Name} indexed with {startIndexKind}", call);
				var newMethod = new SyntheticRangeIndexAccessor(call.Method, specialMethods.IndexType, slicing: false);
				var newCall = CallInstruction.Create(call.OpCode, newMethod);
				newCall.ConstrainedTo = call.ConstrainedTo;
				newCall.ILStackWasEmpty = call.ILStackWasEmpty;
				newCall.Arguments.Add(call.Arguments[0]);
				newCall.Arguments.Add(MakeIndex(startIndexKind, startIndexLoad, specialMethods));
				newCall.Arguments.AddRange(call.Arguments.Skip(2));
				newCall.AddILRange(call);
				for (int i = startPos; i < pos; i++) {
					newCall.AddILRange(block.Instructions[i]);
				}
				call.ReplaceWith(newCall);
				block.Instructions.RemoveRange(startPos, pos - startPos);
			}

			void TransformSlicing(bool sliceLengthWasMisdetectedAsStartOffset = false)
			{
				ILVariable sliceLengthVar;
				ILInstruction sliceLengthVarInit;
				if (sliceLengthWasMisdetectedAsStartOffset) {
					// Special case: when slicing without a start point, the slice length calculation is mis-detected as the start offset,
					// and since it only has a single use, we end in TransformIndexing(), which then calls TransformSlicing
					// on this code path.
					sliceLengthVar = startOffsetVar;
					sliceLengthVarInit = ((StLoc)sliceLengthVar.StoreInstructions.Single()).Value;
					startOffsetVar = null;
					startIndexLoad = new LdcI4(0);
					startIndexKind = IndexKind.TheStart;
				} else {
					// stloc containerLengthVar(call get_Length(ldloc containerVar))
					// stloc startOffset(call GetOffset(startIndexLoad, ldloc length))
					// -- we are here --
					// stloc sliceLengthVar(binary.sub.i4(call GetOffset(endIndexLoad, ldloc length), ldloc startOffset))
					// complex_expr(call Slice(ldloc containerVar, ldloc startOffset, ldloc sliceLength))

					if (!block.Instructions[pos].MatchStLoc(out sliceLengthVar, out sliceLengthVarInit))
						return;
					pos++;
				}

				if (!(sliceLengthVar.IsSingleDefinition && sliceLengthVar.LoadCount == 1))
					return;
				if (!MatchSliceLength(sliceLengthVarInit, out IndexKind endIndexKind, out ILInstruction endIndexLoad, containerLengthVar, ref containerVar, startOffsetVar))
					return;
				if (!CheckContainerLengthVariableUseCount(containerLengthVar, startIndexKind, endIndexKind)) {
					return;
				}
				if (rangeVar != null) {
					return; // this should only ever happen in the second step (ExtendSlicing)
				}
				if (!(sliceLengthVar.LoadInstructions.Single().Parent is CallInstruction call))
					return;
				if (!IsSlicingMethod(call.Method))
					return;
				if (call.Arguments.Count != 3)
					return;
				if (!MatchContainerVar(call.Arguments[0], ref containerVar))
					return;
				if (startOffsetVar == null) {
					Debug.Assert(startIndexKind == IndexKind.TheStart);
					if (!call.Arguments[1].MatchLdcI4(0))
						return;
				} else {
					if (!call.Arguments[1].MatchLdLoc(startOffsetVar))
						return;
					if (!ILInlining.CanMoveInto(startOffsetVarInit, block.Instructions[pos], call.Arguments[1]))
						return;
				}
				if (!call.Arguments[2].MatchLdLoc(sliceLengthVar))
					return;
				if (!ILInlining.CanMoveInto(sliceLengthVarInit, block.Instructions[pos], call.Arguments[2]))
					return;
				if (!CSharpWillGenerateIndexer(call.Method.DeclaringType, slicing: true))
					return;
				var specialMethods = new IndexMethods(context.TypeSystem);
				if (!specialMethods.IsValid)
					return;

				context.Step($"{call.Method.Name} sliced with {startIndexKind}..{endIndexKind}", call);
				var newMethod = new SyntheticRangeIndexAccessor(call.Method, specialMethods.RangeType, slicing: true);
				var newCall = CallInstruction.Create(call.OpCode, newMethod);
				newCall.ConstrainedTo = call.ConstrainedTo;
				newCall.ILStackWasEmpty = call.ILStackWasEmpty;
				newCall.Arguments.Add(call.Arguments[0]);
				newCall.Arguments.Add(MakeRange(startIndexKind, startIndexLoad, endIndexKind, endIndexLoad, specialMethods));
				newCall.AddILRange(call);
				for (int i = startPos; i < pos; i++) {
					newCall.AddILRange(block.Instructions[i]);
				}
				call.ReplaceWith(newCall);
				block.Instructions.RemoveRange(startPos, pos - startPos);
			}

			ILInstruction MakeRange(IndexKind startIndexKind, ILInstruction startIndexLoad, IndexKind endIndexKind, ILInstruction endIndexLoad, IndexMethods specialMethods)
			{
				if (rangeVar != null) {
					return rangeVarInit;
				} else if (startIndexKind == IndexKind.TheStart && endIndexKind == IndexKind.TheEnd && specialMethods.RangeGetAll != null) {
					return new Call(specialMethods.RangeGetAll);
				} else if (startIndexKind == IndexKind.TheStart && specialMethods.RangeEndAt != null) {
					var rangeCtorCall = new Call(specialMethods.RangeEndAt);
					rangeCtorCall.Arguments.Add(MakeIndex(endIndexKind, endIndexLoad, specialMethods));
					return rangeCtorCall;
				} else if (endIndexKind == IndexKind.TheEnd && specialMethods.RangeStartAt != null) {
					var rangeCtorCall = new Call(specialMethods.RangeStartAt);
					rangeCtorCall.Arguments.Add(MakeIndex(startIndexKind, startIndexLoad, specialMethods));
					return rangeCtorCall;
				} else {
					var rangeCtorCall = new NewObj(specialMethods.RangeCtor);
					rangeCtorCall.Arguments.Add(MakeIndex(startIndexKind, startIndexLoad, specialMethods));
					rangeCtorCall.Arguments.Add(MakeIndex(endIndexKind, endIndexLoad, specialMethods));
					return rangeCtorCall;
				}
			}

			void ExtendSlicing()
			{
				// We might be dealing with a situation where we executed TransformSlicing() in a previous run of this transform
				// that only looked at a part of the instructions making up the slicing pattern.
				// The first run would have mis-detected slicing from end as slicing from start.
				// This results in code like:
				//   int length = span.Length;
				//   Console.WriteLine(span[GetIndex(1).GetOffset(length)..GetIndex(2).GetOffset(length)].ToString());
				// or:
				//   int length = span.Length;
				//   Range range = GetRange();
				//   Console.WriteLine(span[range.Start.GetOffset(length)..range.End.GetOffset(length)].ToString());
				if (containerLengthVar == null) {
					return; // need a container length to extend with
				}
				Debug.Assert(containerLengthVar.IsSingleDefinition);
				Debug.Assert(containerLengthVar.LoadCount == 1 || containerLengthVar.LoadCount == 2);
				NewObj rangeCtorCall = null;
				foreach (var inst in containerLengthVar.LoadInstructions[0].Ancestors) {
					if (inst is NewObj newobj && IndexMethods.IsRangeCtor(newobj.Method)) {
						rangeCtorCall = newobj;
						break;
					}
					if (inst == block)
						break;
				}
				if (rangeCtorCall == null)
					return;
				// Now match the pattern that TransformSlicing() generated in the IndexKind.FromStart case
				if (!(rangeCtorCall.Parent is CallInstruction { Method: SyntheticRangeIndexAccessor _ } slicingCall))
					return;
				if (!MatchContainerVar(slicingCall.Arguments[0], ref containerVar))
					return;
				if (!slicingCall.IsDescendantOf(block.Instructions[pos]))
					return;
				Debug.Assert(rangeCtorCall.Arguments.Count == 2);
				if (!MatchIndexImplicitConv(rangeCtorCall.Arguments[0], out var startOffsetInst))
					return;
				if (!MatchIndexImplicitConv(rangeCtorCall.Arguments[1], out var endOffsetInst))
					return;
				var startIndexKind = MatchGetOffset(startOffsetInst, out var startIndexLoad, containerLengthVar, ref containerVar);
				var endIndexKind = MatchGetOffset(endOffsetInst, out var endIndexLoad, containerLengthVar, ref containerVar);
				if (!CheckContainerLengthVariableUseCount(containerLengthVar, startIndexKind, endIndexKind)) {
					return;
				}
				// holds because we've used containerLengthVar at least once
				Debug.Assert(startIndexKind != IndexKind.FromStart || endIndexKind != IndexKind.FromStart);
				if (rangeVar != null) {
					if (!ILInlining.CanMoveInto(rangeVarInit, block.Instructions[pos], startIndexLoad))
						return;
					if (!MatchIndexFromRange(startIndexKind, startIndexLoad, rangeVar, "get_Start"))
						return;
					if (!MatchIndexFromRange(endIndexKind, endIndexLoad, rangeVar, "get_End"))
						return;
				}
				var specialMethods = new IndexMethods(context.TypeSystem);
				if (!specialMethods.IsValid)
					return;
				context.Step("Merge containerLengthVar into slicing", slicingCall);
				rangeCtorCall.ReplaceWith(MakeRange(startIndexKind, startIndexLoad, endIndexKind, endIndexLoad, specialMethods));
				for (int i = startPos; i < pos; i++) {
					slicingCall.AddILRange(block.Instructions[i]);
				}
				block.Instructions.RemoveRange(startPos, pos - startPos);
			}
		}

		private bool MatchIndexImplicitConv(ILInstruction inst, out ILInstruction offsetInst)
		{
			offsetInst = null;
			if (!(inst is CallInstruction call))
				return false;
			if (!(call.Method.IsOperator && call.Method.Name == "op_Implicit"))
				return false;
			var op = call.Method;
			if (!(op.Parameters.Count == 1 && op.Parameters[0].Type.IsKnownType(KnownTypeCode.Int32)))
				return false;
			if (!op.DeclaringType.IsKnownType(KnownTypeCode.Index))
				return false;
			offsetInst = call.Arguments.Single();
			return true;
		}

		static bool IsSlicingMethod(IMethod method)
		{
			if (method.IsExtensionMethod)
				return false;
			if (method.Parameters.Count != 2)
				return false;
			if (!method.Parameters.All(p => p.Type.IsKnownType(KnownTypeCode.Int32)))
				return false;
			return method.Name == "Slice"
				|| (method.Name == "Substring" && method.DeclaringType.IsKnownType(KnownTypeCode.String));
		}

		/// <summary>
		/// Check that the number of uses of the containerLengthVar variable matches those expected in the pattern.
		/// </summary>
		private bool CheckContainerLengthVariableUseCount(ILVariable containerLengthVar, IndexKind startIndexKind, IndexKind endIndexKind = IndexKind.FromStart)
		{
			int expectedUses = 0;
			if (startIndexKind != IndexKind.FromStart && startIndexKind != IndexKind.TheStart)
				expectedUses += 1;
			if (endIndexKind != IndexKind.FromStart && endIndexKind != IndexKind.TheStart)
				expectedUses += 1;
			if (containerLengthVar != null) {
				return containerLengthVar.LoadCount == expectedUses;
			} else {
				return expectedUses <= 1; // can have one inline use
			}
		}

		/// <summary>
		/// Matches 'addressof System.Index(call get_Start/get_End(ldloca rangeVar))'
		/// </summary>
		static bool MatchIndexFromRange(IndexKind indexKind, ILInstruction indexLoad, ILVariable rangeVar, string accessorName)
		{
			if (indexKind != IndexKind.RefSystemIndex)
				return false;
			if (!(indexLoad is AddressOf addressOf))
				return false;
			if (!addressOf.Type.IsKnownType(KnownTypeCode.Index))
				return false;
			if (!(addressOf.Value is Call call))
				return false;
			if (call.Method.Name != accessorName)
				return false;
			if (!call.Method.DeclaringType.IsKnownType(KnownTypeCode.Range))
				return false;
			if (call.Arguments.Count != 1)
				return false;
			return call.Arguments[0].MatchLdLoca(rangeVar);
		}

		static ILInstruction MakeIndex(IndexKind indexKind, ILInstruction indexLoad, IndexMethods specialMethods)
		{
			if (indexKind == IndexKind.RefSystemIndex) {
				//  stloc containerLengthVar(call get_Length/get_Count(ldloc container))
				//  stloc startOffsetVar(call GetOffset(startIndexLoad, ldloc length))
				//  complex_expr(call get_Item(ldloc container, ldloc startOffsetVar))
				// -->
				//  complex_expr(call get_Item(ldloc container, ldobj startIndexLoad))
				return new LdObj(indexLoad, specialMethods.IndexType);
			} else if (indexKind == IndexKind.FromEnd || indexKind == IndexKind.TheEnd) {
				//  stloc offsetVar(binary.sub.i4(call get_Length/get_Count(ldloc container), startIndexLoad))
				//  complex_expr(call get_Item(ldloc container, ldloc startOffsetVar))
				// -->
				//  complex_expr(call get_Item(ldloc container, newobj System.Index(startIndexLoad, fromEnd: true)))
				return new NewObj(specialMethods.IndexCtor) { Arguments = { indexLoad, new LdcI4(1) } };
			} else {
				Debug.Assert(indexKind == IndexKind.FromStart || indexKind == IndexKind.TheStart);
				return new Call(specialMethods.IndexImplicitConv) { Arguments = { indexLoad } };
			}
		}

		/// <summary>
		/// Gets whether the C# compiler will call `container[int]` when using `container[Index]`.
		/// </summary>
		private bool CSharpWillGenerateIndexer(IType declaringType, bool slicing)
		{
			bool foundInt32Overload = false;
			bool foundIndexOverload = false;
			bool foundRangeOverload = false;
			bool foundCountProperty = false;
			foreach (var prop in declaringType.GetProperties(p => p.IsIndexer || (p.Name == "Length" || p.Name == "Count"))) {
				if (prop.IsIndexer && prop.Parameters.Count == 1) {
					var p = prop.Parameters[0];
					if (p.Type.IsKnownType(KnownTypeCode.Int32)) {
						foundInt32Overload = true;
					} else if (p.Type.IsKnownType(KnownTypeCode.Index)) {
						foundIndexOverload = true;
					} else if (p.Type.IsKnownType(KnownTypeCode.Range)) {
						foundRangeOverload = true;
					}
				} else if (prop.Name == "Length" || prop.Name == "Count") {
					foundCountProperty = true;
				}
			}
			if (slicing) {
				return /* foundSlicingMethod && */ foundCountProperty && !foundRangeOverload;
			} else {
				return foundInt32Overload && foundCountProperty && !foundIndexOverload;
			}
		}

		/// <summary>
		/// Matches the instruction:
		///    stloc containerLengthVar(call get_Length/get_Count(ldloc containerVar))
		/// </summary>
		static bool MatchContainerLengthStore(ILInstruction inst, out ILVariable lengthVar, ref ILVariable containerVar)
		{
			if (!inst.MatchStLoc(out lengthVar, out var init))
				return false;
			if (!(lengthVar.IsSingleDefinition && lengthVar.StackType == StackType.I4))
				return false;
			if (lengthVar.LoadCount == 0 || lengthVar.LoadCount > 2)
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
			return MatchContainerVar(call.Arguments[0], ref containerVar);
		}

		static bool MatchContainerVar(ILInstruction inst, ref ILVariable containerVar)
		{ 
			if (containerVar != null) {
				return inst.MatchLdLoc(containerVar) || inst.MatchLdLoca(containerVar);
			} else {
				return inst.MatchLdLoc(out containerVar) || inst.MatchLdLoca(out containerVar);
			}
		}

		enum IndexKind
		{
			/// <summary>
			/// indexLoad is an integer, from the start of the container
			/// </summary>
			FromStart,
			/// <summary>
			/// indexLoad is loading the address of a System.Index struct
			/// </summary>
			RefSystemIndex,
			/// <summary>
			/// indexLoad is an integer, from the end of the container
			/// </summary>
			FromEnd,
			/// <summary>
			/// Always equivalent to `0`, used for the start-index when slicing without a startpoint `a[..end]`
			/// </summary>
			TheStart,
			/// <summary>
			/// Always equivalent to `^0`, used for the end-index when slicing without an endpoint `a[start..]`
			/// </summary>
			TheEnd,
		}

		/// <summary>
		/// Matches an instruction computing an offset:
		///    call System.Index.GetOffset(indexLoad, ldloc containerLengthVar)
		/// or
		///    binary.sub.i4(ldloc containerLengthVar, indexLoad)
		/// 
		/// Anything else not matching these patterns is interpreted as an `int` expression from the start of the container.
		/// </summary>
		static IndexKind MatchGetOffset(ILInstruction inst, out ILInstruction indexLoad,
			ILVariable containerLengthVar, ref ILVariable containerVar)
		{
			indexLoad = inst;
			if (MatchContainerLength(inst, containerLengthVar, ref containerVar)) {
				indexLoad = new LdcI4(0);
				return IndexKind.TheEnd;
			} else if (inst is CallInstruction call) {
				// call System.Index.GetOffset(indexLoad, ldloc containerLengthVar)
				if (call.Method.Name != "GetOffset")
					return IndexKind.FromStart;
				if (!call.Method.DeclaringType.IsKnownType(KnownTypeCode.Index))
					return IndexKind.FromStart;
				if (call.Arguments.Count != 2)
					return IndexKind.FromStart;
				if (!MatchContainerLength(call.Arguments[1], containerLengthVar, ref containerVar))
					return IndexKind.FromStart;
				indexLoad = call.Arguments[0];
				return IndexKind.RefSystemIndex;
			} else if (inst is BinaryNumericInstruction bni && bni.Operator == BinaryNumericOperator.Sub) {
				if (bni.CheckForOverflow || bni.ResultType != StackType.I4 || bni.IsLifted)
					return IndexKind.FromStart;
				// binary.sub.i4(ldloc containerLengthVar, indexLoad)
				if (!MatchContainerLength(bni.Left, containerLengthVar, ref containerVar))
					return IndexKind.FromStart;
				indexLoad = bni.Right;
				return IndexKind.FromEnd;
			} else {
				return IndexKind.FromStart;
			}
		}

		/// <summary>
		/// Matches an instruction computing a slice length:
		///    binary.sub.i4(call GetOffset(endIndexLoad, ldloc length), ldloc startOffset))
		/// </summary>
		static bool MatchSliceLength(ILInstruction inst, out IndexKind endIndexKind, out ILInstruction endIndexLoad, ILVariable containerLengthVar, ref ILVariable containerVar, ILVariable startOffsetVar)
		{
			endIndexKind = default;
			endIndexLoad = default;
			if (inst is BinaryNumericInstruction bni && bni.Operator == BinaryNumericOperator.Sub) {
				if (bni.CheckForOverflow || bni.ResultType != StackType.I4 || bni.IsLifted)
					return false;
				if (startOffsetVar == null) {
					// When slicing without explicit start point: `a[..endIndex]`
					if (!bni.Right.MatchLdcI4(0))
						return false;
				} else {
					if (!bni.Right.MatchLdLoc(startOffsetVar))
						return false;
				}
				endIndexKind = MatchGetOffset(bni.Left, out endIndexLoad, containerLengthVar, ref containerVar);
				return true;
			} else if (startOffsetVar == null) {
				// When slicing without explicit start point: `a[..endIndex]`, the compiler doesn't always emit the "- 0".
				endIndexKind = MatchGetOffset(inst, out endIndexLoad, containerLengthVar, ref containerVar);
				return true;
			} else {
				return false;
			}
		}
	}
}
