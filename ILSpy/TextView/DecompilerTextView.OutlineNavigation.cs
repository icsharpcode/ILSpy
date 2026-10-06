// Copyright (c) 2026 Piero Viano
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

using Avalonia.Threading;

namespace ICSharpCode.ILSpy.TextView
{
	public partial class DecompilerTextView
	{
		/// <summary>
		/// Backs <see cref="DecompilerTabPageModel.NavigateToOffset"/>: places the caret at
		/// <paramref name="offset"/>, centres its line, flashes the caret highlight and focuses the
		/// editor. Offsets outside the current document are ignored.
		/// </summary>
		void NavigateToOffsetNow(int offset)
		{
			var document = Editor.Document;
			if (document == null || offset < 0 || offset > document.TextLength)
				return;
			Editor.TextArea.Caret.Offset = offset;
			Editor.TextArea.Caret.BringCaretToView();
			int line = document.GetLineByOffset(offset).LineNumber;
			// Background priority lets a just-applied document finish measuring before centring.
			Dispatcher.UIThread.Post(() => CenterLineInView(document, line), DispatcherPriority.Background);
			CaretHighlightAdorner.DisplayCaretHighlightAnimation(Editor.TextArea);
			Dispatcher.UIThread.Post(() => Editor.TextArea.Focus());
		}
	}
}
