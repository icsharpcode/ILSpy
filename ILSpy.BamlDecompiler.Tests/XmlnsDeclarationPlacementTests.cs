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
	/// A document that already binds a prefix to a CLR namespace must keep using it. The document
	/// records the assembly the way it was written - "mscorlib" - while a well-known type carries
	/// the assembly it actually resolves to, which is the implementation assembly of whatever
	/// runtime ILSpy runs on. Comparing the two by name never matches, and every use of such a
	/// type then declared a second prefix for a namespace the root already had (issue #2253).
	/// </summary>
	[TestFixture]
	public class XmlnsDeclarationPlacementTests
	{
		const string MscorlibFullName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
		const string SystemNamespaceXmlns = "clr-namespace:System;assembly=mscorlib";

		static ushort TypeId(KnownTypes type) => unchecked((ushort)-(short)type);

		static ushort MemberId(KnownMembers member) => unchecked((ushort)-(short)member);

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

		static string Decompile(Stream baml)
		{
			var location = typeof(XmlnsDeclarationPlacementTests).Assembly.Location;
			using var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, fileStream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			var decompiler = new XamlDecompiler(new BamlDecompilerTypeSystem(file, resolver),
				new BamlDecompilerSettings());
			return decompiler.Decompile(baml).Xaml.ToString();
		}

		/// <summary>
		/// Builds a document whose root binds "sys" to the System namespace of mscorlib and then
		/// puts a System.String into the tree.
		/// </summary>
		static string DecompileDocumentUsingStringUnderAPrefixedRoot()
		{
			return Decompile(CreateBaml(
				new AssemblyInfoRecord { AssemblyId = 0, AssemblyFullName = MscorlibFullName },
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Button) },
				new XmlnsPropertyRecord {
					Prefix = "sys",
					XmlNamespace = SystemNamespaceXmlns,
					AssemblyIds = new ushort[] { 0 }
				},
				new PropertyComplexStartRecord { AttributeId = MemberId(KnownMembers.Button_Content) },
				new ElementStartRecord { TypeId = TypeId(KnownTypes.String) },
				new ElementEndRecord(),
				new PropertyComplexEndRecord(),
				new ElementEndRecord()));
		}

		[Test]
		public void ThePrefixTheDocumentDeclaredIsTheOneThatGetsUsed()
		{
			string xaml = DecompileDocumentUsingStringUnderAPrefixedRoot();

			Assert.That(xaml, Does.Contain("<sys:String"), xaml);
		}

		[Test]
		public void NoSecondPrefixIsDeclaredForANamespaceTheRootAlreadyBinds()
		{
			string xaml = DecompileDocumentUsingStringUnderAPrefixedRoot();

			Assert.Multiple(() => {
				Assert.That(xaml, Does.Not.Contain("xmlns:system="), xaml);
				Assert.That(xaml, Does.Not.Contain("System.Private.CoreLib"), xaml);
			});
		}
	}
}
