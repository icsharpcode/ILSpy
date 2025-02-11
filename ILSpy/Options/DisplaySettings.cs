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

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Description of DisplaySettings.
	/// </summary>
	public class DisplaySettings : ObservableObject, ISettingsSection
	{
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

		private bool enableSmoothScrolling;
		public bool EnableSmoothScrolling {
			get => enableSmoothScrolling;
			set => SetProperty(ref enableSmoothScrolling, value);
		}

		public XName SectionName => "DisplaySettings";

		public void LoadFromXml(XElement section)
		{
			SelectedFont = new FontFamily((string)section.Attribute("Font") ?? "Consolas");
			SelectedFontSize = (double?)section.Attribute("FontSize") ?? 10.0 * 4 / 3;
			ShowLineNumbers = (bool?)section.Attribute("ShowLineNumbers") ?? false;
			ShowMetadataTokens = (bool?)section.Attribute("ShowMetadataTokens") ?? false;
			ShowMetadataTokensInBase10 = (bool?)section.Attribute("ShowMetadataTokensInBase10") ?? false;
			ShowDebugInfo = (bool?)section.Attribute("ShowDebugInfo") ?? false;
			EnableWordWrap = (bool?)section.Attribute("EnableWordWrap") ?? false;
			SortResults = (bool?)section.Attribute("SortResults") ?? true;
			FoldBraces = (bool?)section.Attribute("FoldBraces") ?? false;
			ExpandMemberDefinitions = (bool?)section.Attribute("ExpandMemberDefinitions") ?? false;
			ExpandUsingDeclarations = (bool?)section.Attribute("ExpandUsingDeclarations") ?? false;
			IndentationUseTabs = (bool?)section.Attribute("IndentationUseTabs") ?? true;
			IndentationSize = (int?)section.Attribute("IndentationSize") ?? 4;
			IndentationTabSize = (int?)section.Attribute("IndentationTabSize") ?? 4;
			HighlightMatchingBraces = (bool?)section.Attribute("HighlightMatchingBraces") ?? true;
			HighlightCurrentLine = (bool?)section.Attribute("HighlightCurrentLine") ?? false;
			HideEmptyMetadataTables = (bool?)section.Attribute("HideEmptyMetadataTables") ?? true;
			UseNestedNamespaceNodes = (bool?)section.Attribute("UseNestedNamespaceNodes") ?? false;
			ShowRawOffsetsAndBytesBeforeInstruction = (bool?)section.Attribute("ShowRawOffsetsAndBytesBeforeInstruction") ?? false;
			StyleWindowTitleBar = (bool?)section.Attribute("StyleWindowTitleBar") ?? false;
			EnableSmoothScrolling = (bool?)section.Attribute("EnableSmoothScrolling") ?? true;
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);

			section.SetAttributeValue("Font", SelectedFont.Source);
			section.SetAttributeValue("FontSize", SelectedFontSize);
			section.SetAttributeValue("ShowLineNumbers", ShowLineNumbers);
			section.SetAttributeValue("ShowMetadataTokens", ShowMetadataTokens);
			section.SetAttributeValue("ShowMetadataTokensInBase10", ShowMetadataTokensInBase10);
			section.SetAttributeValue("ShowDebugInfo", ShowDebugInfo);
			section.SetAttributeValue("EnableWordWrap", EnableWordWrap);
			section.SetAttributeValue("SortResults", SortResults);
			section.SetAttributeValue("FoldBraces", FoldBraces);
			section.SetAttributeValue("ExpandMemberDefinitions", ExpandMemberDefinitions);
			section.SetAttributeValue("ExpandUsingDeclarations", ExpandUsingDeclarations);
			section.SetAttributeValue("IndentationUseTabs", IndentationUseTabs);
			section.SetAttributeValue("IndentationSize", IndentationSize);
			section.SetAttributeValue("IndentationTabSize", IndentationTabSize);
			section.SetAttributeValue("HighlightMatchingBraces", HighlightMatchingBraces);
			section.SetAttributeValue("HighlightCurrentLine", HighlightCurrentLine);
			section.SetAttributeValue("HideEmptyMetadataTables", HideEmptyMetadataTables);
			section.SetAttributeValue("UseNestedNamespaceNodes", UseNestedNamespaceNodes);
			section.SetAttributeValue("ShowRawOffsetsAndBytesBeforeInstruction", ShowRawOffsetsAndBytesBeforeInstruction);
			section.SetAttributeValue("StyleWindowTitleBar", StyleWindowTitleBar);
			section.SetAttributeValue("EnableSmoothScrolling", EnableSmoothScrolling);

			return section;
		}
	}
}
