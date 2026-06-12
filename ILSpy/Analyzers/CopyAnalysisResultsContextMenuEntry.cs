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

using System.Composition;
using System.Linq;
using System.Text;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Right-click on an analyser-result row → "Copy results" — puts each child node's
	/// Text on the clipboard, one per line. Visible only when the selection is an
	/// <see cref="AnalyzerSearchTreeNode"/> (the headers under an analysed entity).
	/// </summary>
	[ExportContextMenuEntry(Header = "Copy results", Order = 9100)]
	[Shared]
	public sealed class CopyAnalysisResultsContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.All(n => n is AnalyzerSearchTreeNode);
		}

		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return;
			var builder = new StringBuilder();
			foreach (var node in nodes.OfType<AnalyzerSearchTreeNode>())
			{
				foreach (var child in node.Children)
				{
					var text = child.Text?.ToString();
					if (!string.IsNullOrEmpty(text))
						builder.AppendLine(text);
				}
			}
			if (builder.Length == 0)
				return;
			TryWriteToClipboard(builder.ToString());
		}

		static void TryWriteToClipboard(string payload)
		{
			if (UiContext.Clipboard is not { } clipboard)
				return;
			// SetTextAsync is the extension method on IClipboard — needs Avalonia.Input.Platform.
			_ = clipboard.SetTextAsync(payload);
		}
	}
}
