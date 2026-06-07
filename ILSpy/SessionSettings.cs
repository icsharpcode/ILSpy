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

		public LanguageSettings LanguageSettings { get; private set; } = null!;

		[ObservableProperty]
		private string? activeAssemblyList;

		[ObservableProperty]
		private string? activeLanguageName;

		[ObservableProperty]
		private string? theme;

		[ObservableProperty]
		private string? currentCulture;

		/// <summary>
		/// When true the document tab strip flows its tabs onto multiple rows; when false (the
		/// default) it stays a single scrolling row with an overflow dropdown. Toggled by the mouse
		/// wheel over the strip and persisted so the choice survives across sessions.
		/// </summary>
		[ObservableProperty]
		private bool multiLineDocumentTabs;

		/// <summary>
		/// Path to the previously-selected tree node (one ToString() per ancestor, root-first).
		/// Used to restore the selection on the next launch.
		/// </summary>
		public string[]? ActiveTreeViewPath { get; set; }

		public WindowState WindowState { get; set; } = WindowState.Normal;

		public PixelPoint WindowPosition { get; set; } = DefaultWindowPosition;

		public Size WindowSize { get; set; } = DefaultWindowSize;

		/// <summary>
		/// Per-(page-key, column-name) cache of serialised <c>FilterState</c>s, so the
		/// schema-driven flag-filter dropdowns remember the user's selections across
		/// sessions. Page key = entry-type full name (one filter set per metadata table,
		/// shared across all loaded assemblies). Mutated by
		/// <see cref="Metadata.Filters.FilterStatePersistence"/>; round-tripped to XML below.
		/// </summary>
		public Dictionary<(string PageKey, string ColumnName), XElement> FilterStates { get; }
			= new();

		public void LoadFromXml(XElement section)
		{
			XElement filterSettings = section.Element("FilterSettings") ?? new XElement("FilterSettings");
			LanguageSettings = new LanguageSettings(filterSettings, this);
			LanguageSettings.PropertyChanged += (s, e) => OnPropertyChanged(nameof(LanguageSettings));

			ActiveAssemblyList = (string?)section.Element("ActiveAssemblyList");
			ActiveLanguageName = (string?)section.Element("ActiveLanguageName");
			ActiveTreeViewPath = section.Element("ActiveTreeViewPath")?.Elements().Select(e => (string)e).ToArray();
			WindowState = ParseEnum(section.Element("WindowState")?.Value, WindowState.Normal);
			Theme = (string?)section.Element(nameof(Theme));
			var culture = (string?)section.Element(nameof(CurrentCulture));
			CurrentCulture = string.IsNullOrEmpty(culture) ? null : culture;
			MultiLineDocumentTabs = (bool?)section.Element(nameof(MultiLineDocumentTabs)) ?? false;

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

			FilterStates.Clear();
			foreach (var page in section.Elements("FilterStates").Elements("Page"))
			{
				var pageKey = (string?)page.Attribute("key");
				if (string.IsNullOrEmpty(pageKey))
					continue;
				foreach (var column in page.Elements("Column"))
				{
					var columnName = (string?)column.Attribute("name");
					var stateXml = column.Element("FilterState");
					if (string.IsNullOrEmpty(columnName) || stateXml is null)
						continue;
					FilterStates[(pageKey, columnName)] = new XElement(stateXml);
				}
			}
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);
			if (LanguageSettings != null)
				section.Add(LanguageSettings.SaveAsXml());
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
			if (!string.IsNullOrEmpty(Theme))
				section.Add(new XElement(nameof(Theme), Theme));
			if (!string.IsNullOrEmpty(CurrentCulture))
				section.Add(new XElement(nameof(CurrentCulture), CurrentCulture));
			section.Add(new XElement(nameof(MultiLineDocumentTabs), MultiLineDocumentTabs));

			if (FilterStates.Count > 0)
			{
				var filterStates = new XElement("FilterStates");
				foreach (var byPage in FilterStates.GroupBy(kv => kv.Key.PageKey).OrderBy(g => g.Key, StringComparer.Ordinal))
				{
					var page = new XElement("Page", new XAttribute("key", byPage.Key));
					foreach (var kv in byPage.OrderBy(kv => kv.Key.ColumnName, StringComparer.Ordinal))
					{
						page.Add(new XElement("Column",
							new XAttribute("name", kv.Key.ColumnName),
							new XElement(kv.Value)));
					}
					filterStates.Add(page);
				}
				section.Add(filterStates);
			}
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
