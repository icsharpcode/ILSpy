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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// The dock must render one view per dockable identity, not one per slot, or two dockables
/// sharing a slot render each other's content. <see cref="DockableViewRecycling"/> achieves that
/// by having each dockable own its view (<see cref="IDockableViewOwner"/>) instead of pinning
/// views in an app-lifetime global cache. These tests pin that ownership contract.
/// </summary>
[TestFixture]
public class DockableViewOwnershipTests
{
	[AvaloniaTest]
	public void Build_returns_the_same_view_for_the_same_dockable()
	{
		var recycler = new DockableViewRecycling();
		var page = new ContentTabPage();

		var first = recycler.Build(page, null, null);
		var second = recycler.Build(page, first, null);

		first.Should().NotBeNull();
		second.Should().BeSameAs(first, "a dockable owns one view, reused on every resolution");
	}

	[AvaloniaTest]
	public void Build_returns_different_views_for_different_document_tabs()
	{
		var recycler = new DockableViewRecycling();
		var a = new ContentTabPage();
		var b = new ContentTabPage();

		var viewA = recycler.Build(a, null, null);
		var viewB = recycler.Build(b, null, null);

		((object?)viewB).Should().NotBeSameAs(viewA,
			"distinct document tabs must not share a view, or switching tabs renders the wrong content");
	}

	[AvaloniaTest]
	public void Build_stores_the_realized_view_on_the_owner()
	{
		var recycler = new DockableViewRecycling();
		var page = new ContentTabPage();
		((object?)page.OwnedView).Should().BeNull("the view is realized lazily on first resolution");

		var view = recycler.Build(page, null, null);

		((object?)page.OwnedView).Should().BeSameAs(view, "the dockable owns its realized view");
	}

	[AvaloniaTest]
	public void Reparenting_an_already_parented_view_does_not_throw()
	{
		// On a drag/tab switch the owned view is still parented in its previous slot; the
		// recycler must detach it before the dock re-parents it, or Avalonia throws
		// "already has a visual parent".
		var recycler = new DockableViewRecycling();
		var page = new ContentTabPage();
		var view = (Control)recycler.Build(page, null, null)!;

		var firstSlot = new Panel();
		firstSlot.Children.Add(view);
		((object?)view.Parent).Should().BeSameAs(firstSlot, "baseline: the view is parented");

		Control? resolved = null;
		var resolve = () => resolved = (Control?)recycler.Build(page, null, null);
		resolve.Should().NotThrow("the recycler detaches the view from its old slot before returning it");

		var secondSlot = new Panel();
		var reparent = () => secondSlot.Children.Add(resolved!);
		reparent.Should().NotThrow("the detached view is free to move into a new slot");
		((object?)resolved!.Parent).Should().BeSameAs(secondSlot);
	}

	[AvaloniaTest]
	public void Owner_views_never_enter_the_global_recycler_cache()
	{
		// The leak fix: a document's view is owned by its dockable (released when the tab is
		// dropped), not pinned in the app-lifetime, never-evicting global ControlRecycling cache.
		// So resolving an owner must NOT add an entry to the fallback stock recycler.
		var recycler = new DockableViewRecycling();
		var page = new ContentTabPage();

		recycler.Build(page, null, null);

		recycler.FallbackCache.TryGetValue(page, out _).Should().BeFalse(
			"owner views live on the dockable, not in the global cache that never evicts");
	}

	[AvaloniaTest]
	public void Switching_tabs_does_not_rebind_on_transient_null_or_same_page()
	{
		// Dock nulls and re-sets a document view's DataContext as it detaches/reattaches the
		// active tab on a switch. RebindPage must ignore the transient null and the same-page
		// re-set, or it re-runs ApplyContent -> DecompilerTextView.ApplyDocument and resets
		// scroll / foldings / caret on a mere tab switch. Only a genuine page change rebinds.
		var view = new ContentTabPageView();
		var pageA = new ContentTabPage();
		var pageB = new ContentTabPage();

		view.DataContext = pageA;
		view.BoundPage.Should().BeSameAs(pageA, "binds to the first page");

		view.DataContext = null;
		view.BoundPage.Should().BeSameAs(pageA, "a transient null DataContext must not unbind the owned view");

		view.DataContext = pageA;
		view.BoundPage.Should().BeSameAs(pageA, "re-setting the same page must be a no-op");

		view.DataContext = pageB;
		view.BoundPage.Should().BeSameAs(pageB, "a genuine page change must rebind");
	}
}
