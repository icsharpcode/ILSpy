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

using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Editor-and-tree visual preferences. Persisted under the <c>&lt;DisplaySettings/&gt;</c>
	/// XML section; the schema matches the WPF host so saved settings round-trip across
	/// platforms. Font family is a string (Avalonia's <see cref="global::Avalonia.Media.FontFamily"/>
	/// is constructed from a name at consumption sites — no WPF FontFamily here so this type
	/// stays UI-framework-free).
	/// </summary>
	public sealed partial class DisplaySettings : ObservableObject, ISettingsSection
	{
		[ObservableProperty]
		string selectedFont = "Consolas";

		[ObservableProperty]
		double selectedFontSize = 10.0 * 4 / 3;

		[ObservableProperty]
		bool showLineNumbers;

		[ObservableProperty]
		bool showMetadataTokens;

		[ObservableProperty]
		bool showMetadataTokensInBase10;

		[ObservableProperty]
		bool enableWordWrap;

		[ObservableProperty]
		bool sortResults = true;

		[ObservableProperty]
		bool foldBraces;

		[ObservableProperty]
		bool expandMemberDefinitions;

		[ObservableProperty]
		bool expandUsingDeclarations;

		[ObservableProperty]
		bool showDebugInfo;

		[ObservableProperty]
		bool indentationUseTabs = true;

		[ObservableProperty]
		int indentationTabSize = 4;

		[ObservableProperty]
		int indentationSize = 4;

		[ObservableProperty]
		bool highlightMatchingBraces = true;

		[ObservableProperty]
		bool highlightCurrentLine;

		[ObservableProperty]
		bool hideEmptyMetadataTables = true;

		[ObservableProperty]
		bool useNestedNamespaceNodes;

		[ObservableProperty]
		bool styleWindowTitleBar;

		[ObservableProperty]
		bool showRawOffsetsAndBytesBeforeInstruction;

		[ObservableProperty]
		bool decodeCustomAttributeBlobs;

		public XName SectionName => "DisplaySettings";

		public void LoadFromXml(XElement section)
		{
			SelectedFont = (string?)section.Attribute("Font") ?? "Consolas";
			SelectedFontSize = (double?)section.Attribute("FontSize") ?? 10.0 * 4 / 3;
			ShowLineNumbers = (bool?)section.Attribute(nameof(ShowLineNumbers)) ?? false;
			ShowMetadataTokens = (bool?)section.Attribute(nameof(ShowMetadataTokens)) ?? false;
			ShowMetadataTokensInBase10 = (bool?)section.Attribute(nameof(ShowMetadataTokensInBase10)) ?? false;
			ShowDebugInfo = (bool?)section.Attribute(nameof(ShowDebugInfo)) ?? false;
			EnableWordWrap = (bool?)section.Attribute(nameof(EnableWordWrap)) ?? false;
			SortResults = (bool?)section.Attribute(nameof(SortResults)) ?? true;
			FoldBraces = (bool?)section.Attribute(nameof(FoldBraces)) ?? false;
			ExpandMemberDefinitions = (bool?)section.Attribute(nameof(ExpandMemberDefinitions)) ?? false;
			ExpandUsingDeclarations = (bool?)section.Attribute(nameof(ExpandUsingDeclarations)) ?? false;
			IndentationUseTabs = (bool?)section.Attribute(nameof(IndentationUseTabs)) ?? true;
			IndentationSize = (int?)section.Attribute(nameof(IndentationSize)) ?? 4;
			IndentationTabSize = (int?)section.Attribute(nameof(IndentationTabSize)) ?? 4;
			HighlightMatchingBraces = (bool?)section.Attribute(nameof(HighlightMatchingBraces)) ?? true;
			HighlightCurrentLine = (bool?)section.Attribute(nameof(HighlightCurrentLine)) ?? false;
			HideEmptyMetadataTables = (bool?)section.Attribute(nameof(HideEmptyMetadataTables)) ?? true;
			UseNestedNamespaceNodes = (bool?)section.Attribute(nameof(UseNestedNamespaceNodes)) ?? false;
			ShowRawOffsetsAndBytesBeforeInstruction = (bool?)section.Attribute(nameof(ShowRawOffsetsAndBytesBeforeInstruction)) ?? false;
			StyleWindowTitleBar = (bool?)section.Attribute(nameof(StyleWindowTitleBar)) ?? false;
			DecodeCustomAttributeBlobs = (bool?)section.Attribute(nameof(DecodeCustomAttributeBlobs)) ?? false;
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);
			section.SetAttributeValue("Font", SelectedFont);
			section.SetAttributeValue("FontSize", SelectedFontSize);
			section.SetAttributeValue(nameof(ShowLineNumbers), ShowLineNumbers);
			section.SetAttributeValue(nameof(ShowMetadataTokens), ShowMetadataTokens);
			section.SetAttributeValue(nameof(ShowMetadataTokensInBase10), ShowMetadataTokensInBase10);
			section.SetAttributeValue(nameof(ShowDebugInfo), ShowDebugInfo);
			section.SetAttributeValue(nameof(EnableWordWrap), EnableWordWrap);
			section.SetAttributeValue(nameof(SortResults), SortResults);
			section.SetAttributeValue(nameof(FoldBraces), FoldBraces);
			section.SetAttributeValue(nameof(ExpandMemberDefinitions), ExpandMemberDefinitions);
			section.SetAttributeValue(nameof(ExpandUsingDeclarations), ExpandUsingDeclarations);
			section.SetAttributeValue(nameof(IndentationUseTabs), IndentationUseTabs);
			section.SetAttributeValue(nameof(IndentationSize), IndentationSize);
			section.SetAttributeValue(nameof(IndentationTabSize), IndentationTabSize);
			section.SetAttributeValue(nameof(HighlightMatchingBraces), HighlightMatchingBraces);
			section.SetAttributeValue(nameof(HighlightCurrentLine), HighlightCurrentLine);
			section.SetAttributeValue(nameof(HideEmptyMetadataTables), HideEmptyMetadataTables);
			section.SetAttributeValue(nameof(UseNestedNamespaceNodes), UseNestedNamespaceNodes);
			section.SetAttributeValue(nameof(ShowRawOffsetsAndBytesBeforeInstruction), ShowRawOffsetsAndBytesBeforeInstruction);
			section.SetAttributeValue(nameof(StyleWindowTitleBar), StyleWindowTitleBar);
			section.SetAttributeValue(nameof(DecodeCustomAttributeBlobs), DecodeCustomAttributeBlobs);
			return section;
		}
	}
}
