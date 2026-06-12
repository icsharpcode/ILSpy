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
using System.Text;

using Avalonia.Controls;
using Avalonia.Input;

using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Highlighting;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Copies the editor's selection to the clipboard as both plain text and syntax-coloured HTML, so
	/// pasting into an HTML-aware target (a document, mail, chat) keeps the highlighting. The previous
	/// version did this through AvalonEdit's HtmlClipboard; AvaloniaEdit exposes the fragment builder
	/// but doesn't put HTML on the clipboard, so this wires it up with the per-platform HTML format.
	/// </summary>
	public static class HtmlClipboardCopy
	{
		/// <summary>
		/// Copies the current selection as text + HTML. Returns false (and copies nothing) when the
		/// selection is empty, so the caller can fall back to the default copy.
		/// </summary>
		public static bool Copy(TextEditor editor, RichTextModel? semanticHighlighting = null)
		{
			ArgumentNullException.ThrowIfNull(editor);
			var textArea = editor.TextArea;
			if (textArea.Selection.IsEmpty)
				return false;

			var text = TextUtilities.NormalizeNewLines(textArea.Selection.GetText(), Environment.NewLine);
			var html = CreateHtmlFragmentFromSelection(editor, semanticHighlighting);

			var item = new DataTransferItem();
			item.Set(DataFormat.Text, text);
			if (html is not null && HtmlPlatformFormat() is { } htmlFormat)
				item.Set(htmlFormat, Encoding.UTF8.GetBytes(WrapForPlatform(html)));

			var transfer = new DataTransfer();
			transfer.Add(item);
			TopLevel.GetTopLevel(editor)?.Clipboard?.SetDataAsync(transfer);
			return true;
		}

		/// <summary>
		/// Builds the coloured HTML fragment for the editor's current selection (the plain inner HTML,
		/// without any platform CF_HTML wrapper). Merges both the syntax highlighter (xshd) AND ILSpy's
		/// semantic <see cref="RichTextModel"/> -- the latter carries the decompiler's reference / theme
		/// colours that the xshd highlighter alone doesn't. Returns null when there is no selection.
		/// </summary>
		public static string? CreateHtmlFragmentFromSelection(TextEditor editor, RichTextModel? semanticHighlighting = null)
		{
			ArgumentNullException.ThrowIfNull(editor);
			var textArea = editor.TextArea;
			if (textArea.Selection.IsEmpty)
				return null;

			var document = textArea.Document;
			var highlighter = textArea.TextView.GetService(typeof(IHighlighter)) as IHighlighter;
			var options = new HtmlOptions(textArea.Options);
			var html = new StringBuilder();

			foreach (var segment in textArea.Selection.Segments)
			{
				var line = document.GetLineByOffset(segment.StartOffset);
				while (line != null && line.Offset < segment.EndOffset)
				{
					if (html.Length > 0)
						html.AppendLine("<br>");

					var (offset, length) = Overlap(segment, line);
					var highlightedLine = highlighter?.HighlightLine(line.LineNumber) ?? new HighlightedLine(document, line);

					if (semanticHighlighting is not null)
					{
						// Overlay the semantic colours by merging in a line built from the model's
						// sections (same approach the previous version used).
						var semanticLine = new HighlightedLine(document, line);
						foreach (var section in semanticHighlighting.GetHighlightedSections(offset, length))
							semanticLine.Sections.Add(section);
						highlightedLine.MergeWith(semanticLine);
					}

					html.Append(highlightedLine.ToHtml(offset, offset + length, options));
					line = line.NextLine;
				}
			}
			return html.ToString();
		}

		static (int Offset, int Length) Overlap(ISegment a, ISegment b)
		{
			int start = Math.Max(a.Offset, b.Offset);
			int end = Math.Min(a.EndOffset, b.EndOffset);
			return (start, end - start);
		}

		// The native clipboard format that other apps recognise as HTML. Linux/X11 uses the MIME type,
		// macOS the UTI; Windows uses CF_HTML (the fragment must be wrapped, see WrapForPlatform).
		static DataFormat<byte[]>? HtmlPlatformFormat()
		{
			if (OperatingSystem.IsWindows())
				return DataFormat.CreateBytesPlatformFormat("HTML Format");
			if (OperatingSystem.IsMacOS())
				return DataFormat.CreateBytesPlatformFormat("public.html");
			return DataFormat.CreateBytesPlatformFormat("text/html");
		}

		// Windows CF_HTML requires a descriptive header with byte offsets; other platforms take the raw
		// fragment. The offsets are byte positions into the UTF-8 payload.
		static string WrapForPlatform(string fragment)
		{
			if (!OperatingSystem.IsWindows())
				return fragment;

			const string header =
				"Version:0.9\r\nStartHTML:{0:00000000}\r\nEndHTML:{1:00000000}\r\n" +
				"StartFragment:{2:00000000}\r\nEndFragment:{3:00000000}\r\n";
			const string htmlStart = "<html><body><!--StartFragment-->";
			const string htmlEnd = "<!--EndFragment--></body></html>";

			// Two-pass: the header length depends on the offsets it contains, but the placeholder header
			// is a fixed width (all fields are 8 digits), so a single formatted pass is exact.
			int headerLength = Encoding.UTF8.GetByteCount(string.Format(CultureInfo.InvariantCulture, header, 0, 0, 0, 0));
			int startHtml = headerLength;
			int startFragment = startHtml + Encoding.UTF8.GetByteCount(htmlStart);
			int endFragment = startFragment + Encoding.UTF8.GetByteCount(fragment);
			int endHtml = endFragment + Encoding.UTF8.GetByteCount(htmlEnd);

			return string.Format(CultureInfo.InvariantCulture, header, startHtml, endHtml, startFragment, endFragment)
				+ htmlStart + fragment + htmlEnd;
		}
	}
}
