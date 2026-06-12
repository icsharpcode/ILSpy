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

using System.Collections.Generic;
using System.Linq;

using AvaloniaEdit.Folding;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Captures and restores the set of expanded code-folding regions for a single
	/// decompiled view, so Back/Forward navigation can put the user back exactly where
	/// they left off (caret + scroll are handled separately by the LastKnown* /
	/// Pending* pair on <see cref="DecompilerTabPageModel"/>).
	/// <para>
	/// The protective semantics mirror the WPF implementation: a checksum over the
	/// full folding layout (every folding's offsets, regardless of fold state) acts
	/// as an "is this still the same document" gate. If the new document's foldings
	/// don't checksum to the saved value, restoration declines and the new view
	/// keeps its default fold state — otherwise a stale snapshot could accidentally
	/// expand random regions of a shifted document.
	/// </para>
	/// </summary>
	public static class FoldingsViewState
	{
		/// <summary>
		/// A frozen snapshot of which foldings were expanded at a point in time. The
		/// <see cref="Expanded"/> list is the user-actionable subset; <see cref="Checksum"/>
		/// is the layout fingerprint used by <see cref="Restore"/> to refuse mismatched
		/// restorations.
		/// </summary>
		public readonly record struct Snapshot(IReadOnlyList<(int Start, int End)> Expanded, int Checksum);

		/// <summary>
		/// Tuple-form capture used by tests and by the FoldingSection-form overload. Walks
		/// the input once and accumulates two outputs: the list of (Start, End) pairs whose
		/// IsFolded is false (the "currently expanded" subset), plus a checksum over every
		/// folding's offsets in iteration order.
		/// </summary>
		public static Snapshot Capture(IEnumerable<(int Start, int End, bool IsFolded)> foldings)
		{
			var expanded = new List<(int Start, int End)>();
			int checksum = 0;
			foreach (var (start, end, isFolded) in foldings)
			{
				checksum = unchecked(checksum + start * 3 - end);
				if (!isFolded)
					expanded.Add((start, end));
			}
			return new Snapshot(expanded, checksum);
		}

		/// <summary>
		/// Live-folding-section adapter that delegates to the tuple form. Used by the editor
		/// surface to snapshot its current AvaloniaEdit FoldingManager state.
		/// </summary>
		public static Snapshot Capture(IEnumerable<FoldingSection> foldings)
			=> Capture(foldings.Select(f => (f.StartOffset, f.EndOffset, f.IsFolded)));

		/// <summary>
		/// Applies <paramref name="saved"/> to the freshly-built <paramref name="newFoldings"/>
		/// list (which is about to be installed into a FoldingManager). Returns <c>true</c>
		/// when the saved layout matches the new layout and <see cref="NewFolding.DefaultClosed"/>
		/// has been adjusted to recreate the expanded subset; <c>false</c> when the checksum
		/// mismatch caused restoration to bail out (in which case the new list is untouched).
		/// </summary>
		public static bool Restore(IList<NewFolding> newFoldings, Snapshot saved)
		{
			int checksum = 0;
			foreach (var folding in newFoldings)
				checksum = unchecked(checksum + folding.StartOffset * 3 - folding.EndOffset);
			if (checksum != saved.Checksum)
				return false;
			foreach (var folding in newFoldings)
			{
				bool wasExpanded = false;
				foreach (var (start, end) in saved.Expanded)
				{
					if (start == folding.StartOffset && end == folding.EndOffset)
					{
						wasExpanded = true;
						break;
					}
				}
				folding.DefaultClosed = !wasExpanded;
			}
			return true;
		}
	}
}
