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
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Media;

using ICSharpCode.ILSpy.NuGetFeeds;

namespace ICSharpCode.ILSpy.Tests.NuGetFeeds;

/// <summary>
/// In-memory <see cref="INuGetIconLoader"/>: records every requested URL and returns a distinct
/// placeholder image per URL (or null to model a missing/broken icon), so tests can assert that a
/// package's icon was fetched and swapped in without touching the network.
/// </summary>
public sealed class FakeNuGetIconLoader : INuGetIconLoader
{
	public ConcurrentBag<string> RequestedUrls { get; } = new();

	/// <summary>When false, every load resolves to null (models a broken/missing icon).</summary>
	public bool ReturnImage { get; set; } = true;

	/// <summary>
	/// When set, loads do not complete until this source is resolved (or the call's token fires),
	/// letting a test hold icon loads in flight and then cancel them with a new search.
	/// </summary>
	public TaskCompletionSource? Gate { get; set; }

	public Task<IImage?> LoadIconAsync(string iconUrl, CancellationToken cancellationToken)
	{
		RequestedUrls.Add(iconUrl);
		if (cancellationToken.IsCancellationRequested)
			return Task.FromResult<IImage?>(null);
		if (Gate != null)
			return AwaitGateAsync(iconUrl, cancellationToken);
		return Task.FromResult(Result(iconUrl));
	}

	async Task<IImage?> AwaitGateAsync(string iconUrl, CancellationToken cancellationToken)
	{
		// The contract is "cancellation yields a null icon", not an exception; model that so tests
		// exercise the same path production takes when a search is superseded mid-load.
		try
		{
			await Gate!.Task.WaitAsync(cancellationToken);
		}
		catch (OperationCanceledException)
		{
			return null;
		}
		return Result(iconUrl);
	}

	IImage? Result(string iconUrl) => ReturnImage ? new StubImage(iconUrl) : null;

	/// <summary>A trivial non-drawing <see cref="IImage"/> that carries the URL it stands in for.</summary>
	public sealed class StubImage(string url) : IImage
	{
		public string Url { get; } = url;
		public Size Size => new(64, 64);
		public void Draw(DrawingContext context, Rect sourceRect, Rect destRect) { }
	}
}
