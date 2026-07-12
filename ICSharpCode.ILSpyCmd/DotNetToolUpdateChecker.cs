// Copyright (c) 2023 Christoph Wille
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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace ICSharpCode.ILSpyCmd
{
	internal record PackageCheckResult(NuGetVersion RunningVersion, NuGetVersion LatestVersion, bool UpdateRecommendation);

	// Idea from https://github.com/ErikEJ/EFCorePowerTools/blob/master/src/GUI/efcpt/Services/PackageService.cs
	internal static class DotNetToolUpdateChecker
	{
		static NuGetVersion CurrentPackageVersion()
		{
			return new NuGetVersion(Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
				.InformationalVersion);
		}

		public static async Task<PackageCheckResult> CheckForPackageUpdateAsync(string packageId)
		{
			try
			{
				using var cache = new SourceCacheContext();
				var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
				var resource = await repository.GetResourceAsync<FindPackageByIdResource>().ConfigureAwait(false);

				var versions = await resource.GetAllVersionsAsync(
					packageId,
					cache,
					new NullLogger(),
					CancellationToken.None).ConfigureAwait(false);

				var latestVersion = versions.Where(v => v.Release == "").MaxBy(v => v);
				var runningVersion = CurrentPackageVersion();
				int comparisonResult = latestVersion.CompareTo(runningVersion, VersionComparison.Version);

				return new PackageCheckResult(runningVersion, latestVersion, comparisonResult > 0);
			}
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
			catch (Exception)
			{
			}
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.

			return null;
		}
	}
}
