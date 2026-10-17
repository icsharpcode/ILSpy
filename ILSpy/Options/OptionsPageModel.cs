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
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

			VisiblePages = new ObservableCollection<IOptionPage>(Pages);
			selectedPage = Pages.FirstOrDefault();
			// Bare "Options" string — the menu item's Resources._Options carries the
			// accelerator underscore + "..." suffix which are menu-only conventions.
			Title = Resources.Options;
			// Tree-node selections must not replace the Options tab (ContentPageModel.IsStaticContent).
			IsStaticContent = true;

			ResetCurrentPageCommand = new RelayCommand(ResetCurrentPage);
		}

		public IReadOnlyList<IOptionPage> Pages { get; }

		/// <summary>The pages listed in the tab strip: all of them while <see cref="SearchText"/>
		/// is empty, otherwise only those whose <see cref="IOptionPage.SearchableText"/> has a hit.
		/// Edited in place rather than replaced, so the TabControl keeps the selected page's
		/// panel (and its scroll position) while the user types.</summary>
		public ObservableCollection<IOptionPage> VisiblePages { get; }

		[ObservableProperty]
		IOptionPage? selectedPage;

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(NoMatchesMessage))]
		string searchText = string.Empty;

		public bool HasNoMatches => SearchText.Length > 0 && VisiblePages.Count == 0;

		public string NoMatchesMessage => string.Format(Resources.NoSettingsMatch, SearchText);

		public IRelayCommand ResetCurrentPageCommand { get; }

		void ResetCurrentPage() => SelectedPage?.LoadDefaults();

		partial void OnSearchTextChanged(string value)
		{
			foreach (var page in Pages)
				page.ApplySearch(value);
			var visible = value.Length == 0
				? Pages
				: Pages.Where(p => IsMatch(p, value)).ToArray();
			// Both lists are subsequences of Pages in the same order: drop what fell out, then
			// insert what came back at its position.
			for (int i = VisiblePages.Count - 1; i >= 0; i--)
			{
				if (!visible.Contains(VisiblePages[i]))
					VisiblePages.RemoveAt(i);
			}
			for (int i = 0; i < visible.Count; i++)
			{
				if (i >= VisiblePages.Count || VisiblePages[i] != visible[i])
					VisiblePages.Insert(i, visible[i]);
			}
			OnPropertyChanged(nameof(HasNoMatches));
			// Keep the selection where it is unless the page is no longer listed; then fall
			// back to the first listed page (null when nothing matches at all).
			if (SelectedPage == null || !VisiblePages.Contains(SelectedPage))
				SelectedPage = VisiblePages.FirstOrDefault();
		}

		static bool IsMatch(IOptionPage page, string text)
		{
			// A page that publishes no labels is listed regardless: it may be a plugin page that
			// predates the search box, and hiding it would make its settings unreachable.
			var labels = page.SearchableText.ToList();
			return labels.Count == 0 || labels.Any(l => Contains(l, text));
		}

		/// <summary>Case-insensitive substring match in the current culture, since labels are localized.</summary>
		public static bool Contains(string? label, string text)
			=> label != null && label.Contains(text, StringComparison.CurrentCultureIgnoreCase);
	}
}
