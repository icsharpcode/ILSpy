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

using System.Linq;
using System.Windows;
using System.Windows.Input;

using TomsToolbox.Essentials;
using TomsToolbox.Wpf;

#nullable enable

namespace ICSharpCode.ILSpy.Options
{
	public class OptionsItemViewModel(IOptionPage content) : ObservableObject
	{
		public string Title { get; } = content.Title;

		public IOptionPage Content { get; } = content;
	}

	public class OptionsDialogViewModel : ObservableObject
	{
		private IOptionPage? selectedPage;

		private SettingsSnapshot snapshot;

		private readonly IOptionPage[] optionPages = App.ExportProvider.GetExports<IOptionPage, IOptionsMetadata>("OptionPages")
			.OrderBy(page => page.Metadata?.Order)
			.Select(item => item.Value)
			.ExceptNullItems()
			.ToArray();

		public OptionsDialogViewModel(SettingsService settingsService)
		{
			this.snapshot = settingsService.CreateSnapshot();

			foreach (var optionPage in optionPages)
			{
				optionPage.Load(this.snapshot);
			}

			OptionPages = optionPages.Select(page => new OptionsItemViewModel(page)).ToArray();
			SelectedPage = optionPages.FirstOrDefault();
		}

		public OptionsItemViewModel[] OptionPages { get; }

		public IOptionPage? SelectedPage {
			get => selectedPage;
			set => SetProperty(ref selectedPage, value);
		}

		public ICommand CommitCommand => new DelegateCommand(Commit);

		public ICommand ResetDefaultsCommand => new DelegateCommand(ResetDefaults);

		private void ResetDefaults()
		{
			if (MessageBox.Show(Properties.Resources.ResetToDefaultsConfirmationMessage, "ILSpy", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				SelectedPage?.LoadDefaults();
			}
		}

		private void Commit()
		{
			snapshot.Save();
		}
	}
}
