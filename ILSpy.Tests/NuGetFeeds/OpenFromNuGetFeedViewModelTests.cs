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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.NuGetFeeds;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.NuGetFeeds;

/// <summary>
/// Behavior of the "Open from NuGet feed" dialog's view model against a fake feed client:
/// search runs against the persisted feed with the persisted prerelease flag, typing is
/// debounced into a single query, paging appends via skip/take, and feed errors land in
/// <c>ErrorMessage</c> instead of escaping. The fake records every call, so the assertions
/// pin the exact requests the real NuGet client would receive.
/// </summary>
[TestFixture]
public class OpenFromNuGetFeedViewModelTests
{
	const string CustomFeed = "https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json";

	static (OpenFromNuGetFeedDialogViewModel vm, FakeNuGetFeedClient client, NuGetFeedSettings settings)
		CreateViewModel(TimeSpan? debounceDelay = null, NuGetFeedSettings? settings = null,
			INuGetIconLoader? iconLoader = null)
	{
		var client = new FakeNuGetFeedClient();
		settings ??= new NuGetFeedSettings();
		var vm = new OpenFromNuGetFeedDialogViewModel(
			client, settings, debounceDelay ?? TimeSpan.Zero, iconLoader);
		return (vm, client, settings);
	}

	// The list/selection now carry per-row view models; tests that drive a selection directly
	// wrap the feed DTO the same way a real search result is wrapped.
	static NuGetPackageViewModel Pkg(string id, string version = "1.0.0")
		=> new(FakeNuGetFeedClient.MakePackage(id, version));

	[AvaloniaTest]
	public async Task Refresh_Searches_The_Persisted_Feed_With_Empty_Term_And_Persisted_Prerelease()
	{
		var settings = new NuGetFeedSettings { IncludePrerelease = true };
		settings.RememberFeed(CustomFeed);
		settings.SelectedFeed = CustomFeed;
		var (vm, client, _) = CreateViewModel(settings: settings);

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);

		client.SearchCalls.Should().ContainSingle()
			.Which.Should().Be(new FakeNuGetFeedClient.SearchCall(
				CustomFeed, "", true, 0, OpenFromNuGetFeedDialogViewModel.PageSize),
				"the initial listing must use the persisted feed, prerelease flag, an empty term, and the first page");
		vm.IsSearching.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Typing_Is_Debounced_Into_A_Single_Search_With_The_Final_Term()
	{
		var (vm, client, _) = CreateViewModel(debounceDelay: TimeSpan.FromMilliseconds(75));

		vm.SearchText = "n";
		vm.SearchText = "ne";
		vm.SearchText = "newtonsoft";
		await Waiters.WaitForAsync(() => client.SearchCalls.Count > 0);
		// Allow a full extra debounce window to elapse so stale timers would have fired.
		await Task.Delay(150);

		client.SearchCalls.Should().ContainSingle("intermediate keystrokes must be coalesced")
			.Which.SearchText.Should().Be("newtonsoft");
	}

	[AvaloniaTest]
	public async Task Toggling_Prerelease_Reruns_The_Search_And_Persists_The_Flag()
	{
		var (vm, client, settings) = CreateViewModel();

		vm.IncludePrerelease = true;
		await Waiters.WaitForAsync(() => client.SearchCalls.Count > 0);

		client.SearchCalls.Last().IncludePrerelease.Should().BeTrue();
		settings.IncludePrerelease.Should().BeTrue("the toggle must survive an app restart");
	}

	[AvaloniaTest]
	public async Task Switching_To_A_New_Feed_Searches_It_And_Remembers_It_After_Success()
	{
		var (vm, client, settings) = CreateViewModel();

		vm.SelectedFeedUrl = CustomFeed;
		await Waiters.WaitForAsync(() => client.SearchCalls.Count > 0);
		await Waiters.WaitForAsync(() => vm.FeedUrls.Contains(CustomFeed));

		client.SearchCalls.Last().FeedUrl.Should().Be(CustomFeed);
		settings.Feeds.Should().Contain(CustomFeed,
			"a feed that produced results is worth offering again next time");
		settings.SelectedFeed.Should().Be(CustomFeed);
	}

	[AvaloniaTest]
	public async Task A_Failing_Feed_Does_Not_Get_Remembered()
	{
		var (vm, client, settings) = CreateViewModel();
		client.SearchException = new HttpRequestException("503 Service Unavailable");

		vm.SelectedFeedUrl = CustomFeed;
		await Waiters.WaitForAsync(() => vm.ErrorMessage != null);

		settings.Feeds.Should().NotContain(CustomFeed,
			"feeds are only added to the MRU once they answered a search");
	}

	[AvaloniaTest]
	public async Task A_Full_Page_Enables_LoadMore_Which_Appends_The_Next_Page()
	{
		var (vm, client, _) = CreateViewModel();
		client.SearchHandler = call => FakeNuGetFeedClient.MakePage(
			count: call.Skip == 0 ? OpenFromNuGetFeedDialogViewModel.PageSize : 3,
			offset: call.Skip);

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count == OpenFromNuGetFeedDialogViewModel.PageSize);
		vm.CanLoadMore.Should().BeTrue("a full first page suggests more results exist");

		vm.LoadMoreCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count == OpenFromNuGetFeedDialogViewModel.PageSize + 3);

		client.SearchCalls.Should().HaveCount(2);
		client.SearchCalls[1].Skip.Should().Be(OpenFromNuGetFeedDialogViewModel.PageSize,
			"the second page must start where the first ended");
		vm.CanLoadMore.Should().BeFalse("a short page means the listing is exhausted");
		vm.Packages.Select(p => p.Info.Id).Should().OnlyHaveUniqueItems();
	}

	[AvaloniaTest]
	public async Task ClearSearch_Resets_The_Term_And_Lists_The_Feed_Defaults_Again()
	{
		var (vm, client, _) = CreateViewModel();
		vm.SearchText = "newtonsoft";
		await Waiters.WaitForAsync(() => client.SearchCalls.Count == 1);

		vm.ClearSearchCommand.Execute(null);
		await Waiters.WaitForAsync(() => client.SearchCalls.Count >= 2);

		vm.SearchText.Should().BeEmpty();
		client.SearchCalls.Last().SearchText.Should().BeEmpty();
	}

	[AvaloniaTest]
	public async Task Selecting_A_Package_Loads_Its_Versions_And_Preselects_The_Newest()
	{
		var (vm, client, _) = CreateViewModel();
		client.VersionsToReturn = new[] { "13.0.3", "13.0.2", "12.0.1" };

		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => vm.Versions.Count == 3);

		vm.Versions.Should().Equal(new[] { "13.0.3", "13.0.2", "12.0.1" },
			"the dropdown lists versions in the order the client returns them (newest first)");
		vm.SelectedVersion.Should().Be("13.0.3", "the newest version is the most likely pick");
		client.VersionsCalls.Should().ContainSingle()
			.Which.Should().Be(new FakeNuGetFeedClient.VersionsCall(
				NuGetFeedSettings.DefaultFeed, "Newtonsoft.Json", false,
				OpenFromNuGetFeedDialogViewModel.MaxVersions));
	}

	[AvaloniaTest]
	public async Task Version_Load_Respects_The_Prerelease_Toggle()
	{
		var (vm, client, _) = CreateViewModel();
		vm.IncludePrerelease = true;
		await Waiters.WaitForAsync(() => client.SearchCalls.Count > 0);

		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => client.VersionsCalls.Count > 0);

		client.VersionsCalls.Last().IncludePrerelease.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Switching_Packages_Replaces_The_Version_List()
	{
		var (vm, client, _) = CreateViewModel();
		vm.SelectedPackage = Pkg("PackageA");
		await Waiters.WaitForAsync(() => vm.Versions.Count > 0);

		client.VersionsToReturn = new[] { "2.0.0" };
		vm.SelectedPackage = Pkg("PackageB");
		await Waiters.WaitForAsync(() => vm.Versions.Count == 1);

		vm.Versions.Should().Equal(new[] { "2.0.0" });
		vm.SelectedVersion.Should().Be("2.0.0");
		client.VersionsCalls.Last().PackageId.Should().Be("PackageB");
	}

	[AvaloniaTest]
	public async Task A_Cleared_Version_Selection_Snaps_Back_To_The_Latest_Version()
	{
		var (vm, client, _) = CreateViewModel();
		client.VersionsToReturn = new[] { "13.0.3", "13.0.2" };
		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => vm.SelectedVersion != null);

		// A ComboBox resets its SelectedItem to null when its items change under it;
		// the view model must answer by reselecting the latest version, never sitting
		// on an empty selection while versions are available.
		vm.SelectedVersion = null;

		vm.SelectedVersion.Should().Be("13.0.3");
	}

	[AvaloniaTest]
	public async Task Version_Load_Failure_Surfaces_In_ErrorMessage()
	{
		var (vm, client, _) = CreateViewModel();
		client.VersionsException = new HttpRequestException("flat container unavailable");

		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => vm.ErrorMessage != null);

		vm.ErrorMessage.Should().Contain("flat container unavailable");
		vm.Versions.Should().BeEmpty();
		vm.SelectedVersion.Should().BeNull();
	}

	[AvaloniaTest]
	public async Task Open_Is_Disabled_Until_A_Version_Is_Available()
	{
		var (vm, client, _) = CreateViewModel();
		vm.OpenPackageCommand.CanExecute(null).Should().BeFalse(
			"there is nothing to download before a package/version is selected");

		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => vm.SelectedVersion != null);

		vm.OpenPackageCommand.CanExecute(null).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Open_Downloads_The_Selected_Version_And_Requests_Close_With_The_Nupkg_Path()
	{
		var (vm, client, _) = CreateViewModel();
		string? closedWith = null;
		vm.CloseRequested += path => closedWith = path;

		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => vm.SelectedVersion != null);
		vm.SelectedVersion = "13.0.2";

		vm.OpenPackageCommand.Execute(null);
		await Waiters.WaitForAsync(() => closedWith != null);

		closedWith.Should().Be(client.DownloadResultPath,
			"the dialog result is the cached .nupkg the command then opens");
		vm.DownloadedPackagePath.Should().Be(client.DownloadResultPath);
		client.DownloadCalls.Should().ContainSingle()
			.Which.Should().Be(new FakeNuGetFeedClient.DownloadCall(
				NuGetFeedSettings.DefaultFeed, "Newtonsoft.Json", "13.0.2"));
		vm.IsDownloading.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Cancelling_A_Download_Leaves_The_Dialog_Open_Without_An_Error()
	{
		var (vm, client, _) = CreateViewModel();
		client.PendingDownload = new TaskCompletionSource<string>();
		bool closeRaised = false;
		vm.CloseRequested += _ => closeRaised = true;

		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => vm.SelectedVersion != null);
		vm.OpenPackageCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.IsDownloading);

		vm.OpenPackageCommand.CanExecute(null).Should().BeFalse(
			"a second download must not start while one is running");

		vm.CancelDownloadCommand.Execute(null);
		await Waiters.WaitForAsync(() => !vm.IsDownloading);

		closeRaised.Should().BeFalse("a cancelled download must keep the dialog open");
		vm.ErrorMessage.Should().BeNull("user-initiated cancellation is not an error");
		vm.OpenPackageCommand.CanExecute(null).Should().BeTrue("the user can retry");
	}

	[AvaloniaTest]
	public async Task A_Failed_Download_Surfaces_The_Error_And_Keeps_The_Dialog_Open()
	{
		var (vm, client, _) = CreateViewModel();
		client.DownloadException = new HttpRequestException("connection reset by peer");
		bool closeRaised = false;
		vm.CloseRequested += _ => closeRaised = true;

		vm.SelectedPackage = Pkg("Newtonsoft.Json");
		await Waiters.WaitForAsync(() => vm.SelectedVersion != null);
		vm.OpenPackageCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.ErrorMessage != null);

		vm.ErrorMessage.Should().Contain("connection reset by peer");
		closeRaised.Should().BeFalse();
		vm.IsDownloading.Should().BeFalse();
		vm.OpenPackageCommand.CanExecute(null).Should().BeTrue("the user can retry after a failure");
	}

	[AvaloniaTest]
	public async Task Feed_Errors_Surface_In_ErrorMessage_And_Clear_On_The_Next_Success()
	{
		var (vm, client, _) = CreateViewModel();
		client.SearchException = new HttpRequestException("name or service not known");

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.ErrorMessage != null);

		vm.ErrorMessage.Should().Contain("name or service not known",
			"the user must see why the feed produced nothing");
		vm.IsSearching.Should().BeFalse();
		vm.Packages.Should().BeEmpty();

		client.SearchException = null;
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);
		vm.ErrorMessage.Should().BeNull("a successful retry must dismiss the stale error");
	}

	[AvaloniaTest]
	public async Task A_Result_With_An_Icon_Url_Has_Its_Custom_Icon_Fetched_And_Swapped_In()
	{
		var iconLoader = new FakeNuGetIconLoader();
		var (vm, client, _) = CreateViewModel(iconLoader: iconLoader);
		client.SearchHandler = _ => new[] {
			FakeNuGetFeedClient.MakePackage("WithIcon", iconUrl: "https://example/icon.png")
		};

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);
		await Waiters.WaitForAsync(() => vm.Packages[0].Icon is FakeNuGetIconLoader.StubImage);

		((FakeNuGetIconLoader.StubImage)vm.Packages[0].Icon).Url.Should().Be("https://example/icon.png",
			"the row must show the package's own icon once it loads");
		iconLoader.RequestedUrls.Should().Contain("https://example/icon.png");
	}

	[AvaloniaTest]
	public async Task A_Result_Without_An_Icon_Url_Keeps_The_Default_Icon_And_Is_Never_Fetched()
	{
		var iconLoader = new FakeNuGetIconLoader();
		var (vm, client, _) = CreateViewModel(iconLoader: iconLoader);
		client.SearchHandler = _ => new[] { FakeNuGetFeedClient.MakePackage("NoIcon") };

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);
		// Give any erroneous load a chance to run before asserting it did not happen.
		await Task.Delay(50);

		vm.Packages[0].Icon.Should().BeSameAs(Images.NuGet, "a package with no icon URL stays on the default");
		iconLoader.RequestedUrls.Should().BeEmpty("there is nothing to fetch without an icon URL");
	}

	[AvaloniaTest]
	public async Task A_Broken_Icon_Url_Leaves_The_Default_Icon_In_Place()
	{
		var iconLoader = new FakeNuGetIconLoader { ReturnImage = false };
		var (vm, client, _) = CreateViewModel(iconLoader: iconLoader);
		client.SearchHandler = _ => new[] {
			FakeNuGetFeedClient.MakePackage("WithIcon", iconUrl: "https://example/icon.png")
		};

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);
		await Waiters.WaitForAsync(() => iconLoader.RequestedUrls.Contains("https://example/icon.png"));
		await Task.Delay(50);

		vm.Packages[0].Icon.Should().BeSameAs(Images.NuGet, "a failed fetch must not disturb the default icon");
	}

	[AvaloniaTest]
	public async Task A_New_Search_Cancels_The_Previous_Result_Sets_Pending_Icon_Loads()
	{
		var gate = new TaskCompletionSource();
		var iconLoader = new FakeNuGetIconLoader { Gate = gate };
		var (vm, client, _) = CreateViewModel(iconLoader: iconLoader);
		client.SearchHandler = _ => new[] {
			FakeNuGetFeedClient.MakePackage("Pkg", iconUrl: "https://example/icon.png")
		};

		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0);
		var firstRow = vm.Packages[0];
		// The first row's icon load is now in flight, parked on the gate.
		await Waiters.WaitForAsync(() => iconLoader.RequestedUrls.Count > 0);

		// A new search replaces the result set, which must cancel the first set's pending load.
		vm.RefreshCommand.Execute(null);
		await Waiters.WaitForAsync(() => vm.Packages.Count > 0 && !ReferenceEquals(vm.Packages[0], firstRow));

		gate.SetResult();
		await Task.Delay(50);

		firstRow.Icon.Should().BeSameAs(Images.NuGet,
			"a superseded result set must not paint icons onto its now-discarded rows");
	}
}
