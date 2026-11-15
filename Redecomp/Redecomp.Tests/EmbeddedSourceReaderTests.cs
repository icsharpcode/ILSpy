// Copyright (c) 2026 Franck
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

using Redecomp.Core.Recovery;

using Redecomp.Bench;

namespace Redecomp.Tests;

public static class EmbeddedSourceReaderTests
{
	[Test]
	public static void RecoversEveryOriginalSourceFileByteForByte()
	{
		string assembly = SampleBuilder.Build("Facturation", BuildVariant.EmbeddedSources);
		var files = EmbeddedSourceReader.Read(assembly);
		string sourceDir = SampleBuilder.SourceDirectory("Facturation");
		foreach (string relative in new[] { "Program.cs", "Models/Facture.cs", "Services/CalculateurFacture.cs" })
		{
			var match = files.SingleOrDefault(f => f.OriginalPath.Replace('\\', '/').EndsWith("/" + relative));
			Assert.That(match != null, $"{relative} was not recovered");
			byte[] original = File.ReadAllBytes(Path.Combine(sourceDir, relative));
			Assert.That(original.AsSpan().SequenceEqual(match!.Content), $"{relative} differs from the original");
		}
	}

	[Test]
	public static void ReturnsNothingWhenNoSourcesAreEmbedded()
	{
		string noPdb = SampleBuilder.Build("Facturation", BuildVariant.NoPdb);
		string portable = SampleBuilder.Build("Facturation", BuildVariant.PortablePdbWithDocs);
		Assert.AreEqual(0, EmbeddedSourceReader.Read(noPdb).Count, "files from an assembly without PDB");
		Assert.AreEqual(0, EmbeddedSourceReader.Read(portable).Count, "files from a PDB without embedded sources");
	}
}
