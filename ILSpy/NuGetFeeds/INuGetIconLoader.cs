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

using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media;

namespace ICSharpCode.ILSpy.NuGetFeeds
{
	/// <summary>
	/// Fetches a package's custom icon for the "Open from NuGet feed" dialog. The production
	/// implementation (<see cref="NuGetIconLoader"/>) downloads and decodes the image; tests
	/// substitute a fake so no network is involved.
	/// </summary>
	public interface INuGetIconLoader
	{
		/// <summary>
		/// Downloads and decodes the icon at <paramref name="iconUrl"/>. Returns
		/// <see langword="null"/> for anything that isn't a usable icon - a non-http(s) URL, a
		/// network or decode failure, or cancellation - so callers simply keep the default icon.
		/// </summary>
		Task<IImage?> LoadIconAsync(string iconUrl, CancellationToken cancellationToken);
	}
}
