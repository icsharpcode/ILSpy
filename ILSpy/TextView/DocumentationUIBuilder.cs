// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Builds a FlowDocument for XML documentation.
	/// </summary>
	public class DocumentationUIBuilder
	{
		FlowDocument flowDocument;
		BlockCollection blockCollection;
		InlineCollection inlineCollection;
		IAmbience ambience;

		public DocumentationUIBuilder(IAmbience ambience)
		{
			this.ambience = ambience;
			this.flowDocument = new FlowDocument();
			this.blockCollection = flowDocument.Blocks;

			this.ShowSummary = true;
			this.ShowAllParameters = true;
			this.ShowReturns = true;
			this.ShowThreadSafety = true;
			this.ShowExceptions = true;
			this.ShowTypeParameters = true;

			this.ShowExample = true;
			this.ShowPreliminary = true;
			this.ShowSeeAlso = true;
			this.ShowValue = true;
			this.ShowPermissions = true;
			this.ShowRemarks = true;
		}

		public FlowDocument CreateFlowDocument()
		{
			FlushAddedText(true);
			flowDocument.FontSize = DisplaySettingsPanel.CurrentDisplaySettings.SelectedFontSize;
			return flowDocument;
		}

		public bool ShowExceptions { get; set; }
		public bool ShowPermissions { get; set; }
		public bool ShowExample { get; set; }
		public bool ShowPreliminary { get; set; }
		public bool ShowRemarks { get; set; }
		public bool ShowSummary { get; set; }
		public bool ShowReturns { get; set; }
		public bool ShowSeeAlso { get; set; }
		public bool ShowThreadSafety { get; set; }
		public bool ShowTypeParameters { get; set; }
		public bool ShowValue { get; set; }
		public bool ShowAllParameters { get; set; }

		/// <summary>
		/// Gets/Sets the name of the parameter that should be shown.
		/// </summary>
		public string ParameterName { get; set; }

		public void AddDocumentationElement(XNode node)
		{
			if (node == null)
				throw new ArgumentNullException(nameof(node));
			if (node is XText text) {
				AddText(text.Value);
				return;
			}
			if (!(node is XElement element))
				throw new NotImplementedException();
			switch (element.Name.ToString()) {
				case "b":
					AddSpan(new Bold(), element.Elements());
					break;
				case "i":
					AddSpan(new Italic(), element.ele);
					break;
				case "c":
					AddSpan(new Span { FontFamily = GetCodeFont() }, element.Children);
					break;
				case "code":
					AddCodeBlock(element.Value);
					break;
				case "example":
					if (ShowExample)
						AddSection("Example: ", element.Children);
					break;
				case "exception":
					if (ShowExceptions)
						AddException(element.ReferencedEntity, element.Children);
					break;
				case "list":
					AddList(element.GetAttribute("type"), element.Children);
					break;
				//case "note":
				//	throw new NotImplementedException();
				case "para":
					AddParagraph(new Paragraph { Margin = new Thickness(0, 5, 0, 5) }, element.Children);
					break;
				case "param":
					if (ShowAllParameters || (ParameterName != null && ParameterName == element.GetAttribute("name")))
						AddParam(element.GetAttribute("name"), element.Children);
					break;
				case "paramref":
					AddParamRef(element.GetAttribute("name"));
					break;
				case "permission":
					if (ShowPermissions)
						AddPermission(element.ReferencedEntity, element.Children);
					break;
				case "preliminary":
					if (ShowPreliminary)
						AddPreliminary(element.Children);
					break;
				case "remarks":
					if (ShowRemarks)
						AddSection("Remarks: ", element.Children);
					break;
				case "returns":
					if (ShowReturns)
						AddSection("Returns: ", element.Children);
					break;
				case "see":
					AddSee(element);
					break;
				case "seealso":
					if (inlineCollection != null)
						AddSee(element);
					else if (ShowSeeAlso)
						AddSection(new Run("See also: "), () => AddSee(element));
					break;
				case "summary":
					if (ShowSummary)
						AddSection("Summary: ", element.Children);
					break;
				case "threadsafety":
					if (ShowThreadSafety)
						AddThreadSafety(ParseBool(element.GetAttribute("static")), ParseBool(element.GetAttribute("instance")), element.Children);
					break;
				case "typeparam":
					if (ShowTypeParameters)
						AddSection("Type parameter " + element.GetAttribute("name") + ": ", element.Children);
					break;
				case "typeparamref":
					AddText(element.GetAttribute("name"));
					break;
				case "value":
					if (ShowValue)
						AddSection("Value: ", element.Children);
					break;
				case "exclude":
				case "filterpriority":
				case "overloads":
					// ignore children
					break;
				case "br":
					AddLineBreak();
					break;
				default:
					foreach (var child in element.Children)
						AddDocumentationElement(child);
					break;
			}
		}

		void AddList(string type, IEnumerable<XNode> items)
		{
			List list = new List();
			AddBlock(list);
			list.Margin = new Thickness(0, 5, 0, 5);
			if (type == "number")
				list.MarkerStyle = TextMarkerStyle.Decimal;
			else if (type == "bullet")
				list.MarkerStyle = TextMarkerStyle.Disc;
			var oldBlockCollection = blockCollection;
			try {
				foreach (var itemElement in items) {
					if (itemElement.Name == "listheader" || itemElement.Name == "item") {
						ListItem item = new ListItem();
						blockCollection = item.Blocks;
						inlineCollection = null;
						foreach (var prop in itemElement.Children) {
							AddDocumentationElement(prop);
						}
						FlushAddedText(false);
						list.ListItems.Add(item);
					}
				}
			} finally {
				blockCollection = oldBlockCollection;
			}
		}

		public void AddCodeBlock(string textContent, bool keepLargeMargin = false)
		{
			var document = new ReadOnlyDocument(textContent);
			var highlightingDefinition = HighlightingManager.Instance.GetDefinition("C#");

			var block = DocumentPrinter.ConvertTextDocumentToBlock(document, highlightingDefinition);
			block.FontFamily = GetCodeFont();
			if (!keepLargeMargin)
				block.Margin = new Thickness(0, 6, 0, 6);
			AddBlock(block);
		}

		public void AddSignatureBlock(string signature, int currentParameterOffset, int currentParameterLength, string currentParameterName)
		{
			ParameterName = currentParameterName;
			var document = new ReadOnlyDocument(signature);
			var highlightingDefinition = HighlightingManager.Instance.GetDefinition("C#");

			var richText = DocumentPrinter.ConvertTextDocumentToRichText(document, highlightingDefinition).ToRichTextModel();
			richText.SetFontWeight(currentParameterOffset, currentParameterLength, FontWeights.Bold);
			var block = new Paragraph();
			block.Inlines.AddRange(richText.CreateRuns(document));
			block.FontFamily = GetCodeFont();
			block.TextAlignment = TextAlignment.Left;
			AddBlock(block);
		}

		bool? ParseBool(string input)
		{
			bool result;
			if (bool.TryParse(input, out result))
				return result;
			else
				return null;
		}

		void AddThreadSafety(bool? staticThreadSafe, bool? instanceThreadSafe, IEnumerable<XNode> children)
		{
			AddSection(
				new Run("Thread-safety: "),
				delegate {
					if (staticThreadSafe == true)
						AddText("Any public static members of this type are thread safe. ");
					else if (staticThreadSafe == false)
						AddText("The static members of this type are not thread safe. ");

					if (instanceThreadSafe == true)
						AddText("Any public instance members of this type are thread safe. ");
					else if (instanceThreadSafe == false)
						AddText("Any instance members are not guaranteed to be thread safe. ");

					foreach (var child in children)
						AddDocumentationElement(child);
				});
		}

		FontFamily GetCodeFont()
		{
			return new FontFamily(SD.EditorControlService.GlobalOptions.FontFamily);
		}

		void AddException(IEntity referencedEntity, IList<XNode> children)
		{
			Span span = new Span();
			if (referencedEntity != null)
				span.Inlines.Add(ConvertReference(referencedEntity));
			else
				span.Inlines.Add("Exception");
			span.Inlines.Add(": ");
			AddSection(span, children);
		}


		void AddPermission(IEntity referencedEntity, IList<XNode> children)
		{
			Span span = new Span();
			span.Inlines.Add("Permission");
			if (referencedEntity != null) {
				span.Inlines.Add(" ");
				span.Inlines.Add(ConvertReference(referencedEntity));
			}
			span.Inlines.Add(": ");
			AddSection(span, children);
		}

		Inline ConvertReference(IEntity referencedEntity)
		{
			var h = new Hyperlink(new Run(ambience.ConvertSymbol(referencedEntity)));
			h.Click += CreateNavigateOnClickHandler(referencedEntity);
			return h;
		}

		void AddParam(string name, IEnumerable<XNode> children)
		{
			Span span = new Span();
			span.Inlines.Add(new Run(name ?? string.Empty) { FontStyle = FontStyles.Italic });
			span.Inlines.Add(": ");
			AddSection(span, children);
		}

		void AddParamRef(string name)
		{
			if (name != null) {
				AddInline(new Run(name) { FontStyle = FontStyles.Italic });
			}
		}

		void AddPreliminary(IEnumerable<XNode> children)
		{
			if (children.Any()) {
				foreach (var child in children)
					AddDocumentationElement(child);
			} else {
				AddText("[This is preliminary documentation and subject to change.]");
			}
		}

		void AddSee(XNode element)
		{
			IEntity referencedEntity = element.ReferencedEntity;
			if (referencedEntity != null) {
				if (element.Children.Any()) {
					Hyperlink link = new Hyperlink();
					link.Click += CreateNavigateOnClickHandler(referencedEntity);
					AddSpan(link, element.Children);
				} else {
					AddInline(ConvertReference(referencedEntity));
				}
			} else if (element.GetAttribute("langword") != null) {
				AddInline(new Run(element.GetAttribute("langword")) { FontFamily = GetCodeFont() });
			} else if (element.GetAttribute("href") != null) {
				Uri uri;
				if (Uri.TryCreate(element.GetAttribute("href"), UriKind.Absolute, out uri)) {
					if (element.Children.Any()) {
						AddSpan(new Hyperlink { NavigateUri = uri }, element.Children);
					} else {
						AddInline(new Hyperlink(new Run(element.GetAttribute("href"))) { NavigateUri = uri });
					}
				}
			} else {
				// Invalid reference: print the cref value
				AddText(element.GetAttribute("cref"));
			}
		}

		RoutedEventHandler CreateNavigateOnClickHandler(IEntity referencedEntity)
		{
			// Don't let the anonymous method capture the referenced entity
			// (we don't want to keep the whole compilation in memory)
			// Use the IEntityModel instead.
			var model = referencedEntity.GetModel();
			return delegate (object sender, RoutedEventArgs e) {
				IEntity resolvedEntity = model != null ? model.Resolve() : null;
				if (resolvedEntity != null) {
					bool shouldDisplayHelp = CodeCompletionOptions.TooltipLinkTarget == TooltipLinkTarget.Documentation
						&& resolvedEntity.ParentAssembly.IsPartOfDotnetFramework();
					if (!shouldDisplayHelp || !HelpProvider.ShowHelp(resolvedEntity))
						NavigationService.NavigateTo(resolvedEntity);
				}
				e.Handled = true;
			};
		}

		void AddSection(string title, IEnumerable<XNode> children)
		{
			AddSection(new Run(title), children);
		}

		void AddSection(Inline title, IEnumerable<XNode> children)
		{
			AddSection(
				title, delegate {
					foreach (var child in children)
						AddDocumentationElement(child);
				});
		}

		void AddSection(Inline title, Action addChildren)
		{
			var section = new Section();
			AddBlock(section);
			var oldBlockCollection = blockCollection;
			try {
				blockCollection = section.Blocks;
				inlineCollection = null;

				if (title != null)
					AddInline(new Bold(title));

				addChildren();
				FlushAddedText(false);
			} finally {
				blockCollection = oldBlockCollection;
				inlineCollection = null;
			}
		}

		void AddParagraph(Paragraph para, IEnumerable<XNode> children)
		{
			AddBlock(para);
			try {
				inlineCollection = para.Inlines;

				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			} finally {
				inlineCollection = null;
			}
		}

		void AddSpan(Span span, IEnumerable<XNode> children)
		{
			AddInline(span);
			var oldInlineCollection = inlineCollection;
			try {
				inlineCollection = span.Inlines;
				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			} finally {
				inlineCollection = oldInlineCollection;
			}
		}

		public void AddInline(Inline inline)
		{
			FlushAddedText(false);
			if (inlineCollection == null) {
				var para = new Paragraph();
				para.Margin = new Thickness(0, 0, 0, 5);
				inlineCollection = para.Inlines;
				AddBlock(para);
			}
			inlineCollection.Add(inline);
			ignoreWhitespace = false;
		}

		public void AddBlock(Block block)
		{
			FlushAddedText(true);
			blockCollection.Add(block);
		}

		StringBuilder addedText = new StringBuilder();
		bool ignoreWhitespace;

		public void AddLineBreak()
		{
			TrimEndOfAddedText();
			addedText.AppendLine();
			ignoreWhitespace = true;
		}

		public void AddText(string textContent)
		{
			if (string.IsNullOrEmpty(textContent))
				return;
			for (int i = 0; i < textContent.Length; i++) {
				char c = textContent[i];
				if (c == '\n' && IsEmptyLineBefore(textContent, i)) {
					AddLineBreak(); // empty line -> line break
				} else if (char.IsWhiteSpace(c)) {
					// any whitespace sequence gets converted to a single space (like HTML)
					if (!ignoreWhitespace) {
						addedText.Append(' ');
						ignoreWhitespace = true;
					}
				} else {
					addedText.Append(c);
					ignoreWhitespace = false;
				}
			}
		}

		bool IsEmptyLineBefore(string text, int i)
		{
			// Skip previous whitespace
			do {
				i--;
			} while (i >= 0 && (text[i] == ' ' || text[i] == '\r'));
			// Check if previous non-whitespace char is \n
			return i >= 0 && text[i] == '\n';
		}

		void TrimEndOfAddedText()
		{
			while (addedText.Length > 0 && addedText[addedText.Length - 1] == ' ') {
				addedText.Length--;
			}
		}

		void FlushAddedText(bool trim)
		{
			if (trim) // trim end of current text element
				TrimEndOfAddedText();
			if (addedText.Length == 0)
				return;
			string text = addedText.ToString();
			addedText.Length = 0;
			AddInline(new Run(text));
			ignoreWhitespace = trim; // trim start of next text element
		}
	}
}
