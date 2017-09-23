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
	class UsingTransform : IBlockTransform
	{
		BlockTransformContext context;

		void IBlockTransform.Run(Block block, BlockTransformContext context)
		{
			if (!context.Settings.UsingStatement) return;
			this.context = context;
			for (int i = block.Instructions.Count - 1; i >= 0; i--) {
				if (!TransformUsing(block, i))
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
			if (!(block.Instructions[i] is TryFinally body) || !(block.Instructions[i - 1] is StLoc storeInst))
				return false;
			if (!(body.FinallyBlock is BlockContainer container) || !MatchDisposeBlock(container, storeInst.Variable, storeInst.Value.MatchLdNull()))
				return false;
			ILInstruction resourceExpression;
			if (storeInst.Variable.Type.IsReferenceType != false) {
				if (storeInst.Variable.IsSingleDefinition && storeInst.Variable.LoadCount <= 2) {
					resourceExpression = storeInst.Value;
				} else {
					resourceExpression = storeInst;
				}
			} else {
				if (storeInst.Variable.StoreCount == 1 && storeInst.Variable.LoadCount == 0 && storeInst.Variable.AddressCount == 1) {
					resourceExpression = storeInst.Value;
				} else {
					resourceExpression = storeInst;
				}
			}
			context.Step("UsingTransform", body);
			block.Instructions.RemoveAt(i);
			block.Instructions[i - 1] = new UsingInstruction(resourceExpression, UnwrapNestedContainerIfPossible(body.TryBlock));
			return true;
		}

		ILInstruction UnwrapNestedContainerIfPossible(ILInstruction tryBlock)
		{
			if (!(tryBlock is BlockContainer container))
				return tryBlock;
			if (container.Blocks.Count != 1)
				return tryBlock;
			var nestedBlock = container.Blocks[0];
			if (nestedBlock.Instructions.Count != 2 ||
				!(nestedBlock.Instructions[0] is BlockContainer nestedContainer) ||
				!nestedBlock.Instructions[1].MatchLeave(container))
				return tryBlock;
			return nestedContainer;
		}

		bool MatchDisposeBlock(BlockContainer container, ILVariable objVar, bool usingNull)
		{
			var entryPoint = container.EntryPoint;
			if (entryPoint.Instructions.Count < 2 || entryPoint.Instructions.Count > 3 || entryPoint.IncomingEdgeCount != 1)
				return false;
			int leaveIndex = entryPoint.Instructions.Count == 2 ? 1 : 2;
			int checkIndex = entryPoint.Instructions.Count == 2 ? 0 : 1;
			int castIndex = entryPoint.Instructions.Count == 3 ? 0 : -1;
			bool isReference = objVar.Type.IsReferenceType != false;
			if (castIndex > -1) {
				if (!entryPoint.Instructions[castIndex].MatchStLoc(out var tempVar, out var isinst))
					return false;
				if (!isinst.MatchIsInst(out var load, out var disposableType) || !load.MatchLdLoc(objVar) || !disposableType.IsKnownType(KnownTypeCode.IDisposable))
					return false;
				objVar = tempVar;
				isReference = true;
			}
			if (!entryPoint.Instructions[leaveIndex].MatchLeave(container, out var returnValue) || !returnValue.MatchNop())
				return false;
			CallVirt callVirt;
			// reference types have a null check.
			if (isReference) {
				if (!entryPoint.Instructions[checkIndex].MatchIfInstruction(out var condition, out var disposeInst))
					return false;
				if (!condition.MatchCompNotEquals(out var left, out var right) || !left.MatchLdLoc(objVar) || !right.MatchLdNull())
					return false;
				if (!(disposeInst is Block disposeBlock) || disposeBlock.Instructions.Count != 1)
					return false;
				if (!(disposeBlock.Instructions[0] is CallVirt cv))
					return false;
				callVirt = cv;
			} else {
				if (!(entryPoint.Instructions[checkIndex] is CallVirt cv))
					return false;
				callVirt = cv;
			}
			if (callVirt.Method.Name != "Dispose" || callVirt.Method.DeclaringType.FullName != "System.IDisposable")
				return false;
			if (callVirt.Method.Parameters.Count > 0)
				return false;
			if (callVirt.Arguments.Count != 1)
				return false;
			if (objVar.Type.IsReferenceType != false) {
				return callVirt.Arguments[0].MatchLdLoc(objVar) || (usingNull && callVirt.Arguments[0].MatchLdNull());
			} else {
				return callVirt.Arguments[0].MatchLdLoca(objVar);
			}
		}
	}
}
