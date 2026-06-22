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
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.NuGetFeeds;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// One row in the "Open from NuGet feed" results list: the immutable feed
	/// <see cref="NuGetPackageInfo"/> plus the observable <see cref="Icon"/>. The icon starts as
	/// the default NuGet logo and is swapped for the package's own icon once it has been fetched,
	/// so the list can render immediately and fill in custom icons as they arrive.
	/// </summary>
	public sealed partial class NuGetPackageViewModel : ObservableObject
	{
		public NuGetPackageInfo Info { get; }

		[ObservableProperty]
		IImage icon = Images.NuGet;

		public NuGetPackageViewModel(NuGetPackageInfo info)
		{
			Info = info;
		}

		/// <summary>
		/// Fetches the package's custom icon in the background and, on success, replaces the
		/// default icon. A package without an icon URL keeps the default and never hits the loader.
		/// </summary>
		public async Task LoadIconAsync(INuGetIconLoader loader, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(Info.IconUrl))
				return;
			try
			{
				var image = await loader.LoadIconAsync(Info.IconUrl, cancellationToken);
				// The await resumes on the UI thread (the dialog view model drives this from
				// there), so assigning the observable Icon raises PropertyChanged safely.
				if (image != null && !cancellationToken.IsCancellationRequested)
					Icon = image;
			}
			catch (Exception)
			{
				// This runs fire-and-forget, so nothing observes the returned task: any failure
				// (cancellation from a superseded search, or a misbehaving loader) must be swallowed
				// here rather than surface as an unobserved task exception. Keep the default icon.
			}
		}
	}
}
