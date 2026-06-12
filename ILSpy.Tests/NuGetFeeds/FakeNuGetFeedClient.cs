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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.NuGetFeeds;

namespace ICSharpCode.ILSpy.Tests.NuGetFeeds;

/// <summary>
/// Scriptable in-memory <see cref="INuGetFeedClient"/>: records every call, returns
/// configurable results, throws configurable exceptions, and can hold a download open
/// on a <see cref="TaskCompletionSource"/> so cancellation paths become testable.
/// </summary>
public sealed class FakeNuGetFeedClient : INuGetFeedClient
{
	public sealed record SearchCall(string FeedUrl, string SearchText, bool IncludePrerelease, int Skip, int Take);
	public sealed record VersionsCall(string FeedUrl, string PackageId, bool IncludePrerelease, int MaxCount);
	public sealed record DownloadCall(string FeedUrl, string PackageId, string Version);

	public List<SearchCall> SearchCalls { get; } = new();
	public List<VersionsCall> VersionsCalls { get; } = new();
	public List<DownloadCall> DownloadCalls { get; } = new();

	/// <summary>Computes search results per call; defaults to one well-known package.</summary>
	public Func<SearchCall, IReadOnlyList<NuGetPackageInfo>> SearchHandler { get; set; }
		= call => new[] { MakePackage("Newtonsoft.Json") };

	public Exception? SearchException { get; set; }

	public IReadOnlyList<string> VersionsToReturn { get; set; } = new[] { "13.0.3", "13.0.2", "12.0.1" };

	public Exception? VersionsException { get; set; }

	public string DownloadResultPath { get; set; }
		= @"C:\nuget-cache\newtonsoft.json\13.0.3\newtonsoft.json.13.0.3.nupkg";

	public Exception? DownloadException { get; set; }

	/// <summary>
	/// When set, downloads do not complete until this source is resolved (or the call's
	/// cancellation token fires), letting tests cancel a download mid-flight.
	/// </summary>
	public TaskCompletionSource<string>? PendingDownload { get; set; }

	public static NuGetPackageInfo MakePackage(string id, string version = "1.0.0") => new(
		Id: id,
		LatestVersion: version,
		Description: $"Description of {id}",
		Authors: "Test Author",
		IconUrl: null,
		LicenseUrl: "https://licenses.example/MIT",
		ProjectUrl: $"https://github.example/{id}",
		Tags: "test fake",
		DownloadCount: 42,
		Published: new DateTimeOffset(2024, 1, 2, 0, 0, 0, TimeSpan.Zero));

	public static IReadOnlyList<NuGetPackageInfo> MakePage(int count, int offset = 0)
		=> Enumerable.Range(offset, count).Select(i => MakePackage($"Package{i:D3}")).ToArray();

	public Task<IReadOnlyList<NuGetPackageInfo>> SearchAsync(
		string feedUrl, string searchText, bool includePrerelease,
		int skip, int take, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		var call = new SearchCall(feedUrl, searchText, includePrerelease, skip, take);
		SearchCalls.Add(call);
		if (SearchException != null)
			return Task.FromException<IReadOnlyList<NuGetPackageInfo>>(SearchException);
		return Task.FromResult(SearchHandler(call));
	}

	public Task<IReadOnlyList<string>> GetVersionsAsync(
		string feedUrl, string packageId, bool includePrerelease,
		int maxCount, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		VersionsCalls.Add(new VersionsCall(feedUrl, packageId, includePrerelease, maxCount));
		if (VersionsException != null)
			return Task.FromException<IReadOnlyList<string>>(VersionsException);
		return Task.FromResult(VersionsToReturn);
	}

	public async Task<string> DownloadToGlobalPackagesFolderAsync(
		string feedUrl, string packageId, string version,
		CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		DownloadCalls.Add(new DownloadCall(feedUrl, packageId, version));
		if (DownloadException != null)
			throw DownloadException;
		if (PendingDownload != null)
			return await PendingDownload.Task.WaitAsync(cancellationToken);
		return DownloadResultPath;
	}
}
