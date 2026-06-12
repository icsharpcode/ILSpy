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
using System.Composition;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Content viewmodel for the Options document tab. Wraps the live
	/// <see cref="SettingsService"/>, MEF-discovered <see cref="IOptionPage"/> panels
	/// (ordered by <see cref="IOptionsMetadata.Order"/>), and a Reset command that
	/// rolls the currently-selected panel back to its built-in defaults.
	/// <para>
	/// Panels bind two-way to the live section instances, so every toggle takes effect
	/// immediately — there's no Apply step. XML persistence rides on
	/// <see cref="SettingsService.Save"/> at app exit.
	/// </para>
	/// </summary>
	public sealed partial class OptionsPageModel : ContentPageModel
	{
		public OptionsPageModel(SettingsService settingsService, IEnumerable<ExportFactory<IOptionPage, IOptionsMetadata>> pageFactories)
		{
			Pages = pageFactories
				.OrderBy(f => f.Metadata.Order)
				.Select(f => f.CreateExport().Value)
				.ToArray();

			foreach (var page in Pages)
				page.Load(settingsService);

			selectedPage = Pages.FirstOrDefault();
			// Bare "Options" string — the menu item's Resources._Options carries the
			// accelerator underscore + "..." suffix which are menu-only conventions.
			Title = Resources.Options;
			// Tree-node selections must not replace the Options tab (ContentPageModel.IsStaticContent).
			IsStaticContent = true;

			ResetCurrentPageCommand = new RelayCommand(ResetCurrentPage);
		}

		public IReadOnlyList<IOptionPage> Pages { get; }

		[ObservableProperty]
		IOptionPage? selectedPage;

		public IRelayCommand ResetCurrentPageCommand { get; }

		void ResetCurrentPage() => SelectedPage?.LoadDefaults();
	}
}
