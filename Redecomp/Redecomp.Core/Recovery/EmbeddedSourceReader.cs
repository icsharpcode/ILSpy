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

using System.IO.Compression;
using System.Reflection.Metadata;

namespace Redecomp.Core.Recovery;

/// <summary>
/// Reads the source files that the compiler embedded in the portable PDB
/// (<c>EmbedAllSources</c> or <c>EmbeddedFiles</c>), either inside the assembly or next to it.
/// </summary>
public static class EmbeddedSourceReader
{
	// Kind GUID of the "Embedded Source" custom debug information, from the Portable PDB specification.
	static readonly Guid EmbeddedSourceKind = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");

	public static IReadOnlyList<EmbeddedSourceFile> Read(string assemblyPath)
	{
		using var provider = PortablePdbLocator.Open(assemblyPath);
		if (provider == null)
			return [];
		var pdb = provider.GetMetadataReader();
		var files = new List<EmbeddedSourceFile>();
		foreach (var documentHandle in pdb.Documents)
		{
			var document = pdb.GetDocument(documentHandle);
			foreach (var cdiHandle in pdb.GetCustomDebugInformation(documentHandle))
			{
				var cdi = pdb.GetCustomDebugInformation(cdiHandle);
				if (pdb.GetGuid(cdi.Kind) != EmbeddedSourceKind)
					continue;
				files.Add(new EmbeddedSourceFile(pdb.GetString(document.Name), DecodeBlob(pdb.GetBlobBytes(cdi.Value))));
			}
		}
		return files;
	}

	// The blob starts with an int32: 0 means the text follows uncompressed, otherwise it is the
	// uncompressed length and the text follows as a raw deflate stream.
	static byte[] DecodeBlob(byte[] blob)
	{
		int format = BitConverter.ToInt32(blob, 0);
		if (format == 0)
			return blob[4..];
		var result = new byte[format];
		using var deflate = new DeflateStream(new MemoryStream(blob, 4, blob.Length - 4), CompressionMode.Decompress);
		deflate.ReadExactly(result);
		return result;
	}
}
