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
using System.Text.RegularExpressions;
using System.Xml.Linq;

using Avalonia;
using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy
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
		/// When true (the default) the mouse wheel over the document tab strip toggles
		/// <see cref="MultiLineDocumentTabs"/> (wheel up flows to multiple rows, wheel down collapses
		/// to one). When false the wheel gesture is ignored. Persisted across sessions.
		/// </summary>
		[ObservableProperty]
		private bool mouseWheelTogglesTabStripRows = true;

		/// <summary>
		/// Path to the previously-selected tree node (one ToString() per ancestor, root-first).
		/// Used to restore the selection on the next launch.
		/// </summary>
		public string[]? ActiveTreeViewPath { get; set; }

		/// <summary>
		/// File path of the auto-loaded (dependency-resolved) assembly the user last
		/// selected into. The Avalonia host re-opens this file on startup before the
		/// <see cref="ActiveTreeViewPath"/> walk so the saved path can still resolve;
		/// otherwise the selection sits inside an assembly that isn't part of any
		/// persisted list and the restore silently fails.
		/// </summary>
		[ObservableProperty]
		private string? activeAutoLoadedAssembly;

		/// <summary>
		/// Last-used entry in the search pane's mode picker, persisted as the
		/// <see cref="ICSharpCode.ILSpyX.Search.SearchMode"/> name. <see cref="Search.SearchPaneModel"/>
		/// reads it on construction and writes back on every change.
		/// </summary>
		[ObservableProperty]
		private string? selectedSearchMode;

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

		// Set of child element names LoadFromXml interprets. Children outside this set
		// (legacy AvalonDock layout, a future ILSpy version's new field, …) are stashed
		// in <see cref="unknownChildren"/> and re-emitted unchanged by SaveToXml so we
		// don't strip data we don't understand.
		static readonly HashSet<string> KnownChildren = new(StringComparer.Ordinal) {
			"FilterSettings",
			"ActiveAssemblyList",
			"ActiveLanguageName",
			"ActiveTreeViewPath",
			"ActiveAutoLoadedAssembly",
			"SelectedSearchMode",
			"WindowState",
			nameof(Theme),
			nameof(CurrentCulture),
			nameof(MultiLineDocumentTabs),
			nameof(MouseWheelTogglesTabStripRows),
			"WindowBounds",
			"FilterStates",
		};

		List<XElement> unknownChildren = new();

		public void LoadFromXml(XElement section)
		{
			XElement filterSettings = section.Element("FilterSettings") ?? new XElement("FilterSettings");
			LanguageSettings = new LanguageSettings(filterSettings, this);
			LanguageSettings.PropertyChanged += (s, e) => OnPropertyChanged(nameof(LanguageSettings));

			ActiveAssemblyList = (string?)section.Element("ActiveAssemblyList");
			ActiveLanguageName = (string?)section.Element("ActiveLanguageName");
			ActiveTreeViewPath = section.Element("ActiveTreeViewPath")?.Elements().Select(e => UnescapeNode((string)e)).ToArray();
			ActiveAutoLoadedAssembly = (string?)section.Element("ActiveAutoLoadedAssembly");
			SelectedSearchMode = (string?)section.Element("SelectedSearchMode");
			WindowState = ParseEnum(section.Element("WindowState")?.Value, WindowState.Normal);
			Theme = (string?)section.Element(nameof(Theme));
			var culture = (string?)section.Element(nameof(CurrentCulture));
			CurrentCulture = string.IsNullOrEmpty(culture) ? null : culture;
			MultiLineDocumentTabs = (bool?)section.Element(nameof(MultiLineDocumentTabs)) ?? false;
			MouseWheelTogglesTabStripRows = (bool?)section.Element(nameof(MouseWheelTogglesTabStripRows)) ?? true;

			var bounds = section.Element("WindowBounds");
			if (bounds != null)
			{
				// Legacy WPF host wrote bounds as a CSV body "Left,Top,Width,Height" (Rect
				// TypeConverter format). Honour that shape for users upgrading from ILSpy 10.x
				// so a saved window position survives the move to the Avalonia attribute form.
				if (bounds.HasAttributes)
				{
					int left = ParseInt(bounds.Attribute("Left")?.Value, DefaultWindowPosition.X);
					int top = ParseInt(bounds.Attribute("Top")?.Value, DefaultWindowPosition.Y);
					double width = ParseDouble(bounds.Attribute("Width")?.Value, DefaultWindowSize.Width);
					double height = ParseDouble(bounds.Attribute("Height")?.Value, DefaultWindowSize.Height);
					WindowPosition = new PixelPoint(left, top);
					WindowSize = new Size(width, height);
				}
				else
				{
					var parts = bounds.Value.Split(',');
					if (parts.Length == 4)
					{
						double left = ParseDouble(parts[0], DefaultWindowPosition.X);
						double top = ParseDouble(parts[1], DefaultWindowPosition.Y);
						double width = ParseDouble(parts[2], DefaultWindowSize.Width);
						double height = ParseDouble(parts[3], DefaultWindowSize.Height);
						WindowPosition = new PixelPoint((int)left, (int)top);
						WindowSize = new Size(width, height);
					}
				}
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

			unknownChildren = section.Elements()
				.Where(e => !KnownChildren.Contains(e.Name.LocalName))
				.Select(e => new XElement(e))
				.ToList();
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
				section.Add(new XElement("ActiveTreeViewPath", path.Select(p => new XElement("Node", EscapeNode(p)))));
			if (!string.IsNullOrEmpty(ActiveAutoLoadedAssembly))
				section.Add(new XElement("ActiveAutoLoadedAssembly", ActiveAutoLoadedAssembly));
			if (!string.IsNullOrEmpty(SelectedSearchMode))
				section.Add(new XElement("SelectedSearchMode", SelectedSearchMode));
			section.Add(new XElement("WindowState", WindowState.ToString()));
			// Bounds as a CSV body ("L,T,W,H", the Rect TypeConverter format the WPF host used).
			// Keeping the legacy shape on write means a file written by this Avalonia build
			// can still be read by an older ILSpy 10.x install, and the file diff stays small
			// during the WPF -> Avalonia transition.
			section.Add(new XElement("WindowBounds", string.Format(
				CultureInfo.InvariantCulture, "{0},{1},{2},{3}",
				WindowPosition.X, WindowPosition.Y, WindowSize.Width, WindowSize.Height)));
			if (!string.IsNullOrEmpty(Theme))
				section.Add(new XElement(nameof(Theme), Theme));
			if (!string.IsNullOrEmpty(CurrentCulture))
				section.Add(new XElement(nameof(CurrentCulture), CurrentCulture));
			section.Add(new XElement(nameof(MultiLineDocumentTabs), MultiLineDocumentTabs));
			section.Add(new XElement(nameof(MouseWheelTogglesTabStripRows), MouseWheelTogglesTabStripRows));

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

			// Re-emit children we didn't interpret so unknown / future / retired-but-still-on-disk
			// elements (e.g. the AvalonDock <DockLayout> blob) survive a load+save cycle untouched.
			foreach (var unknown in unknownChildren)
				section.Add(new XElement(unknown));

			return section;
		}

		static T ParseEnum<T>(string? value, T defaultValue) where T : struct
			=> Enum.TryParse(value, out T result) ? result : defaultValue;

		static int ParseInt(string? value, int defaultValue)
			=> int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;

		static double ParseDouble(string? value, double defaultValue)
			=> double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) ? result : defaultValue;

		// Legacy WPF host hex-escaped every non-letter-or-digit char in tree-view path nodes
		// (TomsToolbox.Wpf -> TomsToolbox\x002EWpf). Both directions of the conversion stay
		// here so a file written by this build keeps the legacy on-disk shape — an older
		// ILSpy 10.x install can still read it, and the diff against a pre-existing
		// ILSpy.xml stays small.
		static readonly Regex EscapedCharPattern = new(@"\\x(?<num>[0-9A-Fa-f]{4})", RegexOptions.Compiled);

		static string UnescapeNode(string value)
		{
			if (string.IsNullOrEmpty(value) || value.IndexOf(@"\x", StringComparison.Ordinal) < 0)
				return value;
			return EscapedCharPattern.Replace(value, m => ((char)int.Parse(m.Groups["num"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToString());
		}

		static string EscapeNode(string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;
			var sb = new System.Text.StringBuilder(value.Length);
			foreach (var ch in value)
			{
				if (char.IsLetterOrDigit(ch))
					sb.Append(ch);
				else
					sb.AppendFormat(CultureInfo.InvariantCulture, @"\x{0:X4}", (int)ch);
			}
			return sb.ToString();
		}
	}
}
