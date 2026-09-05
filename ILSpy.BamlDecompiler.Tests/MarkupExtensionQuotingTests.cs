// Copyright (c) 2026 Siegfried Pammer
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

using System.IO;
using System.Reflection.PortableExecutable;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.BamlDecompiler.Xaml;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// A markup extension is written back as text that the XAML parser reads again, and its
	/// grammar gives ',' '=' '{' '}' and the quote characters a meaning. A value carrying one of
	/// them has to be quoted, or the parser splits it into name/value pairs that do not exist -
	/// the reported case is DevExpress' {DXBinding Expr='...'}, whose expressions are full of
	/// commas and equals signs.
	/// </summary>
	[TestFixture]
	public class MarkupExtensionQuotingTests
	{
		static ushort TypeId(KnownTypes type) => unchecked((ushort)-(short)type);

		static ushort MemberId(KnownMembers member) => unchecked((ushort)-(short)member);

		[TestCase("plain", ExpectedResult = "plain", TestName = "AnOrdinaryValueIsLeftAlone")]
		[TestCase("with space", ExpectedResult = "with space", TestName = "InteriorWhitespaceNeedsNoQuotes")]
		[TestCase("a, b", ExpectedResult = "'a, b'", TestName = "CommaSeparatesArguments")]
		[TestCase("a = b", ExpectedResult = "'a = b'", TestName = "EqualsStartsANamedArgument")]
		[TestCase("{}{0} items", ExpectedResult = "'{}{0} items'", TestName = "AnEscapedLeadingBraceIsQuoted")]
		[TestCase("{x:Static local:Thing.Value}", ExpectedResult = "{x:Static local:Thing.Value}", TestName = "ANestedExtensionStaysOne")]
		[TestCase("Element[{http://ns}Name].Value", ExpectedResult = "Element[{http://ns}Name].Value", TestName = "BracesInsideAValueAreNotGrammar")]
		[TestCase("Date: {0:dddd, MMMM dd}", ExpectedResult = "'Date: {0:dddd, MMMM dd}'", TestName = "ACommaInsideBracesStillSeparates")]
		[TestCase("a}b", ExpectedResult = "'a}b'", TestName = "AStrayClosingBraceWouldEndTheExtension")]
		[TestCase("a{b", ExpectedResult = "'a{b'", TestName = "AStrayOpeningBraceWouldStartAnExtension")]
		[TestCase(" padded ", ExpectedResult = "' padded '", TestName = "EdgeWhitespaceWouldBeTrimmed")]
		[TestCase("", ExpectedResult = "''", TestName = "AnEmptyValueNeedsToStayEmpty")]
		[TestCase("it's", ExpectedResult = @"'it\'s'", TestName = "TheQuoteCharacterIsEscaped")]
		[TestCase(@"back\slash", ExpectedResult = @"'back\\slash'", TestName = "TheEscapeCharacterIsEscaped")]
		public string QuotingRules(string value)
		{
			return XamlUtils.QuoteMarkupExtensionValue(value);
		}

		/// <summary>
		/// Decompiles against this test assembly as the main module: the BAML built here refers
		/// only to well-known WPF types, which the decompiler resolves without it.
		/// </summary>
		static string Decompile(Stream baml)
		{
			var location = typeof(MarkupExtensionQuotingTests).Assembly.Location;
			using var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, fileStream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			var decompiler = new XamlDecompiler(new BamlDecompilerTypeSystem(file, resolver),
				new BamlDecompilerSettings());
			return decompiler.Decompile(baml).Xaml.ToString();
		}

		static MemoryStream CreateBaml(params BamlRecord[] records)
		{
			var version = new BamlDocument.BamlVersion { Major = 0, Minor = 0x60 };
			var document = new BamlDocument {
				Signature = "MSBAML",
				ReaderVersion = version,
				UpdaterVersion = version,
				WriterVersion = version
			};
			document.Add(new DocumentStartRecord());
			document.AddRange(records);
			document.Add(new DocumentEndRecord());

			var stream = new MemoryStream();
			BamlWriter.WriteDocument(document, stream);
			stream.Position = 0;
			return stream;
		}

		/// <summary>
		/// Builds "&lt;Button Content="{StaticResource &lt;value&gt;}" /&gt;", whose resource key is
		/// the single argument of the extension.
		/// </summary>
		static string DecompileExtensionArgument(string value)
		{
			return Decompile(CreateBaml(
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Button) },
				new StringInfoRecord { StringId = 0, Value = value },
				new PropertyWithExtensionRecord {
					AttributeId = MemberId(KnownMembers.Button_Content),
					Flags = (ushort)KnownTypes.StaticResourceExtension,
					ValueId = 0
				},
				new ElementEndRecord()));
		}

		[Test]
		public void AnArgumentCarryingACommaIsQuoted()
		{
			string xaml = DecompileExtensionArgument("ctor, arg = with comma");

			Assert.That(xaml, Does.Contain("{StaticResource 'ctor, arg = with comma'}"));
		}

		[Test]
		public void AnOrdinaryArgumentIsNotQuoted()
		{
			// Quoting everything would change every document ILSpy prints today.
			string xaml = DecompileExtensionArgument("MyResourceKey");

			Assert.That(xaml, Does.Contain("{StaticResource MyResourceKey}"));
		}
	}
}
