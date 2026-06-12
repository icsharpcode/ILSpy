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
using System.Diagnostics;

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

using ICSharpCode.ILSpy.NuGetFeeds;
using ICSharpCode.ILSpy.ViewModels;

// Alias the WPF-shared Resources class — Window inherits an IResourceDictionary Resources
// property that would otherwise shadow ICSharpCode.ILSpy.Properties.Resources, turning every
// `Resources.X` into an IResourceDictionary indexer lookup that doesn't compile.
using Loc = ICSharpCode.ILSpy.Properties.Resources;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// "Open from NuGet feed" package-chooser dialog:
	/// search a V3 feed (editable package-source ComboBox, pre-release toggle, paged
	/// results), pick a version from a dropdown, and Open downloads the package into the
	/// global packages folder. Closes with the absolute path of the cached .nupkg, or
	/// null when cancelled; all behavior lives in
	/// <see cref="OpenFromNuGetFeedDialogViewModel"/>.
	/// </summary>
	public partial class OpenFromNuGetFeedDialog : Window
	{
		readonly OpenFromNuGetFeedDialogViewModel viewModel;

		// Runtime-loader/designer constructor; production callers and tests use the
		// overload below to inject the settings service and (in tests) a fake client.
		public OpenFromNuGetFeedDialog()
			: this(settingsService: null, client: new NuGetFeedClient())
		{
		}

		public OpenFromNuGetFeedDialog(
			SettingsService? settingsService,
			INuGetFeedClient client,
			TimeSpan? debounceDelay = null)
		{
			InitializeComponent();

			var settings = settingsService?.GetSettings<NuGetFeedSettings>() ?? new NuGetFeedSettings();
			viewModel = new OpenFromNuGetFeedDialogViewModel(client, settings, debounceDelay);
			DataContext = viewModel;

			Title = Loc.NuGetFeedSelectPackage;
			var searchBox = this.FindControl<TextBox>("SearchBox")!;
			searchBox.PlaceholderText = Loc.NuGetFeedSearchWatermark;
			ToolTip.SetTip(this.FindControl<Button>("RefreshButton")!, Loc.NuGetFeedRefresh);
			this.FindControl<CheckBox>("PrereleaseCheckBox")!.Content = Loc.NuGetFeedShowPrerelease;
			this.FindControl<Label>("PackageSourceLabel")!.Content = Loc.NuGetFeedPackageSource;
			this.FindControl<Button>("LoadMoreButton")!.Content = Loc.NuGetFeedLoadMore;
			this.FindControl<Label>("VersionLabel")!.Content = Loc.NuGetFeedVersion;
			this.FindControl<Button>("OpenButton")!.Content = Loc.OpenListDialog__Open;
			this.FindControl<TextBlock>("DownloadingLabel")!.Text = Loc.NuGetFeedDownloading;
			this.FindControl<Button>("CancelDownloadButton")!.Content = Loc.Cancel;
			this.FindControl<TextBlock>("DescriptionHeaderLabel")!.Text = Loc.NuGetFeedDescription;
			this.FindControl<TextBlock>("AuthorsLabel")!.Text = Loc.NuGetFeedAuthors;
			this.FindControl<TextBlock>("LicenseLabel")!.Text = Loc.NuGetFeedLicense;
			this.FindControl<TextBlock>("PublishedLabel")!.Text = Loc.NuGetFeedPublished;
			this.FindControl<TextBlock>("ProjectUrlLabel")!.Text = Loc.NuGetFeedProjectUrl;
			this.FindControl<TextBlock>("DownloadsLabel")!.Text = Loc.NuGetFeedDownloads;
			this.FindControl<TextBlock>("TagsLabel")!.Text = Loc.NuGetFeedTags;
			var cancelButton = this.FindControl<Button>("CancelButton")!;
			cancelButton.Content = Loc.Cancel;

			// Enter searches immediately, skipping the typing debounce.
			searchBox.KeyDown += (_, e) => {
				if (e.Key == Key.Enter)
				{
					viewModel.RefreshCommand.Execute(null);
					e.Handled = true;
				}
			};

			this.FindControl<TextBlock>("LicenseLink")!.PointerReleased +=
				(s, _) => OpenInBrowser(((TextBlock)s!).Text);
			this.FindControl<TextBlock>("ProjectUrlLink")!.PointerReleased +=
				(s, _) => OpenInBrowser(((TextBlock)s!).Text);

			cancelButton.Click += (_, _) => Close(null);
			viewModel.CloseRequested += path => Close(path);
			Closed += (_, _) => viewModel.CancelAllOperations();

			// Populate the dialog with the feed's default listing so it never opens empty.
			Opened += (_, _) => viewModel.RefreshCommand.Execute(null);
			Opened += (_, _) => searchBox.Focus();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		static void OpenInBrowser(string? url)
		{
			if (string.IsNullOrWhiteSpace(url)
				|| !Uri.TryCreate(url, UriKind.Absolute, out var uri)
				|| (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
			{
				return;
			}
			Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
		}
	}
}
