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

using ILSpy.Commands;
using ILSpy.ViewModels;

namespace ILSpy.Docking
{
	public class ILSpyDockFactory : Factory
	{
		readonly IReadOnlyList<ToolPaneEntry> panes;

		public IDocumentDock? Documents { get; private set; }

		/// <summary>
		/// The single persistent Document the host puts in <see cref="Documents"/>. Its
		/// <see cref="ContentTabPage.Content"/> swaps between viewmodels (decompiler text /
		/// metadata grid) on tree-node selection — DockWorkspace owns the population.
		/// </summary>
		public ContentTabPage? MainTab { get; private set; }

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
			var dockables = panes
				.Where(p => p.Metadata.Alignment == alignment)
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
	}
}
