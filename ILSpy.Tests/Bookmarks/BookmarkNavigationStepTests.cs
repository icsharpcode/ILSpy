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

using AwesomeAssertions;

using ICSharpCode.ILSpy.Bookmarks;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

// Next/Previous bookmark navigation must skip disabled bookmarks while still wrapping around.
[TestFixture]
public class BookmarkNavigationStepTests
{
	// Three bookmarks, the middle one disabled (the reported scenario).
	static readonly IReadOnlyList<bool> ThreeMiddleDisabled = new[] { true, false, true };

	[Test]
	public void Previous_from_the_third_skips_the_disabled_second()
	{
		// On index 2, "previous" must land on 0, not the disabled 1.
		BookmarksPaneModel.NextEnabledIndex(ThreeMiddleDisabled, selectedIndex: 2, delta: -1).Should().Be(0);
	}

	[Test]
	public void Next_from_the_first_skips_the_disabled_second()
	{
		BookmarksPaneModel.NextEnabledIndex(ThreeMiddleDisabled, selectedIndex: 0, delta: 1).Should().Be(2);
	}

	[Test]
	public void Next_wraps_around_past_a_trailing_disabled()
	{
		// [enabled, enabled, disabled]; next from index 1 wraps to 0 (2 is disabled).
		BookmarksPaneModel.NextEnabledIndex(new[] { true, true, false }, selectedIndex: 1, delta: 1).Should().Be(0);
	}

	[Test]
	public void No_selection_picks_the_first_enabled_forward_and_last_enabled_backward()
	{
		var list = new[] { false, true, true, false };
		BookmarksPaneModel.NextEnabledIndex(list, selectedIndex: -1, delta: 1).Should().Be(1);
		BookmarksPaneModel.NextEnabledIndex(list, selectedIndex: -1, delta: -1).Should().Be(2);
	}

	[Test]
	public void All_disabled_yields_nothing()
	{
		BookmarksPaneModel.NextEnabledIndex(new[] { false, false }, selectedIndex: 0, delta: 1).Should().BeNull();
	}

	[Test]
	public void Empty_list_yields_nothing()
	{
		BookmarksPaneModel.NextEnabledIndex(new bool[0], selectedIndex: -1, delta: 1).Should().BeNull();
	}
}
