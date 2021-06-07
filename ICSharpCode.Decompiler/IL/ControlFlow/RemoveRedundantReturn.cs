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
				case BlockContainer c when c.Kind == ContainerKind.Normal:
					// body of try block, or similar: recurse into last instruction in container
					// Note: no need to handle loops/switches here; those already were handled by DetectExitPoints
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
	}
}
