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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpy.NuGetFeeds;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Drives the "Open from NuGet feed" dialog: debounced search against the selected
	/// V3 feed, skip/take paging, a version list for the selected package, and a
	/// cancellable download into the global packages folder. All feed I/O goes through
	/// <see cref="INuGetFeedClient"/>; feed errors land in <see cref="ErrorMessage"/>.
	/// The dialog closes itself when <see cref="CloseRequested"/> fires with the path
	/// of the downloaded .nupkg.
	/// </summary>
	public sealed partial class OpenFromNuGetFeedDialogViewModel : ViewModelBase
	{
		public const int PageSize = 25;
		public const int MaxVersions = 100;

		readonly INuGetFeedClient client;
		readonly NuGetFeedSettings settings;
		readonly TimeSpan debounceDelay;

		CancellationTokenSource? searchCts;
		// Distinguishes the most recent search from superseded ones: a stale search must
		// neither publish its results nor flip IsSearching off under the active one.
		int searchGeneration;

		CancellationTokenSource? versionsCts;
		int versionsGeneration;

		CancellationTokenSource? downloadCts;

		[ObservableProperty]
		string searchText = string.Empty;

		[ObservableProperty]
		bool includePrerelease;

		[ObservableProperty]
		string selectedFeedUrl;

		[ObservableProperty]
		NuGetPackageInfo? selectedPackage;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(OpenPackageCommand))]
		string? selectedVersion;

		[ObservableProperty]
		bool isSearching;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(OpenPackageCommand))]
		bool isDownloading;

		[ObservableProperty]
		bool canLoadMore;

		[ObservableProperty]
		string? errorMessage;

		[ObservableProperty]
		string? downloadedPackagePath;

		public ObservableCollection<string> FeedUrls { get; }
		public ObservableCollection<NuGetPackageInfo> Packages { get; } = new();
		public ObservableCollection<string> Versions { get; } = new();

		/// <summary>
		/// Raised with the path of the downloaded .nupkg once it is ready to be opened;
		/// the view responds by closing the dialog with that path as its result.
		/// </summary>
		public event Action<string?>? CloseRequested;

		public OpenFromNuGetFeedDialogViewModel(
			INuGetFeedClient client, NuGetFeedSettings settings, TimeSpan? debounceDelay = null)
		{
			this.client = client;
			this.settings = settings;
			this.debounceDelay = debounceDelay ?? TimeSpan.FromMilliseconds(300);
			FeedUrls = new ObservableCollection<string>(settings.Feeds);
			selectedFeedUrl = settings.SelectedFeed;
			includePrerelease = settings.IncludePrerelease;
		}

		partial void OnSearchTextChanged(string value) => StartSearch(debounce: true);

		partial void OnIncludePrereleaseChanged(bool value)
		{
			settings.IncludePrerelease = value;
			StartSearch(debounce: true);
		}

		partial void OnSelectedFeedUrlChanged(string value) => StartSearch(debounce: true);

		partial void OnSelectedPackageChanged(NuGetPackageInfo? value)
			=> _ = LoadVersionsAsync(value);

		partial void OnSelectedVersionChanged(string? value)
		{
			// A ComboBox resets its SelectedItem to null when its items change under it;
			// never sit on an empty selection while versions are available - the latest
			// (first) version is always the default.
			if (value == null && Versions.Count > 0)
				SelectedVersion = Versions[0];
		}

		[RelayCommand]
		void Refresh() => StartSearch(debounce: false);

		[RelayCommand]
		void ClearSearch()
		{
			if (SearchText.Length == 0)
				StartSearch(debounce: false);
			else
				SearchText = string.Empty;
		}

		[RelayCommand]
		void LoadMore() => StartSearch(debounce: false, append: true);

		bool CanOpenPackage => SelectedVersion != null && !IsDownloading;

		[RelayCommand(CanExecute = nameof(CanOpenPackage))]
		async Task OpenPackage()
		{
			var package = SelectedPackage;
			var version = SelectedVersion;
			if (package == null || version == null)
				return;

			downloadCts?.Cancel();
			var cts = downloadCts = new CancellationTokenSource();
			IsDownloading = true;
			try
			{
				string path = await client.DownloadToGlobalPackagesFolderAsync(
					SelectedFeedUrl?.Trim() ?? string.Empty, package.Id, version, cts.Token);
				ErrorMessage = null;
				DownloadedPackagePath = path;
				CloseRequested?.Invoke(path);
			}
			catch (OperationCanceledException)
			{
				// User hit Cancel (or closed the dialog); not an error, no close.
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
			finally
			{
				IsDownloading = false;
			}
		}

		[RelayCommand]
		void CancelDownload() => downloadCts?.Cancel();

		/// <summary>Cancels every in-flight feed operation; called when the dialog closes.</summary>
		public void CancelAllOperations()
		{
			searchCts?.Cancel();
			versionsCts?.Cancel();
			downloadCts?.Cancel();
		}

		async Task LoadVersionsAsync(NuGetPackageInfo? package)
		{
			versionsCts?.Cancel();
			var cts = versionsCts = new CancellationTokenSource();
			int generation = ++versionsGeneration;
			Versions.Clear();
			SelectedVersion = null;
			if (package == null)
				return;
			try
			{
				var versions = await client.GetVersionsAsync(
					SelectedFeedUrl?.Trim() ?? string.Empty, package.Id, IncludePrerelease,
					MaxVersions, cts.Token);
				if (generation != versionsGeneration)
					return;
				foreach (var version in versions)
					Versions.Add(version);
				SelectedVersion = Versions.FirstOrDefault();
			}
			catch (OperationCanceledException)
			{
				// Selection moved on or the dialog closed.
			}
			catch (Exception ex)
			{
				if (generation == versionsGeneration)
					ErrorMessage = ex.Message;
			}
		}

		void StartSearch(bool debounce, bool append = false)
			=> _ = RunSearchAsync(debounce, append);

		async Task RunSearchAsync(bool debounce, bool append)
		{
			searchCts?.Cancel();
			var cts = searchCts = new CancellationTokenSource();
			int generation = ++searchGeneration;
			try
			{
				if (debounce && debounceDelay > TimeSpan.Zero)
					await Task.Delay(debounceDelay, cts.Token);

				IsSearching = true;
				string feed = SelectedFeedUrl?.Trim() ?? string.Empty;
				int skip = append ? Packages.Count : 0;
				var results = await client.SearchAsync(
					feed, SearchText, IncludePrerelease, skip, PageSize, cts.Token);
				if (generation != searchGeneration)
					return;

				ErrorMessage = null;
				if (!append)
				{
					Packages.Clear();
					SelectedPackage = null;
				}
				foreach (var package in results)
					Packages.Add(package);
				CanLoadMore = results.Count == PageSize;

				// Only a feed that actually answered earns a spot in the persisted MRU.
				settings.RememberFeed(feed);
				settings.SelectedFeed = settings.Feeds.FirstOrDefault(
					f => string.Equals(f, feed, StringComparison.OrdinalIgnoreCase)) ?? feed;
				if (!FeedUrls.Contains(settings.SelectedFeed, StringComparer.OrdinalIgnoreCase))
					FeedUrls.Add(settings.SelectedFeed);
			}
			catch (OperationCanceledException)
			{
				// Superseded by a newer search or the dialog closed.
			}
			catch (Exception ex)
			{
				if (generation == searchGeneration)
				{
					ErrorMessage = ex.Message;
					Packages.Clear();
					SelectedPackage = null;
					CanLoadMore = false;
				}
			}
			finally
			{
				if (generation == searchGeneration)
					IsSearching = false;
			}
		}
	}
}
