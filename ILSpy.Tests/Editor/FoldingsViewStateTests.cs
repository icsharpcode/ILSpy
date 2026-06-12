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

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Tests for the foldings-persistence helper that backs Back/Forward navigation's
/// "remember which regions the user had expanded" behaviour. The math mirrors WPF's
/// <c>DecompilerTextViewState.SaveFoldingsState</c> / <c>RestoreFoldings</c> so that
/// the protective "skip on layout mismatch" semantics carry over identically.
/// </summary>
[TestFixture]
public class FoldingsViewStateTests
{
	[Test]
	public void Capture_Records_Offsets_Of_Expanded_Foldings_Only()
	{
		// The saved subset is the list of foldings the user has open — folded foldings are
		// already at their default state and don't need preserving. Mirrors WPF's
		// `foldings.Where(f => !f.IsFolded)` filter at the heart of SaveFoldingsState.

		// Arrange — four foldings, two folded and two expanded. Offsets chosen to disambiguate.
		var foldings = new[] {
			(Start: 10, End: 50, IsFolded: false),
			(Start: 60, End: 100, IsFolded: true),
			(Start: 110, End: 200, IsFolded: false),
			(Start: 210, End: 250, IsFolded: true),
		};

		// Act — capture the snapshot.
		var snapshot = FoldingsViewState.Capture(foldings);

		// Assert — only the expanded pair is recorded, in input order.
		snapshot.Expanded.Should().Equal(new[] { (10, 50), (110, 200) });
	}

	[Test]
	public void Capture_Checksum_Is_The_Same_For_Layouts_With_Identical_Offsets()
	{
		// The checksum protects restoration against running on a different document. Two
		// foldings lists with the same `(Start, End)` pairs in the same order must produce
		// equal checksums regardless of which folds are open vs closed.

		// Arrange — same offsets, different IsFolded states.
		var a = new[] { (10, 50, false), (60, 100, true), (110, 200, false) };
		var b = new[] { (10, 50, true), (60, 100, false), (110, 200, true) };

		// Act — capture both.
		var snapA = FoldingsViewState.Capture(a);
		var snapB = FoldingsViewState.Capture(b);

		// Assert — checksums match (layout is identical even though state isn't).
		snapA.Checksum.Should().Be(snapB.Checksum);
	}

	[Test]
	public void Capture_Checksum_Differs_When_Any_Offset_Shifts()
	{
		// A single offset move yields a different checksum — that's the cue that the new
		// document doesn't match the saved state, and restoration must skip. Otherwise
		// "expanded at (10, 50)" might bogusly re-expand "(10, 51)" in the new document.

		// Arrange — identical layouts except one folding shifted by one byte.
		var original = new[] { (10, 50, false), (60, 100, false) };
		var shifted = new[] { (10, 51, false), (60, 100, false) };

		// Act — capture both.
		var checksumOriginal = FoldingsViewState.Capture(original).Checksum;
		var checksumShifted = FoldingsViewState.Capture(shifted).Checksum;

		// Assert — checksums differ.
		checksumShifted.Should().NotBe(checksumOriginal);
	}

	[Test]
	public void Restore_Reopens_Saved_Expanded_Foldings_When_Checksum_Matches()
	{
		// On Back navigation: the freshly-built foldings list comes back default-open
		// (DefaultClosed=false on every NewFolding). Restore must close every folding that
		// wasn't in the saved expanded set, and leave the saved-expanded ones open. This is
		// the inverse of how WPF restores — we don't track "what was closed", only "what was
		// open" — so anything not on the open list defaults to closed.

		// Arrange — three new foldings (all start at DefaultClosed=false), with a saved
		// snapshot saying only the middle one was expanded.
		var newFoldings = new List<NewFolding> {
			new(10, 50),
			new(60, 100),
			new(110, 200),
		};
		var saved = FoldingsViewState.Capture(new[] {
			(10, 50, true),     // was folded
			(60, 100, false),   // was expanded
			(110, 200, true),   // was folded
		});

		// Act — restore against the matching new-foldings layout.
		var restored = FoldingsViewState.Restore(newFoldings, saved);

		// Assert — restoration ran (true return), and the per-folding DefaultClosed reflects
		// the saved state: open in the middle, closed at the ends.
		restored.Should().BeTrue();
		newFoldings[0].DefaultClosed.Should().BeTrue("(10, 50) was not in the saved expanded set");
		newFoldings[1].DefaultClosed.Should().BeFalse("(60, 100) was saved as expanded");
		newFoldings[2].DefaultClosed.Should().BeTrue("(110, 200) was not in the saved expanded set");
	}

	[Test]
	public void Restore_Returns_False_And_Touches_Nothing_When_Checksum_Mismatch()
	{
		// The new document's foldings layout doesn't match what was saved — restoration
		// must bail out without touching `DefaultClosed`. A partial restore over the wrong
		// layout would expand random regions of the new document.

		// Arrange — new foldings with shifted offsets vs. the captured snapshot's source.
		var newFoldings = new List<NewFolding> {
			new(15, 50) { DefaultClosed = true },    // shifted start
			new(60, 100) { DefaultClosed = true },
		};
		var saved = FoldingsViewState.Capture(new[] {
			(10, 50, false),                          // different start
			(60, 100, false),
		});

		// Act — try to restore against the mismatched layout.
		var restored = FoldingsViewState.Restore(newFoldings, saved);

		// Assert — restoration declined; DefaultClosed values are exactly as the caller set
		// them (both true). A regression that "best-effort restored anyway" would flip one
		// of them to false here.
		restored.Should().BeFalse();
		newFoldings[0].DefaultClosed.Should().BeTrue();
		newFoldings[1].DefaultClosed.Should().BeTrue();
	}

	[Test]
	public void Restore_Treats_Empty_New_And_Saved_As_A_Matching_NoOp()
	{
		// Edge case: both lists empty (e.g., the new tab has no foldings at all). The
		// checksum is zero on both sides, so restore "matches" — but with nothing to do.
		// It must return true so the caller doesn't log a spurious "layout changed" warning.

		// Arrange — empty new foldings, empty saved snapshot.
		var newFoldings = new List<NewFolding>();
		var saved = FoldingsViewState.Capture(System.Array.Empty<(int, int, bool)>());

		// Act + Assert — restoration returns true; new list stays empty.
		FoldingsViewState.Restore(newFoldings, saved).Should().BeTrue();
		newFoldings.Should().BeEmpty();
	}

	[Test]
	public void Captured_Expanded_List_Is_A_Snapshot_Not_A_Live_View()
	{
		// The Expanded list must not mutate if the source enumeration is reused after the
		// capture. Without this, a caller passing a List<> and then mutating it would see
		// their navigation history quietly change.

		// Arrange — capture into a snapshot, then mutate the source list.
		var source = new List<(int, int, bool)> {
			(10, 50, false),
			(60, 100, false),
		};
		var snapshot = FoldingsViewState.Capture(source);
		source.Clear();

		// Assert — the captured snapshot still has the original entries.
		snapshot.Expanded.Should().Equal(new[] { (10, 50), (60, 100) });
	}
}
