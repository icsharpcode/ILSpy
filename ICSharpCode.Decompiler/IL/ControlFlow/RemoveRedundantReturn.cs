// Copyright (c) Daniel Grunwald
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

#nullable enable

using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Similar to <see cref="DetectExitPoints"/>, but acts only on <c>leave</c> instructions
	/// leaving the whole function (<c>return</c>/<c>yield break</c>) that can be made implicit
	/// without using goto.
	/// </summary>
	class RemoveRedundantReturn : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var lambda in function.Descendants.OfType<ILFunction>())
			{
				if (lambda.Body is BlockContainer c && ((lambda.AsyncReturnType ?? lambda.ReturnType).Kind == TypeSystem.TypeKind.Void || lambda.IsIterator))
				{
					Block lastBlock = c.Blocks.Last();
					if (lastBlock.Instructions.Last() is Leave { IsLeavingFunction: true })
					{
						ConvertReturnToFallthrough(lastBlock.Instructions.SecondToLastOrDefault());
					}
					else
					{
						if (ConvertReturnToFallthrough(lastBlock.Instructions.Last()))
						{
							lastBlock.Instructions.Add(new Leave(c));
						}
					}
				}
			}
		}

		private static bool ConvertReturnToFallthrough(ILInstruction? inst)
		{
			bool result = false;
			switch (inst)
			{
				case BlockContainer c:
					if (c.Kind != ContainerKind.Normal)
					{
						// loop or switch: turn all "return" into "break"
						result |= ReturnToLeaveInContainer(c);
					}
					else
					{
						// body of try block, or similar: recurse into last instruction in container
						Block lastBlock = c.Blocks.Last();
						if (lastBlock.Instructions.Last() is Leave { IsLeavingFunction: true, Value: Nop } leave)
						{
							leave.TargetContainer = c;
							result = true;
						}
						else if (ConvertReturnToFallthrough(lastBlock.Instructions.Last()))
						{
							lastBlock.Instructions.Add(new Leave(c));
							result = true;
						}
					}
					break;
				case TryCatch tryCatch:
					result |= ConvertReturnToFallthrough(tryCatch.TryBlock);
					foreach (var h in tryCatch.Handlers)
					{
						result |= ConvertReturnToFallthrough(h.Body);
					}
					break;
				case TryFinally tryFinally:
					result |= ConvertReturnToFallthrough(tryFinally.TryBlock);
					break;
				case LockInstruction lockInst:
					result |= ConvertReturnToFallthrough(lockInst.Body);
					break;
				case UsingInstruction usingInst:
					result |= ConvertReturnToFallthrough(usingInst.Body);
					break;
				case PinnedRegion pinnedRegion:
					result |= ConvertReturnToFallthrough(pinnedRegion.Body);
					break;
				case IfInstruction ifInstruction:
					result |= ConvertReturnToFallthrough(ifInstruction.TrueInst);
					result |= ConvertReturnToFallthrough(ifInstruction.FalseInst);
					break;
				case Block block when block.Kind == BlockKind.ControlFlow:
				{
					var lastInst = block.Instructions.LastOrDefault();
					if (lastInst is Leave { IsLeavingFunction: true, Value: Nop })
					{
						block.Instructions.RemoveAt(block.Instructions.Count - 1);
						result = true;
						lastInst = block.Instructions.LastOrDefault();
					}
					result |= ConvertReturnToFallthrough(lastInst);
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// Transforms
		///   loop { ... if (x) return; .. }
		/// to
		///   loop { ... if (x) break; .. } return;
		/// </summary>
		internal static void ReturnToBreak(Block parentBlock, BlockContainer loopOrSwitch, ILTransformContext context)
		{
			Debug.Assert(loopOrSwitch.Parent == parentBlock);
			// This transform is only possible when the loop/switch doesn't already use "break;"
			if (loopOrSwitch.LeaveCount != 0)
				return;
			// loopOrSwitch with LeaveCount==0 has unreachable exit point and thus must be last in block.
			Debug.Assert(parentBlock.Instructions.Last() == loopOrSwitch);
			var nearestFunction = parentBlock.Ancestors.OfType<ILFunction>().First();
			if (nearestFunction.Body is BlockContainer functionContainer)
			{
				context.Step("pull return out of loop/switch: " + loopOrSwitch.EntryPoint.Label, loopOrSwitch);
				if (ReturnToLeaveInContainer(loopOrSwitch))
				{
					// insert a return after the loop
					parentBlock.Instructions.Add(new Leave(functionContainer));
				}
			}
		}

		private static bool ReturnToLeaveInContainer(BlockContainer c)
		{
			bool result = false;
			foreach (var block in c.Blocks)
			{
				result |= ReturnToLeave(block, c);
			}
			return result;
		}

		private static bool ReturnToLeave(ILInstruction inst, BlockContainer c)
		{
			if (inst is Leave { IsLeavingFunction: true, Value: Nop } leave)
			{
				leave.TargetContainer = c;
				return true;
			}
			else if (inst is BlockContainer nested && nested.Kind != ContainerKind.Normal)
			{
				return false;
			}
			else if (inst is ILFunction)
			{
				return false;
			}
			else
			{
				bool b = false;
				foreach (var child in inst.Children)
				{
					b |= ReturnToLeave(child, c);
				}
				return b;
			}
		}
	}
}
