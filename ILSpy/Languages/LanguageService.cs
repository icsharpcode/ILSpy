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

namespace ILSpy.Languages
{
	/// <summary>
	/// MEF-aggregated registry of <see cref="Language"/> exports plus the active selection.
	/// </summary>
	[Export]
	[Shared]
	public sealed partial class LanguageService : ObservableObject
	{
		readonly SettingsService settingsService;

		public IReadOnlyList<Language> Languages { get; }

		[ObservableProperty]
		private Language currentLanguage;

		[ImportingConstructor]
		public LanguageService([ImportMany] IEnumerable<Language> languages, SettingsService settingsService)
		{
			this.settingsService = settingsService;
			Languages = languages.OrderBy(l => l.Name).ToList();
			var saved = settingsService.SessionSettings.ActiveLanguageName;
			currentLanguage = Languages.FirstOrDefault(l => l.Name == saved)
				?? Languages.FirstOrDefault(l => l.Name == "C#")
				?? Languages.First();
		}

		/// <summary>
		/// Looks up a language by name. Falls back to the first registered language (alphabetical
		/// order, so typically C#) when the name isn't recognised.
		/// </summary>
		public Language GetLanguage(string? name)
			=> Languages.FirstOrDefault(l => l.Name == name) ?? Languages.First();

		partial void OnCurrentLanguageChanged(Language value)
		{
			if (value != null)
				settingsService.SessionSettings.ActiveLanguageName = value.Name;
		}
	}
}
