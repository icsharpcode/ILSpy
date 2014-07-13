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
using System.Diagnostics;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Visitor that applies a list of transformations to the IL Ast.
	/// </summary>
	public class TransformingVisitor : ILVisitor<ILInstruction>
	{
		protected override ILInstruction Default(ILInstruction inst)
		{
			inst.TransformChildren(this);
			return inst;
		}
		
		protected internal override ILInstruction VisitBranch(Branch inst)
		{
			// If this branch is the only edge to the target block, we can inline the target block here:
			if (inst.TargetBlock.IncomingEdgeCount == 1 && inst.PopCount == 0) {
				return inst.TargetBlock;
			}
			return base.VisitBranch(inst);
		}
		
		protected internal override ILInstruction VisitBlockContainer(BlockContainer container)
		{
			container.TransformChildren(this);
			
			// VisitBranch() 'steals' blocks from containers. Remove all blocks that were stolen from the block list:
			Debug.Assert(container.EntryPoint.IncomingEdgeCount > 0);
			container.Blocks.RemoveAll(b => b.IncomingEdgeCount == 0);
			
			// If the container only contains a single block, and the block contents do not jump back to the block start,
			// we can remove the container.
			if (container.Blocks.Count == 1 && container.EntryPoint.IncomingEdgeCount == 1) {
				// If the block has only one instruction, we can remove the block too
				if (container.EntryPoint.Instructions.Count == 1)
					return container.EntryPoint.Instructions[0];
				return container.EntryPoint;
			}
			return container;
		}
	}
}
