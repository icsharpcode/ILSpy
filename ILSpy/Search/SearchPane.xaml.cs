// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Composition;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.Extensions;
using ICSharpCode.ILSpyX.Search;

using TomsToolbox.Essentials;
using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

namespace ICSharpCode.ILSpy.Search
{
	/// <summary>
	/// Search pane
	/// </summary>
	[DataTemplate(typeof(SearchPaneModel))]
	[NonShared]
	public partial class SearchPane
	{
		const int MAX_RESULTS = 1000;
		const int MAX_REFRESH_TIME_MS = 10; // More means quicker forward of data, fewer means better responsibility
		RunningSearch currentSearch;
		bool runSearchOnNextShow;
		IComparer<SearchResult> resultsComparer;
		readonly AssemblyTreeModel assemblyTreeModel;
		readonly ITreeNodeFactory treeNodeFactory;
		readonly SettingsService settingsService;

		public ObservableCollection<SearchResult> Results { get; } = [];

		string SearchTerm => searchBox.Text;

		public SearchPane(AssemblyTreeModel assemblyTreeModel, ITreeNodeFactory treeNodeFactory, SettingsService settingsService)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.treeNodeFactory = treeNodeFactory;
			this.settingsService = settingsService;

			InitializeComponent();

			ContextMenuProvider.Add(listBox);
			MessageBus<CurrentAssemblyListChangedEventArgs>.Subscribers += (sender, e) => CurrentAssemblyList_Changed();
			MessageBus<SettingsChangedEventArgs>.Subscribers += (sender, e) => Settings_PropertyChanged(sender, e);

			CompositionTarget.Rendering += UpdateResults;
		}

		void CurrentAssemblyList_Changed()
		{
			if (IsVisible)
			{
				StartSearch(this.SearchTerm);
			}
			else
			{
				StartSearch(null);
				runSearchOnNextShow = true;
			}
		}

		void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (sender is not LanguageSettings)
				return;

			UpdateFilter();
		}

		void UpdateFilter()
		{
			if (IsVisible)
			{
				StartSearch(this.SearchTerm);
			}
			else
			{
				StartSearch(null);
				runSearchOnNextShow = true;
			}
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == IsVisibleProperty)
			{
				if (e.NewValue as bool? != true)
					return;

				if (!runSearchOnNextShow)
					return;

				runSearchOnNextShow = false;
				StartSearch(this.SearchTerm);
			}
			else if (e.Property == Pane.IsActiveProperty)
			{
				if (e.NewValue as bool? != true)
					return;

				if (IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed && !SearchTerm.IsNullOrEmpty())
					return;

				FocusSearchBox();
			}
		}

		void FocusSearchBox()
		{
			this.BeginInvoke(DispatcherPriority.Background, () => {
				searchBox.Focus();
				searchBox.SelectAll();
			});
		}

		void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			StartSearch(searchBox.Text);
		}

		void SearchModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			StartSearch(this.SearchTerm);
		}

		void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			JumpToSelectedItem();
			e.Handled = true;
		}

		void ListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				e.Handled = true;
				JumpToSelectedItem();
			}
			else if (e.Key == Key.Up && listBox.SelectedIndex == 0)
			{
				e.Handled = true;
				listBox.SelectedIndex = -1;
				searchBox.Focus();
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.KeyboardDevice.Modifiers != ModifierKeys.Control)
				return;

			switch (e.Key)
			{
				case Key.T:
					searchModeComboBox.SelectedValue = SearchMode.Type;
					e.Handled = true;
					break;
				case Key.M:
					searchModeComboBox.SelectedValue = SearchMode.Member;
					e.Handled = true;
					break;
				case Key.S:
					searchModeComboBox.SelectedValue = SearchMode.Literal;
					e.Handled = true;
					break;
			}
		}

		void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Down || !listBox.HasItems)
				return;

			e.Handled = true;
			listBox.MoveFocus(new(FocusNavigationDirection.First));
			listBox.SelectedIndex = 0;
		}

		void UpdateResults(object sender, EventArgs e)
		{
			if (currentSearch == null)
				return;

			var timer = Stopwatch.StartNew();
			int resultsAdded = 0;
			while (Results.Count < MAX_RESULTS && timer.ElapsedMilliseconds < MAX_REFRESH_TIME_MS && currentSearch.ResultQueue.TryTake(out var result))
			{
				Results.InsertSorted(result, resultsComparer);
				++resultsAdded;
			}

			if (resultsAdded <= 0 || Results.Count != MAX_RESULTS)
				return;

			Results.Add(new() {
				Name = Properties.Resources.SearchAbortedMoreThan1000ResultsFound,
				Location = null!,
				Assembly = null!,
				Image = null!,
				LocationImage = null!,
				AssemblyImage = null!,
			});

			currentSearch.Cancel();
		}

		async void StartSearch(string searchTerm)
		{
			if (currentSearch != null)
			{
				currentSearch.Cancel();
				currentSearch = null;
			}

			resultsComparer = settingsService.DisplaySettings.SortResults ?
				SearchResult.ComparerByFitness :
				SearchResult.ComparerByName;
			Results.Clear();

			RunningSearch startedSearch = null;
			if (!string.IsNullOrEmpty(searchTerm))
			{

				searchProgressBar.IsIndeterminate = true;
				startedSearch = new(await assemblyTreeModel.AssemblyList.GetAllAssemblies(),
					searchTerm,
					(SearchMode)searchModeComboBox.SelectedIndex,
					assemblyTreeModel.CurrentLanguage,
					treeNodeFactory,
					settingsService);
				currentSearch = startedSearch;

				await startedSearch.Run();
			}

			if (currentSearch == startedSearch)
			{
				//are we still running the same search
				searchProgressBar.IsIndeterminate = false;
			}
		}

		void JumpToSelectedItem()
		{
			if (listBox.SelectedItem is SearchResult result)
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(result.Reference));
			}
		}

		sealed class RunningSearch
		{
			readonly CancellationTokenSource cts = new();
			readonly IList<LoadedAssembly> assemblies;
			readonly SearchRequest searchRequest;
			readonly SearchMode searchMode;
			readonly Language language;
			readonly ApiVisibility apiVisibility;
			readonly ITreeNodeFactory treeNodeFactory;
			readonly SettingsService settingsService;

			public IProducerConsumerCollection<SearchResult> ResultQueue { get; } = new ConcurrentQueue<SearchResult>();

			public RunningSearch(IList<LoadedAssembly> assemblies, string searchTerm, SearchMode searchMode,
				Language language, ITreeNodeFactory treeNodeFactory, SettingsService settingsService)
			{
				this.assemblies = assemblies;
				this.language = language;
				this.searchMode = searchMode;
				this.apiVisibility = settingsService.SessionSettings.LanguageSettings.ShowApiLevel;
				this.treeNodeFactory = treeNodeFactory;
				this.settingsService = settingsService;
				this.searchRequest = Parse(searchTerm);
			}

			SearchRequest Parse(string input)
			{
				string[] parts = CommandLineTools.CommandLineToArgumentArray(input);

				SearchRequest request = new();
				List<string> keywords = new();
				Regex regex = null;
				request.Mode = searchMode;

				foreach (string part in parts)
				{
					// Parse: [prefix:|@]["]searchTerm["]
					// Find quotes used for escaping
					int prefixLength = part.IndexOfAny(new[] { '"', '/' });
					if (prefixLength < 0)
					{
						// no quotes
						prefixLength = part.Length;
					}

					int delimiterLength;
					// Find end of prefix
					if (part.StartsWith("@", StringComparison.Ordinal))
					{
						prefixLength = 1;
						delimiterLength = 0;
					}
					else
					{
						prefixLength = part.IndexOf(':', 0, prefixLength);
						delimiterLength = 1;
					}
					string prefix;
					if (prefixLength <= 0)
					{
						prefix = null;
						prefixLength = -1;
					}
					else
					{
						prefix = part.Substring(0, prefixLength);
					}

					// unescape quotes
					string searchTerm = part.Substring(prefixLength + delimiterLength).Trim();
					if (searchTerm.Length > 0)
					{
						searchTerm = CommandLineTools.CommandLineToArgumentArray(searchTerm)[0];
					}
					else
					{
						// if searchTerm is only "@" or "prefix:",
						// then we do not interpret it as prefix, but as searchTerm.
						searchTerm = part;
						prefix = null;
					}

					if (prefix == null || prefix.Length <= 2)
					{
						if (regex == null && searchTerm.StartsWith("/", StringComparison.Ordinal) && searchTerm.Length > 1)
						{
							int searchTermLength = searchTerm.Length - 1;
							if (searchTerm.EndsWith("/", StringComparison.Ordinal))
							{
								searchTermLength--;
							}

							request.FullNameSearch |= searchTerm.Contains("\\.");

							regex = CreateRegex(searchTerm.Substring(1, searchTermLength));
						}
						else
						{
							request.FullNameSearch |= searchTerm.Contains(".");
							keywords.Add(searchTerm);
						}
						request.OmitGenerics |= !(searchTerm.Contains("<") || searchTerm.Contains("`"));
					}

					switch (prefix?.ToUpperInvariant())
					{
						case "@":
							request.Mode = SearchMode.Token;
							break;
						case "INNAMESPACE":
							request.InNamespace ??= searchTerm;
							break;
						case "INASSEMBLY":
							request.InAssembly ??= searchTerm;
							break;
						case "A":
							request.AssemblySearchKind = AssemblySearchKind.NameOrFileName;
							request.Mode = SearchMode.Assembly;
							break;
						case "AF":
							request.AssemblySearchKind = AssemblySearchKind.FilePath;
							request.Mode = SearchMode.Assembly;
							break;
						case "AN":
							request.AssemblySearchKind = AssemblySearchKind.FullName;
							request.Mode = SearchMode.Assembly;
							break;
						case "N":
							request.Mode = SearchMode.Namespace;
							break;
						case "TM":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.All;
							break;
						case "T":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Type;
							break;
						case "M":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Member;
							break;
						case "MD":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Method;
							break;
						case "F":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Field;
							break;
						case "P":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Property;
							break;
						case "E":
							request.Mode = SearchMode.Member;
							request.MemberSearchKind = MemberSearchKind.Event;
							break;
						case "C":
							request.Mode = SearchMode.Literal;
							break;
						case "R":
							request.Mode = SearchMode.Resource;
							break;
					}
				}

				Regex CreateRegex(string s)
				{
					try
					{
						return new(s, RegexOptions.Compiled);
					}
					catch (ArgumentException)
					{
						return null;
					}
				}

				request.Keywords = keywords.ToArray();
				request.RegEx = regex;
				request.SearchResultFactory = new SearchResultFactory(language);
				request.TreeNodeFactory = this.treeNodeFactory;
				request.DecompilerSettings = settingsService.DecompilerSettings;

				return request;
			}

			public void Cancel()
			{
				cts.Cancel();
			}

			public async Task Run()
			{
				try
				{
					await Task.Factory.StartNew(() => {
						var searcher = GetSearchStrategy(searchRequest);
						if (searcher == null)
							return;
						try
						{
							foreach (var loadedAssembly in assemblies)
							{
								var module = loadedAssembly.GetMetadataFileOrNull();
								if (module == null)
									continue;
								searcher.Search(module, cts.Token);
							}
						}
						catch (OperationCanceledException)
						{
							// ignore cancellation
						}

					}, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					// ignore cancellation
				}
			}

			AbstractSearchStrategy GetSearchStrategy(SearchRequest request)
			{
				if (request.Keywords.Length == 0 && request.RegEx == null)
					return null;

				switch (request.Mode)
				{
					case SearchMode.TypeAndMember:
						return new MemberSearchStrategy(language, apiVisibility, request, ResultQueue);
					case SearchMode.Type:
						return new MemberSearchStrategy(language, apiVisibility, request, ResultQueue, MemberSearchKind.Type);
					case SearchMode.Member:
						return new MemberSearchStrategy(language, apiVisibility, request, ResultQueue, request.MemberSearchKind);
					case SearchMode.Literal:
						return new LiteralSearchStrategy(language, apiVisibility, request, ResultQueue);
					case SearchMode.Method:
						return new MemberSearchStrategy(language, apiVisibility, request, ResultQueue, MemberSearchKind.Method);
					case SearchMode.Field:
						return new MemberSearchStrategy(language, apiVisibility, request, ResultQueue, MemberSearchKind.Field);
					case SearchMode.Property:
						return new MemberSearchStrategy(language, apiVisibility, request, ResultQueue, MemberSearchKind.Property);
					case SearchMode.Event:
						return new MemberSearchStrategy(language, apiVisibility, request, ResultQueue, MemberSearchKind.Event);
					case SearchMode.Token:
						return new MetadataTokenSearchStrategy(language, apiVisibility, request, ResultQueue);
					case SearchMode.Resource:
						return new ResourceSearchStrategy(apiVisibility, request, ResultQueue);
					case SearchMode.Assembly:
						return new AssemblySearchStrategy(request, ResultQueue, AssemblySearchKind.NameOrFileName);
					case SearchMode.Namespace:
						return new NamespaceSearchStrategy(request, ResultQueue);
				}

				return null;
			}
		}
	}

	[ExportToolbarCommand(ToolTip = nameof(Properties.Resources.SearchCtrlShiftFOrCtrlE), ToolbarIcon = "Images/Search", ToolbarCategory = nameof(Properties.Resources.View), ToolbarOrder = 100)]
	[Shared]
	sealed class ShowSearchCommand : CommandWrapper
	{
		private readonly DockWorkspace dockWorkspace;

		public ShowSearchCommand(DockWorkspace dockWorkspace)
			: base(NavigationCommands.Search)
		{
			this.dockWorkspace = dockWorkspace;
			var gestures = NavigationCommands.Search.InputGestures;

			gestures.Clear();
			gestures.Add(new KeyGesture(Key.F, ModifierKeys.Control | ModifierKeys.Shift));
			gestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
		}

		protected override void OnExecute(object sender, ExecutedRoutedEventArgs e)
		{
			dockWorkspace.ShowToolPane(SearchPaneModel.PaneContentId);
		}
	}
}