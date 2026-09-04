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
	/// .NET ships a WindowsBase facade on every platform, so the assembly resolves everywhere - but
	/// it carries none of the types BAML means by it, because those live in the WindowsDesktop
	/// runtime pack. A well-known type that only exists in the real assembly then resolves to
	/// nothing, and the whole resource is lost: ten of the BAML entries in a DevExpress theme
	/// assembly are unreadable on a machine without WPF for exactly this reason.
	/// </summary>
	[TestFixture]
	public class FacadeAssemblyTests
	{
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
			var location = typeof(FacadeAssemblyTests).Assembly.Location;
			using var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, fileStream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			var decompiler = new XamlDecompiler(new BamlDecompilerTypeSystem(file, resolver),
				new BamlDecompilerSettings());
			return decompiler.Decompile(baml).Xaml.ToString();
		}

		[Test]
		public void ATypeOfTheRealWindowsBaseStillDecompiles()
		{
			// System.Windows.Size is a well-known BAML type of WindowsBase, and one the facade does
			// not have.
			string xaml = Decompile(CreateBaml(
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Button) },
				new PropertyComplexStartRecord { AttributeId = MemberId(KnownMembers.Button_Content) },
				new ElementStartRecord { TypeId = TypeId(KnownTypes.Size) },
				new ElementEndRecord(),
				new PropertyComplexEndRecord(),
				new ElementEndRecord()));

			Assert.That(xaml, Does.Contain("Size"), xaml);
		}
	}
}
