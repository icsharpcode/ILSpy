// Copyright (c) 2026 Franck
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

namespace Redecomp.Core.Recovery;

/// <summary>Fetches a source file from the location SourceLink gives for it.</summary>
public interface ISourceDownloader
{
	/// <summary>Returns the file content, or null when it cannot be fetched.</summary>
	byte[]? TryDownload(Uri uri);
}

/// <summary>Downloads source files over HTTP(S).</summary>
public sealed class HttpSourceDownloader : ISourceDownloader, IDisposable
{
	public static HttpSourceDownloader Shared { get; } = new();

	readonly HttpClient client = new() { Timeout = TimeSpan.FromSeconds(30) };

	public void Dispose() => client.Dispose();

	public byte[]? TryDownload(Uri uri)
	{
		try
		{
			using var response = client.GetAsync(uri).GetAwaiter().GetResult();
			return response.IsSuccessStatusCode ? response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult() : null;
		}
		catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
		{
			return null;
		}
	}
}
