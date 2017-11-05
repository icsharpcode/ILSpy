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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// If possible, transforms plain ILAst loops into while (condition), do-while and for-loops.
	/// </summary>
	public class HighLevelLoopTransform : IILTransform
	{
		public void Run(ILFunction function, ILTransformContext context)
		{
			foreach (var loop in function.Descendants.OfType<BlockContainer>()) {
				if (loop.Kind != ContainerKind.Loop)
					continue;
				if (MatchWhileLoop(loop, out var loopBody)) {
					MatchForLoop(loop, loopBody);
					continue;
				}
				if (MatchDoWhileLoop(loop))
					continue;
			}
		}

		bool MatchWhileLoop(BlockContainer loop, out Block loopBody)
		{
			// while-loop:
			// if (loop-condition) br loop-content-block
			// leave loop-container
			// -or-
			// if (loop-condition) block content-block
			// leave loop-container
			loopBody = null;
			if (loop.EntryPoint.Instructions.Count != 2)
				return false;
			if (!(loop.EntryPoint.Instructions[0] is IfInstruction ifInstruction))
				return false;
			if (!ifInstruction.FalseInst.MatchNop())
				return false;
			var trueInst = ifInstruction.TrueInst;
			if (!loop.EntryPoint.Instructions[1].MatchLeave(loop))
				return false;
			if (trueInst is Block b) {
				loopBody = b;
				if (!MatchWhileLoopBody(loopBody, loop))
					return false;
				trueInst.ReplaceWith(new Branch(loopBody));
				loop.Blocks.Insert(1, loopBody);
			} else if (trueInst is Branch br) {
				loopBody = br.TargetBlock;
				if (!MatchWhileLoopBody(loopBody, loop))
					return false;
			} else {
				return false;
			}
			ifInstruction.FalseInst = loop.EntryPoint.Instructions[1];
			loop.EntryPoint.Instructions.RemoveAt(1);
			loop.Kind = ContainerKind.While;
			return true;
		}

		/// <summary>
		/// The last instruction of the while body must be a branch to the loop head.
		/// </summary>
		bool MatchWhileLoopBody(Block loopBody, BlockContainer loop)
		{
			if (loopBody.Instructions.Count == 0)
				return false;
			if (!loopBody.Instructions.Last().MatchBranch(loop.EntryPoint))
				return false;
			return true;
		}

		bool MatchDoWhileLoop(BlockContainer loop)
		{
			return false;
		}

		bool MatchForLoop(BlockContainer loop, Block whileLoopBody)
		{
			return false;
		}
	}
}
