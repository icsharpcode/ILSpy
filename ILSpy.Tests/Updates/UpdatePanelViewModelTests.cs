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

using System.Collections.Generic;
using System.ComponentModel;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Updates;

/// <summary>
/// The update banner colours itself by state: green when ILSpy is up to date, amber when an
/// update is available. <see cref="UpdatePanelViewModel.UpdateAvailable"/> is the boolean that
/// drives that (bound to a style class in the view), so pin that it tracks the download URL.
/// </summary>
[TestFixture]
public class UpdatePanelViewModelTests
{
	[AvaloniaTest]
	public void UpdateAvailable_tracks_the_download_url()
	{
		var vm = AppComposition.Current.GetExport<UpdatePanelViewModel>();

		vm.UpdateAvailableDownloadUrl = null;
		vm.UpdateAvailable.Should().BeFalse("no download URL means ILSpy is up to date");

		vm.UpdateAvailableDownloadUrl = "https://example.invalid/ILSpy.zip";
		vm.UpdateAvailable.Should().BeTrue("a download URL means an update is available");
	}

	[AvaloniaTest]
	public void Setting_the_download_url_notifies_UpdateAvailable()
	{
		var vm = AppComposition.Current.GetExport<UpdatePanelViewModel>();
		vm.UpdateAvailableDownloadUrl = null;

		var changed = new List<string?>();
		vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

		vm.UpdateAvailableDownloadUrl = "https://example.invalid/ILSpy.zip";

		changed.Should().Contain(nameof(UpdatePanelViewModel.UpdateAvailable),
			"the banner colour binds to UpdateAvailable, so it must raise PropertyChanged for it");
	}
}
