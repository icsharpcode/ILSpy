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
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public sealed class PackageSidecarTests
{
	[Test]
	public void PackageFolder_FindsXmlAndPdbSidecarsInTheSameFolder()
	{
		var package = new LoadedPackage(LoadedPackage.PackageKind.Zip, new[] {
			new TestEntry("lib/net8.0/Foo.dll", Array.Empty<byte>()),
			new TestEntry("lib/net8.0/Foo.xml", Encoding.UTF8.GetBytes("<doc>\n  <members>\n    <member name=\"T:Foo\"><summary>package docs</summary></member>\n  </members>\n</doc>\n")),
			new TestEntry("lib/net8.0/Foo.pdb", new byte[] { 1, 2, 3 }),
			new TestEntry("other/Foo.xml", Encoding.UTF8.GetBytes("wrong folder")),
		});

		var folder = package.RootFolder.Folders.Single(folder => folder.Name == "lib").Folders.Single(folder => folder.Name == "net8.0");
		var assembly = folder.Entries.Single(entry => entry.Name == "Foo.dll");
		var xml = folder.FindAssociatedEntry(assembly.Name, ".xml");
		var pdb = folder.FindAssociatedEntry(assembly.Name, ".pdb");
		var xmlFromPackagePath = folder.FindAssociatedEntry("lib/net8.0/Foo.dll", ".xml");

		Assert.That(xml, Is.Not.Null);
		Assert.That(pdb, Is.Not.Null);
		Assert.That(xmlFromPackagePath, Is.SameAs(xml));
		var provider = new XmlDocumentationProvider(xml!.FullName, _ => xml.TryOpenStream());
		Assert.That(provider.GetDocumentation("T:Foo"), Does.Contain("package docs"));
		Assert.That(pdb!.TryOpenStream()!.Length, Is.EqualTo(3));
	}

	sealed class TestEntry : PackageEntry
	{
		readonly byte[] data;

		public TestEntry(string name, byte[] data)
		{
			Name = name;
			this.data = data;
		}

		public override string Name { get; }
		public override string FullName => Name;
		public override string PackageQualifiedFileName => "test://" + Name;
		public override Stream TryOpenStream() => new MemoryStream(data, writable: false);
		public override long? TryGetLength() => data.Length;
	}
}
