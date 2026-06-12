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

using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Right-click → Copy. Lives in the editor's context menu only (gated by
	/// <see cref="TextViewContext.TextView"/>); enabled when the AvaloniaEdit selection is
	/// non-empty. Keyboard Ctrl+C is handled by AvaloniaEdit natively — this entry exists
	/// for the right-click affordance on machines without a keyboard or for discoverability.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.Copy), Category = nameof(Resources.Editor), Order = 100)]
	[Shared]
	public sealed class CopyContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => context.TextView != null;

		public bool IsEnabled(TextViewContext context)
			=> context.TextView is { } view && view.Editor.SelectionLength > 0;

		public void Execute(TextViewContext context)
		{
			// Copy as text + syntax/semantic-coloured HTML; fall back to plain copy if nothing selected.
			if (context.TextView is { } view && !HtmlClipboardCopy.Copy(view.Editor, view.SemanticHighlightingModel))
				view.Editor.Copy();
		}
	}

	/// <summary>
	/// Right-click → Select All. Mirrors WPF's SelectAllContextMenuEntry. Keyboard Ctrl+A is
	/// handled natively by AvaloniaEdit; this entry surfaces the same action from the
	/// right-click menu.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.Select), Category = nameof(Resources.Editor), Order = 110)]
	[Shared]
	public sealed class SelectAllContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => context.TextView != null;

		public bool IsEnabled(TextViewContext context) => context.TextView != null;

		public void Execute(TextViewContext context)
			=> context.TextView?.Editor.SelectAll();
	}
}
