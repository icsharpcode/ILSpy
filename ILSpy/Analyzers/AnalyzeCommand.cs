// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using System.ComponentModel.Composition;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Analyzers
{
	[ExportContextMenuEntry(Header = nameof(Resources.Analyze), Icon = "Images/Search", Category = nameof(Resources.Analyze), InputGestureText = "Ctrl+R", Order = 100)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	internal sealed class AnalyzeContextMenuCommand : IContextMenuEntry
	{
		private static readonly AnalyzerTreeViewModel AnalyzerTreeView = App.ExportProvider.GetExportedValue<AnalyzerTreeViewModel>();

		public bool IsVisible(TextViewContext context)
		{
			if (context.TreeView is AnalyzerTreeView && context.SelectedTreeNodes != null && context.SelectedTreeNodes.All(n => n.Parent.IsRoot))
				return false;
			if (context.SelectedTreeNodes == null)
				return context.Reference != null && IsValidReference(context.Reference.Reference);
			return context.SelectedTreeNodes.All(n => n is IMemberTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
			{
				return context.Reference is { Reference: IEntity };
			}
			return context.SelectedTreeNodes
				.OfType<IMemberTreeNode>()
				.All(node => IsValidReference(node.Member));
		}

		static bool IsValidReference(object reference)
		{
			return reference is IEntity and not IField { IsConst: true };
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes != null)
			{
				foreach (var node in context.SelectedTreeNodes.OfType<IMemberTreeNode>().ToArray())
				{
					AnalyzerTreeView.Analyze(node.Member);
				}
			}
			else if (context.Reference is { Reference: IEntity entity })
			{
				AnalyzerTreeView.Analyze(entity);
			}
		}
	}

	internal sealed class AnalyzeCommand : SimpleCommand
	{
		private static readonly AnalyzerTreeViewModel AnalyzerTreeView = App.ExportProvider.GetExportedValue<AnalyzerTreeViewModel>();

		public override bool CanExecute(object parameter)
		{
			return MainWindow.Instance.SelectedNodes.All(n => n is IMemberTreeNode);
		}

		public override void Execute(object parameter)
		{
			foreach (var node in MainWindow.Instance.SelectedNodes.OfType<IMemberTreeNode>())
			{
				AnalyzerTreeView.Analyze(node.Member);
			}
		}
	}
}
