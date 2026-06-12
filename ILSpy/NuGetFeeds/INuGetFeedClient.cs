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
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.NuGetFeeds
{
	/// <summary>
	/// One search hit from a NuGet feed, reduced to what the package-chooser dialog
	/// displays. Deliberately free of NuGet.* types so view models and tests don't
	/// take a dependency on the NuGet client libraries.
	/// </summary>
	public sealed record NuGetPackageInfo(
		string Id,
		string LatestVersion,
		string? Description,
		string? Authors,
		string? IconUrl,
		string? LicenseUrl,
		string? ProjectUrl,
		string? Tags,
		long? DownloadCount,
		DateTimeOffset? Published);

	/// <summary>
	/// Feed operations the "Open from NuGet feed" dialog needs. The production
	/// implementation (<see cref="NuGetFeedClient"/>) talks to NuGet V3 feeds;
	/// tests substitute a fake so no network is involved.
	/// </summary>
	public interface INuGetFeedClient
	{
		/// <summary>
		/// Searches <paramref name="feedUrl"/> for packages matching <paramref name="searchText"/>
		/// (an empty string yields the feed's default listing, like the NuGet gallery front page).
		/// </summary>
		Task<IReadOnlyList<NuGetPackageInfo>> SearchAsync(
			string feedUrl, string searchText, bool includePrerelease,
			int skip, int take, CancellationToken cancellationToken);

		/// <summary>
		/// Returns at most <paramref name="maxCount"/> versions of <paramref name="packageId"/>,
		/// newest first, excluding prerelease versions unless <paramref name="includePrerelease"/>.
		/// </summary>
		Task<IReadOnlyList<string>> GetVersionsAsync(
			string feedUrl, string packageId, bool includePrerelease,
			int maxCount, CancellationToken cancellationToken);

		/// <summary>
		/// Downloads the package into the NuGet global packages folder (reusing an already
		/// cached copy) and returns the absolute path of the stored .nupkg file.
		/// </summary>
		Task<string> DownloadToGlobalPackagesFolderAsync(
			string feedUrl, string packageId, string version,
			CancellationToken cancellationToken);
	}
}
