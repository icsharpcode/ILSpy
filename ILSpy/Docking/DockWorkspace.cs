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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Dock.Model.Controls;

using ICSharpCode.Decompiler;
using Dock.Model.Core;
using Dock.Model.Core.Events;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	[Export]
	[Shared]
	public partial class DockWorkspace : ObservableObject
	{
		/// <summary>
		/// The active <see cref="ContentTabPage"/> in the documents dock, or null when no
		/// content tab is active (transient layout states, tool-pane focus, etc.). Updated
		/// whenever <c>factory.Documents.ActiveDockable</c> changes via the existing
		/// <see cref="OnDocumentsPropertyChanged"/> subscription. The main-toolbar pickers
		/// bind through this — see <c>ActiveContentTabPage.SupportsLanguageSwitching</c>.
		/// </summary>
		[ObservableProperty]
		private ContentTabPage? activeContentTabPage;

		readonly ILSpyDockFactory factory;
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly LanguageService languageService;
		readonly Metadata.MetadataNavigator metadataNavigator;
		readonly NavigationHistory<NavigationEntry> history = new();
		// Set true while a Back/Forward navigation is rewriting state (SelectedItem,
		// active dockable) so the change notifications don't push fresh entries onto the
		// stack and undo the navigation we just did.
		bool suppressHistoryRecording;

		public IFactory Factory => factory;

		/// <summary>The documents dock — direct access to the carve-out tabs collection.
		/// Used by commands that need to scan existing tabs for ensure-single-instance
		/// behaviour (e.g. ShowOptionsCommand).</summary>
		public IDocumentDock? Documents => factory.Documents;

		public IRelayCommand NavigateBackCommand { get; }
		public IRelayCommand NavigateForwardCommand { get; }
		public IRelayCommand<NavigationEntry> NavigateToHistoryCommand { get; }

		/// <summary>
		/// Brings the search tool pane to focus. Wired to Ctrl+Shift+F and Ctrl+E on the
		/// main window. Idempotent — re-firing on an already-active pane is a no-op.
		/// </summary>
		public IRelayCommand ShowSearchCommand { get; }

		/// <summary>Closes the active document tab. Wired to Ctrl+W on the main window. The
		/// documents dock only ever holds <see cref="ContentTabPage"/>s, so this never touches a
		/// tool pane.</summary>
		public IRelayCommand CloseActiveDocumentCommand { get; }

		// Read-only history snapshots for the Back/Forward split-button dropdowns; oldest-first.
		// The UI reverses these for newest-first display.
		public IReadOnlyList<NavigationEntry> BackHistory => history.BackEntries;
		public IReadOnlyList<NavigationEntry> ForwardHistory => history.ForwardEntries;

		/// <summary>
		/// Prunes navigation-history entries whose tree-node ancestor matches
		/// <paramref name="predicateOnNode"/>. Called from the assembly-tree model
		/// when the active assembly list emits a CollectionChanged with removals:
		/// every history entry pointing at a node that lived in one of the removed
		/// assemblies is dropped so Back/Forward can't surface a detached node.
		/// Static-page entries are left alone (they don't reference removed assemblies).
		/// </summary>
		public void PruneHistory(Predicate<SharpTreeNode> predicateOnNode)
		{
			ArgumentNullException.ThrowIfNull(predicateOnNode);
			history.RemoveAll(entry =>
				entry is TreeNodeEntry t && predicateOnNode(t.Node));
		}

		public IRootDock Layout { get; }

		/// <summary>
		/// Persists the current dock layout to the JSON sidecar next to ILSpy.xml.
		/// Called from <c>MainWindow.OnClosing</c> so the user's pane positions,
		/// splitter ratios, and pinned panels survive a restart. Best-effort: any
		/// serialization failure is logged and swallowed — losing the saved layout
		/// is strictly less bad than blocking shutdown.
		/// </summary>
		public void SaveLayout() => ILSpyDockFactory.SaveLayout(GetLayoutFilePath(), Layout);

		/// <summary>
		/// Resolves <c>ILSpy.Layout.json</c> as a sidecar in the same directory the
		/// XML <c>ILSpy.xml</c> settings file lives in — local-to-binary on portable
		/// installs, %APPDATA%/ICSharpCode/ otherwise. Keeping it next to the XML
		/// makes "delete settings to reset" still work as a single-folder action.
		/// WPF stays XML; this is Avalonia-side only.
		/// </summary>
		static string GetLayoutFilePath()
		{
			var xmlPath = ICSharpCode.ILSpyX.Settings.ILSpySettings.SettingsFilePathProvider?.Invoke();
			if (string.IsNullOrEmpty(xmlPath))
				return "ILSpy.Layout.json";
			var dir = System.IO.Path.GetDirectoryName(xmlPath);
			return string.IsNullOrEmpty(dir)
				? "ILSpy.Layout.json"
				: System.IO.Path.Combine(dir, "ILSpy.Layout.json");
		}

		public IReadOnlyList<ToolPaneMenuItem> ToolPaneMenuItems { get; }

		/// <summary>
		/// Live list of one <see cref="TabPageMenuItem"/> per open document tab. Kept in
		/// sync with <see cref="Documents"/>.<c>VisibleDockables</c> by the
		/// <see cref="OnDocumentMembershipChanged"/> handler — adds/removes mutate this in
		/// place so subscribers (the Window menu) can hook
		/// <see cref="ObservableCollection{T}.CollectionChanged"/> and patch their own items.
		/// </summary>
		public ObservableCollection<TabPageMenuItem> TabPageMenuItems { get; } = new();

		[ImportingConstructor]
		public DockWorkspace(
			AssemblyTreeModel assemblyTreeModel,
			ToolPaneRegistry toolPaneRegistry,
			LanguageService languageService,
			Metadata.MetadataNavigator metadataNavigator)
		{
			ICSharpCode.ILSpy.AppEnv.AppLog.Mark("DockWorkspace ctor entered");
			this.assemblyTreeModel = assemblyTreeModel;
			this.languageService = languageService;
			this.metadataNavigator = metadataNavigator;
			NavigateBackCommand = new RelayCommand(NavigateBack, () => history.CanNavigateBack);
			NavigateForwardCommand = new RelayCommand(NavigateForward, () => history.CanNavigateForward);
			NavigateToHistoryCommand = new RelayCommand<NavigationEntry>(NavigateToHistory,
				entry => entry != null && (history.BackEntries.Contains(entry) || history.ForwardEntries.Contains(entry)));
			ShowSearchCommand = new RelayCommand(ExecuteShowSearch);
			CloseActiveDocumentCommand = new RelayCommand(CloseActiveDocument);
			using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ILSpyDockFactory ctor + CreateLayout + InitLayout"))
			{
				using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ILSpyDockFactory ctor"))
					factory = new ILSpyDockFactory(toolPaneRegistry);
				// Prefer the user's saved layout (ILSpy.Layout.json sidecar next to
				// ILSpy.xml); fall back to the default layout if there is no saved one
				// or it failed to deserialize. The fallback path is the same shape the
				// app uses on first launch.
				IRootDock? loaded;
				using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ILSpyDockFactory.LoadLayout"))
					loaded = factory.LoadLayout(GetLayoutFilePath());
				if (loaded != null)
				{
					Layout = loaded;
					ICSharpCode.ILSpy.AppEnv.AppLog.Mark("ILSpyDockFactory: using LOADED layout");
				}
				else
				{
					using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ILSpyDockFactory.CreateLayout (default)"))
						Layout = factory.CreateLayout();
				}
				// Build the layout, then InitLayout BEFORE the chrome ever sees it, rather than
				// letting the DockControl's InitializeFactory / InitializeLayout flags run it
				// post-template-apply. Wiring owners/factories/locators up front means the
				// layout is fully resolvable before the first DeferredContentControl attaches.
				using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ILSpyDockFactory.InitLayout"))
					factory.InitLayout(Layout);
			}

			assemblyTreeModel.PropertyChanged += OnAssemblyTreePropertyChanged;
			assemblyTreeModel.SelectedItems.CollectionChanged += (_, _) => {
				if (!syncingTreeFromActiveTab)
					ShowSelectedNode();
			};
			languageService.PropertyChanged += OnLanguagePropertyChanged;

			ToolPaneMenuItems = toolPaneRegistry.Panes
				.Select(p => new ToolPaneMenuItem(p.Pane, factory))
				.ToList();

			factory.DockableAdded += OnDocumentMembershipChanged;
			factory.DockableRemoved += OnDocumentMembershipChanged;
			factory.DockableClosed += OnDocumentMembershipChanged;
			factory.DockableClosing += OnDockableClosing;
			// Seed TabPageMenuItems with whatever the loaded layout already contains
			// (typically MainTab) — InitLayout ran before the subscriptions above were
			// in place, so no DockableAdded event fired for those initial members.
			SyncTabPageMenuItems();
			// Seed ActiveContentTabPage from the freshly-initialised layout so the toolbar
			// pickers' IsEnabled bindings have the right value at first paint. The
			// subscription below will keep it in sync from here on.
			ActiveContentTabPage = factory.Documents?.ActiveDockable as ContentTabPage;
			// Dock's IFactory.ActiveDockableChanged only fires from InitActiveDockable (layout
			// structural init), not when the user clicks a different tab — that path sets
			// dock.ActiveDockable = X directly on the dock model. Subscribe to the model's own
			// PropertyChanged so tab clicks reach our sync-tree-to-tab handler.
			if (factory.Documents is INotifyPropertyChanged documentsNotify)
				documentsNotify.PropertyChanged += OnDocumentsPropertyChanged;
			// Close orphaned carve-out tabs when their assembly is removed. The persistent
			// MainTab slot is left alone — its content will swap to whatever the user selects
			// next via the assembly tree. Mirrors WPF's DockWorkspace.CurrentAssemblyList_Changed.
			ICSharpCode.ILSpy.Util.MessageBus<ICSharpCode.ILSpy.Util.CurrentAssemblyListChangedEventArgs>.Subscribers
				+= OnAssemblyListChanged;
			ICSharpCode.ILSpy.AppEnv.AppLog.Mark("DockWorkspace ctor exited");
		}

		void OnAssemblyListChanged(object? sender, ICSharpCode.ILSpy.Util.CurrentAssemblyListChangedEventArgs e)
		{
			var inner = e.Inner;

			// On Reset (assembly list wholesale-cleared), drop ALL history — every entry is
			// stale by definition. Mirrors WPF's assemblyList_CollectionChanged.
			if (inner.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
			{
				PruneHistoryAfterAssemblyListChange(removed: null);
				return;
			}

			if (inner.OldItems is not { Count: > 0 } oldItems)
				return;
			var removed = new HashSet<ICSharpCode.ILSpyX.LoadedAssembly>(
				oldItems.OfType<ICSharpCode.ILSpyX.LoadedAssembly>());
			if (removed.Count == 0)
				return;

			PruneHistoryAfterAssemblyListChange(removed);

			if (factory.Documents?.VisibleDockables is not { } visible)
				return;

			foreach (var dockable in visible.OfType<ViewModels.ContentTabPage>().ToArray())
			{
				if (dockable.Content is not TextView.DecompilerTabPageModel tab || tab.IsStaticContent)
					continue;
				var nodes = tab.CurrentNodes;
				if (nodes.Count == 0)
					continue;
				bool anyTouchesRemoved = false;
				bool anyAlive = false;
				foreach (var n in nodes)
				{
					var owner = n.AncestorsAndSelf().OfType<TreeNodes.AssemblyTreeNode>().LastOrDefault();
					if (owner is null)
						continue;
					if (removed.Contains(owner.LoadedAssembly))
						anyTouchesRemoved = true;
					else
						anyAlive = true;
				}
				if (!anyTouchesRemoved)
					continue;
				// Cancel the in-flight decompile of an assembly the user just removed -- no point
				// finishing work on something no longer in the list. The decompile worker checks
				// its CancellationToken between transforms; the spinner exits when its Task.Delay
				// sees the cancellation. Note AssemblyList.Unload no longer disposes the
				// LoadedAssembly / MetadataFile (its lifetime can't be known safely), so this is a
				// courtesy cancel, not a guard against unmapping a file out from under a reader.
				tab.CancelDecompilationCommand.Execute(null);
				if (anyAlive)
					continue;
				if (ReferenceEquals(dockable, factory.MainTab))
				{
					// Persistent slot: empty it. The CurrentNodes setter unsubscribes from each
					// node's PropertyChanged, so a delayed AssemblyTreeNode.RaisePropertyChanged
					// post-Dispose has no listener to drag into a metadata read.
					tab.CurrentNodes = System.Array.Empty<ILSpyTreeNode>();
					dockable.SourceNode = null;
				}
				else
				{
					factory.CloseDockable(dockable);
				}
			}
		}

		// Drops history entries whose tree node lives inside a now-removed assembly. With
		// <paramref name="removed"/> == null, drops everything (used for the Reset action).
		// Without this, Back/Forward would surface entries whose Node reference has been
		// detached from the live tree, and formatting their DisplayText routes through
		// language-specific formatters that NRE on a stale IEntity.ParentModule.
		void PruneHistoryAfterAssemblyListChange(HashSet<ICSharpCode.ILSpyX.LoadedAssembly>? removed)
		{
			if (removed == null)
			{
				history.RemoveAll(_ => true);
			}
			else
			{
				history.RemoveAll(entry =>
					entry is Navigation.TreeNodeEntry t
					&& t.Node.AncestorsAndSelf()
						.OfType<TreeNodes.AssemblyTreeNode>()
						.Any(a => removed.Contains(a.LoadedAssembly)));
			}
			NavigateBackCommand.NotifyCanExecuteChanged();
			NavigateForwardCommand.NotifyCanExecuteChanged();
		}

		void OnDocumentMembershipChanged(object? sender, EventArgs e)
		{
			UpdateLastDocumentCanClose();
			SyncTabPageMenuItems();
		}

		// Mirror factory.Documents.VisibleDockables into TabPageMenuItems. Mutates in place so
		// existing items keep their PropertyChanged subscriptions and the bound Window-menu
		// entries don't re-bind for tabs that didn't move.
		void SyncTabPageMenuItems()
		{
			if (factory.Documents is not { } docDock)
				return;
			var present = docDock.VisibleDockables?.OfType<ContentTabPage>().ToList()
				?? new List<ContentTabPage>();

			// Drop items for tabs that have left the dock (close / re-parent).
			for (int i = TabPageMenuItems.Count - 1; i >= 0; i--)
			{
				if (!present.Contains(TabPageMenuItems[i].Tab))
				{
					TabPageMenuItems[i].Detach();
					TabPageMenuItems.RemoveAt(i);
				}
			}

			// Append items for newly arrived tabs, preserving the dock's order so the menu
			// reads left-to-right the same as the tab strip.
			for (int i = 0; i < present.Count; i++)
			{
				var tab = present[i];
				int existing = -1;
				for (int j = 0; j < TabPageMenuItems.Count; j++)
				{
					if (ReferenceEquals(TabPageMenuItems[j].Tab, tab))
					{
						existing = j;
						break;
					}
				}
				if (existing == -1)
				{
					TabPageMenuItems.Insert(i, new TabPageMenuItem(tab, factory, docDock));
				}
				else if (existing != i)
				{
					var item = TabPageMenuItems[existing];
					TabPageMenuItems.RemoveAt(existing);
					TabPageMenuItems.Insert(i, item);
				}
			}
		}

		// Convert a tool pane's "close" (X button) into "hide" so the Window menu can restore it.
		// Documents are still closed for real — they get garbage-collected.
		void OnDockableClosing(object? sender, DockableClosingEventArgs e)
		{
			if (e.Dockable is ToolPaneModel toolPane)
			{
				e.Cancel = true;
				factory.HideDockable(toolPane);
			}
		}

		// When only one document is open across the entire workspace, make it un-closeable so
		// the user can't end up with an empty document area. Counts across ALL IDocumentDocks
		// in the layout tree — not just factory.Documents — so a split into side-by-side
		// document docks (each with one tab) still lets the user close either one.
		void UpdateLastDocumentCanClose()
		{
			if (Layout is not IDockable root)
				return;
			var allDocs = FlattenDocumentDocks(root).ToList();
			int totalTabs = allDocs.Sum(d => d.VisibleDockables?.Count ?? 0);
			bool canClose = totalTabs > 1;
			foreach (var dock in allDocs)
			{
				if (dock.VisibleDockables is { } kids)
				{
					foreach (var d in kids)
						d.CanClose = canClose;
				}
			}
		}

		static IEnumerable<IDocumentDock> FlattenDocumentDocks(IDockable root)
		{
			if (root is IDocumentDock doc)
				yield return doc;
			if (root is IDock dock && dock.VisibleDockables is { } kids)
			{
				foreach (var k in kids)
					foreach (var d in FlattenDocumentDocks(k))
						yield return d;
			}
		}

		/// <summary>
		/// Cancels every in-flight decompile across all document tabs and returns a task that
		/// completes once they have unwound. Drives the workspace to quiescence so a background
		/// decompile can't keep running and post a continuation later -- e.g. on app shutdown, or
		/// between headless tests where it would otherwise land in the next test's rebuilt
		/// composition and read the swapped-in singletons.
		/// </summary>
		internal Task CancelPendingOperationsAsync()
		{
			var pending = AllDecompilerTabs()
				.Select(tab => tab.CancelPendingAsync())
				.ToArray();
			return pending.Length == 0 ? Task.CompletedTask : Task.WhenAll(pending);
		}

		/// <summary>
		/// Every decompiler tab model across all document docks, including docks in floating
		/// windows: the preview tab, frozen tabs, and static-content pages alike. Callers
		/// that must skip static content (e.g. output refreshes) filter on
		/// <see cref="TextView.DecompilerTabPageModel.IsStaticContent"/> themselves.
		/// </summary>
		IEnumerable<TextView.DecompilerTabPageModel> AllDecompilerTabs()
		{
			if (Layout is not IDockable root)
				return [];
			return FlattenDocumentDocks(root)
				.SelectMany(dock => dock.VisibleDockables?.OfType<ContentTabPage>() ?? Enumerable.Empty<ContentTabPage>())
				.Select(tab => tab.Content)
				.OfType<TextView.DecompilerTabPageModel>();
		}

		// Set true while syncing the tree's selection FROM the active tab so the
		// SelectionChanged handler doesn't bounce back into ShowSelectedNode and overwrite
		// MainTab.Content with the carved-out tab's node.
		bool syncingTreeFromActiveTab;

		void OnAssemblyTreePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AssemblyTreeModel.SelectedItem))
			{
				if (!syncingTreeFromActiveTab)
					ShowSelectedNode();
				RecordTreeNodeSelection(assemblyTreeModel.SelectedItem);
			}
		}

		void OnDocumentsPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(IDocumentDock.ActiveDockable))
				return;
			// Mirror the dock's active document into ActiveContentTabPage. Tool-pane
			// dockables and non-content dockables fall through to null, which the toolbar
			// reads as "no language-aware tab is active" — pickers stay enabled by default.
			ActiveContentTabPage = factory.Documents?.ActiveDockable as ContentTabPage;
			// Tell each TabPageMenuItem to re-raise IsActive so the Window menu's checkmark
			// follows the dock's selection. Items resolve "am I active?" against the dock's
			// current ActiveDockable on read; this notify just kicks the binding.
			foreach (var item in TabPageMenuItems)
				item.NotifyActiveChanged();
			// Carve-out tab → active: pull the tree's selection over to whatever entity the
			// new active tab is showing. Fires on user-driven tab clicks (via the dock model's
			// ActiveDockable setter) and on programmatic SetActiveDockable calls.
			if (factory.Documents?.ActiveDockable is not ContentTabPage tab)
				return;
			// Restore the tab's FULL selection. A decompiler tab carries its node(s) in
			// CurrentNodes (one or many) — a multi-node tab has no single SourceNode, so reading
			// CurrentNodes is what lets the tree restore the whole multi-selection. Other content
			// (metadata, compare) carries a single SourceNode.
			System.Collections.Generic.IReadOnlyList<SharpTreeNode> nodes;
			if (UnwrapDecompilerTab(tab) is { CurrentNodes.Count: > 0 } dec)
				nodes = dec.CurrentNodes;
			else if (tab.SourceNode is { } single)
				nodes = new[] { single };
			else
				return;
			syncingTreeFromActiveTab = true;
			try
			{
				assemblyTreeModel.SelectNodes(nodes);
			}
			finally
			{
				syncingTreeFromActiveTab = false;
			}
		}

		void RecordTreeNodeSelection(SharpTreeNode? node)
		{
			if (suppressHistoryRecording || node == null)
				return;
			// ShowSelectedNode has already settled the active dockable for this selection —
			// record whichever tab type ended up there (decompiler or metadata grid).
			if (factory.Documents?.ActiveDockable is not TabPageModel tab)
				return;
			RecordHistoryEntry(new TreeNodeEntry(tab, node));
		}

		/// <summary>
		/// Records a static-page entry (e.g. About) into the navigation history. The caller
		/// has already opened <paramref name="tab"/> via <see cref="OpenNewTab"/>. The tab
		/// should have <see cref="DecompilerTabPageModel.IsStaticContent"/> set so that
		/// subsequent tree-node selections route to a different tab and leave it intact.
		/// </summary>
		public void RecordStaticPage(TabPageModel tab, Uri uri)
		{
			ArgumentNullException.ThrowIfNull(tab);
			ArgumentNullException.ThrowIfNull(uri);
			if (suppressHistoryRecording)
				return;
			RecordHistoryEntry(new StaticPageEntry(tab, uri));
		}

		void RecordHistoryEntry(NavigationEntry entry)
		{
			// Snapshot the editor's caret + scroll state into the OUTGOING entry — at this
			// point the editor still shows the previous selection's content, so the values
			// describe where the user was looking before this new entry takes over. The
			// captured state is then restored if/when the user navigates Back/Forward to it.
			CaptureCurrentViewState();
			history.Record(entry);
			NavigateBackCommand.NotifyCanExecuteChanged();
			NavigateForwardCommand.NotifyCanExecuteChanged();
		}

		void CaptureCurrentViewState()
		{
			if (history.Current is not TreeNodeEntry current)
				return;
			// Pull the editor's live view state (caret + scroll + foldings) on demand. Reading it
			// only at this point -- when we record the navigation away -- avoids the poisoned
			// captures a per-event push suffered (AvaloniaEdit jumps the caret to the end on a
			// text replace). Null when no view is attached / nothing to capture.
			if (UnwrapDecompilerTab(current.Tab)?.CaptureViewState?.Invoke() is not { } state)
				return;
			current.CaretOffset = state.CaretOffset;
			current.VerticalOffset = state.VerticalOffset;
			current.HorizontalOffset = state.HorizontalOffset;
			current.Foldings = state.Foldings;
		}

		// The recorded TabPageModel in TreeNodeEntry can be either a DecompilerTabPageModel
		// directly OR a ContentTabPage whose Content is one — `factory.Documents.ActiveDockable`
		// returns the latter in the normal layout. Try both shapes.
		static TextView.DecompilerTabPageModel? UnwrapDecompilerTab(ViewModels.TabPageModel? tab) => tab switch {
			TextView.DecompilerTabPageModel d => d,
			ViewModels.ContentTabPage c => c.Content as TextView.DecompilerTabPageModel,
			_ => null,
		};

		void NavigateBack() => NavigateHistory(forward: false);
		void NavigateForward() => NavigateHistory(forward: true);

		void NavigateHistory(bool forward)
		{
			if (forward ? !history.CanNavigateForward : !history.CanNavigateBack)
				return;
			var target = forward ? history.GoForward() : history.GoBack();
			ApplyNavigationTarget(target);
		}

		void NavigateToHistory(NavigationEntry? entry)
		{
			if (entry == null)
				return;
			bool forward = history.ForwardEntries.Contains(entry);
			if (!forward && !history.BackEntries.Contains(entry))
				return;
			var target = history.GoTo(entry, forward);
			if (target != null)
				ApplyNavigationTarget(target);
		}

		void ApplyNavigationTarget(NavigationEntry target)
		{
			suppressHistoryRecording = true;
			try
			{
				if (factory.Documents?.VisibleDockables is { } docs && docs.Contains(target.Tab))
					factory.SetActiveDockable(target.Tab);
				if (target is TreeNodeEntry treeNode)
				{
					// Stash the captured view state on the tab BEFORE setting SelectedItem.
					// SelectedItem triggers the decompile; the editor's ApplyDocument reads
					// the Pending* fields right after Text lands and restores caret + scroll.
					if (UnwrapDecompilerTab(treeNode.Tab) is { } decompTab)
					{
						decompTab.PendingViewState = new TextView.DecompilerTextViewState(
							treeNode.CaretOffset ?? 0,
							treeNode.VerticalOffset ?? 0,
							treeNode.HorizontalOffset ?? 0,
							treeNode.Foldings);
					}
					assemblyTreeModel.SelectedItem = treeNode.Node;
					// Restore the metadata-table row for entries created by "Go to token".
					if (treeNode.MetadataRow is { } metadataRow && factory.MainTab?.Content is MetadataTablePageModel pm)
						pm.ScrollToRow = metadataRow;
				}
				// StaticPageEntry: just reactivating the tab is enough — its content was
				// preserved because IsStaticContent kept tree-node selections from targeting it.
			}
			finally
			{
				suppressHistoryRecording = false;
			}
			NavigateBackCommand.NotifyCanExecuteChanged();
			NavigateForwardCommand.NotifyCanExecuteChanged();
		}

		void OnLanguagePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(LanguageService.CurrentLanguage) or nameof(LanguageService.CurrentVersion))
			{
				// Every decompiler tab caches its own output, so every one re-decompiles —
				// frozen tabs included, not just the active preview tab. The language
				// version is read per-run inside TryGetLiveDecompilerSettings, so assigning
				// the language and re-running covers both combo boxes.
				foreach (var tab in AllDecompilerTabs())
				{
					if (tab.IsStaticContent)
						continue;
					tab.Language = languageService.CurrentLanguage;
					tab.Redecompile();
				}
			}
		}

		void OnNavigateRequested(ReferenceSegment segment)
		{
			// Hyperlink click in the decompiler view: resolve the segment's reference to a tree
			// node and select it. Falls through silently when we don't know how to model the
			// reference (only types/members/EntityReferences are supported today).
			if (segment.Reference == null)
				return;
			// EntityReferences with a non-"decompile" protocol (e.g. metadata://) get a first
			// pass through registered IProtocolHandler exports. The first handler returning a
			// non-null node wins; if none match we fall through to the default resolver.
			if (segment.Reference is ICSharpCode.ILSpy.EntityReference entity
				&& entity.Protocol != "decompile")
			{
				var module = entity.ResolveAssembly(assemblyTreeModel.AssemblyList!);
				if (module != null)
				{
					foreach (var handler in TryGetProtocolHandlers())
					{
						var resolved = handler.Resolve(entity.Protocol, module, entity.Handle, out _);
						if (resolved != null)
						{
							assemblyTreeModel.SelectedItem = resolved;
							return;
						}
					}
				}
			}
			var node = assemblyTreeModel.FindTreeNode(segment.Reference);
			if (node != null)
				assemblyTreeModel.SelectedItem = node;
		}

		static IEnumerable<Commands.IProtocolHandler> TryGetProtocolHandlers()
			=> AppEnv.AppComposition.TryGetExports<Commands.IProtocolHandler>();

		// Long-lived decompiler viewmodel — kept alive across metadata interludes so going
		// back to text doesn't lose the previous decompile until a fresh one supersedes it.
		// Created lazily on first need.
		DecompilerTabPageModel? decompilerContent;

		// The startup welcome page (About content in the reusable MainTab, non-static). Tracked so
		// Help > About can activate it instead of spawning a duplicate static About tab while it is
		// still on screen. Self-correcting: once a tree-node selection swaps MainTab.Content to the
		// decompiler content, this reference no longer equals MainTab.Content (see IsWelcomePageVisible).
		DecompilerTabPageModel? welcomeContent;

		// Retained static-content singleton tabs (Options, About, embedded resource pages).
		// Keeping the ContentTabPage instance across close/reopen preserves its owned view and
		// content state (e.g. the selected options page) instead of rebuilding the tab each time.
		readonly Dictionary<string, ContentTabPage> singletonTabs = new();

		ILSpyTreeNode[]? lastShownNodes;

		/// <summary>
		/// Force a re-decompile of the active tree selection even if it equals the last
		/// rendered selection. Called by <see cref="AssemblyTree.AssemblyTreeModel"/> at
		/// the end of <c>RefreshInternalAsync</c>: when F5 reloads the same assembly list,
		/// the tree isn't rebuilt and the SelectedItem reference is preserved, so the
		/// normal selection-change cascade would no-op and the editor would keep stale
		/// decompiled text. Resetting <c>lastShownNodes</c> defeats the
		/// dedup short-circuit inside <see cref="ShowSelectedNode"/>. Mirrors WPF's
		/// <c>RefreshDecompiledView()</c> call.
		/// </summary>
		public void ForceRefreshActiveTab()
		{
			lastShownNodes = null;
			ShowSelectedNode();
			// ShowSelectedNode re-projects the selection, but DecompilerTabPageModel.CurrentNodes
			// dedups an unchanged node -- so a same-node refresh (a decompiler-output display setting,
			// or freshly resolved dependencies) wouldn't actually re-run. Force the active decompiler
			// tab to re-decompile so its output reflects the new state.
			ActiveDecompilerTab?.Redecompile();
		}

		/// <summary>
		/// Re-decompiles every decompiler tab's content in place so an output-affecting display
		/// setting takes effect, WITHOUT activating or navigating to any of them. Unlike
		/// <see cref="ForceRefreshActiveTab"/> (which re-projects the tree selection via
		/// <c>ShowSelectedNode</c> and so activates the preview tab), changing an option must not
		/// steal the user's current tab. Covers frozen tabs and floated tabs, whose models each
		/// cache their own output.
		/// </summary>
		public void RefreshDecompilerOutputInPlace()
		{
			foreach (var tab in AllDecompilerTabs())
			{
				if (!tab.IsStaticContent)
					tab.Redecompile();
			}
		}

		void ShowSelectedNode()
		{
			using var _ = ICSharpCode.ILSpy.AppEnv.AppLog.Phase("DockWorkspace.ShowSelectedNode");
			var nodes = assemblyTreeModel.SelectedItems.OfType<ILSpyTreeNode>().ToArray();
			if (nodes.Length == 0)
			{
				lastShownNodes = null;
				return;
			}
			// SelectedItems.CollectionChanged and SelectedItem PropertyChanged both fan into
			// here on a single click, so dedupe to avoid creating two TabPageModels for the
			// same selection — the second one's columns would replace the first's, but the
			// first's filter wiring would be left dangling on stale ColumnFilter instances.
			// Dedupe is also what makes "re-clicking the same node after freeze" a no-op: the
			// frozen tab stays active, no new preview tab spawns.
			if (lastShownNodes is { } prev && prev.SequenceEqual(nodes))
			{
				if (factory.MainTab is { } existingMain)
					ActivateMainTabIfNeeded(existingMain);
				return;
			}
			lastShownNodes = nodes;

			// Route to THE single preview tab ("the One") and replace its content in place.
			var main = GetOrCreateThePreview();
			if (main == null)
				return;

			// Tree-node selections always reuse the single document slot. The Document
			// instance never changes; its inner Content swaps between viewmodels. The
			// wrapper view (ContentTabPageView) holds pre-realised inner views and toggles
			// which is visible based on Content's runtime type — keeps the visual swap
			// deterministic without going through Dock's add+close lifecycle.
			// Carve-outs ("Open in new tab", "Freeze tab") aren't implemented yet and would
			// branch off this path before the Content swap.
			ContentPageModel? customContent;
			using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ShowSelectedNode: node.CreateTab"))
				customContent = nodes.Length == 1 ? nodes[0].CreateTab() : null;

			if (customContent != null)
			{
				// Copy state into the existing inner viewmodel when types match — keeps
				// scroll position / sort order across the navigation. Only swap to a new
				// instance when the content type changes.
				if (main.Content?.GetType() == customContent.GetType())
					CopyContentState(customContent, main.Content);
				else
					AttachCustomContent(main, customContent);
				main.SourceNode = nodes[0];
				ActivateMainTabIfNeeded(main);
				return;
			}

			DecompilerTabPageModel decTab;
			using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ShowSelectedNode: get/create decompilerContent"))
				decTab = decompilerContent ??= CreateDecompilerContent();
			using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ShowSelectedNode: main.Content = decTab (Dock view-recycling)"))
				main.Content = decTab;
			decTab.Language = languageService.CurrentLanguage;
			using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ShowSelectedNode: decTab.CurrentNodes = nodes (kicks off DecompileAsync)"))
				decTab.CurrentNodes = nodes;
			main.SourceNode = nodes.Length == 1 ? nodes[0] : null;
			using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("ShowSelectedNode: ActivateMainTabIfNeeded"))
				ActivateMainTabIfNeeded(main);
		}

		// The "exactly one preview tab" rule: tree selections always route to the single preview
		// ("the One"), tracked by factory.MainTab. Reuse it wherever it sits -- even if a frozen
		// tab is currently active, ActivateMainTabIfNeeded brings it forward -- as long as it is
		// still a writable preview AND still lives in the dock. Otherwise the One was frozen-away
		// or closed, so forge a fresh one at index 0. This is what stops a selection-while-frozen
		// from spawning a SECOND preview.
		ContentTabPage? GetOrCreateThePreview()
		{
			if (factory.Documents is not { } docs)
				return null;
			if (factory.MainTab is { } current
				&& IsWritablePreview(current) is not null
				&& docs.VisibleDockables?.Contains(current) == true)
			{
				return current;
			}
			// A fresh One needs a fresh DecompilerTabPageModel -- the cached one (if any) belongs
			// to a previous tab and must not be re-assigned across tabs.
			decompilerContent = null;
			return factory.CreateThePreviewTab();
		}

		// Pure predicate: returns the tab if it is a writable preview ContentTabPage -- IsPreview
		// true AND not hosting static content (Options, About). Otherwise null. Static pages are
		// born frozen (IsPreview=false) AND carry IsStaticContent=true; either flag alone would
		// suffice, both guard against drift.
		static ContentTabPage? IsWritablePreview(IDockable? dockable)
		{
			if (dockable is not ContentTabPage tab)
				return null;
			if (!tab.IsPreview)
				return null;
			if (tab.Content is { IsStaticContent: true })
				return null;
			return tab;
		}

		// Brings MainTab to the front of the documents dock so the just-updated content is
		// what the user sees. No-op when MainTab is already active, when there's no
		// documents dock, or when a Back/Forward navigation is in flight (ApplyNavigationTarget
		// has already chosen which tab to activate, including possibly a sibling tab — our
		// activation would override that intent).
		void ActivateMainTabIfNeeded(ContentTabPage main)
		{
			if (suppressHistoryRecording)
				return;
			if (factory.Documents is not { } docs)
				return;
			if (ReferenceEquals(docs.ActiveDockable, main))
				return;
			factory.SetActiveDockable(main);
		}

		void AttachCustomContent(ContentTabPage main, ContentPageModel newContent)
		{
			// Detach navigation handlers from the outgoing content; subscribe on the
			// incoming one so token clicks route through OnMetadataCellClicked and
			// row activation routes through OnMetadataRowActivated.
			if (main.Content is MetadataTablePageModel oldMeta)
			{
				oldMeta.NavigateToCellRequested -= OnMetadataCellClicked;
				oldMeta.RowActivated -= OnMetadataRowActivated;
			}
			if (newContent is MetadataTablePageModel newMeta)
			{
				newMeta.NavigateToCellRequested += OnMetadataCellClicked;
				newMeta.RowActivated += OnMetadataRowActivated;
			}
			main.Content = newContent;
		}

		void OnMetadataCellClicked(MetadataCellNavigationEventArgs e)
		{
			// A clicked token cell -> the metadata table + row the token points at.
			if (metadataNavigator.ReadCellToken(e.Row, e.ColumnName) is { } reference)
				NavigateToToken(reference);
		}

		internal void OnMetadataRowActivated(MetadataRowActivationEventArgs e)
		{
			// Row double-click: resolve the row to its tree node, then either select it (single-tab
			// reuse) or open it in a fresh tab (MMB / context-menu carve-out).
			var node = metadataNavigator.ResolveRowToTreeNode(e.Row);
			if (node is null)
				return;
			if (e.OpenInNewTab)
				OpenNodeInNewTab(node);
			else
				assemblyTreeModel.SelectedItem = node;
		}

		/// <summary>
		/// Opens <paramref name="node"/> in a fresh document tab. When the node carries
		/// custom content (metadata tables, resource viewers, …) the new tab hosts that
		/// custom page-type with the same nav / row-activation wiring the single-tab
		/// reuse path applies via <see cref="AttachCustomContent"/>; otherwise a fresh
		/// <see cref="DecompilerTabPageModel"/> is spawned and asked to decompile the
		/// node. Shared between the assembly-tree MMB handler, the metadata-grid MMB
		/// handler, and the "Decompile to new tab" context-menu entry. The created tab
		/// is frozen (born with <see cref="ContentTabPage.IsPreview"/> false) — see
		/// <see cref="OpenNewTab"/>.
		/// </summary>
		public void OpenNodeInNewTab(ILSpyTreeNode node)
		{
			ArgumentNullException.ThrowIfNull(node);
			var customContent = node.CreateTab();
			if (customContent != null)
			{
				if (customContent is MetadataTablePageModel newMeta)
				{
					newMeta.NavigateToCellRequested += OnMetadataCellClicked;
					newMeta.RowActivated += OnMetadataRowActivated;
				}
				OpenNewTab(customContent, sourceNode: node);
			}
			else
			{
				var content = new DecompilerTabPageModel { Language = languageService.CurrentLanguage };
				OpenNewTab(content, sourceNode: node);
				content.CurrentNodes = new[] { node };
			}
		}

		/// <summary>
		/// Resolves a metadata-grid row to the matching assembly-tree node, shared between row
		/// double-click and the "Decompile to new tab" context-menu entry. Returns
		/// <see langword="null"/> when the row's token doesn't resolve to an entity the tree models.
		/// </summary>
		internal ILSpyTreeNode? TryResolveRowToTreeNode(object row)
			=> metadataNavigator.ResolveRowToTreeNode(row);

		public void NavigateToToken(MetadataTokenReference reference)
		{
			if (reference.Handle.IsNil)
				return;
			var tableNode = metadataNavigator.FindTableNode(reference);
			if (tableNode == null)
				return;

			// Let the selection record a history entry so Back returns the user here from wherever
			// the token jump originated (a code view or another metadata table).
			assemblyTreeModel.SelectedItem = tableNode;

			// ShowSelectedNode just settled MainTab.Content to the table's MetadataTablePageModel.
			// Set ScrollToRow to the handle's row number (1-based → 0-based), and stamp that row onto
			// the just-recorded history entry so Back/Forward restores the exact token, not the table top.
			if (factory.MainTab?.Content is MetadataTablePageModel pm)
			{
				int row = MetadataTokens.GetRowNumber((EntityHandle)reference.Handle) - 1;
				pm.ScrollToRow = row;
				if (history.Current is TreeNodeEntry current && ReferenceEquals(current.Node, tableNode))
					current.MetadataRow = row;
			}
		}

		DecompilerTabPageModel CreateDecompilerContent()
		{
			var tab = new DecompilerTabPageModel { Title = "(no selection)" };
			tab.Language = languageService.CurrentLanguage;
			tab.NavigateRequested += OnNavigateRequested;
			return tab;
		}

		static void CopyContentState(ContentPageModel source, ContentPageModel target)
		{
			if (source is MetadataTablePageModel newMeta && target is MetadataTablePageModel oldMeta)
			{
				oldMeta.Title = newMeta.Title;
				oldMeta.Columns = newMeta.Columns;
				oldMeta.Items = newMeta.Items;
				oldMeta.ScrollToRow = newMeta.ScrollToRow;
			}
		}

		public DecompilerTabPageModel? ActiveDecompilerTab
			=> factory.MainTab?.Content as DecompilerTabPageModel is { IsStaticContent: false } d ? d : null;

		/// <summary>
		/// Forwards to <see cref="DecompilerTabPageModel.RunWithCancellation"/> on the active
		/// decompiler tab. Convenience wrapper used by long-running commands (Create Diagram,
		/// CFG, etc.) that need a wait UI with a custom title. Returns a faulted task if no
		/// active decompiler tab is available.
		/// </summary>
		public Task<T> RunWithCancellation<T>(
			Func<CancellationToken, Task<T>> taskCreation,
			string? progressTitle = null)
		{
			var tab = ActiveDecompilerTab;
			if (tab == null)
				return Task.FromException<T>(new InvalidOperationException("No active decompiler tab"));
			return tab.RunWithCancellation(taskCreation, progressTitle);
		}

		/// <summary>
		/// Forwards to <see cref="DecompilerTabPageModel.ShowText"/> on the active decompiler
		/// tab. Convenience wrapper used by command-driven reports (Create Diagram, Generate
		/// PDB, etc.) to surface their output where the user is looking.
		/// </summary>
		public void ShowText(TextView.AvaloniaEditTextOutput output)
		{
			ActiveDecompilerTab?.ShowText(output);
		}

		/// <summary>
		/// Runs a long-running operation in a NEW, frozen document tab with its own
		/// decompile/cancellation scope, then shows the operation's report in that tab. Unlike
		/// <see cref="RunWithCancellation"/> (which runs on the active preview tab, whose
		/// cancellation token tree-node navigation cancels), a frozen tab is never the navigation
		/// target — so selecting another node while the operation runs cannot cancel it. Use this
		/// for project/solution export, PDB generation, and similar work the user may want to keep
		/// running while they browse. Cancellation (the tab's Cancel overlay) is honoured.
		/// </summary>
		public async Task RunInNewTabAsync(string title,
			Func<CancellationToken, Task<TextView.AvaloniaEditTextOutput>> work)
		{
			ArgumentNullException.ThrowIfNull(work);
			var content = new TextView.DecompilerTabPageModel {
				Language = languageService.CurrentLanguage,
				Title = title,
			};
			OpenNewTab(content);
			try
			{
				var output = await content.RunWithCancellation(work, title).ConfigureAwait(true);
				content.ShowText(output);
			}
			catch (OperationCanceledException)
			{
				content.ShowText(CreateCancelledOutput(title));
			}
		}

		/// <summary>
		/// Like <see cref="RunInNewTabAsync(string, Func{CancellationToken, Task{TextView.AvaloniaEditTextOutput}})"/>,
		/// but also hands the work an <see cref="IProgress{T}"/> it can report
		/// <see cref="DecompilationProgress"/> through. The tab turns those reports into a determinate
		/// progress bar and the name of the file currently being written. Used by project/solution export.
		/// </summary>
		public async Task RunInNewTabAsync(string title,
			Func<CancellationToken, IProgress<DecompilationProgress>, Task<TextView.AvaloniaEditTextOutput>> work)
		{
			ArgumentNullException.ThrowIfNull(work);
			var content = new TextView.DecompilerTabPageModel {
				Language = languageService.CurrentLanguage,
				Title = title,
			};
			OpenNewTab(content);
			// Progress<T> captures this (UI) thread's SynchronizationContext, so reports raised from the
			// background export marshal back here before touching the observable progress properties.
			var progress = new Progress<DecompilationProgress>(content.ReportProgress);
			try
			{
				var output = await content.RunWithCancellation(token => work(token, progress), title).ConfigureAwait(true);
				content.ShowText(output);
			}
			catch (OperationCanceledException)
			{
				content.ShowText(CreateCancelledOutput(title));
			}
		}

		/// <summary>
		/// Builds the report shown in a frozen operation tab when the user cancels it: the tab keeps
		/// its title but its body is replaced with a short "Operation was cancelled." note so the tab
		/// is not left blank.
		/// </summary>
		static TextView.AvaloniaEditTextOutput CreateCancelledOutput(string title)
		{
			var output = new TextView.AvaloniaEditTextOutput { Title = title };
			output.Write(ICSharpCode.ILSpy.Properties.Resources.OperationWasCancelled);
			output.WriteLine();
			return output;
		}

		/// <summary>
		/// Brings the tool pane with the given <paramref name="contentId"/> into view and
		/// activates it. Delegates to <see cref="ILSpyDockFactory.ShowToolPane"/>, which
		/// re-materialises the pane (and recreates its home dock) when it was closed or is hidden
		/// by default -- so "Show Search" / "Analyze" reliably reveal the pane even after the user
		/// closed it.
		/// </summary>
		public void ShowToolPane(string contentId) => factory.ShowToolPane(contentId);

		/// <summary>
		/// Opens a fresh frozen tab showing the supplied pre-rendered text output. Used to surface
		/// reports that aren't tied to a tree node (e.g. composition-error listings).
		/// </summary>
		public void ShowTextInNewTab(string title, TextView.AvaloniaEditTextOutput output)
		{
			ArgumentNullException.ThrowIfNull(output);
			var content = new TextView.DecompilerTabPageModel {
				Language = languageService.CurrentLanguage,
				Title = title,
			};
			OpenNewTab(content);
			content.ShowText(output);
		}

		void ExecuteShowSearch()
		{
			ShowToolPane(ICSharpCode.ILSpy.Search.SearchPaneModel.PaneContentId);
			// Hand keyboard focus to the search input AFTER activating the pane — the view's
			// code-behind subscribes to FocusRequested and posts the focus shift onto the
			// dispatcher so the freshly-active pane has a frame to surface in the layout
			// first. Resolving the pane through AppComposition (instead of injecting it)
			// keeps the dock-workspace decoupled from the search namespace. TryGetExport is
			// null in design-time previews / minimal tests, where the activation alone suffices.
			AppEnv.AppComposition.TryGetExport<ICSharpCode.ILSpy.Search.SearchPaneModel>()?.RequestFocus();
		}

		/// <summary>
		/// Opens a fresh sibling tab whose <see cref="ContentTabPage.Content"/> is the
		/// supplied viewmodel. Used by explicit "Open in new tab" gestures and by static
		/// pages (About, License) that must not be overwritten by tree-node selections.
		/// </summary>
		/// <param name="sourceNode">
		/// Optional assembly-tree node this tab represents. Set BEFORE the tab is activated
		/// so <see cref="OnDocumentsPropertyChanged"/> can pick it up and pull the tree's
		/// selection across — setting it after-the-fact misses the activation event.
		/// </param>
		public ContentTabPage OpenNewTab(ContentPageModel content, SharpTreeNode? sourceNode = null)
		{
			// Carve-outs are born frozen — they survive tree-node selections instead of
			// being replaced like the preview MainTab.
			var tab = new ContentTabPage { Content = content, SourceNode = sourceNode, IsPreview = false };
			if (factory.Documents != null)
			{
				factory.AddDockable(factory.Documents, tab);
				factory.ActivateAndFocus(factory.Documents, tab);
			}
			return tab;
		}

		/// <summary>
		/// Open a retained static-content singleton tab (Options, About, an embedded resource
		/// page). If the tab already exists -- visible or previously closed -- the same instance
		/// is reactivated / re-added, so its owned view and content state survive close/reopen.
		/// Otherwise <paramref name="create"/> builds it once and it is retained for the session.
		/// </summary>
		public ContentTabPage OpenSingletonTab(string key, Func<ContentTabPage> create)
		{
			if (singletonTabs.TryGetValue(key, out var existing))
			{
				ReopenTab(existing);
				return existing;
			}
			var tab = create();
			singletonTabs[key] = tab;
			return tab;
		}

		// Re-display a retained tab: re-add it to the Documents dock if it was closed, then make
		// it active. The dockable (and the view it owns) is the same instance throughout.
		void ReopenTab(ContentTabPage tab)
		{
			if (factory.Documents is not { } docs)
				return;
			if (docs.VisibleDockables?.Contains(tab) != true)
				factory.AddDockable(docs, tab);
			factory.ActivateAndFocus(docs, tab);
		}

		/// <summary>
		/// Renders <paramref name="content"/> in the reusable MainTab as a transient welcome
		/// page (the About page shown at startup when nothing is selected). Unlike Help &gt;
		/// About this does not freeze a static page: the content is non-static, so the first
		/// tree-node selection reuses the MainTab and replaces it, leaving the user with a
		/// single tab rather than a stray empty one.
		/// </summary>
		/// <remarks>
		/// The greeting is fired asynchronously at startup (after the tree signals ready), so by
		/// the time it runs the user may already have opened a document tab that doesn't move the
		/// tree selection (e.g. Compare...). Only greet when the MainTab is still empty AND still
		/// the active document; otherwise stealing activation would yank the user off whatever
		/// they just opened.
		/// </remarks>
		public void ShowWelcomePage(TextView.DecompilerTabPageModel content)
		{
			if (factory.MainTab is not { } main)
				return;
			if (main.Content != null)
				return;
			if (factory.Documents is { } docs && !ReferenceEquals(docs.ActiveDockable, main))
				return;
			main.Content = content;
			welcomeContent = content;
			ActivateMainTabIfNeeded(main);
		}

		/// <summary>
		/// True while the startup welcome page is still the live MainTab content (it has not been
		/// replaced by a tree-node selection). The welcome page renders the same About text as
		/// Help &gt; About, so callers use this to avoid opening a duplicate About tab on top of it.
		/// </summary>
		public bool IsWelcomePageVisible
			=> welcomeContent is { } w && factory.MainTab is { } main && ReferenceEquals(main.Content, w);

		/// <summary>
		/// If the welcome page is still showing, bring its MainTab to the front and report success.
		/// Help &gt; About calls this first so that, while the welcome page (identical About content)
		/// is visible, the menu just activates it instead of spawning a second, static About tab.
		/// </summary>
		public bool TryActivateWelcomePage()
		{
			if (!IsWelcomePageVisible || factory.MainTab is not { } main)
				return false;
			if (factory.Documents is { } docs)
			{
				if (docs.VisibleDockables?.Contains(main) != true)
					return false;
				factory.ActivateAndFocus(docs, main);
			}
			return true;
		}

		/// <summary>
		/// Pump dispatcher jobs so synchronously-set selection state (SourceNode, IsPreview,
		/// tab count, Content reference) finishes propagating, without awaiting the
		/// fire-and-forget DecompileAsync. Tests that assert on dock structure can use this
		/// instead of WaitForDecompiledTextAsync to avoid the multi-second decompile wait.
		/// </summary>
		public void SettleSelection()
		{
			Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		}

		/// <summary>
		/// VS-style "freeze tab" gesture. The current <c>factory.MainTab</c> keeps its position,
		/// content, and active state but flips to frozen
		/// (<see cref="ContentTabPage.IsPreview"/> = false). No new tab spawns at freeze time:
		/// the next tree-node selection that finds the active tab frozen will lazily spawn
		/// a fresh preview tab inside <see cref="ShowSelectedNode"/>. Re-clicking the same
		/// node after freeze is a no-op (the dedupe activates the frozen tab).
		/// </summary>
		public void FreezeCurrentTab()
		{
			if (factory.FreezeCurrentMainTab() is null)
				return;
			// Cached decompiler viewmodel was bound to the just-frozen tab — drop the cache
			// so the next selection-change-into-frozen path materialises a fresh
			// DecompilerTabPageModel for the newly-spawned preview tab.
			decompilerContent = null;
		}

		public void CloseAllTabs()
			// Close every carve-out tab but keep the One (the persistent preview/home tab that the
			// rest of the app relies on always existing -- ShowSelectedNode reuses it). This also keeps
			// the document area non-empty, sidestepping Dock's "can't close the last dockable" veto.
			=> CloseTabsWhere(doc => !ReferenceEquals(doc, factory.MainTab));

		// Snapshot first -- CloseTab -> CloseDockable mutates the visible-dockables list.
		void CloseTabsWhere(Func<ContentTabPage, bool> shouldClose)
		{
			var docs = factory.Documents?.VisibleDockables;
			if (docs == null)
				return;
			foreach (var doc in docs.OfType<ContentTabPage>().ToArray())
			{
				if (shouldClose(doc))
					CloseTab(doc);
			}
		}

		/// <summary>Closes one document tab (e.g. the tab context menu's "Close"). Closing the One
		/// drops the cached decompiler viewmodel so the next selection forges a fresh preview tab.</summary>
		public void CloseTab(ContentTabPage tab)
		{
			ArgumentNullException.ThrowIfNull(tab);
			bool wasTheOne = ReferenceEquals(tab, factory.MainTab);
			factory.CloseDockable(tab);
			if (wasTheOne)
				decompilerContent = null;
		}

		/// <summary>Closes every document tab except <paramref name="keep"/> (the tab context menu's
		/// "Close all but this").</summary>
		public void CloseAllTabsExcept(ContentTabPage keep)
		{
			ArgumentNullException.ThrowIfNull(keep);
			CloseTabsWhere(doc => !ReferenceEquals(doc, keep));
		}

		/// <summary>Closes the active document tab (Ctrl+W). The documents dock only holds
		/// <see cref="ContentTabPage"/>s, so tool panes are never affected. Closing the One drops
		/// the cached decompiler viewmodel so the next selection forges a fresh preview tab.</summary>
		public void CloseActiveDocument()
		{
			if (factory.Documents?.ActiveDockable is not ContentTabPage active)
				return;
			bool wasTheOne = ReferenceEquals(active, factory.MainTab);
			factory.CloseDockable(active);
			if (wasTheOne)
				decompilerContent = null;
		}

		public void ResetLayout()
		{
			// Rebuild from the factory's default layout. The active tree node, if any, will be
			// re-projected onto the fresh MainTab through the existing
			// SelectedItem -> ShowSelectedNode plumbing.
			var newLayout = factory.CreateLayout();
			factory.InitLayout(newLayout);
			if (newLayout is IRootDock root && Layout is IRootDock currentRoot)
			{
				currentRoot.VisibleDockables = root.VisibleDockables;
				currentRoot.ActiveDockable = root.ActiveDockable;
				currentRoot.FocusedDockable = root.FocusedDockable;
			}
			// Fresh MainTab means the cached decompiler content (with handlers wired to the
			// old tab) is gone; rebuild on the next selection.
			decompilerContent = null;
			ShowSelectedNode();
		}
	}
}
