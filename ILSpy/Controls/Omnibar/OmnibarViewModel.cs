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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Search;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Controls.Omnibar
{
	/// <summary>
	/// Drives one omnibar instance: the breadcrumb trail for the hosting document's current node
	/// and the inline search that the bar turns into when the user types. Created per-control (not
	/// MEF-shared) so each document tab carries its own trail; shared services (assembly tree,
	/// language, settings, search engine) are resolved lazily from the composition host the same
	/// way <see cref="SearchPaneModel"/> does.
	/// </summary>
	public sealed partial class OmnibarViewModel : ViewModelBase
	{
		// The inline dropdown shows the best few hits; the docked Search pane (Ctrl+Shift+F) stays
		// the place for the full, sortable result list.
		const int MaxSuggestions = 20;

		/// <summary>The breadcrumb trail, root-first (Assembly > Namespace > Type > Member).</summary>
		public ObservableCollection<BreadcrumbSegment> Segments { get; } = new();

		/// <summary>The best <see cref="MaxSuggestions"/> hits of the live search, fitness-sorted.</summary>
		public ObservableCollection<SearchResult> Suggestions { get; } = new();

		// Full streaming sink for the current RunningSearch (kept fitness-sorted by the engine).
		// Suggestions mirrors only its head; this stays private so the bound collection never shows
		// the engine's 1000-result cap row.
		readonly ObservableCollection<SearchResult> searchSink = new();

		[ObservableProperty]
		public partial OmnibarMode Mode { get; set; } = OmnibarMode.Breadcrumb;

		[ObservableProperty]
		public partial string SearchText { get; set; } = string.Empty;

		[ObservableProperty]
		public partial bool IsSearching { get; set; }

		/// <summary>True while the bar shows the breadcrumb trail. Drives view visibility.</summary>
		public bool IsBreadcrumbMode => Mode == OmnibarMode.Breadcrumb;

		/// <summary>True while the bar shows the search input. Drives view visibility.</summary>
		public bool IsSearchMode => Mode == OmnibarMode.Search;

		public OmnibarViewModel()
		{
			PropertyChanged += OnPropertyChangedDispatch;
			searchSink.CollectionChanged += OnSearchSinkChanged;
		}

		partial void OnModeChanged(OmnibarMode value)
		{
			OnPropertyChanged(nameof(IsBreadcrumbMode));
			OnPropertyChanged(nameof(IsSearchMode));
		}

		void OnPropertyChangedDispatch(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(SearchText))
				return;
			// Typing anything turns the bar into a search box; clearing it (or Escape) is what
			// returns to the breadcrumb, handled by ExitSearch.
			if (!string.IsNullOrEmpty(SearchText))
				Mode = OmnibarMode.Search;
			RestartSearch();
		}

		// The node currently shown in the host document, tracked so a repeated SetNode (e.g. an
		// intermediate decompile property change) doesn't rebuild the trail or kick the user out
		// of a search they're in the middle of typing.
		SharpTreeNode? currentNode;

		/// <summary>
		/// Points the breadcrumb at <paramref name="node"/> (the document's current node). A real
		/// change rebuilds the trail and leaves search mode, since the displayed location moved.
		/// </summary>
		public void SetNode(SharpTreeNode? node)
		{
			if (ReferenceEquals(currentNode, node))
				return;
			currentNode = node;
			Segments.Clear();
			foreach (var segment in BuildSegments(node))
				Segments.Add(segment);
			if (Mode == OmnibarMode.Search)
				ExitSearch();
		}

		/// <summary>
		/// The breadcrumb for <paramref name="node"/>: its ancestor chain root-first, excluding the
		/// invisible <c>AssemblyListTreeNode</c> root (whose <see cref="SharpTreeNode.Parent"/> is
		/// null). Mirrors <see cref="AssemblyTreeModel.GetPathForNode"/>'s root exclusion.
		/// </summary>
		public static IReadOnlyList<BreadcrumbSegment> BuildSegments(SharpTreeNode? node)
		{
			var segments = new List<BreadcrumbSegment>();
			for (var current = node; current is { Parent: not null }; current = current.Parent)
				segments.Add(new BreadcrumbSegment(current));
			segments.Reverse();
			return segments;
		}

		/// <summary>
		/// The child nodes of <paramref name="segment"/> wrapped as crumbs, for the chevron
		/// dropdown's sibling list. Materialises the node's lazy children on demand.
		/// </summary>
		public IReadOnlyList<BreadcrumbSegment> GetChildSegments(BreadcrumbSegment? segment)
		{
			if (segment?.Node == null)
				return System.Array.Empty<BreadcrumbSegment>();
			segment.Node.EnsureLazyChildren();
			return segment.Node.Children.Select(child => new BreadcrumbSegment(child)).ToArray();
		}

		/// <summary>Moves the assembly-tree selection to <paramref name="segment"/>'s node.</summary>
		[RelayCommand]
		public void NavigateSegment(BreadcrumbSegment? segment)
		{
			if (segment?.Node == null)
				return;
			AppComposition.TryGetExport<AssemblyTreeModel>()?.SelectNode(segment.Node);
		}

		/// <summary>Switches the bar into search mode (e.g. Ctrl+L / clicking the bar).</summary>
		[RelayCommand]
		public void EnterSearch() => Mode = OmnibarMode.Search;

		/// <summary>Returns to the breadcrumb and clears the query.</summary>
		[RelayCommand]
		public void ExitSearch()
		{
			Mode = OmnibarMode.Breadcrumb;
			SearchText = string.Empty;
		}

		/// <summary>
		/// Navigates to a search suggestion: resolves its reference to a tree node and moves the
		/// selection there (or opens a new document tab when <paramref name="inNewTabPage"/> is
		/// set). Mirrors <see cref="SearchPaneModel.Activate(SearchResult, bool)"/>.
		/// </summary>
		public void Activate(SearchResult? result, bool inNewTabPage = false)
		{
			if (result?.Reference == null)
				return;
			var atm = AppComposition.TryGetExport<AssemblyTreeModel>();
			if (atm == null)
				return;
			var node = atm.FindTreeNode(result.Reference);
			if (node == null)
				return;
			if (inNewTabPage)
				AppComposition.TryGetExport<Docking.DockWorkspace>()?.OpenNodeInNewTab(node);
			else
				atm.SelectedItem = node;
		}

		RunningSearch? currentSearch;

		void RestartSearch()
		{
			currentSearch?.Cancel();
			currentSearch = null;
			searchSink.Clear();
			Suggestions.Clear();
			IsSearching = false;

			var term = SearchText ?? string.Empty;
			if (string.IsNullOrWhiteSpace(term))
				return;

			var assemblyList = AppComposition.TryGetExport<AssemblyTreeModel>()?.AssemblyList;
			if (assemblyList == null)
				return;
			var language = AppComposition.TryGetExport<LanguageService>()?.CurrentLanguage;
			if (language == null)
				return;
			var apiVisibility = AppComposition.TryGetExport<SettingsService>()?.SessionSettings?.LanguageSettings?.ShowApiLevel
				?? ApiVisibility.PublicOnly;

			var run = new RunningSearch(
				assemblyList.GetAssemblies(),
				term,
				SearchMode.TypeAndMember,
				language,
				apiVisibility,
				new AvaloniaSearchResultFactory(language),
				searchSink,
				SearchResult.ComparerByFitness);
			run.Completed += OnRunCompleted;
			currentSearch = run;
			IsSearching = true;
			run.Start();
		}

		void OnRunCompleted(RunningSearch sender)
		{
			if (!ReferenceEquals(sender, currentSearch))
				return;
			IsSearching = false;
		}

		// Mirror only the head of the fitness-sorted sink into the bound Suggestions, updating at
		// the edges so the dropdown changes in place rather than rebuilding on every streamed hit.
		void OnSearchSinkChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add
					when e.NewItems?.Count == 1 && e.NewStartingIndex < MaxSuggestions:
					Suggestions.Insert(e.NewStartingIndex, (SearchResult)e.NewItems[0]!);
					while (Suggestions.Count > MaxSuggestions)
						Suggestions.RemoveAt(Suggestions.Count - 1);
					break;
				case NotifyCollectionChangedAction.Reset:
					Suggestions.Clear();
					break;
			}
		}
	}
}
