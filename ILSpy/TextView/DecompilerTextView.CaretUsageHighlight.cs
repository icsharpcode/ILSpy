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


using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Avalonia.Media;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Highlight usages at caret: while the caret rests on a symbol, every occurrence of that
	/// symbol in the document is marked. Independent of the click highlight, which keeps its own
	/// marks.
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

		/// <summary>Marks painted by the highlight-usages-at-caret feature (exposed for tests).</summary>
		internal IReadOnlyList<TextMarker> CaretUsageMarks => caretUsageMarks;

		void SetupCaretUsageHighlight()
		{
			Editor.TextArea.Caret.PositionChanged += (_, _) => UpdateCaretUsageHighlight();
			if (currentDisplaySettings is INotifyPropertyChanged settings)
			{
				settings.PropertyChanged += (_, e) => {
					if (e.PropertyName == nameof(DisplaySettings.HighlightUsagesAtCaret))
						UpdateCaretUsageHighlight();
				};
			}
		}

		/// <summary>
		/// The reference segment under the caret: one containing the caret offset, or ending right
		/// before it (caret just after an identifier). Hover-only segments are skipped.
		/// </summary>
		ReferenceSegment? GetReferenceSegmentAtCaret()
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
	}
}
