// Copyright (c) 2016 Siegfried Pammer
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Repeats the child transforms until the ILAst no longer changes (fixpoint iteration).
	/// </summary>
	public class LoopingBlockTransform : IBlockTransform
	{
		readonly IBlockTransform[] children;
		bool running;

		public LoopingBlockTransform(params IBlockTransform[] children)
		{
			this.children = children;
		}
		
		public void Run(Block block, BlockTransformContext context)
		{
			if (running)
				throw new InvalidOperationException("LoopingBlockTransform already running. Transforms (and the CSharpDecompiler) are neither neither thread-safe nor re-entrant.");
			running = true;
			try {
				int count = 1;
				do {
					block.ResetDirty();
					block.RunTransforms(children, context);
					if (block.IsDirty)
						context.Step($"Block is dirty; running loop iteration #{++count}.", block);
				} while (block.IsDirty);
			} finally {
				running = false;
			}
		}

		public IReadOnlyCollection<IBlockTransform> Transforms {
			get { return children; }
		}
	}
}


