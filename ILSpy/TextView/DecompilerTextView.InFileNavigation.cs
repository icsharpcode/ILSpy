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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// In-file navigation of the decompiler view: highlight usages of the symbol under the caret,
	/// Go to Line (Ctrl+G while the editor has focus), and the reference under the caret for
	/// caret-driven commands.
	/// </summary>
	public partial class DecompilerTextView
	{
		// Translucent so the caret-usage marks stay readable on top of the click-highlight marks.
		static readonly Color CaretUsageBackground = Color.FromArgb(0x50, 0x87, 0xCE, 0xFA);
		static readonly Color CaretUsageDefinitionBackground = Color.FromArgb(0x80, 0x87, 0xCE, 0xFA);

		readonly List<TextMarker> caretUsageMarks = new();
		// The segment the current caret-usage marks were computed for; re-computing is skipped while
		// the caret moves within that same segment.
		ReferenceSegment? caretUsageSegment;

		// The view holding keyboard focus, tracked so caret-driven commands can find it without
		// walking (or constructing) a window.
		static WeakReference<DecompilerTextView>? focusedView;

		/// <summary>The decompiler view that currently holds keyboard focus, or null.</summary>
		internal static DecompilerTextView? FocusedView
			=> focusedView != null && focusedView.TryGetTarget(out var view) && view.IsKeyboardFocusWithin ? view : null;

		/// <summary>Marks painted by the highlight-usages-at-caret feature (exposed for tests).</summary>
		internal IReadOnlyList<TextMarker> CaretUsageMarks => caretUsageMarks;

		void SetupInFileNavigation()
		{
			Editor.TextArea.Caret.PositionChanged += (_, _) => UpdateCaretUsageHighlight();
			if (currentDisplaySettings is INotifyPropertyChanged settings)
			{
				settings.PropertyChanged += (_, e) => {
					if (e.PropertyName == nameof(DisplaySettings.HighlightUsagesAtCaret))
						UpdateCaretUsageHighlight();
				};
			}
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
		/// The reference segment under the caret: one containing the caret offset, or ending right
		/// before it (caret just after an identifier). Hover-only segments are skipped.
		/// </summary>
		internal ReferenceSegment? GetReferenceSegmentAtCaret()
		{
			if (DataContext is not DecompilerTabPageModel { References: { } references })
				return null;
			int offset = Editor.TextArea.Caret.Offset;
			return Pick(references.FindSegmentsContaining(offset))
				?? (offset > 0 ? Pick(references.FindSegmentsContaining(offset - 1)) : null);

			static ReferenceSegment? Pick(IEnumerable<ReferenceSegment> segments)
				=> segments.FirstOrDefault(s => s.Reference != null && s.Kind != ReferenceMode.HoverOnly);
		}

		void UpdateCaretUsageHighlight()
		{
			if (currentDisplaySettings is not { HighlightUsagesAtCaret: true }
				|| DataContext is not DecompilerTabPageModel { References: { } references }
				|| GetReferenceSegmentAtCaret() is not { } segment
				|| !IsCaretHighlightable(segment))
			{
				ClearCaretUsageMarks();
				return;
			}
			if (ReferenceEquals(segment, caretUsageSegment) && caretUsageMarks.Count > 0)
				return;

			ClearCaretUsageMarks();
			caretUsageSegment = segment;
			int textLength = Editor.Document.TextLength;
			foreach (var r in references)
			{
				if (r.Kind == ReferenceMode.HoverOnly || !AreSameReference(segment.Reference!, r.Reference))
					continue;
				if (r.StartOffset < 0 || r.EndOffset > textLength)
					continue;
				var mark = textMarkerService.Create(r.StartOffset, r.Length);
				mark.BackgroundColor = r.IsDefinition ? CaretUsageDefinitionBackground : CaretUsageBackground;
				caretUsageMarks.Add(mark);
			}
		}

		// Locals and parameters, members, types and unresolved entity references. Opcodes and other
		// link targets are excluded: marking every occurrence of an IL opcode would be noise.
		static bool IsCaretHighlightable(ReferenceSegment segment)
			=> segment.Kind == ReferenceMode.LocalHighlight
				|| segment.Reference is IMember or IType or EntityReference;

		void ClearCaretUsageMarks()
		{
			caretUsageSegment = null;
			foreach (var mark in caretUsageMarks)
				textMarkerService.Remove(mark);
			caretUsageMarks.Clear();
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

		// Applies a captured view state to the editor right away; installed on the bound model as
		// DecompilerTabPageModel.ApplyViewState.
		void ApplyViewStateNow(DecompilerTextViewState state)
		{
			RestoreOrResetViewState(state);
			Editor.TextArea.Focus();
		}
	}
}
