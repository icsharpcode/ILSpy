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

namespace ILSpy.Options
{
	/// <summary>
	/// Content viewmodel for the Options document tab. Wraps a <see cref="SettingsSnapshot"/>,
	/// MEF-discovered <see cref="IOptionPage"/> panels (ordered by <see cref="IOptionsMetadata.Order"/>),
	/// and the page-level Apply / Reset commands the panel views bind to.
	/// </summary>
	public sealed partial class OptionsPageModel : ObservableObject
	{
		readonly SettingsSnapshot snapshot;

		public OptionsPageModel(SettingsService settingsService, IEnumerable<ExportFactory<IOptionPage, IOptionsMetadata>> pageFactories)
		{
			snapshot = settingsService.CreateSnapshot();

			Pages = pageFactories
				.OrderBy(f => f.Metadata.Order)
				.Select(f => f.CreateExport().Value)
				.ToArray();

			foreach (var page in Pages)
				page.Load(snapshot);

			selectedPage = Pages.FirstOrDefault();
			Title = Resources._Options;

			ApplyCommand = new RelayCommand(Apply);
			ResetCurrentPageCommand = new RelayCommand(ResetCurrentPage);
		}

		public string Title { get; }

		/// <summary>Marker for the dock router: tree-node selections must not replace this
		/// tab. Mirrors the <see cref="ILSpy.TextView.DecompilerTabPageModel.IsStaticContent"/>
		/// convention used for the About tab.</summary>
		public bool IsStaticContent => true;

		public IReadOnlyList<IOptionPage> Pages { get; }

		[ObservableProperty]
		IOptionPage? selectedPage;

		public IRelayCommand ApplyCommand { get; }

		public IRelayCommand ResetCurrentPageCommand { get; }

		void Apply() => snapshot.Save();

		void ResetCurrentPage() => SelectedPage?.LoadDefaults();
	}
}
