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
using System.IO;
using System.Text;
using System.Text.Json;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Sniffs resource payloads for displayable text: decides text vs binary and picks the
	/// file extension used to look up syntax highlighting (XML, JSON, ...) for the text view.
	/// </summary>
	public static class TextResourceDetector
	{
		/// <summary>
		/// Payloads larger than this are never rendered as text (the text view would choke on
		/// them anyway); they keep the byte-count-plus-Save presentation.
		/// </summary>
		public const long MaxDisplayableLength = 10 * 1024 * 1024;

		/// <summary>
		/// Maps a resource name's extension to the extension used for the highlighting lookup.
		/// Values must be extensions some definition is actually registered under: ".xml" /
		/// ".cs" are ILSpy's own XSHDs (see HighlightingService), the rest are AvaloniaEdit
		/// built-ins. Empty string = display as plain text.
		/// </summary>
		static readonly Dictionary<string, string> highlightExtensionByFileExtension = new(StringComparer.OrdinalIgnoreCase) {
			// XML family
			[".xml"] = ".xml",
			[".xsd"] = ".xml",
			[".xsl"] = ".xml",
			[".xslt"] = ".xml",
			[".xaml"] = ".xml",
			[".axaml"] = ".xml",
			[".config"] = ".xml",
			[".manifest"] = ".xml",
			[".resx"] = ".xml",
			[".nuspec"] = ".xml",
			[".targets"] = ".xml",
			[".props"] = ".xml",
			[".proj"] = ".xml",
			[".csproj"] = ".xml",
			[".vbproj"] = ".xml",
			[".svg"] = ".xml",
			[".settings"] = ".xml",
			[".wsdl"] = ".xml",
			[".xshd"] = ".xml",
			// One representative extension per AvaloniaEdit built-in definition
			[".json"] = ".json",
			[".cs"] = ".cs",
			[".js"] = ".js",
			[".css"] = ".css",
			[".htm"] = ".html",
			[".html"] = ".html",
			[".md"] = ".md",
			[".py"] = ".py",
			[".pyw"] = ".py",
			[".sql"] = ".sql",
			[".ps1"] = ".ps1",
			[".psm1"] = ".ps1",
			[".psd1"] = ".ps1",
			[".vb"] = ".vb",
			[".c"] = ".cpp",
			[".h"] = ".cpp",
			[".cc"] = ".cpp",
			[".cpp"] = ".cpp",
			[".hpp"] = ".cpp",
			[".java"] = ".java",
			[".php"] = ".php",
			[".patch"] = ".patch",
			[".diff"] = ".patch",
			// Known text formats without a highlighting definition
			[".txt"] = "",
			[".ini"] = "",
			[".cfg"] = "",
			[".conf"] = "",
			[".log"] = "",
			[".csv"] = "",
			[".tsv"] = "",
			[".yml"] = "",
			[".yaml"] = "",
		};

		/// <summary>
		/// Returns the payload decoded as text plus the matching highlighting extension, or
		/// null when the payload is binary, empty, or too large to render. The stream must be
		/// seekable; its position is reset before reading.
		/// </summary>
		public static TextResourceContent? TryDetectText(string resourceName, Stream stream)
		{
			ArgumentNullException.ThrowIfNull(resourceName);
			ArgumentNullException.ThrowIfNull(stream);

			if (!stream.CanSeek || stream.Length == 0 || stream.Length > MaxDisplayableLength)
				return null;
			stream.Position = 0;

			string? text = TryDecode(stream);
			if (text == null || text.Length == 0)
				return null;

			string extension = Path.GetExtension(resourceName);
			if (highlightExtensionByFileExtension.TryGetValue(extension, out var syntaxExtension))
				return new TextResourceContent(text, syntaxExtension);
			return new TextResourceContent(text, SniffSyntax(text));
		}

		/// <summary>
		/// Decodes the stream as text, or returns null when the payload is not text. A BOM
		/// selects UTF-8/16/32; without one, only strict UTF-8 is attempted (which also accepts
		/// plain ASCII). Any remaining control character outside \t \n \r \f (including NUL and
		/// the U+FFFD replacement char the UTF-16/32 decoders substitute for invalid units)
		/// classifies the payload as binary.
		/// </summary>
		static string? TryDecode(Stream stream)
		{
			var strictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
			string text;
			try
			{
				using var reader = new StreamReader(stream, strictUtf8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
				text = reader.ReadToEnd();
			}
			catch (DecoderFallbackException)
			{
				return null;
			}

			foreach (char c in text)
			{
				if (c == '\uFFFD')
					return null;
				if (c < ' ' && c != '\t' && c != '\n' && c != '\r' && c != '\f')
					return null;
				if (c == '\u007F')
					return null;
			}
			return text;
		}

		/// <summary>
		/// Content-based format detection for names without a recognized extension: XML/HTML by
		/// the leading angle bracket, JSON by actually parsing. Anything else is plain text.
		/// </summary>
		static string SniffSyntax(string text)
		{
			var trimmed = text.AsSpan().TrimStart();
			if (trimmed.IsEmpty)
				return "";

			if (trimmed[0] == '<')
			{
				if (trimmed.StartsWith("<!DOCTYPE html", StringComparison.OrdinalIgnoreCase)
					|| trimmed.StartsWith("<html", StringComparison.OrdinalIgnoreCase))
				{
					return ".html";
				}
				return ".xml";
			}

			if (trimmed[0] == '{' || trimmed[0] == '[')
			{
				try
				{
					using var _ = JsonDocument.Parse(text, new JsonDocumentOptions {
						AllowTrailingCommas = true,
						CommentHandling = JsonCommentHandling.Skip,
					});
					return ".json";
				}
				catch (JsonException)
				{
				}
			}

			return "";
		}
	}

	/// <summary>
	/// A resource payload decoded as text, plus the file extension driving the syntax
	/// highlighting lookup (empty string = plain text, no highlighting).
	/// </summary>
	public sealed record TextResourceContent(string Text, string SyntaxExtension);
}
