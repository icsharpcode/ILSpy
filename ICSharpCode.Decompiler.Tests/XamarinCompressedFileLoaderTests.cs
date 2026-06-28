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

using System.IO;
using System.Threading.Tasks;

using ICSharpCode.ILSpyX.FileLoaders;

using K4os.Compression.LZ4;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	public class XamarinCompressedFileLoaderTests
	{
		// Magic used for the Xamarin compressed module header ('XALZ', little-endian).
		const uint CompressedDataMagic = 0x5A4C4158;

		// Builds an XALZ blob: 4-byte magic, 4-byte descriptor index, 4-byte declared
		// uncompressed length, followed by the raw payload bytes.
		static MemoryStream BuildBlob(uint magic, uint declaredUncompressedLength, byte[] payload)
		{
			var ms = new MemoryStream();
			using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
			{
				writer.Write(magic);
				writer.Write((uint)0); // descriptor table index, unused by the loader
				writer.Write(declaredUncompressedLength);
				writer.Write(payload);
			}
			ms.Position = 0;
			return ms;
		}

		static Task<LoadResult?> Load(MemoryStream blob)
		{
			return new XamarinCompressedFileLoader().Load("test.dll", blob, new FileLoadContext(false, null));
		}

		[Test]
		public void NonXalzMagic_ReturnsNull()
		{
			// A stream that does not start with the XALZ magic is not ours; pass it through.
			using var blob = BuildBlob(0x12345678, 16, new byte[16]);

			Assert.That(Load(blob).GetAwaiter().GetResult(), Is.Null);
		}

		[Test]
		public void TooShortForMagic_ReturnsNull()
		{
			// A stream shorter than the 4-byte magic cannot be an XALZ module; pass it through
			// rather than letting the magic read throw EndOfStreamException.
			using var blob = new MemoryStream(new byte[] { 0x58, 0x41 });

			Assert.That(Load(blob).GetAwaiter().GetResult(), Is.Null);
		}

		[Test]
		public void TruncatedHeader_IsRejected()
		{
			// The magic is present but the stream ends before the full 12-byte header. This is a
			// corrupt module and must fail as a catchable InvalidDataException, not an
			// EndOfStreamException from a partial header read.
			var ms = new MemoryStream();
			using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
			{
				writer.Write(CompressedDataMagic);
				writer.Write((uint)0); // only 8 of the required 12 header bytes
			}
			ms.Position = 0;

			Assert.ThrowsAsync<InvalidDataException>(() => Load(ms));
			ms.Dispose();
		}

		[Test]
		public void NegativeDeclaredLength_DoesNotThrowArgumentOutOfRange()
		{
			// A declared length with the high bit set would become negative when cast to int and
			// make ArrayPool.Rent throw ArgumentOutOfRangeException. It must be rejected as
			// malformed input (a catchable InvalidDataException) before any allocation.
			using var blob = BuildBlob(CompressedDataMagic, 0xFFFFFFFF, new byte[] { 1, 2, 3, 4 });

			Assert.ThrowsAsync<InvalidDataException>(() => Load(blob));
		}

		[Test]
		public void ImplausiblyHugeDeclaredLength_IsRejectedWithoutAllocating()
		{
			// A tiny payload declaring a ~2 GB decompressed size is a decompression-bomb header:
			// no LZ4 block that small can expand that far. Reject it instead of renting ~2 GB.
			using var blob = BuildBlob(CompressedDataMagic, 0x7FFFFFFF, new byte[] { 1, 2, 3, 4 });

			Assert.ThrowsAsync<InvalidDataException>(() => Load(blob));
		}

		[Test]
		public void CorruptPayload_IsRejected()
		{
			// Garbage that is not a valid LZ4 block, with an otherwise plausible declared length,
			// must surface as a catchable InvalidDataException rather than producing a buffer of
			// stale/partial bytes.
			byte[] garbage = new byte[64];
			for (int i = 0; i < garbage.Length; i++)
				garbage[i] = (byte)(i * 7 + 1);
			using var blob = BuildBlob(CompressedDataMagic, 4096, garbage);

			Assert.ThrowsAsync<InvalidDataException>(() => Load(blob));
		}

		[Test]
		public void ValidXalz_LoadsDecompressedAssembly()
		{
			// A genuine XALZ wrapper around a real, LZ4-compressed assembly must still load. This
			// also exercises the decoded-length slice: ArrayPool.Rent may hand back a buffer larger
			// than the decompressed data, and the PE parser must see only the real bytes.
			byte[] original = File.ReadAllBytes(typeof(XamarinCompressedFileLoader).Assembly.Location);
			byte[] compressed = new byte[LZ4Codec.MaximumOutputSize(original.Length)];
			int compressedLength = LZ4Codec.Encode(original, 0, original.Length, compressed, 0, compressed.Length);
			byte[] payload = new byte[compressedLength];
			System.Array.Copy(compressed, payload, compressedLength);

			using var blob = BuildBlob(CompressedDataMagic, (uint)original.Length, payload);

			var result = Load(blob).GetAwaiter().GetResult();

			Assert.That(result, Is.Not.Null);
			Assert.That(result!.IsSuccess, Is.True);
			Assert.That(result.MetadataFile, Is.Not.Null);
		}
	}
}
