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
using System.IO;
using System.Linq;
using System.Text;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class TextResourceDetectionTests
{
	static TextResourceContent? Detect(string name, byte[] payload)
		=> TextResourceDetector.TryDetectText(name, new MemoryStream(payload));

	static TextResourceContent? Detect(string name, string payload)
		=> Detect(name, Encoding.UTF8.GetBytes(payload));

	// ------------------------------------------------------------------
	// Text vs binary
	// ------------------------------------------------------------------

	[Test]
	public void Plain_Ascii_Text_Is_Detected_As_Plain_Text()
	{
		var result = Detect("readme.txt", "hello world\nsecond line\n");
		((object?)result).Should().NotBeNull();
		result!.Text.Should().Be("hello world\nsecond line\n");
		result.SyntaxExtension.Should().Be("", ".txt has no syntax highlighting");
	}

	[Test]
	public void Utf8_Bom_Is_Stripped_From_The_Displayed_Text()
	{
		var payload = Encoding.UTF8.GetPreamble()
			.Concat(Encoding.UTF8.GetBytes("content")).ToArray();
		var result = Detect("readme.txt", payload);
		((object?)result).Should().NotBeNull();
		result!.Text.Should().Be("content");
	}

	[Test]
	public void Utf16_LE_Bom_Text_Is_Detected()
	{
		var payload = Encoding.Unicode.GetPreamble()
			.Concat(Encoding.Unicode.GetBytes("wide text")).ToArray();
		var result = Detect("readme.txt", payload);
		((object?)result).Should().NotBeNull();
		result!.Text.Should().Be("wide text");
	}

	[Test]
	public void Png_Payload_Is_Binary()
	{
		// PNG magic followed by arbitrary bytes; the NUL and 0x89 already disqualify it.
		var payload = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x01 };
		((object?)Detect("logo.dat", payload)).Should().BeNull();
	}

	[Test]
	public void Nul_Byte_Means_Binary()
	{
		((object?)Detect("data", "abc\0def")).Should().BeNull();
	}

	[Test]
	public void Invalid_Utf8_Is_Binary()
	{
		// 0xC3 starts a two-byte sequence; 0x28 is not a valid continuation byte.
		((object?)Detect("data", new byte[] { 0x61, 0xC3, 0x28, 0x62 })).Should().BeNull();
	}

	[Test]
	public void Empty_Payload_Is_Not_Text()
	{
		((object?)Detect("empty.txt", Array.Empty<byte>())).Should().BeNull();
	}

	[Test]
	public void Oversized_Payload_Is_Rejected_Without_Reading()
	{
		using var stream = new HugeUnreadableStream();
		((object?)TextResourceDetector.TryDetectText("huge.txt", stream)).Should().BeNull();
	}

	/// <summary>
	/// Claims to be larger than the display cap and throws when anyone tries to read it,
	/// proving the size check happens before any I/O.
	/// </summary>
	sealed class HugeUnreadableStream : Stream
	{
		public override bool CanRead => true;
		public override bool CanSeek => true;
		public override bool CanWrite => false;
		public override long Length => TextResourceDetector.MaxDisplayableLength + 1;
		public override long Position { get; set; }
		public override int Read(byte[] buffer, int offset, int count)
			=> throw new InvalidOperationException("oversized payload must not be read");
		public override void Flush() { }
		public override long Seek(long offset, SeekOrigin origin) => Position;
		public override void SetLength(long value) => throw new NotSupportedException();
		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	}

	// ------------------------------------------------------------------
	// Format detection by resource-name extension
	// ------------------------------------------------------------------

	[TestCase("data.json", ".json")]
	[TestCase("README.md", ".md")]
	[TestCase("script.js", ".js")]
	[TestCase("styles.css", ".css")]
	[TestCase("page.html", ".html")]
	[TestCase("query.sql", ".sql")]
	[TestCase("setup.ps1", ".ps1")]
	[TestCase("source.py", ".py")]
	[TestCase("Template.cs", ".cs")]
	public void Known_Extensions_Select_Their_Highlighting(string name, string expectedExtension)
	{
		var result = Detect(name, "content that is not sniffable");
		((object?)result).Should().NotBeNull();
		result!.SyntaxExtension.Should().Be(expectedExtension);
	}

	[TestCase("app.xml")]
	[TestCase("strings.resx")]
	[TestCase("app.config")]
	[TestCase("icon.svg")]
	[TestCase("schema.xsd")]
	[TestCase("View.axaml")]
	public void Xml_Family_Extensions_Select_Xml_Highlighting(string name)
	{
		var result = Detect(name, "plain payload");
		((object?)result).Should().NotBeNull();
		result!.SyntaxExtension.Should().Be(".xml");
	}

	// ------------------------------------------------------------------
	// Format detection by content sniffing (unknown / missing extension)
	// ------------------------------------------------------------------

	[Test]
	public void Json_Content_Is_Sniffed_Without_Extension()
	{
		var result = Detect("payload", "{ \"key\": [1, 2, 3], \"nested\": { \"a\": true } }");
		((object?)result).Should().NotBeNull();
		result!.SyntaxExtension.Should().Be(".json");
	}

	[Test]
	public void Xml_Content_Is_Sniffed_Without_Extension()
	{
		var result = Detect("payload", "<?xml version=\"1.0\"?>\n<root><child /></root>");
		((object?)result).Should().NotBeNull();
		result!.SyntaxExtension.Should().Be(".xml");
	}

	[Test]
	public void Html_Content_Is_Sniffed_Without_Extension()
	{
		var result = Detect("payload", "<!DOCTYPE html>\n<html><body>hi</body></html>");
		((object?)result).Should().NotBeNull();
		result!.SyntaxExtension.Should().Be(".html");
	}

	[Test]
	public void Invalid_Json_Sniff_Falls_Back_To_Plain_Text()
	{
		var result = Detect("payload", "{ this is not valid json");
		((object?)result).Should().NotBeNull();
		result!.SyntaxExtension.Should().Be("");
	}

	// ------------------------------------------------------------------
	// Display integration: the generic resource nodes render detected text
	// with the matching highlighting override
	// ------------------------------------------------------------------

	static void EnsureComposition() => AppComposition.Current.GetExport<MainWindow>();

	static (AvaloniaEditTextOutput Output, Language Language) PrepareDecompile()
	{
		EnsureComposition();
		var output = new AvaloniaEditTextOutput();
		var language = AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;
		return (output, language);
	}

	[AvaloniaTest]
	public void Resources_Entry_Renders_Json_As_Bare_Highlighted_Content()
	{
		// Entries inside a .resources blob always use the generic ResourceEntryNode; a JSON
		// payload must surface as highlighted text only — no byte-count header, no Save button —
		// matching how the XML/XAML resource nodes present their content.
		var (output, language) = PrepareDecompile();
		const string json = "{ \"answer\": 42 }";
		var node = ResourceEntryNode.Create("settings.json", Encoding.UTF8.GetBytes(json));

		node.Decompile(language, output, new DecompilationOptions(new DecompilerSettings()));

		output.GetText().Should().Be(json);
		output.SyntaxExtensionOverride.Should().Be(".json");
		output.UIElements.Should().BeEmpty("text resources render bare content without the Save button");
	}

	[AvaloniaTest]
	public void Assembly_Resource_Renders_Sniffed_Text_As_Bare_Content()
	{
		// An embedded assembly resource with an unknown extension but textual content must
		// render only the text (plain, no highlighting) — header and Save button are for
		// binary payloads that have nothing else to show.
		var (output, language) = PrepareDecompile();
		const string text = "some plain notes\nwith a second line";
		var node = new ResourceTreeNode(new ByteArrayResource("notes.customext", Encoding.UTF8.GetBytes(text)));

		node.Decompile(language, output, new DecompilationOptions(new DecompilerSettings()));

		output.GetText().Should().Be(text);
		output.SyntaxExtensionOverride.Should().Be("");
		output.UIElements.Should().BeEmpty();
	}

	[AvaloniaTest]
	public void Binary_Assembly_Resource_Keeps_The_Byte_Dump_Only()
	{
		var (output, language) = PrepareDecompile();
		var payload = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x00, 0xFF, 0xFE };
		var node = new ResourceTreeNode(new ByteArrayResource("logo.dat", payload));

		node.Decompile(language, output, new DecompilationOptions(new DecompilerSettings()));

		output.SyntaxExtensionOverride.Should().BeNull();
		output.GetText().Should().Contain("7 bytes", "binary resources keep the metadata header");
		output.UIElements.Should().HaveCount(1, "binary resources keep the Save button");
	}

	[AvaloniaTest]
	public void Json_Highlighting_Definition_Is_Resolvable()
	{
		// AvaloniaEdit ships Json.xshd as a built-in; the resource text view depends on the
		// by-extension lookup finding it (and .md / .js for the other mapped formats).
		HighlightingService.GetByExtension(".json").Should().NotBeNull();
		HighlightingService.GetByExtension(".md").Should().NotBeNull();
		HighlightingService.GetByExtension(".js").Should().NotBeNull();
	}
}
