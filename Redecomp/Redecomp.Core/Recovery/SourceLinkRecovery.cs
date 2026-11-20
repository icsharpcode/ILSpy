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

using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace Redecomp.Core.Recovery;

/// <summary>
/// Downloads the original source files from the locations recorded in the SourceLink map of a PDB.
/// A file is kept only when its checksum equals the one the compiler stored in the PDB, possibly
/// after converting its line endings, so a recovered file is byte-for-byte the file that was compiled.
/// </summary>
public static class SourceLinkRecovery
{
	// Kind GUID of the "Source Link" custom debug information, from the Portable PDB specification.
	static readonly Guid SourceLinkKind = new("CC110556-A091-4D38-9FEC-25AB9A351A6A");
	static readonly Guid Sha1 = new("ff1816ec-aa5e-4d10-87f7-6f4963833460");
	static readonly Guid Sha256 = new("8829d00f-11b8-4213-878b-770e8597ac16");

	/// <summary>Returns the SourceLink map of the PDB, or null when it has none.</summary>
	public static SourceLinkMap? ReadMap(MetadataReader pdb)
	{
		foreach (var handle in pdb.GetCustomDebugInformation(EntityHandle.ModuleDefinition))
		{
			var cdi = pdb.GetCustomDebugInformation(handle);
			if (pdb.GetGuid(cdi.Kind) == SourceLinkKind)
				return SourceLinkMap.Parse(Encoding.UTF8.GetString(pdb.GetBlobBytes(cdi.Value)));
		}
		return null;
	}

	/// <summary>
	/// Downloads every document of the PDB that SourceLink covers, except files the SDK generated,
	/// and returns those that passed the checksum verification.
	/// </summary>
	public static IReadOnlyList<EmbeddedSourceFile> Download(MetadataReader pdb, ISourceDownloader downloader)
	{
		var map = ReadMap(pdb);
		if (map == null)
			return [];
		var files = new List<EmbeddedSourceFile>();
		foreach (var handle in pdb.Documents)
		{
			var document = pdb.GetDocument(handle);
			string path = pdb.GetString(document.Name);
			if (ProjectPaths.IsGenerated(path) || map.Map(path) is not Uri uri)
				continue;
			byte[]? content = downloader.TryDownload(uri);
			if (content != null && CompiledContent(pdb, document, content) is byte[] compiled)
				files.Add(new EmbeddedSourceFile(path, compiled));
		}
		return files;
	}

	/// <summary>
	/// Returns the bytes that were compiled: the downloaded content, or the same content with its
	/// line endings converted, whichever matches the PDB checksum; null when none matches. Git
	/// repositories store LF while Windows checkouts compile CRLF files (and the reverse can happen),
	/// so the conversion is needed for most files built on Windows; the checksum still guarantees
	/// that the result is exactly the compiled file.
	/// </summary>
	static byte[]? CompiledContent(MetadataReader pdb, Document document, byte[] downloaded)
	{
		if (MatchesChecksum(pdb, document, downloaded))
			return downloaded;
		foreach (byte[] candidate in new[] { ToCrLf(downloaded), ToLf(downloaded) })
		{
			if (!candidate.AsSpan().SequenceEqual(downloaded) && MatchesChecksum(pdb, document, candidate))
				return candidate;
		}
		return null;
	}

	static byte[] ToLf(byte[] content)
	{
		var result = new List<byte>(content.Length);
		for (int i = 0; i < content.Length; i++)
		{
			if (content[i] == (byte)'\r' && i + 1 < content.Length && content[i + 1] == (byte)'\n')
				continue;
			result.Add(content[i]);
		}
		return [.. result];
	}

	static byte[] ToCrLf(byte[] content)
	{
		byte[] lf = ToLf(content);
		var result = new List<byte>(lf.Length + lf.Length / 20);
		foreach (byte b in lf)
		{
			if (b == (byte)'\n')
				result.Add((byte)'\r');
			result.Add(b);
		}
		return [.. result];
	}

	static bool MatchesChecksum(MetadataReader pdb, Document document, byte[] content)
	{
		if (document.Hash.IsNil || document.HashAlgorithm.IsNil)
			return false;
		byte[] expected = pdb.GetBlobBytes(document.Hash);
		Guid algorithm = pdb.GetGuid(document.HashAlgorithm);
		byte[]? actual = algorithm == Sha256 ? SHA256.HashData(content)
			: algorithm == Sha1 ? SHA1.HashData(content)
			: null;
		return actual != null && actual.AsSpan().SequenceEqual(expected);
	}
}
