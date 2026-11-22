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
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;

using ICSharpCode.ILSpyX.PdbProvider;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.PdbProvider;

[TestFixture]
public class DebugInfoUtilsTests
{
	[Test]
	public void IsWindowsPdbHandlesShortReads()
	{
		byte[] signature = Encoding.ASCII.GetBytes("Microsoft C/C++ MSF 7.00");
		using var stream = new ShortReadStream(signature);

		Assert.That(DebugInfoUtils.IsWindowsPdb(stream), Is.True);
		Assert.That(stream.Position, Is.EqualTo(signature.Length));
	}

	[Test]
	public void IsEmbeddedPortablePdbTooLargeThrowsWithoutEntireImage()
	{
		string assemblyPath = typeof(ICSharpCode.Decompiler.Metadata.MetadataFile).Assembly.Location;
		using var fullReader = new PEReader(File.OpenRead(assemblyPath));
		var entry = fullReader.ReadDebugDirectory()
			.Single(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
		using var metadataReader = new PEReader(
			File.OpenRead(assemblyPath), PEStreamOptions.PrefetchMetadata);

		Assert.Throws<NotSupportedException>(
			() => DebugInfoUtils.IsEmbeddedPortablePdbTooLarge(metadataReader, entry));
	}

	[Test]
	public void IsEmbeddedPortablePdbTooLargeUsesFilePointerWhenRvaDiffers()
	{
		string assemblyPath = typeof(ICSharpCode.Decompiler.Metadata.MetadataFile).Assembly.Location;
		byte[] image = File.ReadAllBytes(assemblyPath);
		int dataPointerFieldOffset;
		using (var reader = new PEReader(new MemoryStream(image)))
		{
			var entries = reader.ReadDebugDirectory();
			int embeddedEntryIndex = -1;
			for (int i = 0; i < entries.Length; i++)
			{
				if (entries[i].Type == DebugDirectoryEntryType.EmbeddedPortablePdb)
				{
					embeddedEntryIndex = i;
					break;
				}
			}
			Assert.That(embeddedEntryIndex, Is.GreaterThanOrEqualTo(0));

			int debugDirectoryRva = reader.PEHeaders.PEHeader!.DebugTableDirectory.RelativeVirtualAddress;
			var section = reader.PEHeaders.SectionHeaders.Single(s =>
				debugDirectoryRva >= s.VirtualAddress
				&& debugDirectoryRva - s.VirtualAddress < s.SizeOfRawData);
			int debugDirectoryOffset = section.PointerToRawData
				+ debugDirectoryRva - section.VirtualAddress;
			dataPointerFieldOffset = debugDirectoryOffset + embeddedEntryIndex * 28 + 24;
		}

		int appendedDataOffset = image.Length;
		Array.Resize(ref image, image.Length + 8);
		image[appendedDataOffset] = (byte)'M';
		image[appendedDataOffset + 1] = (byte)'P';
		image[appendedDataOffset + 2] = (byte)'D';
		image[appendedDataOffset + 3] = (byte)'B';
		BinaryPrimitives.WriteUInt32LittleEndian(
			image.AsSpan(appendedDataOffset + 4, 4),
			(uint)(DebugInfoUtils.MaxEmbeddedPortablePdbSize + 1));
		BinaryPrimitives.WriteInt32LittleEndian(
			image.AsSpan(dataPointerFieldOffset, 4), appendedDataOffset);

		using var patchedReader = new PEReader(new MemoryStream(image));
		var patchedEntry = patchedReader.ReadDebugDirectory()
			.Single(e => e.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);

		Assert.That(patchedEntry.DataPointer, Is.Not.EqualTo(patchedEntry.DataRelativeVirtualAddress));
		Assert.That(DebugInfoUtils.IsEmbeddedPortablePdbTooLarge(patchedReader, patchedEntry), Is.True);
	}

	sealed class ShortReadStream(byte[] data) : MemoryStream(data)
	{
		public override int Read(byte[] buffer, int offset, int count)
		{
			return base.Read(buffer, offset, Math.Min(count, 1));
		}
	}
}
