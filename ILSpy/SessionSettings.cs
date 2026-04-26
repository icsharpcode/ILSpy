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
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

using Avalonia;
using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX.Settings;

namespace ILSpy
{
	/// <summary>
	/// Per-session UI state (active assembly list, main window bounds and state).
	/// Persisted to ILSpy.xml under &lt;SessionSettings&gt;.
	/// </summary>
	public sealed partial class SessionSettings : ObservableObject, ISettingsSection
	{
		public static readonly PixelPoint DefaultWindowPosition = new(100, 100);
		public static readonly Size DefaultWindowSize = new(900, 600);

		public XName SectionName => "SessionSettings";

		[ObservableProperty]
		private string? activeAssemblyList;

		[ObservableProperty]
		private string? activeLanguageName;

		/// <summary>
		/// Path to the previously-selected tree node (one ToString() per ancestor, root-first).
		/// Used to restore the selection on the next launch.
		/// </summary>
		public string[]? ActiveTreeViewPath { get; set; }

		public WindowState WindowState { get; set; } = WindowState.Normal;

		public PixelPoint WindowPosition { get; set; } = DefaultWindowPosition;

		public Size WindowSize { get; set; } = DefaultWindowSize;

		public void LoadFromXml(XElement section)
		{
			ActiveAssemblyList = (string?)section.Element("ActiveAssemblyList");
			ActiveLanguageName = (string?)section.Element("ActiveLanguageName");
			ActiveTreeViewPath = section.Element("ActiveTreeViewPath")?.Elements().Select(e => (string)e).ToArray();
			WindowState = ParseEnum(section.Element("WindowState")?.Value, WindowState.Normal);

			var bounds = section.Element("WindowBounds");
			if (bounds != null)
			{
				int left = ParseInt(bounds.Attribute("Left")?.Value, DefaultWindowPosition.X);
				int top = ParseInt(bounds.Attribute("Top")?.Value, DefaultWindowPosition.Y);
				double width = ParseDouble(bounds.Attribute("Width")?.Value, DefaultWindowSize.Width);
				double height = ParseDouble(bounds.Attribute("Height")?.Value, DefaultWindowSize.Height);
				WindowPosition = new PixelPoint(left, top);
				WindowSize = new Size(width, height);
			}
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);
			if (!string.IsNullOrEmpty(ActiveAssemblyList))
				section.Add(new XElement("ActiveAssemblyList", ActiveAssemblyList));
			if (!string.IsNullOrEmpty(ActiveLanguageName))
				section.Add(new XElement("ActiveLanguageName", ActiveLanguageName));
			if (ActiveTreeViewPath is { Length: > 0 } path)
				section.Add(new XElement("ActiveTreeViewPath", path.Select(p => new XElement("Node", p))));
			section.Add(new XElement("WindowState", WindowState.ToString()));
			section.Add(new XElement("WindowBounds",
				new XAttribute("Left", WindowPosition.X.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("Top", WindowPosition.Y.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("Width", WindowSize.Width.ToString(CultureInfo.InvariantCulture)),
				new XAttribute("Height", WindowSize.Height.ToString(CultureInfo.InvariantCulture))));
			return section;
		}

		static T ParseEnum<T>(string? value, T defaultValue) where T : struct
			=> Enum.TryParse(value, out T result) ? result : defaultValue;

		static int ParseInt(string? value, int defaultValue)
			=> int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;

		static double ParseDouble(string? value, double defaultValue)
			=> double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;
	}
}
