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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.Updates
{
	/// <summary>
	/// Per-build version identity reported to <see cref="UpdateService"/> for comparing
	/// against the latest available release. Composed from
	/// <see cref="DecompilerVersionInfo"/>'s parts so the build pipeline drives it.
	/// </summary>
	public static class AppUpdateService
	{
		public static readonly Version CurrentVersion = new(
			DecompilerVersionInfo.Major + "." + DecompilerVersionInfo.Minor + "." +
			DecompilerVersionInfo.Build + "." + DecompilerVersionInfo.Revision);
	}

	/// <summary>Result of a successful <see cref="UpdateService.GetLatestVersionAsync"/> poll.</summary>
	public sealed class AvailableVersionInfo
	{
		public required Version Version { get; init; }
		public string? DownloadUrl { get; init; }
	}

	/// <summary>
	/// Polls icsharpcode.github.io/ILSpy/updates.xml for the latest stable-band release
	/// and exposes both a one-shot check (<see cref="CheckForUpdatesAsync"/>) and an
	/// auto-throttled check that respects the user's "automatic update check" preference
	/// and the 7-day cooldown (<see cref="CheckForUpdatesIfEnabledAsync"/>).
	/// </summary>
	public static class UpdateService
	{
		const string ReleaseTagBaseUrl = "https://github.com/icsharpcode/ILSpy/releases/tag/";
		static readonly Uri UpdateUrl = new("https://icsharpcode.github.io/ILSpy/updates.xml");
		const string Band = "stable";

		public static AvailableVersionInfo? LatestAvailableVersion { get; private set; }

		public static async Task<AvailableVersionInfo> GetLatestVersionAsync()
		{
			using var client = new HttpClient(new HttpClientHandler {
				UseProxy = true,
				UseDefaultCredentials = true,
			});
			return await GetLatestVersionAsync(client, UpdateUrl).ConfigureAwait(false);
		}

		internal static async Task<AvailableVersionInfo> GetLatestVersionAsync(HttpClient client, Uri updateUrl)
		{
			string data = await GetWithRedirectsAsync(client, updateUrl).ConfigureAwait(false);

			var doc = XDocument.Load(new StringReader(data));
			var bands = doc.Root!.Elements("band").ToList();
			var currentBand = bands.FirstOrDefault(b => (string?)b.Attribute("id") == Band) ?? bands.First();
			var version = new Version((string)currentBand.Element("latestVersion")!);

			string? url = null;
			var releaseTag = (string?)currentBand.Element("releaseTag");
			if (releaseTag != null)
			{
				url = ReleaseTagBaseUrl + releaseTag;
				// Path-traversal guard: normalised URI must still start with the expected base.
				if (!new Uri(url).AbsoluteUri.StartsWith(ReleaseTagBaseUrl, StringComparison.Ordinal))
					url = null;
			}
			else
			{
				url = (string?)currentBand.Element("downloadUrl");
				if (url != null && !new Uri(url).AbsoluteUri.StartsWith(ReleaseTagBaseUrl, StringComparison.Ordinal))
					url = null;
			}

			LatestAvailableVersion = new AvailableVersionInfo { Version = version, DownloadUrl = url };
			return LatestAvailableVersion;
		}

		static async Task<string> GetWithRedirectsAsync(HttpClient client, Uri uri, int maxRedirects = 5)
		{
			for (int i = 0; i <= maxRedirects; i++)
			{
				using var response = await client.GetAsync(uri).ConfigureAwait(false);
				if (response.StatusCode is HttpStatusCode.MovedPermanently
					or HttpStatusCode.Found
					or HttpStatusCode.TemporaryRedirect
					or HttpStatusCode.PermanentRedirect)
				{
					var location = response.Headers.Location;
					if (location == null)
						throw new HttpRequestException($"Redirect from {uri} had no Location header");
					uri = location.IsAbsoluteUri ? location : new Uri(uri, location);
					continue;
				}
				response.EnsureSuccessStatusCode();
				return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			throw new HttpRequestException($"Exceeded maximum redirect limit ({maxRedirects}).");
		}

		/// <summary>
		/// Auto-throttled check: runs only if automatic checks are enabled in settings AND
		/// the last successful check was more than 7 days ago (or never). Returns the
		/// download URL of a newer version, or <c>null</c> when no update is available /
		/// no check was performed.
		/// </summary>
		public static async Task<string?> CheckForUpdatesIfEnabledAsync(UpdateSettings settings)
		{
			if (!settings.AutomaticUpdateCheckEnabled)
				return null;
			if (settings.LastSuccessfulUpdateCheck == null
				|| settings.LastSuccessfulUpdateCheck < DateTime.UtcNow.AddDays(-7)
				|| settings.LastSuccessfulUpdateCheck > DateTime.UtcNow)
			{
				return await CheckForUpdateInternal(settings).ConfigureAwait(false);
			}
			return null;
		}

		/// <summary>Force a check regardless of the throttle. Used by the Help → Check For Updates menu entry.</summary>
		public static Task<string?> CheckForUpdatesAsync(UpdateSettings settings)
			=> CheckForUpdateInternal(settings);

		static async Task<string?> CheckForUpdateInternal(UpdateSettings settings)
		{
			try
			{
				var v = await GetLatestVersionAsync().ConfigureAwait(false);
				settings.LastSuccessfulUpdateCheck = DateTime.UtcNow;
				if (v.Version > AppUpdateService.CurrentVersion)
					return v.DownloadUrl;
				return null;
			}
			catch
			{
				// Network errors swallowed — the panel just shows "no update found".
				return null;
			}
		}
	}
}
