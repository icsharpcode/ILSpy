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
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace ICSharpCode.ILSpyX.Symbols
{
	/// <summary>
	/// Finds the PDB of a PE file along a <see cref="SymbolPath"/>, downloading it from HTTP symbol
	/// servers into the element's cache directory. Every candidate is verified against the key
	/// before it is returned, so a stale or mismatching file is never handed to the caller.
	/// Results (hits and misses) are memoized per key for the lifetime of the locator.
	/// </summary>
	public sealed class SymbolLocator
	{
		static readonly Lazy<HttpClient> sharedClient = new(() => new HttpClient(
			new HttpClientHandler { UseProxy = true, UseDefaultCredentials = true }) {
			Timeout = System.Threading.Timeout.InfiniteTimeSpan
		});

		const string LegacyPdbPrefix = "Microsoft C/C++ MSF 7.00";

		readonly HttpClient httpClient;
		readonly ConcurrentDictionary<string, Task<string?>> lookups = new(StringComparer.OrdinalIgnoreCase);

		public SymbolPath SymbolPath { get; }

		/// <summary>Per-request timeout for HTTP downloads.</summary>
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

		/// <summary>
		/// When <c>true</c>, assemblies that are opened explicitly look up missing PDBs on the symbol
		/// path while they load. The locator itself does not read this; it is a policy for hosts.
		/// </summary>
		public bool AutoDownload { get; set; }

		/// <summary>Raised for every location probed; useful for logging.</summary>
		public event Action<string>? Probing;

		public SymbolLocator(SymbolPath symbolPath, HttpClient? httpClient = null)
		{
			SymbolPath = symbolPath ?? throw new ArgumentNullException(nameof(symbolPath));
			this.httpClient = httpClient ?? sharedClient.Value;
		}

		/// <summary>
		/// Returns the local path of the first PDB that matches one of the module's CodeView entries,
		/// or <c>null</c> when none is found.
		/// </summary>
		public async Task<string?> FindPdbAsync(PEReader reader, CancellationToken cancellationToken = default)
		{
			foreach (var key in SymbolKey.GetPdbKeys(reader))
			{
				var path = await FindFileAsync(key, cancellationToken).ConfigureAwait(false);
				if (path != null)
					return path;
			}
			return null;
		}

		/// <summary>Forgets memoized results so the next lookup probes the symbol path again.</summary>
		public void ClearCache() => lookups.Clear();

		public Task<string?> FindFileAsync(SymbolKey key, CancellationToken cancellationToken = default)
		{
			var task = lookups.GetOrAdd(key.Key, _ => FindFileCoreAsync(key, cancellationToken));
			if (task.IsCanceled || task.IsFaulted)
			{
				// A cancelled lookup must not poison later ones.
				lookups.TryRemove(key.Key, out _);
				task = lookups.GetOrAdd(key.Key, _ => FindFileCoreAsync(key, cancellationToken));
			}
			return task;
		}

		async Task<string?> FindFileCoreAsync(SymbolKey key, CancellationToken cancellationToken)
		{
			foreach (var element in SymbolPath.Elements)
			{
				cancellationToken.ThrowIfCancellationRequested();
				string? path = element.IsHttp
					? await FindOnServerAsync(element, key, cancellationToken).ConfigureAwait(false)
					: FindInDirectory(element.Location, key);
				if (path != null)
					return path;
			}
			return null;
		}

		string? FindInDirectory(string directory, SymbolKey key)
		{
			foreach (var candidate in new[] { CombineKey(directory, key.Key), Path.Combine(directory, key.FileName) })
			{
				Probing?.Invoke(candidate);
				if (File.Exists(candidate) && Matches(candidate, key))
					return candidate;
			}
			return null;
		}

		async Task<string?> FindOnServerAsync(SymbolPathElement element, SymbolKey key, CancellationToken cancellationToken)
		{
			string cachePath = CombineKey(element.CacheDirectory!, key.Key);
			if (File.Exists(cachePath))
			{
				if (Matches(cachePath, key))
					return cachePath;
				TryDelete(cachePath);
			}

			string url = element.Location.TrimEnd('/') + "/" + key.Key;
			Probing?.Invoke(url);
			using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			timeout.CancelAfter(Timeout);
			string tempPath = cachePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
			try
			{
				using var request = new HttpRequestMessage(HttpMethod.Get, url);
				if (key.ChecksumHeader != null)
					request.Headers.TryAddWithoutValidation("SymbolChecksum", key.ChecksumHeader);
				using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token)
					.ConfigureAwait(false);
				if (response.StatusCode != HttpStatusCode.OK)
					return null;
				Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
				using (var target = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write))
				{
					await response.Content.CopyToAsync(target, timeout.Token).ConfigureAwait(false);
				}
				if (!Matches(tempPath, key))
					return null;
				File.Move(tempPath, cachePath, overwrite: true);
				return cachePath;
			}
			catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
			{
				// Per-request timeout: treat the server as not having the file.
				return null;
			}
			catch (Exception ex) when (ex is HttpRequestException || ex is IOException || ex is UnauthorizedAccessException)
			{
				return null;
			}
			finally
			{
				TryDelete(tempPath);
			}
		}

		static string CombineKey(string directory, string key)
			=> Path.Combine(directory, key.Replace('/', Path.DirectorySeparatorChar));

		static void TryDelete(string path)
		{
			try
			{
				if (File.Exists(path))
					File.Delete(path);
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		/// <summary>
		/// Checks that the file at <paramref name="path"/> is the one <paramref name="key"/> names:
		/// the PDB id for portable PDBs, the format signature for Windows PDBs, and the time stamp and
		/// image size for PE files.
		/// </summary>
		public static bool Matches(string path, SymbolKey key)
		{
			try
			{
				using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
				switch (key.Kind)
				{
					case SymbolFileKind.PortablePdb:
					{
						using var provider = MetadataReaderProvider.FromPortablePdbStream(stream, MetadataStreamOptions.LeaveOpen);
						var id = provider.GetMetadataReader().DebugMetadataHeader?.Id;
						if (id == null || id.Value.Length < 20)
							return false;
						var bytes = id.Value;
						var guid = new Guid(bytes.AsSpan(0, 16));
						uint stamp = BitConverter.ToUInt32(bytes.AsSpan(16, 4));
						return guid == key.Guid && stamp == key.AgeOrStamp;
					}
					case SymbolFileKind.WindowsPdb:
					{
						var buffer = new byte[LegacyPdbPrefix.Length];
						return stream.Read(buffer, 0, buffer.Length) == buffer.Length
							&& Encoding.ASCII.GetString(buffer) == LegacyPdbPrefix;
					}
					case SymbolFileKind.PE:
					{
						using var reader = new PEReader(stream, PEStreamOptions.LeaveOpen);
						return SymbolKey.GetPEKey(reader, key.FileName)?.Key == key.Key;
					}
					default:
						return false;
				}
			}
			catch (Exception ex) when (ex is BadImageFormatException || ex is IOException || ex is UnauthorizedAccessException)
			{
				return false;
			}
		}
	}
}
