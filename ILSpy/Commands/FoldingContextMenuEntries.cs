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

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click in the decompiled code → "Toggle folding": folds/unfolds the innermost block under
	/// the clicked line. The menu equivalent of Ctrl+M. Only shown when the document actually has foldings.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources._ToggleFolding), Category = nameof(Resources.Folding), Order = 300)]
	[Shared]
	public sealed class ToggleFoldingContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => context.TextView?.HasFoldings == true;

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (context.TextView is not { } view)
				return;
			// Toggle the fold the user right-clicked on. TextLocation is the offset under the pointer;
			// fall back to the caret only when the menu was raised without a recorded click position.
			if (context.TextLocation is { } offset)
				view.ToggleFoldingAt(offset);
			else
				view.ToggleFoldingAtCaret();
		}
	}

	/// <summary>
	/// Right-click in the decompiled code → "Toggle all folding": collapses every fold when any is open,
	/// otherwise expands them all. The menu equivalent of Ctrl+Shift+M.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.ToggleFolding), Category = nameof(Resources.Folding), Order = 310)]
	[Shared]
	public sealed class ToggleAllFoldingsContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => context.TextView?.HasFoldings == true;

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context) => context.TextView?.ToggleAllFoldings();
	}
}
