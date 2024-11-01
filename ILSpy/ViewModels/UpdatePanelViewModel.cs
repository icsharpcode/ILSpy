// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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

using System.Composition;
using System.Threading.Tasks;
using System.Windows.Input;

using ICSharpCode.ILSpy.Updates;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.ViewModels;

[Export]
[NonShared]
public class UpdatePanelViewModel : ObservableObject
{
	bool isPanelVisible;
	string updateAvailableDownloadUrl;
	readonly SettingsService settingsService;

	public UpdatePanelViewModel(SettingsService settingsService)
	{
		this.settingsService = settingsService;

		MessageBus<CheckIfUpdateAvailableEventArgs>.Subscribers += (_, e) => CheckIfUpdatesAvailableAsync(e.Notify).IgnoreExceptions();
	}

	public bool IsPanelVisible {
		get => isPanelVisible;
		set => SetProperty(ref isPanelVisible, value);
	}

	public string UpdateAvailableDownloadUrl {
		get => updateAvailableDownloadUrl;
		set => SetProperty(ref updateAvailableDownloadUrl, value);
	}

	public ICommand CloseCommand => new DelegateCommand(() => IsPanelVisible = false);

	public ICommand DownloadOrCheckUpdateCommand => new DelegateCommand(DownloadOrCheckUpdate);

	[PropertyDependency(nameof(UpdateAvailableDownloadUrl))]
	public string ButtonText => UpdateAvailableDownloadUrl != null
		? Properties.Resources.Download
		: Properties.Resources.CheckAgain;

	[PropertyDependency(nameof(UpdateAvailableDownloadUrl))]
	public string Message => UpdateAvailableDownloadUrl != null
		? Properties.Resources.ILSpyVersionAvailable
		: Properties.Resources.UpdateILSpyFound;

	async Task CheckIfUpdatesAvailableAsync(bool notify = false)
	{
		var settings = settingsService.GetSettings<UpdateSettings>();

		string downloadUrl = notify
			? await UpdateService.CheckForUpdatesAsync(settings)
			: await UpdateService.CheckForUpdatesIfEnabledAsync(settings);

		AdjustUpdateUIAfterCheck(downloadUrl, notify);
	}

	async void DownloadOrCheckUpdate()
	{
		if (updateAvailableDownloadUrl != null)
		{
			GlobalUtils.OpenLink(updateAvailableDownloadUrl);
		}
		else
		{
			IsPanelVisible = false;

			string downloadUrl = await UpdateService.CheckForUpdatesAsync(settingsService.GetSettings<UpdateSettings>());

			AdjustUpdateUIAfterCheck(downloadUrl, true);
		}
	}

	void AdjustUpdateUIAfterCheck(string downloadUrl, bool notify)
	{
		UpdateAvailableDownloadUrl = downloadUrl;
		IsPanelVisible = notify;
	}
}