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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	public class ILSpyDockFactory : Factory
	{
		readonly IReadOnlyList<ToolPaneEntry> panes;

		public IDocumentDock? Documents { get; private set; }

		/// <summary>
		/// The single persistent Document the host puts in <see cref="Documents"/>. Its
		/// <see cref="ContentTabPage.Content"/> swaps between viewmodels (decompiler text /
		/// metadata grid) on tree-node selection — DockWorkspace owns the population.
		/// Identity rotates through <see cref="FreezeCurrentMainTab"/> — after a freeze, the
		/// just-frozen tab keeps its position and content while MainTab points at a fresh
		/// preview tab beside it.
		/// </summary>
		public ContentTabPage? MainTab { get; internal set; }

		/// <summary>
		/// Flips the current <see cref="MainTab"/> to frozen
		/// (<see cref="ContentTabPage.IsPreview"/> = false). Returns the frozen tab, or
		/// <see langword="null"/> when there's nothing to freeze (no current MainTab, MainTab
		/// already frozen). Does NOT spawn a new preview tab — the next selection change
		/// finds no preview to reuse and forges a fresh One lazily via
		/// <see cref="CreateThePreviewTab"/>.
		/// </summary>
		public ContentTabPage? FreezeCurrentMainTab()
		{
			if (MainTab is not { IsPreview: true } current)
				return null;
			current.IsPreview = false;
			return current;
		}

		/// <summary>
		/// Creates THE single preview <see cref="ContentTabPage"/> ("the One"), attaches it to
		/// <see cref="Documents"/> at index 0, and sets <see cref="MainTab"/> to it. Called when a
		/// tree-node selection has no preview to reuse (the previous One was frozen, closed, or
		/// none exists yet). Returns the new tab, or <see langword="null"/> if
		/// <see cref="Documents"/> is missing.
		/// </summary>
		public ContentTabPage? CreateThePreviewTab()
		{
			if (Documents is not { } docs)
				return null;
			var fresh = new ContentTabPage { Title = "(no selection)" };
			// The One always occupies index 0 (leftmost); frozen tabs live to its right. Use
			// InsertDockable (not AddDockable, which appends) so it lands at the front.
			InsertDockable(docs, fresh, 0);
			MainTab = fresh;
			return fresh;
		}

		// The One sits at documents-dock index 0. A within-strip reorder drag commits through this
		// same-dock MoveDockable (Dock's DocumentTabStripItem/ItemDragHelper does NOT consult
		// IDockable.CanDrag), so this override is the choke point: dragging the One FREEZES it (it
		// then moves like any other tab), and while it is still the preview no frozen tab may slip
		// in front of it.
		public override void MoveDockable(IDock dock, IDockable sourceDockable, IDockable targetDockable)
		{
			if (ReferenceEquals(dock, Documents))
			{
				// Dragging the One freezes it (replacing the old "refuse the move" behaviour): it
				// stops being the preview slot and lands wherever it's dropped; the next tree
				// selection forges a fresh One at index 0.
				if (ReferenceEquals(sourceDockable, MainTab) && MainTab is { IsPreview: true })
				{
					FreezeCurrentMainTab();
				}
				// While the One is still the preview, a frozen tab must not land at or before it:
				// retarget to the tab at index 1 so the source lands at position 1, not 0.
				else if (ReferenceEquals(targetDockable, MainTab) && MainTab is { IsPreview: true })
				{
					var kids = dock.VisibleDockables;
					if (kids is { Count: > 1 })
						targetDockable = kids[1];
					else
						return;
				}
			}
			base.MoveDockable(dock, sourceDockable, targetDockable);
		}

		// Cross-dock move (only reachable if the document area is ever split): dragging the One out
		// freezes it too, consistent with the in-strip case.
		public override void MoveDockable(IDock sourceDock, IDock targetDock, IDockable sourceDockable, IDockable? targetDockable)
		{
			if (ReferenceEquals(sourceDockable, MainTab) && MainTab is { IsPreview: true })
				FreezeCurrentMainTab();
			base.MoveDockable(sourceDock, targetDock, sourceDockable, targetDockable);
		}

		// Tearing the One out into its own floating window doesn't go through MoveDockable -- the
		// drag-to-float gesture funnels through CreateWindowFrom (FloatDockable -> SplitToWindow ->
		// CreateWindowFrom), with the torn-off document passed straight through before it's wrapped
		// in a fresh dock. Freeze the One here too so a float-out is just another way to keep it.
		public override IDockWindow? CreateWindowFrom(IDockable dockable)
		{
			if (ReferenceEquals(dockable, MainTab) && MainTab is { IsPreview: true })
				FreezeCurrentMainTab();
			return base.CreateWindowFrom(dockable);
		}

		// Self-healing insurance: re-assert the One at index 0 after any drag -- but ONLY while it is
		// still the preview. Once dragged (which freezes it) it's an ordinary tab and must stay where
		// the user dropped it. Idempotent; guards against a future Dock version routing a reorder
		// past MoveDockable.
		public override void OnDockableMoved(IDockable? dockable)
		{
			base.OnDockableMoved(dockable);
			if (MainTab is not { IsPreview: true } one || Documents?.VisibleDockables is not { } kids)
				return;
			int idx = kids.IndexOf(one);
			if (idx > 0)
			{
				kids.RemoveAt(idx);
				kids.Insert(0, one);
			}
		}

		public ILSpyDockFactory(ToolPaneRegistry registry)
		{
			this.panes = registry.Panes;
		}

		/// <summary>
		/// Populate the locator dictionaries with explicit id-to-factory mappings before the
		/// base init wires owners/factories down the tree, rather than relying on
		/// <see cref="DockControl.InitializeFactory"/> to populate them post-template-apply.
		/// Wiring the locators (and the HostWindowLocator) up front means the layout is fully
		/// resolvable before the chrome realises any content.
		///
		/// <para>
		/// Tool panes are <c>[Shared]</c> MEF singletons, so the lambdas in
		/// <see cref="IFactory.ContextLocator"/> / <see cref="IFactory.DockableLocator"/>
		/// can safely return the cached pane instance for both context and dockable
		/// lookup. Documents and the root dock get explicit entries for navigate-by-id
		/// (<c>root.Navigate.Execute("Documents")</c>).
		/// </para>
		/// </summary>
		public override void InitLayout(IDockable layout)
		{
			ContextLocator = new Dictionary<string, Func<object?>>();
			DockableLocator = new Dictionary<string, Func<IDockable?>>();
			HostWindowLocator = new Dictionary<string, Func<IHostWindow?>> {
				[nameof(IDockWindow)] = () => new HostWindow()
			};

			// Tool panes: the VM is its own context (no separate model class), and the
			// dockable IS the pane instance, so both locators map id → same singleton.
			foreach (var entry in panes)
			{
				var pane = entry.Pane;
				if (string.IsNullOrEmpty(pane.Id))
					continue;
				ContextLocator[pane.Id] = () => pane;
				DockableLocator[pane.Id] = () => pane;
			}

			// Documents dock + the persistent main content tab — these are the structural
			// slots ShowSelectedNode / OpenNewTab target. Carve-out tabs (per-node spawned
			// ContentTabPages) don't get id mappings because they're transient.
			if (Documents is { Id: { Length: > 0 } docsId } docs)
				DockableLocator[docsId] = () => docs;
			if (MainTab is { Id: { Length: > 0 } tabId } mainTab)
				DockableLocator[tabId] = () => mainTab;
			if (layout is IRootDock { Id: { Length: > 0 } rootId } root)
				DockableLocator[rootId] = () => root;

			base.InitLayout(layout);
		}

		/// <summary>
		/// Serializes <paramref name="layout"/> to <paramref name="path"/> using
		/// <see cref="ILSpyDockJson.Options"/>. Best-effort: any IO or serialization
		/// exception is swallowed; losing the saved layout is strictly less bad than
		/// crashing the app on shutdown.
		/// </summary>
		public static void SaveLayout(string path, IRootDock layout)
		{
			ArgumentNullException.ThrowIfNull(path);
			ArgumentNullException.ThrowIfNull(layout);
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path)!);
				using var stream = File.Create(path);
				System.Text.Json.JsonSerializer.Serialize(stream, layout, ILSpyDockJson.Options);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[ILSpyDockFactory] SaveLayout failed: {ex}");
			}
		}

		/// <summary>
		/// Tries to deserialize a previously-saved layout from <paramref name="path"/>.
		/// Returns null on any failure — missing file, malformed JSON, version drift —
		/// so the caller can fall back to <see cref="CreateLayout"/> without a user-
		/// visible error. On success, also rehydrates <see cref="Documents"/> and
		/// <see cref="MainTab"/> by walking the loaded tree — these are the same
		/// fields <see cref="CreateLayout"/> populates on fresh starts, and
		/// <see cref="DockWorkspace.ShowSelectedNode"/> silently no-ops on null
		/// <see cref="MainTab"/> (the user-visible "decompile view stays empty on
		/// second launch" bug).
		/// </summary>
		public IRootDock? LoadLayout(string path)
		{
			ArgumentNullException.ThrowIfNull(path);
			if (!File.Exists(path))
				return null;
			IRootDock? loaded;
			try
			{
				using var stream = File.OpenRead(path);
				loaded = System.Text.Json.JsonSerializer.Deserialize<IRootDock>(stream, ILSpyDockJson.Options);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[ILSpyDockFactory] LoadLayout failed: {ex}");
				return null;
			}
			if (loaded != null)
				RehydrateFromLoadedLayout(loaded);
			return loaded;
		}

		void RehydrateFromLoadedLayout(IDockable root)
		{
			// Without ReferenceHandler.Preserve (disabled because STJ's $id-not-first
			// ordering and Dock's JsonConverterList<T> per-element $id reset don't compose
			// with it), each JSON occurrence of a dockable deserialises into a fresh
			// instance — so an IDock's ActiveDockable / FocusedDockable / DefaultDockable
			// ends up pointing at a separate Activator-created object from the matching
			// VisibleDockables member. Same Id and Title, distinct CLR identity. The chrome
			// renders ActiveDockable, but ShowSelectedNode mutates the VisibleDockables
			// member, so Content never reaches the visible tab. Rebind those three slots
			// to the matching VisibleDockables member by Id so the chrome and the factory
			// share one CLR instance per logical dockable. Tool-pane singletons aren't
			// affected (their CreateObject hook funnels both encounters through the same
			// MEF [Shared] export); ContentTabPage isn't [Export]-ed, so the Activator
			// fallback produces distinct instances per encounter without this step.
			foreach (var dockable in Flatten(root))
			{
				if (dockable is IDock dock && dock.VisibleDockables is { } kids)
				{
					dock.ActiveDockable = UnifyReference(dock.ActiveDockable, kids);
					dock.FocusedDockable = UnifyReference(dock.FocusedDockable, kids);
					dock.DefaultDockable = UnifyReference(dock.DefaultDockable, kids);
				}
			}

			// Walk the loaded tree and rebind the two structural slots that CreateLayout
			// would have set. Documents = the single DocumentDock; MainTab = its first
			// ContentTabPage child (we can't look up tab.Owner because back-references
			// are stripped during serialization to break cycles). Tool-pane singletons
			// resolve through the CreateObject hook (StripCycleProneProperties) so they
			// don't need rebinding here.
			Documents = Flatten(root).OfType<IDocumentDock>().FirstOrDefault();
			MainTab = Documents?.VisibleDockables?
				.OfType<ContentTabPage>()
				.FirstOrDefault();

			// Documents aren't persisted (ILSpyDockJson strips IDocumentDock child slots),
			// so the loaded Documents.VisibleDockables comes back null/empty. Repopulate
			// with a fresh MainTab here so the user lands in the same "(no selection)"
			// state as on first launch — DockWorkspace.ShowSelectedNode then re-projects
			// the tree selection through this fresh tab as normal.
			if (Documents is { } docs && MainTab is null)
			{
				MainTab = new ContentTabPage { Title = "(no selection)" };
				docs.VisibleDockables ??= CreateList<IDockable>(MainTab);
				if (docs.VisibleDockables.Count == 0)
					docs.VisibleDockables.Add(MainTab);
				docs.ActiveDockable = MainTab;
			}
		}

		// Matches a deserialised stand-in against the matching member of its dock's
		// VisibleDockables by Id, returning the pool member when a match exists. Null /
		// empty Id is treated as "don't try to unify" — better to leave the stand-in
		// untouched than risk matching everything-with-empty-id against the first pool
		// member. Cross-dock references (candidate not in this pool) are also left as-is
		// for the same reason; ILSpy's layout doesn't use them today.
		static IDockable? UnifyReference(IDockable? candidate, IList<IDockable> pool)
		{
			if (candidate == null || string.IsNullOrEmpty(candidate.Id))
				return candidate;
			foreach (var member in pool)
			{
				if (string.Equals(member.Id, candidate.Id, StringComparison.Ordinal))
					return member;
			}
			return candidate;
		}

		static IEnumerable<IDockable> Flatten(IDockable root)
		{
			yield return root;
			if (root is IDock dock && dock.VisibleDockables is { } kids)
				foreach (var k in kids)
					foreach (var f in Flatten(k))
						yield return f;
		}

		public override IRootDock CreateLayout()
		{
			var documents = new DocumentDock {
				Id = "Documents",
				Title = "Documents",
				IsCollapsable = false,
				Proportion = 0.6,
				// Dock's theme template binds against DockCapabilityPolicy.{CanDrag, CanDrop,
				// CanPin, CanClose} on every dock. Pre-populate so the binding sees a non-null
				// source on first evaluation; otherwise every dock startup logs ~6 [Binding]
				// errors as the chrome template is realised.
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};
			Documents = documents;

			MainTab = new ContentTabPage { Title = "(no selection)" };
			documents.VisibleDockables = CreateList<IDockable>(MainTab);
			documents.ActiveDockable = MainTab;

			ToolDock? leftToolDock = BuildToolDock("LeftTools", ToolPaneAlignment.Left, 0.25);
			ToolDock? topToolDock = BuildToolDock("TopTools", ToolPaneAlignment.Top, 0.2);
			ToolDock? rightToolDock = BuildToolDock("RightTools", ToolPaneAlignment.Right, 0.25);
			ToolDock? bottomToolDock = BuildToolDock("BottomTools", ToolPaneAlignment.Bottom, 0.2);

			// Vertical column: top tool dock (if any), documents, bottom tool dock (if any),
			// with splitters between siblings.
			var verticalChildren = new List<IDockable>();
			if (topToolDock != null)
			{
				verticalChildren.Add(topToolDock);
				verticalChildren.Add(new ProportionalDockSplitter { Id = "TopSplitter" });
			}
			verticalChildren.Add(documents);
			if (bottomToolDock != null)
			{
				verticalChildren.Add(new ProportionalDockSplitter { Id = "BottomSplitter" });
				verticalChildren.Add(bottomToolDock);
			}

			var rightVertical = new ProportionalDock {
				Id = "MiddleColumn",
				Orientation = Orientation.Vertical,
				Proportion = ComputeMiddleColumnProportion(leftToolDock, rightToolDock),
				VisibleDockables = CreateList(verticalChildren.ToArray()),
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};

			// Horizontal row: left tool dock, middle column, right tool dock — splitters
			// between siblings only.
			var horizontalChildren = new List<IDockable>();
			if (leftToolDock != null)
			{
				horizontalChildren.Add(leftToolDock);
				horizontalChildren.Add(new ProportionalDockSplitter { Id = "LeftSplitter" });
			}
			horizontalChildren.Add(rightVertical);
			if (rightToolDock != null)
			{
				horizontalChildren.Add(new ProportionalDockSplitter { Id = "RightSplitter" });
				horizontalChildren.Add(rightToolDock);
			}

			var horizontal = new ProportionalDock {
				Id = "MainLayout",
				Orientation = Orientation.Horizontal,
				VisibleDockables = CreateList(horizontalChildren.ToArray()),
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};

			var root = CreateRootDock();
			root.Id = "Root";
			root.IsCollapsable = false;
			root.VisibleDockables = CreateList<IDockable>(horizontal);
			root.DefaultDockable = horizontal;
			root.ActiveDockable = horizontal;
			root.DockCapabilityPolicy = new DockCapabilityPolicy();

			return root;
		}

		ToolDock? BuildToolDock(string id, ToolPaneAlignment alignment, double proportion)
		{
			// Only panes that opt into the default layout are placed up front; the rest
			// (Search, Analyzer, Debug Steps) are materialised on demand by ShowToolPane, so the
			// default config shows just the assembly-tree pane while keeping every pane's home
			// location intact for when it is first opened.
			var dockables = panes
				.Where(p => p.Metadata.Alignment == alignment && p.Metadata.IsVisibleByDefault)
				.OrderBy(p => p.Metadata.Order)
				.Select(p => (IDockable)p.Pane)
				.ToArray();
			if (dockables.Length == 0)
				return null;
			return new ToolDock {
				Id = id,
				Proportion = proportion,
				VisibleDockables = CreateList(dockables),
				ActiveDockable = dockables[0],
				Alignment = alignment switch {
					ToolPaneAlignment.Top => Alignment.Top,
					ToolPaneAlignment.Right => Alignment.Right,
					ToolPaneAlignment.Bottom => Alignment.Bottom,
					_ => Alignment.Left,
				},
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};
		}

		static double ComputeMiddleColumnProportion(ToolDock? left, ToolDock? right)
		{
			double remaining = 1.0;
			if (left != null)
				remaining -= left.Proportion;
			if (right != null)
				remaining -= right.Proportion;
			return remaining > 0 ? remaining : 0.5;
		}

		static (string Id, Alignment DockAlignment, double Proportion) ToolDockSpec(ToolPaneAlignment alignment)
			=> alignment switch {
				ToolPaneAlignment.Left => ("LeftTools", Alignment.Left, 0.25),
				ToolPaneAlignment.Right => ("RightTools", Alignment.Right, 0.25),
				ToolPaneAlignment.Top => ("TopTools", Alignment.Top, 0.2),
				_ => ("BottomTools", Alignment.Bottom, 0.2),
			};

		/// <summary>
		/// Makes <paramref name="dockable"/> both the active and the focused dockable within
		/// <paramref name="owner"/>. The two always go together -- surfacing a dockable without
		/// focusing it (or vice versa) is a half-done activation -- so they're paired here and
		/// callers do the (varying) "add it to the dock first" step themselves.
		/// </summary>
		public void ActivateAndFocus(IDock owner, IDockable dockable)
		{
			SetActiveDockable(dockable);
			SetFocusedDockable(owner, dockable);
		}

		/// <summary>
		/// Brings a tool pane into view: activates it if it is already shown, otherwise
		/// materialises it -- (re)creating its home <see cref="ToolDock"/> at the pane's
		/// alignment and splicing that dock back into the layout if it was never built or was
		/// removed when the user closed its last pane. This is the single entry point both for
		/// "Show Search / Analyze" and for revealing a pane that is hidden by default.
		/// Returns the pane, or null when <paramref name="contentId"/> matches no registered pane.
		/// </summary>
		public ToolPaneModel? ShowToolPane(string contentId)
		{
			var entry = panes.FirstOrDefault(e => string.Equals(e.Pane.Id, contentId, StringComparison.Ordinal));
			if (entry?.Pane is not { } pane)
				return null;

			// Already live in the layout -> just activate it where it sits.
			if (pane.Owner is IDock current && current.VisibleDockables?.Contains(pane) == true)
			{
				ActivateAndFocus(current, pane);
				return pane;
			}

			var dock = FindOrCreateToolDock(entry.Metadata.Alignment);
			if (dock is null)
				return pane;
			AddDockable(dock, pane);
			ActivateAndFocus(dock, pane);
			return pane;
		}

		// Finds the live ToolDock for an alignment, or builds an empty one and splices it into the
		// layout at the same position CreateLayout would have (top/bottom inside the documents'
		// vertical column; left/right inside the outer horizontal row), separated by a splitter.
		ToolDock? FindOrCreateToolDock(ToolPaneAlignment alignment)
		{
			var (id, dockAlignment, proportion) = ToolDockSpec(alignment);

			var root = GetRootDock();
			if (root != null)
			{
				var existing = Flatten(root).OfType<ToolDock>()
					.FirstOrDefault(t => t.Alignment == dockAlignment);
				if (existing != null)
					return existing;
			}

			// vertical = the documents' column (top/bottom siblings); horizontal = the outer row.
			if (Documents?.Owner is not IDock vertical || vertical.VisibleDockables is null)
				return null;

			var dock = new ToolDock {
				Id = id,
				Proportion = proportion,
				Alignment = dockAlignment,
				VisibleDockables = CreateList(System.Array.Empty<IDockable>()),
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};

			switch (alignment)
			{
				case ToolPaneAlignment.Bottom:
					AddDockable(vertical, new ProportionalDockSplitter { Id = "BottomSplitter" });
					AddDockable(vertical, dock);
					break;
				case ToolPaneAlignment.Top:
					InsertDockable(vertical, new ProportionalDockSplitter { Id = "TopSplitter" }, 0);
					InsertDockable(vertical, dock, 0);
					break;
				case ToolPaneAlignment.Left:
				case ToolPaneAlignment.Right:
					if (vertical.Owner is not IDock horizontal)
						return null;
					if (alignment == ToolPaneAlignment.Left)
					{
						InsertDockable(horizontal, new ProportionalDockSplitter { Id = "LeftSplitter" }, 0);
						InsertDockable(horizontal, dock, 0);
					}
					else
					{
						AddDockable(horizontal, new ProportionalDockSplitter { Id = "RightSplitter" });
						AddDockable(horizontal, dock);
					}
					break;
			}
			return dock;
		}

		IDock? GetRootDock()
		{
			IDockable? d = Documents;
			while (d?.Owner is { } owner)
				d = owner;
			return d as IDock;
		}
	}
}
