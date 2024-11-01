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
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Updates
{
	internal static class UpdateService
	{
		static readonly Uri UpdateUrl = new Uri("https://ilspy.net/updates.xml");
		const string band = "stable";

		public static AvailableVersionInfo LatestAvailableVersion { get; private set; }

		public static async Task<AvailableVersionInfo> GetLatestVersionAsync()
		{
			var client = new HttpClient(new HttpClientHandler() {
				UseProxy = true,
				UseDefaultCredentials = true,
			});
			string data = await client.GetStringAsync(UpdateUrl).ConfigureAwait(false);

			XDocument doc = XDocument.Load(new StringReader(data));
			var bands = doc.Root.Elements("band");
			var currentBand = bands.FirstOrDefault(b => (string)b.Attribute("id") == band) ?? bands.First();
			Version version = new Version((string)currentBand.Element("latestVersion"));
			string url = (string)currentBand.Element("downloadUrl");
			if (!(url.StartsWith("http://", StringComparison.Ordinal) || url.StartsWith("https://", StringComparison.Ordinal)))
				url = null; // don't accept non-urls

			LatestAvailableVersion = new AvailableVersionInfo { Version = version, DownloadUrl = url };
			return LatestAvailableVersion;
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
