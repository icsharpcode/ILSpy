using System;
using System.Windows;

using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;

namespace ICSharpCode.ILSpy.Commands
{
#if DEBUG
	[ExportContextMenuEntry(Header = "DEBUG -- Show CFG")]
	internal class ShowCFGContextMenuEntry : IContextMenuEntry
	{
		public void Execute(TextViewContext context)
		{
			try
			{
				var container = (BlockContainer)context.Reference.Reference;
				var cfg = new ControlFlowGraph(container);
				ControlFlowNode.ExportGraph(cfg.cfg).Show();
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
	}
#endif
}
