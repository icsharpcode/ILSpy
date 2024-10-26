// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Linq;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpyX;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy
{
	[Export]
	[Shared]
	public class LanguageService : ObservableObjectBase
	{
		private readonly LanguageSettings languageSettings;

		public LanguageService(IEnumerable<Language> languages, SettingsService settingsService, DockWorkspace dockWorkspace)
		{
			languageSettings = settingsService.SessionSettings.LanguageSettings;

			var sortedLanguages = languages.ToList();

			sortedLanguages.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));
#if DEBUG
			sortedLanguages.AddRange(ILAstLanguage.GetDebugLanguages(dockWorkspace));
			sortedLanguages.AddRange(CSharpLanguage.GetDebugLanguages());
#endif
			AllLanguages = sortedLanguages.AsReadOnly();

			this.language = GetLanguage(languageSettings.LanguageId);
			this.languageVersion = Language.LanguageVersions.FirstOrDefault(v => v.Version == languageSettings.LanguageVersionId) ?? Language.LanguageVersions.LastOrDefault();
		}

		/// <summary>
		/// A list of all languages.
		/// </summary>
		public ReadOnlyCollection<Language> AllLanguages { get; }

		/// <summary>
		/// Gets a language using its name.
		/// If the language is not found, C# is returned instead.
		/// </summary>
		public Language GetLanguage(string? name)
		{
			return AllLanguages.FirstOrDefault(l => l.Name == name) ?? AllLanguages.First();
		}

		ILLanguage? ilLanguage;

		public ILLanguage ILLanguage => ilLanguage ??= (ILLanguage)GetLanguage("IL");

		/// <summary>
		/// This dictionary is necessary to remember language versions across language changes. For example, 
		/// the user first select C# 10, then switches to IL, then switches back to C#. After that we must be
		/// able to restore the original selection (i.e., C# 10).
		/// </summary>
		private readonly Dictionary<Language, LanguageVersion?> languageVersionHistory = new();

		Language language;

		/// <summary>
		/// Gets/Sets the current language.
		/// </summary>
		/// <remarks>
		/// While this isn't related to filtering, having it as part of the FilterSettings
		/// makes it easy to pass it down into all tree nodes.
		/// </remarks>
		public Language Language {
			get => language;
			set {
				if (language == value)
					return;

				if (language is { HasLanguageVersions: true })
				{
					languageVersionHistory[language] = languageVersion;
				}

				language = value;
				OnPropertyChanged();

				languageSettings.LanguageId = language.Name;

				if (language.HasLanguageVersions)
				{
					LanguageVersion = languageVersionHistory.TryGetValue(value, out var version) ? version : Language.LanguageVersions.Last();
				}
				else
				{
					LanguageVersion = default;
				}
			}
		}

		LanguageVersion? languageVersion;

		/// <summary>
		/// Gets/Sets the current language version.
		/// </summary>
		/// <remarks>
		/// While this isn't related to filtering, having it as part of the FilterSettings
		/// makes it easy to pass it down into all tree nodes.
		/// </remarks>
		public LanguageVersion? LanguageVersion {
			get { return languageVersion; }
			set {
				if (languageVersion == value)
					return;

				languageVersion = value;
				OnPropertyChanged();

				if (Language.HasLanguageVersions)
				{
					languageVersionHistory[Language] = languageVersion;
				}

				languageSettings.LanguageVersionId = languageVersion?.Version;
			}
		}
	}
}
