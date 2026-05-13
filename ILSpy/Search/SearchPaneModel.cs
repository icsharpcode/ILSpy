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

using System.Collections.ObjectModel;
using System.Composition;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Search;

using ILSpy.AppEnv;
using ILSpy.AssemblyTree;
using ILSpy.Commands;
using ILSpy.Languages;
using ILSpy.ViewModels;

namespace ILSpy.Search
{
	/// <summary>
	/// One entry in the search-mode picker. The <see cref="Mode"/> determines which
	/// <c>ICSharpCode.ILSpyX.Search</c> strategy runs when the user types a query;
	/// <see cref="Name"/> is the label rendered in the ComboBox.
	/// </summary>
	public sealed class SearchModeEntry
	{
		public required SearchMode Mode { get; init; }
		public required string Name { get; init; }
	}

	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Top, Order = 0)]
	[Shared]
	public partial class SearchPaneModel : ToolPaneModel
	{
		public const string PaneContentId = "Search";

		public SearchPaneModel()
		{
			Id = PaneContentId;
			Title = "Search";
			SelectedSearchMode = SearchModes[0];
			PropertyChanged += OnPropertyChangedDispatch;
		}

		void OnPropertyChangedDispatch(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(SearchTerm) or nameof(SelectedSearchMode))
				RestartSearch();
		}

		/// <summary>
		/// All twelve modes from <see cref="ICSharpCode.ILSpyX.Search.SearchMode"/>, in the
		/// order the WPF pane uses (most inclusive first). Display names mirror the WPF
		/// strings so users moving between builds see the same labels.
		/// </summary>
		public SearchModeEntry[] SearchModes { get; } = new[] {
			new SearchModeEntry { Mode = SearchMode.TypeAndMember, Name = "Types and Members" },
			new SearchModeEntry { Mode = SearchMode.Type, Name = "Type" },
			new SearchModeEntry { Mode = SearchMode.Member, Name = "Member" },
			new SearchModeEntry { Mode = SearchMode.Method, Name = "Method" },
			new SearchModeEntry { Mode = SearchMode.Field, Name = "Field" },
			new SearchModeEntry { Mode = SearchMode.Property, Name = "Property" },
			new SearchModeEntry { Mode = SearchMode.Event, Name = "Event" },
			new SearchModeEntry { Mode = SearchMode.Literal, Name = "Constant" },
			new SearchModeEntry { Mode = SearchMode.Token, Name = "Metadata Token" },
			new SearchModeEntry { Mode = SearchMode.Resource, Name = "Resource" },
			new SearchModeEntry { Mode = SearchMode.Assembly, Name = "Assembly" },
			new SearchModeEntry { Mode = SearchMode.Namespace, Name = "Namespace" },
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

		RunningSearch? currentSearch;

		void RestartSearch()
		{
			currentSearch?.Cancel();
			currentSearch = null;
			Results.Clear();

			var term = SearchTerm ?? string.Empty;

			// Push the term into LanguageSettings so the assembly-tree filter cascade
			// (every Filter override calls SearchTermMatches) hides non-matching rows
			// while a search is active. Clearing the term restores the full tree.
			var settings = TryGetSettings()?.SessionSettings?.LanguageSettings;
			if (settings != null)
				settings.SearchTerm = term;

			if (string.IsNullOrWhiteSpace(term))
				return;

			var assemblyTreeModel = TryGetAssemblyTreeModel();
			var assemblyList = assemblyTreeModel?.AssemblyList;
			if (assemblyList == null)
				return;
			var language = TryGetLanguage();
			if (language == null)
				return;
			var apiVisibility = TryGetSettings()?.SessionSettings?.LanguageSettings?.ShowApiLevel
				?? ApiVisibility.PublicOnly;

			var factory = new AvaloniaSearchResultFactory(language);
			currentSearch = new RunningSearch(
				assemblyList.GetAssemblies(),
				term,
				SelectedSearchMode.Mode,
				language,
				apiVisibility,
				factory,
				Results);
			currentSearch.Start();
		}

		static AssemblyTreeModel? TryGetAssemblyTreeModel()
		{
			try
			{
				return AppComposition.Current.GetExport<AssemblyTreeModel>();
			}
			catch
			{
				return null;
			}
		}

		static Language? TryGetLanguage()
		{
			try
			{
				return AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;
			}
			catch
			{
				return null;
			}
		}

		static SettingsService? TryGetSettings()
		{
			try
			{
				return AppComposition.Current.GetExport<SettingsService>();
			}
			catch
			{
				return null;
			}
		}
	}
}
