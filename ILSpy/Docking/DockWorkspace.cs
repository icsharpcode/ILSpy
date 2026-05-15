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
using System.ComponentModel;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Core.Events;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy.AssemblyTree;
using ILSpy.Commands;
using ILSpy.Languages;
using ILSpy.Metadata;
using ILSpy.Navigation;
using ILSpy.TextView;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;

namespace ILSpy.Docking
{
	[Export]
	[Shared]
	public class DockWorkspace
	{
		readonly ILSpyDockFactory factory;
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly LanguageService languageService;
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

		// Read-only history snapshots for the Back/Forward split-button dropdowns; oldest-first.
		// The UI reverses these for newest-first display.
		public IReadOnlyList<NavigationEntry> BackHistory => history.BackEntries;
		public IReadOnlyList<NavigationEntry> ForwardHistory => history.ForwardEntries;

		public IRootDock Layout { get; }

		public IReadOnlyList<ToolPaneMenuItem> ToolPaneMenuItems { get; }

		[ImportingConstructor]
		public DockWorkspace(
			AssemblyTreeModel assemblyTreeModel,
			ToolPaneRegistry toolPaneRegistry,
			LanguageService languageService)
		{
			ILSpy.AppEnv.StartupLog.Mark("DockWorkspace ctor entered");
			this.assemblyTreeModel = assemblyTreeModel;
			this.languageService = languageService;
			NavigateBackCommand = new RelayCommand(NavigateBack, () => history.CanNavigateBack);
			NavigateForwardCommand = new RelayCommand(NavigateForward, () => history.CanNavigateForward);
			NavigateToHistoryCommand = new RelayCommand<NavigationEntry>(NavigateToHistory,
				entry => entry != null && (history.BackEntries.Contains(entry) || history.ForwardEntries.Contains(entry)));
			ShowSearchCommand = new RelayCommand(ExecuteShowSearch);
			using (ILSpy.AppEnv.StartupLog.Phase("ILSpyDockFactory ctor + CreateLayout"))
			{
				factory = new ILSpyDockFactory(toolPaneRegistry);
				Layout = factory.CreateLayout();
			}

			assemblyTreeModel.PropertyChanged += OnAssemblyTreePropertyChanged;
			assemblyTreeModel.SelectedItems.CollectionChanged += (_, _) => {
				if (!syncingTreeFromActiveTab)
					ShowSelectedNode();
			};
			languageService.PropertyChanged += OnLanguagePropertyChanged;
			// Layout/factory initialization (locators, parent/factory wiring) is done by
			// the DockControl in MainWindow.axaml via InitializeFactory/InitializeLayout.

			ToolPaneMenuItems = toolPaneRegistry.Panes
				.Select(p => new ToolPaneMenuItem(p.Pane, factory))
				.ToList();

			factory.DockableAdded += OnDocumentMembershipChanged;
			factory.DockableRemoved += OnDocumentMembershipChanged;
			factory.DockableClosed += OnDocumentMembershipChanged;
			factory.DockableClosing += OnDockableClosing;
			factory.ActiveDockableChanged += OnActiveDockableChanged;
			ILSpy.AppEnv.StartupLog.Mark("DockWorkspace ctor exited");

			// TODO: layout persistence (load on startup, save on exit). DockSerializer.SystemTextJson
			// trips System.Text.Json's MaxDepth=64 limit on our layout because Dock's JsonConverterList<T>
			// calls JsonSerializer.Serialize per list element, which doesn't preserve the writer's
			// reference-tracking state between calls — so Owner/Factory back-refs aren't deduplicated as
			// $ref. Options when revisiting: try Dock.Serializer.Newtonsoft (better cycle handling),
			// or build a custom JsonSerializerOptions with MaxDepth raised + replicate Dock's internal
			// polymorphic resolver.
		}

		void OnDocumentMembershipChanged(object? sender, EventArgs e) => UpdateLastDocumentCanClose();

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

		// When only one document is open, make it un-closeable so the user can't end up with an
		// empty document area.
		void UpdateLastDocumentCanClose()
		{
			var docs = factory.Documents?.VisibleDockables;
			if (docs == null)
				return;
			bool canClose = docs.Count > 1;
			foreach (var d in docs)
			{
				d.CanClose = canClose;
			}
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

		void OnActiveDockableChanged(object? sender, ActiveDockableChangedEventArgs e)
		{
			// Carve-out tab → active: pull the tree's selection over to whatever entity the
			// tab is showing. The same hook fires when the user clicks a different tab in
			// the tab strip, keeping the tree in lockstep with the visible content.
			if (e.Dockable is not ContentTabPage tab || tab.SourceNode is not { } node)
				return;
			if (ReferenceEquals(assemblyTreeModel.SelectedItem, node))
				return;
			syncingTreeFromActiveTab = true;
			try
			{
				assemblyTreeModel.SelectedItem = node;
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
			var decompTab = UnwrapDecompilerTab(current.Tab);
			if (decompTab == null)
				return;
			// Foldings have no model-side property-change event we can mirror onto, so the
			// text view's snapshot delegate has to be invoked synchronously here. Caret and
			// scroll values are kept fresh by per-event push from the view; foldings aren't.
			decompTab.CaptureFoldingsState?.Invoke();
			current.CaretOffset = decompTab.LastKnownCaretOffset;
			current.VerticalOffset = decompTab.LastKnownVerticalOffset;
			current.HorizontalOffset = decompTab.LastKnownHorizontalOffset;
			current.Foldings = decompTab.LastKnownFoldings;
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
						decompTab.PendingCaretOffset = treeNode.CaretOffset;
						decompTab.PendingVerticalOffset = treeNode.VerticalOffset;
						decompTab.PendingHorizontalOffset = treeNode.HorizontalOffset;
						decompTab.PendingFoldings = treeNode.Foldings;
					}
					assemblyTreeModel.SelectedItem = treeNode.Node;
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
			if (e.PropertyName == nameof(LanguageService.CurrentLanguage))
			{
				if (ActiveDecompilerTab is { } tab)
				{
					tab.Language = languageService.CurrentLanguage;
					// Re-decompile by re-assigning the same node so the tab refreshes for the new language.
					var node = tab.CurrentNode;
					tab.CurrentNode = null;
					tab.CurrentNode = node;
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
			if (segment.Reference is global::ILSpy.EntityReference entity
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
		{
			try
			{
				return AppEnv.AppComposition.Current.GetExports<Commands.IProtocolHandler>();
			}
			catch
			{
				return System.Array.Empty<Commands.IProtocolHandler>();
			}
		}

		// Long-lived decompiler viewmodel — kept alive across metadata interludes so going
		// back to text doesn't lose the previous decompile until a fresh one supersedes it.
		// Created lazily on first need.
		DecompilerTabPageModel? decompilerContent;

		ILSpyTreeNode[]? lastShownNodes;

		void ShowSelectedNode()
		{
			var nodes = assemblyTreeModel.SelectedItems.OfType<ILSpyTreeNode>().ToArray();
			if (nodes.Length == 0)
			{
				lastShownNodes = null;
				return;
			}
			if (factory.MainTab is not { } main)
				return;
			// SelectedItems.CollectionChanged and SelectedItem PropertyChanged both fan into
			// here on a single click, so dedupe to avoid creating two TabPageModels for the
			// same selection — the second one's columns would replace the first's, but the
			// first's filter wiring would be left dangling on stale ColumnFilter instances.
			if (lastShownNodes is { } prev && prev.SequenceEqual(nodes))
				return;
			lastShownNodes = nodes;

			// Tree-node selections always reuse the single document slot. The Document
			// instance never changes; its inner Content swaps between viewmodels. The
			// wrapper view (ContentTabPageView) holds pre-realised inner views and toggles
			// which is visible based on Content's runtime type — keeps the visual swap
			// deterministic without going through Dock's add+close lifecycle.
			// Carve-outs ("Open in new tab", "Freeze tab") aren't implemented yet and would
			// branch off this path before the Content swap.
			var customContent = nodes.Length == 1 ? nodes[0].CreateTab() : null;

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
				return;
			}

			var decTab = decompilerContent ??= CreateDecompilerContent();
			main.Content = decTab;
			decTab.Language = languageService.CurrentLanguage;
			decTab.CurrentNodes = nodes;
			main.SourceNode = nodes.Length == 1 ? nodes[0] : null;
		}

		void AttachCustomContent(ContentTabPage main, TabPageModel newContent)
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
			// (row, columnName) → metadata token + module → matching table tree node + row index.
			// Resolved via reflection because each entry struct buries its MetadataFile in a
			// private readonly field with the same name across all 43 typed table viewmodels.
			var prop = e.Row.GetType().GetProperty(e.ColumnName);
			if (prop?.GetValue(e.Row) is not int token || token == 0)
				return;
			var fileField = e.Row.GetType().GetField("metadataFile", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fileField?.GetValue(e.Row) is not MetadataFile metadataFile)
				return;
			NavigateToToken(new MetadataTokenReference(metadataFile, MetadataTokens.EntityHandle(token)));
		}

		internal void OnMetadataRowActivated(MetadataRowActivationEventArgs e)
		{
			// Row double-click: resolve the row's Token + metadataFile to an IEntity, then
			// either select the matching tree node (single-tab reuse) or open it in a fresh
			// tab (MMB / context-menu carve-out).
			var node = TryResolveRowToTreeNode(e.Row);
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
		/// handler, and the "Decompile to new tab" context-menu entry.
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
		/// Reflection-based resolution of a metadata-grid row to the matching assembly-tree
		/// node, sharing the path between row double-click and the "Decompile to new tab"
		/// context-menu entry. Returns <see langword="null"/> when the row's token doesn't
		/// resolve to an <see cref="IEntity"/> the tree knows how to model (heap rows,
		/// AssemblyRef rows pointing at unloaded assemblies, etc.).
		/// </summary>
		internal ILSpyTreeNode? TryResolveRowToTreeNode(object row)
		{
			var fileField = row.GetType().GetField("metadataFile", BindingFlags.Instance | BindingFlags.NonPublic);
			if (fileField?.GetValue(row) is not MetadataFile metadataFile)
				return null;
			var tokenProp = row.GetType().GetProperty("Token");
			if (tokenProp?.GetValue(row) is not int token || token == 0)
				return null;
			var handle = MetadataTokens.EntityHandle(token);
			if (handle.IsNil)
				return null;

			var assemblies = assemblyTreeModel.AssemblyList?.GetAssemblies();
			if (assemblies is null)
				return null;
			LoadedAssembly? owningAssembly = null;
			foreach (var a in assemblies)
			{
				if (ReferenceEquals(a.GetMetadataFileOrNull(), metadataFile))
				{
					owningAssembly = a;
					break;
				}
			}
			if (owningAssembly is null)
				return null;
			var ts = owningAssembly.GetTypeSystemOrNull();
			if (ts?.MainModule is not MetadataModule metadataModule)
				return null;
			IEntity? entity;
			try
			{ entity = metadataModule.ResolveEntity(handle); }
			catch { return null; }
			if (entity is null)
				return null;
			return assemblyTreeModel.FindTreeNode(entity);
		}

		public void NavigateToToken(MetadataTokenReference reference)
		{
			if (reference.Handle.IsNil)
				return;
			// Find the MetadataTreeNode for the referenced module, drill down to its
			// Tables container, and pick the table whose Kind matches the handle's runtime
			// kind — HandleKind values for table-backed entities double as TableIndex.
			var assemblies = assemblyTreeModel.AssemblyList?.GetAssemblies();
			if (assemblies == null)
				return;
			AssemblyTreeNode? targetAssembly = null;
			if (assemblyTreeModel.Root != null)
			{
				foreach (var child in assemblyTreeModel.Root.Children.OfType<AssemblyTreeNode>())
				{
					if (ReferenceEquals(child.LoadedAssembly.GetMetadataFileOrNull(), reference.MetadataFile))
					{
						targetAssembly = child;
						break;
					}
				}
			}
			if (targetAssembly == null)
				return;
			targetAssembly.EnsureLazyChildren();
			var metaNode = targetAssembly.Children.OfType<MetadataTreeNode>().FirstOrDefault();
			if (metaNode == null)
				return;
			metaNode.EnsureLazyChildren();
			var tablesNode = metaNode.Children.OfType<MetadataTablesTreeNode>().FirstOrDefault();
			if (tablesNode == null)
				return;
			tablesNode.EnsureLazyChildren();
			var tableIndex = (TableIndex)(int)reference.Handle.Kind;
			var tableNode = tablesNode.Children.OfType<MetadataTableTreeNode>()
				.FirstOrDefault(t => t.Kind == tableIndex);
			if (tableNode == null)
				return;

			suppressHistoryRecording = true;
			try
			{
				assemblyTreeModel.SelectedItem = tableNode;
			}
			finally
			{
				suppressHistoryRecording = false;
			}

			// ShowSelectedNode just settled MainTab.Content to the table's MetadataTablePageModel.
			// Set ScrollToRow to the handle's row number (1-based → 0-based).
			if (factory.MainTab?.Content is MetadataTablePageModel pm)
				pm.ScrollToRow = MetadataTokens.GetRowNumber((EntityHandle)reference.Handle) - 1;
		}

		DecompilerTabPageModel CreateDecompilerContent()
		{
			var tab = new DecompilerTabPageModel { Title = "(no selection)" };
			tab.Language = languageService.CurrentLanguage;
			tab.NavigateRequested += OnNavigateRequested;
			return tab;
		}

		static void CopyContentState(object source, object target)
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
		/// Opens a fresh sibling tab whose <see cref="ContentTabPage.Content"/> is the
		/// supplied viewmodel. Used by explicit "Open in new tab" gestures and by static
		/// pages (About, License) that must not be overwritten by tree-node selections.
		/// </summary>
		/// <param name="sourceNode">
		/// Optional assembly-tree node this tab represents. Set BEFORE the tab is activated
		/// so <see cref="OnActiveDockableChanged"/> can pick it up and pull the tree's
		/// selection across — setting it after-the-fact misses the activation event.
		/// </param>
		/// <summary>
		/// Brings the tool pane with the given <paramref name="contentId"/> to focus. Walks
		/// every <see cref="IDockable"/> in the layout via <see cref="GetAllDockables"/> and
		/// matches on <see cref="IDockable.Id"/>. Silently no-ops when the pane isn't in the
		/// layout — e.g. it was closed via its X button and hasn't been re-added.
		/// </summary>
		public void ShowToolPane(string contentId)
		{
			foreach (var dockable in GetAllDockables(Layout))
			{
				if (dockable is ToolPaneModel toolPane && toolPane.Id == contentId)
				{
					factory.SetActiveDockable(toolPane);
					if (toolPane.Owner is IDock owner)
						factory.SetFocusedDockable(owner, toolPane);
					return;
				}
			}
		}

		void ExecuteShowSearch()
		{
			ShowToolPane(ILSpy.Search.SearchPaneModel.PaneContentId);
			// Hand keyboard focus to the search input AFTER activating the pane — the view's
			// code-behind subscribes to FocusRequested and posts the focus shift onto the
			// dispatcher so the freshly-active pane has a frame to surface in the layout
			// first. Resolving the pane through AppComposition (instead of injecting it)
			// keeps the dock-workspace decoupled from the search namespace.
			try
			{
				var search = AppEnv.AppComposition.Current.GetExport<ILSpy.Search.SearchPaneModel>();
				search.RequestFocus();
			}
			catch
			{
				// Composition isn't available in design-time previews / minimal tests; the
				// activation alone is enough to be useful there.
			}
		}

		static System.Collections.Generic.IEnumerable<IDockable> GetAllDockables(IDockable? root)
		{
			if (root == null)
				yield break;
			yield return root;
			if (root is IDock dock && dock.VisibleDockables != null)
			{
				foreach (var child in dock.VisibleDockables)
				{
					foreach (var descendant in GetAllDockables(child))
						yield return descendant;
				}
			}
		}

		public ContentTabPage OpenNewTab(object content, SharpTreeNode? sourceNode = null)
		{
			var tab = new ContentTabPage { Content = content, SourceNode = sourceNode };
			if (factory.Documents != null)
			{
				factory.AddDockable(factory.Documents, tab);
				factory.SetActiveDockable(tab);
				factory.SetFocusedDockable(factory.Documents, tab);
			}
			return tab;
		}

		public void CloseAllTabs()
		{
			var docs = factory.Documents?.VisibleDockables;
			if (docs == null)
				return;
			// Snapshot first — CloseDockable mutates VisibleDockables.
			foreach (var doc in System.Linq.Enumerable.ToArray(docs))
				factory.CloseDockable(doc);
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
