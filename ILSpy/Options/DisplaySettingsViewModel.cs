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
using System.Linq;
using System.Xml.Linq;

using global::Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.Properties;

namespace ILSpy.Options
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

		// Avalonia equivalent of WPF's Fonts.SystemFontFamilies. FontManager.Current populates
		// from the platform font enumerator (DirectWrite on Windows, FontConfig on Linux,
		// CoreText on macOS), so the list is realistic on every supported target.
		public IReadOnlyList<string> AvailableFonts { get; } = FontManager.Current.SystemFonts
			.Select(t => t.Name)
			.OrderBy(n => n, System.StringComparer.OrdinalIgnoreCase)
			.ToArray();

		public void Load(SettingsSnapshot snapshot)
		{
			Settings = snapshot.GetSettings<DisplaySettings>();
		}

		public void LoadDefaults()
		{
			// Reset by replaying LoadFromXml against an empty element — every attribute is
			// absent so each property falls back to its built-in default.
			Settings.LoadFromXml(new XElement("DisplaySettings"));
		}
	}
}
