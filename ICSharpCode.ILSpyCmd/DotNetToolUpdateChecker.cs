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
	// Idea from https://github.com/ErikEJ/EFCorePowerTools/blob/master/src/GUI/efcpt/Services/PackageService.cs
	internal static class DotNetToolUpdateChecker
	{
		static NuGetVersion CurrentPackageVersion()
		{
			return new NuGetVersion(Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
				.InformationalVersion);
		}

		public static async Task<NuGetVersion> CheckForPackageUpdateAsync(string packageId)
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
				if (latestVersion > CurrentPackageVersion())
				{
					return latestVersion;
				}
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
