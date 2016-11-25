using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL.ControlFlow;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	/// <summary>
	/// Per-block IL transform.
	/// </summary>
	public interface IBlockTransform
	{
		void Run(Block block, BlockTransformContext context);
	}

	/// <summary>
	/// Parameter class holding various arguments for <see cref="IBlockTransform.Run(ILFunction, BlockTransformContext)"/>.
	/// </summary>
	public class BlockTransformContext : ILTransformContext
	{
		/// <summary>
		/// The function containing the block currently being processed.
		/// </summary>
		public ILFunction Function { get; set; }

		/// <summary>
		/// The container containing the block currently being processed.
		/// </summary>
		public BlockContainer Container { get; set; }

		/// <summary>
		/// The block to process.
		/// </summary>
		/// <remarks>
		/// Should be identical to the <c>block</c> parameter to <c>IBlockTransform.Run</c>.
		/// </remarks>
		public Block Block { get; set; }

		/// <summary>
		/// The control flow node corresponding to the block being processed.
		/// </summary>
		public ControlFlowNode ControlFlowNode { get; set; }

		internal readonly Dictionary<Block, ControlFlowNode> cfg = new Dictionary<Block, ControlFlowNode>();

		public BlockTransformContext(ILTransformContext context) : base(context)
		{
		}

		/// <summary>
		/// Gets the ControlFlowNode for the block.
		/// Precondition: the block belonged to the <c>Container</c> at the start of the block transforms
		/// (when the control flow graph was created).
		/// </summary>
		public ControlFlowNode GetNode(Block block)
		{
			return cfg[block];
		}
	}


	/// <summary>
	/// IL transform that runs a list of per-block transforms.
	/// </summary>
	public class BlockILTransform : IILTransform
	{
		readonly IBlockTransform[] blockTransforms;

		public BlockILTransform(params IBlockTransform[] blockTransforms)
		{
			this.blockTransforms = blockTransforms;
		}

		public void Run(ILFunction function, ILTransformContext context)
		{
			var blockContext = new BlockTransformContext(context);
			blockContext.Function = function;
			foreach (var container in function.Descendants.OfType<BlockContainer>()) {
				context.CancellationToken.ThrowIfCancellationRequested();
				var cfg = LoopDetection.BuildCFG(container);
				Dominance.ComputeDominance(cfg[0], context.CancellationToken);

				blockContext.Container = container;
				blockContext.cfg.Clear();
				for (int i = 0; i < cfg.Length; i++) {
					blockContext.cfg.Add(container.Blocks[i], cfg[i]);
				}
				VisitBlock(cfg[0], blockContext);
				// TODO: handle unreachable code?
			}
		}

		void VisitBlock(ControlFlowNode cfgNode, BlockTransformContext context)
		{
			Block block = (Block)cfgNode.UserData;
			context.Stepper.StartGroup(block.Label, block);
			// First, process the children in the dominator tree.
			// The ConditionDetection transform requires dominated blocks to
			// be already processed.
			foreach (var child in cfgNode.DominatorTreeChildren) {
				VisitBlock(child, context);
			}

			context.ControlFlowNode = cfgNode;
			context.Block = block;
			block.CheckInvariant(ILPhase.Normal);
			foreach (var transform in blockTransforms) {
				context.CancellationToken.ThrowIfCancellationRequested();
				transform.Run(context.Block, context);
				block.CheckInvariant(ILPhase.Normal);
			}
			context.Stepper.EndGroup();
		}
	}
}
