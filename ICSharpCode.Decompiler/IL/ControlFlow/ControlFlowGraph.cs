using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL.ControlFlow
{
	/// <summary>
	/// Holds the control flow graph.
	/// A separate graph is computed for each BlockContainer at the start of the block transforms
	/// (before loop detection).
	/// </summary>
	public class ControlFlowGraph
	{
		readonly BlockContainer container;

		/// <summary>
		/// The container for which the ControlFlowGraph was created.
		/// 
		/// This may differ from the container currently holding a block,
		/// because a transform could have moved the block since the CFG was created.
		/// </summary>
		public BlockContainer Container { get { return container; } }

		/// <summary>
		/// Nodes array, indexed by original block index.
		/// 
		/// Originally <c>cfg[i].UserData == container.Blocks[i]</c>,
		/// but the ILAst blocks may be moved/reordered by transforms.
		/// </summary>
		internal readonly ControlFlowNode[] cfg;

		/// <summary>
		/// Dictionary from Block to ControlFlowNode.
		/// Unlike the cfg array, this can be used to discover control flow nodes even after
		/// blocks were moved/reordered by transforms.
		/// </summary>
		readonly Dictionary<Block, ControlFlowNode> dict = new Dictionary<Block, ControlFlowNode>();

		/// <summary>
		/// nodeHasDirectExitOutOfContainer[i] == true iff cfg[i] directly contains a
		/// branch/leave instruction leaving the <c>container</c>.
		/// </summary>
		readonly BitSet nodeHasDirectExitOutOfContainer;

		/// <summary>
		/// nodeHasReachableExit[i] == true iff there is a path from cfg[i] to a node not dominated by cfg[i],
		/// or if there is a path from cfg[i] to a branch/leave instruction leaving the <c>container</c>.
		/// </summary>
		readonly BitSet nodeHasReachableExit;

		/// <summary>
		/// Constructs a control flow graph for the blocks in the given block container.
		/// 
		/// Return statements, exceptions, or branches leaving the block container are not
		/// modeled by the control flow graph.
		/// </summary>
		public ControlFlowGraph(BlockContainer container, CancellationToken cancellationToken = default(CancellationToken))
		{
			this.container = container;
			this.cfg = new ControlFlowNode[container.Blocks.Count];
			this.nodeHasDirectExitOutOfContainer = new BitSet(cfg.Length);
			for (int i = 0; i < cfg.Length; i++)
			{
				Block block = container.Blocks[i];
				cfg[i] = new ControlFlowNode { UserIndex = i, UserData = block };
				dict.Add(block, cfg[i]);
			}

			CreateEdges(cancellationToken);
			Dominance.ComputeDominance(cfg[0], cancellationToken);
			this.nodeHasReachableExit = Dominance.MarkNodesWithReachableExits(cfg);
			this.nodeHasReachableExit.UnionWith(FindNodesWithExitsOutOfContainer());
		}

		void CreateEdges(CancellationToken cancellationToken)
		{
			for (int i = 0; i < container.Blocks.Count; i++)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var block = container.Blocks[i];
				var sourceNode = cfg[i];
				foreach (var node in block.Descendants)
				{
					if (node is Branch branch)
					{
						if (branch.TargetBlock.Parent == container)
						{
							sourceNode.AddEdgeTo(cfg[container.Blocks.IndexOf(branch.TargetBlock)]);
						}
						else if (branch.TargetBlock.IsDescendantOf(container))
						{
							// Internal control flow within a nested container.
						}
						else
						{
							// Branch out of this container into a parent container.
							// Like return statements and exceptional exits,
							// we ignore this for the CFG and the dominance calculation.
							// However, it's relevant for HasReachableExit().
							nodeHasDirectExitOutOfContainer.Set(i);
						}
					}
					else if (node is Leave leave && !leave.TargetContainer.IsDescendantOf(block))
					{
						// Leave instructions (like other exits out of the container)
						// are ignored for the CFG and dominance,
						// but is relevant for HasReachableExit().
						// However, a 'leave' that exits the whole function represents a return,
						// and is not considered a reachable exit.
						if (!leave.IsLeavingFunction)
						{
							nodeHasDirectExitOutOfContainer.Set(i);
						}
					}
				}
			}
		}

		BitSet FindNodesWithExitsOutOfContainer()
		{
			// Also mark the nodes that exit the block container altogether.
			// Invariant: leaving[n.UserIndex] == true implies leaving[n.ImmediateDominator.UserIndex] == true
			var leaving = new BitSet(cfg.Length);
			foreach (var node in cfg)
			{
				if (leaving[node.UserIndex])
					continue;
				if (nodeHasDirectExitOutOfContainer[node.UserIndex])
				{
					for (ControlFlowNode p = node; p != null; p = p.ImmediateDominator)
					{
						if (leaving[p.UserIndex])
						{
							// we can stop marking when we've reached an already-marked node
							break;
						}
						leaving.Set(p.UserIndex);
					}
				}
			}
			return leaving;
		}

		/// <summary>
		/// Gets the ControlFlowNode for the block.
		/// 
		/// Precondition: the block belonged to the <c>container</c> at the start of the block transforms
		/// (when the control flow graph was created).
		/// </summary>
		public ControlFlowNode GetNode(Block block)
		{
			return dict[block];
		}

		/// <summary>
		/// Returns true iff there is a control flow path from <c>node</c> to one of the following:
		///  * branch or leave instruction leaving <c>this.Container</c>
		///  * branch instruction within this container to another node that is not dominated by <c>node</c>.
		/// 
		/// If this function returns false, the only way control flow can leave the set of nodes
		/// dominated by <c>node</c> is by executing a <c>return</c> or <c>throw</c> instruction.
		/// </summary>
		public bool HasReachableExit(ControlFlowNode node)
		{
			Debug.Assert(cfg[node.UserIndex] == node);
			return nodeHasReachableExit[node.UserIndex];
		}

		/// <summary>
		/// Gets whether the control flow node directly contains a branch/leave instruction
		/// exiting the container.
		/// </summary>
		public bool HasDirectExitOutOfContainer(ControlFlowNode node)
		{
			Debug.Assert(cfg[node.UserIndex] == node);
			return nodeHasDirectExitOutOfContainer[node.UserIndex];
		}
	}
}
