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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Util
{
	[TestFixture]
	public class AvaloniaResourcesFileTests
	{
		[Test]
		public void ReadsEntriesWithRootedPathsAndPayloads()
		{
			var blob = BuildBlob(
				("/App.axaml", Encoding.UTF8.GetBytes("<Application/>")),
				("Views/MainWindow.axaml", Encoding.UTF8.GetBytes("<Window/>")),
				("/assets/logo.png", new byte[] { 0x89, 0x50, 0x4E, 0x47 }));

			var entries = new AvaloniaResourcesFile(new MemoryStream(blob)).ToList();

			Assert.That(entries.Select(e => e.Key), Is.EqualTo(new[] {
				"/App.axaml",
				// A path without a leading slash is rooted, matching Avalonia's AssetLoader.
				"/Views/MainWindow.axaml",
				"/assets/logo.png",
			}));
			Assert.That(entries[0].Value, Is.EqualTo(Encoding.UTF8.GetBytes("<Application/>")));
			Assert.That(entries[1].Value, Is.EqualTo(Encoding.UTF8.GetBytes("<Window/>")));
			Assert.That(entries[2].Value, Is.EqualTo(new byte[] { 0x89, 0x50, 0x4E, 0x47 }));
		}

		[Test]
		public void EmptyResourceTableYieldsNoEntries()
		{
			var blob = BuildBlob();
			var entries = new AvaloniaResourcesFile(new MemoryStream(blob)).ToList();
			Assert.That(entries, Is.Empty);
		}

		[Test]
		public void SharedDataSectionIsReadPerEntry()
		{
			// The build task deduplicates identical files, so two paths can point at the same
			// (offset, size) range in the data section. Both must read the shared bytes.
			byte[] shared = Encoding.UTF8.GetBytes("dup");
			var data = new MemoryStream();
			data.Write(shared);
			var index = new List<(string Path, int Offset, int Size)> {
				("/a.txt", 0, shared.Length),
				("/b.txt", 0, shared.Length),
			};
			var blob = Assemble(index, data.ToArray());

			var entries = new AvaloniaResourcesFile(new MemoryStream(blob)).ToList();

			Assert.That(entries.Select(e => e.Key), Is.EqualTo(new[] { "/a.txt", "/b.txt" }));
			Assert.That(entries[0].Value, Is.EqualTo(shared));
			Assert.That(entries[1].Value, Is.EqualTo(shared));
		}

		[Test]
		public void AliasedEntriesShareOneBufferInsteadOfAmplifying()
		{
			// Many entries legitimately aliasing one deduplicated slice must not materialize a
			// separate copy each: N entries over one D-byte slice cost O(D), not O(N * D). A crafted
			// index exploits per-entry copies to amplify a small file into a huge allocation.
			const int slice = 1024 * 1024;
			byte[] data = new byte[slice];
			var index = Enumerable.Range(0, 500)
				.Select(i => ($"/f{i}", 0, slice))
				.ToList<(string, int, int)>();
			byte[] blob = Assemble(index, data);

			long before = GC.GetAllocatedBytesForCurrentThread();
			var entries = new AvaloniaResourcesFile(new MemoryStream(blob)).ToList();
			long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

			Assert.That(entries, Has.Count.EqualTo(500));
			// Per-entry copies would be 500 * 1 MB; sharing the single slice keeps it near one copy.
			Assert.That(allocated, Is.LessThan(16 * 1024 * 1024),
				$"parser allocated {allocated / (1024 * 1024)} MB materializing aliased entries");
		}

		[Test]
		public void OverlappingDataRangesAreRejected()
		{
			// Honest entries reference non-overlapping regions that cannot total more than the data
			// section. Distinct, overlapping ranges that sum past it are the amplification vector and
			// must be rejected, not copied.
			byte[] data = new byte[1000];
			var index = new List<(string Path, int Offset, int Size)> {
				("/a", 0, 1000),
				("/b", 0, 999), // distinct range overlapping the first; total 1999 > 1000
			};
			byte[] blob = Assemble(index, data);

			Assert.Throws<BadImageFormatException>(() => new AvaloniaResourcesFile(new MemoryStream(blob)).ToList());
		}

		[Test]
		public void UnsupportedVersionThrowsBadImageFormat()
		{
			var ms = new MemoryStream();
			using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				bw.Write(0); // index length placeholder
				bw.Write(1); // legacy XML index version, unsupported
				bw.Write(0); // entry count
			}
			PatchIndexLength(ms);

			Assert.Throws<BadImageFormatException>(() => new AvaloniaResourcesFile(ms).ToList());
		}

		[Test]
		public void HugeEntryCountDoesNotPreallocate()
		{
			// The entry count is attacker-controlled and must never drive the entry-array
			// allocation. A tiny crafted index that claims 100 million entries has to be rejected
			// as malformed without first reserving the ~1.6 GB such a count would otherwise pre-size.
			var ms = new MemoryStream();
			using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				bw.Write(0); // index length placeholder, patched below
				bw.Write(2); // BinaryCurrentVersion
				bw.Write(100_000_000); // absurd entry count with no entries to back it
			}
			PatchIndexLength(ms);
			byte[] blob = ms.ToArray();

			long before = GC.GetAllocatedBytesForCurrentThread();
			Assert.Throws<BadImageFormatException>(() => new AvaloniaResourcesFile(new MemoryStream(blob)).ToList());
			long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

			// A count-driven pre-allocation would be 100_000_000 * sizeof(reference pair); the parser
			// must instead bound the reservation to what the few-byte index can actually hold.
			Assert.That(allocated, Is.LessThan(16 * 1024 * 1024),
				$"parser allocated {allocated / (1024 * 1024)} MB for a {blob.Length}-byte index; the count field drove the allocation");
		}

		[Test]
		public void EntrySpillingPastIndexIntoDataSectionIsRejected()
		{
			// An entry whose path string starts inside the index but runs past the declared index
			// end would otherwise silently read its remaining bytes (and the offset/size fields
			// after it) out of the data section. The index boundary must hold for every byte of
			// every entry, not just at entry starts.
			var ms = new MemoryStream();
			using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				bw.Write(0); // index length, patched below
				bw.Write(2); // BinaryCurrentVersion
				bw.Write(1); // entry count
				bw.Write("/a-path-longer-than-the-declared-index");
				bw.Write(0); // offset
				bw.Write(0); // size
			}
			byte[] blob = ms.ToArray();
			// Declare the index to end in the middle of the path string; the bytes after that
			// point are data-section bytes that happen to parse as the rest of the entry.
			BitConverter.GetBytes(20).CopyTo(blob, 0);

			Assert.Throws<BadImageFormatException>(() => new AvaloniaResourcesFile(new MemoryStream(blob)).ToList());
		}

		[Test]
		public void HugeClaimedPathLengthIsRejectedWithoutHugeAllocation()
		{
			// The 7-bit length prefix of a path string is attacker-controlled; a prefix claiming a
			// ~256 MB path backed by only a few real bytes must fail as malformed without the
			// claimed length driving any allocation.
			var ms = new MemoryStream();
			using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				bw.Write(0); // index length, patched below
				bw.Write(2); // BinaryCurrentVersion
				bw.Write(1); // entry count
				ms.Write(new byte[] { 0xFF, 0xFF, 0xFF, 0x7F }); // 7-bit-encoded length 0x0FFFFFFF
				ms.Write(new byte[] { (byte)'/', (byte)'a' }); // far fewer bytes than claimed
			}
			PatchIndexLength(ms);
			byte[] blob = ms.ToArray();

			long before = GC.GetAllocatedBytesForCurrentThread();
			Assert.Throws<BadImageFormatException>(() => new AvaloniaResourcesFile(new MemoryStream(blob)).ToList());
			long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

			Assert.That(allocated, Is.LessThan(1024 * 1024),
				$"parser allocated {allocated} bytes for a {blob.Length}-byte blob; the claimed path length drove the allocation");
		}

		[Test]
		public void OutOfBoundsEntryThrowsBadImageFormat()
		{
			// A crafted index whose (offset, size) runs past the data section must be rejected,
			// not read out of bounds.
			var index = new List<(string Path, int Offset, int Size)> {
				("/evil", 0, 1024),
			};
			var blob = Assemble(index, new byte[] { 1, 2, 3 });

			Assert.Throws<BadImageFormatException>(() => new AvaloniaResourcesFile(new MemoryStream(blob)).ToList());
		}

		static byte[] BuildBlob(params (string Path, byte[] Data)[] files)
		{
			var data = new MemoryStream();
			var index = new List<(string Path, int Offset, int Size)>();
			foreach (var (path, bytes) in files)
			{
				index.Add((path, (int)data.Position, bytes.Length));
				data.Write(bytes);
			}
			return Assemble(index, data.ToArray());
		}

		static byte[] Assemble(List<(string Path, int Offset, int Size)> index, byte[] data)
		{
			var ms = new MemoryStream();
			using (var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				bw.Write(0); // index length placeholder, patched below
				bw.Write(2); // BinaryCurrentVersion
				bw.Write(index.Count);
				foreach (var (path, offset, size) in index)
				{
					bw.Write(path);
					bw.Write(offset);
					bw.Write(size);
				}
			}
			PatchIndexLength(ms);
			ms.Position = ms.Length;
			ms.Write(data);
			return ms.ToArray();
		}

		static void PatchIndexLength(MemoryStream ms)
		{
			int indexLength = (int)(ms.Length - 4);
			ms.Position = 0;
			using var bw = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
			bw.Write(indexLength);
		}
	}
}
