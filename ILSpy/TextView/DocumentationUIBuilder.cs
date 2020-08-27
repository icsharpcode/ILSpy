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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Renders XML documentation into a WPF <see cref="FlowDocument"/>.
	/// </summary>
	public class DocumentationUIBuilder
	{
		readonly IAmbience ambience;
		readonly IHighlightingDefinition highlightingDefinition;
		readonly FlowDocument document;
		BlockCollection blockCollection;
		InlineCollection inlineCollection;

		public DocumentationUIBuilder(IAmbience ambience, IHighlightingDefinition highlightingDefinition)
		{
			this.ambience = ambience;
			this.highlightingDefinition = highlightingDefinition;
			this.document = new FlowDocument();
			this.blockCollection = document.Blocks;

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

		public FlowDocument CreateDocument()
		{
			FlushAddedText(true);
			return document;
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

		public void AddCodeBlock(string textContent, bool keepLargeMargin = false)
		{
			var document = new TextDocument(textContent);
			var highlighter = new DocumentHighlighter(document, highlightingDefinition);
			var richText = DocumentPrinter.ConvertTextDocumentToRichText(document, highlighter).ToRichTextModel();

			var block = new Paragraph();
			block.Inlines.AddRange(richText.CreateRuns(document));
			block.FontFamily = GetCodeFont();
			if (!keepLargeMargin)
				block.Margin = new Thickness(0, 6, 0, 6);
			AddBlock(block);
		}

		public void AddSignatureBlock(string signature, RichTextModel highlighting = null)
		{
			var document = new TextDocument(signature);
			var richText = highlighting ?? DocumentPrinter.ConvertTextDocumentToRichText(document, new DocumentHighlighter(document, highlightingDefinition)).ToRichTextModel();
			var block = new Paragraph();
			// HACK: measure width of signature using a TextBlock
			// Paragraph sadly does not support TextWrapping.NoWrap
			var text = new TextBlock {
				FontFamily = GetCodeFont(),
				FontSize = DisplaySettingsPanel.CurrentDisplaySettings.SelectedFontSize,
				TextAlignment = TextAlignment.Left
			};
			text.Inlines.AddRange(richText.CreateRuns(document));
			text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			this.document.MinPageWidth = Math.Min(text.DesiredSize.Width, MainWindow.Instance.ActualWidth);
			block.Inlines.AddRange(richText.CreateRuns(document));
			block.FontFamily = GetCodeFont();
			block.TextAlignment = TextAlignment.Left;
			AddBlock(block);
		}

		public void AddXmlDocumentation(string xmlDocumentation, IEntity declaringEntity, Func<string, IEntity> resolver)
		{
			if (xmlDocumentation == null)
				return;
			Debug.WriteLine(xmlDocumentation);
			var xml = XElement.Parse("<doc>" + xmlDocumentation + "</doc>");
			AddDocumentationElement(new XmlDocumentationElement(xml, declaringEntity, resolver));
		}


		/// <summary>
		/// Gets/Sets the name of the parameter that should be shown.
		/// </summary>
		public string ParameterName { get; set; }

		public void AddDocumentationElement(XmlDocumentationElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (element.IsTextNode)
			{
				AddText(element.TextContent);
				return;
			}
			switch (element.Name)
			{
				case "b":
					AddSpan(new Bold(), element.Children);
					break;
				case "i":
					AddSpan(new Italic(), element.Children);
					break;
				case "c":
					AddSpan(new Span { FontFamily = GetCodeFont() }, element.Children);
					break;
				case "code":
					AddCodeBlock(element.TextContent);
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

		void AddList(string type, IEnumerable<XmlDocumentationElement> items)
		{
			List list = new List();
			AddBlock(list);
			list.Margin = new Thickness(0, 5, 0, 5);
			if (type == "number")
				list.MarkerStyle = TextMarkerStyle.Decimal;
			else if (type == "bullet")
				list.MarkerStyle = TextMarkerStyle.Disc;
			var oldBlockCollection = blockCollection;
			try
			{
				foreach (var itemElement in items)
				{
					if (itemElement.Name == "listheader" || itemElement.Name == "item")
					{
						ListItem item = new ListItem();
						blockCollection = item.Blocks;
						inlineCollection = null;
						foreach (var prop in itemElement.Children)
						{
							AddDocumentationElement(prop);
						}
						FlushAddedText(false);
						list.ListItems.Add(item);
					}
				}
			}
			finally
			{
				blockCollection = oldBlockCollection;
			}
		}

		bool? ParseBool(string input)
		{
			if (bool.TryParse(input, out bool result))
				return result;
			else
				return null;
		}

		void AddThreadSafety(bool? staticThreadSafe, bool? instanceThreadSafe, IEnumerable<XmlDocumentationElement> children)
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

		void AddException(IEntity referencedEntity, IList<XmlDocumentationElement> children)
		{
			Span span = new Span();
			if (referencedEntity != null)
				span.Inlines.Add(ConvertReference(referencedEntity));
			else
				span.Inlines.Add("Exception");
			span.Inlines.Add(": ");
			AddSection(span, children);
		}


		void AddPermission(IEntity referencedEntity, IList<XmlDocumentationElement> children)
		{
			Span span = new Span();
			span.Inlines.Add("Permission");
			if (referencedEntity != null)
			{
				span.Inlines.Add(" ");
				span.Inlines.Add(ConvertReference(referencedEntity));
			}
			span.Inlines.Add(": ");
			AddSection(span, children);
		}

		Inline ConvertReference(IEntity referencedEntity)
		{
			var h = new Hyperlink(new Run(ambience.ConvertSymbol(referencedEntity)));
			h.Click += (sender, e) => {
				MainWindow.Instance.JumpToReference(referencedEntity);
			};
			return h;
		}

		void AddParam(string name, IEnumerable<XmlDocumentationElement> children)
		{
			Span span = new Span();
			span.Inlines.Add(new Run(name ?? string.Empty) { FontStyle = FontStyles.Italic });
			span.Inlines.Add(": ");
			AddSection(span, children);
		}

		void AddParamRef(string name)
		{
			if (name != null)
			{
				AddInline(new Run(name) { FontStyle = FontStyles.Italic });
			}
		}

		void AddPreliminary(IEnumerable<XmlDocumentationElement> children)
		{
			if (children.Any())
			{
				foreach (var child in children)
					AddDocumentationElement(child);
			}
			else
			{
				AddText("[This is preliminary documentation and subject to change.]");
			}
		}

		void AddSee(XmlDocumentationElement element)
		{
			IEntity referencedEntity = element.ReferencedEntity;
			if (referencedEntity != null)
			{
				if (element.Children.Any())
				{
					Hyperlink link = new Hyperlink();
					link.Click += (sender, e) => {
						MainWindow.Instance.JumpToReference(referencedEntity);
					};
					AddSpan(link, element.Children);
				}
				else
				{
					AddInline(ConvertReference(referencedEntity));
				}
			}
			else if (element.GetAttribute("langword") != null)
			{
				AddInline(new Run(element.GetAttribute("langword")) { FontFamily = GetCodeFont() });
			}
			else if (element.GetAttribute("href") != null)
			{
				Uri uri;
				if (Uri.TryCreate(element.GetAttribute("href"), UriKind.Absolute, out uri))
				{
					if (element.Children.Any())
					{
						AddSpan(new Hyperlink { NavigateUri = uri }, element.Children);
					}
					else
					{
						AddInline(new Hyperlink(new Run(element.GetAttribute("href"))) { NavigateUri = uri });
					}
				}
			}
			else
			{
				// Invalid reference: print the cref value
				AddText(element.GetAttribute("cref"));
			}
		}

		static string GetCref(string cref)
		{
			if (cref == null || cref.Trim().Length == 0)
			{
				return "";
			}
			if (cref.Length < 2)
			{
				return cref;
			}
			if (cref.Substring(1, 1) == ":")
			{
				return cref.Substring(2, cref.Length - 2);
			}
			return cref;
		}

		FontFamily GetCodeFont()
		{
			return DisplaySettingsPanel.CurrentDisplaySettings.SelectedFont;
		}

		public void AddInline(Inline inline)
		{
			FlushAddedText(false);
			if (inlineCollection == null)
			{
				var para = new Paragraph();
				para.Margin = new Thickness(0, 0, 0, 5);
				inlineCollection = para.Inlines;
				AddBlock(para);
			}
			inlineCollection.Add(inline);
			ignoreWhitespace = false;
		}

		void AddSection(string title, IEnumerable<XmlDocumentationElement> children)
		{
			AddSection(new Run(title), children);
		}

		void AddSection(Inline title, IEnumerable<XmlDocumentationElement> children)
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
			try
			{
				blockCollection = section.Blocks;
				inlineCollection = null;

				if (title != null)
					AddInline(new Bold(title));

				addChildren();
				FlushAddedText(false);
			}
			finally
			{
				blockCollection = oldBlockCollection;
				inlineCollection = null;
			}
		}

		void AddParagraph(Paragraph para, IEnumerable<XmlDocumentationElement> children)
		{
			AddBlock(para);
			try
			{
				inlineCollection = para.Inlines;

				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			}
			finally
			{
				inlineCollection = null;
			}
		}

		void AddSpan(Span span, IEnumerable<XmlDocumentationElement> children)
		{
			AddInline(span);
			var oldInlineCollection = inlineCollection;
			try
			{
				inlineCollection = span.Inlines;
				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			}
			finally
			{
				inlineCollection = oldInlineCollection;
			}
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
			for (int i = 0; i < textContent.Length; i++)
			{
				char c = textContent[i];
				if (c == '\n' && IsEmptyLineBefore(textContent, i))
				{
					AddLineBreak(); // empty line -> line break
				}
				else if (char.IsWhiteSpace(c))
				{
					// any whitespace sequence gets converted to a single space (like HTML)
					if (!ignoreWhitespace)
					{
						addedText.Append(' ');
						ignoreWhitespace = true;
					}
				}
				else
				{
					addedText.Append(c);
					ignoreWhitespace = false;
				}
			}
		}

		bool IsEmptyLineBefore(string text, int i)
		{
			// Skip previous whitespace
			do
			{
				i--;
			} while (i >= 0 && (text[i] == ' ' || text[i] == '\r'));
			// Check if previous non-whitespace char is \n
			return i >= 0 && text[i] == '\n';
		}

		void TrimEndOfAddedText()
		{
			while (addedText.Length > 0 && addedText[addedText.Length - 1] == ' ')
			{
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
