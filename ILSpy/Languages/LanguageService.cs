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

using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.Languages
{
	/// <summary>
	/// MEF-aggregated registry of <see cref="Language"/> exports plus the active selection.
	/// </summary>
	[Export]
	[Shared]
	public sealed partial class LanguageService : ObservableObject
	{
		readonly SettingsService settingsService;

		// Per-language remembered version selection. After flipping C# → IL → C# the user
		// expects their last C# version back, not a reset to the latest. WPF carries the same
		// dictionary on its LanguageService for the same reason.
		readonly Dictionary<Language, LanguageVersion?> versionHistory = new();

		public IReadOnlyList<Language> Languages { get; }

		[ObservableProperty]
		private Language currentLanguage;

		[ObservableProperty]
		private LanguageVersion? currentVersion;

		[ImportingConstructor]
		public LanguageService([ImportMany] IEnumerable<Language> languages, SettingsService settingsService)
		{
			using var _ = ICSharpCode.ILSpy.AppEnv.AppLog.Phase("LanguageService ctor (enumerate [ImportMany] Language exports)");
			this.settingsService = settingsService;
			List<Language> ordered;
			using (ICSharpCode.ILSpy.AppEnv.AppLog.Phase("LanguageService: materialise languages.OrderBy"))
				ordered = languages.OrderBy(l => l.Name).ToList();
			ICSharpCode.ILSpy.AppEnv.AppLog.Mark($"LanguageService: {ordered.Count} languages resolved");
			Languages = ordered;
			var saved = settingsService.SessionSettings.ActiveLanguageName;
			currentLanguage = Languages.FirstOrDefault(l => l.Name == saved)
				?? Languages.FirstOrDefault(l => l.Name == "C#")
				?? Languages.First();

			// Restore the last-saved version for the current language; fall back to the latest
			// available so a fresh install lands on the most useful default.
			var savedVersionId = settingsService.SessionSettings.LanguageSettings.LanguageVersionId;
			currentVersion = currentLanguage.LanguageVersions.FirstOrDefault(v => v.Version == savedVersionId)
				?? currentLanguage.LanguageVersions.LastOrDefault();
		}

		/// <summary>
		/// Looks up a language by name. Falls back to the first registered language (alphabetical
		/// order, so typically C#) when the name isn't recognised.
		/// </summary>
		public Language GetLanguage(string? name)
			=> Languages.FirstOrDefault(l => l.Name == name) ?? Languages.First();

		// The MVVM-toolkit source generator declares the partial method with nullable
		// reference types (the backing field is a nullable T). Match the signature exactly
		// to avoid a CS8611 nullability mismatch.
		partial void OnCurrentLanguageChanged(Language? oldValue, Language newValue)
		{
			if (newValue == null)
				return;
			settingsService.SessionSettings.ActiveLanguageName = newValue.Name;

			// Stash the outgoing language's chosen version so a later flip-back restores it.
			if (oldValue is { HasLanguageVersions: true })
				versionHistory[oldValue] = currentVersion;

			// Pull the new language's previously-selected version (or the latest if first time
			// here, or null if the language doesn't differentiate versions at all).
			if (newValue.HasLanguageVersions)
			{
				CurrentVersion = versionHistory.TryGetValue(newValue, out var remembered)
					? remembered
					: newValue.LanguageVersions.LastOrDefault();
			}
			else
			{
				CurrentVersion = null;
			}
		}

		partial void OnCurrentVersionChanged(LanguageVersion? value)
		{
			if (currentLanguage is { HasLanguageVersions: true })
				versionHistory[currentLanguage] = value;
			settingsService.SessionSettings.LanguageSettings.LanguageVersionId = value?.Version;
		}
	}
}
