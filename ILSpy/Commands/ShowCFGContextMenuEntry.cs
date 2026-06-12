// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

#if DEBUG
using System;
using System.Collections.Generic;
using System.Composition;

using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;

using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// DEBUG-only: right-click a <see cref="BlockContainer"/> reference in the IL output
	/// → "DEBUG -- Show CFG". Builds the control-flow graph for the containing method,
	/// writes it as a GraphViz <c>.gv</c> file in the temp dir, shells out to <c>dot</c>
	/// to render PNG, and opens the result with the OS default image viewer.
	/// </summary>
	[ExportContextMenuEntry(Header = "DEBUG -- Show CFG", Category = "Diagnostics", Order = 9000)]
	[Shared]
	internal sealed class ShowCFGContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
			=> context.Reference?.Reference is BlockContainer;

		public bool IsEnabled(TextViewContext context)
			=> context.Reference?.Reference is BlockContainer;

		public void Execute(TextViewContext context)
		{
			try
			{
				var container = (BlockContainer)context.Reference!.Reference!;
				var cfg = new ControlFlowGraph(container);
				ExportGraph(cfg.Nodes).Show();
			}
			catch (Exception ex)
			{
				// Most likely failure: `dot` isn't on PATH. Surface the full exception via
				// stderr/console so the developer running a DEBUG build sees it (there's no
				// MessageBox primitive on the Avalonia side and this entry is dev-only).
				System.Diagnostics.Debug.WriteLine("Error generating CFG — requires GraphViz dot on PATH: " + ex);
			}
		}

		internal static GraphVizGraph ExportGraph(IReadOnlyList<ControlFlowNode> nodes, Func<ControlFlowNode, string?>? labelFunc = null)
		{
			labelFunc ??= node => {
				var block = node.UserData as Block;
				return block != null ? block.Label : node.UserData?.ToString();
			};
			var g = new GraphVizGraph();
			var n = new GraphVizNode[nodes.Count];
			for (int i = 0; i < n.Length; i++)
			{
				n[i] = new GraphVizNode(nodes[i].UserIndex) {
					shape = "box",
					label = labelFunc(nodes[i]),
				};
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
					g.AddEdge(new GraphVizEdge(source.ImmediateDominator.UserIndex, source.UserIndex) {
						color = "green",
					});
				}
			}
			return g;
		}
	}
}
#endif
