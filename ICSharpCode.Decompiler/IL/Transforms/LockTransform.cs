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
	public class LockTransform : IBlockTransform
	{
		BlockTransformContext context;

		void IBlockTransform.Run(Block block, BlockTransformContext context)
		{
			if (!context.Settings.LockStatement) return;
			this.context = context;
			for (int i = block.Instructions.Count - 1; i >= 0; i--) {
				if (!TransformLockRoslyn(block, i))
					if (!TransformLockV4(block, i))
						if (!TransformLockV2(block, i))
							TransformLockMCS(block, i);
				// This happens in some cases:
				// Use correct index after transformation.
				if (i >= block.Instructions.Count)
					i = block.Instructions.Count;
			}
		}

		/// <summary>
		///	stloc lockObj(lockExpression)
		///	call Enter(ldloc lockObj)
		///	.try BlockContainer {
		///		Block lockBlock (incoming: 1) {
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		///
		///	} finally BlockContainer {
		///		Block exitBlock (incoming: 1) {
		///			call Exit(ldloc lockObj)
		///			leave exitBlock (nop)
		///		}
		///
		///	}
		/// =>
		/// .lock (lockExpression) BlockContainer {
		/// 	Block lockBlock (incoming: 1) {
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		/// }
		/// </summary>
		bool TransformLockMCS(Block block, int i)
		{
			if (i < 2) return false;
			if (!(block.Instructions[i] is TryFinally body) || !(block.Instructions[i - 2] is StLoc objectStore) ||
				!MatchCall(block.Instructions[i - 1] as Call, "Enter", objectStore.Variable))
				return false;
			if (!objectStore.Variable.IsSingleDefinition)
				return false;
			if (!(body.TryBlock is BlockContainer tryContainer) || tryContainer.EntryPoint.Instructions.Count == 0 || tryContainer.EntryPoint.IncomingEdgeCount != 1)
				return false;
			if (!(body.FinallyBlock is BlockContainer finallyContainer) || !MatchExitBlock(finallyContainer.EntryPoint, null, objectStore.Variable))
				return false;
			if (objectStore.Variable.LoadCount > 2)
				return false;
			context.Step("LockTransformMCS", block);
			block.Instructions.RemoveAt(i - 1);
			block.Instructions.RemoveAt(i - 2);
			body.ReplaceWith(new LockInstruction(objectStore.Value, body.TryBlock).WithILRange(objectStore));
			return true;
		}

		/// <summary>
		///	stloc lockObj(ldloc tempVar)
		///	call Enter(ldloc tempVar)
		///	.try BlockContainer {
		///		Block lockBlock(incoming: 1) {
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		///	} finally BlockContainer {
		///		Block exitBlock(incoming: 1) {
		///			call Exit(ldloc lockObj)
		///			leave exitBlock (nop)
		///		}
		/// }
		/// =>
		/// .lock (lockObj) BlockContainer {
		/// 	Block lockBlock (incoming: 1) {
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		/// }
		/// </summary>
		bool TransformLockV2(Block block, int i)
		{
			if (i < 2) return false;
			if (!(block.Instructions[i] is TryFinally body) || !(block.Instructions[i - 2] is StLoc objectStore) ||
				!objectStore.Value.MatchLdLoc(out var tempVar) || !MatchCall(block.Instructions[i - 1] as Call, "Enter", tempVar))
				return false;
			if (!objectStore.Variable.IsSingleDefinition)
				return false;
			if (!(body.TryBlock is BlockContainer tryContainer) || tryContainer.EntryPoint.Instructions.Count == 0 || tryContainer.EntryPoint.IncomingEdgeCount != 1)
				return false;
			if (!(body.FinallyBlock is BlockContainer finallyContainer) || !MatchExitBlock(finallyContainer.EntryPoint, null, objectStore.Variable))
				return false;
			if (objectStore.Variable.LoadCount > 1)
				return false;
			context.Step("LockTransformV2", block);
			block.Instructions.RemoveAt(i - 1);
			block.Instructions.RemoveAt(i - 2);
			body.ReplaceWith(new LockInstruction(objectStore.Value, body.TryBlock).WithILRange(objectStore));
			return true;
		}

		/// <summary>
		///	stloc flag(ldc.i4 0)
		///	.try BlockContainer {
		///		Block lockBlock (incoming: 1) {
		///			call Enter(stloc obj(lockObj), ldloca flag)
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		///	} finally BlockContainer {
		///		Block (incoming: 1) {
		///			if (ldloc flag) Block  {
		///				call Exit(ldloc obj)
		///			}
		///			leave lockBlock (nop)
		///		}		
		/// }
		/// =>
		/// .lock (lockObj) BlockContainer {
		/// 	Block lockBlock (incoming: 1) {
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		/// }
		/// </summary>
		bool TransformLockV4(Block block, int i)
		{
			if (i < 1) return false;
			if (!(block.Instructions[i] is TryFinally body) || !(block.Instructions[i - 1] is StLoc flagStore))
				return false;
			if (!flagStore.Variable.Type.IsKnownType(KnownTypeCode.Boolean) || !flagStore.Value.MatchLdcI4(0))
				return false;
			if (!(body.TryBlock is BlockContainer tryContainer) || !MatchLockEntryPoint(tryContainer.EntryPoint, flagStore.Variable, out StLoc objectStore))
				return false;
			if (!(body.FinallyBlock is BlockContainer finallyContainer) || !MatchExitBlock(finallyContainer.EntryPoint, flagStore.Variable, objectStore.Variable))
				return false;
			if (objectStore.Variable.LoadCount > 1)
				return false;
			context.Step("LockTransformV4", block);
			block.Instructions.RemoveAt(i - 1);
			tryContainer.EntryPoint.Instructions.RemoveAt(0);
			body.ReplaceWith(new LockInstruction(objectStore.Value, body.TryBlock).WithILRange(objectStore));
			return true;
		}

		/// <summary>
		/// stloc obj(lockObj)
		///	stloc flag(ldc.i4 0)
		///	.try BlockContainer {
		///		Block lockBlock (incoming: 1) {
		///			call Enter(ldloc obj, ldloca flag)
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		///	} finally BlockContainer {
		///		Block (incoming: 1) {
		///			if (ldloc flag) Block  {
		///				call Exit(ldloc obj)
		///			}
		///			leave lockBlock (nop)
		///		}		
		/// }
		/// =>
		/// .lock (lockObj) BlockContainer {
		/// 	Block lockBlock (incoming: 1) {
		///			call WriteLine()
		///			leave lockBlock (nop)
		///		}
		/// }
		/// </summary>
		bool TransformLockRoslyn(Block block, int i)
		{
			if (i < 2) return false;
			if (!(block.Instructions[i] is TryFinally body) || !(block.Instructions[i - 1] is StLoc flagStore) || !(block.Instructions[i - 2] is StLoc objectStore))
				return false;
			if (!objectStore.Variable.IsSingleDefinition || !flagStore.Variable.Type.IsKnownType(KnownTypeCode.Boolean) || !flagStore.Value.MatchLdcI4(0))
				return false;
			if (!(body.TryBlock is BlockContainer tryContainer) || !MatchLockEntryPoint(tryContainer.EntryPoint, flagStore.Variable, objectStore.Variable))
				return false;
			if (!(body.FinallyBlock is BlockContainer finallyContainer) || !MatchExitBlock(finallyContainer.EntryPoint, flagStore.Variable, objectStore.Variable))
				return false;
			if (objectStore.Variable.LoadCount > 2)
				return false;
			context.Step("LockTransformRoslyn", block);
			block.Instructions.RemoveAt(i - 1);
			block.Instructions.RemoveAt(i - 2);
			tryContainer.EntryPoint.Instructions.RemoveAt(0);
			body.ReplaceWith(new LockInstruction(objectStore.Value, body.TryBlock).WithILRange(objectStore));
			return true;
		}

		bool MatchExitBlock(Block entryPoint, ILVariable flag, ILVariable obj)
		{
			if (entryPoint.Instructions.Count != 2 || entryPoint.IncomingEdgeCount != 1)
				return false;
			if (flag != null) {
				if (!entryPoint.Instructions[0].MatchIfInstruction(out var cond, out var trueInst) || !(trueInst is Block trueBlock))
					return false;
				if (!(cond.MatchLdLoc(flag) || (cond.MatchCompNotEquals(out var left, out var right) && left.MatchLdLoc(flag) && right.MatchLdcI4(0))) || !MatchExitBlock(trueBlock, obj))
					return false;
			} else {
				if (!MatchCall(entryPoint.Instructions[0] as Call, "Exit", obj))
					return false;
			}
			if (!entryPoint.Instructions[1].MatchLeave((BlockContainer)entryPoint.Parent, out var retVal) || !retVal.MatchNop())
				return false;
			return true;
		}

		bool MatchExitBlock(Block exitBlock, ILVariable obj)
		{
			if (exitBlock.Instructions.Count != 1)
				return false;
			if (!MatchCall(exitBlock.Instructions[0] as Call, "Exit", obj))
				return false;
			return true;
		}

		bool MatchLockEntryPoint(Block entryPoint, ILVariable flag, ILVariable obj)
		{
			if (entryPoint.Instructions.Count == 0 || entryPoint.IncomingEdgeCount != 1)
				return false;
			if (!MatchCall(entryPoint.Instructions[0] as Call, "Enter", obj, flag))
				return false;
			return true;
		}

		bool MatchLockEntryPoint(Block entryPoint, ILVariable flag, out StLoc obj)
		{
			obj = null;
			if (entryPoint.Instructions.Count == 0 || entryPoint.IncomingEdgeCount != 1)
				return false;
			if (!MatchCall(entryPoint.Instructions[0] as Call, "Enter", flag, out obj))
				return false;
			return true;
		}

		bool MatchCall(Call call, string methodName, ILVariable flag, out StLoc obj)
		{
			obj = null;
			const string ThreadingMonitor = "System.Threading.Monitor";
			if (call == null || call.Method.Name != methodName || call.Method.DeclaringType.FullName != ThreadingMonitor ||
				call.Method.TypeArguments.Count != 0 || call.Arguments.Count != 2)
				return false;
			if (!call.Arguments[1].MatchLdLoca(flag) || !(call.Arguments[0] is StLoc val))
				return false;
			obj = val;
			return true;
		}

		bool MatchCall(Call call, string methodName, params ILVariable[] variables)
		{
			const string ThreadingMonitor = "System.Threading.Monitor";
			if (call == null || call.Method.Name != methodName || call.Method.DeclaringType.FullName != ThreadingMonitor ||
				call.Method.TypeArguments.Count != 0 || call.Arguments.Count != variables.Length)
				return false;
			if (!call.Arguments[0].MatchLdLoc(variables[0]))
				return false;
			if (variables.Length == 2) {
				if (!call.Arguments[1].MatchLdLoca(variables[1]))
					return false;
			}
			return true;
		}
	}
}
