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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Updates
{
	internal static class UpdateService
	{
		const string ReleaseTagBaseUrl = "https://github.com/icsharpcode/ILSpy/releases/tag/";
		static readonly Uri UpdateUrl = new Uri("https://icsharpcode.github.io/ILSpy/updates.xml");
		const string band = "stable";

		public static AvailableVersionInfo LatestAvailableVersion { get; private set; }

		public static async Task<AvailableVersionInfo> GetLatestVersionAsync()
		{
			using var client = new HttpClient(new HttpClientHandler() {
				UseProxy = true,
				UseDefaultCredentials = true
			});

			return await GetLatestVersionAsync(client, UpdateUrl).ConfigureAwait(false);
		}

		internal static async Task<AvailableVersionInfo> GetLatestVersionAsync(HttpClient client, Uri updateUrl)
		{
			// Issue #3707: Remove 301 redirect logic once ilspy.net CNAME gone
			string data = await GetWithRedirectsAsync(client, updateUrl).ConfigureAwait(false);

			XDocument doc = XDocument.Load(new StringReader(data));
			var bands = doc.Root.Elements("band").ToList();
			var currentBand = bands.FirstOrDefault(b => (string)b.Attribute("id") == band) ?? bands.First();
			Version version = new Version((string)currentBand.Element("latestVersion"));

			string url = null;
			string releaseTag = (string)currentBand.Element("releaseTag");

			if (releaseTag != null)
			{
				url = ReleaseTagBaseUrl + releaseTag;

				// Prevent path traversal: normalize the URI and verify it still starts with the expected base
				if (!new Uri(url).AbsoluteUri.StartsWith(ReleaseTagBaseUrl, StringComparison.Ordinal))
					url = null;
			}
			else
			{
				// Issue #3707: Remove else branch fallback logic once releaseTag version has shipped + 6 months
				url = (string)currentBand.Element("downloadUrl");

				// Prevent arbitrary URLs: verify it starts with the expected base
				if (!new Uri(url).AbsoluteUri.StartsWith(ReleaseTagBaseUrl, StringComparison.Ordinal))
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
					uri = location?.IsAbsoluteUri == true ? location : new Uri(uri, location);
					continue;
				}

				response.EnsureSuccessStatusCode();
				return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}

			throw new HttpRequestException($"Exceeded maximum redirect limit ({maxRedirects}).");
		}

		/// <summary>
		/// If automatic update checking is enabled, checks if there are any updates available.
		/// Returns the download URL if an update is available.
		/// Returns null if no update is available, or if no check was performed.
		/// </summary>
		public static async Task<string> CheckForUpdatesIfEnabledAsync(UpdateSettings settings)
		{
			// If we're in an MSIX package, updates work differently
			if (!settings.AutomaticUpdateCheckEnabled)
				return null;

			// perform update check if we never did one before;
			// or if the last check wasn't in the past 7 days
			if (settings.LastSuccessfulUpdateCheck == null
				|| settings.LastSuccessfulUpdateCheck < DateTime.UtcNow.AddDays(-7)
				|| settings.LastSuccessfulUpdateCheck > DateTime.UtcNow)
			{
				return await CheckForUpdateInternal(settings).ConfigureAwait(false);
			}

			return null;
		}

		public static Task<string> CheckForUpdatesAsync(UpdateSettings settings)
		{
			return CheckForUpdateInternal(settings);
		}

		static async Task<string> CheckForUpdateInternal(UpdateSettings settings)
		{
			try
			{
				var v = await GetLatestVersionAsync().ConfigureAwait(false);
				settings.LastSuccessfulUpdateCheck = DateTime.UtcNow;
				if (v.Version > AppUpdateService.CurrentVersion)
					return v.DownloadUrl;
				else
					return null;
			}
			catch (Exception)
			{
				// ignore errors getting the version info
				return null;
			}
		}
	}
}
