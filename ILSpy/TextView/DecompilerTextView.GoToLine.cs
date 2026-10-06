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


using System;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

using ICSharpCode.ILSpy.Navigation;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Go to Line in the decompiler view: Ctrl+G while the editor has focus opens a prompt that
	/// moves the caret to a line (and optional column).
	/// </summary>
	public partial class DecompilerTextView
	{
		// The view holding keyboard focus, tracked so the Go to Line command can find it without
		// walking (or constructing) a window.
		static WeakReference<DecompilerTextView>? focusedView;

		/// <summary>The decompiler view that currently holds keyboard focus, or null.</summary>
		internal static DecompilerTextView? FocusedView
			=> focusedView != null && focusedView.TryGetTarget(out var view) && view.IsKeyboardFocusWithin ? view : null;

		void SetupGoToLine()
		{
			// Ctrl+G is scoped to the editor: elsewhere (e.g. metadata grids) the same gesture means
			// "Go to token". Tunnel so the gesture wins before AvaloniaEdit's own key handling.
			AddHandler(KeyDownEvent, OnGoToLineKeyDown, RoutingStrategies.Tunnel);
			PropertyChanged += (_, e) => {
				if (e.Property == IsKeyboardFocusWithinProperty && IsKeyboardFocusWithin)
					focusedView = new WeakReference<DecompilerTextView>(this);
			};
		}

		void OnGoToLineKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.G && e.KeyModifiers == KeyModifiers.Control && Editor.TextArea.IsKeyboardFocusWithin)
			{
				e.Handled = ShowGoToLinePrompt();
			}
		}

		/// <summary>
		/// Moves the caret to <paramref name="line"/> (1-based) and optional
		/// <paramref name="column"/> (1-based), both clamped to the document, centres the line and
		/// plays the line highlight. Returns false when there is no document to move in.
		/// </summary>
		internal bool GoToLine(int line, int? column = null)
		{
			var document = Editor.Document;
			if (document == null || document.LineCount == 0)
				return false;
			line = Math.Clamp(line, 1, document.LineCount);
			var documentLine = document.GetLineByNumber(line);
			int col = Math.Clamp(column ?? 1, 1, documentLine.Length + 1);
			Editor.TextArea.Caret.Offset = documentLine.Offset + col - 1;
			Editor.TextArea.Caret.BringCaretToView();
			Dispatcher.UIThread.Post(() => {
				if (!ReferenceEquals(Editor.Document, document) || line > document.LineCount)
					return;
				CenterLineInView(document, line);
				LineHighlightAdorner.DisplayLineHighlight(Editor.TextArea, line);
			}, DispatcherPriority.Background);
			Editor.TextArea.Focus();
			return true;
		}

		/// <summary>
		/// Opens the Go to Line prompt owned by this view's window. Returns false when the view has
		/// no window or no document.
		/// </summary>
		internal bool ShowGoToLinePrompt()
		{
			if (TopLevel.GetTopLevel(this) is not Window owner || Editor.Document is not { LineCount: > 0 } document)
				return false;
			var prompt = new GoToLineWindow(document.LineCount, Editor.TextArea.Caret.Line,
				(line, column) => GoToLine(line, column));
			prompt.Show(owner);
			return true;
		}
	}
}
