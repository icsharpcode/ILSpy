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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ICSharpCode.ILSpy.NuGetFeeds
{
	/// <summary>
	/// Downloads package icons over HTTP and decodes them into Avalonia bitmaps. Results are
	/// cached by URL for the loader's lifetime, so re-searching or re-displaying the same package
	/// reuses the already-fetched icon and each URL is downloaded at most once. Icons are decoded
	/// to a small fixed width since the dialog renders them at 32px.
	/// </summary>
	public sealed class NuGetIconLoader : INuGetIconLoader
	{
		// Rendered at 32px in the list and the detail header; 64px keeps it crisp on HiDPI
		// without holding the often-large published icon (128px+) in memory per result.
		const int DecodeWidth = 64;

		// Cap how much an icon response may buffer into memory; a broken or hostile feed could
		// otherwise stream an unbounded payload. Package icons are tiny, so a few MB is generous
		// and GetByteArrayAsync throws (caught below) once a response exceeds it.
		const int MaxResponseBytes = 4 * 1024 * 1024;

		static readonly HttpClient httpClient = new() {
			Timeout = TimeSpan.FromSeconds(15),
			MaxResponseContentBufferSize = MaxResponseBytes
		};

		// The cached value is the in-flight (or completed) decode task, so concurrent requests for
		// one URL share a single download instead of racing. A failed/missing icon caches as a
		// completed null - we don't retry a dead URL within a session.
		readonly ConcurrentDictionary<string, Task<IImage?>> cache =
			new(StringComparer.OrdinalIgnoreCase);

		public async Task<IImage?> LoadIconAsync(string iconUrl, CancellationToken cancellationToken)
		{
			if (!Uri.TryCreate(iconUrl, UriKind.Absolute, out var uri)
				|| (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
			{
				return null;
			}
			// One shared download per URL feeds every caller, so the cache dedups regardless of who
			// asked. Each caller awaits it through its own token, though, so a superseded search
			// stops waiting at once and gets null (per the interface contract) instead of hanging
			// on the in-flight download until HttpClient.Timeout.
			var download = cache.GetOrAdd(iconUrl,
				static (url, client) => DownloadAndDecodeAsync(client, url), httpClient);
			try
			{
				return await download.WaitAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				return null;
			}
		}

		static async Task<IImage?> DownloadAndDecodeAsync(HttpClient client, string url)
		{
			try
			{
				byte[] bytes = await client.GetByteArrayAsync(url).ConfigureAwait(false);
				using var stream = new MemoryStream(bytes);
				return Bitmap.DecodeToWidth(stream, DecodeWidth);
			}
			catch (Exception)
			{
				// A broken or unreachable icon URL is not an error worth surfacing: the dialog
				// just keeps showing the default NuGet icon for that package.
				return null;
			}
		}
	}
}
