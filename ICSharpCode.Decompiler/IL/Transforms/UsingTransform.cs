// Copyright (c) 2017 Siegfried Pammer
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
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	public class UsingTransform : IBlockTransform
	{
		BlockTransformContext context;

		void IBlockTransform.Run(Block block, BlockTransformContext context)
		{
			if (!context.Settings.UsingStatement) return;
			this.context = context;
			for (int i = block.Instructions.Count - 1; i >= 0; i--) {
				if (!TransformUsing(block, i) && !TransformUsingVB(block, i) && !TransformAsyncUsing(block, i))
					continue;
				// This happens in some cases:
				// Use correct index after transformation.
				if (i >= block.Instructions.Count)
					i = block.Instructions.Count;
			}
		}

		/// <summary>
		/// stloc obj(resourceExpression)
		/// .try BlockContainer {
		///		Block IL_0003(incoming: 1) {
		///			call WriteLine(ldstr "using (null)")
		///			leave IL_0003(nop)
		///		}
		///	} finally BlockContainer {
		///		Block IL_0012(incoming: 1) {
		///			if (comp(ldloc obj != ldnull)) Block IL_001a  {
		///				callvirt Dispose(ldnull)
		///			}
		///			leave IL_0012(nop)
		///		}
		/// }
		/// leave IL_0000(nop)
		/// =>
		/// using (resourceExpression) {
		///		BlockContainer {
		///			Block IL_0003(incoming: 1) {
		///				call WriteLine(ldstr "using (null)")
		///				leave IL_0003(nop)
		///			}
		///		}
		/// }
		/// </summary>
		bool TransformUsing(Block block, int i)
		{
			if (i < 1) return false;
			if (!(block.Instructions[i] is TryFinally tryFinally) || !(block.Instructions[i - 1] is StLoc storeInst))
				return false;
			if (!(storeInst.Value.MatchLdNull() || CheckResourceType(storeInst.Variable.Type)))
				return false;
			if (storeInst.Variable.Kind != VariableKind.Local)
				return false;
			if (storeInst.Variable.LoadInstructions.Any(ld => !ld.IsDescendantOf(tryFinally)))
				return false;
			if (storeInst.Variable.AddressInstructions.Any(la => !la.IsDescendantOf(tryFinally) || (la.IsDescendantOf(tryFinally.TryBlock) && !ILInlining.IsUsedAsThisPointerInCall(la))))
				return false;
			if (storeInst.Variable.StoreInstructions.Count > 1)
				return false;
			if (!(tryFinally.FinallyBlock is BlockContainer container) || !MatchDisposeBlock(container, storeInst.Variable, storeInst.Value.MatchLdNull()))
				return false;
			context.Step("UsingTransform", tryFinally);
			storeInst.Variable.Kind = VariableKind.UsingLocal;
			block.Instructions.RemoveAt(i);
			block.Instructions[i - 1] = new UsingInstruction(storeInst.Variable, storeInst.Value, tryFinally.TryBlock) {
				IsRefStruct = context.Settings.IntroduceRefModifiersOnStructs && storeInst.Variable.Type.Kind == TypeKind.Struct && storeInst.Variable.Type.IsByRefLike
			}.WithILRange(storeInst);
			return true;
		}

		/// <summary>
		/// .try BlockContainer {
		///		Block IL_0003(incoming: 1) {
		///			stloc obj(resourceExpression)
		///			call WriteLine(ldstr "using (null)")
		///			leave IL_0003(nop)
		///		}
		///	} finally BlockContainer {
		///		Block IL_0012(incoming: 1) {
		///			if (comp(ldloc obj != ldnull)) Block IL_001a  {
		///				callvirt Dispose(ldnull)
		///			}
		///			leave IL_0012(nop)
		///		}
		/// }
		/// leave IL_0000(nop)
		/// =>
		/// using (resourceExpression) {
		///		BlockContainer {
		///			Block IL_0003(incoming: 1) {
		///				call WriteLine(ldstr "using (null)")
		///				leave IL_0003(nop)
		///			}
		///		}
		/// }
		/// </summary>
		bool TransformUsingVB(Block block, int i)
		{
			if (!(block.Instructions[i] is TryFinally tryFinally))
				return false;
			if (!(tryFinally.TryBlock is BlockContainer tryContainer && tryContainer.EntryPoint.Instructions.FirstOrDefault() is StLoc storeInst))
				return false;
			if (!(storeInst.Value.MatchLdNull() || CheckResourceType(storeInst.Variable.Type)))
				return false;
			if (storeInst.Variable.Kind != VariableKind.Local)
				return false;
			if (storeInst.Variable.LoadInstructions.Any(ld => !ld.IsDescendantOf(tryFinally)))
				return false;
			if (storeInst.Variable.AddressInstructions.Any(la => !la.IsDescendantOf(tryFinally) || (la.IsDescendantOf(tryFinally.TryBlock) && !ILInlining.IsUsedAsThisPointerInCall(la))))
				return false;
			if (storeInst.Variable.StoreInstructions.Count > 1)
				return false;
			if (!(tryFinally.FinallyBlock is BlockContainer container) || !MatchDisposeBlock(container, storeInst.Variable, storeInst.Value.MatchLdNull()))
				return false;
			context.Step("UsingTransformVB", tryFinally);
			storeInst.Variable.Kind = VariableKind.UsingLocal;
			tryContainer.EntryPoint.Instructions.RemoveAt(0);
			block.Instructions[i] = new UsingInstruction(storeInst.Variable, storeInst.Value, tryFinally.TryBlock);
			return true;
		}

		bool CheckResourceType(IType type)
		{
			// non-generic IEnumerator does not implement IDisposable.
			// This is a workaround for non-generic foreach.
			if (type.IsKnownType(KnownTypeCode.IEnumerator) || type.GetAllBaseTypes().Any(b => b.IsKnownType(KnownTypeCode.IEnumerator)))
				return true;
			if (NullableType.GetUnderlyingType(type).GetAllBaseTypes().Any(b => b.IsKnownType(KnownTypeCode.IDisposable)))
				return true;
			if (context.Settings.IntroduceRefModifiersOnStructs && type.Kind == TypeKind.Struct && type.IsByRefLike)
				return true;
			// General GetEnumerator-pattern?
			if (!type.GetMethods(m => m.Name == "GetEnumerator" && m.TypeParameters.Count == 0 && m.Parameters.Count == 0).Any(m => ImplementsForeachPattern(m.ReturnType)))
				return false;
			return true;
		}

		bool ImplementsForeachPattern(IType type)
		{
			if (!type.GetMethods(m => m.Name == "MoveNext" && m.TypeParameters.Count == 0 && m.Parameters.Count == 0).Any(m => m.ReturnType.IsKnownType(KnownTypeCode.Boolean)))
				return false;
			if (!type.GetProperties(p => p.Name == "Current" && p.CanGet && !p.IsIndexer).Any())
				return false;
			return true;
		}

		bool MatchDisposeBlock(BlockContainer container, ILVariable objVar, bool usingNull, in string disposeMethodFullName = "System.IDisposable.Dispose", KnownTypeCode disposeTypeCode = KnownTypeCode.IDisposable)
		{
			var entryPoint = container.EntryPoint;
			if (entryPoint.Instructions.Count < 2 || entryPoint.Instructions.Count > 3 || entryPoint.IncomingEdgeCount != 1)
				return false;
			int leaveIndex = entryPoint.Instructions.Count == 2 ? 1 : 2;
			int checkIndex = entryPoint.Instructions.Count == 2 ? 0 : 1;
			int castIndex = entryPoint.Instructions.Count == 3 ? 0 : -1;
			var checkInst = entryPoint.Instructions[checkIndex];
			bool isReference = objVar.Type.IsReferenceType != false;
			if (castIndex > -1) {
				if (!entryPoint.Instructions[castIndex].MatchStLoc(out var tempVar, out var isinst))
					return false;
				if (!isinst.MatchIsInst(out var load, out var disposableType) || !load.MatchLdLoc(objVar) || !disposableType.IsKnownType(disposeTypeCode))
					return false;
				if (!tempVar.IsSingleDefinition)
					return false;
				isReference = true;
				if (!MatchDisposeCheck(tempVar, checkInst, isReference, usingNull, out int numObjVarLoadsInCheck, disposeMethodFullName, disposeTypeCode))
					return false;
				if (tempVar.LoadCount != numObjVarLoadsInCheck)
					return false;
			} else {
				if (!MatchDisposeCheck(objVar, checkInst, isReference, usingNull, out _, disposeMethodFullName, disposeTypeCode))
					return false;
			}
			if (!entryPoint.Instructions[leaveIndex].MatchLeave(container, out var returnValue) || !returnValue.MatchNop())
				return false;
			return true;
		}

		bool MatchDisposeCheck(ILVariable objVar, ILInstruction checkInst, bool isReference, bool usingNull, out int numObjVarLoadsInCheck, string disposeMethodFullName, KnownTypeCode disposeTypeCode)
		{
			numObjVarLoadsInCheck = 2;
			ILInstruction disposeInvocation;
			CallInstruction disposeCall;
			if (objVar.Type.IsKnownType(KnownTypeCode.NullableOfT)) {
				if (checkInst.MatchIfInstruction(out var condition, out var disposeInst)) {
					if (!NullableLiftingTransform.MatchHasValueCall(condition, objVar))
						return false;
					if (!(disposeInst is Block disposeBlock) || disposeBlock.Instructions.Count != 1)
						return false;
					disposeInvocation = disposeBlock.Instructions[0];
				} else if (checkInst.MatchNullableRewrap(out disposeInst)) {
					disposeInvocation = disposeInst;
				} else {
					return false;
				}
				if (disposeTypeCode == KnownTypeCode.IAsyncDisposable) {
					if (!UnwrapAwait(ref disposeInvocation))
						return false;
				}
				disposeCall = disposeInvocation as CallVirt;
				if (disposeCall == null)
					return false;
				if (disposeCall.Method.FullName != disposeMethodFullName)
					return false;
				if (disposeCall.Method.Parameters.Count > 0)
					return false;
				if (disposeCall.Arguments.Count != 1)
					return false;
				var firstArg = disposeCall.Arguments.FirstOrDefault();
				if (!(firstArg.MatchUnboxAny(out var innerArg1, out var unboxType) && unboxType.IsKnownType(disposeTypeCode))) {
					if (!firstArg.MatchAddressOf(out var innerArg2, out _))
						return false;
					return NullableLiftingTransform.MatchGetValueOrDefault(innerArg2, objVar)
						|| (innerArg2 is NullableUnwrap unwrap
							&& unwrap.Argument.MatchLdLoc(objVar));
				} else {
					if (!(innerArg1.MatchBox(out firstArg, out var boxType) && boxType.IsKnownType(KnownTypeCode.NullableOfT) &&
					NullableType.GetUnderlyingType(boxType).Equals(NullableType.GetUnderlyingType(objVar.Type))))
						return false;
					return firstArg.MatchLdLoc(objVar);
				}
			} else {
				ILInstruction target;
				bool boxedValue = false;
				if (isReference && checkInst is NullableRewrap rewrap) {
					// the null check of reference types might have been transformed into "objVar?.Dispose();"
					if (!(rewrap.Argument is CallVirt cv))
						return false;
					if (!(cv.Arguments.FirstOrDefault() is NullableUnwrap unwrap))
						return false;
					numObjVarLoadsInCheck = 1;
					disposeCall = cv;
					target = unwrap.Argument;
				} else if (isReference) {
					// reference types have a null check.
					if (!checkInst.MatchIfInstruction(out var condition, out var disposeInst))
						return false;
					if (!condition.MatchCompNotEquals(out var left, out var right) || !left.MatchLdLoc(objVar) || !right.MatchLdNull())
						return false;
					if (!(disposeInst is Block disposeBlock) || disposeBlock.Instructions.Count != 1)
						return false;
					disposeInvocation = disposeBlock.Instructions[0];
					if (disposeTypeCode == KnownTypeCode.IAsyncDisposable) {
						if (!UnwrapAwait(ref disposeInvocation))
							return false;
					}
					if (!(disposeInvocation is CallVirt cv))
						return false;
					target = cv.Arguments.FirstOrDefault();
					if (target == null)
						return false;
					if (target.MatchBox(out var newTarget, out var type) && type.Equals(objVar.Type))
						target = newTarget;
					disposeCall = cv;
				} else if (objVar.Type.Kind == TypeKind.Struct && objVar.Type.IsByRefLike) {
					if (!(checkInst is Call call && call.Method.DeclaringType == objVar.Type))
						return false;
					target = call.Arguments.FirstOrDefault();
					if (target == null)
						return false;
					if (call.Method.Name != "Dispose")
						return false;
					disposeMethodFullName = call.Method.FullName;
					disposeCall = call;
				} else {
					if (disposeTypeCode == KnownTypeCode.IAsyncDisposable) {
						if (!UnwrapAwait(ref checkInst))
							return false;
					}
					if (!(checkInst is CallInstruction cv))
						return false;
					target = cv.Arguments.FirstOrDefault();
					if (target == null)
						return false;
					if (target.MatchBox(out var newTarget, out var type) && type.Equals(objVar.Type)) {
						boxedValue = type.IsReferenceType != true;
						target = newTarget;
					}
					disposeCall = cv;
				}
				if (disposeCall.Method.FullName != disposeMethodFullName)
					return false;
				if (disposeCall.Method.Parameters.Count > 0)
					return false;
				if (disposeCall.Arguments.Count != 1)
					return false;
				return target.MatchLdLocRef(objVar)
					|| (boxedValue && target.MatchLdLoc(objVar))
					|| (usingNull && disposeCall.Arguments[0].MatchLdNull())
					|| (isReference && checkInst is NullableRewrap
						&& target.MatchIsInst(out var arg, out var type2)
						&& arg.MatchLdLoc(objVar) && type2.IsKnownType(disposeTypeCode));
			}
		}

		/// <summary>
		/// stloc test(resourceExpression)
		/// .try BlockContainer {
		/// 	Block IL_002b (incoming: 1) {
		/// 		call Use(ldloc test)
		/// 		leave IL_002b (nop)
		/// 	}
		/// 
		/// } finally BlockContainer {
		/// 	Block IL_0045 (incoming: 1) {
		/// 		if (comp.o(ldloc test == ldnull)) leave IL_0045 (nop)
		/// 		br IL_00ae
		/// 	}
		/// 
		/// 	Block IL_00ae (incoming: 1) {
		/// 		await(addressof System.Threading.Tasks.ValueTask(callvirt DisposeAsync(ldloc test)))
		/// 		leave IL_0045 (nop)
		/// 	}
		/// 
		/// }
		/// </summary>
		private bool TransformAsyncUsing(Block block, int i)
		{
			if (i < 1 || !context.Settings.AsyncUsingAndForEachStatement) return false;
			if (!(block.Instructions[i] is TryFinally tryFinally) || !(block.Instructions[i - 1] is StLoc storeInst))
				return false;
			if (!CheckAsyncResourceType(storeInst.Variable.Type, out string disposeMethodFullName))
				return false;
			if (storeInst.Variable.Kind != VariableKind.Local)
				return false;
			if (storeInst.Variable.LoadInstructions.Any(ld => !ld.IsDescendantOf(tryFinally)))
				return false;
			if (storeInst.Variable.AddressInstructions.Any(la => !la.IsDescendantOf(tryFinally) || (la.IsDescendantOf(tryFinally.TryBlock) && !ILInlining.IsUsedAsThisPointerInCall(la))))
				return false;
			if (storeInst.Variable.StoreInstructions.Count > 1)
				return false;
			if (!(tryFinally.FinallyBlock is BlockContainer container) || !MatchDisposeBlock(container, storeInst.Variable, usingNull: false, disposeMethodFullName, KnownTypeCode.IAsyncDisposable))
				return false;
			context.Step("AsyncUsingTransform", tryFinally);
			storeInst.Variable.Kind = VariableKind.UsingLocal;
			block.Instructions.RemoveAt(i);
			block.Instructions[i - 1] = new UsingInstruction(storeInst.Variable, storeInst.Value, tryFinally.TryBlock) { IsAsync = true }
				.WithILRange(storeInst);
			return true;
		}

		bool CheckAsyncResourceType(IType type, out string disposeMethodFullName)
		{
			disposeMethodFullName = null;
			IType t = NullableType.GetUnderlyingType(type);
			if (t.GetAllBaseTypes().Any(b => b.IsKnownType(KnownTypeCode.IAsyncDisposable))) {
				disposeMethodFullName = "System.IAsyncDisposable.DisposeAsync";
				return true;
			}

			IMethod disposeMethod = t
				.GetMethods(m => m.Parameters.Count == 0 && m.TypeParameters.Count == 0 && m.Name == "DisposeAsync")
				.SingleOrDefault();
			if (disposeMethod != null) {
				disposeMethodFullName = disposeMethod.FullName;
				return true;
			}

			return false;
		}

		bool UnwrapAwait(ref ILInstruction awaitInstruction)
		{
			if (awaitInstruction == null)
				return false;
			if (!awaitInstruction.MatchAwait(out var arg))
				return false;
			if (!arg.MatchAddressOf(out awaitInstruction, out var type))
				return false;
			// TODO check type: does it match the structural 'Awaitable' pattern?
			return true;
		}
	}
}
