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

using System;
using System.IO;
using System.Reflection.PortableExecutable;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.BamlDecompiler.Baml;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// StartupUri is written in App.xaml but compiled into App.g.cs, not into the BAML: the markup
	/// compiler turns the attribute into an assignment inside InitializeComponent. The project
	/// exporter deletes the generated members, so without recovering the assignment the exported
	/// application builds and then opens no window (issue #2253).
	/// </summary>
	public class TestApplication
	{
		public Uri StartupUri { get; set; }
	}

	/// <summary>
	/// What the markup compiler generates for an Application with a StartupUri.
	/// </summary>
	public class AppWithStartupUri : TestApplication
	{
		public void InitializeComponent()
		{
			StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
			Uri resourceLocator = new Uri("/Demo;component/app.xaml", UriKind.Relative);
			GC.KeepAlive(resourceLocator);
		}
	}

	/// <summary>
	/// The same without one, which must stay without one.
	/// </summary>
	public class AppWithoutStartupUri : TestApplication
	{
		public void InitializeComponent()
		{
			Uri resourceLocator = new Uri("/Demo;component/app.xaml", UriKind.Relative);
			GC.KeepAlive(resourceLocator);
		}
	}

	[TestFixture]
	public class StartupUriTests
	{
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
			var location = typeof(StartupUriTests).Assembly.Location;
			using var fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
			var file = new PEFile(location, fileStream, streamOptions: PEStreamOptions.PrefetchEntireImage);
			var resolver = new UniversalAssemblyResolver(location, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			var decompiler = new XamlDecompiler(new BamlDecompilerTypeSystem(file, resolver),
				new BamlDecompilerSettings());
			return decompiler.Decompile(baml).Xaml.ToString();
		}

		/// <summary>
		/// A document whose root is <paramref name="typeName"/> of this assembly, which is what
		/// makes the BAML decompiler treat it as the code-behind class of the document.
		/// </summary>
		static string DecompileDocumentOf(string typeName)
		{
			return Decompile(CreateBaml(
				new AssemblyInfoRecord { AssemblyId = 0, AssemblyFullName = "ILSpy.BamlDecompiler.Tests" },
				new TypeInfoRecord { TypeId = 0, AssemblyId = 0, TypeFullName = "ILSpy.BamlDecompiler.Tests." + typeName },
				new ElementStartRecord { TypeId = 0 },
				new ElementEndRecord()));
		}

		[Test]
		public void TheStartupUriOfTheGeneratedCodeComesBackAsAnAttribute()
		{
			string xaml = DecompileDocumentOf(nameof(AppWithStartupUri));

			Assert.That(xaml, Does.Contain(@"StartupUri=""MainWindow.xaml"""), xaml);
		}

		[Test]
		public void NoStartupUriIsInventedForADocumentThatHasNone()
		{
			string xaml = DecompileDocumentOf(nameof(AppWithoutStartupUri));

			Assert.That(xaml, Does.Not.Contain("StartupUri=\""), xaml);
		}
	}
}
