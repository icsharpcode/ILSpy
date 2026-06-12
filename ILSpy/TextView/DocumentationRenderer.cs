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
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Renders an entity signature plus its XML documentation into an Avalonia control tree
	/// suitable for hosting inside a hover popup. Output is a tree of
	/// <see cref="SelectableTextBlock"/> + <see cref="StackPanel"/> children.
	/// </summary>
	public sealed class DocumentationRenderer
	{
		static readonly IBrush HyperlinkBrush = new SolidColorBrush(Color.FromRgb(0x00, 0x66, 0xCC));

		readonly IAmbience ambience;
		readonly FontFamily codeFont;
		readonly double fontSize;

		readonly StackPanel rootStack;
		Panel currentPanel;
		InlineCollection? inlineCollection;
		readonly StringBuilder addedText = new();
		bool ignoreWhitespace;

		public DocumentationRenderer(IAmbience ambience, FontFamily codeFont, double fontSize)
		{
			this.ambience = ambience;
			this.codeFont = codeFont;
			this.fontSize = fontSize;

			rootStack = new StackPanel { Orientation = Orientation.Vertical };
			currentPanel = rootStack;

			ShowSummary = true;
			ShowAllParameters = true;
			ShowReturns = true;
			ShowThreadSafety = true;
			ShowExceptions = true;
			ShowTypeParameters = true;
			ShowExample = true;
			ShowPreliminary = true;
			ShowSeeAlso = true;
			ShowValue = true;
			ShowPermissions = true;
			ShowRemarks = true;
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

		public string? ParameterName { get; set; }

		/// <summary>
		/// Raised after the user follows any link in the rendered documentation, so a
		/// hosting popup can close itself while the navigation takes effect.
		/// </summary>
		public event EventHandler? LinkClicked;

		/// <summary>
		/// Wraps the accumulated content in a chrome border + scroll viewer and returns the
		/// final control tree for use in a popup.
		/// </summary>
		public Control CreateView(double maxWidth = 900, double maxHeight = 400)
		{
			FlushAddedText(true);

			var scroll = new ScrollViewer {
				Content = rootStack,
				HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
				VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
				MaxHeight = maxHeight,
			};
			return new Border {
				BorderThickness = new Thickness(1),
				BorderBrush = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA)),
				Background = new SolidColorBrush(Color.FromRgb(0xFC, 0xFC, 0xFC)),
				Padding = new Thickness(6),
				MaxWidth = maxWidth,
				Child = scroll,
			};
		}

		public void AddSignatureBlock(RichText signature)
		{
			ArgumentNullException.ThrowIfNull(signature);
			FlushAddedText(true);
			inlineCollection = null;

			// Wrap rather than NoWrap so a signature wider than the popup's MaxWidth folds
			// to multiple lines instead of being clipped at the right edge — the outer
			// ScrollViewer disables horizontal scrolling, so NoWrap meant "the right half
			// of long generic method signatures is invisible". WPF parity: the equivalent
			// FlowDocument Paragraph wraps by default (Paragraph doesn't even support NoWrap).
			var block = new SelectableTextBlock {
				FontFamily = codeFont,
				FontSize = fontSize,
				TextWrapping = TextWrapping.Wrap,
				Margin = new Thickness(0, 0, 0, 6),
				Inlines = new InlineCollection(),
			};
			AppendRichText(block.Inlines!, signature);
			currentPanel.Children.Add(block);
		}

		public void AddXmlDocumentation(string xmlDocumentation, IEntity? declaringEntity, Func<string, IEntity?>? resolver)
		{
			if (xmlDocumentation == null)
				return;
			try
			{
				var xml = XElement.Parse("<doc>" + xmlDocumentation + "</doc>");
				AddDocumentationElement(new XmlDocumentationElement(xml, declaringEntity, resolver));
			}
			catch (XmlException)
			{
				// Malformed XML in the .xml file — ignore as WPF does.
			}
		}

		public void AddDocumentationElement(XmlDocumentationElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
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
					AddSpan(new Span { FontFamily = codeFont }, element.Children);
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
				case "para":
					AddParagraph(element.Children, new Thickness(0, 5, 0, 5));
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
					AddText(element.GetAttribute("name") ?? string.Empty);
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
					Debug.WriteLine($"DocumentationRenderer: unsupported tag <{element.Name}> — rendering inner content flat.");
					foreach (var child in element.Children)
						AddDocumentationElement(child);
					break;
			}
		}

		// AvaloniaEdit ships RichText/RichTextModel/DocumentHighlighter but not WPF AvalonEdit's
		// DocumentPrinter.ConvertTextDocumentToRichText — so we render <code> blocks plain
		// monospace for now. Syntax-highlighting code blocks is a nice-to-have follow-up.
		void AddCodeBlock(string textContent)
		{
			FlushAddedText(true);
			inlineCollection = null;

			var block = new SelectableTextBlock {
				FontFamily = codeFont,
				FontSize = fontSize,
				TextWrapping = TextWrapping.NoWrap,
				Margin = new Thickness(0, 6, 0, 6),
				Text = textContent,
			};
			currentPanel.Children.Add(block);
		}

		void AddList(string? type, IList<XmlDocumentationElement> items)
		{
			FlushAddedText(true);
			var list = new StackPanel {
				Orientation = Orientation.Vertical,
				Margin = new Thickness(0, 5, 0, 5),
			};
			currentPanel.Children.Add(list);

			var oldPanel = currentPanel;
			var oldInline = inlineCollection;
			int index = 1;
			try
			{
				foreach (var itemElement in items)
				{
					if (itemElement.Name != "listheader" && itemElement.Name != "item")
						continue;
					var marker = type == "number" ? $"{index++}." : "•";
					var markerBlock = new TextBlock {
						Text = marker,
						Margin = new Thickness(0, 0, 4, 0),
					};
					var itemContent = new StackPanel { Orientation = Orientation.Vertical };
					Grid.SetColumn(markerBlock, 0);
					Grid.SetColumn(itemContent, 1);
					var row = new Grid { ColumnDefinitions = new ColumnDefinitions("16,*") };
					row.Children.Add(markerBlock);
					row.Children.Add(itemContent);
					list.Children.Add(row);

					currentPanel = itemContent;
					inlineCollection = null;
					foreach (var prop in itemElement.Children)
						AddDocumentationElement(prop);
					FlushAddedText(false);
				}
			}
			finally
			{
				currentPanel = oldPanel;
				inlineCollection = oldInline;
			}
		}

		static bool? ParseBool(string? input)
		{
			if (bool.TryParse(input, out bool result))
				return result;
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

		void AddException(IEntity? referencedEntity, IList<XmlDocumentationElement> children)
		{
			var span = new Span();
			if (referencedEntity != null)
				span.Inlines.Add(ConvertReference(referencedEntity));
			else
				span.Inlines.Add(new Run("Exception"));
			span.Inlines.Add(new Run(": "));
			AddSection(span, children);
		}

		void AddPermission(IEntity? referencedEntity, IList<XmlDocumentationElement> children)
		{
			var span = new Span();
			span.Inlines.Add(new Run("Permission"));
			if (referencedEntity != null)
			{
				span.Inlines.Add(new Run(" "));
				span.Inlines.Add(ConvertReference(referencedEntity));
			}
			span.Inlines.Add(new Run(": "));
			AddSection(span, children);
		}

		Inline ConvertReference(IEntity referencedEntity)
		{
			var linkText = CreateLinkTextBlock();
			linkText.Text = ambience.ConvertSymbol(referencedEntity);
			return CreateLink(linkText, () => MessageBus.Send(this, new NavigateToReferenceEventArgs(referencedEntity)));
		}

		// Inlines (Run/Span) are not InputElements in Avalonia, so a clickable link is an
		// embedded TextBlock inside an InlineUIContainer.
		static TextBlock CreateLinkTextBlock()
		{
			return new TextBlock {
				Foreground = HyperlinkBrush,
				TextDecorations = TextDecorations.Underline,
				Cursor = new Cursor(StandardCursorType.Hand),
				// A null background makes the TextBlock hit-test invisible — clicks would
				// fall through to the surrounding paragraph.
				Background = Brushes.Transparent,
				Classes = { "doc-link" },
			};
		}

		Inline CreateLink(TextBlock linkText, Action onClick)
		{
			// Press is handled so the surrounding SelectableTextBlock doesn't start a text
			// selection (its capture would also swallow the release); the click fires on
			// release over the link, like a button.
			bool pressed = false;
			linkText.PointerPressed += (_, e) => {
				if (!e.GetCurrentPoint(linkText).Properties.IsLeftButtonPressed)
					return;
				pressed = true;
				e.Handled = true;
			};
			linkText.PointerReleased += (_, e) => {
				if (!pressed)
					return;
				pressed = false;
				e.Handled = true;
				onClick();
				LinkClicked?.Invoke(this, EventArgs.Empty);
			};
			return new InlineUIContainer(linkText) { BaselineAlignment = BaselineAlignment.TextBottom };
		}

		/// <summary>
		/// Renders <paramref name="children"/> into a clickable link (used by
		/// <c>&lt;see&gt;</c> elements that carry their own display text).
		/// </summary>
		void AddLinkSpan(Action onClick, IList<XmlDocumentationElement> children)
		{
			var linkText = CreateLinkTextBlock();
			linkText.Inlines = new InlineCollection();
			AddInline(CreateLink(linkText, onClick));
			var oldInline = inlineCollection;
			try
			{
				inlineCollection = linkText.Inlines;
				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			}
			finally
			{
				inlineCollection = oldInline;
			}
		}

		void AddParam(string? name, IEnumerable<XmlDocumentationElement> children)
		{
			var span = new Span();
			var nameRun = new Run(name ?? string.Empty) { FontStyle = FontStyle.Italic };
			span.Inlines.Add(nameRun);
			span.Inlines.Add(new Run(": "));
			AddSection(span, children);
		}

		void AddParamRef(string? name)
		{
			if (name != null)
				AddInline(new Run(name) { FontStyle = FontStyle.Italic });
		}

		void AddPreliminary(IList<XmlDocumentationElement> children)
		{
			if (children.Count > 0)
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
			IEntity? referencedEntity = element.ReferencedEntity;
			if (referencedEntity != null)
			{
				if (element.Children.Count > 0)
				{
					AddLinkSpan(
						() => MessageBus.Send(this, new NavigateToReferenceEventArgs(referencedEntity)),
						element.Children);
				}
				else
				{
					AddInline(ConvertReference(referencedEntity));
				}
			}
			else if (element.GetAttribute("langword") != null)
			{
				AddInline(new Run(element.GetAttribute("langword")!) { FontFamily = codeFont });
			}
			else if (element.GetAttribute("href") is { } href)
			{
				if (Uri.TryCreate(href, UriKind.Absolute, out var uri))
				{
					if (element.Children.Count > 0)
					{
						AddLinkSpan(() => OpenExternalUri(uri), element.Children);
					}
					else
					{
						var linkText = CreateLinkTextBlock();
						linkText.Text = href;
						AddInline(CreateLink(linkText, () => OpenExternalUri(uri)));
					}
				}
			}
			else
			{
				// Invalid reference: print the cref value
				AddText(element.GetAttribute("cref") ?? string.Empty);
			}
		}

		void OpenExternalUri(Uri uri)
		{
			// The rendered view lives in a popup whose root is a TopLevel once shown, so
			// the launcher is resolvable from any control of the tree.
			if (TopLevel.GetTopLevel(rootStack) is { } topLevel)
				topLevel.Launcher.LaunchUriAsync(uri).HandleExceptions();
		}

		public void AddInline(Inline inline)
		{
			FlushAddedText(false);
			if (inlineCollection == null)
			{
				var para = new SelectableTextBlock {
					TextWrapping = TextWrapping.Wrap,
					Margin = new Thickness(0, 0, 0, 5),
					Inlines = new InlineCollection(),
				};
				inlineCollection = para.Inlines;
				currentPanel.Children.Add(para);
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
			AddSection(title, () => {
				foreach (var child in children)
					AddDocumentationElement(child);
			});
		}

		void AddSection(Inline title, Action addChildren)
		{
			FlushAddedText(true);
			var section = new StackPanel { Orientation = Orientation.Vertical };
			currentPanel.Children.Add(section);
			var oldPanel = currentPanel;
			var oldInline = inlineCollection;
			try
			{
				currentPanel = section;
				inlineCollection = null;
				if (title != null)
				{
					var bold = new Bold();
					bold.Inlines.Add(title);
					AddInline(bold);
				}
				addChildren();
				FlushAddedText(false);
			}
			finally
			{
				currentPanel = oldPanel;
				inlineCollection = oldInline;
			}
		}

		void AddParagraph(IEnumerable<XmlDocumentationElement> children, Thickness margin)
		{
			FlushAddedText(true);
			var para = new SelectableTextBlock {
				TextWrapping = TextWrapping.Wrap,
				Margin = margin,
				Inlines = new InlineCollection(),
			};
			currentPanel.Children.Add(para);
			var oldInline = inlineCollection;
			try
			{
				inlineCollection = para.Inlines;
				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			}
			finally
			{
				inlineCollection = oldInline;
			}
		}

		void AddSpan(Span span, IEnumerable<XmlDocumentationElement> children)
		{
			AddInline(span);
			var oldInline = inlineCollection;
			try
			{
				inlineCollection = span.Inlines;
				foreach (var child in children)
					AddDocumentationElement(child);
				FlushAddedText(false);
			}
			finally
			{
				inlineCollection = oldInline;
			}
		}

		void AddLineBreak()
		{
			TrimEndOfAddedText();
			addedText.AppendLine();
			ignoreWhitespace = true;
		}

		// Walks RichText highlighted sections and emits one Avalonia Run per
		// uniformly-coloured span.
		internal static void AppendRichText(InlineCollection inlines, RichText rich)
		{
			foreach (var section in rich.GetHighlightedSections(0, rich.Length))
			{
				var run = new Run(rich.Text.Substring(section.Offset, section.Length));
				if (section.Color is { } color)
				{
					if (color.Foreground?.GetBrush(null) is { } fg)
						run.Foreground = fg;
					if (color.Background?.GetBrush(null) is { } bg)
						run.Background = bg;
					if (color.FontWeight is { } weight)
						run.FontWeight = weight;
					if (color.FontStyle is { } style)
						run.FontStyle = style;
				}
				inlines.Add(run);
			}
		}

		void AddText(string textContent)
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

		static bool IsEmptyLineBefore(string text, int i)
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
			while (addedText.Length > 0 && addedText[^1] == ' ')
				addedText.Length--;
		}

		void FlushAddedText(bool trim)
		{
			if (trim)
				TrimEndOfAddedText();
			if (addedText.Length == 0)
				return;
			string text = addedText.ToString();
			addedText.Length = 0;
			AddInline(new Run(text));
			ignoreWhitespace = trim;
		}
	}
}
