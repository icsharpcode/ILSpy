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
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using global::Avalonia.Media;

using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>Viewmodel for the Display panel. Exposes the snapshot's
	/// <see cref="DisplaySettings"/> through <see cref="Settings"/> for direct two-way
	/// binding in the panel view, plus the list of system font families for the picker.</summary>
	[ExportOptionPage(Order = 20)]
	public sealed partial class DisplaySettingsViewModel : ObservableObject, IOptionPage
	{
		public string Title => Resources.Display;

		[ObservableProperty]
		DisplaySettings settings = null!;

		// Session settings carry the active Theme. The theme ComboBox binds two-way to
		// SessionSettings.Theme; ThemeManager already applies the change live on that property.
		[ObservableProperty]
		SessionSettings sessionSettings = null!;

		public System.Collections.Generic.IReadOnlyCollection<string> AllThemes => Themes.ThemeManager.AllThemes;

		// Avalonia equivalent of WPF's Fonts.SystemFontFamilies. FontManager.Current populates
		// from the platform font enumerator (DirectWrite on Windows, FontConfig on Linux,
		// CoreText on macOS), so the list is realistic on every supported target.
		public IReadOnlyList<string> AvailableFonts { get; } = FontManager.Current.SystemFonts
			.Select(t => t.Name)
			.OrderBy(n => n, System.StringComparer.OrdinalIgnoreCase)
			.ToArray();

		/// <summary>
		/// Derived FontFamily for the preview TextBlock. Avalonia's runtime binding pipeline
		/// doesn't auto-coerce a <c>string</c> source to <see cref="FontFamily"/> (the implicit
		/// conversion fires at XAML parse time, not on binding evaluation), so the preview
		/// goes through this property — recomputed on every <see cref="DisplaySettings.SelectedFont"/>
		/// change via the PropertyChanged subscription in <see cref="Load"/>.
		/// </summary>
		public FontFamily CurrentFontFamily =>
			Settings != null && !string.IsNullOrEmpty(Settings.SelectedFont)
				? new FontFamily(Settings.SelectedFont)
				: FontFamily.Default;

		public void Load(SettingsService service)
		{
			if (Settings != null)
				Settings.PropertyChanged -= OnSettingsPropertyChanged;
			Settings = service.DisplaySettings;
			Settings.PropertyChanged += OnSettingsPropertyChanged;
			SessionSettings = service.SessionSettings;
			OnPropertyChanged(nameof(CurrentFontFamily));
		}

		void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DisplaySettings.SelectedFont))
				OnPropertyChanged(nameof(CurrentFontFamily));
		}

		public void LoadDefaults()
		{
			// Reset by replaying LoadFromXml against an empty element — every attribute is
			// absent so each property falls back to its built-in default.
			Settings.LoadFromXml(new XElement("DisplaySettings"));
			SessionSettings.Theme = Themes.ThemeManager.Current.DefaultTheme;
		}
	}
}
