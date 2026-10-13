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
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyCmd
{
	internal record PackageCheckResult(Version RunningVersion, Version LatestVersion, bool UpdateRecommendation);

	// Idea from https://github.com/ErikEJ/EFCorePowerTools/blob/master/src/GUI/efcpt/Services/PackageService.cs
	internal static class DotNetToolUpdateChecker
	{
		// The flat container is the smallest nuget.org endpoint that lists a package's versions:
		// {"versions":["1.0.0","1.1.0-preview1",...]}, lowercase ids, prereleases included.
		const string FlatContainerIndexUrl = "https://api.nuget.org/v3-flatcontainer/{0}/index.json";

		public static async Task<PackageCheckResult> CheckForPackageUpdateAsync(string packageId)
		{
			try
			{
				using var http = new HttpClient();
				var index = await http.GetFromJsonAsync(
					string.Format(FlatContainerIndexUrl, packageId.ToLowerInvariant()),
					FlatContainerJsonContext.Default.FlatContainerIndex).ConfigureAwait(false);

				var latestVersion = LatestStableVersion(index?.Versions ?? Array.Empty<string>());
				if (latestVersion == null)
					return null;
				var runningVersion = RunningVersion(Assembly.GetEntryAssembly()!
					.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion);

				return new PackageCheckResult(runningVersion, latestVersion, latestVersion > runningVersion);
			}
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception.
			catch (Exception)
			{
			}
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception.

			return null;
		}

		/// <summary>
		/// The highest listed version without a prerelease label, or null if there is none.
		/// </summary>
		internal static Version LatestStableVersion(IEnumerable<string> versions)
		{
			Version latest = null;
			foreach (string version in versions)
			{
				if (version.Contains('-') || !Version.TryParse(version, out Version parsed))
					continue;
				parsed = Normalize(parsed);
				if (latest == null || parsed > latest)
					latest = parsed;
			}
			return latest;
		}

		/// <summary>
		/// The numeric part of an informational version such as "11.1.0.9927-pr4241+0dab0067ec".
		/// </summary>
		internal static Version RunningVersion(string informationalVersion)
		{
			int end = informationalVersion.IndexOfAny(new[] { '-', '+' });
			return Normalize(Version.Parse(end < 0 ? informationalVersion : informationalVersion.Substring(0, end)));
		}

		// System.Version compares a missing component as -1, so "11.1.0" would rank below
		// "11.1.0.0"; NuGet treats them as equal.
		static Version Normalize(Version version)
		{
			return new Version(version.Major, version.Minor, Math.Max(version.Build, 0), Math.Max(version.Revision, 0));
		}
	}

	sealed class FlatContainerIndex
	{
		[JsonPropertyName("versions")]
		public string[] Versions { get; set; }
	}

	[JsonSerializable(typeof(FlatContainerIndex))]
	partial class FlatContainerJsonContext : JsonSerializerContext
	{
	}
}
