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

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Right-click on an analyser-error row → "Copy error message" — puts the full
	/// <see cref="AnalyzerErrorNode.Details"/> payload (typically <c>ex.ToString()</c>,
	/// including stack trace) on the clipboard. Only surfaces when every selected node
	/// is an error node, so it doesn't crowd the regular result-row menu.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.CopyErrorMessage), Order = 9300)]
	[Shared]
	public sealed class CopyAnalyzerErrorContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return false;
			return nodes.All(n => n is AnalyzerErrorNode);
		}

		public bool IsEnabled(TextViewContext context) => IsVisible(context);

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return;
			var builder = new StringBuilder();
			foreach (var node in nodes.OfType<AnalyzerErrorNode>())
			{
				if (builder.Length > 0)
					builder.AppendLine();
				builder.AppendLine(node.Details);
			}
			if (builder.Length == 0)
				return;
			TryWriteToClipboard(builder.ToString());
		}

		static void TryWriteToClipboard(string payload)
		{
			if (UiContext.Clipboard is not { } clipboard)
				return;
			_ = clipboard.SetTextAsync(payload);
		}
	}
}
