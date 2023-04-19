// Copyright (c) 2022 Siegfried Pammer
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

using System.Linq;
using System.Text;
using System.Windows;

namespace ICSharpCode.ILSpy.Analyzers
{
	[ExportContextMenuEntry(Header = "Copy results", Category = "Analyze", Order = 200)]
	internal sealed class CopyAnalysisResultsContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.TreeView is AnalyzerTreeView && context.SelectedTreeNodes != null && context.SelectedTreeNodes.All(n => n is AnalyzerSearchTreeNode))
				return true;
			return false;
		}

		public bool IsEnabled(TextViewContext context)
		{
			return true;
		}

		public void Execute(TextViewContext context)
		{
			StringBuilder sb = new StringBuilder();
			if (context.SelectedTreeNodes != null)
			{
				foreach (var node in context.SelectedTreeNodes)
				{
					foreach (var item in node.Children)
					{
						sb.AppendLine(item.Text.ToString());
					}
				}
			}
			Clipboard.SetText(sb.ToString());
		}
	}
}
