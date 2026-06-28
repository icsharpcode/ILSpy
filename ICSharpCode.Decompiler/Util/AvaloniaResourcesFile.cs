#nullable enable
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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ICSharpCode.Decompiler.Util
{
	/// <summary>
	/// Reader for the <c>!AvaloniaResources</c> manifest resource that the Avalonia UI framework
	/// embeds in compiled assemblies. The blob packs every Avalonia resource (compiled XAML,
	/// images, ...) behind a single index, much like a <c>.resources</c> file packs a managed
	/// resource set.
	/// </summary>
	/// <remarks>
	/// Layout (all integers little-endian):
	/// <list type="bullet">
	/// <item><description>Int32 indexLength: byte length of the index section that follows.</description></item>
	/// <item><description>Index section: Int32 version (must be 2), Int32 entryCount, then per entry a
	/// 7-bit length-prefixed UTF-8 path, Int32 offset and Int32 size.</description></item>
	/// <item><description>Data section: concatenated file bytes; each entry's offset is relative to the
	/// start of this section (i.e. to <c>indexLength + 4</c>).</description></item>
	/// </list>
	/// </remarks>
	public sealed class AvaloniaResourcesFile : IEnumerable<KeyValuePair<string, byte[]>>
	{
		/// <summary>
		/// Name of the manifest resource that holds the packed Avalonia resources.
		/// </summary>
		public const string ResourceName = "!AvaloniaResources";

		// Index format version written by Avalonia's AvaloniaResourcesIndexReaderWriter. Version 1
		// was a legacy XML index that this reader does not support.
		const int BinaryCurrentVersion = 2;

		readonly List<KeyValuePair<string, byte[]>> entries = new();

		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="resourceName"/> is the manifest
		/// resource name Avalonia uses for its packed resources.
		/// </summary>
		public static bool IsAvaloniaResourcesEntry(string resourceName)
			=> resourceName == ResourceName;

		/// <summary>
		/// Parses the <c>!AvaloniaResources</c> blob read from <paramref name="stream"/>.
		/// </summary>
		/// <exception cref="BadImageFormatException">The blob is truncated, uses an unsupported
		/// index version, or contains an entry that points outside the data section.</exception>
		public AvaloniaResourcesFile(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			byte[] blob;
			using (var buffer = new MemoryStream())
			{
				stream.CopyTo(buffer);
				blob = buffer.ToArray();
			}

			try
			{
				Parse(blob);
			}
			catch (EndOfStreamException ex)
			{
				throw new BadImageFormatException("Truncated !AvaloniaResources index.", ex);
			}
		}

		void Parse(byte[] blob)
		{
			using var reader = new BinaryReader(new MemoryStream(blob), Encoding.UTF8);

			int indexLength = reader.ReadInt32();
			// The data section begins right after the 4-byte length prefix and the index.
			long baseOffset = 4L + indexLength;
			if (indexLength < 0 || baseOffset > blob.Length)
				throw new BadImageFormatException("Invalid !AvaloniaResources index length.");

			int version = reader.ReadInt32();
			if (version != BinaryCurrentVersion)
				throw new BadImageFormatException($"Unsupported !AvaloniaResources index version {version}.");

			int count = reader.ReadInt32();
			if (count < 0)
				throw new BadImageFormatException("Invalid !AvaloniaResources entry count.");

			// The count is attacker-controlled, so it must not be trusted as an allocation size: an
			// honest entry occupies at least a 1-byte path length prefix plus two Int32s, so the index
			// section cannot hold more than this many entries. A lying count is left to the per-entry
			// spill guard below instead of forcing a huge pre-allocation.
			const int MinEntrySize = 1 + 4 + 4;
			long maxEntries = (baseOffset - reader.BaseStream.Position) / MinEntrySize;
			entries.Capacity = (int)Math.Min(count, Math.Max(0, maxEntries));

			// The build task deduplicates identical files, so many entries can legitimately alias one
			// (offset, size) range in the data section; those share a single buffer here rather than
			// each copying the bytes. Distinct ranges in an honest index are non-overlapping and thus
			// cannot total more than the data section they occupy, so the sum of copied bytes is
			// bounded by the data section length. Rejecting anything larger stops a crafted index from
			// amplifying a small file into a huge allocation via overlapping, per-entry-copied slices.
			long dataSectionLength = blob.Length - baseOffset;
			long copiedBytes = 0;
			var buffersByRange = new Dictionary<long, byte[]>();
			for (int i = 0; i < count; i++)
			{
				string path = GetPathRooted(reader.ReadString());
				int offset = reader.ReadInt32();
				int size = reader.ReadInt32();

				// Every byte of every entry must come from the index section: an entry that runs
				// past the declared index end would otherwise silently consume data-section bytes.
				// This also rejects a count claiming more entries than the index holds.
				if (reader.BaseStream.Position > baseOffset)
					throw new BadImageFormatException("Malformed !AvaloniaResources index.");

				if (offset < 0 || size < 0 || baseOffset + offset + size > blob.Length)
					throw new BadImageFormatException($"!AvaloniaResources entry '{path}' points outside the data section.");

				long rangeKey = ((long)offset << 32) | (uint)size;
				if (!buffersByRange.TryGetValue(rangeKey, out var data))
				{
					copiedBytes += size;
					if (copiedBytes > dataSectionLength)
						throw new BadImageFormatException("!AvaloniaResources index references more data than the data section contains.");
					data = new byte[size];
					Buffer.BlockCopy(blob, (int)baseOffset + offset, data, 0, size);
					buffersByRange[rangeKey] = data;
				}
				entries.Add(new KeyValuePair<string, byte[]>(path, data));
			}
		}

		// Avalonia's AssetLoader keys resources by a path with a leading slash; a path stored
		// without one is rooted on read so the keys match what Avalonia itself resolves.
		static string GetPathRooted(string path)
			=> path.Length == 0 || path[0] != '/' ? "/" + path : path;

		public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator() => entries.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => entries.GetEnumerator();
	}
}
