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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using Dock.Model.Core;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Docking;

/// <summary>
/// Save-side pins for the Avalonia layout-persistence wiring. The Save path writes a
/// JSON sidecar next to <c>ILSpy.xml</c> using <c>Dock.Serializer.SystemTextJson</c>,
/// with a custom <c>JsonTypeInfoResolver</c> modifier that strips back-reference
/// properties (Owner / Factory / Task-shaped properties) so the dock tree doesn't
/// surface a real cycle that <see cref="System.Text.Json.Serialization.ReferenceHandler.Preserve"/>
/// can't bridge across Dock's per-element <c>JsonConverterList&lt;T&gt;</c> calls.
/// <para>
/// Load currently returns null on this same layout because the emitted JSON has both
/// <c>$id</c> (from Preserve) and <c>$type</c> (from polymorphism) on the same objects,
/// and System.Text.Json's reader requires <c>$type</c> to come first — a documented
/// incompatibility we'd need either a Dock-side fix or a Newtonsoft swap to resolve.
/// The fallback to <see cref="ILSpyDockFactory.CreateLayout"/> on null load means the
/// app's user-visible behaviour is "layout still resets each restart" — same as before
/// — but the infrastructure for the eventual full fix is in place.
/// </para>
/// </summary>
[TestFixture]
public class LayoutPersistenceTests
{
	[AvaloniaTest]
	public async Task SaveLayout_Followed_By_LoadLayout_Round_Trips_The_Live_Layout()
	{
		_ = await TestHarness.BootAsync();

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		// Search is hidden by default; surface it so the round-trip covers a tool pane that
		// lives in a materialised-on-demand dock, not just the assembly tree.
		dockWorkspace.ShowToolPane(ICSharpCode.ILSpy.Search.SearchPaneModel.PaneContentId);
		var path = Path.Combine(Path.GetTempPath(), $"ILSpy.Layout.test.{System.Guid.NewGuid():N}.json");

		try
		{
			ILSpyDockFactory.SaveLayout(path, dockWorkspace.Layout);

			File.Exists(path).Should().BeTrue("SaveLayout must write the file");
			new FileInfo(path).Length.Should().BeGreaterThan(0);

			// Inspect the saved JSON so the test pins the actual shape, not just
			// "anything non-null came back". The persisted layout MUST:
			//   - have $type discriminators (polymorphism intact)
			//   - have no $id markers (cycle handling stripped)
			//   - reference the live tool pane IDs (so locator can re-attach on load)
			var json = File.ReadAllText(path);
			json.Should().Contain("\"$type\":", "polymorphism discriminator must be emitted");
			json.Should().NotContain("\"$id\":", "ReferenceHandler.Preserve must be off — $id breaks load");
			json.Should().Contain("\"Id\": \"AssemblyTree\"",
				"the live AssemblyTreeModel singleton ID must persist so LoadLayout can re-attach it");
			json.Should().Contain("\"Id\": \"Search\"",
				"SearchPaneModel's singleton ID must persist");

			// Real Load — and the loaded layout must be structurally usable, not just
			// non-null. Specifically: a RootDock with VisibleDockables containing the
			// proportional dock tree, AND the tool panes resolved as the live singletons
			// (CreateObject hook should have returned the MEF instances, not fresh ones).
			var registryForRoundTrip = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Commands.ToolPaneRegistry>();
			var roundTrip = new ILSpyDockFactory(registryForRoundTrip).LoadLayout(path);
			((object?)roundTrip).Should().NotBeNull("LoadLayout must materialise the saved root dock");
			var dockables = roundTrip!.VisibleDockables;
			(dockables == null ? 0 : dockables.Count)
				.Should().BeGreaterThan(0, "loaded root must have visible dockables");

			// Walk every dockable in the loaded tree. Every ToolPaneModel-typed entry
			// must be reference-equal to the live MEF singleton — that's what proves
			// the CreateObject hook fired during deserialization, not a fresh ctor.
			var liveAssemblyTree = AppComposition.Current.GetExport<ICSharpCode.ILSpy.AssemblyTree.AssemblyTreeModel>();
			var liveSearch = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Search.SearchPaneModel>();
			var allLoaded = Flatten(roundTrip).ToList();
			allLoaded.OfType<ICSharpCode.ILSpy.AssemblyTree.AssemblyTreeModel>().Should().Contain(liveAssemblyTree,
				"loaded AssemblyTreeModel must be the live MEF singleton, not a fresh instance");
			allLoaded.OfType<ICSharpCode.ILSpy.Search.SearchPaneModel>().Should().Contain(liveSearch,
				"loaded SearchPaneModel must be the live MEF singleton, not a fresh instance");
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}

	[AvaloniaTest]
	public async Task Second_Launch_Restores_MainTab_And_Documents_So_Tree_Selections_Still_Decompile()
	{
		// Repro of the user-reported "decompile view is empty on second launch" bug.
		// First launch: factory.CreateLayout() runs, which sets factory.MainTab and
		// factory.Documents alongside building the layout. SaveLayout writes the JSON.
		// Second launch: LoadLayout returns a fresh IRootDock — but factory.MainTab /
		// factory.Documents stay null, so ShowSelectedNode silently no-ops at
		// `if (factory.MainTab is not { } main) return;` and the user sees an empty
		// editor when they click a tree node.

		_ = await TestHarness.BootAsync();

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();
		var path = Path.Combine(Path.GetTempPath(), $"ILSpy.Layout.test.{System.Guid.NewGuid():N}.json");

		try
		{
			ILSpyDockFactory.SaveLayout(path, dockWorkspace.Layout);

			// Simulate the second-launch path: a NEW factory loads the saved layout
			// instead of creating one. The factory MUST still expose MainTab + Documents
			// so DockWorkspace can populate decompile content into them.
			var registry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Commands.ToolPaneRegistry>();
			var factory = new ILSpyDockFactory(registry);
			var loaded = factory.LoadLayout(path);

			((object?)loaded).Should().NotBeNull("LoadLayout must materialise the saved root dock");
			((object?)factory.Documents).Should().NotBeNull(
				"after LoadLayout the factory must expose the loaded DocumentDock as Documents");
			((object?)factory.MainTab).Should().NotBeNull(
				"after LoadLayout the factory must expose the loaded MainTab so ShowSelectedNode can populate decompile content");
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}

	static IEnumerable<IDockable> Flatten(IDockable root)
	{
		yield return root;
		if (root is IDock d && d.VisibleDockables is { } kids)
			foreach (var k in kids)
				foreach (var f in Flatten(k))
					yield return f;
	}

	[AvaloniaTest]
	public async Task Documents_Beyond_MainTab_Are_Not_Persisted_Across_Save_And_Load()
	{
		// The user-reported bug: opening multiple document tabs in a session and restarting
		// brings them all back "broken except the first one". Reason: every
		// ContentTabPage in the DocumentDock gets serialised, but their Content is a
		// runtime-only TabPageModel (live decompiler/metadata/compare state, Tasks,
		// CancellationTokenSources, IEntity refs) that can't survive a process restart —
		// Activator reconstructs the ContentTabPage shell with Content=null, and the
		// chrome shows a blank tab.
		//
		// Correct behaviour: persist the *structural* layout (tool docks, splitter ratios,
		// pinned panes) but treat document children as session-only. On load the
		// DocumentDock comes back with a single fresh MainTab; user's tree selection
		// re-projects through ShowSelectedNode as on first launch.

		_ = await TestHarness.BootAsync();

		var dockWorkspace = AppComposition.Current.GetExport<DockWorkspace>();

		// Open two extra documents on top of the persistent MainTab. Content type is
		// irrelevant for this test — the bug is structural, about ContentTabPage shells
		// being persisted at all — so any ContentPageModel will do.
		dockWorkspace.OpenNewTab(new ICSharpCode.ILSpy.TextView.DecompilerTabPageModel());
		dockWorkspace.OpenNewTab(new ICSharpCode.ILSpy.TextView.DecompilerTabPageModel());
		TestCapture.Step("three-document-tabs-open");
		var sourceDocs = dockWorkspace.Documents!.VisibleDockables!
			.OfType<ContentTabPage>().Count();
		sourceDocs.Should().Be(3, "test must open 3 tabs (MainTab + 2 extras) before saving");

		var path = Path.Combine(Path.GetTempPath(), $"ILSpy.Layout.test.{System.Guid.NewGuid():N}.json");

		try
		{
			ILSpyDockFactory.SaveLayout(path, dockWorkspace.Layout);

			// JSON shape: the saved file must not embed any ContentTabPage entries at all.
			// Stripping at the IDocumentDock.VisibleDockables level is the only way to
			// guarantee no broken-tab shells survive a restart.
			var json = File.ReadAllText(path);
			json.Should().NotContain("ContentTabPage",
				"document children must be excluded from persistence so broken tab shells "
				+ "can't come back after a restart");

			// Round-trip with a fresh factory. The loaded DocumentDock must contain exactly
			// one ContentTabPage (the fresh MainTab repopulated by the load path); none of
			// the extras may resurrect.
			var registry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Commands.ToolPaneRegistry>();
			var factory = new ILSpyDockFactory(registry);
			var loaded = factory.LoadLayout(path);
			((object?)loaded).Should().NotBeNull();

			factory.Documents.Should().NotBeNull(
				"a DocumentDock must still exist post-load so navigation has a target");
			factory.MainTab.Should().NotBeNull(
				"a fresh MainTab must be in place post-load so tree selections still decompile");
			var loadedDocs = factory.Documents!.VisibleDockables!
				.OfType<ContentTabPage>().Count();
			loadedDocs.Should().Be(1,
				"only the freshly-created MainTab must exist; persisted document children "
				+ "must not survive the round-trip");
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}

	[AvaloniaTest]
	public void LoadLayout_Returns_Null_When_The_File_Does_Not_Exist()
	{
		// The "no saved layout yet" path on first launch. DockWorkspace falls back
		// to factory.CreateLayout() when this returns null — that's the contract.
		var registry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Commands.ToolPaneRegistry>();
		var factory = new ILSpyDockFactory(registry);
		var nonExistent = Path.Combine(Path.GetTempPath(), $"ILSpy.Layout.missing.{System.Guid.NewGuid():N}.json");

		var result = factory.LoadLayout(nonExistent);

		((object?)result).Should().BeNull();
	}

	[AvaloniaTest]
	public void LoadLayout_Returns_Null_On_Malformed_Json()
	{
		// A corrupt sidecar (manually edited, version drift, mid-write crash) must
		// not block startup. DockWorkspace silently falls back to defaults.
		var registry = AppComposition.Current.GetExport<ICSharpCode.ILSpy.Commands.ToolPaneRegistry>();
		var factory = new ILSpyDockFactory(registry);
		var path = Path.Combine(Path.GetTempPath(), $"ILSpy.Layout.malformed.{System.Guid.NewGuid():N}.json");
		File.WriteAllText(path, "{ this is not valid JSON");

		try
		{
			var result = factory.LoadLayout(path);
			((object?)result).Should().BeNull("malformed JSON must surface as null, not throw");
		}
		finally
		{
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}
