// Copyright (c) 2026 Piero Viano
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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.ILSpyX.PdbProvider;

#nullable enable

namespace ICSharpCode.ILSpyX.Symbols
{
	public enum OriginalSourceOrigin
	{
		/// <summary>The source text is embedded in the PDB.</summary>
		Embedded,
		/// <summary>The source text was downloaded through the PDB's Source Link map.</summary>
		SourceLink,
	}

	/// <param name="ChecksumMatches"><c>true</c> if the text hashes to the checksum recorded in the PDB;
	/// <c>false</c> on a mismatch (commonly a line-ending difference); <c>null</c> when the PDB records
	/// no checksum with a known algorithm.</param>
	public sealed record OriginalSource(string DocumentPath, string Text, OriginalSourceOrigin Origin,
		string? Uri, bool? ChecksumMatches);

	/// <summary>
	/// Retrieves the original source documents recorded in a portable PDB: the embedded source when
	/// present, otherwise the file the Source Link map points at.
	/// </summary>
	public sealed class OriginalSourceProvider
	{
		static readonly Guid EmbeddedSourceKind = new("0E8A571B-6926-466E-B4AD-8AB04611F5FE");
		static readonly Guid SourceLinkKind = new("CC110556-A091-4D38-9FEC-25AB9A351A6A");
		static readonly Guid Sha1Algorithm = new("ff1816ec-aa5e-4d10-87f7-6f4963833460");
		static readonly Guid Sha256Algorithm = new("8829d00f-11b8-4213-878b-770e8597ac16");

		static readonly Lazy<HttpClient> sharedClient = new(() => new HttpClient(
			new HttpClientHandler { UseProxy = true, UseDefaultCredentials = true }) {
			Timeout = TimeSpan.FromSeconds(30)
		});

		readonly MetadataReader pdb;
		readonly HttpClient httpClient;
		readonly string? cacheDirectory;

		/// <summary>The PDB's Source Link map, or <c>null</c> if it has none.</summary>
		public SourceLinkMap? SourceLink { get; }

		/// <param name="cacheDirectory">Where downloaded files whose checksum matches are kept;
		/// <c>null</c> disables caching.</param>
		public OriginalSourceProvider(MetadataReader pdb, HttpClient? httpClient = null, string? cacheDirectory = null)
		{
			this.pdb = pdb ?? throw new ArgumentNullException(nameof(pdb));
			this.httpClient = httpClient ?? sharedClient.Value;
			this.cacheDirectory = cacheDirectory;
			foreach (var handle in pdb.GetCustomDebugInformation(EntityHandle.ModuleDefinition))
			{
				var cdi = pdb.GetCustomDebugInformation(handle);
				if (pdb.GetGuid(cdi.Kind) == SourceLinkKind)
				{
					SourceLink = SourceLinkMap.Parse(Encoding.UTF8.GetString(pdb.GetBlobBytes(cdi.Value)));
					break;
				}
			}
		}

		/// <summary>
		/// Creates a provider for <paramref name="debugInfo"/> when it is a portable PDB (file or
		/// embedded); Windows PDBs carry no Source Link map.
		/// </summary>
		public static OriginalSourceProvider? TryCreate(IDebugInfoProvider? debugInfo, HttpClient? httpClient = null,
			string? cacheDirectory = null)
		{
			if (debugInfo is not PortableDebugInfoProvider portable)
				return null;
			var reader = portable.GetMetadataReader();
			return reader == null ? null : new OriginalSourceProvider(reader, httpClient, cacheDirectory);
		}

		public IEnumerable<DocumentHandle> AllDocuments => pdb.Documents;

		/// <summary>The distinct documents the given methods' sequence points refer to, in first-use order.</summary>
		public IReadOnlyList<DocumentHandle> GetDocuments(IEnumerable<MethodDefinitionHandle> methods)
		{
			var result = new List<DocumentHandle>();
			var seen = new HashSet<DocumentHandle>();
			foreach (var method in methods)
			{
				var info = pdb.GetMethodDebugInformation(method.ToDebugInformationHandle());
				if (!info.Document.IsNil)
				{
					if (seen.Add(info.Document))
						result.Add(info.Document);
					continue;
				}
				if (info.SequencePointsBlob.IsNil)
					continue;
				foreach (var sp in info.GetSequencePoints())
				{
					if (!sp.Document.IsNil && seen.Add(sp.Document))
						result.Add(sp.Document);
				}
			}
			return result;
		}

		public string GetDocumentPath(DocumentHandle document)
			=> pdb.GetString(pdb.GetDocument(document).Name);

		/// <summary>The embedded text of the document, or <c>null</c> if it is not embedded.</summary>
		public string? GetEmbeddedSource(DocumentHandle document)
		{
			foreach (var handle in pdb.GetCustomDebugInformation(document))
			{
				var cdi = pdb.GetCustomDebugInformation(handle);
				if (pdb.GetGuid(cdi.Kind) != EmbeddedSourceKind)
					continue;
				var blob = pdb.GetBlobReader(cdi.Value);
				int format = blob.ReadInt32();
				byte[] bytes = blob.ReadBytes(blob.RemainingBytes);
				if (format > 0)
				{
					var uncompressed = new byte[format];
					using var deflate = new DeflateStream(new MemoryStream(bytes), CompressionMode.Decompress);
					deflate.ReadExactly(uncompressed);
					bytes = uncompressed;
				}
				return Decode(bytes);
			}
			return null;
		}

		/// <summary>The Source Link URL of the document, or <c>null</c>.</summary>
		public string? GetSourceLinkUri(DocumentHandle document)
			=> SourceLink?.GetUri(GetDocumentPath(document));

		/// <summary>
		/// Returns the original source of the document: embedded text first, then the Source Link
		/// download. Returns <c>null</c> when neither is available or the download fails.
		/// </summary>
		public async Task<OriginalSource?> GetSourceAsync(DocumentHandle document, CancellationToken cancellationToken = default)
		{
			string path = GetDocumentPath(document);
			string? embedded = GetEmbeddedSource(document);
			if (embedded != null)
				return new OriginalSource(path, embedded, OriginalSourceOrigin.Embedded, null, null);

			string? uri = GetSourceLinkUri(document);
			if (uri == null)
				return null;
			var (algorithm, expectedHash) = GetChecksum(document);
			string? cachePath = cacheDirectory != null && expectedHash != null
				? Path.Combine(cacheDirectory, Convert.ToHexString(expectedHash).ToLowerInvariant(), SymbolKey.GetFileName(path))
				: null;
			if (cachePath != null && File.Exists(cachePath))
			{
				var cached = await File.ReadAllBytesAsync(cachePath, cancellationToken).ConfigureAwait(false);
				if (HashMatches(cached, algorithm, expectedHash) == true)
					return new OriginalSource(path, Decode(cached), OriginalSourceOrigin.SourceLink, uri, true);
			}
			byte[] bytes;
			try
			{
				using var response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
				if (response.StatusCode != HttpStatusCode.OK)
					return null;
				bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
			}
			catch (HttpRequestException)
			{
				return null;
			}
			catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
			{
				return null;
			}
			bool? matches = HashMatches(bytes, algorithm, expectedHash);
			if (matches == true && cachePath != null)
			{
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
					await File.WriteAllBytesAsync(cachePath, bytes, cancellationToken).ConfigureAwait(false);
				}
				catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
				{
				}
			}
			return new OriginalSource(path, Decode(bytes), OriginalSourceOrigin.SourceLink, uri, matches);
		}

		(Guid Algorithm, byte[]? Hash) GetChecksum(DocumentHandle document)
		{
			var doc = pdb.GetDocument(document);
			if (doc.HashAlgorithm.IsNil || doc.Hash.IsNil)
				return (Guid.Empty, null);
			return (pdb.GetGuid(doc.HashAlgorithm), pdb.GetBlobBytes(doc.Hash));
		}

		static bool? HashMatches(byte[] bytes, Guid algorithm, byte[]? expected)
		{
			if (expected == null)
				return null;
			byte[] actual;
			if (algorithm == Sha256Algorithm)
				actual = SHA256.HashData(bytes);
			else if (algorithm == Sha1Algorithm)
				actual = SHA1.HashData(bytes);
			else
				return null;
			return actual.AsSpan().SequenceEqual(expected);
		}

		static string Decode(byte[] bytes)
		{
			using var reader = new StreamReader(new MemoryStream(bytes), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
			return reader.ReadToEnd();
		}
	}
}
