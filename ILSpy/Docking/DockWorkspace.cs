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

using CommunityToolkit.Mvvm.Input;

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Core.Events;

using ICSharpCode.ILSpyX.TreeView;

using ILSpy.AssemblyTree;
using ILSpy.Commands;
using ILSpy.Languages;
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
			if (factory.InitialDecompilerTab is { } initialTab)
			{
				initialTab.Language = languageService.CurrentLanguage;
				initialTab.NavigateRequested += OnNavigateRequested;
			}

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
			var tab = ResolveDecompilerTab();
			if (tab == null)
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
				if (GetDecompilerContentTab() is { } tab)
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

		void ShowSelectedNode()
		{
			var nodes = assemblyTreeModel.SelectedItems.OfType<ILSpyTreeNode>().ToArray();
			if (nodes.Length == 0)
				return;

			// Single-node selections that opt into a custom tab (e.g. metadata-table nodes
			// returning a DataGrid model) get routed there directly. Multi-select keeps the
			// decompiler tab because there's no sensible composite for two unrelated grids.
			if (nodes.Length == 1 && nodes[0].CreateTab() is { } customTab)
			{
				ShowCustomTab(customTab);
				return;
			}

			var tab = ResolveDecompilerTab();
			if (tab == null)
				return;
			if (factory.Documents?.ActiveDockable != tab)
			{
				factory.SetActiveDockable(tab);
				if (factory.Documents != null)
					factory.SetFocusedDockable(factory.Documents, tab);
			}
			tab.Language = languageService.CurrentLanguage;
			tab.CurrentNodes = nodes;
		}

		void ShowCustomTab(TabPageModel tab)
		{
			if (factory.Documents == null)
				return;
			factory.AddDockable(factory.Documents, tab);
			factory.SetActiveDockable(tab);
			factory.SetFocusedDockable(factory.Documents, tab);
		}

		public DecompilerTabPageModel? ActiveDecompilerTab => GetDecompilerContentTab();

		/// <summary>
		/// Returns the tab tree-node selections should be rendered into. Prefers the active
		/// dockable when it's a non-static decompiler tab, falls back to any other non-static
		/// decompiler tab, and otherwise lazily attaches the factory's initial decompiler tab.
		/// Returns null if the document dock isn't realised yet.
		/// </summary>
		DecompilerTabPageModel? ResolveDecompilerTab()
		{
			if (factory.Documents == null)
				return null;
			if (GetDecompilerContentTab() is { } existing)
				return existing;
			var tab = factory.InitialDecompilerTab;
			if (tab == null)
				return null;
			factory.AddDockable(factory.Documents, tab);
			factory.SetActiveDockable(tab);
			factory.SetFocusedDockable(factory.Documents, tab);
			return tab;
		}

		// Decompiler-content tabs only — static-content tabs (About / License) are excluded so
		// tree-node selections never overwrite them.
		DecompilerTabPageModel? GetDecompilerContentTab()
		{
			if (factory.Documents?.ActiveDockable is DecompilerTabPageModel { IsStaticContent: false } active)
				return active;
			if (factory.Documents?.VisibleDockables != null)
			{
				foreach (var d in factory.Documents.VisibleDockables)
					if (d is DecompilerTabPageModel { IsStaticContent: false } m)
						return m;
			}
			return null;
		}

		public TabPageModel OpenNewTab(TabPageModel tab)
		{
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
			// re-projected onto the freshly-created decompiler tab through the existing
			// SelectedItem -> ShowSelectedNode plumbing.
			var newLayout = factory.CreateLayout();
			factory.InitLayout(newLayout);
			if (newLayout is IRootDock root && Layout is IRootDock currentRoot)
			{
				currentRoot.VisibleDockables = root.VisibleDockables;
				currentRoot.ActiveDockable = root.ActiveDockable;
				currentRoot.FocusedDockable = root.FocusedDockable;
			}
			if (factory.InitialDecompilerTab is { } initialTab)
			{
				initialTab.Language = languageService.CurrentLanguage;
				initialTab.NavigateRequested += OnNavigateRequested;
			}
			ShowSelectedNode();
		}
	}
}
