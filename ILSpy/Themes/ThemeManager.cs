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

using Avalonia;
using Avalonia.Styling;

namespace ILSpy.Themes
{
	/// <summary>
	/// Light/Dark switcher backed by Avalonia's <see cref="Application.RequestedThemeVariant"/>.
	/// The full WPF theme set (R#, VS Code +/- variants) hasn't been ported yet — this is a
	/// minimal implementation so the View > Theme submenu does something useful today.
	/// </summary>
	public sealed class ThemeManager
	{
		public static ThemeManager Current { get; } = new();

		public string DefaultTheme => "Light";

		public static IReadOnlyCollection<string> AllThemes => new[] {
			"Light",
			"Dark",
		};

		public string? Theme { get; private set; }

		public bool IsDarkTheme => Theme == "Dark";

		ThemeManager()
		{
		}

		/// <summary>
		/// Wires this manager to a <see cref="SessionSettings"/> instance: applies the saved
		/// theme immediately and re-applies whenever Theme changes.
		/// </summary>
		public void Attach(SessionSettings settings)
		{
			UpdateTheme(settings.Theme);
			settings.PropertyChanged += OnSettingsChanged;

			void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
			{
				if (e.PropertyName == nameof(SessionSettings.Theme))
					UpdateTheme(settings.Theme);
			}
		}

		void UpdateTheme(string? themeName)
		{
			Theme = themeName ?? DefaultTheme;
			if (Application.Current is { } app)
			{
				app.RequestedThemeVariant = Theme == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
			}
		}
	}
}
