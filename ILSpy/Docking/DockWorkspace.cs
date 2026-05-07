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

using CommunityToolkit.Mvvm.Input;

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Core.Events;

using ICSharpCode.Decompiler.Metadata;
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

		public IRelayCommand NavigateBackCommand { get; }
		public IRelayCommand NavigateForwardCommand { get; }
		public IRelayCommand<NavigationEntry> NavigateToHistoryCommand { get; }

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
			this.assemblyTreeModel = assemblyTreeModel;
			this.languageService = languageService;
			NavigateBackCommand = new RelayCommand(NavigateBack, () => history.CanNavigateBack);
			NavigateForwardCommand = new RelayCommand(NavigateForward, () => history.CanNavigateForward);
			NavigateToHistoryCommand = new RelayCommand<NavigationEntry>(NavigateToHistory,
				entry => entry != null && (history.BackEntries.Contains(entry) || history.ForwardEntries.Contains(entry)));
			factory = new ILSpyDockFactory(toolPaneRegistry);
			Layout = factory.CreateLayout();

			assemblyTreeModel.PropertyChanged += OnAssemblyTreePropertyChanged;
			assemblyTreeModel.SelectedItems.CollectionChanged += (_, _) => ShowSelectedNode();
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

		void OnAssemblyTreePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(AssemblyTreeModel.SelectedItem))
			{
				ShowSelectedNode();
				RecordTreeNodeSelection(assemblyTreeModel.SelectedItem);
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
			history.Record(entry);
			NavigateBackCommand.NotifyCanExecuteChanged();
			NavigateForwardCommand.NotifyCanExecuteChanged();
		}

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
					assemblyTreeModel.SelectedItem = treeNode.Node;
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
			var node = assemblyTreeModel.FindTreeNode(segment.Reference);
			if (node != null)
				assemblyTreeModel.SelectedItem = node;
		}

		// Long-lived decompiler viewmodel — kept alive across metadata interludes so going
		// back to text doesn't lose the previous decompile until a fresh one supersedes it.
		// Created lazily on first need.
		DecompilerTabPageModel? decompilerContent;

		void ShowSelectedNode()
		{
			var nodes = assemblyTreeModel.SelectedItems.OfType<ILSpyTreeNode>().ToArray();
			if (nodes.Length == 0)
				return;
			if (factory.MainTab is not { } main)
				return;

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
				return;
			}

			var decTab = decompilerContent ??= CreateDecompilerContent();
			main.Content = decTab;
			decTab.Language = languageService.CurrentLanguage;
			decTab.CurrentNodes = nodes;
		}

		void AttachCustomContent(ContentTabPage main, TabPageModel newContent)
		{
			// Detach navigation handlers from the outgoing content; subscribe on the
			// incoming one so token clicks route through OnMetadataCellClicked.
			if (main.Content is MetadataTablePageModel oldMeta)
				oldMeta.NavigateToCellRequested -= OnMetadataCellClicked;
			if (newContent is MetadataTablePageModel newMeta)
				newMeta.NavigateToCellRequested += OnMetadataCellClicked;
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

		void NavigateToToken(MetadataTokenReference reference)
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
		/// Opens a fresh sibling tab whose <see cref="ContentTabPage.Content"/> is the
		/// supplied viewmodel. Used by explicit "Open in new tab" gestures and by static
		/// pages (About, License) that must not be overwritten by tree-node selections.
		/// </summary>
		public ContentTabPage OpenNewTab(object content)
		{
			var tab = new ContentTabPage { Content = content };
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
