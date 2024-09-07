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

using System.Windows.Media;
using System.Xml.Linq;

using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpyX.Settings;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Description of DisplaySettings.
	/// </summary>
	public class DisplaySettings : ObservableObject
	{
		public DisplaySettings()
		{
			this.theme = ThemeManager.Current.DefaultTheme;
			this.selectedFont = new FontFamily("Consolas");
			this.selectedFontSize = 10.0 * 4 / 3;
			this.sortResults = true;
			this.indentationUseTabs = true;
			this.indentationSize = 4;
			this.indentationTabSize = 4;
			this.highlightMatchingBraces = true;
		}

		string theme;

		public string Theme {
			get => theme;
			set => SetProperty(ref theme, value);
		}

		FontFamily selectedFont;

		public FontFamily SelectedFont {
			get => selectedFont;
			set => SetProperty(ref selectedFont, value);
		}

		double selectedFontSize;

		public double SelectedFontSize {
			get => selectedFontSize;
			set => SetProperty(ref selectedFontSize, value);
		}

		bool showLineNumbers;

		public bool ShowLineNumbers {
			get => showLineNumbers;
			set => SetProperty(ref showLineNumbers, value);
		}

		bool showMetadataTokens;

		public bool ShowMetadataTokens {
			get => showMetadataTokens;
			set => SetProperty(ref showMetadataTokens, value);
		}

		bool showMetadataTokensInBase10;

		public bool ShowMetadataTokensInBase10 {
			get => showMetadataTokensInBase10;
			set => SetProperty(ref showMetadataTokensInBase10, value);
		}

		bool enableWordWrap;

		public bool EnableWordWrap {
			get => enableWordWrap;
			set => SetProperty(ref enableWordWrap, value);
		}

		bool sortResults;

		public bool SortResults {
			get => sortResults;
			set => SetProperty(ref sortResults, value);
		}

		bool foldBraces;

		public bool FoldBraces {
			get => foldBraces;
			set => SetProperty(ref foldBraces, value);
		}

		bool expandMemberDefinitions;

		public bool ExpandMemberDefinitions {
			get => expandMemberDefinitions;
			set => SetProperty(ref expandMemberDefinitions, value);
		}

		bool expandUsingDeclarations;

		public bool ExpandUsingDeclarations {
			get => expandUsingDeclarations;
			set => SetProperty(ref expandUsingDeclarations, value);
		}

		bool showDebugInfo;

		public bool ShowDebugInfo {
			get => showDebugInfo;
			set => SetProperty(ref showDebugInfo, value);
		}

		bool indentationUseTabs;

		public bool IndentationUseTabs {
			get => indentationUseTabs;
			set => SetProperty(ref indentationUseTabs, value);
		}

		int indentationTabSize;

		public int IndentationTabSize {
			get => indentationTabSize;
			set => SetProperty(ref indentationTabSize, value);
		}

		int indentationSize;

		public int IndentationSize {
			get => indentationSize;
			set => SetProperty(ref indentationSize, value);
		}

		bool highlightMatchingBraces;

		public bool HighlightMatchingBraces {
			get => highlightMatchingBraces;
			set => SetProperty(ref highlightMatchingBraces, value);
		}

		bool highlightCurrentLine;

		public bool HighlightCurrentLine {
			get => highlightCurrentLine;
			set => SetProperty(ref highlightCurrentLine, value);
		}

		bool hideEmptyMetadataTables;

		public bool HideEmptyMetadataTables {
			get => hideEmptyMetadataTables;
			set => SetProperty(ref hideEmptyMetadataTables, value);
		}

		bool useNestedNamespaceNodes;

		public bool UseNestedNamespaceNodes {
			get => useNestedNamespaceNodes;
			set => SetProperty(ref useNestedNamespaceNodes, value);
		}

		private bool styleWindowTitleBar;

		public bool StyleWindowTitleBar {
			get => styleWindowTitleBar;
			set => SetProperty(ref styleWindowTitleBar, value);
		}

		private bool showRawOffsetsAndBytesBeforeInstruction;

		public bool ShowRawOffsetsAndBytesBeforeInstruction {
			get => showRawOffsetsAndBytesBeforeInstruction;
			set => SetProperty(ref showRawOffsetsAndBytesBeforeInstruction, value);
		}

		public void CopyValues(DisplaySettings s)
		{
			this.Theme = s.Theme;
			this.SelectedFont = s.selectedFont;
			this.SelectedFontSize = s.selectedFontSize;
			this.ShowLineNumbers = s.showLineNumbers;
			this.ShowMetadataTokens = s.showMetadataTokens;
			this.ShowMetadataTokensInBase10 = s.showMetadataTokensInBase10;
			this.ShowDebugInfo = s.showDebugInfo;
			this.EnableWordWrap = s.enableWordWrap;
			this.SortResults = s.sortResults;
			this.FoldBraces = s.foldBraces;
			this.ExpandMemberDefinitions = s.expandMemberDefinitions;
			this.ExpandUsingDeclarations = s.expandUsingDeclarations;
			this.IndentationUseTabs = s.indentationUseTabs;
			this.IndentationTabSize = s.indentationTabSize;
			this.IndentationSize = s.indentationSize;
			this.HighlightMatchingBraces = s.highlightMatchingBraces;
			this.HighlightCurrentLine = s.highlightCurrentLine;
			this.HideEmptyMetadataTables = s.hideEmptyMetadataTables;
			this.UseNestedNamespaceNodes = s.useNestedNamespaceNodes;
			this.ShowRawOffsetsAndBytesBeforeInstruction = s.showRawOffsetsAndBytesBeforeInstruction;
			this.StyleWindowTitleBar = s.styleWindowTitleBar;
		}

		public static DisplaySettings Load(ILSpySettings settings, SessionSettings sessionSettings = null)
		{
			XElement e = settings["DisplaySettings"];
			var s = new DisplaySettings {
				SelectedFont = new FontFamily((string)e.Attribute("Font") ?? "Consolas"),
				SelectedFontSize = (double?)e.Attribute("FontSize") ?? 10.0 * 4 / 3,
				ShowLineNumbers = (bool?)e.Attribute("ShowLineNumbers") ?? false,
				ShowMetadataTokens = (bool?)e.Attribute("ShowMetadataTokens") ?? false,
				ShowMetadataTokensInBase10 = (bool?)e.Attribute("ShowMetadataTokensInBase10") ?? false,
				ShowDebugInfo = (bool?)e.Attribute("ShowDebugInfo") ?? false,
				EnableWordWrap = (bool?)e.Attribute("EnableWordWrap") ?? false,
				SortResults = (bool?)e.Attribute("SortResults") ?? true,
				FoldBraces = (bool?)e.Attribute("FoldBraces") ?? false,
				ExpandMemberDefinitions = (bool?)e.Attribute("ExpandMemberDefinitions") ?? false,
				ExpandUsingDeclarations = (bool?)e.Attribute("ExpandUsingDeclarations") ?? false,
				IndentationUseTabs = (bool?)e.Attribute("IndentationUseTabs") ?? true,
				IndentationSize = (int?)e.Attribute("IndentationSize") ?? 4,
				IndentationTabSize = (int?)e.Attribute("IndentationTabSize") ?? 4,
				HighlightMatchingBraces = (bool?)e.Attribute("HighlightMatchingBraces") ?? true,
				HighlightCurrentLine = (bool?)e.Attribute("HighlightCurrentLine") ?? false,
				HideEmptyMetadataTables = (bool?)e.Attribute("HideEmptyMetadataTables") ?? true,
				UseNestedNamespaceNodes = (bool?)e.Attribute("UseNestedNamespaceNodes") ?? false,
				ShowRawOffsetsAndBytesBeforeInstruction = (bool?)e.Attribute("ShowRawOffsetsAndBytesBeforeInstruction") ?? false,
				StyleWindowTitleBar = (bool?)e.Attribute("StyleWindowTitleBar") ?? false,
				Theme = (sessionSettings ?? SettingsService.Instance.SessionSettings).Theme
			};

			return s;
		}

		public void Save(XElement root)
		{
			var s = this;

			var section = new XElement("DisplaySettings");
			section.SetAttributeValue("Font", s.SelectedFont.Source);
			section.SetAttributeValue("FontSize", s.SelectedFontSize);
			section.SetAttributeValue("ShowLineNumbers", s.ShowLineNumbers);
			section.SetAttributeValue("ShowMetadataTokens", s.ShowMetadataTokens);
			section.SetAttributeValue("ShowMetadataTokensInBase10", s.ShowMetadataTokensInBase10);
			section.SetAttributeValue("ShowDebugInfo", s.ShowDebugInfo);
			section.SetAttributeValue("EnableWordWrap", s.EnableWordWrap);
			section.SetAttributeValue("SortResults", s.SortResults);
			section.SetAttributeValue("FoldBraces", s.FoldBraces);
			section.SetAttributeValue("ExpandMemberDefinitions", s.ExpandMemberDefinitions);
			section.SetAttributeValue("ExpandUsingDeclarations", s.ExpandUsingDeclarations);
			section.SetAttributeValue("IndentationUseTabs", s.IndentationUseTabs);
			section.SetAttributeValue("IndentationSize", s.IndentationSize);
			section.SetAttributeValue("IndentationTabSize", s.IndentationTabSize);
			section.SetAttributeValue("HighlightMatchingBraces", s.HighlightMatchingBraces);
			section.SetAttributeValue("HighlightCurrentLine", s.HighlightCurrentLine);
			section.SetAttributeValue("HideEmptyMetadataTables", s.HideEmptyMetadataTables);
			section.SetAttributeValue("UseNestedNamespaceNodes", s.UseNestedNamespaceNodes);
			section.SetAttributeValue("ShowRawOffsetsAndBytesBeforeInstruction", s.ShowRawOffsetsAndBytesBeforeInstruction);
			section.SetAttributeValue("StyleWindowTitleBar", s.StyleWindowTitleBar);

			SettingsService.Instance.SessionSettings.Theme = s.Theme;
			var sessionSettings = SettingsService.Instance.SessionSettings.ToXml();

			SettingsService.Instance.DisplaySettings.CopyValues(s);

			Update(section);
			Update(sessionSettings);

			void Update(XElement element)
			{
				var existingElement = root.Element(element.Name);
				if (existingElement != null)
					existingElement.ReplaceWith(element);
				else
					root.Add(element);
			}
		}
	}
}
