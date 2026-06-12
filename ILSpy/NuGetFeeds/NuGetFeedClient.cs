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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Packaging.Signing;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace ICSharpCode.ILSpy.NuGetFeeds
{
	/// <summary>
	/// <see cref="INuGetFeedClient"/> backed by the NuGet client libraries, talking to
	/// V3 feeds. Downloads land in the user's NuGet global packages folder (honoring
	/// NuGet.Config / NUGET_PACKAGES overrides), so packages fetched here are shared
	/// with every other NuGet consumer on the machine and never downloaded twice.
	/// </summary>
	public sealed class NuGetFeedClient : INuGetFeedClient
	{
		static readonly ILogger Logger = NullLogger.Instance;

		readonly ConcurrentDictionary<string, SourceRepository> repositories =
			new(StringComparer.OrdinalIgnoreCase);
		readonly SourceCacheContext cache = new();

		SourceRepository GetRepository(string feedUrl)
			=> repositories.GetOrAdd(feedUrl, url => Repository.Factory.GetCoreV3(url));

		public async Task<IReadOnlyList<NuGetPackageInfo>> SearchAsync(
			string feedUrl, string searchText, bool includePrerelease,
			int skip, int take, CancellationToken cancellationToken)
		{
			var search = await GetRepository(feedUrl)
				.GetResourceAsync<PackageSearchResource>(cancellationToken).ConfigureAwait(false)
				?? throw new InvalidOperationException($"The feed '{feedUrl}' does not support searching.");
			var results = await search.SearchAsync(
				searchText, new SearchFilter(includePrerelease), skip, take,
				Logger, cancellationToken).ConfigureAwait(false);
			return results.Select(m => new NuGetPackageInfo(
				Id: m.Identity.Id,
				LatestVersion: m.Identity.Version.ToNormalizedString(),
				Description: m.Description,
				Authors: m.Authors,
				IconUrl: m.IconUrl?.ToString(),
				LicenseUrl: m.LicenseUrl?.ToString(),
				ProjectUrl: m.ProjectUrl?.ToString(),
				Tags: m.Tags,
				DownloadCount: m.DownloadCount,
				Published: m.Published)).ToArray();
		}

		public async Task<IReadOnlyList<string>> GetVersionsAsync(
			string feedUrl, string packageId, bool includePrerelease,
			int maxCount, CancellationToken cancellationToken)
		{
			var byId = await GetRepository(feedUrl)
				.GetResourceAsync<FindPackageByIdResource>(cancellationToken).ConfigureAwait(false);
			var versions = await byId.GetAllVersionsAsync(
				packageId, cache, Logger, cancellationToken).ConfigureAwait(false);
			return versions
				.Where(v => includePrerelease || !v.IsPrerelease)
				.OrderByDescending(v => v)
				.Take(maxCount)
				.Select(v => v.ToNormalizedString())
				.ToArray();
		}

		public async Task<string> DownloadToGlobalPackagesFolderAsync(
			string feedUrl, string packageId, string version,
			CancellationToken cancellationToken)
		{
			var identity = new PackageIdentity(packageId, NuGetVersion.Parse(version));
			var nugetSettings = Settings.LoadDefaultSettings(root: null);
			string packagesFolder = SettingsUtility.GetGlobalPackagesFolder(nugetSettings);
			string nupkgPath = new VersionFolderPathResolver(packagesFolder)
				.GetPackageFilePath(identity.Id, identity.Version);

			// Already installed (by ILSpy or any other NuGet consumer): reuse it.
			var cached = GlobalPackagesFolderUtility.GetPackage(identity, packagesFolder);
			if (cached != null)
			{
				cached.Dispose();
				if (File.Exists(nupkgPath))
					return nupkgPath;
			}

			var byId = await GetRepository(feedUrl)
				.GetResourceAsync<FindPackageByIdResource>(cancellationToken).ConfigureAwait(false);
			using var packageStream = new MemoryStream();
			bool found = await byId.CopyNupkgToStreamAsync(
				identity.Id, identity.Version, packageStream, cache,
				Logger, cancellationToken).ConfigureAwait(false);
			if (!found)
			{
				throw new InvalidOperationException(
					$"Package {packageId} {version} was not found on '{feedUrl}'.");
			}

			packageStream.Position = 0;
			var clientPolicy = ClientPolicyContext.GetClientPolicy(nugetSettings, Logger);
			var added = await GlobalPackagesFolderUtility.AddPackageAsync(
				feedUrl, identity, packageStream, packagesFolder, parentId: Guid.Empty,
				clientPolicy, Logger, cancellationToken).ConfigureAwait(false);
			added.Dispose();
			return nupkgPath;
		}
	}
}
