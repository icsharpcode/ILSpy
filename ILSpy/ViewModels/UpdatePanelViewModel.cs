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
using System.ComponentModel;
using System.Composition;
using System.Diagnostics;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpy.Updates;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Drives the toolbar-strip banner shown under the main toolbar when an update is
	/// available or the user explicitly asks the Help → Check For Updates menu entry.
	/// Mirrors WPF's <c>UpdatePanelViewModel</c>: a single <see cref="IsPanelVisible"/>
	/// flag flips it on/off, and <see cref="UpdateAvailableDownloadUrl"/> toggles the
	/// button between "Download" (when a URL is set) and "Check Again" (when null).
	/// </summary>
	[Export]
	[Shared]
	public sealed partial class UpdatePanelViewModel : ObservableObject
	{
		readonly SettingsService settingsService;

		[ImportingConstructor]
		public UpdatePanelViewModel(SettingsService settingsService)
		{
			this.settingsService = settingsService;
		}

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ButtonText))]
		[NotifyPropertyChangedFor(nameof(Message))]
		[NotifyPropertyChangedFor(nameof(UpdateAvailable))]
		string? updateAvailableDownloadUrl;

		[ObservableProperty]
		bool isPanelVisible;

		/// <summary>"Download" while we have a URL, "Check Again" otherwise.</summary>
		public string ButtonText => UpdateAvailableDownloadUrl != null ? "Download" : "Check Again";

		/// <summary>Banner body — switches between "new version available" and "you're up to date".</summary>
		public string Message => UpdateAvailableDownloadUrl != null
			? "A new version of ILSpy is available."
			: "ILSpy is up to date.";

		/// <summary>True once a download URL is known. Drives the banner colour: amber when an
		/// update is available, green when ILSpy is up to date.</summary>
		public bool UpdateAvailable => UpdateAvailableDownloadUrl != null;

		[RelayCommand]
		void Close() => IsPanelVisible = false;

		[RelayCommand]
		async Task DownloadOrCheckUpdateAsync()
		{
			if (UpdateAvailableDownloadUrl != null)
			{
				OpenLink(UpdateAvailableDownloadUrl);
				return;
			}
			IsPanelVisible = false;
			var downloadUrl = await UpdateService.CheckForUpdatesAsync(settingsService.UpdateSettings);
			AdjustUpdateUiAfterCheck(downloadUrl, notify: true);
		}

		/// <summary>
		/// One-shot check on app startup (typically fire-and-forget from the first assembly
		/// load). Auto-throttled by the 7-day cooldown — does nothing if the user disabled
		/// automatic checks, and the panel only shows when an update is actually available.
		/// </summary>
		public async Task CheckIfUpdatesAvailableAsync(bool notifyOnUpToDate = false)
		{
			var downloadUrl = notifyOnUpToDate
				? await UpdateService.CheckForUpdatesAsync(settingsService.UpdateSettings)
				: await UpdateService.CheckForUpdatesIfEnabledAsync(settingsService.UpdateSettings);
			AdjustUpdateUiAfterCheck(downloadUrl, notifyOnUpToDate);
		}

		void AdjustUpdateUiAfterCheck(string? downloadUrl, bool notify)
		{
			UpdateAvailableDownloadUrl = downloadUrl;
			// Surface the panel when we have an update, or when the user explicitly clicked
			// Check For Updates and wants confirmation of an "up to date" result.
			IsPanelVisible = downloadUrl != null || notify;
		}

		static void OpenLink(string url)
		{
			try
			{
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			}
			catch
			{
				// Best-effort: silently ignore browser-launch failures (offline, no default
				// handler, etc.). The URL is also printed in the panel so the user can copy it.
			}
		}
	}
}
