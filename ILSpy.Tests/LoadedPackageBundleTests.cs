// Copyright (c) 2026 Christoph Wille
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
using System.Linq;
using System.Text;

using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// The sizes in a single-file bundle manifest are attacker-controlled. These tests pin down
/// that opening a compressed entry never allocates based on the declared size and stops
/// inflating as soon as the data exceeds it, so a small bundle cannot become a multi-gigabyte
/// allocation.
/// </summary>
[TestFixture]
public class LoadedPackageBundleTests
{
	// The 32-byte bundle signature (SHA-256 of ".net core bundle"), as in SingleFileBundle.IsBundle.
	static readonly byte[] Signature = new byte[] {
		0x8b, 0x12, 0x02, 0xb9, 0x6a, 0x61, 0x20, 0x38,
		0x72, 0x7b, 0x93, 0x02, 0x14, 0xd7, 0xa0, 0x32,
		0x13, 0xf5, 0xb9, 0xe6, 0xef, 0xae, 0x33, 0x18,
		0xee, 0x3b, 0x2d, 0xce, 0x24, 0xb3, 0x6a, 0xae
	};

	readonly List<string> tempFiles = new();

	[TearDown]
	public void DeleteTempFiles()
	{
		// The memory-mapped view stays alive as long as the bundle entries do, so the file
		// can only be deleted once those have been collected.
		GC.Collect();
		GC.WaitForPendingFinalizers();
		foreach (var file in tempFiles)
		{
			try
			{
				File.Delete(file);
			}
			catch (IOException)
			{
				// best effort; the temp directory is cleaned up by the OS eventually
			}
		}
	}

	[Test]
	public void CompressedEntry_WithMatchingDeclaredSize_RoundTrips()
	{
		byte[] content = Enumerable.Range(0, 10_000).Select(i => (byte)(i * 7)).ToArray();
		var bundle = WriteBundle(new BundleFile("lib.dll", Deflate(content), content.Length));

		var package = LoadedPackage.FromBundle(bundle);

		Assert.That(package, Is.Not.Null);
		var entry = package!.Entries.Single();
		Assert.That(entry.TryGetLength(), Is.EqualTo(content.Length));
		using var stream = entry.TryOpenStream();
		Assert.That(ReadAll(stream!), Is.EqualTo(content));
	}

	[Test]
	public void UncompressedEntry_RoundTrips()
	{
		byte[] content = Encoding.ASCII.GetBytes("plain content");
		var bundle = WriteBundle(new BundleFile("plain.dll", content, content.Length, Compressed: false));

		var package = LoadedPackage.FromBundle(bundle);

		using var stream = package!.Entries.Single().TryOpenStream();
		Assert.That(ReadAll(stream!), Is.EqualTo(content));
	}

	[Test]
	public void CompressedEntry_DeclaredSizeBeyondInt32_IsRejectedAsInvalidData()
	{
		// A declared size that does not fit an int used to be truncated to a negative
		// MemoryStream capacity; any size that cannot be held in memory must be reported as
		// corrupt bundle data instead.
		byte[] content = new byte[100];
		var bundle = WriteBundle(new BundleFile("huge.dll", Deflate(content), DeclaredSize: 3L * 1024 * 1024 * 1024));

		var entry = LoadedPackage.FromBundle(bundle)!.Entries.Single();

		Assert.Throws<InvalidDataException>(() => entry.TryOpenStream());
	}

	[Test]
	public void CompressedEntry_InflatingBeyondDeclaredSize_StopsEarly()
	{
		// A decompression bomb: a few kilobytes that inflate to 32 MB, declared as 16 bytes.
		// Decompression must stop as soon as the declared size is exceeded rather than
		// inflating everything first and comparing lengths afterwards, so the memory the
		// attacker can force is bounded by what the manifest declares, not by the bomb.
		const int inflatedSize = 32 * 1024 * 1024;
		var bundle = WriteBundle(new BundleFile("bomb.dll", Deflate(new byte[inflatedSize]), DeclaredSize: 16));
		var entry = LoadedPackage.FromBundle(bundle)!.Entries.Single();

		long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
		Assert.Throws<InvalidDataException>(() => entry.TryOpenStream());
		long allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

		Assert.That(allocated, Is.LessThan(4 * 1024 * 1024), "opening the entry inflated far past its declared size");
	}

	sealed record BundleFile(string RelativePath, byte[] StoredBytes, long DeclaredSize, bool Compressed = true);

	static byte[] Deflate(byte[] content)
	{
		var ms = new MemoryStream();
		using (var deflate = new DeflateStream(ms, CompressionLevel.Optimal, leaveOpen: true))
		{
			deflate.Write(content);
		}
		return ms.ToArray();
	}

	static byte[] ReadAll(Stream stream)
	{
		var ms = new MemoryStream();
		stream.CopyTo(ms);
		return ms.ToArray();
	}

	/// <summary>
	/// Writes a minimal version-6 single-file bundle: the stored file bytes, the manifest,
	/// the 8-byte manifest offset and the bundle signature, in that order.
	/// </summary>
	string WriteBundle(params BundleFile[] files)
	{
		var ms = new MemoryStream();
		var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);
		var offsets = new long[files.Length];
		for (int i = 0; i < files.Length; i++)
		{
			offsets[i] = ms.Position;
			writer.Write(files[i].StoredBytes);
		}

		long manifestOffset = ms.Position;
		writer.Write(6u); // MajorVersion (v6 adds the compressed size field)
		writer.Write(0u); // MinorVersion
		writer.Write(files.Length); // FileCount
		writer.Write("test-bundle"); // BundleID
		writer.Write(0L); // DepsJsonOffset
		writer.Write(0L); // DepsJsonSize
		writer.Write(0L); // RuntimeConfigJsonOffset
		writer.Write(0L); // RuntimeConfigJsonSize
		writer.Write(0UL); // Flags
		for (int i = 0; i < files.Length; i++)
		{
			writer.Write(offsets[i]); // Offset
			writer.Write(files[i].DeclaredSize); // Size
			writer.Write(files[i].Compressed ? files[i].StoredBytes.Length : 0L); // CompressedSize
			writer.Write((byte)1); // FileType.Assembly
			writer.Write(files[i].RelativePath);
		}

		writer.Write(manifestOffset);
		writer.Write(Signature);
		writer.Flush();

		string path = Path.Combine(Path.GetTempPath(), "ILSpyBundleTest-" + Guid.NewGuid().ToString("N") + ".exe");
		File.WriteAllBytes(path, ms.ToArray());
		tempFiles.Add(path);
		return path;
	}
}
