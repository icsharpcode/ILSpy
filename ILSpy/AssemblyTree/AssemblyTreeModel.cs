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
using System.Collections.Specialized;
using System.Composition;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.AssemblyTree
{
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Left, Order = 0)]
	[Shared]
	public partial class AssemblyTreeModel : ToolPaneModel
	{
		public const string PaneContentId = "AssemblyTree";

		readonly SettingsService settingsService;
		readonly LanguageService languageService;
		AssemblyListManager? listManager;
		// True until the first assembly-list activation completes — gates the startup-only
		// About-page greeting so switching lists later never re-triggers it.
		bool initialActivation = true;
		AssemblyListTreeNode? assemblyListTreeNode;

		[ObservableProperty]
		[property: IgnoreDataMember]
		private SharpTreeNode? root;

		/// <summary>
		/// Multi-selection set. Each entry is kept in sync with its
		/// <see cref="SharpTreeNode.IsSelected"/>. <see cref="SelectedItem"/> is a convenience
		/// wrapper around the *primary* (last-added) entry — drives decompilation, navigation
		/// history, and tree-view-path persistence — but the underlying state is single-sourced
		/// here.
		/// </summary>
		[IgnoreDataMember]
		public ObservableCollection<SharpTreeNode> SelectedItems { get; } = [];

		/// <summary>
		/// Primary (last) selection. Get returns the most recently selected entry of
		/// <see cref="SelectedItems"/>, or <c>null</c>; set replaces the entire selection
		/// with the supplied node (clears the collection then adds it). All
		/// <c>PropertyChanged(SelectedItem)</c> notifications are fired by
		/// <see cref="SelectedItems.CollectionChanged"/>.
		/// </summary>
		[IgnoreDataMember]
		public SharpTreeNode? SelectedItem {
			get => SelectedItems.Count > 0 ? SelectedItems[^1] : null;
			set {
				if (SelectedItem == value)
					return;
				SelectNodes(value == null ? System.Array.Empty<SharpTreeNode>() : new[] { value });
			}
		}

		/// <summary>
		/// Replaces the whole selection with <paramref name="nodes"/> in ONE logical change. The
		/// collection can't be swapped atomically (Clear()+Add() passes through a transient empty,
		/// add-before-remove through a transient multi), so the selection-changed fan-out is
		/// batched: per-element IsSelected toggling still happens, but the PropertyChanged / path /
		/// message-bus notifications fire once, AFTER, with the final set. Without this a transient
		/// empty poisons the grid sync's deferred guard (tree stops following tab activation) and a
		/// transient multi confuses count-sensitive consumers (metadata-tab reuse). Drives both the
		/// single-node <see cref="SelectedItem"/> setter and multi-node tab-activation restore.
		/// </summary>
		public void SelectNodes(IReadOnlyList<SharpTreeNode> nodes)
		{
			ArgumentNullException.ThrowIfNull(nodes);
			if (SelectionMatches(nodes))
				return;
			batchingSelectionChange = true;
			try
			{
				SelectedItems.Clear();
				foreach (var node in nodes)
				{
					if (node != null && !SelectedItems.Contains(node))
						SelectedItems.Add(node);
				}
			}
			finally
			{
				batchingSelectionChange = false;
			}
			RaiseSelectionChanged();
		}

		bool SelectionMatches(IReadOnlyList<SharpTreeNode> nodes)
		{
			if (SelectedItems.Count != nodes.Count)
				return false;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (!SelectedItems.Contains(nodes[i]))
					return false;
			}
			return true;
		}

		[ObservableProperty]
		[property: IgnoreDataMember]
		private string? activeListName;

		[IgnoreDataMember]
		public AssemblyList? AssemblyList { get; private set; }

		[IgnoreDataMember]
		public ObservableCollection<string> AssemblyLists { get; } = [];

		[ImportingConstructor]
		public AssemblyTreeModel(SettingsService settingsService, LanguageService languageService)
		{
			AppEnv.AppLog.Mark("AssemblyTreeModel ctor entered");
			this.settingsService = settingsService;
			this.languageService = languageService;
			languageService.PropertyChanged += (_, e) => {
				if (e.PropertyName == nameof(LanguageService.CurrentLanguage) && Root != null)
					NotifyTextChanged(Root);
			};
			SelectedItems.CollectionChanged += OnSelectedItemsChanged;
			// Single hub for "navigate to this reference, optionally highlighting that source"
			// — mirrors WPF AssemblyTreeModel's JumpToReference subscription. The analyzer
			// pane, metadata tables, and future decompile commands all push through this same
			// channel.
			Util.MessageBus<Util.NavigateToReferenceEventArgs>.Subscribers += OnNavigateToReference;
			// Live re-render when Display Settings change. WPF leaves these as apply-on-next-
			// load; Avalonia opts into reactivity because the Options dialog stays open while
			// the user toggles. Property dispatch keeps the work narrow to the affected nodes.
			Util.MessageBus<Util.SettingsChangedEventArgs>.Subscribers += OnSettingsChanged;
			Id = PaneContentId;
			Title = "Assemblies";
			CanClose = false;
			AppEnv.AppLog.Mark("AssemblyTreeModel ctor exited");
		}

		void OnNavigateToReference(object? sender, Util.NavigateToReferenceEventArgs e)
		{
			if (e.Reference is not IEntity entity)
				return;
			var resolved = FindTreeNode(entity);
			if (resolved == null)
				return;
			if (e.InNewTabPage)
			{
				// Open the definition in a fresh carve-out tab instead of replacing the current view
				// (e.g. "Decompile to new tab" on a symbol in the code).
				AppEnv.AppComposition.TryGetExport<Docking.DockWorkspace>()?.OpenNodeInNewTab(resolved);
			}
			else
			{
				SelectedItem = resolved;
			}
			// Source is the originally-analysed entity (set by AnalyzerEntityTreeNode.ActivateItem).
			// Push it onto the active decompiler tab's HighlightedReference so the editor view
			// paints local-reference marks on every match once the new Text lands.
			if (e.Source is null)
				return;
			var dockWorkspace = AppEnv.AppComposition.TryGetExport<Docking.DockWorkspace>();
			if (dockWorkspace?.ActiveDecompilerTab is { } decompTab)
				decompTab.HighlightedReference = e.Source;
		}

		// True while the SelectedItem setter is replacing the collection via Clear()+Add();
		// suppresses the selection-changed fan-out until the final state is in place so consumers
		// never observe the transient empty/multi mid-replace.
		bool batchingSelectionChange;

		void OnSelectedItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
				foreach (SharpTreeNode n in e.NewItems)
					n.IsSelected = true;
			if (e.OldItems != null)
				foreach (SharpTreeNode n in e.OldItems)
					n.IsSelected = false;

			// During a SelectedItem-setter batch the fan-out is deferred to the single
			// RaiseSelectionChanged() the setter issues once the final selection is in place.
			if (batchingSelectionChange)
				return;
			RaiseSelectionChanged();
		}

		void RaiseSelectionChanged()
		{
			// SelectedItem is a wrapper over this collection — anyone bound to it must be
			// notified, and the saved path must follow the new primary.
			OnPropertyChanged(nameof(SelectedItem));
			settingsService.SessionSettings.ActiveTreeViewPath = GetPathForNode(SelectedItem);
			// Auto-loaded (dependency-resolved) assemblies are not part of the saved list, so
			// on next launch the tree-path walk would stop at the assembly-list node. Storing
			// the file lets the restore re-add it before walking the path.
			settingsService.SessionSettings.ActiveAutoLoadedAssembly = GetAutoLoadedAssemblyPath(SelectedItem);

			// Pub-sub fan-out: panes (e.g. Debug Steps) that need to invalidate their state
			// when the selection moves listen on this message.
			Util.MessageBus.Send(this, new Util.AssemblyTreeSelectionChangedEventArgs());

			// Selection-dependent menu commands (Save, Analyze-via-menu, ...) re-evaluate CanExecute.
			Commands.CommandManager.InvalidateRequerySuggested();
		}

		// Walks already-materialized children and re-raises Text PropertyChanged so the cell
		// templates pick up the new language's formatting -- without collapsing the user's
		// expanded state. Lazy-loaded subtrees that haven't been opened yet are skipped (they'll
		// format with the active language the next time they get expanded).
		static void NotifyTextChanged(SharpTreeNode node)
		{
			node.RaisePropertyChanged(nameof(SharpTreeNode.Text));
			if (node.LazyLoading)
				return;
			foreach (var child in node.Children)
				NotifyTextChanged(child);
		}

		void OnSettingsChanged(object? sender, Util.SettingsChangedEventArgs e)
		{
			if (sender is not Options.DisplaySettings)
				return;
			if (Root == null)
				return;
			// One classification table (DisplaySettingReactions) drives every reaction, so a
			// newly-added setting can't silently fall through -- the coverage test fails until it's
			// listed. The Options page is non-modal/live-apply, so there is no full-refresh-on-close
			// fallback the way the WPF host had; each bucket must do its own update.
			var name = e.Inner.PropertyName;
			switch (Options.DisplaySettingReactions.For(name))
			{
				case Options.DisplaySettingReaction.TreeText:
					// Text suffix on member nodes is computed at read time — just fire the
					// notification so bound cell templates re-pull.
					NotifyTextChanged(Root);
					break;
				case Options.DisplaySettingReaction.TreeShape:
					if (name == nameof(Options.DisplaySettings.UseNestedNamespaceNodes))
					{
						// Every loaded AssemblyTreeNode's namespace subtree needs rebuilding.
						foreach (var asm in Root.Children.OfType<TreeNodes.AssemblyTreeNode>())
							asm.ReloadChildren();
					}
					else
					{
						// Visible MetadataTablesTreeNode instances get their children regenerated;
						// untouched (lazy) ones already pick up the new value on first expand.
						RebuildMetadataTablesIn(Root);
					}
					// AssemblyListPane caches a snapshot of each expanded node's children via the
					// HierarchicalOptions.ChildrenSelector — mid-expand mutations of node.Children
					// aren't observed. Re-raising Root forces BindTree to fire, which creates a fresh
					// HierarchicalModel that re-reads children on demand.
					OnPropertyChanged(nameof(Root));
					break;
				case Options.DisplaySettingReaction.Redecompile:
					// Baked into the decompiler/disassembler output (folding, member/using
					// expansion, debug info, IL detail, indentation), so it only shows after a
					// re-decompile. Refresh in place: changing an option must not switch the
					// user's current tab.
					RefreshDecompiledViewInPlace();
					break;
					// EditorLive / None: the text view applies editor settings to AvaloniaEdit itself,
					// and the rest have no model-side reaction.
			}
		}

		static void RebuildMetadataTablesIn(SharpTreeNode node)
		{
			if (node is Metadata.MetadataTablesTreeNode tables)
			{
				tables.ReloadChildren();
				return;
			}
			if (node.LazyLoading)
				return;
			foreach (var child in node.Children)
				RebuildMetadataTablesIn(child);
		}

		readonly TaskCompletionSource<bool> treeReadyTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

		/// <summary>
		/// Completes when the assembly-tree view (<c>AssemblyListPane</c>) has fired its
		/// <c>Loaded</c> event for the first time. <c>RestoreSelectedPathAsync</c> awaits
		/// this before assigning <see cref="SelectedItem"/> so the saved-selection path
		/// (which kicks off a decompilation through the dock workspace) doesn't paint
		/// the document area before the tree itself is on screen.
		/// </summary>
		public Task TreeReady => treeReadyTcs.Task;

		/// <summary>
		/// Called by <c>AssemblyListPane</c> from its <c>Loaded</c> handler to resolve
		/// <see cref="TreeReady"/>. Idempotent — only the first call wins.
		/// </summary>
		internal void MarkTreeReady()
		{
			if (treeReadyTcs.TrySetResult(true))
				AppEnv.AppLog.Mark("AssemblyTreeModel.TreeReady completed");
		}

		public void Initialize()
		{
			using var _ = AppEnv.AppLog.Phase("AssemblyTreeModel.Initialize body");
			listManager = settingsService.AssemblyListManager;
			using (AppEnv.AppLog.Phase("CreateDefaultAssemblyLists"))
				listManager.CreateDefaultAssemblyLists();

			SyncListNames();
			listManager.AssemblyLists.CollectionChanged += (_, _) => SyncListNames();

			var saved = settingsService.SessionSettings.ActiveAssemblyList;
			ActiveListName = !string.IsNullOrEmpty(saved) && AssemblyLists.Contains(saved)
				? saved
				: AssemblyListManager.DefaultListName;
		}

		void SyncListNames()
		{
			if (listManager == null)
				return;
			AssemblyLists.Clear();
			foreach (var name in listManager.AssemblyLists)
				AssemblyLists.Add(name);
		}

		partial void OnActiveListNameChanged(string? value)
		{
			if (listManager == null || string.IsNullOrEmpty(value))
				return;

			settingsService.SessionSettings.ActiveAssemblyList = value;
			ShowAssemblyList(value);

			// First activation is the startup one; a later switch to a different list isn't.
			// Only startup gets the empty-selection About-page greeting.
			bool isInitial = initialActivation;
			initialActivation = false;

			// Restore the previously-selected tree node off the UI critical path. The walk
			// crosses an AssemblyTreeNode whose EnsureLazyChildren synchronously blocks on
			// GetLoadResultAsync — by going async here and awaiting the load, we let the
			// initial paint happen first and the UI stays responsive while metadata loads.
			//
			// Skipped when --navigateto was supplied on the command line: the user explicitly
			// asked us to navigate somewhere else, and racing the saved-path restore against
			// the explicit target produces two concurrent decompiles (the saved one then the
			// requested one — last write wins on SelectedItem). For perf-benchmarking via
			// `-n T:Some.Type` this matters: the saved decompile pollutes the measurement.
			if (App.CommandLineArguments?.NavigateTo is { Length: > 0 })
				return;
			var savedPath = settingsService.SessionSettings.ActiveTreeViewPath;
			// Re-open the previously auto-loaded dependency assembly before walking the saved
			// path -- otherwise the walk stops at the assembly-list root and the selection
			// silently doesn't restore. File.Exists guards against the user having moved or
			// deleted the file between sessions; a missing dependency just degrades to "tree
			// path doesn't resolve" instead of crashing startup.
			var autoLoaded = settingsService.SessionSettings.ActiveAutoLoadedAssembly;
			if (!string.IsNullOrEmpty(autoLoaded) && System.IO.File.Exists(autoLoaded))
				AssemblyList?.OpenAssembly(autoLoaded, isAutoLoaded: true);
			_ = RestoreSelectedPathAsync(savedPath, isInitial);
		}

		async Task RestoreSelectedPathAsync(string[]? path, bool isInitial)
		{
			using var _ = AppEnv.AppLog.Phase($"RestoreSelectedPathAsync ({path?.Length ?? 0} segments, initial={isInitial})");
			try
			{
				// Snapshot — if the user selects something else before the restore completes,
				// don't yank their selection out from under them.
				var initialSelection = SelectedItem;
				SharpTreeNode? node = null;
				if (path is { Length: > 0 } && Root != null)
				{
					node = Root;
					foreach (var element in path)
					{
						if (node == null)
							break;
						// Awaiting GetLoadResultAsync keeps EnsureLazyChildren — which itself does
						// GetAwaiter().GetResult() on the same task — from blocking the UI thread.
						// Once the load completes the .GetAwaiter().GetResult() returns instantly.
						if (node is AssemblyTreeNode asm)
						{
							using (AppEnv.AppLog.Phase($"await GetLoadResultAsync ({asm.LoadedAssembly.ShortName})"))
								await asm.LoadedAssembly.GetLoadResultAsync().ConfigureAwait(true);
						}
						using (AppEnv.AppLog.Phase($"EnsureLazyChildren ({node.GetType().Name} \"{element}\")"))
							node.EnsureLazyChildren();
						node = node.Children.FirstOrDefault(c => c.ToString() == element);
					}
				}
				// Wait for the tree view to be Loaded before assigning SelectedItem. Without
				// this, the SelectedItem assignment runs ShowSelectedNode → CurrentNodes →
				// async decompilation, and the user can see the decompiled output appear
				// BEFORE the assembly tree has rendered — a confusing reverse order.
				// The 5-second timeout is a safety net for environments where the pane
				// never loads (headless tests, design-time previews).
				using (AppEnv.AppLog.Phase("await TreeReady before SelectedItem assignment"))
				{
					await Task.WhenAny(TreeReady, Task.Delay(TimeSpan.FromSeconds(5)))
						.ConfigureAwait(true);
				}
				// Bail if the user selected something else while we were resolving.
				if (!ReferenceEquals(SelectedItem, initialSelection))
					return;
				if (node != null && node != Root)
				{
					SelectedItem = node;
				}
				else if (isInitial && SelectedItem == null)
				{
					// Launched with nothing to restore (no saved path, or it no longer resolves):
					// greet the user with the About page in the main tab instead of a blank view.
					ShowAboutWelcomePage();
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[AssemblyTreeModel] saved-path restore failed: {ex}");
			}
		}

		// Open the About page as the startup welcome screen. The command is resolved lazily
		// through the menu registry (it's an ExportFactory, not instantiated until needed),
		// and its instance IS the AboutCommand, so we can drive its welcome path directly.
		void ShowAboutWelcomePage()
		{
			var registry = AppEnv.AppComposition.TryGetExport<MainMenuCommandRegistry>();
			var export = registry?.Commands
				.FirstOrDefault(c => c.Metadata.Header == nameof(ICSharpCode.ILSpy.Properties.Resources._About))
				?.CreateExport();
			if (export?.Value is Commands.AboutCommand about)
				about.ShowWelcome();
		}

		void ShowAssemblyList(string name)
		{
			if (listManager == null)
				return;
			AssemblyList list;
			using (AppEnv.AppLog.Phase($"LoadList({name})"))
				list = listManager.LoadList(name);
			if (AssemblyList == null || list.ListName != AssemblyList.ListName)
				ShowAssemblyList(list);
		}

		void ShowAssemblyList(AssemblyList list)
		{
			using var _ = AppEnv.AppLog.Phase("ShowAssemblyList(list)");
			// Detach the previous list's collection-changed wiring so the MessageBus
			// republisher and the navigation-history pruning don't fire against a stale
			// list. Re-attach on the new list so panes (DockWorkspace, SearchPaneModel)
			// that subscribe to CurrentAssemblyListChangedEventArgs see add/remove events
			// from the live list.
			if (AssemblyList is { } previous)
				previous.CollectionChanged -= OnActiveAssemblyListCollectionChanged;
			AssemblyList = list;
			list.CollectionChanged += OnActiveAssemblyListCollectionChanged;
			if (list.GetAssemblies().Length == 0 && list.ListName == AssemblyListManager.DefaultListName)
			{
				using (AppEnv.AppLog.Phase("LoadInitialAssemblies"))
					LoadInitialAssemblies(list, listManager);
			}
			AppEnv.AppLog.Mark($"AssemblyList contains {list.GetAssemblies().Length} assemblies");
			using (AppEnv.AppLog.Phase("new AssemblyListTreeNode"))
				assemblyListTreeNode = new AssemblyListTreeNode(list);
			Root = assemblyListTreeNode;
			AppEnv.AppLog.Mark("Root assigned");
			ScheduleBackgroundLoadSweep(list);
		}

		/// <summary>
		/// LoadedAssembly entries are now lazy — their <c>Task.Run(LoadAsync)</c> only kicks
		/// off when something asks for the metadata. The active assembly's load is awaited
		/// by <see cref="RestoreSelectedPathAsync"/>, so it gets a clean run. Everything else
		/// only loads on-demand (tree expansion, hyperlink follow, …) which can stretch
		/// quietly into "the user never sees a populated icon for assemblies they don't
		/// touch".
		///
		/// To strike a middle ground, schedule a one-shot sweep that fires once the tree
		/// view is on screen (<see cref="TreeReady"/>).
		/// Gating on <see cref="TreeReady"/> rather than a wall-clock delay keeps the sweep
		/// off slow startups (heavy layout, debugger attached) and ensures the user has
		/// genuinely seen the tree before the thread pool fills with sibling-assembly loads.
		/// </summary>
		void ScheduleBackgroundLoadSweep(AssemblyList list)
		{
			_ = Task.Run(async () => {
				try
				{
					await TreeReady.ConfigureAwait(false);
					AppEnv.AppLog.Mark("Background-load sweep starting");
					// Cap concurrent loads so a 200-assembly list doesn't kick off 200
					// simultaneous Task.Run + GetLoadResultAsync chains. Each load reads PE
					// headers + metadata tables; mostly IO-bound but not zero CPU/memory.
					// Throttling to 4 keeps the peak allocation rate predictable so Server GC
					// has fewer reasons to pause the UI thread if the user clicks back into
					// the tree mid-sweep.
					using var throttle = new SemaphoreSlim(4);
					var loadTasks = new List<Task>();
					foreach (var assembly in list.GetAssemblies())
					{
						await throttle.WaitAsync().ConfigureAwait(false);
						loadTasks.Add(Task.Run(async () => {
							try
							{ await assembly.GetLoadResultAsync().ConfigureAwait(false); }
							finally
							{ throttle.Release(); }
						}));
					}
					await Task.WhenAll(loadTasks).ConfigureAwait(false);
					AppEnv.AppLog.Mark("Background-load sweep dispatched");
					// Load errors only surface once a load completes (no list change fires for them),
					// so re-evaluate now -- this is what enables "Remove assemblies with load errors".
					Commands.CommandManager.InvalidateRequerySuggested();
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"[AssemblyTreeModel] background load sweep failed: {ex}");
				}
			});
		}

		/// <summary>
		/// Walks down from <see cref="Root"/> matching each path segment against
		/// <see cref="object.ToString"/>, expanding lazy children along the way.
		/// </summary>
		public SharpTreeNode? FindNodeByPath(string[]? path, bool returnBestMatch)
			=> TreeNodeLocator.FindNodeByPath(Root, path, returnBestMatch);

		/// <summary>
		/// The path of <paramref name="node"/>'s ancestors (root excluded), in root-first order.
		/// </summary>
		public static string[]? GetPathForNode(SharpTreeNode? node)
			=> TreeNodeLocator.GetPathForNode(node);

		/// <summary>
		/// File path of the auto-loaded assembly under <paramref name="node"/>'s ancestor
		/// chain, or null when the selection lives in an explicitly-listed assembly. Mirrors
		/// the WPF host's <c>GetAutoLoadedAssemblyNode</c> contract.
		/// </summary>
		static string? GetAutoLoadedAssemblyPath(SharpTreeNode? node)
		{
			while (node != null && node is not TreeNodes.AssemblyTreeNode)
				node = node.Parent;
			if (node is not TreeNodes.AssemblyTreeNode asmNode)
				return null;
			var loaded = asmNode.LoadedAssembly;
			if (!loaded.IsLoaded || !loaded.IsAutoLoaded)
				return null;
			return loaded.FileName;
		}

		internal AssemblyTreeNode? FindAssemblyNode(LoadedAssembly asm)
			=> assemblyListTreeNode?.FindAssemblyNode(asm);

		/// <summary>
		/// Finds the tree node corresponding to <paramref name="reference"/> — used by
		/// hyperlink clicks in the decompiler view to route to the right entity. Currently
		/// only covers the reference kinds the tree knows how to model.
		/// </summary>
		public ILSpyTreeNode? FindTreeNode(object? reference)
			=> TreeNodeLocator.FindTreeNode(assemblyListTreeNode, AssemblyList, reference);

		static void LoadInitialAssemblies(AssemblyList assemblyList, AssemblyListManager? manager)
		{
			// Headless tests opt out of the full-framework seed (it would re-open ~150 assemblies
			// per test); they fall back to the minimal trio the previous version shipped so their
			// expectations and runtime stay unchanged.
			if (!App.SeedFullFrameworkDefaultList || manager == null)
			{
				System.Reflection.Assembly[] minimal = {
					typeof(object).Assembly,
					typeof(Uri).Assembly,
					typeof(System.Linq.Enumerable).Assembly,
				};
				foreach (var asm in minimal)
				{
					if (!string.IsNullOrEmpty(asm.Location))
						assemblyList.OpenAssembly(asm.Location);
				}
				return;
			}

			// First-run default: seed the .NET framework assemblies ILSpy itself is running on -
			// every managed assembly in the shared-framework directory that hosts the running
			// runtime (the folder containing System.Private.CoreLib). The manager applies the
			// same managed-PE filter the preconfigured runtime lists use, so native runtime
			// libraries (coreclr, clrjit, *_cor3.dll, ...) are skipped.
			var coreLibLocation = typeof(object).Assembly.Location;
			if (string.IsNullOrEmpty(coreLibLocation))
				return;
			var frameworkDirectory = System.IO.Path.GetDirectoryName(coreLibLocation);
			if (string.IsNullOrEmpty(frameworkDirectory))
				return;
			manager.AddFrameworkAssembliesFromDirectory(assemblyList, frameworkDirectory);
		}

		public void SelectNode(SharpTreeNode? node)
		{
			if (node == null)
				return;
			SelectedItem = node;
		}

		/// <summary>
		/// Resolves <paramref name="type"/> to the matching <see cref="TypeTreeNode"/> in the
		/// loaded assembly list and selects it. Returns false when the type's parent module is
		/// not loaded (or the namespace / nested-type chain can't be walked) — used by the
		/// Base/Derived Types entry nodes to jump along an inheritance chain.
		/// </summary>
		public bool JumpToType(ITypeDefinition? type)
		{
			if (type == null || assemblyListTreeNode == null)
				return false;
			var node = TreeNodeLocator.FindTypeNode(assemblyListTreeNode, type);
			if (node == null)
				return false;
			SelectNode(node);
			return true;
		}

		public void OpenFiles(string[] fileNames, bool focusNode = true)
		{
			ArgumentNullException.ThrowIfNull(fileNames);
			LoadAssemblies(fileNames, focusNode: focusNode);
		}

		/// <summary>
		/// Applies parsed startup arguments: switches the active language, loads any assemblies
		/// passed positionally, then navigates to the requested entity / namespace if any. Safe
		/// to call before assemblies finish loading — awaits each one's metadata before resolving
		/// the navigation target. Search-string handling is deferred until the search pane lands.
		/// </summary>
		public async Task HandleCommandLineArgumentsAsync(AppEnv.CommandLineArguments args)
		{
			ArgumentNullException.ThrowIfNull(args);
			if (args.Language is { Length: > 0 } languageName)
				languageService.CurrentLanguage = languageService.GetLanguage(languageName);

			var newlyLoaded = new List<LoadedAssembly>();
			if (args.AssembliesToLoad is { Count: > 0 })
				LoadAssemblies(args.AssembliesToLoad, newlyLoaded, focusNode: false);

			// "all currently-loaded entries that the navigation target may live in" — newly
			// loaded ones first (matches WPF's "command-line files take precedence") plus the
			// existing list as fallback.
			var relevant = newlyLoaded.Count > 0
				? new List<LoadedAssembly>(newlyLoaded)
				: AssemblyList?.GetAssemblies().ToList() ?? new List<LoadedAssembly>();

			if (args.NavigateTo is { Length: > 0 } navigateTo)
				await NavigateOnLaunchAsync(navigateTo, relevant);
			else if (newlyLoaded.Count == 1 && FindAssemblyNode(newlyLoaded[0]) is { } singleNode)
				SelectNode(singleNode);

			// Search-pane wiring lands with task 6. Until then the arg parses but is a no-op
			// rather than crashing.
		}

		async Task NavigateOnLaunchAsync(string navigateTo, IList<LoadedAssembly> relevant)
		{
			// "none" is a sentinel used by the WPF VS add-in to suppress initial navigation —
			// the real target arrives later via IPC.
			if (navigateTo == "none")
				return;

			if (navigateTo.StartsWith("N:", StringComparison.Ordinal))
			{
				var namespaceName = navigateTo.Substring(2);
				foreach (var asm in relevant)
				{
					var assemblyNode = FindAssemblyNode(asm);
					if (assemblyNode == null)
						continue;
					await asm.GetMetadataFileAsync().ConfigureAwait(true);
					var nsNode = assemblyNode.FindNamespaceNode(namespaceName);
					if (nsNode != null)
					{
						SelectNode(nsNode);
						return;
					}
				}
				return;
			}

			foreach (var asm in relevant)
				await asm.GetMetadataFileAsync().ConfigureAwait(true);

			var entity = await Task.Run(() => FindEntityInRelevantAssemblies(navigateTo, relevant));
			if (entity != null)
			{
				var node = FindTreeNode(entity);
				if (node != null)
					SelectNode(node);
			}
		}

		internal static IEntity? FindEntityInRelevantAssemblies(string navigateTo, IEnumerable<LoadedAssembly> relevantAssemblies)
		{
			ITypeReference typeRef;
			IMemberReference? memberRef = null;
			if (navigateTo.StartsWith("T:", StringComparison.Ordinal))
			{
				typeRef = IdStringProvider.ParseTypeName(navigateTo);
			}
			else
			{
				memberRef = IdStringProvider.ParseMemberIdString(navigateTo);
				typeRef = memberRef.DeclaringTypeReference;
			}
			foreach (var asm in relevantAssemblies)
			{
				var module = asm.GetMetadataFileOrNull();
				if (module != null && CanResolveTypeInPEFile(module, typeRef, out var typeHandle))
				{
					ICompilation compilation = typeHandle.Kind == HandleKind.ExportedType
						? new DecompilerTypeSystem(module, module.GetAssemblyResolver())
						: new SimpleCompilation((PEFile)module, MinimalCorlib.Instance);
					return memberRef == null
						? typeRef.Resolve(new SimpleTypeResolveContext(compilation)) as ITypeDefinition
						: memberRef.Resolve(new SimpleTypeResolveContext(compilation));
				}
			}
			return null;
		}

		static bool CanResolveTypeInPEFile(MetadataFile module, ITypeReference typeRef, out EntityHandle typeHandle)
		{
			// Reference assemblies are skipped so the loop keeps looking for an actual definition.
			if (module.IsReferenceAssembly())
			{
				typeHandle = default;
				return false;
			}

			switch (typeRef)
			{
				case GetPotentiallyNestedClassTypeReference topLevelType:
					typeHandle = topLevelType.ResolveInPEFile(module);
					return !typeHandle.IsNil;
				case NestedTypeReference nestedType:
					if (!CanResolveTypeInPEFile(module, nestedType.DeclaringTypeReference, out typeHandle))
						return false;
					if (typeHandle.Kind == HandleKind.ExportedType)
						return true;
					var typeDef = module.Metadata.GetTypeDefinition((TypeDefinitionHandle)typeHandle);
					typeHandle = typeDef.GetNestedTypes().FirstOrDefault(t => {
						var td = module.Metadata.GetTypeDefinition(t);
						var typeName = ReflectionHelper.SplitTypeParameterCountFromReflectionName(module.Metadata.GetString(td.Name), out int typeParameterCount);
						return nestedType.AdditionalTypeParameterCount == typeParameterCount && nestedType.Name == typeName;
					});
					return !typeHandle.IsNil;
				default:
					typeHandle = default;
					return false;
			}
		}

		void LoadAssemblies(IEnumerable<string> fileNames, List<LoadedAssembly>? loadedAssemblies = null, bool focusNode = true)
		{
			if (AssemblyList == null)
				return;

			AssemblyTreeNode? lastNode = null;
			foreach (var file in fileNames)
			{
				var assembly = AssemblyList.OpenAssembly(file);
				if (loadedAssemblies != null)
				{
					loadedAssemblies.Add(assembly);
					continue;
				}
				var node = assemblyListTreeNode?.FindAssemblyNode(assembly);
				if (node != null && focusNode)
					lastNode = node;
			}

			if (focusNode && lastNode != null)
				SelectNode(lastNode);
		}

		public void SortAssemblyList()
		{
			if (AssemblyList == null)
				return;

			// Sorting rebuilds every top-level assembly node, which drops the selection and snaps
			// the list back to the top -- the user sees the tree visibly reshuffle. Capture the
			// selected assemblies first and re-select them afterwards so the view settles on the
			// same items (the selection binder reveals one of them) instead of jumping to the top.
			var selectedAssemblies = SelectedItems
				.Select(AssemblyOf)
				.Where(a => a != null)
				.Distinct()
				.ToList();

			AssemblyList.Sort(AssemblyComparer.Instance);

			if (selectedAssemblies.Count == 0 || assemblyListTreeNode == null)
				return;
			var nodes = selectedAssemblies
				.Select(a => assemblyListTreeNode.FindAssemblyNode(a!))
				.Where(n => n != null)
				.Cast<SharpTreeNode>()
				.ToList();
			if (nodes.Count > 0)
				SelectNodes(nodes);
		}

		// Maps a selected node to the assembly it belongs to: the node itself when an assembly is
		// selected, otherwise the assembly ancestor of a selected member/namespace/type.
		static LoadedAssembly? AssemblyOf(SharpTreeNode node)
			=> (node as AssemblyTreeNode ?? node.Ancestors().OfType<AssemblyTreeNode>().FirstOrDefault())?.LoadedAssembly;

		sealed class AssemblyComparer : IComparer<LoadedAssembly>
		{
			public static readonly AssemblyComparer Instance = new();
			public int Compare(LoadedAssembly? x, LoadedAssembly? y)
				=> string.Compare(x?.ShortName, y?.ShortName, StringComparison.CurrentCulture);
		}

		public void CollapseAll() => CollapseChildren(Root);

		static void CollapseChildren(SharpTreeNode? node)
		{
			if (node is null)
				return;
			foreach (var child in node.Children)
			{
				if (!child.IsExpanded)
					continue;
				CollapseChildren(child);
				child.IsExpanded = false;
			}
		}

		/// <summary>
		/// Fan-out for changes to the currently-active assembly list (assemblies added or
		/// removed). Re-publishes via <see cref="Util.MessageBus"/> so panes that don't
		/// directly hold a reference to <see cref="AssemblyList"/> can react — the search
		/// pane restarts, the dock workspace prunes orphaned tabs. Mirrors WPF's
		/// <c>assemblyList_CollectionChanged</c> shape.
		/// </summary>
		void OnActiveAssemblyListCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			// Prune navigation-history entries that pointed at tree nodes inside removed
			// assemblies BEFORE re-publishing — Back/Forward consumers (the toolbar
			// commands + dropdowns) re-evaluate their CanExecute when the bus fires, so
			// they must see the post-prune state. Mirrors WPF's history.RemoveAll(...)
			// inside assemblyList_CollectionChanged.
			if (e.OldItems is { Count: > 0 } oldItems)
			{
				var removed = new HashSet<LoadedAssembly>(oldItems.OfType<LoadedAssembly>());
				if (removed.Count > 0)
				{
					AppEnv.AppComposition.TryGetExport<Docking.DockWorkspace>()?.PruneHistory(node =>
						node.AncestorsAndSelf()
							.OfType<AssemblyTreeNode>()
							.Any(a => removed.Contains(a.LoadedAssembly)));
				}
			}
			Util.MessageBus.Send(this, new Util.CurrentAssemblyListChangedEventArgs(e));

			// List-dependent menu commands (Clear assembly list, Remove assemblies with load errors)
			// re-evaluate CanExecute now that the list gained or lost entries.
			Commands.CommandManager.InvalidateRequerySuggested();
		}

		// Coalesces burst F5 / programmatic Refresh() calls into a single async pipeline.
		// Without the gate, two Refresh() in quick succession would run two parallel
		// ShowAssemblyList + GetMetadataFileAsync cycles, doubling the work and producing
		// visible flicker. The gate is a simple "running flag" — a queued refresh becomes
		// a no-op while the previous one is still in flight.
		bool refreshInFlight;

		public void Refresh()
		{
			if (refreshInFlight)
				return;
			_ = RunRefresh();

			async Task RunRefresh()
			{
				refreshInFlight = true;
				try
				{ await RefreshInternalAsync(); }
				finally { refreshInFlight = false; }
			}
		}

		/// <summary>
		/// Re-runs decompilation of the active tab WITHOUT reloading the assembly list. Mirrors
		/// WPF's RefreshDecompiledView(). Unlike <see cref="Refresh"/> (F5), this must not rebuild
		/// the list from persisted state -- that would discard on-demand auto-loaded assemblies
		/// (e.g. the ones <see cref="LoadDependenciesAsync"/> just resolved).
		/// </summary>
		public void RefreshDecompiledView()
			=> AppEnv.AppComposition.TryGetExport<Docking.DockWorkspace>()?.ForceRefreshActiveTab();

		/// <summary>
		/// Re-decompiles the decompiler tab's content in place for an output-affecting display setting,
		/// without activating or navigating to it. Changing an option must not switch the user's current
		/// tab, so this avoids the selection re-projection that <see cref="RefreshDecompiledView"/> does.
		/// </summary>
		public void RefreshDecompiledViewInPlace()
			=> AppEnv.AppComposition.TryGetExport<Docking.DockWorkspace>()?.RefreshDecompilerOutputInPlace();

		/// <summary>
		/// Resolves every assembly reference of each supplied assembly node through that
		/// assembly's own resolver -- which auto-loads the targets into the live list -- then
		/// re-decompiles the active tab so newly available references render. Mirrors WPF's
		/// LoadDependencies command.
		/// </summary>
		public async Task LoadDependenciesAsync(IReadOnlyList<SharpTreeNode> nodes)
		{
			var tasks = new List<Task>();
			foreach (var node in nodes)
			{
				if (node is not AssemblyTreeNode { LoadedAssembly: { } la })
					continue;
				var resolver = la.GetAssemblyResolver();
				var module = la.GetMetadataFileOrNull();
				if (module is null)
					continue;
				foreach (var assyRef in module.Metadata.AssemblyReferences)
					tasks.Add(resolver.ResolveAsync(
						new ICSharpCode.Decompiler.Metadata.AssemblyReference(module, assyRef)));
			}
			await Task.WhenAll(tasks);
			RefreshDecompiledView();
		}

		async Task RefreshInternalAsync()
		{
			if (AssemblyList == null || listManager == null)
				return;
			var path = GetPathForNode(SelectedItem);
			ShowAssemblyList(listManager.LoadList(AssemblyList.ListName));

			// Ensure the assembly's children are realised before FindNodeByPath walks them.
			// Lazy-loaded resource children (e.g. .baml entries inside an embedded
			// .resources file) only materialise after the assembly's metadata-file is
			// loaded; without this await the path-walk runs against an empty resource
			// folder and the selection collapses to the resources folder itself (#3705 in
			// the WPF tree). If the user navigated to a different node while we waited,
			// honour that new selection rather than overwriting it with the pre-refresh path.
			if (path is { Length: > 0 })
			{
				var rootAssembly = AssemblyList.FindAssembly(path[0]);
				if (rootAssembly != null)
				{
					var preAwaitSelection = SelectedItem;
					try
					{ await rootAssembly.GetMetadataFileAsync().ConfigureAwait(true); }
					catch { /* corrupt assembly — let FindNodeByPath best-match below */ }
					if (!ReferenceEquals(SelectedItem, preAwaitSelection))
						return;
				}
			}

			SelectNode(FindNodeByPath(path, returnBestMatch: true));

			// Defensive re-decompile: F5 on the same assembly list doesn't rebuild the
			// tree, so FindNodeByPath returns the same tree-node reference, the
			// SelectedItem setter early-outs, and DockWorkspace.ShowSelectedNode's
			// dedup short-circuits — leaving stale decompiled text. Force a fresh
			// render. Mirrors WPF's RefreshDecompiledView() call.
			AppEnv.AppComposition.TryGetExport<Docking.DockWorkspace>()?.ForceRefreshActiveTab();
		}
	}
}
