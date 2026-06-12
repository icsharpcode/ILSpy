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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Composition;
using System.Linq;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Search
{
	/// <summary>
	/// One entry in the search-mode picker. The <see cref="Mode"/> determines which
	/// <c>ICSharpCode.ILSpyX.Search</c> strategy runs when the user types a query;
	/// <see cref="Name"/> is the label rendered in the ComboBox and <see cref="Image"/>
	/// is the icon shown alongside it (both in the dropdown and in the closed selector).
	/// </summary>
	public sealed class SearchModeEntry
	{
		public required SearchMode Mode { get; init; }
		public required string Name { get; init; }
		public required IImage Image { get; init; }
	}

	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Top, Order = 0, IsVisibleByDefault = false)]
	[Shared]
	public partial class SearchPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "Search";

		public SearchPaneModel()
		{
			Id = PaneContentId;
			Title = "Search";
			SelectedSearchMode = ResolvePersistedMode() ?? SearchModes[0];
			PropertyChanged += OnPropertyChangedDispatch;
			// Refresh search results when the active assembly list mutates. Skip the
			// restart when ONLY auto-loaded (dependency) assemblies are added — those
			// fire from navigating through results in a large assembly and would cause
			// a tight feedback loop / flicker. Mirrors WPF's #3734 fix.
			Util.MessageBus<Util.CurrentAssemblyListChangedEventArgs>.Subscribers += OnAssemblyListChanged;
		}

		void OnAssemblyListChanged(object? sender, Util.CurrentAssemblyListChangedEventArgs e)
		{
			var inner = e.Inner;
			if (inner.Action == NotifyCollectionChangedAction.Add
				&& inner.NewItems?.Cast<LoadedAssembly>().All(asm => asm.IsAutoLoaded) == true)
			{
				return;
			}
			if (string.IsNullOrEmpty(SearchTerm))
				return;
			RestartSearch();
		}

		void OnPropertyChangedDispatch(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(SearchTerm) or nameof(SelectedSearchMode))
				RestartSearch();
			if (e.PropertyName == nameof(SelectedSearchMode))
				PersistSelectedMode();
		}

		SearchModeEntry? ResolvePersistedMode()
		{
			// Lookup is best-effort: during the very first composition pass SettingsService may
			// not have materialised yet; the default entry from SearchModes[0] is fine in that
			// case and gets overwritten the first time the user picks a mode.
			var saved = AppComposition.TryGetExport<SettingsService>()?.SessionSettings?.SelectedSearchMode;
			if (string.IsNullOrEmpty(saved))
				return null;
			if (!Enum.TryParse(saved, ignoreCase: false, out SearchMode mode))
				return null;
			return SearchModes.FirstOrDefault(m => m.Mode == mode);
		}

		void PersistSelectedMode()
		{
			var sessionSettings = AppComposition.TryGetExport<SettingsService>()?.SessionSettings;
			if (sessionSettings == null)
				return;
			sessionSettings.SelectedSearchMode = SelectedSearchMode?.Mode.ToString();
		}

		/// <summary>
		/// All twelve modes from <see cref="ICSharpCode.ILSpyX.Search.SearchMode"/>, in the
		/// order the WPF pane uses (most inclusive first). Display names mirror the WPF
		/// strings so users moving between builds see the same labels.
		/// </summary>
		public SearchModeEntry[] SearchModes { get; } = new[] {
			new SearchModeEntry { Mode = SearchMode.TypeAndMember, Name = "Types and Members", Image = Images.Library },
			new SearchModeEntry { Mode = SearchMode.Type, Name = "Type", Image = Images.Class },
			new SearchModeEntry { Mode = SearchMode.Member, Name = "Member", Image = Images.Property },
			new SearchModeEntry { Mode = SearchMode.Method, Name = "Method", Image = Images.Method },
			new SearchModeEntry { Mode = SearchMode.Field, Name = "Field", Image = Images.Field },
			new SearchModeEntry { Mode = SearchMode.Property, Name = "Property", Image = Images.Property },
			new SearchModeEntry { Mode = SearchMode.Event, Name = "Event", Image = Images.Event },
			new SearchModeEntry { Mode = SearchMode.Literal, Name = "Constant", Image = Images.Literal },
			new SearchModeEntry { Mode = SearchMode.Token, Name = "Metadata Token", Image = Images.Library },
			new SearchModeEntry { Mode = SearchMode.Resource, Name = "Resource", Image = Images.Resource },
			new SearchModeEntry { Mode = SearchMode.Assembly, Name = "Assembly", Image = Images.Assembly },
			new SearchModeEntry { Mode = SearchMode.Namespace, Name = "Namespace", Image = Images.Namespace },
		};

		/// <summary>
		/// Current query string. Bound two-way to the TextBox in the pane's view. The
		/// setter raises <see cref="ObservableObject.PropertyChanged"/> so the background
		/// streaming orchestrator and the filter cascade through
		/// <c>LanguageSettings.SearchTerm</c> react on every keystroke.
		/// </summary>
		[ObservableProperty]
		public partial string SearchTerm { get; set; } = string.Empty;

		/// <summary>
		/// Active mode in the picker. Defaults to <see cref="SearchMode.TypeAndMember"/>.
		/// </summary>
		[ObservableProperty]
		public partial SearchModeEntry SelectedSearchMode { get; set; }

		/// <summary>
		/// Streaming sink for results from the current search run. The view's ListBox binds
		/// here directly; the background orchestrator inserts rows via
		/// <c>Dispatcher.UIThread.Post</c> as the strategies emit them.
		/// </summary>
		public ObservableCollection<SearchResult> Results { get; } = new();

		/// <summary>
		/// True while the background search is in flight. Bound to the pane's
		/// <c>ProgressBar.IsIndeterminate</c> so the user sees activity for long-running
		/// scans (large assembly lists can take a few seconds). Flips to true at
		/// <see cref="RunningSearch.Start"/> and back to false when
		/// <see cref="RunningSearch.Completed"/> fires.
		/// </summary>
		[ObservableProperty]
		public partial bool IsSearching { get; set; }

		/// <summary>
		/// Raised when the pane should hand keyboard focus to its search input. The view's
		/// code-behind subscribes and calls <c>SearchInput.Focus()</c> on the dispatcher;
		/// the VM stays UI-framework-agnostic. Fires from <see cref="RequestFocus"/>, which
		/// <see cref="Docking.DockWorkspace.ShowSearchCommand"/> calls after bringing the
		/// pane to active.
		/// </summary>
		public event Action? FocusRequested;

		public void RequestFocus() => FocusRequested?.Invoke();

		/// <summary>
		/// User clicked (or double-tapped) a result row. Walks the result's <c>Reference</c>
		/// to the matching assembly-tree node via <see cref="AssemblyTreeModel.FindTreeNode"/>
		/// and moves selection there. Silently no-ops when the reference can't be resolved
		/// (resource and assembly results have non-entity references that the assembly tree
		/// doesn't index — a follow-up commit can extend FindTreeNode to cover them).
		/// </summary>
		public void Activate(SearchResult result) => Activate(result, inNewTabPage: false);

		/// <summary>
		/// Navigates to <paramref name="result"/>. When <paramref name="inNewTabPage"/> is
		/// false the assembly-tree selection moves to the matching node (reusing the active
		/// document tab); when true the node is opened in a fresh document tab via
		/// <see cref="Docking.DockWorkspace.OpenNodeInNewTab"/> -- wired to Ctrl+Enter and a
		/// middle-click on a result row.
		/// </summary>
		public void Activate(SearchResult result, bool inNewTabPage)
		{
			ArgumentNullException.ThrowIfNull(result);
			if (result.Reference is null)
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

		/// <summary>
		/// Selects the picker entry carrying <paramref name="mode"/>, if any. Backs the
		/// Ctrl+T / Ctrl+M / Ctrl+S accelerators in the pane's view; a no-op when no entry
		/// exposes the requested mode.
		/// </summary>
		public void SelectMode(SearchMode mode)
		{
			var entry = SearchModes.FirstOrDefault(m => m.Mode == mode);
			if (entry != null)
				SelectedSearchMode = entry;
		}

		RunningSearch? currentSearch;

		void RestartSearch()
		{
			currentSearch?.Cancel();
			currentSearch = null;
			Results.Clear();
			IsSearching = false;

			var term = SearchTerm ?? string.Empty;
			if (string.IsNullOrWhiteSpace(term))
				return;

			var assemblyTreeModel = AppComposition.TryGetExport<AssemblyTreeModel>();
			var assemblyList = assemblyTreeModel?.AssemblyList;
			if (assemblyList == null)
				return;
			var language = AppComposition.TryGetExport<LanguageService>()?.CurrentLanguage;
			if (language == null)
				return;
			var apiVisibility = AppComposition.TryGetExport<SettingsService>()?.SessionSettings?.LanguageSettings?.ShowApiLevel
				?? ApiVisibility.PublicOnly;

			var factory = new AvaloniaSearchResultFactory(language);
			// Capture the comparer at start-of-search (matches WPF SearchPane.xaml.cs:288).
			// Toggling "Sort results by fitness" mid-run won't reshuffle results already on
			// screen — it only takes effect on the next search.
			var sortComparer = (AppComposition.TryGetExport<SettingsService>()?.DisplaySettings.SortResults ?? true)
				? SearchResult.ComparerByFitness
				: SearchResult.ComparerByName;
			var run = new RunningSearch(
				assemblyList.GetAssemblies(),
				term,
				SelectedSearchMode.Mode,
				language,
				apiVisibility,
				factory,
				Results,
				sortComparer);
			run.Completed += OnRunCompleted;
			currentSearch = run;
			IsSearching = true;
			run.Start();
		}

		void OnRunCompleted(RunningSearch sender)
		{
			// Ignore late completions from cancelled runs — those are noise.
			if (!ReferenceEquals(sender, currentSearch))
				return;
			IsSearching = false;
		}

	}
}
