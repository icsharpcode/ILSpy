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
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// Obfuscators put characters into BAML strings that XML cannot carry at all - not even as a
	/// numeric character reference. They have to be escaped before they reach the XDocument;
	/// otherwise writing the decompiled XAML throws and the whole resource is lost.
	/// </summary>
	[TestFixture]
	public class InvalidXmlCharacterTests
	{
		static ushort TypeId(KnownTypes type) => unchecked((ushort)-(short)type);

		static ushort MemberId(KnownMembers member) => unchecked((ushort)-(short)member);

		/// <summary>
		/// Builds a BAML stream out of <paramref name="records"/>, wrapped in the document
		/// start/end records and the header the reader insists on.
		/// </summary>
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
		/// Decompiles against this test assembly as the main module: the BAML built here refers
		/// only to well-known WPF types, which the decompiler resolves without it.
		/// </summary>
		static string Decompile(Stream baml)
		{
			var location = typeof(InvalidXmlCharacterTests).Assembly.Location;
			using var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, fileStream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			var decompiler = new XamlDecompiler(new BamlDecompilerTypeSystem(file, resolver),
				new BamlDecompilerSettings());
			return decompiler.Decompile(baml).Xaml.ToString();
		}

		[Test]
		public void ControlCharacterInPropertyValue_IsEscaped()
		{
			string xaml = Decompile(CreateBaml(
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Button) },
				new PropertyRecord {
					AttributeId = MemberId(KnownMembers.Button_Content),
					Value = "a\u0018b"
				},
				new ElementEndRecord()));

			Assert.That(xaml, Does.Contain(@"Content=""a\u0018b"""));
		}

		[Test]
		public void ControlCharacterInNamespaceUri_IsEscaped()
		{
			// The URI ends up both in the xmlns declaration and in the namespace of every element
			// name, so it cannot be repaired after the document has been built.
			string xaml = Decompile(CreateBaml(
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Button) },
				new XmlnsPropertyRecord {
					Prefix = "obf",
					XmlNamespace = "clr-namespace:Obfuscated\u0018Namespace",
					AssemblyIds = new ushort[0]
				},
				new ElementEndRecord()));

			Assert.That(xaml, Does.Contain(@"xmlns:obf=""clr-namespace:Obfuscated\u0018Namespace"""));
		}

		[Test]
		public void CharactersXmlCanCarry_AreLeftAlone()
		{
			// Tab, newline and astral characters are valid XML content; escaping them would
			// change the output of every ordinary document.
			string xaml = Decompile(CreateBaml(
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Button) },
				new PropertyRecord {
					AttributeId = MemberId(KnownMembers.Button_Content),
					Value = "tab\tastral\U0001F600"
				},
				new ElementEndRecord()));

			Assert.Multiple(() => {
				Assert.That(xaml, Does.Contain("\U0001F600"), "the surrogate pair stays intact");
				Assert.That(xaml, Does.Not.Contain("\\u"), "nothing valid gets escaped");
			});
		}
	}
}
