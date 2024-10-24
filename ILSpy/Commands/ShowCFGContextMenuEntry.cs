using System;
using System.Collections.Generic;
using System.Composition;
using System.Windows;

using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;

namespace ICSharpCode.ILSpy.Commands
{
#if DEBUG
	[ExportContextMenuEntry(Header = "DEBUG -- Show CFG")]
	[Shared]
	internal class ShowCFGContextMenuEntry : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			try
			{
				var container = (BlockContainer)context.Reference.Reference;
				var cfg = new ControlFlowGraph(container);
				ExportGraph(cfg.Nodes).Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error generating CFG - requires GraphViz dot.exe in PATH" + Environment.NewLine + Environment.NewLine + ex.ToString());
			}
		}

		public bool IsEnabled(TextViewContext context)
		{
			return context.Reference?.Reference is BlockContainer;
		}

		public bool IsVisible(TextViewContext context)
		{
			return context.Reference?.Reference is BlockContainer;
		}

		internal static GraphVizGraph ExportGraph(IReadOnlyList<ControlFlowNode> nodes, Func<ControlFlowNode, string> labelFunc = null)
		{
			if (labelFunc == null)
			{
				labelFunc = node => {
					var block = node.UserData as Block;
					return block != null ? block.Label : node.UserData?.ToString();
				};
			}
			GraphVizGraph g = new GraphVizGraph();
			GraphVizNode[] n = new GraphVizNode[nodes.Count];
			for (int i = 0; i < n.Length; i++)
			{
				n[i] = new GraphVizNode(nodes[i].UserIndex);
				n[i].shape = "box";
				n[i].label = labelFunc(nodes[i]);
				g.AddNode(n[i]);
			}
			foreach (var source in nodes)
			{
				foreach (var target in source.Successors)
				{
					g.AddEdge(new GraphVizEdge(source.UserIndex, target.UserIndex));
				}
				if (source.ImmediateDominator != null)
				{
					g.AddEdge(
						new GraphVizEdge(source.ImmediateDominator.UserIndex, source.UserIndex) {
							color = "green"
						});
				}
			}
			return g;
		}
	}
#endif
}
