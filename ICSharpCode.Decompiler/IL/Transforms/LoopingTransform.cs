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

using System.Collections.Generic;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Repeats the child transforms until the ILAst no longer changes.
	/// </summary>
	public class LoopingTransform : IILTransform
	{
		readonly IILTransform[] children;

		public LoopingTransform(params IILTransform[] children)
		{
			this.children = children;
		}

		public void Run(ILFunction function, ILTransformContext context)
		{
			do {
				function.ResetDirty();
				function.RunTransforms(children, context);
				if (function.IsDirty)
					context.Step("Function is dirty; running another loop iteration.", function);
			} while (function.IsDirty);
		}

		public IReadOnlyCollection<IILTransform> Transforms
		{
			get { return children; }
		}
	}

	/// <summary>
	/// Repeats the child transforms until the ILAst no longer changes.
	/// </summary>
	public class LoopingBlockTransform : IBlockTransform
	{
		readonly IBlockTransform[] children;
		
		public LoopingBlockTransform(params IBlockTransform[] children)
		{
			this.children = children;
		}
		
		public void Run(Block block, BlockTransformContext context)
		{
			do {
				block.ResetDirty();
				block.RunTransforms(children, context);
				if (block.IsDirty)
					context.Step("Block is dirty; running another loop iteration.", block);
			} while (block.IsDirty);
		}

		public IReadOnlyCollection<IBlockTransform> Transforms {
			get { return children; }
		}
	}
}


