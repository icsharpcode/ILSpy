// Copyright (c) 2017 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// IL transform that runs on a sequence of statements within a block.
	/// </summary>
	/// <remarks>
	/// Interleaving different statement-combining transforms on a per-statement level
	/// improves detection of nested constructs.
	/// For example, array initializers can assume that each element assignment was already
	/// reduced to a single statement, even if the element contains a high-level construct
	/// detected by a different transform (e.g. object initializer).
	/// </remarks>
	public interface IStatementTransform
	{
		/// <summary>
		/// Runs the transform on the statements within a block.
		/// 
		/// Note: the transform may only modify block.Instructions[pos..].
		/// The transform will be called repeatedly for pos=block.Instructions.Count-1, pos=block.Instructions.Count-2, ..., pos=0.
		/// </summary>
		/// <param name="block">The current block.</param>
		/// <param name="pos">The starting position where the transform is allowed to work.</param>
		/// <param name="context">Additional parameters.</param>
		/// <remarks>
		/// Instructions prior to block.Instructions[pos] must not be modified.
		/// It is valid to read such instructions, but not recommended as those have not been transformed yet.
		/// </remarks>
		void Run(Block block, int pos, StatementTransformContext context);
	}

	/// <summary>
	/// Parameter class holding various arguments for <see cref="IStatementTransform.Run"/>.
	/// </summary>
	public class StatementTransformContext : ILTransformContext
	{
		public BlockTransformContext BlockContext { get; }

		public StatementTransformContext(BlockTransformContext blockContext) : base(blockContext)
		{
			this.BlockContext = blockContext ?? throw new ArgumentNullException(nameof(blockContext));
		}

		/// <summary>
		/// Gets the block on which the transform is running.
		/// </summary>
		public Block Block => BlockContext.Block;

		internal bool rerunCurrentPosition;
		internal int? rerunPosition;

		/// <summary>
		/// After the current statement transform has completed,
		/// do not continue with the next statement transform at the same position.
		/// Instead, re-run all statement transforms (including the current transform) starting at the specified position.
		/// </summary>
		public void RequestRerun(int pos)
		{
			if (rerunPosition == null || pos > rerunPosition) {
				rerunPosition = pos;
			}
		}

		/// <summary>
		/// After the current statement transform has completed,
		/// repeat all statement transforms on the current position.
		/// </summary>
		public void RequestRerun()
		{
			rerunCurrentPosition = true;
		}
	}

	/// <summary>
	/// Block transform that runs a list of statement transforms.
	/// </summary>
	public class StatementTransform : IBlockTransform
	{
		readonly IStatementTransform[] children;
		
		public StatementTransform(params IStatementTransform[] children)
		{
			this.children = children;
		}

		public void Run(Block block, BlockTransformContext context)
		{
			var ctx = new StatementTransformContext(context);
			int pos = 0;
			ctx.rerunPosition = block.Instructions.Count - 1;
			while (pos >= 0) {
				if (ctx.rerunPosition != null) {
					Debug.Assert(ctx.rerunPosition >= pos);
#if DEBUG
					for (; pos < ctx.rerunPosition; ++pos) {
						block.Instructions[pos].ResetDirty();
					}
#else
					pos = ctx.rerunPosition.Value;
#endif
					Debug.Assert(pos == ctx.rerunPosition);
					ctx.rerunPosition = null;
				}
				foreach (var transform in children) {
					transform.Run(block, pos, ctx);
#if DEBUG
					block.Instructions[pos].CheckInvariant(ILPhase.Normal);
					for (int i = Math.Max(0, pos - 100); i < pos; ++i) {
						if (block.Instructions[i].IsDirty) {
							Debug.Fail($"{transform.GetType().Name} modified an instruction before pos");
						}
					}
#endif
					if (ctx.rerunCurrentPosition) {
						ctx.rerunCurrentPosition = false;
						ctx.RequestRerun(pos);
					}
					if (ctx.rerunPosition != null) {
						break;
					}
				}
				if (ctx.rerunPosition == null) {
					pos--;
				}
			}
		}
	}
}
